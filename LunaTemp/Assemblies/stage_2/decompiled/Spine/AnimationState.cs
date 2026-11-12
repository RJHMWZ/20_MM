using System;
using System.Collections.Generic;
using System.Text;

namespace Spine
{
	public class AnimationState
	{
		public delegate void TrackEntryDelegate(TrackEntry trackEntry);

		public delegate void TrackEntryEventDelegate(TrackEntry trackEntry, Event e);

		private static readonly Animation EmptyAnimation = new Animation("<empty>", new ExposedList<Timeline>(), 0f);

		internal const int Subsequent = 0;

		internal const int First = 1;

		internal const int HoldSubsequent = 2;

		internal const int HoldFirst = 3;

		internal const int HoldMix = 4;

		internal const int Setup = 1;

		internal const int Current = 2;

		protected AnimationStateData data;

		private readonly ExposedList<TrackEntry> tracks = new ExposedList<TrackEntry>();

		private readonly ExposedList<Event> events = new ExposedList<Event>();

		private readonly EventQueue queue;

		private readonly HashSet<int> propertyIDs = new HashSet<int>();

		private bool animationsChanged;

		private float timeScale = 1f;

		private int unkeyedState;

		private readonly Pool<TrackEntry> trackEntryPool = new Pool<TrackEntry>();

		public float TimeScale
		{
			get
			{
				return timeScale;
			}
			set
			{
				timeScale = value;
			}
		}

		public AnimationStateData Data
		{
			get
			{
				return data;
			}
			set
			{
				if (data == null)
				{
					throw new ArgumentNullException("data", "data cannot be null.");
				}
				data = value;
			}
		}

		public ExposedList<TrackEntry> Tracks => tracks;

		public event TrackEntryDelegate Start;

		public event TrackEntryDelegate Interrupt;

		public event TrackEntryDelegate End;

		public event TrackEntryDelegate Dispose;

		public event TrackEntryDelegate Complete;

		public event TrackEntryEventDelegate Event;

		internal void OnStart(TrackEntry entry)
		{
			if (this.Start != null)
			{
				this.Start(entry);
			}
		}

		internal void OnInterrupt(TrackEntry entry)
		{
			if (this.Interrupt != null)
			{
				this.Interrupt(entry);
			}
		}

		internal void OnEnd(TrackEntry entry)
		{
			if (this.End != null)
			{
				this.End(entry);
			}
		}

		internal void OnDispose(TrackEntry entry)
		{
			if (this.Dispose != null)
			{
				this.Dispose(entry);
			}
		}

		internal void OnComplete(TrackEntry entry)
		{
			if (this.Complete != null)
			{
				this.Complete(entry);
			}
		}

		internal void OnEvent(TrackEntry entry, Event e)
		{
			if (this.Event != null)
			{
				this.Event(entry, e);
			}
		}

		public void AssignEventSubscribersFrom(AnimationState src)
		{
			this.Event = src.Event;
			this.Start = src.Start;
			this.Interrupt = src.Interrupt;
			this.End = src.End;
			this.Dispose = src.Dispose;
			this.Complete = src.Complete;
		}

		public void AddEventSubscribersFrom(AnimationState src)
		{
			Event += src.Event;
			Start += src.Start;
			Interrupt += src.Interrupt;
			End += src.End;
			Dispose += src.Dispose;
			Complete += src.Complete;
		}

		public AnimationState(AnimationStateData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			this.data = data;
			queue = new EventQueue(this, delegate
			{
				animationsChanged = true;
			}, trackEntryPool);
		}

		public void Update(float delta)
		{
			delta *= timeScale;
			TrackEntry[] tracksItems = tracks.Items;
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				TrackEntry current = tracksItems[i];
				if (current == null)
				{
					continue;
				}
				current.animationLast = current.nextAnimationLast;
				current.trackLast = current.nextTrackLast;
				float currentDelta = delta * current.timeScale;
				if (current.delay > 0f)
				{
					current.delay -= currentDelta;
					if (current.delay > 0f)
					{
						continue;
					}
					currentDelta = 0f - current.delay;
					current.delay = 0f;
				}
				TrackEntry next = current.next;
				if (next != null)
				{
					float nextTime = current.trackLast - next.delay;
					if (nextTime >= 0f)
					{
						next.delay = 0f;
						next.trackTime += ((current.timeScale == 0f) ? 0f : ((nextTime / current.timeScale + delta) * next.timeScale));
						current.trackTime += currentDelta;
						SetCurrent(i, next, true);
						while (next.mixingFrom != null)
						{
							next.mixTime += delta;
							next = next.mixingFrom;
						}
						continue;
					}
				}
				else if (current.trackLast >= current.trackEnd && current.mixingFrom == null)
				{
					tracksItems[i] = null;
					queue.End(current);
					DisposeNext(current);
					continue;
				}
				if (current.mixingFrom != null && UpdateMixingFrom(current, delta))
				{
					TrackEntry from = current.mixingFrom;
					current.mixingFrom = null;
					if (from != null)
					{
						from.mixingTo = null;
					}
					while (from != null)
					{
						queue.End(from);
						from = from.mixingFrom;
					}
				}
				current.trackTime += currentDelta;
			}
			queue.Drain();
		}

		private bool UpdateMixingFrom(TrackEntry to, float delta)
		{
			TrackEntry from = to.mixingFrom;
			if (from == null)
			{
				return true;
			}
			bool finished = UpdateMixingFrom(from, delta);
			from.animationLast = from.nextAnimationLast;
			from.trackLast = from.nextTrackLast;
			if (to.mixTime > 0f && to.mixTime >= to.mixDuration)
			{
				if (from.totalAlpha == 0f || to.mixDuration == 0f)
				{
					to.mixingFrom = from.mixingFrom;
					if (from.mixingFrom != null)
					{
						from.mixingFrom.mixingTo = to;
					}
					to.interruptAlpha = from.interruptAlpha;
					queue.End(from);
				}
				return finished;
			}
			from.trackTime += delta * from.timeScale;
			to.mixTime += delta;
			return false;
		}

		public bool Apply(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (animationsChanged)
			{
				AnimationsChanged();
			}
			ExposedList<Event> events = this.events;
			bool applied = false;
			TrackEntry[] tracksItems = tracks.Items;
			int i = 0;
			for (int k = tracks.Count; i < k; i++)
			{
				TrackEntry current = tracksItems[i];
				if (current == null || current.delay > 0f)
				{
					continue;
				}
				applied = true;
				MixBlend blend = ((i == 0) ? MixBlend.First : current.mixBlend);
				float mix = current.alpha;
				if (current.mixingFrom != null)
				{
					mix *= ApplyMixingFrom(current, skeleton, blend);
				}
				else if (current.trackTime >= current.trackEnd && current.next == null)
				{
					mix = 0f;
				}
				float animationLast = current.animationLast;
				float animationTime = current.AnimationTime;
				int timelineCount = current.animation.timelines.Count;
				ExposedList<Timeline> timelines = current.animation.timelines;
				Timeline[] timelinesItems = timelines.Items;
				if ((i == 0 && mix == 1f) || blend == MixBlend.Add)
				{
					for (int ii2 = 0; ii2 < timelineCount; ii2++)
					{
						Timeline timeline2 = timelinesItems[ii2];
						if (timeline2 is AttachmentTimeline)
						{
							ApplyAttachmentTimeline((AttachmentTimeline)timeline2, skeleton, animationTime, blend, true);
						}
						else
						{
							timeline2.Apply(skeleton, animationLast, animationTime, events, mix, blend, MixDirection.In);
						}
					}
				}
				else
				{
					int[] timelineMode = current.timelineMode.Items;
					bool firstFrame = current.timelinesRotation.Count != timelineCount << 1;
					if (firstFrame)
					{
						current.timelinesRotation.Resize(timelines.Count << 1);
					}
					float[] timelinesRotation = current.timelinesRotation.Items;
					for (int ii = 0; ii < timelineCount; ii++)
					{
						Timeline timeline = timelinesItems[ii];
						MixBlend timelineBlend = ((timelineMode[ii] == 0) ? blend : MixBlend.Setup);
						if (timeline is RotateTimeline rotateTimeline)
						{
							ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, mix, timelineBlend, timelinesRotation, ii << 1, firstFrame);
						}
						else if (timeline is AttachmentTimeline)
						{
							ApplyAttachmentTimeline((AttachmentTimeline)timeline, skeleton, animationTime, blend, true);
						}
						else
						{
							timeline.Apply(skeleton, animationLast, animationTime, events, mix, timelineBlend, MixDirection.In);
						}
					}
				}
				QueueEvents(current, animationTime);
				events.Clear(false);
				current.nextAnimationLast = animationTime;
				current.nextTrackLast = current.trackTime;
			}
			int setupState = unkeyedState + 1;
			Slot[] slots = skeleton.slots.Items;
			int j = 0;
			for (int l = skeleton.slots.Count; j < l; j++)
			{
				Slot slot = slots[j];
				if (slot.attachmentState == setupState)
				{
					string attachmentName = slot.data.attachmentName;
					slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slot.data.index, attachmentName));
				}
			}
			unkeyedState += 2;
			queue.Drain();
			return applied;
		}

		public bool ApplyEventTimelinesOnly(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			ExposedList<Event> events = this.events;
			bool applied = false;
			TrackEntry[] tracksItems = tracks.Items;
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				TrackEntry current = tracksItems[i];
				if (current == null || current.delay > 0f)
				{
					continue;
				}
				applied = true;
				if (current.mixingFrom != null)
				{
					ApplyMixingFromEventTimelinesOnly(current, skeleton);
				}
				float animationLast = current.animationLast;
				float animationTime = current.AnimationTime;
				int timelineCount = current.animation.timelines.Count;
				ExposedList<Timeline> timelines = current.animation.timelines;
				Timeline[] timelinesItems = timelines.Items;
				for (int ii = 0; ii < timelineCount; ii++)
				{
					Timeline timeline = timelinesItems[ii];
					if (timeline is EventTimeline)
					{
						timeline.Apply(skeleton, animationLast, animationTime, events, 1f, MixBlend.Setup, MixDirection.In);
					}
				}
				QueueEvents(current, animationTime);
				events.Clear(false);
				current.nextAnimationLast = animationTime;
				current.nextTrackLast = current.trackTime;
			}
			queue.Drain();
			return applied;
		}

		private float ApplyMixingFrom(TrackEntry to, Skeleton skeleton, MixBlend blend)
		{
			TrackEntry from = to.mixingFrom;
			if (from.mixingFrom != null)
			{
				ApplyMixingFrom(from, skeleton, blend);
			}
			float mix;
			if (to.mixDuration == 0f)
			{
				mix = 1f;
				if (blend == MixBlend.First)
				{
					blend = MixBlend.Setup;
				}
			}
			else
			{
				mix = to.mixTime / to.mixDuration;
				if (mix > 1f)
				{
					mix = 1f;
				}
				if (blend != MixBlend.First)
				{
					blend = from.mixBlend;
				}
			}
			ExposedList<Event> eventBuffer = ((mix < from.eventThreshold) ? events : null);
			bool attachments = mix < from.attachmentThreshold;
			bool drawOrder = mix < from.drawOrderThreshold;
			float animationLast = from.animationLast;
			float animationTime = from.AnimationTime;
			ExposedList<Timeline> timelines = from.animation.timelines;
			int timelineCount = timelines.Count;
			Timeline[] timelinesItems = timelines.Items;
			float alphaHold = from.alpha * to.interruptAlpha;
			float alphaMix = alphaHold * (1f - mix);
			if (blend == MixBlend.Add)
			{
				for (int j = 0; j < timelineCount; j++)
				{
					timelinesItems[j].Apply(skeleton, animationLast, animationTime, eventBuffer, alphaMix, blend, MixDirection.Out);
				}
			}
			else
			{
				int[] timelineMode = from.timelineMode.Items;
				TrackEntry[] timelineHoldMix = from.timelineHoldMix.Items;
				bool firstFrame = from.timelinesRotation.Count != timelineCount << 1;
				if (firstFrame)
				{
					from.timelinesRotation.Resize(timelines.Count << 1);
				}
				float[] timelinesRotation = from.timelinesRotation.Items;
				from.totalAlpha = 0f;
				for (int i = 0; i < timelineCount; i++)
				{
					Timeline timeline = timelinesItems[i];
					MixDirection direction = MixDirection.Out;
					MixBlend timelineBlend;
					float alpha;
					switch (timelineMode[i])
					{
					case 0:
						if (!drawOrder && timeline is DrawOrderTimeline)
						{
							continue;
						}
						timelineBlend = blend;
						alpha = alphaMix;
						break;
					case 1:
						timelineBlend = MixBlend.Setup;
						alpha = alphaMix;
						break;
					case 2:
						timelineBlend = blend;
						alpha = alphaHold;
						break;
					case 3:
						timelineBlend = MixBlend.Setup;
						alpha = alphaHold;
						break;
					default:
					{
						timelineBlend = MixBlend.Setup;
						TrackEntry holdMix = timelineHoldMix[i];
						alpha = alphaHold * Math.Max(0f, 1f - holdMix.mixTime / holdMix.mixDuration);
						break;
					}
					}
					from.totalAlpha += alpha;
					if (timeline is RotateTimeline rotateTimeline)
					{
						ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, alpha, timelineBlend, timelinesRotation, i << 1, firstFrame);
						continue;
					}
					if (timeline is AttachmentTimeline)
					{
						ApplyAttachmentTimeline((AttachmentTimeline)timeline, skeleton, animationTime, timelineBlend, attachments);
						continue;
					}
					if (drawOrder && timeline is DrawOrderTimeline && timelineBlend == MixBlend.Setup)
					{
						direction = MixDirection.In;
					}
					timeline.Apply(skeleton, animationLast, animationTime, eventBuffer, alpha, timelineBlend, direction);
				}
			}
			if (to.mixDuration > 0f)
			{
				QueueEvents(from, animationTime);
			}
			events.Clear(false);
			from.nextAnimationLast = animationTime;
			from.nextTrackLast = from.trackTime;
			return mix;
		}

		private float ApplyMixingFromEventTimelinesOnly(TrackEntry to, Skeleton skeleton)
		{
			TrackEntry from = to.mixingFrom;
			if (from.mixingFrom != null)
			{
				ApplyMixingFromEventTimelinesOnly(from, skeleton);
			}
			float mix;
			if (to.mixDuration == 0f)
			{
				mix = 1f;
			}
			else
			{
				mix = to.mixTime / to.mixDuration;
				if (mix > 1f)
				{
					mix = 1f;
				}
			}
			ExposedList<Event> eventBuffer = ((mix < from.eventThreshold) ? events : null);
			if (eventBuffer == null)
			{
				return mix;
			}
			float animationLast = from.animationLast;
			float animationTime = from.AnimationTime;
			ExposedList<Timeline> timelines = from.animation.timelines;
			int timelineCount = timelines.Count;
			Timeline[] timelinesItems = timelines.Items;
			for (int i = 0; i < timelineCount; i++)
			{
				Timeline timeline = timelinesItems[i];
				if (timeline is EventTimeline)
				{
					timeline.Apply(skeleton, animationLast, animationTime, eventBuffer, 0f, MixBlend.Setup, MixDirection.Out);
				}
			}
			if (to.mixDuration > 0f)
			{
				QueueEvents(from, animationTime);
			}
			events.Clear(false);
			from.nextAnimationLast = animationTime;
			from.nextTrackLast = from.trackTime;
			return mix;
		}

		private void ApplyAttachmentTimeline(AttachmentTimeline timeline, Skeleton skeleton, float time, MixBlend blend, bool attachments)
		{
			Slot slot = skeleton.slots.Items[timeline.slotIndex];
			if (!slot.bone.active)
			{
				return;
			}
			float[] frames = timeline.frames;
			if (time < frames[0])
			{
				if (blend == MixBlend.Setup || blend == MixBlend.First)
				{
					SetAttachment(skeleton, slot, slot.data.attachmentName, attachments);
				}
			}
			else
			{
				int frameIndex = ((!(time >= frames[frames.Length - 1])) ? (Animation.BinarySearch(frames, time) - 1) : (frames.Length - 1));
				SetAttachment(skeleton, slot, timeline.attachmentNames[frameIndex], attachments);
			}
			if (slot.attachmentState <= unkeyedState)
			{
				slot.attachmentState = unkeyedState + 1;
			}
		}

		private void SetAttachment(Skeleton skeleton, Slot slot, string attachmentName, bool attachments)
		{
			slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slot.data.index, attachmentName));
			if (attachments)
			{
				slot.attachmentState = unkeyedState + 2;
			}
		}

		private static void ApplyRotateTimeline(RotateTimeline timeline, Skeleton skeleton, float time, float alpha, MixBlend blend, float[] timelinesRotation, int i, bool firstFrame)
		{
			if (firstFrame)
			{
				timelinesRotation[i] = 0f;
			}
			if (alpha == 1f)
			{
				timeline.Apply(skeleton, 0f, time, null, 1f, blend, MixDirection.In);
				return;
			}
			Bone bone = skeleton.bones.Items[timeline.boneIndex];
			if (!bone.active)
			{
				return;
			}
			float[] frames = timeline.frames;
			float r2;
			float r1;
			if (time < frames[0])
			{
				switch (blend)
				{
				default:
					return;
				case MixBlend.Setup:
					bone.rotation = bone.data.rotation;
					return;
				case MixBlend.First:
					break;
				}
				r1 = bone.rotation;
				r2 = bone.data.rotation;
			}
			else
			{
				r1 = ((blend == MixBlend.Setup) ? bone.data.rotation : bone.rotation);
				if (time >= frames[frames.Length - 2])
				{
					r2 = bone.data.rotation + frames[frames.Length + -1];
				}
				else
				{
					int frame = Animation.BinarySearch(frames, time, 2);
					float prevRotation = frames[frame + -1];
					float frameTime = frames[frame];
					float percent = timeline.GetCurvePercent((frame >> 1) - 1, 1f - (time - frameTime) / (frames[frame + -2] - frameTime));
					r2 = frames[frame + 1] - prevRotation;
					r2 -= (float)((16384 - (int)(16384.499999999996 - (double)(r2 / 360f))) * 360);
					r2 = prevRotation + r2 * percent + bone.data.rotation;
					r2 -= (float)((16384 - (int)(16384.499999999996 - (double)(r2 / 360f))) * 360);
				}
			}
			float diff = r2 - r1;
			diff -= (float)((16384 - (int)(16384.499999999996 - (double)(diff / 360f))) * 360);
			float total;
			if (diff == 0f)
			{
				total = timelinesRotation[i];
			}
			else
			{
				float lastTotal;
				float lastDiff;
				if (firstFrame)
				{
					lastTotal = 0f;
					lastDiff = diff;
				}
				else
				{
					lastTotal = timelinesRotation[i];
					lastDiff = timelinesRotation[i + 1];
				}
				bool current = diff > 0f;
				bool dir = lastTotal >= 0f;
				if (Math.Sign(lastDiff) != Math.Sign(diff) && Math.Abs(lastDiff) <= 90f)
				{
					if (Math.Abs(lastTotal) > 180f)
					{
						lastTotal += (float)(360 * Math.Sign(lastTotal));
					}
					dir = current;
				}
				total = diff + lastTotal - lastTotal % 360f;
				if (dir != current)
				{
					total += (float)(360 * Math.Sign(lastTotal));
				}
				timelinesRotation[i] = total;
			}
			timelinesRotation[i + 1] = diff;
			r1 += total * alpha;
			bone.rotation = r1 - (float)((16384 - (int)(16384.499999999996 - (double)(r1 / 360f))) * 360);
		}

		private void QueueEvents(TrackEntry entry, float animationTime)
		{
			float animationStart = entry.animationStart;
			float animationEnd = entry.animationEnd;
			float duration = animationEnd - animationStart;
			float trackLastWrapped = entry.trackLast % duration;
			ExposedList<Event> events = this.events;
			Event[] eventsItems = events.Items;
			int i = 0;
			int j;
			for (j = events.Count; i < j; i++)
			{
				Event e2 = eventsItems[i];
				if (e2.time < trackLastWrapped)
				{
					break;
				}
				if (!(e2.time > animationEnd))
				{
					queue.Event(entry, e2);
				}
			}
			bool complete = false;
			if ((!entry.loop) ? (animationTime >= animationEnd && entry.animationLast < animationEnd) : (duration == 0f || trackLastWrapped > entry.trackTime % duration))
			{
				queue.Complete(entry);
			}
			for (; i < j; i++)
			{
				Event e = eventsItems[i];
				if (!(e.time < animationStart))
				{
					queue.Event(entry, eventsItems[i]);
				}
			}
		}

		public void ClearTracks()
		{
			bool oldDrainDisabled = queue.drainDisabled;
			queue.drainDisabled = true;
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				ClearTrack(i);
			}
			tracks.Clear();
			queue.drainDisabled = oldDrainDisabled;
			queue.Drain();
		}

		public void ClearTrack(int trackIndex)
		{
			if (trackIndex >= tracks.Count)
			{
				return;
			}
			TrackEntry current = tracks.Items[trackIndex];
			if (current == null)
			{
				return;
			}
			queue.End(current);
			DisposeNext(current);
			TrackEntry entry = current;
			while (true)
			{
				TrackEntry from = entry.mixingFrom;
				if (from == null)
				{
					break;
				}
				queue.End(from);
				entry.mixingFrom = null;
				entry.mixingTo = null;
				entry = from;
			}
			tracks.Items[current.trackIndex] = null;
			queue.Drain();
		}

		private void SetCurrent(int index, TrackEntry current, bool interrupt)
		{
			TrackEntry from = ExpandToIndex(index);
			tracks.Items[index] = current;
			if (from != null)
			{
				if (interrupt)
				{
					queue.Interrupt(from);
				}
				current.mixingFrom = from;
				from.mixingTo = current;
				current.mixTime = 0f;
				if (from.mixingFrom != null && from.mixDuration > 0f)
				{
					current.interruptAlpha *= Math.Min(1f, from.mixTime / from.mixDuration);
				}
				from.timelinesRotation.Clear();
			}
			queue.Start(current);
		}

		public TrackEntry SetAnimation(int trackIndex, string animationName, bool loop)
		{
			Animation animation = data.skeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName, "animationName");
			}
			return SetAnimation(trackIndex, animation, loop);
		}

		public TrackEntry SetAnimation(int trackIndex, Animation animation, bool loop)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation", "animation cannot be null.");
			}
			bool interrupt = true;
			TrackEntry current = ExpandToIndex(trackIndex);
			if (current != null)
			{
				if (current.nextTrackLast == -1f)
				{
					tracks.Items[trackIndex] = current.mixingFrom;
					queue.Interrupt(current);
					queue.End(current);
					DisposeNext(current);
					current = current.mixingFrom;
					interrupt = false;
				}
				else
				{
					DisposeNext(current);
				}
			}
			TrackEntry entry = NewTrackEntry(trackIndex, animation, loop, current);
			SetCurrent(trackIndex, entry, interrupt);
			queue.Drain();
			return entry;
		}

		public TrackEntry AddAnimation(int trackIndex, string animationName, bool loop, float delay)
		{
			Animation animation = data.skeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName, "animationName");
			}
			return AddAnimation(trackIndex, animation, loop, delay);
		}

		public TrackEntry AddAnimation(int trackIndex, Animation animation, bool loop, float delay)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation", "animation cannot be null.");
			}
			TrackEntry last = ExpandToIndex(trackIndex);
			if (last != null)
			{
				while (last.next != null)
				{
					last = last.next;
				}
			}
			TrackEntry entry = NewTrackEntry(trackIndex, animation, loop, last);
			if (last == null)
			{
				SetCurrent(trackIndex, entry, true);
				queue.Drain();
			}
			else
			{
				last.next = entry;
				if (delay <= 0f)
				{
					float duration = last.animationEnd - last.animationStart;
					if (duration != 0f)
					{
						delay = ((!last.loop) ? (delay + Math.Max(duration, last.trackTime)) : (delay + duration * (float)(1 + (int)(last.trackTime / duration))));
						delay -= data.GetMix(last.animation, animation);
					}
					else
					{
						delay = last.trackTime;
					}
				}
			}
			entry.delay = delay;
			return entry;
		}

		public TrackEntry SetEmptyAnimation(int trackIndex, float mixDuration)
		{
			TrackEntry entry = SetAnimation(trackIndex, EmptyAnimation, false);
			entry.mixDuration = mixDuration;
			entry.trackEnd = mixDuration;
			return entry;
		}

		public TrackEntry AddEmptyAnimation(int trackIndex, float mixDuration, float delay)
		{
			if (delay <= 0f)
			{
				delay -= mixDuration;
			}
			TrackEntry entry = AddAnimation(trackIndex, EmptyAnimation, false, delay);
			entry.mixDuration = mixDuration;
			entry.trackEnd = mixDuration;
			return entry;
		}

		public void SetEmptyAnimations(float mixDuration)
		{
			bool oldDrainDisabled = queue.drainDisabled;
			queue.drainDisabled = true;
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				TrackEntry current = tracks.Items[i];
				if (current != null)
				{
					SetEmptyAnimation(current.trackIndex, mixDuration);
				}
			}
			queue.drainDisabled = oldDrainDisabled;
			queue.Drain();
		}

		private TrackEntry ExpandToIndex(int index)
		{
			if (index < tracks.Count)
			{
				return tracks.Items[index];
			}
			tracks.Resize(index + 1);
			return null;
		}

		private TrackEntry NewTrackEntry(int trackIndex, Animation animation, bool loop, TrackEntry last)
		{
			TrackEntry entry = trackEntryPool.Obtain();
			entry.trackIndex = trackIndex;
			entry.animation = animation;
			entry.loop = loop;
			entry.holdPrevious = false;
			entry.eventThreshold = 0f;
			entry.attachmentThreshold = 0f;
			entry.drawOrderThreshold = 0f;
			entry.animationStart = 0f;
			entry.animationEnd = animation.Duration;
			entry.animationLast = -1f;
			entry.nextAnimationLast = -1f;
			entry.delay = 0f;
			entry.trackTime = 0f;
			entry.trackLast = -1f;
			entry.nextTrackLast = -1f;
			entry.trackEnd = float.MaxValue;
			entry.timeScale = 1f;
			entry.alpha = 1f;
			entry.interruptAlpha = 1f;
			entry.mixTime = 0f;
			entry.mixDuration = ((last == null) ? 0f : data.GetMix(last.animation, animation));
			return entry;
		}

		private void DisposeNext(TrackEntry entry)
		{
			for (TrackEntry next = entry.next; next != null; next = next.next)
			{
				queue.Dispose(next);
			}
			entry.next = null;
		}

		private void AnimationsChanged()
		{
			animationsChanged = false;
			propertyIDs.Clear();
			TrackEntry[] tracksItems = tracks.Items;
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				TrackEntry entry = tracksItems[i];
				if (entry == null)
				{
					continue;
				}
				while (entry.mixingFrom != null)
				{
					entry = entry.mixingFrom;
				}
				do
				{
					if (entry.mixingTo == null || entry.mixBlend != MixBlend.Add)
					{
						ComputeHold(entry);
					}
					entry = entry.mixingTo;
				}
				while (entry != null);
			}
		}

		private void ComputeHold(TrackEntry entry)
		{
			TrackEntry to = entry.mixingTo;
			Timeline[] timelines = entry.animation.timelines.Items;
			int timelinesCount = entry.animation.timelines.Count;
			int[] timelineMode = entry.timelineMode.Resize(timelinesCount).Items;
			entry.timelineHoldMix.Clear();
			TrackEntry[] timelineHoldMix = entry.timelineHoldMix.Resize(timelinesCount).Items;
			HashSet<int> propertyIDs = this.propertyIDs;
			if (to != null && to.holdPrevious)
			{
				for (int j = 0; j < timelinesCount; j++)
				{
					timelineMode[j] = (propertyIDs.Add(timelines[j].PropertyId) ? 3 : 2);
				}
				return;
			}
			for (int i = 0; i < timelinesCount; i++)
			{
				Timeline timeline = timelines[i];
				int id = timeline.PropertyId;
				if (!propertyIDs.Add(id))
				{
					timelineMode[i] = 0;
					continue;
				}
				if (to == null || timeline is AttachmentTimeline || timeline is DrawOrderTimeline || timeline is EventTimeline || !to.animation.HasTimeline(id))
				{
					timelineMode[i] = 1;
					continue;
				}
				TrackEntry next = to.mixingTo;
				while (true)
				{
					if (next != null)
					{
						if (next.animation.HasTimeline(id))
						{
							next = next.mixingTo;
							continue;
						}
						if (next.mixDuration > 0f)
						{
							timelineMode[i] = 4;
							timelineHoldMix[i] = next;
							break;
						}
					}
					timelineMode[i] = 3;
					break;
				}
			}
		}

		public TrackEntry GetCurrent(int trackIndex)
		{
			if (trackIndex >= tracks.Count)
			{
				return null;
			}
			return tracks.Items[trackIndex];
		}

		public void ClearListenerNotifications()
		{
			queue.Clear();
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			int i = 0;
			for (int j = tracks.Count; i < j; i++)
			{
				TrackEntry entry = tracks.Items[i];
				if (entry != null)
				{
					if (buffer.Length > 0)
					{
						buffer.Append(", ");
					}
					buffer.Append(entry.ToString());
				}
			}
			if (buffer.Length == 0)
			{
				return "<none>";
			}
			return buffer.ToString();
		}
	}
}

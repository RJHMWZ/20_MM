using System;

namespace Spine
{
	public class AttachmentTimeline : Timeline, ISlotTimeline
	{
		internal int slotIndex;

		internal float[] frames;

		internal string[] attachmentNames;

		public int PropertyId => 67108864 + slotIndex;

		public int FrameCount => frames.Length;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				slotIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public string[] AttachmentNames
		{
			get
			{
				return attachmentNames;
			}
			set
			{
				attachmentNames = value;
			}
		}

		public AttachmentTimeline(int frameCount)
		{
			frames = new float[frameCount];
			attachmentNames = new string[frameCount];
		}

		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			frames[frameIndex] = time;
			attachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (!slot.bone.active)
			{
				return;
			}
			if (direction == MixDirection.Out)
			{
				if (blend == MixBlend.Setup)
				{
					SetAttachment(skeleton, slot, slot.data.attachmentName);
				}
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				if (blend == MixBlend.Setup || blend == MixBlend.First)
				{
					SetAttachment(skeleton, slot, slot.data.attachmentName);
				}
			}
			else
			{
				int frameIndex = ((!(time >= frames[frames.Length - 1])) ? (Animation.BinarySearch(frames, time) - 1) : (frames.Length - 1));
				SetAttachment(skeleton, slot, attachmentNames[frameIndex]);
			}
		}

		private void SetAttachment(Skeleton skeleton, Slot slot, string attachmentName)
		{
			slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
		}
	}
}

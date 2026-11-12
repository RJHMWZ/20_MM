namespace Spine
{
	public class EventTimeline : Timeline
	{
		internal float[] frames;

		private Event[] events;

		public int PropertyId => 117440512;

		public int FrameCount => frames.Length;

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

		public Event[] Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		public EventTimeline(int frameCount)
		{
			frames = new float[frameCount];
			events = new Event[frameCount];
		}

		public void SetFrame(int frameIndex, Event e)
		{
			frames[frameIndex] = e.Time;
			events[frameIndex] = e;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			if (firedEvents == null)
			{
				return;
			}
			float[] frames = this.frames;
			int frameCount = frames.Length;
			if (lastTime > time)
			{
				Apply(skeleton, lastTime, 2.1474836E+09f, firedEvents, alpha, blend, direction);
				lastTime = -1f;
			}
			else if (lastTime >= frames[frameCount - 1])
			{
				return;
			}
			if (time < frames[0])
			{
				return;
			}
			int frame;
			if (lastTime < frames[0])
			{
				frame = 0;
			}
			else
			{
				frame = Animation.BinarySearch(frames, lastTime);
				float frameTime = frames[frame];
				while (frame > 0 && frames[frame - 1] == frameTime)
				{
					frame--;
				}
			}
			for (; frame < frameCount && time >= frames[frame]; frame++)
			{
				firedEvents.Add(events[frame]);
			}
		}
	}
}

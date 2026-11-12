using System;
using System.Collections.Generic;

namespace Spine
{
	public class Animation
	{
		internal string name;

		internal ExposedList<Timeline> timelines;

		internal HashSet<int> timelineIds;

		internal float duration;

		public ExposedList<Timeline> Timelines
		{
			get
			{
				return timelines;
			}
			set
			{
				timelines = value;
			}
		}

		public float Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public string Name => name;

		public Animation(string name, ExposedList<Timeline> timelines, float duration)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			if (timelines == null)
			{
				throw new ArgumentNullException("timelines", "timelines cannot be null.");
			}
			int[] propertyIDs = new int[timelines.Count];
			for (int i = 0; i < timelines.Count; i++)
			{
				propertyIDs[i] = timelines.Items[i].PropertyId;
			}
			timelineIds = new HashSet<int>(propertyIDs);
			this.name = name;
			this.timelines = timelines;
			this.duration = duration;
		}

		public bool HasTimeline(int id)
		{
			return timelineIds.Contains(id);
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, bool loop, ExposedList<Event> events, float alpha, MixBlend blend, MixDirection direction)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (loop && duration != 0f)
			{
				time %= duration;
				if (lastTime > 0f)
				{
					lastTime %= duration;
				}
			}
			ExposedList<Timeline> timelines = this.timelines;
			int i = 0;
			for (int j = timelines.Count; i < j; i++)
			{
				timelines.Items[i].Apply(skeleton, lastTime, time, events, alpha, blend, direction);
			}
		}

		public override string ToString()
		{
			return name;
		}

		internal static int BinarySearch(float[] values, float target, int step)
		{
			int low = 0;
			int high = values.Length / step - 2;
			if (high == 0)
			{
				return step;
			}
			int current = (int)((uint)high >> 1);
			while (true)
			{
				if (values[(current + 1) * step] <= target)
				{
					low = current + 1;
				}
				else
				{
					high = current;
				}
				if (low == high)
				{
					break;
				}
				current = (int)((uint)(low + high) >> 1);
			}
			return (low + 1) * step;
		}

		internal static int BinarySearch(float[] values, float target)
		{
			int low = 0;
			int high = values.Length - 2;
			if (high == 0)
			{
				return 1;
			}
			int current = (int)((uint)high >> 1);
			while (true)
			{
				if (values[current + 1] <= target)
				{
					low = current + 1;
				}
				else
				{
					high = current;
				}
				if (low == high)
				{
					break;
				}
				current = (int)((uint)(low + high) >> 1);
			}
			return low + 1;
		}

		internal static int LinearSearch(float[] values, float target, int step)
		{
			int i = 0;
			for (int last = values.Length - step; i <= last; i += step)
			{
				if (values[i] > target)
				{
					return i;
				}
			}
			return -1;
		}
	}
}

using System;

namespace Spine
{
	public class PathConstraintPositionTimeline : CurveTimeline
	{
		public const int ENTRIES = 2;

		protected const int PREV_TIME = -2;

		protected const int PREV_VALUE = -1;

		protected const int VALUE = 1;

		internal int pathConstraintIndex;

		internal float[] frames;

		public override int PropertyId => 184549376 + pathConstraintIndex;

		public int PathConstraintIndex
		{
			get
			{
				return pathConstraintIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				pathConstraintIndex = value;
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

		public PathConstraintPositionTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 2];
		}

		public void SetFrame(int frameIndex, float time, float position)
		{
			frameIndex *= 2;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = position;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			PathConstraint constraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			if (!constraint.active)
			{
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				switch (blend)
				{
				case MixBlend.Setup:
					constraint.position = constraint.data.position;
					break;
				case MixBlend.First:
					constraint.position += (constraint.data.position - constraint.position) * alpha;
					break;
				}
				return;
			}
			float position;
			if (time >= frames[frames.Length - 2])
			{
				position = frames[frames.Length + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 2);
				position = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 2 - 1, 1f - (time - frameTime) / (frames[frame + -2] - frameTime));
				position += (frames[frame + 1] - position) * percent;
			}
			if (blend == MixBlend.Setup)
			{
				constraint.position = constraint.data.position + (position - constraint.data.position) * alpha;
			}
			else
			{
				constraint.position += (position - constraint.position) * alpha;
			}
		}
	}
}

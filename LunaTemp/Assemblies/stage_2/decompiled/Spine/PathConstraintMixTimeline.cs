using System;

namespace Spine
{
	public class PathConstraintMixTimeline : CurveTimeline
	{
		public const int ENTRIES = 3;

		private const int PREV_TIME = -3;

		private const int PREV_ROTATE = -2;

		private const int PREV_TRANSLATE = -1;

		private const int ROTATE = 1;

		private const int TRANSLATE = 2;

		internal int pathConstraintIndex;

		internal float[] frames;

		public override int PropertyId => 218103808 + pathConstraintIndex;

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

		public PathConstraintMixTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
		}

		public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = rotateMix;
			frames[frameIndex + 2] = translateMix;
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
					constraint.rotateMix = constraint.data.rotateMix;
					constraint.translateMix = constraint.data.translateMix;
					break;
				case MixBlend.First:
					constraint.rotateMix += (constraint.data.rotateMix - constraint.rotateMix) * alpha;
					constraint.translateMix += (constraint.data.translateMix - constraint.translateMix) * alpha;
					break;
				}
				return;
			}
			float rotate;
			float translate;
			if (time >= frames[frames.Length - 3])
			{
				rotate = frames[frames.Length + -2];
				translate = frames[frames.Length + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 3);
				rotate = frames[frame + -2];
				translate = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 3 - 1, 1f - (time - frameTime) / (frames[frame + -3] - frameTime));
				rotate += (frames[frame + 1] - rotate) * percent;
				translate += (frames[frame + 2] - translate) * percent;
			}
			if (blend == MixBlend.Setup)
			{
				constraint.rotateMix = constraint.data.rotateMix + (rotate - constraint.data.rotateMix) * alpha;
				constraint.translateMix = constraint.data.translateMix + (translate - constraint.data.translateMix) * alpha;
			}
			else
			{
				constraint.rotateMix += (rotate - constraint.rotateMix) * alpha;
				constraint.translateMix += (translate - constraint.translateMix) * alpha;
			}
		}
	}
}

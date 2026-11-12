using System;

namespace Spine
{
	public class TransformConstraintTimeline : CurveTimeline
	{
		public const int ENTRIES = 5;

		private const int PREV_TIME = -5;

		private const int PREV_ROTATE = -4;

		private const int PREV_TRANSLATE = -3;

		private const int PREV_SCALE = -2;

		private const int PREV_SHEAR = -1;

		private const int ROTATE = 1;

		private const int TRANSLATE = 2;

		private const int SCALE = 3;

		private const int SHEAR = 4;

		internal int transformConstraintIndex;

		internal float[] frames;

		public override int PropertyId => 167772160 + transformConstraintIndex;

		public int TransformConstraintIndex
		{
			get
			{
				return transformConstraintIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				transformConstraintIndex = value;
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

		public TransformConstraintTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 5];
		}

		public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix, float scaleMix, float shearMix)
		{
			frameIndex *= 5;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = rotateMix;
			frames[frameIndex + 2] = translateMix;
			frames[frameIndex + 3] = scaleMix;
			frames[frameIndex + 4] = shearMix;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			TransformConstraint constraint = skeleton.transformConstraints.Items[transformConstraintIndex];
			if (!constraint.active)
			{
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				TransformConstraintData data2 = constraint.data;
				switch (blend)
				{
				case MixBlend.Setup:
					constraint.rotateMix = data2.rotateMix;
					constraint.translateMix = data2.translateMix;
					constraint.scaleMix = data2.scaleMix;
					constraint.shearMix = data2.shearMix;
					break;
				case MixBlend.First:
					constraint.rotateMix += (data2.rotateMix - constraint.rotateMix) * alpha;
					constraint.translateMix += (data2.translateMix - constraint.translateMix) * alpha;
					constraint.scaleMix += (data2.scaleMix - constraint.scaleMix) * alpha;
					constraint.shearMix += (data2.shearMix - constraint.shearMix) * alpha;
					break;
				}
				return;
			}
			float rotate;
			float translate;
			float scale;
			float shear;
			if (time >= frames[frames.Length - 5])
			{
				int i = frames.Length;
				rotate = frames[i + -4];
				translate = frames[i + -3];
				scale = frames[i + -2];
				shear = frames[i + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 5);
				rotate = frames[frame + -4];
				translate = frames[frame + -3];
				scale = frames[frame + -2];
				shear = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 5 - 1, 1f - (time - frameTime) / (frames[frame + -5] - frameTime));
				rotate += (frames[frame + 1] - rotate) * percent;
				translate += (frames[frame + 2] - translate) * percent;
				scale += (frames[frame + 3] - scale) * percent;
				shear += (frames[frame + 4] - shear) * percent;
			}
			if (blend == MixBlend.Setup)
			{
				TransformConstraintData data = constraint.data;
				constraint.rotateMix = data.rotateMix + (rotate - data.rotateMix) * alpha;
				constraint.translateMix = data.translateMix + (translate - data.translateMix) * alpha;
				constraint.scaleMix = data.scaleMix + (scale - data.scaleMix) * alpha;
				constraint.shearMix = data.shearMix + (shear - data.shearMix) * alpha;
			}
			else
			{
				constraint.rotateMix += (rotate - constraint.rotateMix) * alpha;
				constraint.translateMix += (translate - constraint.translateMix) * alpha;
				constraint.scaleMix += (scale - constraint.scaleMix) * alpha;
				constraint.shearMix += (shear - constraint.shearMix) * alpha;
			}
		}
	}
}

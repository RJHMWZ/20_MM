using System;

namespace Spine
{
	public class IkConstraintTimeline : CurveTimeline
	{
		public const int ENTRIES = 6;

		private const int PREV_TIME = -6;

		private const int PREV_MIX = -5;

		private const int PREV_SOFTNESS = -4;

		private const int PREV_BEND_DIRECTION = -3;

		private const int PREV_COMPRESS = -2;

		private const int PREV_STRETCH = -1;

		private const int MIX = 1;

		private const int SOFTNESS = 2;

		private const int BEND_DIRECTION = 3;

		private const int COMPRESS = 4;

		private const int STRETCH = 5;

		internal int ikConstraintIndex;

		internal float[] frames;

		public override int PropertyId => 150994944 + ikConstraintIndex;

		public int IkConstraintIndex
		{
			get
			{
				return ikConstraintIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				ikConstraintIndex = value;
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

		public IkConstraintTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 6];
		}

		public void SetFrame(int frameIndex, float time, float mix, float softness, int bendDirection, bool compress, bool stretch)
		{
			frameIndex *= 6;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = mix;
			frames[frameIndex + 2] = softness;
			frames[frameIndex + 3] = bendDirection;
			frames[frameIndex + 4] = (compress ? 1 : 0);
			frames[frameIndex + 5] = (stretch ? 1 : 0);
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			IkConstraint constraint = skeleton.ikConstraints.Items[ikConstraintIndex];
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
					constraint.mix = constraint.data.mix;
					constraint.softness = constraint.data.softness;
					constraint.bendDirection = constraint.data.bendDirection;
					constraint.compress = constraint.data.compress;
					constraint.stretch = constraint.data.stretch;
					break;
				case MixBlend.First:
					constraint.mix += (constraint.data.mix - constraint.mix) * alpha;
					constraint.softness += (constraint.data.softness - constraint.softness) * alpha;
					constraint.bendDirection = constraint.data.bendDirection;
					constraint.compress = constraint.data.compress;
					constraint.stretch = constraint.data.stretch;
					break;
				}
				return;
			}
			if (time >= frames[frames.Length - 6])
			{
				if (blend == MixBlend.Setup)
				{
					constraint.mix = constraint.data.mix + (frames[frames.Length + -5] - constraint.data.mix) * alpha;
					constraint.softness = constraint.data.softness + (frames[frames.Length + -4] - constraint.data.softness) * alpha;
					if (direction == MixDirection.Out)
					{
						constraint.bendDirection = constraint.data.bendDirection;
						constraint.compress = constraint.data.compress;
						constraint.stretch = constraint.data.stretch;
					}
					else
					{
						constraint.bendDirection = (int)frames[frames.Length + -3];
						constraint.compress = frames[frames.Length + -2] != 0f;
						constraint.stretch = frames[frames.Length + -1] != 0f;
					}
				}
				else
				{
					constraint.mix += (frames[frames.Length + -5] - constraint.mix) * alpha;
					constraint.softness += (frames[frames.Length + -4] - constraint.softness) * alpha;
					if (direction == MixDirection.In)
					{
						constraint.bendDirection = (int)frames[frames.Length + -3];
						constraint.compress = frames[frames.Length + -2] != 0f;
						constraint.stretch = frames[frames.Length + -1] != 0f;
					}
				}
				return;
			}
			int frame = Animation.BinarySearch(frames, time, 6);
			float mix = frames[frame + -5];
			float softness = frames[frame + -4];
			float frameTime = frames[frame];
			float percent = GetCurvePercent(frame / 6 - 1, 1f - (time - frameTime) / (frames[frame + -6] - frameTime));
			if (blend == MixBlend.Setup)
			{
				constraint.mix = constraint.data.mix + (mix + (frames[frame + 1] - mix) * percent - constraint.data.mix) * alpha;
				constraint.softness = constraint.data.softness + (softness + (frames[frame + 2] - softness) * percent - constraint.data.softness) * alpha;
				if (direction == MixDirection.Out)
				{
					constraint.bendDirection = constraint.data.bendDirection;
					constraint.compress = constraint.data.compress;
					constraint.stretch = constraint.data.stretch;
				}
				else
				{
					constraint.bendDirection = (int)frames[frame + -3];
					constraint.compress = frames[frame + -2] != 0f;
					constraint.stretch = frames[frame + -1] != 0f;
				}
			}
			else
			{
				constraint.mix += (mix + (frames[frame + 1] - mix) * percent - constraint.mix) * alpha;
				constraint.softness += (softness + (frames[frame + 2] - softness) * percent - constraint.softness) * alpha;
				if (direction == MixDirection.In)
				{
					constraint.bendDirection = (int)frames[frame + -3];
					constraint.compress = frames[frame + -2] != 0f;
					constraint.stretch = frames[frame + -1] != 0f;
				}
			}
		}
	}
}

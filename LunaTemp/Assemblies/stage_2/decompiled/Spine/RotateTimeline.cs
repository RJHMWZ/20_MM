using System;

namespace Spine
{
	public class RotateTimeline : CurveTimeline, IBoneTimeline
	{
		public const int ENTRIES = 2;

		internal const int PREV_TIME = -2;

		internal const int PREV_ROTATION = -1;

		internal const int ROTATION = 1;

		internal int boneIndex;

		internal float[] frames;

		public override int PropertyId => boneIndex;

		public int BoneIndex
		{
			get
			{
				return boneIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				boneIndex = value;
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

		public RotateTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount << 1];
		}

		public void SetFrame(int frameIndex, float time, float degrees)
		{
			frameIndex <<= 1;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = degrees;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			if (!bone.active)
			{
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				switch (blend)
				{
				case MixBlend.Setup:
					bone.rotation = bone.data.rotation;
					break;
				case MixBlend.First:
				{
					float r3 = bone.data.rotation - bone.rotation;
					bone.rotation += (r3 - (float)((16384 - (int)(16384.499999999996 - (double)(r3 / 360f))) * 360)) * alpha;
					break;
				}
				}
				return;
			}
			if (time >= frames[frames.Length - 2])
			{
				float r2 = frames[frames.Length + -1];
				switch (blend)
				{
				default:
					return;
				case MixBlend.Setup:
					bone.rotation = bone.data.rotation + r2 * alpha;
					return;
				case MixBlend.First:
				case MixBlend.Replace:
					r2 += bone.data.rotation - bone.rotation;
					r2 -= (float)((16384 - (int)(16384.499999999996 - (double)(r2 / 360f))) * 360);
					break;
				case MixBlend.Add:
					break;
				}
				bone.rotation += r2 * alpha;
				return;
			}
			int frame = Animation.BinarySearch(frames, time, 2);
			float prevRotation = frames[frame + -1];
			float frameTime = frames[frame];
			float percent = GetCurvePercent((frame >> 1) - 1, 1f - (time - frameTime) / (frames[frame + -2] - frameTime));
			float r = frames[frame + 1] - prevRotation;
			r = prevRotation + (r - (float)((16384 - (int)(16384.499999999996 - (double)(r / 360f))) * 360)) * percent;
			switch (blend)
			{
			default:
				return;
			case MixBlend.Setup:
				bone.rotation = bone.data.rotation + (r - (float)((16384 - (int)(16384.499999999996 - (double)(r / 360f))) * 360)) * alpha;
				return;
			case MixBlend.First:
			case MixBlend.Replace:
				r += bone.data.rotation - bone.rotation;
				break;
			case MixBlend.Add:
				break;
			}
			bone.rotation += (r - (float)((16384 - (int)(16384.499999999996 - (double)(r / 360f))) * 360)) * alpha;
		}
	}
}

using System;

namespace Spine
{
	public class TranslateTimeline : CurveTimeline, IBoneTimeline
	{
		public const int ENTRIES = 3;

		protected const int PREV_TIME = -3;

		protected const int PREV_X = -2;

		protected const int PREV_Y = -1;

		protected const int X = 1;

		protected const int Y = 2;

		internal int boneIndex;

		internal float[] frames;

		public override int PropertyId => 16777216 + boneIndex;

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

		public TranslateTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
		}

		public void SetFrame(int frameIndex, float time, float x, float y)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = x;
			frames[frameIndex + 2] = y;
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
					bone.x = bone.data.x;
					bone.y = bone.data.y;
					break;
				case MixBlend.First:
					bone.x += (bone.data.x - bone.x) * alpha;
					bone.y += (bone.data.y - bone.y) * alpha;
					break;
				}
				return;
			}
			float x;
			float y;
			if (time >= frames[frames.Length - 3])
			{
				x = frames[frames.Length + -2];
				y = frames[frames.Length + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 3);
				x = frames[frame + -2];
				y = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 3 - 1, 1f - (time - frameTime) / (frames[frame + -3] - frameTime));
				x += (frames[frame + 1] - x) * percent;
				y += (frames[frame + 2] - y) * percent;
			}
			switch (blend)
			{
			case MixBlend.Setup:
				bone.x = bone.data.x + x * alpha;
				bone.y = bone.data.y + y * alpha;
				break;
			case MixBlend.First:
			case MixBlend.Replace:
				bone.x += (bone.data.x + x - bone.x) * alpha;
				bone.y += (bone.data.y + y - bone.y) * alpha;
				break;
			case MixBlend.Add:
				bone.x += x * alpha;
				bone.y += y * alpha;
				break;
			}
		}
	}
}

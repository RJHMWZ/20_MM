using System;

namespace Spine
{
	public class ScaleTimeline : TranslateTimeline, IBoneTimeline
	{
		public override int PropertyId => 33554432 + boneIndex;

		public ScaleTimeline(int frameCount)
			: base(frameCount)
		{
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			if (!bone.active)
			{
				return;
			}
			float[] frames = base.frames;
			if (time < frames[0])
			{
				switch (blend)
				{
				case MixBlend.Setup:
					bone.scaleX = bone.data.scaleX;
					bone.scaleY = bone.data.scaleY;
					break;
				case MixBlend.First:
					bone.scaleX += (bone.data.scaleX - bone.scaleX) * alpha;
					bone.scaleY += (bone.data.scaleY - bone.scaleY) * alpha;
					break;
				}
				return;
			}
			float x;
			float y;
			if (time >= frames[frames.Length - 3])
			{
				x = frames[frames.Length + -2] * bone.data.scaleX;
				y = frames[frames.Length + -1] * bone.data.scaleY;
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 3);
				x = frames[frame + -2];
				y = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 3 - 1, 1f - (time - frameTime) / (frames[frame + -3] - frameTime));
				x = (x + (frames[frame + 1] - x) * percent) * bone.data.scaleX;
				y = (y + (frames[frame + 2] - y) * percent) * bone.data.scaleY;
			}
			if (alpha == 1f)
			{
				if (blend == MixBlend.Add)
				{
					bone.scaleX += x - bone.data.scaleX;
					bone.scaleY += y - bone.data.scaleY;
				}
				else
				{
					bone.scaleX = x;
					bone.scaleY = y;
				}
			}
			else if (direction == MixDirection.Out)
			{
				switch (blend)
				{
				case MixBlend.Setup:
				{
					float bx = bone.data.scaleX;
					float by = bone.data.scaleY;
					bone.scaleX = bx + (Math.Abs(x) * (float)Math.Sign(bx) - bx) * alpha;
					bone.scaleY = by + (Math.Abs(y) * (float)Math.Sign(by) - by) * alpha;
					break;
				}
				case MixBlend.First:
				case MixBlend.Replace:
				{
					float bx = bone.scaleX;
					float by = bone.scaleY;
					bone.scaleX = bx + (Math.Abs(x) * (float)Math.Sign(bx) - bx) * alpha;
					bone.scaleY = by + (Math.Abs(y) * (float)Math.Sign(by) - by) * alpha;
					break;
				}
				case MixBlend.Add:
				{
					float bx = bone.scaleX;
					float by = bone.scaleY;
					bone.scaleX = bx + (Math.Abs(x) * (float)Math.Sign(bx) - bone.data.scaleX) * alpha;
					bone.scaleY = by + (Math.Abs(y) * (float)Math.Sign(by) - bone.data.scaleY) * alpha;
					break;
				}
				}
			}
			else
			{
				switch (blend)
				{
				case MixBlend.Setup:
				{
					float bx = Math.Abs(bone.data.scaleX) * (float)Math.Sign(x);
					float by = Math.Abs(bone.data.scaleY) * (float)Math.Sign(y);
					bone.scaleX = bx + (x - bx) * alpha;
					bone.scaleY = by + (y - by) * alpha;
					break;
				}
				case MixBlend.First:
				case MixBlend.Replace:
				{
					float bx = Math.Abs(bone.scaleX) * (float)Math.Sign(x);
					float by = Math.Abs(bone.scaleY) * (float)Math.Sign(y);
					bone.scaleX = bx + (x - bx) * alpha;
					bone.scaleY = by + (y - by) * alpha;
					break;
				}
				case MixBlend.Add:
				{
					float bx = Math.Sign(x);
					float by = Math.Sign(y);
					bone.scaleX = Math.Abs(bone.scaleX) * bx + (x - Math.Abs(bone.data.scaleX) * bx) * alpha;
					bone.scaleY = Math.Abs(bone.scaleY) * by + (y - Math.Abs(bone.data.scaleY) * by) * alpha;
					break;
				}
				}
			}
		}
	}
}

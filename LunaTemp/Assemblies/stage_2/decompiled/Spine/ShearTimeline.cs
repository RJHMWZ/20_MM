namespace Spine
{
	public class ShearTimeline : TranslateTimeline, IBoneTimeline
	{
		public override int PropertyId => 50331648 + boneIndex;

		public ShearTimeline(int frameCount)
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
					bone.shearX = bone.data.shearX;
					bone.shearY = bone.data.shearY;
					break;
				case MixBlend.First:
					bone.shearX += (bone.data.shearX - bone.shearX) * alpha;
					bone.shearY += (bone.data.shearY - bone.shearY) * alpha;
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
				bone.shearX = bone.data.shearX + x * alpha;
				bone.shearY = bone.data.shearY + y * alpha;
				break;
			case MixBlend.First:
			case MixBlend.Replace:
				bone.shearX += (bone.data.shearX + x - bone.shearX) * alpha;
				bone.shearY += (bone.data.shearY + y - bone.shearY) * alpha;
				break;
			case MixBlend.Add:
				bone.shearX += x * alpha;
				bone.shearY += y * alpha;
				break;
			}
		}
	}
}

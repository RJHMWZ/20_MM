namespace Spine
{
	public class PathConstraintSpacingTimeline : PathConstraintPositionTimeline
	{
		public override int PropertyId => 201326592 + pathConstraintIndex;

		public PathConstraintSpacingTimeline(int frameCount)
			: base(frameCount)
		{
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> events, float alpha, MixBlend blend, MixDirection direction)
		{
			PathConstraint constraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			if (!constraint.active)
			{
				return;
			}
			float[] frames = base.frames;
			if (time < frames[0])
			{
				switch (blend)
				{
				case MixBlend.Setup:
					constraint.spacing = constraint.data.spacing;
					break;
				case MixBlend.First:
					constraint.spacing += (constraint.data.spacing - constraint.spacing) * alpha;
					break;
				}
				return;
			}
			float spacing;
			if (time >= frames[frames.Length - 2])
			{
				spacing = frames[frames.Length + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 2);
				spacing = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 2 - 1, 1f - (time - frameTime) / (frames[frame + -2] - frameTime));
				spacing += (frames[frame + 1] - spacing) * percent;
			}
			if (blend == MixBlend.Setup)
			{
				constraint.spacing = constraint.data.spacing + (spacing - constraint.data.spacing) * alpha;
			}
			else
			{
				constraint.spacing += (spacing - constraint.spacing) * alpha;
			}
		}
	}
}

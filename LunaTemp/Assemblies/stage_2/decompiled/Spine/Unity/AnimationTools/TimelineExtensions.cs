using UnityEngine;

namespace Spine.Unity.AnimationTools
{
	public static class TimelineExtensions
	{
		public static Vector2 Evaluate(this TranslateTimeline timeline, float time, SkeletonData skeletonData = null)
		{
			float[] frames = timeline.frames;
			if (time < frames[0])
			{
				return Vector2.zero;
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
				float percent = timeline.GetCurvePercent(frame / 3 - 1, 1f - (time - frameTime) / (frames[frame + -3] - frameTime));
				x += (frames[frame + 1] - x) * percent;
				y += (frames[frame + 2] - y) * percent;
			}
			Vector2 xy = new Vector2(x, y);
			if (skeletonData == null)
			{
				return xy;
			}
			BoneData boneData = skeletonData.bones.Items[timeline.boneIndex];
			return xy + new Vector2(boneData.x, boneData.y);
		}

		public static TranslateTimeline FindTranslateTimelineForBone(this Animation a, int boneIndex)
		{
			foreach (Timeline timeline in a.timelines)
			{
				if (timeline.GetType().IsSubclassOf(typeof(TranslateTimeline)) || !(timeline is TranslateTimeline translateTimeline) || translateTimeline.boneIndex != boneIndex)
				{
					continue;
				}
				return translateTimeline;
			}
			return null;
		}
	}
}

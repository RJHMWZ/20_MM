using UnityEngine;

namespace Spine.Unity
{
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonRootMotion")]
	public class SkeletonRootMotion : SkeletonRootMotionBase
	{
		private const int DefaultAnimationTrackFlags = -1;

		public int animationTrackFlags = -1;

		private AnimationState animationState;

		private Canvas canvas;

		protected override float AdditionalScale => canvas ? canvas.referencePixelsPerUnit : 1f;

		public override Vector2 GetRemainingRootMotion(int trackIndex)
		{
			TrackEntry track = animationState.GetCurrent(trackIndex);
			if (track == null)
			{
				return Vector2.zero;
			}
			Animation animation = track.Animation;
			float start = track.AnimationTime;
			float end = animation.duration;
			return GetAnimationRootMotion(start, end, animation);
		}

		public override RootMotionInfo GetRootMotionInfo(int trackIndex)
		{
			TrackEntry track = animationState.GetCurrent(trackIndex);
			if (track == null)
			{
				return default(RootMotionInfo);
			}
			Animation animation = track.Animation;
			float time = track.AnimationTime;
			return GetAnimationRootMotionInfo(track.Animation, time);
		}

		protected override void Reset()
		{
			base.Reset();
			animationTrackFlags = -1;
		}

		protected override void Start()
		{
			base.Start();
			animationState = (skeletonComponent as IAnimationStateComponent)?.AnimationState;
			if (GetComponent<CanvasRenderer>() != null)
			{
				canvas = GetComponentInParent<Canvas>();
			}
		}

		protected override Vector2 CalculateAnimationsMovementDelta()
		{
			Vector2 localDelta = Vector2.zero;
			int trackCount = animationState.Tracks.Count;
			for (int trackIndex = 0; trackIndex < trackCount; trackIndex++)
			{
				if (animationTrackFlags != -1 && (animationTrackFlags & (1 << trackIndex)) == 0)
				{
					continue;
				}
				TrackEntry track = animationState.GetCurrent(trackIndex);
				TrackEntry next = null;
				while (track != null)
				{
					Animation animation = track.Animation;
					float start = track.animationLast;
					float end = track.AnimationTime;
					Vector2 currentDelta = GetAnimationRootMotion(start, end, animation);
					if (currentDelta != Vector2.zero)
					{
						ApplyMixAlphaToDelta(ref currentDelta, next, track);
						localDelta += currentDelta;
					}
					next = track;
					track = track.mixingFrom;
				}
			}
			return localDelta;
		}

		private void ApplyMixAlphaToDelta(ref Vector2 currentDelta, TrackEntry next, TrackEntry track)
		{
			float mix;
			if (next != null)
			{
				if (next.mixDuration == 0f)
				{
					mix = 1f;
				}
				else
				{
					mix = next.mixTime / next.mixDuration;
					if (mix > 1f)
					{
						mix = 1f;
					}
				}
				float mixAndAlpha = track.alpha * next.interruptAlpha * (1f - mix);
				currentDelta *= mixAndAlpha;
				return;
			}
			if (track.mixDuration == 0f)
			{
				mix = 1f;
			}
			else
			{
				mix = track.alpha * (track.mixTime / track.mixDuration);
				if (mix > 1f)
				{
					mix = 1f;
				}
			}
			currentDelta *= mix;
		}
	}
}

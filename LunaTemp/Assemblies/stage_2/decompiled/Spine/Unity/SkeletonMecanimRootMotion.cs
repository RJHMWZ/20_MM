using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonMecanimRootMotion")]
	public class SkeletonMecanimRootMotion : SkeletonRootMotionBase
	{
		private const int DefaultMecanimLayerFlags = -1;

		public int mecanimLayerFlags = -1;

		protected Vector2 movementDelta;

		private SkeletonMecanim skeletonMecanim;

		public SkeletonMecanim SkeletonMecanim => skeletonMecanim ? skeletonMecanim : (skeletonMecanim = GetComponent<SkeletonMecanim>());

		public override Vector2 GetRemainingRootMotion(int layerIndex)
		{
			KeyValuePair<Animation, float> pair = skeletonMecanim.Translator.GetActiveAnimationAndTime(layerIndex);
			Animation animation = pair.Key;
			float time = pair.Value;
			if (animation == null)
			{
				return Vector2.zero;
			}
			float start = time;
			float end = animation.duration;
			return GetAnimationRootMotion(start, end, animation);
		}

		public override RootMotionInfo GetRootMotionInfo(int layerIndex)
		{
			KeyValuePair<Animation, float> pair = skeletonMecanim.Translator.GetActiveAnimationAndTime(layerIndex);
			Animation animation = pair.Key;
			float time = pair.Value;
			if (animation == null)
			{
				return default(RootMotionInfo);
			}
			return GetAnimationRootMotionInfo(animation, time);
		}

		protected override void Reset()
		{
			base.Reset();
			mecanimLayerFlags = -1;
		}

		protected override void Start()
		{
			base.Start();
			skeletonMecanim = GetComponent<SkeletonMecanim>();
			if ((bool)skeletonMecanim)
			{
				skeletonMecanim.Translator.OnClipApplied -= OnClipApplied;
				skeletonMecanim.Translator.OnClipApplied += OnClipApplied;
			}
		}

		private void OnClipApplied(Animation animation, int layerIndex, float weight, float time, float lastTime, bool playsBackward)
		{
			if ((mecanimLayerFlags & (1 << layerIndex)) != 0 && weight != 0f)
			{
				if (!playsBackward)
				{
					movementDelta += weight * GetAnimationRootMotion(lastTime, time, animation);
				}
				else
				{
					movementDelta -= weight * GetAnimationRootMotion(time, lastTime, animation);
				}
			}
		}

		protected override Vector2 CalculateAnimationsMovementDelta()
		{
			Vector2 result = movementDelta;
			movementDelta = Vector2.zero;
			return result;
		}
	}
}

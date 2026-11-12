using System;
using System.Collections.Generic;
using Spine.Unity.AnimationTools;
using UnityEngine;

namespace Spine.Unity
{
	public abstract class SkeletonRootMotionBase : MonoBehaviour
	{
		public struct RootMotionInfo
		{
			public Vector2 start;

			public Vector2 current;

			public Vector2 mid;

			public Vector2 end;

			public bool timeIsPastMid;
		}

		[SpineBone("", "", true, false)]
		[SerializeField]
		protected string rootMotionBoneName = "root";

		public bool transformPositionX = true;

		public bool transformPositionY = true;

		public float rootMotionScaleX = 1f;

		public float rootMotionScaleY = 1f;

		public float rootMotionTranslateXPerY = 0f;

		public float rootMotionTranslateYPerX = 0f;

		[Header("Optional")]
		public Rigidbody2D rigidBody2D;

		public Rigidbody rigidBody;

		protected ISkeletonComponent skeletonComponent;

		protected Bone rootMotionBone;

		protected int rootMotionBoneIndex;

		protected List<Bone> topLevelBones = new List<Bone>();

		protected Vector2 initialOffset = Vector2.zero;

		protected Vector2 tempSkeletonDisplacement;

		protected Vector2 rigidbodyDisplacement;

		public bool UsesRigidbody => rigidBody != null || rigidBody2D != null;

		protected virtual float AdditionalScale => 1f;

		protected virtual void Reset()
		{
			FindRigidbodyComponent();
		}

		protected virtual void Start()
		{
			skeletonComponent = GetComponent<ISkeletonComponent>();
			GatherTopLevelBones();
			SetRootMotionBone(rootMotionBoneName);
			if (rootMotionBone != null)
			{
				initialOffset = new Vector2(rootMotionBone.x, rootMotionBone.y);
			}
			if (skeletonComponent is ISkeletonAnimation skeletonAnimation)
			{
				skeletonAnimation.UpdateLocal -= HandleUpdateLocal;
				skeletonAnimation.UpdateLocal += HandleUpdateLocal;
			}
		}

		protected virtual void FixedUpdate()
		{
			if (base.isActiveAndEnabled)
			{
				if (rigidBody2D != null)
				{
					rigidBody2D.MovePosition(new Vector2(base.transform.position.x, base.transform.position.y) + rigidbodyDisplacement);
				}
				if (rigidBody != null)
				{
					rigidBody.MovePosition(base.transform.position + new Vector3(rigidbodyDisplacement.x, rigidbodyDisplacement.y, 0f));
				}
				GetScaleAffectingRootMotion(out var parentBoneScale);
				ClearEffectiveBoneOffsets(parentBoneScale);
				rigidbodyDisplacement = Vector2.zero;
				tempSkeletonDisplacement = Vector2.zero;
			}
		}

		protected virtual void OnDisable()
		{
			rigidbodyDisplacement = Vector2.zero;
			tempSkeletonDisplacement = Vector2.zero;
		}

		protected void FindRigidbodyComponent()
		{
			rigidBody2D = GetComponent<Rigidbody2D>();
			if (!rigidBody2D)
			{
				rigidBody = GetComponent<Rigidbody>();
			}
			if (!rigidBody2D && !rigidBody)
			{
				rigidBody2D = GetComponentInParent<Rigidbody2D>();
				if (!rigidBody2D)
				{
					rigidBody = GetComponentInParent<Rigidbody>();
				}
			}
		}

		protected abstract Vector2 CalculateAnimationsMovementDelta();

		public abstract Vector2 GetRemainingRootMotion(int trackIndex = 0);

		public abstract RootMotionInfo GetRootMotionInfo(int trackIndex = 0);

		public void SetRootMotionBone(string name)
		{
			Skeleton skeleton = skeletonComponent.Skeleton;
			int index = skeleton.FindBoneIndex(name);
			if (index >= 0)
			{
				rootMotionBoneIndex = index;
				rootMotionBone = skeleton.bones.Items[index];
			}
			else
			{
				Debug.Log("Bone named \"" + name + "\" could not be found.");
				rootMotionBoneIndex = 0;
				rootMotionBone = skeleton.RootBone;
			}
		}

		public void AdjustRootMotionToDistance(Vector2 distanceToTarget, int trackIndex = 0, bool adjustX = true, bool adjustY = true, float minX = 0f, float maxX = float.MaxValue, float minY = 0f, float maxY = float.MaxValue, bool allowXTranslation = false, bool allowYTranslation = false)
		{
			Vector2 distanceToTargetSkeletonSpace = base.transform.InverseTransformVector(distanceToTarget);
			Vector2 scaleAffectingRootMotion = GetScaleAffectingRootMotion();
			if (UsesRigidbody)
			{
				distanceToTargetSkeletonSpace -= tempSkeletonDisplacement;
			}
			Vector2 remainingRootMotionSkeletonSpace = GetRemainingRootMotion(trackIndex);
			remainingRootMotionSkeletonSpace.Scale(scaleAffectingRootMotion);
			if (remainingRootMotionSkeletonSpace.x == 0f)
			{
				remainingRootMotionSkeletonSpace.x = 0.0001f;
			}
			if (remainingRootMotionSkeletonSpace.y == 0f)
			{
				remainingRootMotionSkeletonSpace.y = 0.0001f;
			}
			if (adjustX)
			{
				rootMotionScaleX = Math.Min(maxX, Math.Max(minX, distanceToTargetSkeletonSpace.x / remainingRootMotionSkeletonSpace.x));
			}
			if (adjustY)
			{
				rootMotionScaleY = Math.Min(maxY, Math.Max(minY, distanceToTargetSkeletonSpace.y / remainingRootMotionSkeletonSpace.y));
			}
			if (allowXTranslation)
			{
				rootMotionTranslateXPerY = (distanceToTargetSkeletonSpace.x - remainingRootMotionSkeletonSpace.x * rootMotionScaleX) / remainingRootMotionSkeletonSpace.y;
			}
			if (allowYTranslation)
			{
				rootMotionTranslateYPerX = (distanceToTargetSkeletonSpace.y - remainingRootMotionSkeletonSpace.y * rootMotionScaleY) / remainingRootMotionSkeletonSpace.x;
			}
		}

		public Vector2 GetAnimationRootMotion(Animation animation)
		{
			return GetAnimationRootMotion(0f, animation.duration, animation);
		}

		public Vector2 GetAnimationRootMotion(float startTime, float endTime, Animation animation)
		{
			TranslateTimeline timeline = animation.FindTranslateTimelineForBone(rootMotionBoneIndex);
			if (timeline != null)
			{
				return GetTimelineMovementDelta(startTime, endTime, timeline, animation);
			}
			return Vector2.zero;
		}

		public RootMotionInfo GetAnimationRootMotionInfo(Animation animation, float currentTime)
		{
			RootMotionInfo rootMotion = default(RootMotionInfo);
			TranslateTimeline timeline = animation.FindTranslateTimelineForBone(rootMotionBoneIndex);
			if (timeline != null)
			{
				float duration = animation.duration;
				float mid = duration * 0.5f;
				rootMotion.start = timeline.Evaluate(0f);
				rootMotion.current = timeline.Evaluate(currentTime);
				rootMotion.mid = timeline.Evaluate(mid);
				rootMotion.end = timeline.Evaluate(duration);
				rootMotion.timeIsPastMid = currentTime > mid;
			}
			return rootMotion;
		}

		private Vector2 GetTimelineMovementDelta(float startTime, float endTime, TranslateTimeline timeline, Animation animation)
		{
			if (startTime > endTime)
			{
				return timeline.Evaluate(animation.duration) - timeline.Evaluate(startTime) + (timeline.Evaluate(endTime) - timeline.Evaluate(0f));
			}
			if (startTime != endTime)
			{
				return timeline.Evaluate(endTime) - timeline.Evaluate(startTime);
			}
			return Vector2.zero;
		}

		private void GatherTopLevelBones()
		{
			topLevelBones.Clear();
			Skeleton skeleton = skeletonComponent.Skeleton;
			foreach (Bone bone in skeleton.Bones)
			{
				if (bone.Parent == null)
				{
					topLevelBones.Add(bone);
				}
			}
		}

		private void HandleUpdateLocal(ISkeletonAnimation animatedSkeletonComponent)
		{
			if (base.isActiveAndEnabled)
			{
				Vector2 boneLocalDelta = CalculateAnimationsMovementDelta();
				Vector2 parentBoneScale;
				Vector2 skeletonDelta = GetSkeletonSpaceMovementDelta(boneLocalDelta, out parentBoneScale);
				ApplyRootMotion(skeletonDelta, parentBoneScale);
			}
		}

		private void ApplyRootMotion(Vector2 skeletonDelta, Vector2 parentBoneScale)
		{
			if (UsesRigidbody)
			{
				rigidbodyDisplacement += (Vector2)base.transform.TransformVector(skeletonDelta);
				tempSkeletonDisplacement += skeletonDelta;
				SetEffectiveBoneOffsetsTo(tempSkeletonDisplacement, parentBoneScale);
			}
			else
			{
				base.transform.position += base.transform.TransformVector(skeletonDelta);
				ClearEffectiveBoneOffsets(parentBoneScale);
			}
		}

		private Vector2 GetScaleAffectingRootMotion()
		{
			Vector2 parentBoneScale;
			return GetScaleAffectingRootMotion(out parentBoneScale);
		}

		private Vector2 GetScaleAffectingRootMotion(out Vector2 parentBoneScale)
		{
			Skeleton skeleton = skeletonComponent.Skeleton;
			Vector2 totalScale = Vector2.one;
			totalScale.x *= skeleton.ScaleX;
			totalScale.y *= skeleton.ScaleY;
			parentBoneScale = Vector2.one;
			Bone scaleBone = rootMotionBone;
			while ((scaleBone = scaleBone.parent) != null)
			{
				parentBoneScale.x *= scaleBone.ScaleX;
				parentBoneScale.y *= scaleBone.ScaleY;
			}
			totalScale = Vector2.Scale(totalScale, parentBoneScale);
			return totalScale * AdditionalScale;
		}

		private Vector2 GetSkeletonSpaceMovementDelta(Vector2 boneLocalDelta, out Vector2 parentBoneScale)
		{
			Vector2 skeletonDelta = boneLocalDelta;
			Vector2 totalScale = GetScaleAffectingRootMotion(out parentBoneScale);
			skeletonDelta.Scale(totalScale);
			Vector2 rootMotionTranslation = new Vector2(rootMotionTranslateXPerY * skeletonDelta.y, rootMotionTranslateYPerX * skeletonDelta.x);
			skeletonDelta.x *= rootMotionScaleX;
			skeletonDelta.y *= rootMotionScaleY;
			skeletonDelta.x += rootMotionTranslation.x;
			skeletonDelta.y += rootMotionTranslation.y;
			if (!transformPositionX)
			{
				skeletonDelta.x = 0f;
			}
			if (!transformPositionY)
			{
				skeletonDelta.y = 0f;
			}
			return skeletonDelta;
		}

		private void SetEffectiveBoneOffsetsTo(Vector2 displacementSkeletonSpace, Vector2 parentBoneScale)
		{
			Skeleton skeleton = skeletonComponent.Skeleton;
			foreach (Bone topLevelBone in topLevelBones)
			{
				if (topLevelBone == rootMotionBone)
				{
					if (transformPositionX)
					{
						topLevelBone.x = displacementSkeletonSpace.x / skeleton.ScaleX;
					}
					if (transformPositionY)
					{
						topLevelBone.y = displacementSkeletonSpace.y / skeleton.ScaleY;
					}
					continue;
				}
				float offsetX = (initialOffset.x - rootMotionBone.x) * parentBoneScale.x;
				float offsetY = (initialOffset.y - rootMotionBone.y) * parentBoneScale.y;
				if (transformPositionX)
				{
					topLevelBone.x = displacementSkeletonSpace.x / skeleton.ScaleX + offsetX;
				}
				if (transformPositionY)
				{
					topLevelBone.y = displacementSkeletonSpace.y / skeleton.ScaleY + offsetY;
				}
			}
		}

		private void ClearEffectiveBoneOffsets(Vector2 parentBoneScale)
		{
			SetEffectiveBoneOffsetsTo(Vector2.zero, parentBoneScale);
		}
	}
}

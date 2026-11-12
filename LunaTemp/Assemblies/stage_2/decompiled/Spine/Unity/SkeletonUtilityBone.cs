using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Spine/SkeletonUtilityBone")]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonUtilityBone")]
	public class SkeletonUtilityBone : MonoBehaviour
	{
		public enum Mode
		{
			Follow,
			Override
		}

		public enum UpdatePhase
		{
			Local,
			World,
			Complete
		}

		public string boneName;

		public Transform parentReference;

		public Mode mode;

		public bool position;

		public bool rotation;

		public bool scale;

		public bool zPosition = true;

		[Range(0f, 1f)]
		public float overrideAlpha = 1f;

		public SkeletonUtility hierarchy;

		[NonSerialized]
		public Bone bone;

		[NonSerialized]
		public bool transformLerpComplete;

		[NonSerialized]
		public bool valid;

		private Transform cachedTransform;

		private Transform skeletonTransform;

		private bool incompatibleTransformMode;

		public bool IncompatibleTransformMode => incompatibleTransformMode;

		public void Reset()
		{
			bone = null;
			cachedTransform = base.transform;
			valid = hierarchy != null && hierarchy.IsValid;
			if (valid)
			{
				skeletonTransform = hierarchy.transform;
				hierarchy.OnReset -= HandleOnReset;
				hierarchy.OnReset += HandleOnReset;
				DoUpdate(UpdatePhase.Local);
			}
		}

		private void OnEnable()
		{
			if (hierarchy == null)
			{
				hierarchy = base.transform.GetComponentInParent<SkeletonUtility>();
			}
			if (!(hierarchy == null))
			{
				hierarchy.RegisterBone(this);
				hierarchy.OnReset += HandleOnReset;
			}
		}

		private void HandleOnReset()
		{
			Reset();
		}

		private void OnDisable()
		{
			if (hierarchy != null)
			{
				hierarchy.OnReset -= HandleOnReset;
				hierarchy.UnregisterBone(this);
			}
		}

		public void DoUpdate(UpdatePhase phase)
		{
			if (!valid)
			{
				Reset();
				return;
			}
			Skeleton skeleton = hierarchy.Skeleton;
			if (bone == null)
			{
				if (string.IsNullOrEmpty(boneName))
				{
					return;
				}
				bone = skeleton.FindBone(boneName);
				if (bone == null)
				{
					Debug.LogError("Bone not found: " + boneName, this);
					return;
				}
			}
			if (!bone.Active)
			{
				return;
			}
			float positionScale = hierarchy.PositionScale;
			Transform thisTransform = cachedTransform;
			float skeletonFlipRotation = Mathf.Sign(skeleton.ScaleX * skeleton.ScaleY);
			if (mode == Mode.Follow)
			{
				switch (phase)
				{
				case UpdatePhase.Local:
					if (position)
					{
						thisTransform.localPosition = new Vector3(bone.x * positionScale, bone.y * positionScale, 0f);
					}
					if (rotation)
					{
						if (bone.data.transformMode.InheritsRotation())
						{
							thisTransform.localRotation = Quaternion.Euler(0f, 0f, bone.rotation);
						}
						else
						{
							Vector3 euler = skeletonTransform.rotation.eulerAngles;
							thisTransform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z + bone.WorldRotationX * skeletonFlipRotation);
						}
					}
					if (scale)
					{
						thisTransform.localScale = new Vector3(bone.scaleX, bone.scaleY, 1f);
						incompatibleTransformMode = BoneTransformModeIncompatible(bone);
					}
					break;
				case UpdatePhase.World:
				case UpdatePhase.Complete:
					if (!bone.appliedValid)
					{
						bone.UpdateAppliedTransform();
					}
					if (position)
					{
						thisTransform.localPosition = new Vector3(bone.ax * positionScale, bone.ay * positionScale, 0f);
					}
					if (rotation)
					{
						if (bone.data.transformMode.InheritsRotation())
						{
							thisTransform.localRotation = Quaternion.Euler(0f, 0f, bone.AppliedRotation);
						}
						else
						{
							Vector3 euler2 = skeletonTransform.rotation.eulerAngles;
							thisTransform.rotation = Quaternion.Euler(euler2.x, euler2.y, euler2.z + bone.WorldRotationX * skeletonFlipRotation);
						}
					}
					if (scale)
					{
						thisTransform.localScale = new Vector3(bone.ascaleX, bone.ascaleY, 1f);
						incompatibleTransformMode = BoneTransformModeIncompatible(bone);
					}
					break;
				}
			}
			else
			{
				if (mode != Mode.Override || transformLerpComplete)
				{
					return;
				}
				if (parentReference == null)
				{
					if (position)
					{
						Vector3 clp = thisTransform.localPosition / positionScale;
						bone.x = Mathf.Lerp(bone.x, clp.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, clp.y, overrideAlpha);
					}
					if (rotation)
					{
						float angle2 = Mathf.LerpAngle(bone.Rotation, thisTransform.localRotation.eulerAngles.z, overrideAlpha);
						bone.Rotation = angle2;
						bone.AppliedRotation = angle2;
					}
					if (scale)
					{
						Vector3 cls2 = thisTransform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, cls2.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, cls2.y, overrideAlpha);
					}
				}
				else
				{
					if (transformLerpComplete)
					{
						return;
					}
					if (position)
					{
						Vector3 pos = parentReference.InverseTransformPoint(thisTransform.position) / positionScale;
						bone.x = Mathf.Lerp(bone.x, pos.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, pos.y, overrideAlpha);
					}
					if (rotation)
					{
						float angle = Mathf.LerpAngle(bone.Rotation, Quaternion.LookRotation(Vector3.forward, parentReference.InverseTransformDirection(thisTransform.up)).eulerAngles.z, overrideAlpha);
						bone.Rotation = angle;
						bone.AppliedRotation = angle;
					}
					if (scale)
					{
						Vector3 cls = thisTransform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, cls.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, cls.y, overrideAlpha);
					}
					incompatibleTransformMode = BoneTransformModeIncompatible(bone);
				}
				transformLerpComplete = true;
			}
		}

		public static bool BoneTransformModeIncompatible(Bone bone)
		{
			return !bone.data.transformMode.InheritsScale();
		}

		public void AddBoundingBox(string skinName, string slotName, string attachmentName)
		{
			SkeletonUtility.AddBoneRigidbody2D(base.transform.gameObject);
			SkeletonUtility.AddBoundingBoxGameObject(bone.skeleton, skinName, slotName, attachmentName, base.transform);
		}
	}
}

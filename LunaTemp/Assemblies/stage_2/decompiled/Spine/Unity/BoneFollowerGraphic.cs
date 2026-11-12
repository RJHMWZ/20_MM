using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Spine/UI/BoneFollowerGraphic")]
	[HelpURL("http://esotericsoftware.com/spine-unity#BoneFollowerGraphic")]
	public class BoneFollowerGraphic : MonoBehaviour
	{
		public SkeletonGraphic skeletonGraphic;

		public bool initializeOnAwake = true;

		[SpineBone("", "skeletonGraphic", true, false)]
		public string boneName;

		public bool followBoneRotation = true;

		[Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
		public bool followSkeletonFlip = true;

		[Tooltip("Follows the target bone's local scale. BoneFollower cannot inherit world/skewed scale because of UnityEngine.Transform property limitations.")]
		public bool followLocalScale = false;

		public bool followXYPosition = true;

		public bool followZPosition = true;

		[Tooltip("Applies when 'Follow Skeleton Flip' is disabled but 'Follow Bone Rotation' is enabled. When flipping the skeleton by scaling its Transform, this follower's rotation is adjusted instead of its scale to follow the bone orientation. When one of the axes is flipped,  only one axis can be followed, either the X or the Y axis, which is selected here.")]
		public BoneFollower.AxisOrientation maintainedAxisOrientation = BoneFollower.AxisOrientation.XAxis;

		[NonSerialized]
		public Bone bone;

		private Transform skeletonTransform;

		private bool skeletonTransformIsParent;

		[NonSerialized]
		public bool valid;

		public SkeletonGraphic SkeletonGraphic
		{
			get
			{
				return skeletonGraphic;
			}
			set
			{
				skeletonGraphic = value;
				Initialize();
			}
		}

		public bool SetBone(string name)
		{
			bone = skeletonGraphic.Skeleton.FindBone(name);
			if (bone == null)
			{
				Debug.LogError("Bone not found: " + name, this);
				return false;
			}
			boneName = name;
			return true;
		}

		public void Awake()
		{
			if (initializeOnAwake)
			{
				Initialize();
			}
		}

		public void Initialize()
		{
			bone = null;
			valid = skeletonGraphic != null && skeletonGraphic.IsValid;
			if (valid)
			{
				skeletonTransform = skeletonGraphic.transform;
				skeletonTransformIsParent = (object)skeletonTransform == base.transform.parent;
				if (!string.IsNullOrEmpty(boneName))
				{
					bone = skeletonGraphic.Skeleton.FindBone(boneName);
				}
			}
		}

		public void LateUpdate()
		{
			if (!valid)
			{
				Initialize();
				return;
			}
			if (bone == null)
			{
				if (string.IsNullOrEmpty(boneName))
				{
					return;
				}
				bone = skeletonGraphic.Skeleton.FindBone(boneName);
				if (!SetBone(boneName))
				{
					return;
				}
			}
			RectTransform thisTransform = base.transform as RectTransform;
			if (thisTransform == null)
			{
				return;
			}
			Canvas canvas = skeletonGraphic.canvas;
			if (canvas == null)
			{
				canvas = skeletonGraphic.GetComponentInParent<Canvas>();
			}
			float scale = ((canvas != null) ? canvas.referencePixelsPerUnit : 100f);
			float additionalFlipScale = 1f;
			if (skeletonTransformIsParent)
			{
				thisTransform.localPosition = new Vector3(followXYPosition ? (bone.worldX * scale) : thisTransform.localPosition.x, followXYPosition ? (bone.worldY * scale) : thisTransform.localPosition.y, followZPosition ? 0f : thisTransform.localPosition.z);
				if (followBoneRotation)
				{
					thisTransform.localRotation = bone.GetQuaternion();
				}
			}
			else
			{
				Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.worldX * scale, bone.worldY * scale, 0f));
				if (!followZPosition)
				{
					targetWorldPosition.z = thisTransform.position.z;
				}
				if (!followXYPosition)
				{
					targetWorldPosition.x = thisTransform.position.x;
					targetWorldPosition.y = thisTransform.position.y;
				}
				Vector3 skeletonLossyScale = skeletonTransform.lossyScale;
				Transform transformParent = thisTransform.parent;
				Vector3 parentLossyScale = ((transformParent != null) ? transformParent.lossyScale : Vector3.one);
				if (followBoneRotation)
				{
					float boneWorldRotation = bone.WorldRotationX;
					if (skeletonLossyScale.x * skeletonLossyScale.y < 0f)
					{
						boneWorldRotation = 0f - boneWorldRotation;
					}
					if (followSkeletonFlip || maintainedAxisOrientation == BoneFollower.AxisOrientation.XAxis)
					{
						if (skeletonLossyScale.x * parentLossyScale.x < 0f)
						{
							boneWorldRotation += 180f;
						}
					}
					else if (skeletonLossyScale.y * parentLossyScale.y < 0f)
					{
						boneWorldRotation += 180f;
					}
					Vector3 worldRotation = skeletonTransform.rotation.eulerAngles;
					if (followLocalScale && bone.scaleX < 0f)
					{
						boneWorldRotation += 180f;
					}
					thisTransform.SetPositionAndRotation(targetWorldPosition, Quaternion.Euler(worldRotation.x, worldRotation.y, worldRotation.z + boneWorldRotation));
				}
				else
				{
					thisTransform.position = targetWorldPosition;
				}
				additionalFlipScale = Mathf.Sign(skeletonLossyScale.x * parentLossyScale.x * skeletonLossyScale.y * parentLossyScale.y);
			}
			Vector3 localScale = (followLocalScale ? new Vector3(bone.scaleX, bone.scaleY, 1f) : new Vector3(1f, 1f, 1f));
			if (followSkeletonFlip)
			{
				localScale.y *= Mathf.Sign(bone.skeleton.ScaleX * bone.skeleton.ScaleY) * additionalFlipScale;
			}
			thisTransform.localScale = localScale;
		}
	}
}

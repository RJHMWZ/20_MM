using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Spine/Point Follower")]
	[HelpURL("http://esotericsoftware.com/spine-unity#PointFollower")]
	public class PointFollower : MonoBehaviour, IHasSkeletonRenderer, IHasSkeletonComponent
	{
		public SkeletonRenderer skeletonRenderer;

		[SpineSlot("", "skeletonRenderer", false, true, false)]
		public string slotName;

		[SpineAttachment(true, false, false, "slotName", "skeletonRenderer", "", true, true)]
		public string pointAttachmentName;

		public bool followRotation = true;

		public bool followSkeletonFlip = true;

		public bool followSkeletonZPosition = false;

		private Transform skeletonTransform;

		private bool skeletonTransformIsParent;

		private PointAttachment point;

		private Bone bone;

		private bool valid;

		public SkeletonRenderer SkeletonRenderer => skeletonRenderer;

		public ISkeletonComponent SkeletonComponent => skeletonRenderer;

		public bool IsValid => valid;

		public void Initialize()
		{
			valid = skeletonRenderer != null && skeletonRenderer.valid;
			if (valid)
			{
				UpdateReferences();
			}
		}

		private void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer)
		{
			Initialize();
		}

		private void UpdateReferences()
		{
			skeletonTransform = skeletonRenderer.transform;
			skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
			skeletonRenderer.OnRebuild += HandleRebuildRenderer;
			skeletonTransformIsParent = (object)skeletonTransform == base.transform.parent;
			bone = null;
			point = null;
			if (!string.IsNullOrEmpty(pointAttachmentName))
			{
				Skeleton skeleton = skeletonRenderer.Skeleton;
				int slotIndex = skeleton.FindSlotIndex(slotName);
				if (slotIndex >= 0)
				{
					Slot slot = skeleton.slots.Items[slotIndex];
					bone = slot.bone;
					point = skeleton.GetAttachment(slotIndex, pointAttachmentName) as PointAttachment;
				}
			}
		}

		private void OnDestroy()
		{
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
			}
		}

		public void LateUpdate()
		{
			if (point == null)
			{
				if (string.IsNullOrEmpty(pointAttachmentName))
				{
					return;
				}
				UpdateReferences();
				if (point == null)
				{
					return;
				}
			}
			Vector2 worldPos = default(Vector2);
			point.ComputeWorldPosition(bone, out worldPos.x, out worldPos.y);
			float rotation = point.ComputeWorldRotation(bone);
			Transform thisTransform = base.transform;
			if (skeletonTransformIsParent)
			{
				thisTransform.localPosition = new Vector3(worldPos.x, worldPos.y, followSkeletonZPosition ? 0f : thisTransform.localPosition.z);
				if (followRotation)
				{
					float halfRotation = rotation * 0.5f * (3.14159265f / 180f);
					Quaternion q = default(Quaternion);
					q.z = Mathf.Sin(halfRotation);
					q.w = Mathf.Cos(halfRotation);
					thisTransform.localRotation = q;
				}
			}
			else
			{
				Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(worldPos.x, worldPos.y, 0f));
				if (!followSkeletonZPosition)
				{
					targetWorldPosition.z = thisTransform.position.z;
				}
				Transform transformParent = thisTransform.parent;
				if (transformParent != null)
				{
					Matrix4x4 i = transformParent.localToWorldMatrix;
					if (i.m00 * i.m11 - i.m01 * i.m10 < 0f)
					{
						rotation = 0f - rotation;
					}
				}
				if (followRotation)
				{
					Vector3 transformWorldRotation = skeletonTransform.rotation.eulerAngles;
					thisTransform.SetPositionAndRotation(targetWorldPosition, Quaternion.Euler(transformWorldRotation.x, transformWorldRotation.y, transformWorldRotation.z + rotation));
				}
				else
				{
					thisTransform.position = targetWorldPosition;
				}
			}
			if (followSkeletonFlip)
			{
				Vector3 localScale = thisTransform.localScale;
				localScale.y = Mathf.Abs(localScale.y) * Mathf.Sign(bone.skeleton.ScaleX * bone.skeleton.ScaleY);
				thisTransform.localScale = localScale;
			}
		}
	}
}

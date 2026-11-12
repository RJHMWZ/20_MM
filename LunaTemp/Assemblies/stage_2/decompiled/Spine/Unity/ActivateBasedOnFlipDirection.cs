using UnityEngine;

namespace Spine.Unity
{
	public class ActivateBasedOnFlipDirection : MonoBehaviour
	{
		public SkeletonRenderer skeletonRenderer;

		public SkeletonGraphic skeletonGraphic;

		public GameObject activeOnNormalX;

		public GameObject activeOnFlippedX;

		private HingeJoint2D[] jointsNormalX;

		private HingeJoint2D[] jointsFlippedX;

		private ISkeletonComponent skeletonComponent;

		private bool wasFlippedXBefore = false;

		private void Start()
		{
			jointsNormalX = activeOnNormalX.GetComponentsInChildren<HingeJoint2D>();
			jointsFlippedX = activeOnFlippedX.GetComponentsInChildren<HingeJoint2D>();
			ISkeletonComponent obj;
			if (!(skeletonRenderer != null))
			{
				ISkeletonComponent skeletonComponent = skeletonGraphic;
				obj = skeletonComponent;
			}
			else
			{
				ISkeletonComponent skeletonComponent = skeletonRenderer;
				obj = skeletonComponent;
			}
			this.skeletonComponent = obj;
		}

		private void FixedUpdate()
		{
			bool isFlippedX = skeletonComponent.Skeleton.ScaleX < 0f;
			if (isFlippedX != wasFlippedXBefore)
			{
				HandleFlip(isFlippedX);
			}
			wasFlippedXBefore = isFlippedX;
		}

		private void HandleFlip(bool isFlippedX)
		{
			GameObject gameObjectToActivate = (isFlippedX ? activeOnFlippedX : activeOnNormalX);
			GameObject gameObjectToDeactivate = (isFlippedX ? activeOnNormalX : activeOnFlippedX);
			gameObjectToActivate.SetActive(true);
			gameObjectToDeactivate.SetActive(false);
			ResetJointPositions(isFlippedX ? jointsFlippedX : jointsNormalX);
			ResetJointPositions(isFlippedX ? jointsNormalX : jointsFlippedX);
			CompensateMovementAfterFlipX(gameObjectToActivate.transform, gameObjectToDeactivate.transform);
		}

		private void ResetJointPositions(HingeJoint2D[] joints)
		{
			foreach (HingeJoint2D joint in joints)
			{
				Transform parent = joint.connectedBody.transform;
				joint.transform.position = parent.TransformPoint(joint.connectedAnchor);
			}
		}

		private void CompensateMovementAfterFlipX(Transform toActivate, Transform toDeactivate)
		{
			Transform targetLocation = toDeactivate.GetChild(0);
			Transform currentLocation = toActivate.GetChild(0);
			toActivate.position += targetLocation.position - currentLocation.position;
		}
	}
}

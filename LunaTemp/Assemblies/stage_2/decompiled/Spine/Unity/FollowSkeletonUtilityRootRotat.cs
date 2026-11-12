using UnityEngine;

namespace Spine.Unity
{
	public class FollowSkeletonUtilityRootRotation : MonoBehaviour
	{
		private const float FLIP_ANGLE_THRESHOLD = 100f;

		public Transform reference;

		private Vector3 prevLocalEulerAngles;

		private void Start()
		{
			prevLocalEulerAngles = base.transform.localEulerAngles;
		}

		private void FixedUpdate()
		{
			base.transform.rotation = reference.rotation;
			bool wasFlippedAroundY = Mathf.Abs(base.transform.localEulerAngles.y - prevLocalEulerAngles.y) > 100f;
			bool wasFlippedAroundX = Mathf.Abs(base.transform.localEulerAngles.x - prevLocalEulerAngles.x) > 100f;
			if (wasFlippedAroundY)
			{
				CompensatePositionToYRotation();
			}
			if (wasFlippedAroundX)
			{
				CompensatePositionToXRotation();
			}
			prevLocalEulerAngles = base.transform.localEulerAngles;
		}

		private void CompensatePositionToYRotation()
		{
			Vector3 newPosition = reference.position + (reference.position - base.transform.position);
			newPosition.y = base.transform.position.y;
			base.transform.position = newPosition;
		}

		private void CompensatePositionToXRotation()
		{
			Vector3 newPosition = reference.position + (reference.position - base.transform.position);
			newPosition.x = base.transform.position.x;
			base.transform.position = newPosition;
		}
	}
}

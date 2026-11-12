using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class NormalElementView : ElementView
	{
		[Header("Spine Settings")]
		public SkeletonDataAsset spineDataAsset;

		[SpineAnimation("", "spineDataAsset", true, true)]
		public string changeAnimationName = "change";

		[Tooltip("Adjust this to easily scale the Spine animation up or down")]
		public float spineScaleMultiplier = 1f;

		public bool clearEffectAlreadyPlayed;

		private string currentSkinName;

		public int NormalElementId { get; private set; }

		public void Initialize(NormalElementInfo info, int x, int y, Vector3 worldPosition, float cellSize)
		{
			if (info != null)
			{
				NormalElementId = info.id;
				currentSkinName = info.spineSkinName;
				UpdateVisuals(info.sprite, cellSize);
				SetPosition(x, y, worldPosition);
			}
		}

		public void PlayChangeAnimation()
		{
			if (!(this == null) && !(base.gameObject == null) && !(base.transform.Find("SpineAnimation") != null) && spineDataAsset != null)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.enabled = false;
				}
				GameObject spineObj = new GameObject("SpineAnimation");
				spineObj.transform.SetParent(base.transform);
				spineObj.transform.localPosition = Vector3.zero;
				spineObj.transform.localScale = Vector3.one * Mathf.Abs(spineScaleMultiplier * targetCellSize);
				SkeletonAnimation skeletonAnimation = spineObj.AddComponent<SkeletonAnimation>();
				skeletonAnimation.skeletonDataAsset = spineDataAsset;
				skeletonAnimation.Initialize(true);
				MeshRenderer meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.sortingLayerName = "Default";
					meshRenderer.sortingOrder = 55;
				}
				if (!string.IsNullOrEmpty(currentSkinName))
				{
					skeletonAnimation.Skeleton.SetSkin(currentSkinName);
					skeletonAnimation.Skeleton.SetSlotsToSetupPose();
					skeletonAnimation.LateUpdate();
				}
				if (!string.IsNullOrEmpty(changeAnimationName))
				{
					skeletonAnimation.AnimationState.SetAnimation(0, changeAnimationName, false);
				}
			}
		}
	}
}

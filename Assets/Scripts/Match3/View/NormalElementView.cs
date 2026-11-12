using UnityEngine;

namespace Match3
{
    /// <summary>
    /// Attached to the unified Normal Element Prefab.
    /// </summary>
    public class NormalElementView : ElementView
    {
        public int NormalElementId { get; private set; }

        [Header("Spine Settings")]
        public Spine.Unity.SkeletonDataAsset spineDataAsset;
        [Spine.Unity.SpineAnimation(dataField: "spineDataAsset", fallbackToTextField: true)]
        public string changeAnimationName = "change";
        [Tooltip("Adjust this to easily scale the Spine animation up or down")]
        public float spineScaleMultiplier = 1f;

        /// <summary>
        /// 魔力球光线到达时已提前播放过消除特效，销毁时不再重复播放
        /// </summary>
        public bool clearEffectAlreadyPlayed;

        private string currentSkinName;

        /// <summary>
        /// Initializes the normal element with data from the ElementDatabase.
        /// </summary>
        public void Initialize(NormalElementInfo info, int x, int y, Vector3 worldPosition, float cellSize)
        {
            if (info == null) return;
            
            NormalElementId = info.id;
            currentSkinName = info.spineSkinName;
            
            UpdateVisuals(info.sprite, cellSize);
            SetPosition(x, y, worldPosition);
            
            // Note: Since this is the unified prefab, it will use the same animator/effects 
            // for all normal elements. Only the sprite changes.
        }

        public void PlayChangeAnimation()
        {
            if (this == null || gameObject == null) return; // 检查物体是否被销毁

            if (transform.Find("SpineAnimation") != null) return; // Already playing

            if (spineDataAsset != null)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = false;

                GameObject spineObj = new GameObject("SpineAnimation");
                spineObj.transform.SetParent(transform);
                spineObj.transform.localPosition = Vector3.zero;
                // 使用 targetCellSize 匹配当前棋盘缩放比例，使用 Mathf.Abs 防止 WebGL 矩阵翻转报错
                spineObj.transform.localScale = Vector3.one * Mathf.Abs(spineScaleMultiplier * targetCellSize);

                var skeletonAnimation = spineObj.AddComponent<Spine.Unity.SkeletonAnimation>();
                skeletonAnimation.skeletonDataAsset = spineDataAsset;
                skeletonAnimation.Initialize(true);
                
                var meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
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
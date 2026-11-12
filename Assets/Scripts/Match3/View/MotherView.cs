using UnityEngine;
using Spine.Unity;
using TMPro;
using DG.Tweening;

namespace Match3
{
    public class MotherView : ElementView
    {
        public MotherData Data { get; private set; }
        public int CurrentCount { get; private set; }
        
        // 用于解决同一帧多个子物体同时飞向母体导致超额扣减计数的问题
        public int PendingReceiveCount { get; set; } = 0;

        [Header("UI")]
        [Tooltip("Text component to show remaining count")]
        public TextMeshPro countText;

        [Header("Spine Settings")]
        public SkeletonAnimation spineAnimation;
        public float spineScaleMultiplier = 1f;

        [Header("Rendering")]
        [Tooltip("Sorting order for the Spine animation")]
        public int spineSortingOrder = 30;
        [Tooltip("Sorting order for the text (should be higher than spine)")]
        public int textSortingOrder = 35;

        private Vector3 initialSpineScale = Vector3.zero;

        public void Initialize(MotherData data, int x, int y, Vector3 worldPosition, float cellSize, float cellSpacing)
        {
            if (data == null) return;
            Data = data;
            CurrentCount = data.requiredCount;
            SetPosition(x, y, worldPosition);

            // Get the center offset that was saved in the editor
            int offsetX = data.gridWidth > 1 || data.gridHeight > 1 ? 0 : 0; // fallback
            
            // NOTE: We do NOT shift the root transform anymore.
            // The root transform MUST stay exactly at `worldPosition` (the bottom-left cell's position)
            // so that Grid interactions and particle effects targeted at this element line up correctly.
            // We only shift the visual children (Spine and Text) in local space below.
            
            transform.position = worldPosition;

            if (spineAnimation != null)
            {
                if (initialSpineScale == Vector3.zero) initialSpineScale = spineAnimation.transform.localScale;
                
                if (spineAnimation.skeleton != null)
                {
                    spineAnimation.transform.localScale = Vector3.one;
                    spineAnimation.skeleton.ScaleX = Mathf.Abs(initialSpineScale.x * spineScaleMultiplier);
                    spineAnimation.skeleton.ScaleY = Mathf.Abs(initialSpineScale.y * spineScaleMultiplier);
                }
                else
                {
                    spineAnimation.transform.localScale = new Vector3(
                        Mathf.Abs(initialSpineScale.x * spineScaleMultiplier),
                        Mathf.Abs(initialSpineScale.y * spineScaleMultiplier),
                        Mathf.Abs(initialSpineScale.z * spineScaleMultiplier)
                    );
                }

                var meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.sortingLayerName = "Default"; 
                    meshRenderer.sortingOrder = spineSortingOrder;
                }
            }
            
            transform.localScale = Vector3.one * Mathf.Abs(cellSize);
            
            // Adjust the actual visual models (Spine and Text) to be centered, instead of moving the root transform
            // Because the root transform's position is still tied to the (x,y) logic grid for tracking.
            // By shifting ALL visual elements uniformly, we ensure everything (backgrounds, texts, spines) stays together.
            
            // NOTE: We only want to offset them ONCE when spawned. If we pool or re-init, we shouldn't offset an already offset object.
            // A simple way is to check if it's already offset, or just set absolute positions based on prefab defaults.
            // To make it robust:
            float localOffsetX = (data.gridWidth - 1) / 2f * (1f + cellSpacing / cellSize) + data.visualOffset.x;
            float localOffsetY = (data.gridHeight - 1) / 2f * (1f + cellSpacing / cellSize) + data.visualOffset.y;
            
            // Loop through all direct children of this MotherView and offset them
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                
                // Assuming the prefab's children are originally modeled around (0,0,z).
                // If they have their own local offsets in the prefab, we should ideally add our grid offset to their base offset.
                // However, since we just instantiated it, we can just ADD the offset to whatever localPosition it currently has.
                // To prevent double-offsetting on reuse (if you implement object pooling later), you would need to store original positions.
                // For now, since we Instantiate fresh every time:
                
                // ONLY offset if we haven't already (simple check to prevent double offset if Initialize is called twice)
                if (child.localPosition.x < 0.1f && child.localPosition.y < 0.1f && localOffsetX > 0)
                {
                     Vector3 currentPos = child.localPosition;
                     child.localPosition = new Vector3(currentPos.x + localOffsetX, currentPos.y + localOffsetY, currentPos.z);
                }
            }
            
            if (countText != null)
            {
                countText.GetComponent<MeshRenderer>().sortingLayerName = "Default";
                countText.GetComponent<MeshRenderer>().sortingOrder = textSortingOrder;
            }

            UpdateText();
            PlayIdle();
        }

        private void UpdateText()
        {
            if (countText != null)
            {
                countText.text = CurrentCount > 0 ? CurrentCount.ToString() : "";
            }
        }

        public void PlayIdle()
        {
            if (spineAnimation != null && !string.IsNullOrEmpty(Data.idleAnimation))
            {
                try {
                    if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.idleAnimation) != null)
                    {
                        spineAnimation.AnimationState.SetAnimation(0, Data.idleAnimation, true);
                    }
                } catch (System.Exception e) {
                    Debug.LogWarning($"Spine animation error (idle): {e.Message}");
                }
            }
        }

        public void ReceiveChild(System.Action onCompleteCallback)
        {
            if (CurrentCount <= 0) return;

            CurrentCount--;
            UpdateText();

            if (AudioManager.Instance != null && Data.receiveSound != null)
            {
                AudioManager.Instance.PlayGeneratorSound(Data.receiveSound);
            }

            if (CurrentCount > 0)
            {
                if (spineAnimation != null && !string.IsNullOrEmpty(Data.receiveAnimation))
                {
                    try {
                        if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.receiveAnimation) != null)
                        {
                            spineAnimation.AnimationState.SetAnimation(0, Data.receiveAnimation, false);
                            if (!string.IsNullOrEmpty(Data.idleAnimation) && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.idleAnimation) != null)
                            {
                                spineAnimation.AnimationState.AddAnimation(0, Data.idleAnimation, true, 0f);
                            }
                        }
                    } catch (System.Exception e) {
                        Debug.LogWarning($"Spine animation error (receive): {e.Message}");
                    }
                }
                
                // Do not punch the root transform, punch the visual children to avoid messing up the grid scale
                if (spineAnimation != null)
                {
                    spineAnimation.transform.DOKill(true);
                    spineAnimation.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1f);
                }
            }
            else
            {
                if (AudioManager.Instance != null && Data.completeSound != null)
                {
                    AudioManager.Instance.PlayGeneratorSound(Data.completeSound);
                }

                float delay = 0f;
                if (spineAnimation != null && !string.IsNullOrEmpty(Data.completeAnimation))
                {
                    try {
                        if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.completeAnimation) != null)
                        {
                            var track = spineAnimation.AnimationState.SetAnimation(0, Data.completeAnimation, false);
                            if (track != null) delay = track.Animation.Duration;
                        }
                    } catch (System.Exception e) {
                        Debug.LogWarning($"Spine animation error (complete): {e.Message}");
                    }
                }
                
                // Hide text immediately
                if (countText != null) countText.gameObject.SetActive(false);

                // Ensure the delay is at least 0.01f to avoid issues with DelayedCall immediately completing
                if (delay <= 0.05f) delay = 0.05f;

                DOVirtual.DelayedCall(delay, () => {
                    if (this != null && gameObject != null)
                    {
                        onCompleteCallback?.Invoke();
                    }
                });
            }
        }
    }
}
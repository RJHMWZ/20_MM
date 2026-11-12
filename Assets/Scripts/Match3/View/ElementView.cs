using UnityEngine;
using DG.Tweening;

namespace Match3
{
    /// <summary>
    /// Base class for all element visuals in the game scene.
    /// This should be attached to the root of your element prefab.
    /// </summary>
    public abstract class ElementView : MonoBehaviour
    {
        [Header("Visual Components")]
        [Tooltip("The SpriteRenderer used to display the element's icon")]
        public SpriteRenderer spriteRenderer;

        // Current grid coordinates
        public int GridX { get; protected set; }
        public int GridY { get; protected set; }

        protected float targetCellSize = 1f;

        public virtual void SetPosition(int x, int y, Vector3 worldPosition)
        {
            GridX = x;
            GridY = y;
            transform.position = worldPosition;
        }

        public virtual void MoveTo(int x, int y, Vector3 targetWorldPos, float duration = 0.3f, TweenCallback onComplete = null)
        {
            GridX = x;
            GridY = y;
            // 确保不会因为 DOTween 移动而重置或改变旋转
            transform.DOMove(targetWorldPos, duration).SetEase(Ease.InOutQuad).OnComplete(onComplete);
        }

        private Vector3 originalRootScale = Vector3.zero;

        protected virtual void Awake()
        {
            originalRootScale = transform.localScale;
        }

        public virtual void UpdateVisuals(Sprite sprite, float cellSize)
        {
            targetCellSize = cellSize;
            
            if (originalRootScale == Vector3.zero)
            {
                originalRootScale = transform.localScale;
            }

            if (this is SpecialElementView)
            {
                // FOR SPECIAL ELEMENTS:
                // We forcefully set the root scale to exactly 'cellSize'.
                // This ignores any weird scaling saved in the Prefab and guarantees uniform size across all devices.
                transform.localScale = Vector3.one * Mathf.Abs(cellSize);
                
                if (spriteRenderer != null && sprite != null)
                {
                    spriteRenderer.sprite = sprite;
                    float spriteWidth = sprite.bounds.size.x;
                    float spriteHeight = sprite.bounds.size.y;
                    
                    // Since the root is exactly 'cellSize', we just need to scale the Sprite 
                    // so its local size is 1x1, and then multiply by 0.95 for a little padding.
                    float scaleFactorX = 0.95f / spriteWidth;
                    float scaleFactorY = 0.95f / spriteHeight;
                    
                    float finalScale = Mathf.Min(Mathf.Abs(scaleFactorX), Mathf.Abs(scaleFactorY));
                    spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
                    
                    spriteRenderer.sortingLayerName = "Default";
                    spriteRenderer.sortingOrder = 20;
                }
            }
            else if (this is RetractingCollectibleView)
            {
                // FOR RETRACTING COLLECTIBLES:
                // Similar to normal elements but without overwriting local scale logic if spine is used
                // 绝对禁止在这里重置 rotation !
                if (spriteRenderer != null && sprite != null)
                {
                    spriteRenderer.sprite = sprite;
                    float spriteWidth = sprite.bounds.size.x;
                    float spriteHeight = sprite.bounds.size.y;
                    
                    float scaleFactorX = (cellSize * 0.95f) / spriteWidth;
                    float scaleFactorY = (cellSize * 0.95f) / spriteHeight;
                    
                    float finalScale = Mathf.Min(Mathf.Abs(scaleFactorX), Mathf.Abs(scaleFactorY));
                    spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
                    
                    spriteRenderer.sortingLayerName = "Default";
                    spriteRenderer.sortingOrder = 20;
                }
            }
            else
            {
                // FOR NORMAL ELEMENTS:
                // The SpriteRenderer is usually on the root object itself.
                if (spriteRenderer != null && sprite != null)
                {
                    spriteRenderer.sprite = sprite;
                    float spriteWidth = sprite.bounds.size.x;
                    float spriteHeight = sprite.bounds.size.y;
                    
                    // Multiply by cellSize directly here
                    float scaleFactorX = (cellSize * 0.95f) / spriteWidth;
                    float scaleFactorY = (cellSize * 0.95f) / spriteHeight;
                    
                    float finalScale = Mathf.Min(Mathf.Abs(scaleFactorX), Mathf.Abs(scaleFactorY));
                    spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
                    
                    spriteRenderer.sortingLayerName = "Default";
                    spriteRenderer.sortingOrder = 20;
                }
            }
        }
        
        // You can add animation methods here later (e.g., PlayMatchAnimation, MoveTo, etc.)
    }
}
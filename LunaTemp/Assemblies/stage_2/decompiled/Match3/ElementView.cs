using DG.Tweening;
using UnityEngine;

namespace Match3
{
	public abstract class ElementView : MonoBehaviour
	{
		[Header("Visual Components")]
		[Tooltip("The SpriteRenderer used to display the element's icon")]
		public SpriteRenderer spriteRenderer;

		protected float targetCellSize = 1f;

		private Vector3 originalRootScale = Vector3.zero;

		public int GridX { get; protected set; }

		public int GridY { get; protected set; }

		public virtual void SetPosition(int x, int y, Vector3 worldPosition)
		{
			GridX = x;
			GridY = y;
			base.transform.position = worldPosition;
		}

		public virtual void MoveTo(int x, int y, Vector3 targetWorldPos, float duration = 0.3f, TweenCallback onComplete = null)
		{
			GridX = x;
			GridY = y;
			base.transform.DOMove(targetWorldPos, duration).SetEase(Ease.InOutQuad).OnComplete(onComplete);
		}

		protected virtual void Awake()
		{
			originalRootScale = base.transform.localScale;
		}

		public virtual void UpdateVisuals(Sprite sprite, float cellSize)
		{
			targetCellSize = cellSize;
			if (originalRootScale == Vector3.zero)
			{
				originalRootScale = base.transform.localScale;
			}
			if (this is SpecialElementView)
			{
				base.transform.localScale = Vector3.one * Mathf.Abs(cellSize);
				if (spriteRenderer != null && sprite != null)
				{
					spriteRenderer.sprite = sprite;
					float spriteWidth = sprite.bounds.size.x;
					float spriteHeight2 = sprite.bounds.size.y;
					float scaleFactorX2 = 0.95f / spriteWidth;
					float scaleFactorY3 = 0.95f / spriteHeight2;
					float finalScale3 = Mathf.Min(Mathf.Abs(scaleFactorX2), Mathf.Abs(scaleFactorY3));
					spriteRenderer.transform.localScale = new Vector3(finalScale3, finalScale3, 1f);
					spriteRenderer.sortingLayerName = "Default";
					spriteRenderer.sortingOrder = 20;
				}
			}
			else if (this is RetractingCollectibleView)
			{
				if (spriteRenderer != null && sprite != null)
				{
					spriteRenderer.sprite = sprite;
					float spriteWidth3 = sprite.bounds.size.x;
					float spriteHeight3 = sprite.bounds.size.y;
					float scaleFactorX3 = cellSize * 0.95f / spriteWidth3;
					float scaleFactorY2 = cellSize * 0.95f / spriteHeight3;
					float finalScale2 = Mathf.Min(Mathf.Abs(scaleFactorX3), Mathf.Abs(scaleFactorY2));
					spriteRenderer.transform.localScale = new Vector3(finalScale2, finalScale2, 1f);
					spriteRenderer.sortingLayerName = "Default";
					spriteRenderer.sortingOrder = 20;
				}
			}
			else if (spriteRenderer != null && sprite != null)
			{
				spriteRenderer.sprite = sprite;
				float spriteWidth2 = sprite.bounds.size.x;
				float spriteHeight = sprite.bounds.size.y;
				float scaleFactorX = cellSize * 0.95f / spriteWidth2;
				float scaleFactorY = cellSize * 0.95f / spriteHeight;
				float finalScale = Mathf.Min(Mathf.Abs(scaleFactorX), Mathf.Abs(scaleFactorY));
				spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
				spriteRenderer.sortingLayerName = "Default";
				spriteRenderer.sortingOrder = 20;
			}
		}
	}
}

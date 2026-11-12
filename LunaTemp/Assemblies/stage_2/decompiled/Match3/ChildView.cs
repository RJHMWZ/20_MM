using UnityEngine;

namespace Match3
{
	public class ChildView : ElementView
	{
		public ChildData Data { get; private set; }

		public void Initialize(ChildData data, int x, int y, Vector3 worldPosition, float cellSize)
		{
			if (!(data == null))
			{
				Data = data;
				SetPosition(x, y, worldPosition);
				base.transform.localScale = Vector3.one * Mathf.Abs(cellSize);
				MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.sortingLayerName = "Default";
					meshRenderer.sortingOrder = 40;
				}
				SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null)
				{
					spriteRenderer.sortingLayerName = "Default";
					spriteRenderer.sortingOrder = 40;
				}
			}
		}

		public void PlayCollectEffect()
		{
			if (!(Data.collectEffectPrefab != null))
			{
				return;
			}
			GameObject effect = Object.Instantiate(Data.collectEffectPrefab, base.transform.position, Quaternion.identity);
			if (base.transform.parent != null)
			{
				effect.transform.SetParent(base.transform.parent, true);
			}
			effect.transform.localScale = Vector3.one * targetCellSize;
			ParticleSystem[] psList = effect.GetComponentsInChildren<ParticleSystem>(true);
			if (psList != null)
			{
				ParticleSystem[] array = psList;
				foreach (ParticleSystem ps in array)
				{
					ps.gameObject.SetActive(true);
					ParticleSystem.MainModule main = ps.main;
					main.loop = false;
					ps.Play(false);
				}
			}
			Object.Destroy(effect, 2f);
		}
	}
}

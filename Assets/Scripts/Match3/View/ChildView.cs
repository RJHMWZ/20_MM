using UnityEngine;

namespace Match3
{
    public class ChildView : ElementView
    {
        public ChildData Data { get; private set; }

        public void Initialize(ChildData data, int x, int y, Vector3 worldPosition, float cellSize)
        {
            if (data == null) return;
            Data = data;
            SetPosition(x, y, worldPosition);
            transform.localScale = Vector3.one * Mathf.Abs(cellSize);
            
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingLayerName = "Default"; 
                meshRenderer.sortingOrder = 40; 
            }
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Default"; 
                spriteRenderer.sortingOrder = 40; 
            }
        }

        public void PlayCollectEffect()
        {
            if (Data.collectEffectPrefab != null)
            {
                GameObject effect = Instantiate(Data.collectEffectPrefab, transform.position, Quaternion.identity);
                if (transform.parent != null)
                {
                    effect.transform.SetParent(transform.parent, true);
                }
                effect.transform.localScale = Vector3.one * targetCellSize;

                ParticleSystem[] psList = effect.GetComponentsInChildren<ParticleSystem>(true);
                if (psList != null)
                {
                    foreach (var ps in psList)
                    {
                        ps.gameObject.SetActive(true);
                        var main = ps.main;
                        main.loop = false;
                        ps.Play(false);
                    }
                }
                
                Destroy(effect, 2f);
            }
        }
    }
}
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace Match3
{
    public class ObstacleView : ElementView
    {
        public ObstacleData Data { get; private set; }
        public int CurrentHealth { get; private set; }

        [Header("Spine Settings")]
        public SkeletonAnimation spineAnimation;
        public float spineScaleMultiplier = 1f;

        private Vector3 initialSpineScale = Vector3.zero;

        public void Initialize(ObstacleData data, int x, int y, Vector3 worldPosition, float cellSize)
        {
            if (data == null) return;

            Data = data;
            CurrentHealth = data.maxHealth;
            SetPosition(x, y, worldPosition);

            // Handle Spine Animation scaling
            if (spineAnimation != null)
            {
                if (initialSpineScale == Vector3.zero)
                {
                    initialSpineScale = spineAnimation.transform.localScale;
                }
                
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
                    meshRenderer.sortingOrder = 40; 
                }
            }
            
            // Set scale to fit cell size if not using spine or as base
            transform.localScale = Vector3.one * Mathf.Abs(cellSize);
            
            // Apply visual offset
            if (data.visualOffset != Vector2.zero)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    Vector3 currentPos = child.localPosition;
                    child.localPosition = new Vector3(currentPos.x + data.visualOffset.x, currentPos.y + data.visualOffset.y, currentPos.z);
                }
            }

            UpdateStateVisual();
        }

        public void TakeDamage()
        {
            if (CurrentHealth <= 0) return;

            CurrentHealth--;

            // Play damage effect
            int damageEffectIndex = Data.maxHealth - CurrentHealth - 1;
            if (damageEffectIndex >= 0 && Data.damageEffects != null && damageEffectIndex < Data.damageEffects.Count)
            {
                GameObject effectPrefab = Data.damageEffects[damageEffectIndex];
                if (effectPrefab != null)
                {
                    SpawnEffect(effectPrefab);
                }
            }
            
            if (CurrentHealth > 0)
            {
                if (AudioManager.Instance != null && Data.damageSound != null)
                {
                    AudioManager.Instance.PlayObstacleDamageSound(Data.damageSound);
                }

                PlayTransitionAndNextState(damageEffectIndex);
                // Play a small punch animation
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1f);
            }
        }

        private void PlayTransitionAndNextState(int transitionIndex)
        {
            if (spineAnimation == null) return;

            int nextStateIndex = Data.maxHealth - CurrentHealth;
            string nextStateAnim = (Data.stateAnimations != null && nextStateIndex < Data.stateAnimations.Count) 
                                    ? Data.stateAnimations[nextStateIndex] : null;
            string nextStateSkin = (Data.stateSkins != null && nextStateIndex < Data.stateSkins.Count)
                                    ? Data.stateSkins[nextStateIndex] : null;

            if (Data.transitionAnimations != null && transitionIndex >= 0 && transitionIndex < Data.transitionAnimations.Count)
            {
                string transitionAnim = Data.transitionAnimations[transitionIndex];
                if (!string.IsNullOrEmpty(transitionAnim))
                {
                    // Apply skin before transition if available
                    if (!string.IsNullOrEmpty(nextStateSkin))
                    {
                        spineAnimation.skeleton.SetSkin(nextStateSkin);
                        spineAnimation.skeleton.SetSlotsToSetupPose();
                    }

                    // Play the transition animation once
                    spineAnimation.AnimationState.SetAnimation(0, transitionAnim, false);
                    
                    // Queue the next state animation to loop afterwards
                    if (!string.IsNullOrEmpty(nextStateAnim))
                    {
                        spineAnimation.AnimationState.AddAnimation(0, nextStateAnim, true, 0f);
                    }
                    return;
                }
            }

            // Fallback to instantly changing state if no transition animation exists
            if (!string.IsNullOrEmpty(nextStateSkin))
            {
                spineAnimation.skeleton.SetSkin(nextStateSkin);
                spineAnimation.skeleton.SetSlotsToSetupPose();
            }
            if (!string.IsNullOrEmpty(nextStateAnim))
            {
                spineAnimation.AnimationState.SetAnimation(0, nextStateAnim, true);
            }
        }

        private void UpdateStateVisual()
        {
            int stateIndex = Data.maxHealth - CurrentHealth;
            if (spineAnimation != null)
            {
                if (Data.stateSkins != null && stateIndex < Data.stateSkins.Count)
                {
                    string skinName = Data.stateSkins[stateIndex];
                    if (!string.IsNullOrEmpty(skinName))
                    {
                        spineAnimation.skeleton.SetSkin(skinName);
                        spineAnimation.skeleton.SetSlotsToSetupPose();
                    }
                }

                if (Data.stateAnimations != null && stateIndex < Data.stateAnimations.Count)
                {
                    string animName = Data.stateAnimations[stateIndex];
                    if (!string.IsNullOrEmpty(animName))
                    {
                        spineAnimation.AnimationState.SetAnimation(0, animName, true);
                    }
                }
            }
        }

        public void PlayCollectEffect()
        {
            if (AudioManager.Instance != null && Data.destroySound != null)
            {
                AudioManager.Instance.PlayObstacleDestroySound(Data.destroySound);
            }

            if (Data.collectEffect != null)
            {
                SpawnEffect(Data.collectEffect);
            }
        }

        private void SpawnEffect(GameObject prefab)
        {
            GameObject effect = Instantiate(prefab, transform.position, Quaternion.identity);
            
            // Assuming effect needs to be scaled
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

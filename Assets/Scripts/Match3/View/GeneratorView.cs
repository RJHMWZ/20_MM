using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace Match3
{
    public class GeneratorView : ElementView
    {
        public GeneratorData Data { get; private set; }
        
        [Header("Spine Settings")]
        public SkeletonAnimation spineAnimation;
        public float spineScaleMultiplier = 1f;

        private Vector3 initialSpineScale = Vector3.zero;
        private int currentGenerations = 0;
        private int currentHits = 0;

        public void Initialize(GeneratorData data, int x, int y, Vector3 worldPosition, float cellSize)
        {
            if (data == null) return;
            Data = data;
            SetPosition(x, y, worldPosition);

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
                    meshRenderer.sortingOrder = 30; // Behind normal elements maybe, or same
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

            PlayIdle();
        }

        public void PlayIdle()
        {
            UpdateStateVisual(true);
        }

        private void UpdateStateVisual(bool playAnimation = true)
        {
            if (spineAnimation == null) return;
            
            bool isEmpty = Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations;
            if (isEmpty)
            {
                if (!string.IsNullOrEmpty(Data.emptyAnimation))
                    spineAnimation.AnimationState.SetAnimation(0, Data.emptyAnimation, true);
                return;
            }
            
            // Apply Skin
            if (Data.stateSkins != null && currentHits < Data.stateSkins.Count && !string.IsNullOrEmpty(Data.stateSkins[currentHits]))
            {
                spineAnimation.skeleton.SetSkin(Data.stateSkins[currentHits]);
                spineAnimation.skeleton.SetSlotsToSetupPose();
            }
            
            // Apply Animation
            if (playAnimation)
            {
                string animToPlay = Data.idleAnimation;
                if (Data.stateAnimations != null && currentHits < Data.stateAnimations.Count && !string.IsNullOrEmpty(Data.stateAnimations[currentHits]))
                {
                    animToPlay = Data.stateAnimations[currentHits];
                }
                
                if (!string.IsNullOrEmpty(animToPlay))
                {
                    spineAnimation.AnimationState.SetAnimation(0, animToPlay, true);
                }
            }
        }

        public bool AdvanceState()
        {
            if (Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations) return false;
            
            currentHits++;
            int targetHits = Mathf.Max(1, Data.hitsToGenerate);
            
            if (currentHits >= targetHits)
            {
                currentHits = 0; // Reset for the next generation cycle
                return true; // Ready to generate
            }
            else
            {
                // Just advance state visually
                if (AudioManager.Instance != null && Data.hitSound != null)
                {
                    AudioManager.Instance.PlayGeneratorSound(Data.hitSound);
                }
                
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1f);
                
                string transitionAnim = null;
                if (Data.transitionAnimations != null && (currentHits - 1) < Data.transitionAnimations.Count)
                {
                    transitionAnim = Data.transitionAnimations[currentHits - 1];
                }
                
                string nextIdle = Data.idleAnimation;
                if (Data.stateAnimations != null && currentHits < Data.stateAnimations.Count && !string.IsNullOrEmpty(Data.stateAnimations[currentHits]))
                {
                    nextIdle = Data.stateAnimations[currentHits];
                }
                
                if (!string.IsNullOrEmpty(transitionAnim))
                {
                    UpdateStateVisual(false); // Update skin but don't play idle
                    spineAnimation.AnimationState.SetAnimation(0, transitionAnim, false);
                    if (!string.IsNullOrEmpty(nextIdle))
                    {
                        spineAnimation.AnimationState.AddAnimation(0, nextIdle, true, 0f);
                    }
                }
                else
                {
                    UpdateStateVisual(true);
                }
                
                return false; // Not generating yet
            }
        }

        public void TriggerGeneration(int targetIndex, Vector3 uiTargetPos)
        {
            if (Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations) return;

            currentGenerations++;
            bool isEmpty = Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations;

            if (AudioManager.Instance != null && Data.generateSound != null)
            {
                AudioManager.Instance.PlayGeneratorSound(Data.generateSound);
            }

            if (spineAnimation != null && !string.IsNullOrEmpty(Data.workingAnimation))
            {
                var track = spineAnimation.AnimationState.SetAnimation(0, Data.workingAnimation, false);
                if (isEmpty && !string.IsNullOrEmpty(Data.emptyAnimation))
                {
                    spineAnimation.AnimationState.AddAnimation(0, Data.emptyAnimation, true, 0f);
                }
                else
                {
                    string nextIdle = Data.idleAnimation;
                    if (Data.stateAnimations != null && currentHits < Data.stateAnimations.Count && !string.IsNullOrEmpty(Data.stateAnimations[currentHits]))
                    {
                        nextIdle = Data.stateAnimations[currentHits];
                    }
                    if (!string.IsNullOrEmpty(nextIdle))
                    {
                        UpdateStateVisual(false); // Reset to base state skin immediately, then play idle after work
                        spineAnimation.AnimationState.AddAnimation(0, nextIdle, true, 0f);
                    }
                }
            }

            if (Data.generatedItemPrefab != null)
            {
                GameObject paper = Instantiate(Data.generatedItemPrefab, transform.position, Quaternion.identity);
                
                if (transform.parent != null)
                {
                    paper.transform.SetParent(transform.parent, true);
                }
                
                // Make paper start from scale 0 and pop out to a slightly smaller scale to prevent it from looking too big
                paper.transform.localScale = Vector3.zero;
                float finalPaperScale = targetCellSize * 0.7f; // 缩放调整为原来大小的70%

                // Ensure Paper Spine renders on top
                var paperMesh = paper.GetComponentInChildren<MeshRenderer>();
                if (paperMesh != null)
                {
                    paperMesh.sortingLayerName = "Default";
                    paperMesh.sortingOrder = 100;
                }
                var paperSprite = paper.GetComponentInChildren<SpriteRenderer>();
                if (paperSprite != null)
                {
                    paperSprite.sortingLayerName = "Default";
                    paperSprite.sortingOrder = 100;
                }

                // Jump animation: Go up relative to the generator's position (Lowered height)
                Vector3 jumpPos = transform.position + new Vector3(0f, targetCellSize * 0.6f, 0f);
                
                Sequence seq = DOTween.Sequence();
                seq.Append(paper.transform.DOJump(jumpPos, targetCellSize * 0.3f, 1, 0.4f));
                seq.Join(paper.transform.DOScale(Vector3.one * finalPaperScale, 0.4f).SetEase(Ease.OutBack));

                if (targetIndex >= 0 && uiTargetPos != Vector3.zero)
                {
                    // Fly to UI
                    seq.AppendInterval(0.1f);
                    
                    // Switch to Target Icon before flying
                    seq.AppendCallback(() => {
                        if (Data.targetIcon != null)
                        {
                            // Hide Spine renderer
                            if (paperMesh != null) paperMesh.enabled = false;
                            
                            // Show static sprite renderer
                            if (paperSprite != null)
                            {
                                paperSprite.sprite = Data.targetIcon;
                                paperSprite.enabled = true;
                                
                                // Adjust scale for the static sprite so it looks good while flying
                                float iconWidth = Data.targetIcon.bounds.size.x;
                                float scaleFactor = (targetCellSize * 0.95f) / iconWidth;
                                paperSprite.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                            }
                        }
                    });

                    // CatmullRom path
                    Vector3 midPoint = jumpPos + (uiTargetPos - jumpPos) / 2f + new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), 0f);
                    Vector3[] path = new Vector3[] { midPoint, uiTargetPos };

                    seq.Append(paper.transform.DOPath(path, 0.6f, PathType.CatmullRom).SetEase(Ease.InOutQuad));
                    seq.Join(paper.transform.DOScale(paper.transform.localScale * 0.5f, 0.6f));
                    seq.OnComplete(() => {
                        if (AudioManager.Instance != null) AudioManager.Instance.PlayCollectSound();
                        Destroy(paper);
                    });
                }
                else
                {
                    // If not a target, just fade out and destroy
                    seq.AppendInterval(0.5f);
                    seq.Append(paper.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
                    seq.OnComplete(() => Destroy(paper));
                }
            }
        }
    }
}
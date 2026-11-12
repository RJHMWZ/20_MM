using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

namespace Match3
{
    /// <summary>
    /// Attached to individual Special Element Prefabs.
    /// </summary>
    public class SpecialElementView : ElementView
    {
        public SpecialElementData Data { get; private set; }
        public SpecialBehaviorType BehaviorType { get; private set; }
        public string SpecialElementId { get; private set; }

        [Header("Special Effects")]
        [Tooltip("Optional: Particle system to play when this special element is created or idling")]
        public ParticleSystem idleParticles;

        [Header("Spine Animations")]
        public SkeletonAnimation spineAnimation;
        [SpineAnimation(dataField: "spineAnimation", fallbackToTextField: true)]
        public string creationAnimation;
        [SpineAnimation(dataField: "spineAnimation", fallbackToTextField: true)]
        public string idleAnimation;
        [SpineAnimation(dataField: "spineAnimation", fallbackToTextField: true)]
        public string triggerAnimation;

        [Header("Swap Animations")]
        [Tooltip("交换/组合动画绑定列表：每个条目对应一种组合，支持独立骨骼 Data 的 Spine 子物体层")]
        public List<SwapAnimationBinding> swapAnimations = new List<SwapAnimationBinding>();

        [Header("Swap Particle Effect (挂载接口)")]
        [Tooltip("交换动画播放期间同步播放的粒子特效预制体（如魔力球+魔力球交换时的中心能量特效）。在预制体上挂载，PlaySwapParticleEffect() 播放")]
        public GameObject swapParticlePrefab;

        /// <summary>
        /// 组合交换动画是否已播放。播放成功后销毁元素时不再播放 Trigger 动画（直接淡出），
        /// 避免 Trigger 动画覆盖/穿插交换表演。
        /// </summary>
        public bool swapAnimationPlayed = false;

        [Header("Spine Settings")]
        [Tooltip("Adjust this to easily scale the Spine animation up or down")]
        public float spineScaleMultiplier = 1f;

        private Vector3 initialSpineScale = Vector3.zero;

        /// <summary>
        /// Initializes the special element.
        /// </summary>
        public void Initialize(SpecialElementData data, int x, int y, Vector3 worldPosition, float cellSize)
        {
            if (data == null) return;

            Data = data;
            SpecialElementId = data.elementId;
            BehaviorType = data.behaviorType;
            swapAnimationPlayed = false;
            
            UpdateVisuals(data.sprite, cellSize);
            SetPosition(x, y, worldPosition);

            // Handle Spine Animation scaling and sorting
            if (spineAnimation != null)
            {
                if (initialSpineScale == Vector3.zero)
                {
                    initialSpineScale = spineAnimation.transform.localScale;
                }
                
                // WebGL Bug Fix: Use Spine's internal scaling instead of Unity's Transform scaling
                // to avoid Luna/Playworks rounding float scales to integers on the Transform.
                if (spineAnimation.skeleton != null)
                {
                    spineAnimation.transform.localScale = Vector3.one; // Reset Transform scale
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
                    meshRenderer.sortingOrder = 50; 
                }
            }

            // Handle visual rotation for Vertical Rockets
            if (BehaviorType == SpecialBehaviorType.LineClearVertical && spriteRenderer != null)
            {
                spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }

            if (idleParticles != null && !idleParticles.isPlaying)
            {
                idleParticles.Play();
            }

            PlayCreationAnimation();
        }

        private void PlayCreationAnimation()
        {
            if (spineAnimation != null && !string.IsNullOrEmpty(creationAnimation))
            {
                var animData = spineAnimation.Skeleton.Data.FindAnimation(creationAnimation);
                if (animData == null)
                {
                    Debug.LogError($"[Spine Error] Cannot find Creation Animation '{creationAnimation}' in SkeletonData for {gameObject.name}!");
                    return;
                }

                // Ensure Spine is visible AND active
                spineAnimation.gameObject.SetActive(true);
                var meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
                if (meshRenderer != null) meshRenderer.enabled = true;
                
                // Hide static sprite during Spine animation
                if (spriteRenderer != null) spriteRenderer.enabled = false;

                // Play the creation animation exactly once (no loop)
                var trackEntry = spineAnimation.AnimationState.SetAnimation(0, creationAnimation, false);
                Debug.Log($"[Spine] Successfully playing Creation Animation '{creationAnimation}' on {gameObject.name}");
                
                // Register callback to switch to static sprite when animation ends
                trackEntry.Complete += OnCreationAnimationComplete;

                // If there's an idle animation, play it after creation (this will override the static sprite fallback)
                if (!string.IsNullOrEmpty(idleAnimation))
                {
                    spineAnimation.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
                }
            }
            else if (spineAnimation != null && !string.IsNullOrEmpty(idleAnimation))
            {
                spineAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
            }
        }

        private void OnCreationAnimationComplete(Spine.TrackEntry trackEntry)
        {
            // Only switch to static image if there is NO idle animation configured
            if (string.IsNullOrEmpty(idleAnimation))
            {
                // Hide Spine
                var meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
                if (meshRenderer != null) meshRenderer.enabled = false;

                // Show static Sprite
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                
                Debug.Log($"[Spine] Animation complete, switched to static sprite for {gameObject.name}");
            }
        }

        // Specific animation methods for special elements can go here
        public void PlayTriggerAnimation(bool loop = false)
        {
            if (spineAnimation != null && !string.IsNullOrEmpty(triggerAnimation))
            {
                spineAnimation.AnimationState.SetAnimation(0, triggerAnimation, loop);
            }
        }

        public float GetTriggerAnimationDuration()
        {
            if (spineAnimation != null && !string.IsNullOrEmpty(triggerAnimation))
            {
                var anim = spineAnimation.Skeleton.Data.FindAnimation(triggerAnimation);
                if (anim != null) return anim.Duration;
            }
            return 0f;
        }

        /// <summary>
        /// 获取创建动画的时长（秒）。未配置或找不到动画时返回 0。
        /// 用于转换出的特效在触发前等待创建动画完整播放。
        /// </summary>
        public float GetCreationAnimationDuration()
        {
            if (spineAnimation != null && !string.IsNullOrEmpty(creationAnimation))
            {
                var anim = spineAnimation.Skeleton.Data.FindAnimation(creationAnimation);
                if (anim != null) return anim.Duration;
            }
            return 0f;
        }

        /// <summary>
        /// 停止并隐藏元素原本的待机显示（静态 Sprite 与主 Spine 的 Idle 循环动画）。
        /// 用于交换/组合表演时，双方元素都不再显示待机样子，只保留交换动画层。
        /// </summary>
        public void StopIdleVisuals()
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (spineAnimation != null)
            {
                var mainMeshRenderer = spineAnimation.GetComponent<MeshRenderer>();
                if (mainMeshRenderer != null) mainMeshRenderer.enabled = false;
                if (spineAnimation.AnimationState != null)
                {
                    spineAnimation.AnimationState.ClearTrack(0);
                }
            }
        }

        /// <summary>
        /// 播放交换动画：按组合的对方类型查找绑定并播放（支持独立骨骼 Data 的 Spine 层）。
        /// 返回是否成功播放，供调用方实现"只播一个元素"的规则。
        /// </summary>
        public bool PlaySwapAnimation(SpecialBehaviorType partnerType, bool loop)
        {
            if (swapAnimations == null) return false;

            foreach (SwapAnimationBinding binding in swapAnimations)
            {
                if (binding == null || binding.partnerBehaviorType != partnerType) continue;
                if (TryPlaySwapAnimation(binding, loop)) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取匹配绑定组合的交换动画时长（秒）。未配置或找不到动画时返回 0。
        /// 用于销毁元素前等待交换动画完整播放一遍，避免动画被提前打断。
        /// </summary>
        public float GetSwapAnimationDuration(SpecialBehaviorType partnerType)
        {
            if (swapAnimations == null) return 0f;

            foreach (SwapAnimationBinding binding in swapAnimations)
            {
                if (binding == null || binding.partnerBehaviorType != partnerType) continue;
                SkeletonAnimation target = binding.swapSpine != null ? binding.swapSpine : spineAnimation;
                if (target == null || string.IsNullOrEmpty(binding.animationName)) continue;
                var animData = target.Skeleton.Data.FindAnimation(binding.animationName);
                if (animData != null) return animData.Duration;
            }
            return 0f;
        }

        private bool TryPlaySwapAnimation(SwapAnimationBinding binding, bool loop)
        {
            SkeletonAnimation target = binding.swapSpine != null ? binding.swapSpine : spineAnimation;
            if (target == null || string.IsNullOrEmpty(binding.animationName)) return false;

            var animData = target.Skeleton.Data.FindAnimation(binding.animationName);
            if (animData == null)
            {
                Debug.LogError($"[Spine Error] Cannot find Swap Animation '{binding.animationName}' in SkeletonData for {gameObject.name}!");
                return false;
            }

            // 确保 Spine 可见且激活，并置于主 Spine(50) 之上
            target.gameObject.SetActive(true);
            var meshRenderer = target.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                meshRenderer.sortingLayerName = "Default";
                meshRenderer.sortingOrder = 60;
            }

            // 播放交换动画后隐藏元素原有的静止显示（静态 Sprite 与主 Spine），只保留交换动画层。
            // 复用主 spineAnimation 播交换动画时（swapSpine 留空）则不隐藏。
            if (binding.swapSpine != spineAnimation)
            {
                StopIdleVisuals();
            }

            // WebGL Bug Fix: Transform 缩放会被 Luna 取整，改用 Spine 内部缩放（与主 spine 处理一致）
            if (target.skeleton != null)
            {
                target.transform.localScale = Vector3.one;
                target.skeleton.ScaleX = Mathf.Abs(binding.scaleMultiplier);
                target.skeleton.ScaleY = Mathf.Abs(binding.scaleMultiplier);
            }

            target.AnimationState.SetAnimation(0, binding.animationName, loop);
            swapAnimationPlayed = true;
            return true;
        }

        /// <summary>
        /// 播放交换动画期间的粒子特效（挂载接口）。
        /// 在交换 Spine 动画播放的同时，在指定位置生成 swapParticlePrefab 粒子特效，
        /// 并自动在 duration 秒后销毁（用于魔力球+魔力球交换等组合场景）。
        /// 未配置 swapParticlePrefab 时返回 null，不产生任何效果。
        /// </summary>
        public GameObject PlaySwapParticleEffect(Vector3 position, float duration)
        {
            return Match3EffectPlayer.Play(swapParticlePrefab, position, transform.parent,
                lifetime: duration, stopParticleLoops: false);
        }
    }

    /// <summary>
    /// 交换动画绑定：一个条目对应一种"与对方特殊元素交换"的动画。
    /// swapSpine 可为独立骨骼 Data 的 Spine 子物体（如魔力球交换动画专用骨骼）；
    /// 留空则使用元素自身的主 spineAnimation 骨骼。
    /// </summary>
    [System.Serializable]
    public class SwapAnimationBinding
    {
        [Tooltip("与哪种特殊元素交换时播放此动画")]
        public SpecialBehaviorType partnerBehaviorType;

        [Tooltip("交换动画专用 Spine 层（子物体，独立骨骼 Data）。留空则使用主 spineAnimation")]
        public SkeletonAnimation swapSpine;

        [SpineAnimation(dataField: "swapSpine", fallbackToTextField: true)]
        [Tooltip("交换动画名（拖入 swapSpine 后自动列出该骨骼的动画）")]
        public string animationName;

        [Tooltip("交换动画缩放倍率（1 = 骨骼原始大小）")]
        public float scaleMultiplier = 1f;
    }
}

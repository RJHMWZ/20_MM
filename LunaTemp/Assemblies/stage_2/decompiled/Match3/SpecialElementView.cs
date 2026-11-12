using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class SpecialElementView : ElementView
	{
		[Header("Special Effects")]
		[Tooltip("Optional: Particle system to play when this special element is created or idling")]
		public ParticleSystem idleParticles;

		[Header("Spine Animations")]
		public SkeletonAnimation spineAnimation;

		[SpineAnimation("", "spineAnimation", true, true)]
		public string creationAnimation;

		[SpineAnimation("", "spineAnimation", true, true)]
		public string idleAnimation;

		[SpineAnimation("", "spineAnimation", true, true)]
		public string triggerAnimation;

		[Header("Swap Animations")]
		[Tooltip("交换/组合动画绑定列表：每个条目对应一种组合，支持独立骨骼 Data 的 Spine 子物体层")]
		public List<SwapAnimationBinding> swapAnimations = new List<SwapAnimationBinding>();

		[Header("Swap Particle Effect (挂载接口)")]
		[Tooltip("交换动画播放期间同步播放的粒子特效预制体（如魔力球+魔力球交换时的中心能量特效）。在预制体上挂载，PlaySwapParticleEffect() 播放")]
		public GameObject swapParticlePrefab;

		public bool swapAnimationPlayed = false;

		[Header("Spine Settings")]
		[Tooltip("Adjust this to easily scale the Spine animation up or down")]
		public float spineScaleMultiplier = 1f;

		private Vector3 initialSpineScale = Vector3.zero;

		public SpecialElementData Data { get; private set; }

		public SpecialBehaviorType BehaviorType { get; private set; }

		public string SpecialElementId { get; private set; }

		public void Initialize(SpecialElementData data, int x, int y, Vector3 worldPosition, float cellSize)
		{
			if (data == null)
			{
				return;
			}
			Data = data;
			SpecialElementId = data.elementId;
			BehaviorType = data.behaviorType;
			swapAnimationPlayed = false;
			UpdateVisuals(data.sprite, cellSize);
			SetPosition(x, y, worldPosition);
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
					spineAnimation.transform.localScale = new Vector3(Mathf.Abs(initialSpineScale.x * spineScaleMultiplier), Mathf.Abs(initialSpineScale.y * spineScaleMultiplier), Mathf.Abs(initialSpineScale.z * spineScaleMultiplier));
				}
				MeshRenderer meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.sortingLayerName = "Default";
					meshRenderer.sortingOrder = 50;
				}
			}
			if (BehaviorType == SpecialBehaviorType.LineClearVertical && spriteRenderer != null)
			{
				spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
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
				Spine.Animation animData = spineAnimation.Skeleton.Data.FindAnimation(creationAnimation);
				if (animData == null)
				{
					Debug.LogError("[Spine Error] Cannot find Creation Animation '" + creationAnimation + "' in SkeletonData for " + base.gameObject.name + "!");
					return;
				}
				spineAnimation.gameObject.SetActive(true);
				MeshRenderer meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.enabled = true;
				}
				if (spriteRenderer != null)
				{
					spriteRenderer.enabled = false;
				}
				TrackEntry trackEntry = spineAnimation.AnimationState.SetAnimation(0, creationAnimation, false);
				Debug.Log("[Spine] Successfully playing Creation Animation '" + creationAnimation + "' on " + base.gameObject.name);
				trackEntry.Complete += OnCreationAnimationComplete;
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

		private void OnCreationAnimationComplete(TrackEntry trackEntry)
		{
			if (string.IsNullOrEmpty(idleAnimation))
			{
				MeshRenderer meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.enabled = false;
				}
				if (spriteRenderer != null)
				{
					spriteRenderer.enabled = true;
				}
				Debug.Log("[Spine] Animation complete, switched to static sprite for " + base.gameObject.name);
			}
		}

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
				Spine.Animation anim = spineAnimation.Skeleton.Data.FindAnimation(triggerAnimation);
				if (anim != null)
				{
					return anim.Duration;
				}
			}
			return 0f;
		}

		public float GetCreationAnimationDuration()
		{
			if (spineAnimation != null && !string.IsNullOrEmpty(creationAnimation))
			{
				Spine.Animation anim = spineAnimation.Skeleton.Data.FindAnimation(creationAnimation);
				if (anim != null)
				{
					return anim.Duration;
				}
			}
			return 0f;
		}

		public void StopIdleVisuals()
		{
			if (spriteRenderer != null)
			{
				spriteRenderer.enabled = false;
			}
			if (spineAnimation != null)
			{
				MeshRenderer mainMeshRenderer = spineAnimation.GetComponent<MeshRenderer>();
				if (mainMeshRenderer != null)
				{
					mainMeshRenderer.enabled = false;
				}
				if (spineAnimation.AnimationState != null)
				{
					spineAnimation.AnimationState.ClearTrack(0);
				}
			}
		}

		public bool PlaySwapAnimation(SpecialBehaviorType partnerType, bool loop)
		{
			if (swapAnimations == null)
			{
				return false;
			}
			foreach (SwapAnimationBinding binding in swapAnimations)
			{
				if (binding == null || binding.partnerBehaviorType != partnerType || !TryPlaySwapAnimation(binding, loop))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		public float GetSwapAnimationDuration(SpecialBehaviorType partnerType)
		{
			if (swapAnimations == null)
			{
				return 0f;
			}
			foreach (SwapAnimationBinding binding in swapAnimations)
			{
				if (binding == null || binding.partnerBehaviorType != partnerType)
				{
					continue;
				}
				SkeletonAnimation target = ((binding.swapSpine != null) ? binding.swapSpine : spineAnimation);
				if (!(target == null) && !string.IsNullOrEmpty(binding.animationName))
				{
					Spine.Animation animData = target.Skeleton.Data.FindAnimation(binding.animationName);
					if (animData != null)
					{
						return animData.Duration;
					}
				}
			}
			return 0f;
		}

		private bool TryPlaySwapAnimation(SwapAnimationBinding binding, bool loop)
		{
			SkeletonAnimation target = ((binding.swapSpine != null) ? binding.swapSpine : spineAnimation);
			if (target == null || string.IsNullOrEmpty(binding.animationName))
			{
				return false;
			}
			Spine.Animation animData = target.Skeleton.Data.FindAnimation(binding.animationName);
			if (animData == null)
			{
				Debug.LogError("[Spine Error] Cannot find Swap Animation '" + binding.animationName + "' in SkeletonData for " + base.gameObject.name + "!");
				return false;
			}
			target.gameObject.SetActive(true);
			MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = true;
				meshRenderer.sortingLayerName = "Default";
				meshRenderer.sortingOrder = 60;
			}
			if (binding.swapSpine != spineAnimation)
			{
				StopIdleVisuals();
			}
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

		public GameObject PlaySwapParticleEffect(Vector3 position, float duration)
		{
			return Match3EffectPlayer.Play(swapParticlePrefab, position, base.transform.parent, null, duration, null, false);
		}
	}
}

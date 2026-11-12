using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class ObstacleView : ElementView
	{
		[Header("Spine Settings")]
		public SkeletonAnimation spineAnimation;

		public float spineScaleMultiplier = 1f;

		private Vector3 initialSpineScale = Vector3.zero;

		public ObstacleData Data { get; private set; }

		public int CurrentHealth { get; private set; }

		public void Initialize(ObstacleData data, int x, int y, Vector3 worldPosition, float cellSize)
		{
			if (data == null)
			{
				return;
			}
			Data = data;
			CurrentHealth = data.maxHealth;
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
					meshRenderer.sortingOrder = 40;
				}
			}
			base.transform.localScale = Vector3.one * Mathf.Abs(cellSize);
			if (data.visualOffset != Vector2.zero)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					Transform child = base.transform.GetChild(i);
					Vector3 currentPos = child.localPosition;
					child.localPosition = new Vector3(currentPos.x + data.visualOffset.x, currentPos.y + data.visualOffset.y, currentPos.z);
				}
			}
			UpdateStateVisual();
		}

		public void TakeDamage()
		{
			if (CurrentHealth <= 0)
			{
				return;
			}
			CurrentHealth--;
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
				base.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
			}
		}

		private void PlayTransitionAndNextState(int transitionIndex)
		{
			if (spineAnimation == null)
			{
				return;
			}
			int nextStateIndex = Data.maxHealth - CurrentHealth;
			string nextStateAnim = ((Data.stateAnimations != null && nextStateIndex < Data.stateAnimations.Count) ? Data.stateAnimations[nextStateIndex] : null);
			string nextStateSkin = ((Data.stateSkins != null && nextStateIndex < Data.stateSkins.Count) ? Data.stateSkins[nextStateIndex] : null);
			if (Data.transitionAnimations != null && transitionIndex >= 0 && transitionIndex < Data.transitionAnimations.Count)
			{
				string transitionAnim = Data.transitionAnimations[transitionIndex];
				if (!string.IsNullOrEmpty(transitionAnim))
				{
					if (!string.IsNullOrEmpty(nextStateSkin))
					{
						spineAnimation.skeleton.SetSkin(nextStateSkin);
						spineAnimation.skeleton.SetSlotsToSetupPose();
					}
					spineAnimation.AnimationState.SetAnimation(0, transitionAnim, false);
					if (!string.IsNullOrEmpty(nextStateAnim))
					{
						spineAnimation.AnimationState.AddAnimation(0, nextStateAnim, true, 0f);
					}
					return;
				}
			}
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
			if (!(spineAnimation != null))
			{
				return;
			}
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
			GameObject effect = Object.Instantiate(prefab, base.transform.position, Quaternion.identity);
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

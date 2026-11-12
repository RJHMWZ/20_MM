using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class GeneratorView : ElementView
	{
		[Header("Spine Settings")]
		public SkeletonAnimation spineAnimation;

		public float spineScaleMultiplier = 1f;

		private Vector3 initialSpineScale = Vector3.zero;

		private int currentGenerations = 0;

		private int currentHits = 0;

		public GeneratorData Data { get; private set; }

		public void Initialize(GeneratorData data, int x, int y, Vector3 worldPosition, float cellSize)
		{
			if (data == null)
			{
				return;
			}
			Data = data;
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
					meshRenderer.sortingOrder = 30;
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
			PlayIdle();
		}

		public void PlayIdle()
		{
			UpdateStateVisual();
		}

		private void UpdateStateVisual(bool playAnimation = true)
		{
			if (spineAnimation == null)
			{
				return;
			}
			if (Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations)
			{
				if (!string.IsNullOrEmpty(Data.emptyAnimation))
				{
					spineAnimation.AnimationState.SetAnimation(0, Data.emptyAnimation, true);
				}
				return;
			}
			if (Data.stateSkins != null && currentHits < Data.stateSkins.Count && !string.IsNullOrEmpty(Data.stateSkins[currentHits]))
			{
				spineAnimation.skeleton.SetSkin(Data.stateSkins[currentHits]);
				spineAnimation.skeleton.SetSlotsToSetupPose();
			}
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
			if (Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations)
			{
				return false;
			}
			currentHits++;
			int targetHits = Mathf.Max(1, Data.hitsToGenerate);
			if (currentHits >= targetHits)
			{
				currentHits = 0;
				return true;
			}
			if (AudioManager.Instance != null && Data.hitSound != null)
			{
				AudioManager.Instance.PlayGeneratorSound(Data.hitSound);
			}
			base.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
			string transitionAnim = null;
			if (Data.transitionAnimations != null && currentHits - 1 < Data.transitionAnimations.Count)
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
				UpdateStateVisual(false);
				spineAnimation.AnimationState.SetAnimation(0, transitionAnim, false);
				if (!string.IsNullOrEmpty(nextIdle))
				{
					spineAnimation.AnimationState.AddAnimation(0, nextIdle, true, 0f);
				}
			}
			else
			{
				UpdateStateVisual();
			}
			return false;
		}

		public void TriggerGeneration(int targetIndex, Vector3 uiTargetPos)
		{
			if (Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations)
			{
				return;
			}
			currentGenerations++;
			bool isEmpty = Data.maxGenerations > 0 && currentGenerations >= Data.maxGenerations;
			if (AudioManager.Instance != null && Data.generateSound != null)
			{
				AudioManager.Instance.PlayGeneratorSound(Data.generateSound);
			}
			if (spineAnimation != null && !string.IsNullOrEmpty(Data.workingAnimation))
			{
				TrackEntry track = spineAnimation.AnimationState.SetAnimation(0, Data.workingAnimation, false);
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
						UpdateStateVisual(false);
						spineAnimation.AnimationState.AddAnimation(0, nextIdle, true, 0f);
					}
				}
			}
			if (!(Data.generatedItemPrefab != null))
			{
				return;
			}
			GameObject paper = Object.Instantiate(Data.generatedItemPrefab, base.transform.position, Quaternion.identity);
			if (base.transform.parent != null)
			{
				paper.transform.SetParent(base.transform.parent, true);
			}
			paper.transform.localScale = Vector3.zero;
			float finalPaperScale = targetCellSize * 0.7f;
			MeshRenderer paperMesh = paper.GetComponentInChildren<MeshRenderer>();
			if (paperMesh != null)
			{
				paperMesh.sortingLayerName = "Default";
				paperMesh.sortingOrder = 100;
			}
			SpriteRenderer paperSprite = paper.GetComponentInChildren<SpriteRenderer>();
			if (paperSprite != null)
			{
				paperSprite.sortingLayerName = "Default";
				paperSprite.sortingOrder = 100;
			}
			Vector3 jumpPos = base.transform.position + new Vector3(0f, targetCellSize * 0.6f, 0f);
			Sequence seq = DOTween.Sequence();
			seq.Append(paper.transform.DOJump(jumpPos, targetCellSize * 0.3f, 1, 0.4f));
			seq.Join(paper.transform.DOScale(Vector3.one * finalPaperScale, 0.4f).SetEase(Ease.OutBack));
			if (targetIndex >= 0 && uiTargetPos != Vector3.zero)
			{
				seq.AppendInterval(0.1f);
				seq.AppendCallback(delegate
				{
					if (Data.targetIcon != null)
					{
						if (paperMesh != null)
						{
							paperMesh.enabled = false;
						}
						if (paperSprite != null)
						{
							paperSprite.sprite = Data.targetIcon;
							paperSprite.enabled = true;
							float x = Data.targetIcon.bounds.size.x;
							float num = targetCellSize * 0.95f / x;
							paperSprite.transform.localScale = new Vector3(num, num, 1f);
						}
					}
				});
				Vector3 midPoint = jumpPos + (uiTargetPos - jumpPos) / 2f + new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), 0f);
				Vector3[] path = new Vector3[2] { midPoint, uiTargetPos };
				seq.Append(paper.transform.DOPath(path, 0.6f, PathType.CatmullRom).SetEase(Ease.InOutQuad));
				seq.Join(paper.transform.DOScale(paper.transform.localScale * 0.5f, 0.6f));
				seq.OnComplete(delegate
				{
					if (AudioManager.Instance != null)
					{
						AudioManager.Instance.PlayCollectSound();
					}
					Object.Destroy(paper);
				});
			}
			else
			{
				seq.AppendInterval(0.5f);
				seq.Append(paper.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
				seq.OnComplete(delegate
				{
					Object.Destroy(paper);
				});
			}
		}
	}
}

using System;
using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Match3
{
	public class MotherView : ElementView
	{
		[Header("UI")]
		[Tooltip("Text component to show remaining count")]
		public TextMeshPro countText;

		[Header("Spine Settings")]
		public SkeletonAnimation spineAnimation;

		public float spineScaleMultiplier = 1f;

		[Header("Rendering")]
		[Tooltip("Sorting order for the Spine animation")]
		public int spineSortingOrder = 30;

		[Tooltip("Sorting order for the text (should be higher than spine)")]
		public int textSortingOrder = 35;

		private Vector3 initialSpineScale = Vector3.zero;

		public MotherData Data { get; private set; }

		public int CurrentCount { get; private set; }

		public int PendingReceiveCount { get; set; } = 0;


		public void Initialize(MotherData data, int x, int y, Vector3 worldPosition, float cellSize, float cellSpacing)
		{
			if (data == null)
			{
				return;
			}
			Data = data;
			CurrentCount = data.requiredCount;
			SetPosition(x, y, worldPosition);
			int offsetX = ((data.gridWidth > 1 || data.gridHeight > 1) ? 0 : 0);
			base.transform.position = worldPosition;
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
					meshRenderer.sortingOrder = spineSortingOrder;
				}
			}
			base.transform.localScale = Vector3.one * Mathf.Abs(cellSize);
			float localOffsetX = (float)(data.gridWidth - 1) / 2f * (1f + cellSpacing / cellSize) + data.visualOffset.x;
			float localOffsetY = (float)(data.gridHeight - 1) / 2f * (1f + cellSpacing / cellSize) + data.visualOffset.y;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (child.localPosition.x < 0.1f && child.localPosition.y < 0.1f && localOffsetX > 0f)
				{
					Vector3 currentPos = child.localPosition;
					child.localPosition = new Vector3(currentPos.x + localOffsetX, currentPos.y + localOffsetY, currentPos.z);
				}
			}
			if (countText != null)
			{
				countText.GetComponent<MeshRenderer>().sortingLayerName = "Default";
				countText.GetComponent<MeshRenderer>().sortingOrder = textSortingOrder;
			}
			UpdateText();
			PlayIdle();
		}

		private void UpdateText()
		{
			if (countText != null)
			{
				countText.text = ((CurrentCount > 0) ? CurrentCount.ToString() : "");
			}
		}

		public void PlayIdle()
		{
			if (!(spineAnimation != null) || string.IsNullOrEmpty(Data.idleAnimation))
			{
				return;
			}
			try
			{
				if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.idleAnimation) != null)
				{
					spineAnimation.AnimationState.SetAnimation(0, Data.idleAnimation, true);
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("Spine animation error (idle): " + e.Message);
			}
		}

		public void ReceiveChild(Action onCompleteCallback)
		{
			if (CurrentCount <= 0)
			{
				return;
			}
			CurrentCount--;
			UpdateText();
			if (AudioManager.Instance != null && Data.receiveSound != null)
			{
				AudioManager.Instance.PlayGeneratorSound(Data.receiveSound);
			}
			if (CurrentCount > 0)
			{
				if (spineAnimation != null && !string.IsNullOrEmpty(Data.receiveAnimation))
				{
					try
					{
						if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.receiveAnimation) != null)
						{
							spineAnimation.AnimationState.SetAnimation(0, Data.receiveAnimation, false);
							if (!string.IsNullOrEmpty(Data.idleAnimation) && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.idleAnimation) != null)
							{
								spineAnimation.AnimationState.AddAnimation(0, Data.idleAnimation, true, 0f);
							}
						}
					}
					catch (Exception e2)
					{
						Debug.LogWarning("Spine animation error (receive): " + e2.Message);
					}
				}
				if (spineAnimation != null)
				{
					spineAnimation.transform.DOKill(true);
					spineAnimation.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
				}
				return;
			}
			if (AudioManager.Instance != null && Data.completeSound != null)
			{
				AudioManager.Instance.PlayGeneratorSound(Data.completeSound);
			}
			float delay = 0f;
			if (spineAnimation != null && !string.IsNullOrEmpty(Data.completeAnimation))
			{
				try
				{
					if (spineAnimation.skeletonDataAsset != null && spineAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(Data.completeAnimation) != null)
					{
						TrackEntry track = spineAnimation.AnimationState.SetAnimation(0, Data.completeAnimation, false);
						if (track != null)
						{
							delay = track.Animation.Duration;
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning("Spine animation error (complete): " + e.Message);
				}
			}
			if (countText != null)
			{
				countText.gameObject.SetActive(false);
			}
			if (delay <= 0.05f)
			{
				delay = 0.05f;
			}
			DOVirtual.DelayedCall(delay, delegate
			{
				if (this != null && base.gameObject != null)
				{
					onCompleteCallback?.Invoke();
				}
			});
		}
	}
}

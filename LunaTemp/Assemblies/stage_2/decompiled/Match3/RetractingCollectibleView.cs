using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class RetractingCollectibleView : ElementView
	{
		[Header("Collectible Setup")]
		[Tooltip("Parent transform to hold instantiated body parts")]
		public Transform bodyContainer;

		[Tooltip("Multiplier for the width (thickness) of the body and tail parts")]
		[Range(0.1f, 2f)]
		public float bodyThicknessMultiplier = 0.95f;

		[Header("Spine Integration (Optional)")]
		[Tooltip("If assigned, Spine animations will be used instead of dynamic SpriteRenderers for the body.")]
		public SkeletonAnimation spineAnimation;

		public string idleAnimationPrefix = "idle_";

		public string retractAnimationPrefix = "retract_";

		public float spineScaleMultiplier = 1f;

		[Tooltip("收缩动画播放速度倍率。strike_* 动画原始时长 0.5s，默认 2.5 倍加速至约 0.2s，与普通消除的销毁节奏一致，避免下落等待过长。")]
		public float retractSpeedMultiplier = 2.5f;

		[Tooltip("Fine-tune the pivot offset. If the Spine pivot is at the exact center of the entire body, leave X at 0. If it's at the Head, you may need to adjust this.")]
		public float spineOffsetX = 0f;

		public float spineOffsetY = 0f;

		[Tooltip("Base rotation offset for the Spine artwork. If your Spine is drawn pointing UP instead of RIGHT, set this to -90.")]
		public float spineBaseRotation = 0f;

		private List<SpriteRenderer> bodyParts = new List<SpriteRenderer>();

		private float _cellSize;

		private float _cellSpacing;

		private float _rotationZ;

		private Vector3 _initialSpineScale = Vector3.zero;

		private float _originalSpineTimeScale = 1f;

		public Direction CurrentDirection { get; private set; }

		public int CurrentLength { get; private set; }

		public CollectibleElementData collectibleData { get; private set; }

		public bool IsRetracting { get; private set; }

		public void Initialize(CollectibleElementData data, int tailX, int tailY, int headX, int headY, Vector3 tailWorldPos, float cellSize, float cellSpacing)
		{
			if (data == null)
			{
				return;
			}
			collectibleData = data;
			Vector2Int dirVec = new Vector2Int(headX - tailX, headY - tailY);
			CurrentLength = Mathf.Max(Mathf.Abs(dirVec.x), Mathf.Abs(dirVec.y)) + 1;
			if (dirVec.x > 0)
			{
				CurrentDirection = Direction.Right;
			}
			else if (dirVec.x < 0)
			{
				CurrentDirection = Direction.Left;
			}
			else if (dirVec.y > 0)
			{
				CurrentDirection = Direction.Up;
			}
			else
			{
				CurrentDirection = Direction.Down;
			}
			_cellSize = cellSize;
			_cellSpacing = cellSpacing;
			UpdateVisuals((data.tailSprite != null) ? data.tailSprite : data.bodySprite, cellSize);
			SetPosition(tailX, tailY, tailWorldPos);
			float explicitAngle = 0f;
			switch (CurrentDirection)
			{
			case Direction.Right:
				explicitAngle = 0f;
				break;
			case Direction.Left:
				explicitAngle = -180f;
				break;
			case Direction.Down:
				explicitAngle = -90f;
				break;
			case Direction.Up:
				explicitAngle = -270f;
				break;
			}
			_rotationZ = explicitAngle + spineBaseRotation;
			Debug.Log($"[RetractingCollectibleView] Initialize: dirVec={dirVec}, CurrentDirection={CurrentDirection}, explicitAngle={explicitAngle}, _rotationZ={_rotationZ}");
			base.transform.rotation = Quaternion.Euler(0f, 0f, _rotationZ);
			base.transform.localEulerAngles = new Vector3(0f, 0f, _rotationZ);
			if (spriteRenderer != null && spriteRenderer.transform != base.transform)
			{
				spriteRenderer.transform.localRotation = Quaternion.identity;
			}
			if (spineAnimation == null)
			{
				spineAnimation = GetComponentInChildren<SkeletonAnimation>();
			}
			if (spineAnimation != null)
			{
				if (_initialSpineScale == Vector3.zero)
				{
					_initialSpineScale = spineAnimation.transform.localScale;
				}
				spineAnimation.gameObject.SetActive(true);
				SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
				SpriteRenderer[] array = allSprites;
				foreach (SpriteRenderer sr in array)
				{
					sr.enabled = false;
				}
				spineAnimation.transform.localScale = new Vector3(Mathf.Abs(_initialSpineScale.x * spineScaleMultiplier * cellSize), Mathf.Abs(_initialSpineScale.y * spineScaleMultiplier * cellSize), Mathf.Abs(_initialSpineScale.z * spineScaleMultiplier * cellSize));
				if (spineAnimation.transform != base.transform)
				{
					spineAnimation.transform.localRotation = Quaternion.identity;
				}
				spineAnimation.transform.localPosition = new Vector3(spineOffsetX, spineOffsetY, 0f);
				MeshRenderer meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.sortingLayerName = "Default";
					meshRenderer.sortingOrder = 30;
				}
				int displayLength = Mathf.Max(1, CurrentLength - 1);
				PlaySpineAnimation(idleAnimationPrefix + displayLength, true);
			}
			else
			{
				GenerateBody(data, _cellSize, _cellSpacing, _rotationZ);
			}
		}

		private void PlaySpineAnimation(string animName, bool loop)
		{
			if (spineAnimation == null)
			{
				return;
			}
			Spine.Animation animData = spineAnimation.Skeleton.Data.FindAnimation(animName);
			if (animData != null)
			{
				spineAnimation.AnimationState.SetAnimation(0, animName, loop);
				return;
			}
			string avail = "";
			foreach (Spine.Animation a in spineAnimation.Skeleton.Data.Animations)
			{
				avail = avail + a.Name + ", ";
			}
			Debug.LogError("[Spine Error] Cannot find animation '" + animName + "' on " + base.gameObject.name + ". Available animations are: " + avail);
		}

		public void UpdateCellSize(float cellSize, float cellSpacing)
		{
			_cellSize = cellSize;
			_cellSpacing = cellSpacing;
			if (!(collectibleData != null))
			{
				return;
			}
			UpdateVisuals((collectibleData.tailSprite != null) ? collectibleData.tailSprite : collectibleData.bodySprite, cellSize);
			if (spineAnimation == null)
			{
				spineAnimation = GetComponentInChildren<SkeletonAnimation>();
			}
			if (spineAnimation == null)
			{
				GenerateBody(collectibleData, _cellSize, _cellSpacing, _rotationZ);
				return;
			}
			if (_initialSpineScale == Vector3.zero)
			{
				_initialSpineScale = spineAnimation.transform.localScale;
			}
			spineAnimation.transform.localScale = new Vector3(Mathf.Abs(_initialSpineScale.x * spineScaleMultiplier * cellSize), Mathf.Abs(_initialSpineScale.y * spineScaleMultiplier * cellSize), Mathf.Abs(_initialSpineScale.z * spineScaleMultiplier * cellSize));
			SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
			SpriteRenderer[] array = allSprites;
			foreach (SpriteRenderer sr in array)
			{
				sr.enabled = false;
			}
		}

		private void GenerateBody(CollectibleElementData data, float cellSize, float cellSpacing, float rotationZ)
		{
			foreach (SpriteRenderer part in bodyParts)
			{
				if (part != null)
				{
					Object.Destroy(part.gameObject);
				}
			}
			bodyParts.Clear();
			Vector3 offsetStep = Vector3.zero;
			float stepDistance = cellSize + cellSpacing;
			offsetStep = Vector3.right * stepDistance;
			for (int i = 1; i < CurrentLength; i++)
			{
				GameObject partObj = new GameObject("BodyPart_" + i);
				partObj.transform.SetParent((bodyContainer == null) ? base.transform : bodyContainer);
				partObj.transform.localPosition = offsetStep * i;
				partObj.transform.localRotation = Quaternion.identity;
				SpriteRenderer sr = partObj.AddComponent<SpriteRenderer>();
				sr.sortingLayerID = spriteRenderer.sortingLayerID;
				sr.sortingOrder = spriteRenderer.sortingOrder + 1;
				Sprite targetSprite = (sr.sprite = ((i == CurrentLength - 1 && data.headSprite != null) ? data.headSprite : data.bodySprite));
				if (targetSprite != null)
				{
					float spriteWidth = targetSprite.bounds.size.x;
					float spriteHeight = targetSprite.bounds.size.y;
					float targetScaleX = cellSize * bodyThicknessMultiplier / spriteWidth;
					float targetScaleY = ((i != CurrentLength - 1) ? ((cellSize + cellSpacing + 0.15f) / spriteHeight) : (cellSize * bodyThicknessMultiplier / spriteHeight));
					Vector3 lossy = base.transform.lossyScale;
					float localScaleX = ((lossy.x != 0f) ? (targetScaleX / lossy.x) : targetScaleX);
					float localScaleY = ((lossy.y != 0f) ? (targetScaleY / lossy.y) : targetScaleY);
					partObj.transform.localScale = new Vector3(localScaleX, localScaleY, 1f);
				}
				bodyParts.Add(sr);
			}
		}

		public void RetractOneCell()
		{
			if (CurrentLength <= 0)
			{
				return;
			}
			int oldLength = CurrentLength;
			if (CurrentLength <= 2)
			{
				CurrentLength = 0;
			}
			else
			{
				CurrentLength--;
			}
			if (spineAnimation != null)
			{
				spineAnimation.gameObject.SetActive(true);
				SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
				SpriteRenderer[] array = allSprites;
				foreach (SpriteRenderer sr in array)
				{
					sr.enabled = false;
				}
				spineAnimation.transform.localPosition = new Vector3(spineOffsetX, spineOffsetY, 0f);
				int displayOldLength = Mathf.Max(1, oldLength - 1);
				TrackEntry track = spineAnimation.AnimationState.SetAnimation(0, retractAnimationPrefix + displayOldLength, false);
				if (track != null)
				{
					if (!IsRetracting)
					{
						_originalSpineTimeScale = spineAnimation.timeScale;
					}
					IsRetracting = true;
					spineAnimation.timeScale = _originalSpineTimeScale * retractSpeedMultiplier;
					track.Complete += delegate
					{
						IsRetracting = false;
						if (spineAnimation != null)
						{
							spineAnimation.timeScale = _originalSpineTimeScale;
						}
					};
				}
				if (CurrentLength > 0)
				{
					int displayNewLength = Mathf.Max(1, CurrentLength - 1);
					spineAnimation.AnimationState.AddAnimation(0, idleAnimationPrefix + displayNewLength, true, 0f);
				}
			}
			if (CurrentLength > 0)
			{
				if (spineAnimation == null)
				{
					GenerateBody(collectibleData, _cellSize, _cellSpacing, _rotationZ);
				}
				else
				{
					spineAnimation.gameObject.SetActive(true);
				}
				return;
			}
			if (spriteRenderer != null)
			{
				spriteRenderer.enabled = false;
			}
			foreach (SpriteRenderer part in bodyParts)
			{
				if (part != null)
				{
					part.enabled = false;
				}
			}
		}
	}
}

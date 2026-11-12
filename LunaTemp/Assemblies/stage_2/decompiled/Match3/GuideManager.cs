using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
	public class GuideManager : MonoBehaviour
	{
		private BoardManager boardManager;

		private SpriteRenderer darkMask;

		private SpriteRenderer fingerSpriteRenderer;

		private bool isGuideActive = false;

		public float delayToShowGuide = 1.2f;

		private float idleTimer = 0f;

		private float pendingTutorialDelay = 0.5f;

		private List<ElementView> highlightedElements = new List<ElementView>();

		private List<int> originalElementOrders = new List<int>();

		private List<SpriteRenderer> highlightedBgTiles = new List<SpriteRenderer>();

		private List<int> originalBgOrders = new List<int>();

		private List<MeshRenderer> highlightedMeshes = new List<MeshRenderer>();

		private List<int> originalMeshOrders = new List<int>();

		private List<TutorialStep> tutorialSteps;

		private int currentStepIndex = 0;

		private Sprite defaultFingerSprite;

		private GameObject currentPopupInstance;

		private Canvas popupCanvasContainer;

		private Vector2 originalPopupAnchoredPos;

		private Vector3 originalPopupLocalPos;

		public bool IsGuideActive => isGuideActive;

		public bool HasPendingTutorial => tutorialSteps != null && currentStepIndex < tutorialSteps.Count;

		public Vector2Int CurrentTutorialFrom => (tutorialSteps != null && currentStepIndex < tutorialSteps.Count) ? tutorialSteps[currentStepIndex].tutorialSwipeFrom : Vector2Int.zero;

		public Vector2Int CurrentTutorialTo => (tutorialSteps != null && currentStepIndex < tutorialSteps.Count) ? tutorialSteps[currentStepIndex].tutorialSwipeTo : Vector2Int.zero;

		public void Initialize(BoardManager board, Sprite fingerSprite, List<TutorialStep> steps)
		{
			boardManager = board;
			defaultFingerSprite = fingerSprite;
			tutorialSteps = steps;
			currentStepIndex = 0;
			CreateMask();
			CreateFinger(fingerSprite);
		}

		private void CreateMask()
		{
			GameObject maskObj = new GameObject("GuideMask");
			maskObj.transform.SetParent(base.transform);
			maskObj.transform.position = new Vector3(0f, 0f, -1f);
			darkMask = maskObj.AddComponent<SpriteRenderer>();
			Texture2D tex = Texture2D.whiteTexture;
			Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
			darkMask.sprite = sprite;
			darkMask.color = new Color(0f, 0f, 0f, 0.7f);
			darkMask.sortingOrder = 25000;
			float height = Camera.main.orthographicSize * 2f;
			float width = height * Camera.main.aspect;
			float maxDim = Mathf.Max(width, height) * 100f * 2f;
			maskObj.transform.localScale = new Vector3(maxDim, maxDim, 1f);
			maskObj.SetActive(false);
		}

		private void CreateFinger(Sprite customSprite)
		{
			GameObject fingerObj = new GameObject("GuideFinger");
			fingerObj.transform.SetParent(base.transform);
			fingerSpriteRenderer = fingerObj.AddComponent<SpriteRenderer>();
			if (customSprite != null)
			{
				fingerSpriteRenderer.sprite = customSprite;
			}
			else
			{
				Texture2D tex = new Texture2D(64, 64);
				Color[] colors = new Color[4096];
				float radius = 30f;
				Vector2 center = new Vector2(32f, 32f);
				for (int i = 0; i < 4096; i++)
				{
					int x = i % 64;
					int y = i / 64;
					float dist = Vector2.Distance(new Vector2(x, y), center);
					colors[i] = ((dist <= radius) ? Color.white : Color.clear);
				}
				tex.SetPixels(colors);
				tex.Apply();
				Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 100f);
				fingerSpriteRenderer.sprite = sprite;
			}
			fingerSpriteRenderer.sortingOrder = 50;
			fingerObj.transform.localScale = Vector3.one;
			fingerObj.SetActive(false);
		}

		private void Update()
		{
			if (boardManager == null)
			{
				return;
			}
			if (boardManager.gameManager != null && boardManager.gameManager.isGameOver)
			{
				if (isGuideActive)
				{
					Object.Destroy(this);
				}
			}
			else if (boardManager.currentState == BoardManager.BoardState.Waiting)
			{
				if (HasPendingTutorial && !isGuideActive)
				{
					idleTimer += Time.deltaTime;
					float delay = ((currentStepIndex == 0) ? delayToShowGuide : pendingTutorialDelay);
					if (idleTimer >= delay)
					{
						ShowGuide();
					}
				}
				else
				{
					if (!isGuideActive || !HasPendingTutorial)
					{
						return;
					}
					TutorialStep currentStep = tutorialSteps[currentStepIndex];
					if (currentPopupInstance != null)
					{
						RectTransform rectTransform = currentPopupInstance.GetComponent<RectTransform>();
						if (rectTransform != null)
						{
							rectTransform.anchoredPosition = originalPopupAnchoredPos + currentStep.popupOffset;
						}
						else
						{
							currentPopupInstance.transform.localPosition = originalPopupLocalPos + new Vector3(currentStep.popupOffset.x, currentStep.popupOffset.y, 0f);
						}
					}
				}
			}
			else
			{
				idleTimer = 0f;
			}
		}

		private void ShowGuide()
		{
			if (tutorialSteps == null || currentStepIndex >= tutorialSteps.Count)
			{
				EndTutorial();
				return;
			}
			TutorialStep currentStep = tutorialSteps[currentStepIndex];
			Vector2Int tutorialFrom = currentStep.tutorialSwipeFrom;
			Vector2Int tutorialTo = currentStep.tutorialSwipeTo;
			if (!boardManager.IsValidTutorialMove(tutorialFrom, tutorialTo))
			{
				Debug.LogWarning($"[GuideManager] Tutorial move from {tutorialFrom} to {tutorialTo} is NOT valid! Guide destroyed.");
				EndTutorial();
				return;
			}
			isGuideActive = true;
			if (currentStep.showMask)
			{
				darkMask.gameObject.SetActive(true);
			}
			if (currentStep.showMask && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
			{
				boardManager.gameManager.uiManager.SetTutorialUIMask(true);
			}
			highlightedElements.Clear();
			originalElementOrders.Clear();
			highlightedBgTiles.Clear();
			originalBgOrders.Clear();
			highlightedMeshes.Clear();
			originalMeshOrders.Clear();
			List<Vector2Int> highlightCells = new List<Vector2Int>();
			if (currentStep.tutorialHighlightCells != null)
			{
				foreach (Vector2Int h in currentStep.tutorialHighlightCells)
				{
					if (h.x >= 0 && h.y >= 0)
					{
						highlightCells.Add(h);
					}
				}
			}
			if (highlightCells.Count == 0)
			{
				highlightCells.Add(tutorialFrom);
				if (tutorialFrom != tutorialTo)
				{
					highlightCells.Add(tutorialTo);
				}
			}
			if (currentStep.showMask)
			{
				foreach (Vector2Int pos in highlightCells)
				{
					if (pos.x < 0 || pos.x >= boardManager.activeElements.GetLength(0) || pos.y < 0 || pos.y >= boardManager.activeElements.GetLength(1))
					{
						continue;
					}
					ElementView element = boardManager.activeElements[pos.x, pos.y];
					if (element != null)
					{
						if (element.spriteRenderer != null)
						{
							highlightedElements.Add(element);
							originalElementOrders.Add(element.spriteRenderer.sortingOrder);
							element.spriteRenderer.sortingOrder = 25002;
						}
						if (element is SpecialElementView specialView && specialView.spineAnimation != null)
						{
							MeshRenderer meshRenderer = specialView.spineAnimation.GetComponent<MeshRenderer>();
							if (meshRenderer != null)
							{
								highlightedMeshes.Add(meshRenderer);
								originalMeshOrders.Add(meshRenderer.sortingOrder);
								meshRenderer.sortingOrder = 25002;
							}
						}
					}
					Transform bgTile = boardManager.backgroundContainer.Find($"Tile_{pos.x}_{pos.y}");
					if (bgTile != null)
					{
						SpriteRenderer sr = bgTile.GetComponent<SpriteRenderer>();
						if (sr != null)
						{
							highlightedBgTiles.Add(sr);
							originalBgOrders.Add(sr.sortingOrder);
							sr.sortingOrder = 25001;
						}
					}
				}
			}
			if (currentPopupInstance != null)
			{
				Object.Destroy(currentPopupInstance);
				currentPopupInstance = null;
			}
			if (currentStep.showPopup && currentStep.popupPrefab != null)
			{
				Transform parentTransform = base.transform;
				if (boardManager != null && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
				{
					parentTransform = boardManager.gameManager.uiManager.transform;
				}
				if (popupCanvasContainer == null)
				{
					GameObject canvasObj = new GameObject("GuidePopupCanvas");
					canvasObj.transform.SetParent(parentTransform, false);
					RectTransform rt = canvasObj.AddComponent<RectTransform>();
					rt.anchorMin = Vector2.zero;
					rt.anchorMax = Vector2.one;
					rt.sizeDelta = Vector2.zero;
					rt.anchoredPosition = Vector2.zero;
					rt.localScale = Vector3.one;
					popupCanvasContainer = canvasObj.AddComponent<Canvas>();
					popupCanvasContainer.overrideSorting = true;
					popupCanvasContainer.sortingOrder = 32000;
					popupCanvasContainer.sortingLayerName = "Default";
					canvasObj.AddComponent<GraphicRaycaster>();
				}
				currentPopupInstance = Object.Instantiate(currentStep.popupPrefab, popupCanvasContainer.transform, false);
				RectTransform rectTransform = currentPopupInstance.GetComponent<RectTransform>();
				if (rectTransform != null)
				{
					originalPopupAnchoredPos = rectTransform.anchoredPosition;
					rectTransform.anchoredPosition = originalPopupAnchoredPos + currentStep.popupOffset;
				}
				else
				{
					originalPopupLocalPos = currentPopupInstance.transform.localPosition;
					currentPopupInstance.transform.localPosition = originalPopupLocalPos + new Vector3(currentStep.popupOffset.x, currentStep.popupOffset.y, 0f);
				}
				currentPopupInstance.SetActive(true);
			}
			fingerSpriteRenderer.sortingOrder = 25005;
			fingerSpriteRenderer.gameObject.SetActive(true);
			float scale = ((currentStep.tutorialFingerScale > 0f) ? currentStep.tutorialFingerScale : 1f);
			Vector2 offset = currentStep.tutorialFingerOffset;
			fingerSpriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);
			Vector3 offset3D = new Vector3(offset.x, offset.y, 0f);
			Vector3 fromWorld = boardManager.GetWorldPosition(tutorialFrom.x, tutorialFrom.y) + offset3D;
			Vector3 toWorld = boardManager.GetWorldPosition(tutorialTo.x, tutorialTo.y) + offset3D;
			fingerSpriteRenderer.transform.position = fromWorld;
			Sequence seq = DOTween.Sequence();
			if (tutorialFrom != tutorialTo)
			{
				seq.Append(fingerSpriteRenderer.transform.DOMove(toWorld, 1f).SetEase(Ease.InOutSine));
			}
			else
			{
				seq.Append(fingerSpriteRenderer.transform.DOScale(scale * 0.8f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
			}
			seq.AppendCallback(delegate
			{
				fingerSpriteRenderer.transform.position = fromWorld;
				fingerSpriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);
			});
			seq.AppendInterval(0.2f);
			seq.SetLoops(-1, LoopType.Restart);
			fingerSpriteRenderer.transform.DOKill();
			seq.SetId(fingerSpriteRenderer.transform);
		}

		public void ClearGuideInstantly()
		{
			isGuideActive = false;
			if (currentPopupInstance != null)
			{
				Object.Destroy(currentPopupInstance);
				currentPopupInstance = null;
			}
			if (popupCanvasContainer != null)
			{
				Object.Destroy(popupCanvasContainer.gameObject);
				popupCanvasContainer = null;
			}
			if (darkMask != null)
			{
				darkMask.gameObject.SetActive(false);
			}
			if (fingerSpriteRenderer != null)
			{
				fingerSpriteRenderer.transform.DOKill();
				fingerSpriteRenderer.gameObject.SetActive(false);
			}
			for (int i = 0; i < highlightedElements.Count; i++)
			{
				if (highlightedElements[i] != null && highlightedElements[i].spriteRenderer != null)
				{
					highlightedElements[i].spriteRenderer.sortingOrder = originalElementOrders[i];
				}
			}
			for (int k = 0; k < highlightedBgTiles.Count; k++)
			{
				if (highlightedBgTiles[k] != null)
				{
					highlightedBgTiles[k].sortingOrder = originalBgOrders[k];
				}
			}
			for (int j = 0; j < highlightedMeshes.Count; j++)
			{
				if (highlightedMeshes[j] != null)
				{
					highlightedMeshes[j].sortingOrder = originalMeshOrders[j];
				}
			}
			highlightedElements.Clear();
			originalElementOrders.Clear();
			highlightedBgTiles.Clear();
			originalBgOrders.Clear();
			highlightedMeshes.Clear();
			originalMeshOrders.Clear();
		}

		public void CompleteTutorial()
		{
			if (!isGuideActive)
			{
				return;
			}
			isGuideActive = false;
			idleTimer = 0f;
			if (currentPopupInstance != null)
			{
				Object.Destroy(currentPopupInstance);
				currentPopupInstance = null;
			}
			if (popupCanvasContainer != null)
			{
				Object.Destroy(popupCanvasContainer.gameObject);
				popupCanvasContainer = null;
			}
			if (darkMask != null)
			{
				darkMask.gameObject.SetActive(false);
			}
			if (boardManager != null && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
			{
				boardManager.gameManager.uiManager.SetTutorialUIMask(false);
			}
			if (fingerSpriteRenderer != null)
			{
				DOTween.Kill(fingerSpriteRenderer.transform);
				fingerSpriteRenderer.gameObject.SetActive(false);
			}
			for (int i = 0; i < highlightedElements.Count; i++)
			{
				if (highlightedElements[i] != null && highlightedElements[i].spriteRenderer != null)
				{
					highlightedElements[i].spriteRenderer.sortingOrder = originalElementOrders[i];
				}
			}
			for (int k = 0; k < highlightedBgTiles.Count; k++)
			{
				if (highlightedBgTiles[k] != null)
				{
					highlightedBgTiles[k].sortingOrder = originalBgOrders[k];
				}
			}
			for (int j = 0; j < highlightedMeshes.Count; j++)
			{
				if (highlightedMeshes[j] != null)
				{
					highlightedMeshes[j].sortingOrder = originalMeshOrders[j];
				}
			}
			highlightedElements.Clear();
			originalElementOrders.Clear();
			highlightedBgTiles.Clear();
			originalBgOrders.Clear();
			highlightedMeshes.Clear();
			originalMeshOrders.Clear();
			currentStepIndex++;
			if (currentStepIndex >= tutorialSteps.Count)
			{
				EndTutorial();
			}
		}

		private void EndTutorial()
		{
			if (boardManager != null && boardManager.gameManager != null)
			{
				boardManager.gameManager.guideManager = null;
			}
			Object.Destroy(this);
		}

		private void OnDestroy()
		{
			if (currentPopupInstance != null)
			{
				Object.Destroy(currentPopupInstance);
				currentPopupInstance = null;
			}
			if (popupCanvasContainer != null)
			{
				Object.Destroy(popupCanvasContainer.gameObject);
				popupCanvasContainer = null;
			}
			if (darkMask != null && darkMask.gameObject != null)
			{
				Object.Destroy(darkMask.gameObject);
			}
			if (fingerSpriteRenderer != null && fingerSpriteRenderer.gameObject != null)
			{
				fingerSpriteRenderer.transform.DOKill();
				Object.Destroy(fingerSpriteRenderer.gameObject);
			}
		}
	}
}

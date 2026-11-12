using System.Collections;
using System.Collections.Generic;
using BlockPuzzle.Views;
using DG.Tweening;
using Luna.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.UI
{
	public class GameUIManager : MonoBehaviour
	{
		private struct SettlementOverlayState
		{
			public readonly Transform transform;

			public readonly Transform parent;

			public readonly int siblingIndex;

			public readonly Canvas canvas;

			public readonly RenderMode renderMode;

			public readonly Camera worldCamera;

			public readonly float planeDistance;

			public readonly bool overrideSorting;

			public readonly int sortingLayerID;

			public readonly int sortingOrder;

			private List<RendererSortingState> rendererStates;

			public SettlementOverlayState(Transform transform, Canvas canvas)
			{
				this.transform = transform;
				parent = transform.parent;
				siblingIndex = transform.GetSiblingIndex();
				this.canvas = canvas;
				renderMode = ((!(canvas != null)) ? RenderMode.ScreenSpaceCamera : canvas.renderMode);
				worldCamera = ((canvas != null) ? canvas.worldCamera : null);
				planeDistance = ((canvas != null) ? canvas.planeDistance : 0f);
				overrideSorting = canvas != null && canvas.overrideSorting;
				sortingLayerID = ((canvas != null) ? canvas.sortingLayerID : 0);
				sortingOrder = ((canvas != null) ? canvas.sortingOrder : 0);
				rendererStates = new List<RendererSortingState>();
			}

			public void RaiseSettlementRenderers(GameObject overlay)
			{
				Renderer[] renderers = overlay.GetComponentsInChildren<Renderer>(true);
				Renderer[] array = renderers;
				foreach (Renderer renderer in array)
				{
					rendererStates.Add(new RendererSortingState(renderer));
					renderer.sortingLayerName = "Default";
					renderer.sortingOrder = 32767;
				}
			}

			public void RestoreRenderers()
			{
				foreach (RendererSortingState state in rendererStates)
				{
					if (!(state.renderer == null))
					{
						state.renderer.sortingLayerID = state.sortingLayerID;
						state.renderer.sortingOrder = state.sortingOrder;
					}
				}
			}
		}

		private struct RendererSortingState
		{
			public readonly Renderer renderer;

			public readonly int sortingLayerID;

			public readonly int sortingOrder;

			public RendererSortingState(Renderer renderer)
			{
				this.renderer = renderer;
				sortingLayerID = renderer.sortingLayerID;
				sortingOrder = renderer.sortingOrder;
			}
		}

		public const int SettlementCanvasSortingOrder = 32000;

		public const int SettlementRendererSortingOrder = 32767;

		public const int BoardDuringSettlementSortingOrder = 30000;

		[Header("Top Section: Status Bar")]
		public RectTransform topSectionRect;

		public Image logoImage;

		public Transform targetsContainer;

		public TargetUIItem targetItemPrefab;

		public TextMeshProUGUI stepsText;

		[Header("Middle Section: Board Area")]
		public RectTransform boardAreaRect;

		public RectTransform centerBorderRect;

		[Header("Bottom Section: Call to Action")]
		public RectTransform bottomSectionRect;

		public TextMeshProUGUI adText;

		public Button downloadButton;

		[Header("Endgame Section")]
		public GameObject nextLevelPanel;

		public Button nextLevelButton;

		public GameObject victoryPanel;

		public GameObject defeatPanel;

		public Button victoryDownloadButton;

		public Button defeatDownloadButton;

		[Header("Victory Settlement Section")]
		[Tooltip("Drag the existing settlement UI GameObject here. No UI is generated at runtime.")]
		[SerializeField]
		private GameObject victorySettlementScreen;

		[Tooltip("Seconds to show the settlement UI before opening the victory/next-level panel.")]
		[SerializeField]
		[Min(0f)]
		private float victorySettlementDuration = 1.5f;

		[Header("Global Ad Section")]
		public GameObject globalAdOverlay;

		public Button globalAdButton;

		public GameObject downloadPanel;

		private List<TargetUIItem> activeTargetItems = new List<TargetUIItem>();

		private Image topMaskInstance;

		private Image centerMaskInstance;

		private int lastScreenWidth;

		private int lastScreenHeight;

		[Header("Fade Transition")]
		private Image fadeOverlayImage;

		private Coroutine updateLayoutCoroutine;

		public void SetTutorialUIMask(bool active)
		{
			if (active)
			{
				if (bottomSectionRect != null)
				{
					Canvas canvas2 = bottomSectionRect.GetComponent<Canvas>();
					if (canvas2 == null)
					{
						canvas2 = bottomSectionRect.gameObject.AddComponent<Canvas>();
						canvas2.overrideSorting = true;
						bottomSectionRect.gameObject.AddComponent<GraphicRaycaster>();
					}
					canvas2.sortingOrder = 26000;
				}
			}
			else if (bottomSectionRect != null)
			{
				Canvas canvas = bottomSectionRect.GetComponent<Canvas>();
				if (canvas != null)
				{
					canvas.overrideSorting = false;
				}
			}
		}

		private IEnumerator Start()
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
			if (downloadButton != null)
			{
				downloadButton.onClick.AddListener(OnDownloadClicked);
			}
			if (victoryDownloadButton != null)
			{
				victoryDownloadButton.onClick.AddListener(OnDownloadClicked);
			}
			if (defeatDownloadButton != null)
			{
				defeatDownloadButton.onClick.AddListener(OnDownloadClicked);
			}
			if (nextLevelButton != null)
			{
				nextLevelButton.onClick.AddListener(OnNextLevelClicked);
			}
			if (globalAdButton != null)
			{
				globalAdButton.onClick.AddListener(OnDownloadClicked);
			}
			if (victoryPanel != null)
			{
				victoryPanel.SetActive(false);
			}
			if (defeatPanel != null)
			{
				defeatPanel.SetActive(false);
			}
			if (nextLevelPanel != null)
			{
				nextLevelPanel.SetActive(false);
			}
			if (victorySettlementScreen != null)
			{
				victorySettlementScreen.SetActive(false);
			}
			if (globalAdOverlay != null)
			{
				globalAdOverlay.SetActive(false);
			}
			if (downloadPanel != null)
			{
				downloadPanel.SetActive(false);
			}
			if (OrientationDetector.Instance != null)
			{
				OrientationDetector.Instance.OnOrientationChanged += OnOrientationChanged;
				OnOrientationChanged(OrientationDetector.Instance.CurrentOrientation);
			}
			else
			{
				OnOrientationChanged((Screen.width <= Screen.height) ? ScreenOrientationType.Portrait : ScreenOrientationType.Landscape);
			}
			yield return new WaitForEndOfFrame();
			PlayIntroAnimation();
		}

		private void OnDestroy()
		{
			if (OrientationDetector.Instance != null)
			{
				OrientationDetector.Instance.OnOrientationChanged -= OnOrientationChanged;
			}
		}

		public IEnumerator FadeOutCoroutine(float duration)
		{
			EnsureFadeOverlay();
			fadeOverlayImage.gameObject.SetActive(true);
			fadeOverlayImage.color = new Color(0f, 0f, 0f, 0f);
			yield return fadeOverlayImage.DOFade(1f, duration).WaitForCompletion();
		}

		public IEnumerator FadeInCoroutine(float duration)
		{
			EnsureFadeOverlay();
			fadeOverlayImage.color = new Color(0f, 0f, 0f, 1f);
			yield return fadeOverlayImage.DOFade(0f, duration).WaitForCompletion();
			fadeOverlayImage.gameObject.SetActive(false);
		}

		private void EnsureFadeOverlay()
		{
			if (fadeOverlayImage == null)
			{
				GameObject fadeObj = new GameObject("FadeOverlay");
				fadeObj.transform.SetParent(base.transform, false);
				fadeObj.transform.SetAsLastSibling();
				Canvas canvas = fadeObj.AddComponent<Canvas>();
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				fadeOverlayImage = fadeObj.AddComponent<Image>();
				fadeOverlayImage.color = new Color(0f, 0f, 0f, 0f);
				fadeOverlayImage.raycastTarget = true;
				RectTransform rt = fadeObj.GetComponent<RectTransform>();
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;
			}
			else
			{
				fadeOverlayImage.transform.SetAsLastSibling();
			}
		}

		private void Update()
		{
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				if (updateLayoutCoroutine != null)
				{
					StopCoroutine(updateLayoutCoroutine);
				}
				updateLayoutCoroutine = StartCoroutine(UpdateBoardLayoutAfterUIUpdate());
			}
		}

		private void OnOrientationChanged(ScreenOrientationType newOrientation)
		{
			float screenWidth = Screen.width;
			float screenHeight = Screen.height;
			float refWidth = 1080f;
			float refHeight = 1920f;
			CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
			if (newOrientation == ScreenOrientationType.Landscape)
			{
				if (canvasScaler != null)
				{
					canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
					canvasScaler.referenceResolution = new Vector2(refWidth, refHeight);
					canvasScaler.matchWidthOrHeight = 1f;
				}
			}
			else if (canvasScaler != null)
			{
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.referenceResolution = new Vector2(refWidth, refHeight);
				canvasScaler.matchWidthOrHeight = 0f;
			}
			ResetRectToFull(topSectionRect);
			ResetRectToFull(bottomSectionRect);
			ResetRectToFull(centerBorderRect);
			ResetRectToFull(boardAreaRect);
			if (updateLayoutCoroutine != null)
			{
				StopCoroutine(updateLayoutCoroutine);
			}
			updateLayoutCoroutine = StartCoroutine(UpdateBoardLayoutAfterUIUpdate());
		}

		private void ResetRectToFull(RectTransform rect)
		{
			if (!(rect == null) && (rect == centerBorderRect || rect == boardAreaRect))
			{
				rect.anchorMin = new Vector2(0.5f, 0.5f);
				rect.anchorMax = new Vector2(0.5f, 0.5f);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.anchoredPosition = Vector2.zero;
				if (rect == boardAreaRect)
				{
					rect.sizeDelta = new Vector2(1000f, 1000f);
				}
			}
		}

		private IEnumerator UpdateBoardLayoutAfterUIUpdate()
		{
			yield return new WaitForEndOfFrame();
			Canvas.ForceUpdateCanvases();
			yield return new WaitForEndOfFrame();
			if (boardAreaRect != null)
			{
				boardAreaRect.anchorMin = new Vector2(0.5f, 0.5f);
				boardAreaRect.anchorMax = new Vector2(0.5f, 0.5f);
				boardAreaRect.pivot = new Vector2(0.5f, 0.5f);
				boardAreaRect.anchoredPosition = Vector2.zero;
				boardAreaRect.sizeDelta = new Vector2(1000f, 1000f);
			}
			GameManager gm = Object.FindObjectOfType<GameManager>();
			if (!(gm != null) || !(gm.boardManager != null))
			{
				yield break;
			}
			gm.boardManager.UpdateBoardLayout(boardAreaRect);
			if (centerBorderRect != null && boardAreaRect != null && centerBorderRect != boardAreaRect)
			{
				int cols = gm.boardManager.currentLevel.width;
				int rows = gm.boardManager.currentLevel.height;
				float cellWorldSize = gm.boardManager.cellSize;
				float spacingWorldSize = gm.boardManager.cellSpacing;
				float boardWorldWidth = (float)cols * cellWorldSize + (float)(cols - 1) * spacingWorldSize;
				float boardWorldHeight = (float)rows * cellWorldSize + (float)(rows - 1) * spacingWorldSize;
				Vector3[] corners = new Vector3[4];
				boardAreaRect.GetWorldCorners(corners);
				float uiWorldWidth = Vector3.Distance(corners[0], corners[3]);
				float uiWorldHeight = Vector3.Distance(corners[0], corners[1]);
				if (uiWorldWidth > 0f && uiWorldHeight > 0f)
				{
					float widthRatio = boardWorldWidth / uiWorldWidth;
					float heightRatio = boardWorldHeight / uiWorldHeight;
					float targetUIWidth2 = boardAreaRect.rect.width * widthRatio;
					float targetUIHeight2 = boardAreaRect.rect.height * heightRatio;
					float visualBorderPadding = 50f;
					targetUIWidth2 += visualBorderPadding;
					targetUIHeight2 += visualBorderPadding;
					centerBorderRect.DOKill();
					centerBorderRect.anchorMin = new Vector2(0.5f, 0.5f);
					centerBorderRect.anchorMax = new Vector2(0.5f, 0.5f);
					centerBorderRect.pivot = new Vector2(0.5f, 0.5f);
					centerBorderRect.position = boardAreaRect.position;
					centerBorderRect.sizeDelta = new Vector2(targetUIWidth2, targetUIHeight2);
				}
			}
		}

		private void PlayIntroAnimation()
		{
			float screenWidth = Screen.width;
			if (topSectionRect != null)
			{
				topSectionRect.DOKill();
				Vector3 finalPos3 = topSectionRect.anchoredPosition3D;
				topSectionRect.anchoredPosition3D = new Vector3(finalPos3.x + screenWidth, finalPos3.y, finalPos3.z);
				topSectionRect.DOAnchorPos3DX(finalPos3.x, 0.6f).SetEase(Ease.OutBack).SetDelay(0.1f);
			}
			if (centerBorderRect != null)
			{
				centerBorderRect.DOKill();
				Vector3 finalPos2 = centerBorderRect.anchoredPosition3D;
				centerBorderRect.anchoredPosition3D = new Vector3(finalPos2.x + screenWidth, finalPos2.y, finalPos2.z);
				centerBorderRect.DOAnchorPos3DX(finalPos2.x, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
			}
			if (bottomSectionRect != null)
			{
				bottomSectionRect.DOKill();
				Vector3 finalPos = bottomSectionRect.anchoredPosition3D;
				bottomSectionRect.anchoredPosition3D = new Vector3(finalPos.x + screenWidth, finalPos.y, finalPos.z);
				bottomSectionRect.DOAnchorPos3DX(finalPos.x, 0.6f).SetEase(Ease.OutBack).SetDelay(0.3f);
			}
		}

		public void InitializeUI(string adMessage, int initialSteps)
		{
			UpdateSteps(initialSteps);
			if (adText != null)
			{
				adText.text = adMessage;
			}
		}

		public void SetupTargets(List<TargetData> targets)
		{
			foreach (TargetUIItem item2 in activeTargetItems)
			{
				Object.Destroy(item2.gameObject);
			}
			activeTargetItems.Clear();
			foreach (TargetData target in targets)
			{
				TargetUIItem item = Object.Instantiate(targetItemPrefab, targetsContainer);
				Sprite targetSprite = target.icon;
				item.Setup(targetSprite, target.requiredAmount);
				activeTargetItems.Add(item);
			}
		}

		public void UpdateTarget(int targetIndex, int currentAmount)
		{
			if (targetIndex >= 0 && targetIndex < activeTargetItems.Count)
			{
				activeTargetItems[targetIndex].UpdateAmount(currentAmount);
			}
		}

		public Vector3 GetTargetWorldPosition(int targetIndex)
		{
			if (targetIndex >= 0 && targetIndex < activeTargetItems.Count)
			{
				Vector3 uiPos = activeTargetItems[targetIndex].iconImage.transform.position;
				Canvas canvas = GetComponentInParent<Canvas>();
				if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay && Camera.main != null)
				{
					uiPos.z = Mathf.Abs(Camera.main.transform.position.z);
					return Camera.main.ScreenToWorldPoint(uiPos);
				}
				return uiPos;
			}
			return Vector3.zero;
		}

		public void UpdateSteps(int remainingSteps)
		{
			if (stepsText != null)
			{
				stepsText.text = remainingSteps.ToString();
			}
		}

		public void ShowVictoryPanel()
		{
			if (victoryPanel != null)
			{
				victoryPanel.transform.SetAsLastSibling();
				Canvas canvas = victoryPanel.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = victoryPanel.gameObject.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				if (victoryPanel.GetComponent<GraphicRaycaster>() == null)
				{
					victoryPanel.gameObject.AddComponent<GraphicRaycaster>();
				}
				victoryPanel.SetActive(true);
				victoryPanel.transform.localScale = Vector3.zero;
				victoryPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
		}

		public void MountVictorySettlementScreen(GameObject screen)
		{
			victorySettlementScreen = screen;
		}

		public IEnumerator PlayVictorySettlement()
		{
			if (victorySettlementScreen == null)
			{
				Debug.LogWarning("No victory settlement GameObject is mounted; skipping the victory settlement screen.", this);
				yield break;
			}
			SettlementOverlayState overlayState = PrepareTopmostOverlay(victorySettlementScreen);
			yield return new WaitForSecondsRealtime(victorySettlementDuration);
			victorySettlementScreen.SetActive(false);
			RestoreOverlay(overlayState);
		}

		private static SettlementOverlayState PrepareTopmostOverlay(GameObject overlay)
		{
			overlay.SetActive(true);
			SettlementOverlayState state = new SettlementOverlayState(overlay.transform, overlay.GetComponent<Canvas>());
			if (state.canvas == null)
			{
				overlay.transform.SetAsLastSibling();
				Debug.LogError("The mounted victory settlement root needs its own Canvas component to render above the board.", overlay);
				return state;
			}
			overlay.transform.SetParent(null, false);
			state.canvas.renderMode = RenderMode.ScreenSpaceCamera;
			state.canvas.worldCamera = ((state.worldCamera != null) ? state.worldCamera : Camera.main);
			state.canvas.planeDistance = ((state.planeDistance > 0f) ? state.planeDistance : 100f);
			state.canvas.overrideSorting = true;
			state.canvas.sortingLayerName = "Default";
			state.canvas.sortingOrder = 32000;
			state.RaiseSettlementRenderers(overlay);
			Canvas.ForceUpdateCanvases();
			return state;
		}

		private static void RestoreOverlay(SettlementOverlayState state)
		{
			if (state.canvas != null)
			{
				state.canvas.renderMode = state.renderMode;
				state.canvas.worldCamera = state.worldCamera;
				state.canvas.planeDistance = state.planeDistance;
				state.canvas.overrideSorting = state.overrideSorting;
				state.canvas.sortingLayerID = state.sortingLayerID;
				state.canvas.sortingOrder = state.sortingOrder;
			}
			if (state.transform != null)
			{
				state.transform.SetParent(state.parent, false);
				if (state.parent != null)
				{
					state.transform.SetSiblingIndex(state.siblingIndex);
				}
			}
			state.RestoreRenderers();
			Canvas.ForceUpdateCanvases();
		}

		public void ShowNextLevelPanel()
		{
			if (nextLevelPanel != null)
			{
				nextLevelPanel.transform.SetAsLastSibling();
				Canvas canvas = nextLevelPanel.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = nextLevelPanel.gameObject.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				if (nextLevelPanel.GetComponent<GraphicRaycaster>() == null)
				{
					nextLevelPanel.gameObject.AddComponent<GraphicRaycaster>();
				}
				nextLevelPanel.SetActive(true);
				nextLevelPanel.transform.localScale = Vector3.zero;
				nextLevelPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
		}

		public void HideNextLevelPanel()
		{
			if (nextLevelPanel != null)
			{
				nextLevelPanel.SetActive(false);
			}
		}

		public void ShowDefeatPanel()
		{
			if (defeatPanel != null)
			{
				defeatPanel.transform.SetAsLastSibling();
				Canvas canvas = defeatPanel.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = defeatPanel.gameObject.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				if (defeatPanel.GetComponent<GraphicRaycaster>() == null)
				{
					defeatPanel.gameObject.AddComponent<GraphicRaycaster>();
				}
				defeatPanel.SetActive(true);
				defeatPanel.transform.localScale = Vector3.zero;
				defeatPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
		}

		public void ShowGlobalAdOverlay()
		{
			if (globalAdOverlay != null)
			{
				GameManager gm = Object.FindObjectOfType<GameManager>();
				if (gm != null && gm.guideManager != null)
				{
					gm.guideManager.StopAllCoroutines();
					gm.guideManager.CompleteTutorial();
					Object.Destroy(gm.guideManager);
					SetTutorialUIMask(false);
				}
				globalAdOverlay.transform.SetAsLastSibling();
				Canvas canvas = globalAdOverlay.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = globalAdOverlay.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				if (globalAdOverlay.GetComponent<GraphicRaycaster>() == null)
				{
					globalAdOverlay.AddComponent<GraphicRaycaster>();
				}
				globalAdOverlay.SetActive(true);
			}
		}

		public void ShowDownloadPanel()
		{
			if (downloadPanel != null)
			{
				GameManager gm = Object.FindObjectOfType<GameManager>();
				if (gm != null && gm.guideManager != null)
				{
					gm.guideManager.StopAllCoroutines();
					gm.guideManager.ClearGuideInstantly();
					SetTutorialUIMask(false);
					Object.Destroy(gm.guideManager);
					gm.guideManager = null;
				}
				downloadPanel.SetActive(true);
				downloadPanel.transform.SetAsLastSibling();
				Canvas canvas = downloadPanel.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = downloadPanel.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 32767;
				canvas.sortingLayerName = "Default";
				Vector3 localPos = downloadPanel.transform.localPosition;
				localPos.z = -5000f;
				downloadPanel.transform.localPosition = localPos;
				if (downloadPanel.GetComponent<GraphicRaycaster>() == null)
				{
					downloadPanel.AddComponent<GraphicRaycaster>();
				}
				Canvas.ForceUpdateCanvases();
				downloadPanel.transform.localScale = Vector3.zero;
				downloadPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
		}

		private void OnNextLevelClicked()
		{
			GameManager gm = Object.FindObjectOfType<GameManager>();
			if (gm != null)
			{
				gm.LoadNextLevel();
			}
		}

		private void OnDownloadClicked()
		{
			GameManager gm = Object.FindObjectOfType<GameManager>();
			if (gm != null)
			{
				gm.DownloadGame();
			}
		}

		public void AdSkip()
		{
			Playable.InstallFullGame();
			LifeCycle.GameEnded();
		}
	}
}

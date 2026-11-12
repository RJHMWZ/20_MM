using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro is used, fallback to standard UI if not.
using System.Collections.Generic;
using DG.Tweening;
using Luna.Unity;

namespace Match3.UI
{
    public class GameUIManager : MonoBehaviour
    {
        public const int SettlementCanvasSortingOrder = 32000;
        public const int SettlementRendererSortingOrder = short.MaxValue;
        public const int BoardDuringSettlementSortingOrder = 30000;

        [Header("Top Section: Status Bar")]
        public RectTransform topSectionRect;
        public Image logoImage;
        public Transform targetsContainer; // Parent for target items
        public TargetUIItem targetItemPrefab; // Prefab for individual target
        public TextMeshProUGUI stepsText;

        [Header("Middle Section: Board Area")]
        public RectTransform boardAreaRect; // The area where the grid will be instantiated
        public RectTransform centerBorderRect; // The border or background for the middle section

        [Header("Bottom Section: Call to Action")]
        public RectTransform bottomSectionRect;
        public TextMeshProUGUI adText;
        public Button downloadButton;

        [Header("Endgame Section")]
        public GameObject nextLevelPanel; // Optional: specific panel for next level. If null, victoryPanel can be used if tweaked or we just rely on nextLevelPanel.
        public Button nextLevelButton;
        
        public GameObject victoryPanel;
        public GameObject defeatPanel;
        public Button victoryDownloadButton;
        public Button defeatDownloadButton;

        [Header("Victory Settlement Section")]
        [Tooltip("Drag the existing settlement UI GameObject here. No UI is generated at runtime.")]
        [SerializeField] private GameObject victorySettlementScreen;
        [Tooltip("Seconds to show the settlement UI before opening the victory/next-level panel.")]
        [SerializeField, Min(0f)] private float victorySettlementDuration = 1.5f;

        [Header("Global Ad Section")]
        public GameObject globalAdOverlay; // An invisible panel covering the whole screen
        public Button globalAdButton; // The button attached to that invisible panel
        public GameObject downloadPanel; // A visible download UI panel

        private List<TargetUIItem> activeTargetItems = new List<TargetUIItem>();
        
        private Image topMaskInstance;
        private Image centerMaskInstance;

        public void SetTutorialUIMask(bool active)
        {
            if (active)
            {
                // To keep the bottom section visible over the unified guide mask (sortingOrder 25000)
                if (bottomSectionRect != null)
                {
                    Canvas canvas = bottomSectionRect.GetComponent<Canvas>();
                    if (canvas == null)
                    {
                        canvas = bottomSectionRect.gameObject.AddComponent<Canvas>();
                        canvas.overrideSorting = true;
                        bottomSectionRect.gameObject.AddComponent<GraphicRaycaster>();
                    }
                    canvas.sortingOrder = 26000;
                }
            }
            else
            {
                if (bottomSectionRect != null)
                {
                    Canvas canvas = bottomSectionRect.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.overrideSorting = false;
                        // We do not destroy the Canvas to avoid breaking the UI layout rebuilds,
                        // just disabling overrideSorting returns it to the default flow.
                    }
                }
            }
        }

        private int lastScreenWidth;
        private int lastScreenHeight;

        private System.Collections.IEnumerator Start()
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
            
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (defeatPanel != null) defeatPanel.SetActive(false);
            if (nextLevelPanel != null) nextLevelPanel.SetActive(false);
            if (victorySettlementScreen != null) victorySettlementScreen.SetActive(false);
            if (globalAdOverlay != null) globalAdOverlay.SetActive(false);
            if (downloadPanel != null) downloadPanel.SetActive(false);

            if (BlockPuzzle.Views.OrientationDetector.Instance != null)
            {
                BlockPuzzle.Views.OrientationDetector.Instance.OnOrientationChanged += OnOrientationChanged;
                OnOrientationChanged(BlockPuzzle.Views.OrientationDetector.Instance.CurrentOrientation);
            }
            else 
            {
                // 如果没有方向检测器，也要手动调用一次以重置 RectTransform 和触发布局
                OnOrientationChanged(Screen.width > Screen.height ? BlockPuzzle.Views.ScreenOrientationType.Landscape : BlockPuzzle.Views.ScreenOrientationType.Portrait);
            }
            
            // Wait to ensure everything is settled before intro
            yield return new WaitForEndOfFrame();
            
            PlayIntroAnimation();
        }

        private void OnDestroy()
        {
            if (BlockPuzzle.Views.OrientationDetector.Instance != null)
            {
                BlockPuzzle.Views.OrientationDetector.Instance.OnOrientationChanged -= OnOrientationChanged;
            }
        }

        [Header("Fade Transition")]
        private Image fadeOverlayImage;

        public System.Collections.IEnumerator FadeOutCoroutine(float duration)
        {
            EnsureFadeOverlay();
            fadeOverlayImage.gameObject.SetActive(true);
            fadeOverlayImage.color = new Color(0, 0, 0, 0);
            yield return fadeOverlayImage.DOFade(1f, duration).WaitForCompletion();
        }

        public System.Collections.IEnumerator FadeInCoroutine(float duration)
        {
            EnsureFadeOverlay();
            fadeOverlayImage.color = new Color(0, 0, 0, 1);
            yield return fadeOverlayImage.DOFade(0f, duration).WaitForCompletion();
            fadeOverlayImage.gameObject.SetActive(false);
        }

        private void EnsureFadeOverlay()
        {
            if (fadeOverlayImage == null)
            {
                GameObject fadeObj = new GameObject("FadeOverlay");
                fadeObj.transform.SetParent(this.transform, false);
                fadeObj.transform.SetAsLastSibling();

                Canvas canvas = fadeObj.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 32767; // highest

                fadeOverlayImage = fadeObj.AddComponent<Image>();
                fadeOverlayImage.color = new Color(0, 0, 0, 0);
                fadeOverlayImage.raycastTarget = true; // block clicks during fade

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
            // Update every frame for smooth dragging resolution changes in Editor / WebGL
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                
                // 【修复】：不使用 StopAllCoroutines，因为它会中断 Start 里的协程（如 PlayIntroAnimation 的等待），
                // 也会中断所有与分辨率无关的正在执行的协程。
                // 而是记录正在执行的更新布局协程并单独停止。
                if (updateLayoutCoroutine != null)
                {
                    StopCoroutine(updateLayoutCoroutine);
                }
                updateLayoutCoroutine = StartCoroutine(UpdateBoardLayoutAfterUIUpdate());
            }
        }

        private Coroutine updateLayoutCoroutine;

        private void OnOrientationChanged(BlockPuzzle.Views.ScreenOrientationType newOrientation)
        {
            // 获取当前屏幕宽高
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 假设 UI 的基准设计分辨率为 1080x1920（标准竖屏）
            float refWidth = 1080f;
            float refHeight = 1920f;

            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

            if (newOrientation == BlockPuzzle.Views.ScreenOrientationType.Landscape)
            {
                // 【横屏逻辑】：
                // 如果需要横屏时整体按高度缩小，保持竖屏比例居中
                // 必须通过 Match Width Or Height 来控制
                if (canvasScaler != null)
                {
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(refWidth, refHeight);
                    canvasScaler.matchWidthOrHeight = 1f; // 完全按高度适配 (Match Height)
                }
            }
            else
            {
                // 【竖屏逻辑】：
                // 竖屏时，通常按照宽度来适配
                if (canvasScaler != null)
                {
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(refWidth, refHeight);
                    canvasScaler.matchWidthOrHeight = 0f; // 完全按宽度适配 (Match Width)
                }
            }

            // 【关键修复】：如果之前用代码计算了各种 Anchor (FitRectInArea)，需要禁用或重置
            // 这里我们强制重置一些关键 UI 的锚点和偏移，确保它们按原始预制体/竖屏布局排列
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
            if (rect == null) return;
            // 恢复 UI 元素自身设定的锚点和尺寸，不再被硬编码的百分比强行拉伸
            // 依赖于在 Unity 编辑器中保存的原始竖屏状态
            
            // 为了防止之前 FitRectInArea 导致的残留锚点影响，这里将中间关键区域强制设定为绝对居中
            // 对于 BoardArea 和 CenterBorder：
            if (rect == centerBorderRect || rect == boardAreaRect)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                
                // 给它一个固定的尺寸，比如屏幕大部分区域，或者保持原预制体尺寸即可
                // 如果需要固定宽高比，可以给它们挂 AspectRatioFitter，或者在这里写死大小
                // 假设中心区域是 1000x1000
                if (rect == boardAreaRect)
                {
                    rect.sizeDelta = new Vector2(1000f, 1000f);
                }
            }
        }

        private System.Collections.IEnumerator UpdateBoardLayoutAfterUIUpdate()
        {
            // Wait for end of frame to ensure OrientationActiveSwitcher has fully applied new layout to RectTransforms
            yield return new WaitForEndOfFrame();
            
            // Force canvas to update so that RectTransforms have the correct world corners after OrientationActiveSwitcher modifies them
            Canvas.ForceUpdateCanvases();
            // 在修改 RectTransform 之后，为了确保 GetWorldCorners 拿到的是最新且正确计算后的物理坐标，
            // 必须再次等待一帧让引擎底层彻底刷新矩阵，或者至少使用 Canvas.ForceUpdateCanvases()。
            // 为了绝对稳妥，这里再等待一帧，确保第一帧启动时能够取到完全正确的 UI 尺寸。
            yield return new WaitForEndOfFrame();

            // 【关键修复】：为了确保棋盘能够与其他UI同步缩放，并且在整体UI停止放大时也停止放大，
            // 我们必须确保 boardAreaRect 不会因为锚点拉伸 (Stretch) 而随屏幕物理边界无限扩大。
            // 将其固定在屏幕中心，并赋予一个在 Canvas 坐标系下固定的最大可用尺寸。
            if (boardAreaRect != null)
            {
                boardAreaRect.anchorMin = new Vector2(0.5f, 0.5f);
                boardAreaRect.anchorMax = new Vector2(0.5f, 0.5f);
                boardAreaRect.pivot = new Vector2(0.5f, 0.5f);
                boardAreaRect.anchoredPosition = Vector2.zero;
                // 赋予一个基于基准分辨率的最大尺寸（比如 1000x1000）。当 Canvas 缩放停止时，它的物理像素大小也会停止扩大。
                boardAreaRect.sizeDelta = new Vector2(1000f, 1000f);
            }

            var gm = FindObjectOfType<GameManager>();
            if (gm != null && gm.boardManager != null)
            {
                gm.boardManager.UpdateBoardLayout(boardAreaRect);

                // 【修复】：让背景边框 (centerBorderRect) 始终紧紧包裹住实际的棋盘大小，防止因为屏幕比例变宽导致的边框与棋盘脱节缝隙
                if (centerBorderRect != null && boardAreaRect != null && centerBorderRect != boardAreaRect)
                {
                    int cols = gm.boardManager.currentLevel.width;
                    int rows = gm.boardManager.currentLevel.height;
                    float cellWorldSize = gm.boardManager.cellSize;
                    float spacingWorldSize = gm.boardManager.cellSpacing;

                    // 计算棋盘在世界坐标下的实际宽高
                    float boardWorldWidth = cols * cellWorldSize + (cols - 1) * spacingWorldSize;
                    float boardWorldHeight = rows * cellWorldSize + (rows - 1) * spacingWorldSize;

                    // 计算 UI 容器在世界坐标下的宽高
                    Vector3[] corners = new Vector3[4];
                    boardAreaRect.GetWorldCorners(corners);
                    float uiWorldWidth = Vector3.Distance(corners[0], corners[3]);
                    float uiWorldHeight = Vector3.Distance(corners[0], corners[1]);

                    if (uiWorldWidth > 0 && uiWorldHeight > 0)
                    {
                        // 计算比例
                        float widthRatio = boardWorldWidth / uiWorldWidth;
                        float heightRatio = boardWorldHeight / uiWorldHeight;

                        // 得到棋盘在 UI 中的实际尺寸
                        float targetUIWidth = boardAreaRect.rect.width * widthRatio;
                        float targetUIHeight = boardAreaRect.rect.height * heightRatio;

                    // 为了防止 Padding 为 0 时方块刚好压在边框的图片边缘上（重合）
                    // 动态增加一点边框的额外像素，使其包裹住方块而不被遮挡
                    float visualBorderPadding = 50f; // 给予边框 50 像素的视觉留白
                    targetUIWidth += visualBorderPadding;
                    targetUIHeight += visualBorderPadding;

                    // 强制边框居中并应用紧密包裹的尺寸
                    centerBorderRect.DOKill();
                    centerBorderRect.anchorMin = new Vector2(0.5f, 0.5f);
                    centerBorderRect.anchorMax = new Vector2(0.5f, 0.5f);
                    centerBorderRect.pivot = new Vector2(0.5f, 0.5f);
                    centerBorderRect.position = boardAreaRect.position;
                    centerBorderRect.sizeDelta = new Vector2(targetUIWidth, targetUIHeight);
                    }
                }
            }
        }

        private void PlayIntroAnimation()
        {
            float screenWidth = Screen.width;

            // Animate Top Section
            if (topSectionRect != null)
            {
                topSectionRect.DOKill();
                Vector3 finalPos = topSectionRect.anchoredPosition3D;
                topSectionRect.anchoredPosition3D = new Vector3(finalPos.x + screenWidth, finalPos.y, finalPos.z);
                topSectionRect.DOAnchorPos3DX(finalPos.x, 0.6f).SetEase(Ease.OutBack).SetDelay(0.1f);
            }

            // Animate Center Border
            if (centerBorderRect != null)
            {
                centerBorderRect.DOKill();
                Vector3 finalPos = centerBorderRect.anchoredPosition3D;
                centerBorderRect.anchoredPosition3D = new Vector3(finalPos.x + screenWidth, finalPos.y, finalPos.z);
                centerBorderRect.DOAnchorPos3DX(finalPos.x, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
            }

            // Animate Bottom Section
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
            // Clear existing targets
            foreach (var item in activeTargetItems)
            {
                Destroy(item.gameObject);
            }
            activeTargetItems.Clear();

            // Create new target UI items
            foreach (var target in targets)
            {
                var item = Instantiate(targetItemPrefab, targetsContainer);
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
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    // Convert screen space to world space for the main camera
                    if (Camera.main != null)
                    {
                        uiPos.z = Mathf.Abs(Camera.main.transform.position.z);
                        return Camera.main.ScreenToWorldPoint(uiPos);
                    }
                }
                return uiPos; // Works for ScreenSpace-Camera and WorldSpace
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
                // Ensure it renders on top of everything else by setting it as the last sibling in its parent
                victoryPanel.transform.SetAsLastSibling();
                
                // If there's a canvas on the panel, force the sorting order to be extremely high to cover world space objects
                Canvas canvas = victoryPanel.GetComponent<Canvas>();
                if (canvas == null) canvas = victoryPanel.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 32767;
                
                // Need a GraphicRaycaster for buttons to work if we added a Canvas
                if (victoryPanel.GetComponent<GraphicRaycaster>() == null)
                {
                    victoryPanel.gameObject.AddComponent<GraphicRaycaster>();
                }
                
                victoryPanel.SetActive(true);
                // Optional: Add simple pop-in animation
                victoryPanel.transform.localScale = Vector3.zero;
                victoryPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }

        /// <summary>
        /// Runtime mount point for an existing victory settlement UI.
        /// </summary>
        public void MountVictorySettlementScreen(GameObject screen)
        {
            victorySettlementScreen = screen;
        }

        public System.Collections.IEnumerator PlayVictorySettlement()
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

            // Detaching prevents the main Canvas' opaque/background graphics from also
            // moving above the world-space board. Only the settlement becomes Overlay.
            overlay.transform.SetParent(null, false);
            // Keep particles and UI in the same camera render path. ParticleSystemRenderer
            // cannot be rendered by a Screen Space Overlay canvas.
            state.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            state.canvas.worldCamera = state.worldCamera != null ? state.worldCamera : Camera.main;
            state.canvas.planeDistance = state.planeDistance > 0f ? state.planeDistance : 100f;
            state.canvas.overrideSorting = true;
            state.canvas.sortingLayerName = "Default";
            state.canvas.sortingOrder = SettlementCanvasSortingOrder;
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
                if (state.parent != null) state.transform.SetSiblingIndex(state.siblingIndex);
            }
            state.RestoreRenderers();
            Canvas.ForceUpdateCanvases();
        }

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
                renderMode = canvas != null ? canvas.renderMode : RenderMode.ScreenSpaceCamera;
                worldCamera = canvas != null ? canvas.worldCamera : null;
                planeDistance = canvas != null ? canvas.planeDistance : 0f;
                overrideSorting = canvas != null && canvas.overrideSorting;
                sortingLayerID = canvas != null ? canvas.sortingLayerID : 0;
                sortingOrder = canvas != null ? canvas.sortingOrder : 0;
                rendererStates = new List<RendererSortingState>();
            }

            public void RaiseSettlementRenderers(GameObject overlay)
            {
                Renderer[] renderers = overlay.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    rendererStates.Add(new RendererSortingState(renderer));
                    renderer.sortingLayerName = "Default";
                    renderer.sortingOrder = SettlementRendererSortingOrder;
                }
            }

            public void RestoreRenderers()
            {
                foreach (RendererSortingState state in rendererStates)
                {
                    if (state.renderer == null) continue;
                    state.renderer.sortingLayerID = state.sortingLayerID;
                    state.renderer.sortingOrder = state.sortingOrder;
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

        public void ShowNextLevelPanel()
        {
            if (nextLevelPanel != null)
            {
                nextLevelPanel.transform.SetAsLastSibling();
                
                Canvas canvas = nextLevelPanel.GetComponent<Canvas>();
                if (canvas == null) canvas = nextLevelPanel.gameObject.AddComponent<Canvas>();
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
                // Ensure it renders on top of everything else by setting it as the last sibling in its parent
                defeatPanel.transform.SetAsLastSibling();
                
                // If there's a canvas on the panel, force the sorting order to be extremely high to cover world space objects
                Canvas canvas = defeatPanel.GetComponent<Canvas>();
                if (canvas == null) canvas = defeatPanel.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 32767;
                
                // Need a GraphicRaycaster for buttons to work if we added a Canvas
                if (defeatPanel.GetComponent<GraphicRaycaster>() == null)
                {
                    defeatPanel.gameObject.AddComponent<GraphicRaycaster>();
                }
                
                defeatPanel.SetActive(true);
                // Optional: Add simple pop-in animation
                defeatPanel.transform.localScale = Vector3.zero;
                defeatPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }

        public void ShowGlobalAdOverlay()
        {
            if (globalAdOverlay != null)
            {
                // 先清理并强制停止可能正在运行的引导动画，防止它的遮罩盖过我们的弹窗
                var gm = FindObjectOfType<GameManager>();
                if (gm != null && gm.guideManager != null)
                {
                    gm.guideManager.StopAllCoroutines();
                    // 这里增加调用 CompleteTutorial() 来还原方块和背景原本的 SortingOrder
                    // 防止高亮的方块（25002）遮挡 UI
                    gm.guideManager.CompleteTutorial(); 
                    Destroy(gm.guideManager);
                    SetTutorialUIMask(false);
                }

                globalAdOverlay.transform.SetAsLastSibling();
                
                Canvas canvas = globalAdOverlay.GetComponent<Canvas>();
                if (canvas == null) canvas = globalAdOverlay.AddComponent<Canvas>();
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
                // 先清理并强制停止可能正在运行的引导动画，防止它的遮罩盖过我们的弹窗
                var gm = FindObjectOfType<GameManager>();
                if (gm != null && gm.guideManager != null)
                {
                    gm.guideManager.StopAllCoroutines();
                    // 使用最新的强制清理方法，确保无视判断直接清除
                    gm.guideManager.ClearGuideInstantly();
                    SetTutorialUIMask(false);
                    Destroy(gm.guideManager);
                    gm.guideManager = null;
                }

                // 先激活 GameObject，再添加和设置 Canvas 组件，这样能确保 Unity Canvas 能够立刻注册排序并生效
                downloadPanel.SetActive(true);
                downloadPanel.transform.SetAsLastSibling();
                
                Canvas canvas = downloadPanel.GetComponent<Canvas>();
                if (canvas == null) canvas = downloadPanel.AddComponent<Canvas>();
                
                // 将 sortingOrder 设为最高，并重写 Sorting
                canvas.overrideSorting = true;
                canvas.sortingOrder = 32767;
                canvas.sortingLayerName = "Default"; // 强制指定到同一默认层级上比大小
                
                // 强制调整 Z 轴位置，如果使用的是 Screen Space - Camera 模式，确保它距离摄像机最近
                Vector3 localPos = downloadPanel.transform.localPosition;
                localPos.z = -5000f;
                downloadPanel.transform.localPosition = localPos;
                
                if (downloadPanel.GetComponent<GraphicRaycaster>() == null)
                {
                    downloadPanel.AddComponent<GraphicRaycaster>();
                }
                
                // 强制立刻更新 Canvas，以确保在这一帧层级生效
                Canvas.ForceUpdateCanvases();
                
                downloadPanel.transform.localScale = Vector3.zero;
                downloadPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }

        private void OnNextLevelClicked()
        {
            // Try to find GameManager and call NextLevel
            var gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.LoadNextLevel();
            }
        }

        private void OnDownloadClicked()
        {
            var gm = FindObjectOfType<GameManager>();
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

    [System.Serializable]
    public class TargetData
    {
        public Sprite icon;
        public int requiredAmount;
    }
}

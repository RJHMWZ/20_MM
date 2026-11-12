using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Match3
{
    public class GuideManager : MonoBehaviour
    {
        private BoardManager boardManager;
        private SpriteRenderer darkMask;
        private SpriteRenderer fingerSpriteRenderer;

        public bool IsGuideActive { get { return isGuideActive; } }
        private bool isGuideActive = false;
        public float delayToShowGuide = 1.2f; // Wait for board intro animation
        private float idleTimer = 0f;
        private float pendingTutorialDelay = 0.5f; // Wait time between steps

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

        public bool HasPendingTutorial => tutorialSteps != null && currentStepIndex < tutorialSteps.Count;
        public Vector2Int CurrentTutorialFrom => tutorialSteps != null && currentStepIndex < tutorialSteps.Count ? tutorialSteps[currentStepIndex].tutorialSwipeFrom : Vector2Int.zero;
        public Vector2Int CurrentTutorialTo => tutorialSteps != null && currentStepIndex < tutorialSteps.Count ? tutorialSteps[currentStepIndex].tutorialSwipeTo : Vector2Int.zero;

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
            maskObj.transform.SetParent(transform);
            maskObj.transform.position = new Vector3(0, 0, -1f);
            
            darkMask = maskObj.AddComponent<SpriteRenderer>();
            
            // Create a simple 1x1 white sprite
            Texture2D tex = Texture2D.whiteTexture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            darkMask.sprite = sprite;
            darkMask.color = new Color(0, 0, 0, 0.7f);
            
            darkMask.sortingOrder = 25000; 
            
            // Scale it to cover the screen
            float height = Camera.main.orthographicSize * 2f;
            float width = height * Camera.main.aspect;
            float maxDim = Mathf.Max(width, height) * 100f * 2f;
            maskObj.transform.localScale = new Vector3(maxDim, maxDim, 1f);
            
            maskObj.SetActive(false);
        }

        private void CreateFinger(Sprite customSprite)
        {
            GameObject fingerObj = new GameObject("GuideFinger");
            fingerObj.transform.SetParent(transform);
            
            fingerSpriteRenderer = fingerObj.AddComponent<SpriteRenderer>();
            
            if (customSprite != null)
            {
                fingerSpriteRenderer.sprite = customSprite;
            }
            else
            {
                // Create a simple circle texture for the finger/pointer
                Texture2D tex = new Texture2D(64, 64);
                Color[] colors = new Color[64 * 64];
                float radius = 30f;
                Vector2 center = new Vector2(32f, 32f);
                for (int i = 0; i < 64 * 64; i++) 
                {
                    int x = i % 64;
                    int y = i / 64;
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    colors[i] = dist <= radius ? Color.white : Color.clear;
                }
                tex.SetPixels(colors);
                tex.Apply();
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 100f);
                fingerSpriteRenderer.sprite = sprite;
            }
            
            fingerSpriteRenderer.sortingOrder = 50; 
            fingerObj.transform.localScale = Vector3.one;
            
            fingerObj.SetActive(false);
        }

        private void Update()
        {
            if (boardManager == null) return;
            if (boardManager.gameManager != null && boardManager.gameManager.isGameOver)
            {
                // 如果游戏结束，直接销毁自身即可，不使用已被删除的 HideGuide
                if (isGuideActive) Destroy(this);
                return;
            }

            if (boardManager.currentState == BoardManager.BoardState.Waiting)
            {
                if (HasPendingTutorial && !isGuideActive)
                {
                    idleTimer += Time.deltaTime;
                    float delay = currentStepIndex == 0 ? delayToShowGuide : pendingTutorialDelay;
                    if (idleTimer >= delay)
                    {
                        ShowGuide();
                    }
                }
                else if (isGuideActive && HasPendingTutorial)
                {
                    // Allow real-time tweaking of offsets in the Inspector during play mode
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

            // Only show if the tutorial swipe is still valid
            if (!boardManager.IsValidTutorialMove(tutorialFrom, tutorialTo))
            {
                Debug.LogWarning($"[GuideManager] Tutorial move from {tutorialFrom} to {tutorialTo} is NOT valid! Guide destroyed.");
                EndTutorial();
                return;
            }

            isGuideActive = true;
            
            // Show Mask
            if (currentStep.showMask)
            {
                darkMask.gameObject.SetActive(true);
            }
            
            // Mask UI
            if (currentStep.showMask && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
            {
                boardManager.gameManager.uiManager.SetTutorialUIMask(true);
            }

            // Highlight all specified cells
            highlightedElements.Clear();
            originalElementOrders.Clear();
            highlightedBgTiles.Clear();
            originalBgOrders.Clear();
            highlightedMeshes.Clear();
            originalMeshOrders.Clear();

            List<Vector2Int> highlightCells = new List<Vector2Int>();
            if (currentStep.tutorialHighlightCells != null)
            {
                foreach(var h in currentStep.tutorialHighlightCells)
                {
                    if (h.x >= 0 && h.y >= 0) highlightCells.Add(h);
                }
            }
            if (highlightCells.Count == 0)
            {
                highlightCells.Add(tutorialFrom);
                if (tutorialFrom != tutorialTo) highlightCells.Add(tutorialTo);
            }

            if (currentStep.showMask)
            {
                foreach (Vector2Int pos in highlightCells)
                {
                    if (pos.x < 0 || pos.x >= boardManager.activeElements.GetLength(0) || pos.y < 0 || pos.y >= boardManager.activeElements.GetLength(1))
                        continue;

                    // Highlight Element
                    ElementView element = boardManager.activeElements[pos.x, pos.y];
                    if (element != null)
                    {
                        if (element.spriteRenderer != null)
                        {
                            highlightedElements.Add(element);
                            originalElementOrders.Add(element.spriteRenderer.sortingOrder);
                            element.spriteRenderer.sortingOrder = 25002; // Above the unified mask
                        }
                        
                        // Handle Spine animation if present
                        if (element is SpecialElementView specialView && specialView.spineAnimation != null)
                        {
                            var meshRenderer = specialView.spineAnimation.GetComponent<MeshRenderer>();
                            if (meshRenderer != null)
                            {
                                highlightedMeshes.Add(meshRenderer);
                                originalMeshOrders.Add(meshRenderer.sortingOrder);
                                meshRenderer.sortingOrder = 25002;
                            }
                        }
                    }

                    // Highlight Background Tile
                    Transform bgTile = boardManager.backgroundContainer.Find($"Tile_{pos.x}_{pos.y}");
                    if (bgTile != null)
                    {
                        SpriteRenderer sr = bgTile.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            highlightedBgTiles.Add(sr);
                            originalBgOrders.Add(sr.sortingOrder);
                            sr.sortingOrder = 25001; // Just below the elements, but above mask
                        }
                    }
                }
            }

            // Show Popup
            if (currentPopupInstance != null)
            {
                Destroy(currentPopupInstance);
                currentPopupInstance = null;
            }

            if (currentStep.showPopup && currentStep.popupPrefab != null)
            {
                Transform parentTransform = transform;
                if (boardManager != null && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
                {
                    parentTransform = boardManager.gameManager.uiManager.transform;
                }

                // Create a dedicated Canvas container for popups if it doesn't exist
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
                    
                    canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }

                currentPopupInstance = Instantiate(currentStep.popupPrefab, popupCanvasContainer.transform, false);
                
                // Reset RectTransform so it aligns with the UI properly, but preserve the prefab's original anchored position
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

            // Animate finger
            fingerSpriteRenderer.sortingOrder = 25005;
            fingerSpriteRenderer.gameObject.SetActive(true);
            
            float scale = currentStep.tutorialFingerScale > 0f ? currentStep.tutorialFingerScale : 1f;
            Vector2 offset = currentStep.tutorialFingerOffset;
            fingerSpriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);

            // Apply offset to the world positions
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
                // For clicking a single element (e.g. Special)
                seq.Append(fingerSpriteRenderer.transform.DOScale(scale * 0.8f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
            }
            // 不要改变手指透明度，直接重置位置即可
            seq.AppendCallback(() => {
                fingerSpriteRenderer.transform.position = fromWorld;
                fingerSpriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);
            });
            // 停顿一小会，再重新开始动画循环
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
                Destroy(currentPopupInstance);
                currentPopupInstance = null;
            }
            if (popupCanvasContainer != null)
            {
                Destroy(popupCanvasContainer.gameObject);
                popupCanvasContainer = null;
            }
            if (darkMask != null) darkMask.gameObject.SetActive(false);
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
            for (int i = 0; i < highlightedBgTiles.Count; i++)
            {
                if (highlightedBgTiles[i] != null)
                {
                    highlightedBgTiles[i].sortingOrder = originalBgOrders[i];
                }
            }
            for (int i = 0; i < highlightedMeshes.Count; i++)
            {
                if (highlightedMeshes[i] != null)
                {
                    highlightedMeshes[i].sortingOrder = originalMeshOrders[i];
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
            if (!isGuideActive) return;
            isGuideActive = false;
            idleTimer = 0f;
            if (currentPopupInstance != null)
            {
                Destroy(currentPopupInstance);
                currentPopupInstance = null;
            }
            if (popupCanvasContainer != null)
            {
                Destroy(popupCanvasContainer.gameObject);
                popupCanvasContainer = null;
            }
            if (darkMask != null) darkMask.gameObject.SetActive(false);

            if (boardManager != null && boardManager.gameManager != null && boardManager.gameManager.uiManager != null)
            {
                boardManager.gameManager.uiManager.SetTutorialUIMask(false);
            }

            if (fingerSpriteRenderer != null)
            {
                DOTween.Kill(fingerSpriteRenderer.transform);
                fingerSpriteRenderer.gameObject.SetActive(false);
            }

            // Restore sorting orders
            for (int i = 0; i < highlightedElements.Count; i++)
            {
                if (highlightedElements[i] != null && highlightedElements[i].spriteRenderer != null)
                {
                    highlightedElements[i].spriteRenderer.sortingOrder = originalElementOrders[i];
                }
            }

            for (int i = 0; i < highlightedBgTiles.Count; i++)
            {
                if (highlightedBgTiles[i] != null)
                {
                    highlightedBgTiles[i].sortingOrder = originalBgOrders[i];
                }
            }

            for (int i = 0; i < highlightedMeshes.Count; i++)
            {
                if (highlightedMeshes[i] != null)
                {
                    highlightedMeshes[i].sortingOrder = originalMeshOrders[i];
                }
            }

            // 【关键修改】：如果是由广告强制中断的，我们需要确保把这些记录给清空
            highlightedElements.Clear();
            originalElementOrders.Clear();
            highlightedBgTiles.Clear();
            originalBgOrders.Clear();
            highlightedMeshes.Clear();
            originalMeshOrders.Clear();
            
            // Advance to next step
            currentStepIndex++;
            
            if (currentStepIndex >= tutorialSteps.Count)
            {
                EndTutorial();
            }
        }

        private void EndTutorial()
        {
            // Unregister from GameManager so it doesn't show as Missing
            if (boardManager != null && boardManager.gameManager != null)
            {
                boardManager.gameManager.guideManager = null;
            }
            
            // Self-destroy after tutorial is complete
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (currentPopupInstance != null)
            {
                Destroy(currentPopupInstance);
                currentPopupInstance = null;
            }
            if (popupCanvasContainer != null)
            {
                Destroy(popupCanvasContainer.gameObject);
                popupCanvasContainer = null;
            }
            if (darkMask != null && darkMask.gameObject != null)
            {
                Destroy(darkMask.gameObject);
            }
            if (fingerSpriteRenderer != null && fingerSpriteRenderer.gameObject != null)
            {
                fingerSpriteRenderer.transform.DOKill();
                Destroy(fingerSpriteRenderer.gameObject);
            }
        }
    }
}

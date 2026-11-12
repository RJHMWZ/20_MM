using UnityEngine;
using System.Collections.Generic;
using Match3.UI;

namespace Match3
{
    public class GameManager : MonoBehaviour
    {
        [Header("Data References")]
        public List<LevelData> levelDataList = new List<LevelData>();
        public LevelData currentLevelData => (levelDataList != null && levelDataList.Count > 0 && currentLevelIndex < levelDataList.Count) ? levelDataList[currentLevelIndex] : null;
        public int currentLevelIndex { get; private set; } = 0;

        public ElementDatabase elementDatabase;
        
        [Header("Scene References")]
        public GameUIManager uiManager;
        public BoardManager boardManager;

        [Header("Tutorial References")]
        public Sprite tutorialFingerSprite;

        [Header("Startup Loading Screen")]
        [SerializeField, Min(0f)] private float simulatedLoadingDuration = 1.6f;

        private int currentSteps;
        public int playedSteps = 0; // 记录玩家在当前关卡已经走的步数
        public bool isGameOver { get; private set; } = false;
        public bool isAdShowing = false;
        // Keep track of runtime target progress
        public List<RuntimeTarget> activeTargets = new List<RuntimeTarget>();

        public GuideManager guideManager;

        private System.Collections.IEnumerator Start()
        {
            RuntimeLoadingScreen loadingScreen = RuntimeLoadingScreen.Create();

            if (levelDataList != null && levelDataList.Count > 0 && uiManager != null && elementDatabase != null)
            {
                // Create GuideManager
                guideManager = gameObject.AddComponent<GuideManager>();
                
                // Wait for the UI layout to be fully built and orientation to be applied
                // 这里等待的时间必须足够长，确保 GameUIManager 里的 Start 和所有初始化都已经完成
                yield return new WaitForEndOfFrame();
                
                // IMPORTANT: Wait one more frame to ensure any DOTween animations started by UI have properly initialized their targets
                // before we potentially start board animations.
                yield return new WaitForEndOfFrame();

                float elapsed = 0f;
                while (elapsed < simulatedLoadingDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float normalizedProgress = simulatedLoadingDuration <= 0f
                        ? 1f
                        : Mathf.Clamp01(elapsed / simulatedLoadingDuration);

                    // Ease-out makes the simulated loading feel less mechanical.
                    loadingScreen.SetProgress(1f - Mathf.Pow(1f - normalizedProgress, 2f));
                    yield return null;
                }

                loadingScreen.SetProgress(1f);
                yield return new WaitForSecondsRealtime(0.12f);
                loadingScreen.Close();
                
                StartGame();
            }
            else
            {
                loadingScreen.Close();
                Debug.LogError("GameManager: Missing references! Please assign LevelData list, ElementDatabase, and GameUIManager.");
            }
        }

        public void StartGame()
        {
            isGameOver = false;
            
            // 恢复播放BGM（如果是切换关卡或重玩，BGM可能在胜利/失败时被停止）
            if (AudioManager.Instance != null && AudioManager.Instance.bgmClip != null)
            {
                if (AudioManager.Instance.bgmSource != null && (!AudioManager.Instance.bgmSource.isPlaying || AudioManager.Instance.bgmSource.clip != AudioManager.Instance.bgmClip))
                {
                    AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmClip);
                }
            }

            // 1. Setup Steps
            currentSteps = currentLevelData.maxSteps;
            playedSteps = 0; // 新关卡重置步数
            
            // 2. Setup Targets
            activeTargets.Clear();
            List<TargetData> uiTargets = new List<TargetData>();
            
            foreach (var target in currentLevelData.levelTargets)
            {
                RuntimeTarget rt = new RuntimeTarget();
                rt.definition = target;
                rt.remainingAmount = target.requiredAmount;
                activeTargets.Add(rt);
                
                // Get the correct sprite for the UI based on what we are collecting
                Sprite iconSprite = null;
                if (target.targetType == CellItemType.SpecificNormal)
                {
                    var norm = elementDatabase.GetNormalElement(target.normalElementId);
                    if (norm != null) iconSprite = norm.sprite;
                }
                else if (target.targetType == CellItemType.Special && target.specialElement != null)
                {
                    iconSprite = target.specialElement.sprite;
                }
                else if (target.targetType == CellItemType.RetractingCollectible && target.collectibleData != null)
                {
                    iconSprite = target.collectibleData.headSprite;
                }
                else if (target.targetType == CellItemType.Obstacle && target.obstacleData != null)
                {
                    if (target.obstacleData.prefab != null)
                    {
                        var sr = target.obstacleData.prefab.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null) iconSprite = sr.sprite;
                    }
                }
                else if (target.targetType == CellItemType.Generator && target.generatorData != null)
                {
                    iconSprite = target.generatorData.targetIcon;
                }
                else if (target.targetType == CellItemType.Mother && target.motherData != null)
                {
                    iconSprite = target.motherData.targetIcon;
                }
                else if (target.targetType == CellItemType.Child && target.childData != null)
                {
                    if (target.childData.prefab != null)
                    {
                        var sr = target.childData.prefab.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null) iconSprite = sr.sprite;
                    }
                }

                uiTargets.Add(new TargetData { icon = iconSprite, requiredAmount = target.requiredAmount });
            }

            // 3. Initialize UI
            uiManager.InitializeUI("Download Now for more levels!", currentSteps);
            uiManager.SetupTargets(uiTargets);
            
            // 4. Initialize Board
            if (boardManager != null)
            {
                boardManager.gameManager = this;
                boardManager.InitializeBoard(currentLevelData, elementDatabase, uiManager.boardAreaRect);
                
                // Refresh GuideManager if needed
                if (guideManager == null)
                {
                    guideManager = gameObject.AddComponent<GuideManager>();
                }
                
                if (currentLevelData.hasTutorial && currentLevelData.tutorialSteps != null && currentLevelData.tutorialSteps.Count > 0) 
                {
                    guideManager.Initialize(boardManager, tutorialFingerSprite, currentLevelData.tutorialSteps);
                }
            }
            else
            {
                Debug.LogError("GameManager: BoardManager is not assigned!");
            }
        }

        /// <summary>
        /// Call this method when the player makes a move.
        /// </summary>
        public void OnPlayerMove()
        {
            if (isGameOver) return;
            currentSteps--;
            playedSteps++;
            
            uiManager.UpdateSteps(currentSteps);
            
            // Check win condition after every move in case they ran out of steps
            CheckWinCondition();
            
            // （此处移除了原本在完成滑动后的广告检测逻辑，改为了在 BoardManager 玩家按下时拦截）
        }

        public void TriggerGlobalAd()
        {
            if (isAdShowing) return;
            isAdShowing = true;

            // 隐藏棋盘元素，防止在 WebGL 中由于层级问题遮挡广告弹窗
            if (boardManager != null)
            {
                // 兜底清理魔力球聚光遮罩，避免残留遮罩与广告弹窗叠加
                boardManager.ClearMagicFocusMask();
                if (boardManager.backgroundContainer != null) boardManager.backgroundContainer.gameObject.SetActive(false);
                if (boardManager.elementsContainer != null) boardManager.elementsContainer.gameObject.SetActive(false);
            }
            if (uiManager != null && uiManager.centerBorderRect != null)
            {
                uiManager.centerBorderRect.gameObject.SetActive(false);
            }

            if (uiManager != null)
            {
                if (Manager.ConfigManager.Instance != null && Manager.ConfigManager.Instance.adTriggerType == Manager.AdTriggerType.DownloadPanel)
                {
                    uiManager.ShowDownloadPanel();
                }
                else
                {
                    uiManager.ShowGlobalAdOverlay();
                }
            }
        }

        /// <summary>
        /// Call this method when an element is collected/destroyed on the board.
        /// Returns the target index if it was a target, or -1 if not.
        /// </summary>
        public int OnElementCollected(CellItemType type, int normalId = -1, SpecialElementData special = null, CollectibleElementData collectible = null, ObstacleData obstacle = null, GeneratorData generator = null, MotherData mother = null, ChildData child = null)
        {
            if (isGameOver) return -1;
            bool UIneedsUpdate = false;
            int collectedTargetIndex = -1;

            for (int i = 0; i < activeTargets.Count; i++)
            {
                var rt = activeTargets[i];
                if (rt.remainingAmount <= 0) continue; // Already completed this target

                bool isMatch = false;
                if (rt.definition.targetType == type)
                {
                    if (type == CellItemType.SpecificNormal && rt.definition.normalElementId == normalId) isMatch = true;
                    else if (type == CellItemType.Special && rt.definition.specialElement == special) isMatch = true;
                    else if (type == CellItemType.RetractingCollectible && rt.definition.collectibleData == collectible) isMatch = true;
                    else if (type == CellItemType.Obstacle && rt.definition.obstacleData == obstacle) isMatch = true;
                    else if (type == CellItemType.Generator && rt.definition.generatorData == generator) isMatch = true;
                    else if (type == CellItemType.Mother && rt.definition.motherData == mother) isMatch = true;
                    else if (type == CellItemType.Child && rt.definition.childData == child) isMatch = true;
                }

                if (isMatch)
                {
                    rt.remainingAmount--;
                    uiManager.UpdateTarget(i, rt.remainingAmount);
                    UIneedsUpdate = true;
                    collectedTargetIndex = i;
                    break; // Only collect one target per element
                }
            }

            if (UIneedsUpdate)
            {
                CheckWinCondition();
            }
            
            return collectedTargetIndex;
        }

        public void OnBoardSettled()
        {
            if (!isGameOver)
            {
                CheckWinCondition();
            }
        }

        private void CheckWinCondition()
        {
            if (isGameOver) return;

            bool allTargetsMet = true;
            foreach (var rt in activeTargets)
            {
                if (rt.remainingAmount > 0)
                {
                    allTargetsMet = false;
                    break;
                }
            }

            if (allTargetsMet)
            {
                isGameOver = true;
                Debug.Log("Level Complete! You Win!");
                if (AudioManager.Instance != null) AudioManager.Instance.PlayVictorySound();

                // If there are more levels, show Next Level UI, otherwise Victory
                bool hasNextLevel = (levelDataList != null && currentLevelIndex < levelDataList.Count - 1);
                StartCoroutine(ShowSettlementUIWithDelay(true, hasNextLevel));
            }
            else if (currentSteps <= 0)
            {
                // Only declare defeat if the board is fully settled
                if (boardManager != null && boardManager.currentState != BoardManager.BoardState.Waiting)
                {
                    return;
                }
                
                isGameOver = true;
                Debug.Log("Out of Steps! You Lose!");
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDefeatSound();
                StartCoroutine(ShowSettlementUIWithDelay(false, false));
            }
        }

        private System.Collections.IEnumerator ShowSettlementUIWithDelay(bool isWin, bool hasNextLevel = false)
        {
            // Delay before showing the UI
            yield return new WaitForSeconds(1.0f);

            // Wait until board is no longer animating
            if (boardManager != null)
            {
                yield return new WaitUntil(() => boardManager.currentState == BoardManager.BoardState.Waiting);
            }

            if (isWin && hasNextLevel && Manager.ConfigManager.Instance.levelTransitionType == Manager.LevelTransitionType.Fade)
            {
                if (uiManager != null)
                {
                    PlaySettlementSound();
                    yield return StartCoroutine(PlayVictorySettlementAboveBoard());
                }

                // Fade out transition
                if (uiManager != null)
                {
                    yield return StartCoroutine(uiManager.FadeOutCoroutine(0.5f));
                }

                // Proceed to next level silently
                currentLevelIndex++;
                StartGame();

                // Fade in transition
                if (uiManager != null)
                {
                    yield return StartCoroutine(uiManager.FadeInCoroutine(0.5f));
                }
                
                yield break;
            }

            // Standard Popup logic
            if (isWin && uiManager != null)
            {
                PlaySettlementSound();
                yield return StartCoroutine(PlayVictorySettlementAboveBoard());
            }

            // Hide board elements
            if (boardManager != null)
            {
                if (boardManager.backgroundContainer != null) boardManager.backgroundContainer.gameObject.SetActive(false);
                if (boardManager.elementsContainer != null) boardManager.elementsContainer.gameObject.SetActive(false);
            }
            if (uiManager != null && uiManager.centerBorderRect != null)
            {
                uiManager.centerBorderRect.gameObject.SetActive(false);
            }
            
            // Show corresponding UI panel
            if (uiManager != null)
            {
                if (isWin)
                {
                    if (hasNextLevel)
                    {
                        uiManager.ShowNextLevelPanel();
                    }
                    else
                    {
                        uiManager.ShowVictoryPanel();
                    }
                }
                else
                {
                    PlaySettlementSound();
                    uiManager.ShowDefeatPanel();
                }
            }
        }

        /// <summary>
        /// 结算界面出现时的音效（由 AudioManager.settlementClip 提供）。
        /// </summary>
        private void PlaySettlementSound()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySettlementSound();
        }

        private System.Collections.IEnumerator PlayVictorySettlementAboveBoard()
        {
            if (uiManager == null) yield break;

            // Tutorial highlighting can raise board renderers to 25002. Clear it before
            // settlement so it cannot leak above the settlement canvas.
            if (guideManager != null)
            {
                guideManager.ClearGuideInstantly();
                uiManager.SetTutorialUIMask(false);
            }
            // 魔力球聚光遮罩同样兜底清理，防止残留遮罩盖住结算界面
            if (boardManager != null)
            {
                boardManager.ClearMagicFocusMask();
            }

            Dictionary<Renderer, RendererSortingState> boardRendererStates = new Dictionary<Renderer, RendererSortingState>();
            SetBoardSettlementSorting(boardManager != null ? boardManager.backgroundContainer : null, boardRendererStates);
            SetBoardSettlementSorting(boardManager != null ? boardManager.elementsContainer : null, boardRendererStates);

            yield return StartCoroutine(uiManager.PlayVictorySettlement());

            foreach (KeyValuePair<Renderer, RendererSortingState> entry in boardRendererStates)
            {
                if (entry.Key == null) continue;
                entry.Key.sortingLayerID = entry.Value.sortingLayerID;
                entry.Key.sortingOrder = entry.Value.sortingOrder;
            }
        }

        private static void SetBoardSettlementSorting(Transform root, Dictionary<Renderer, RendererSortingState> states)
        {
            if (root == null) return;
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || states.ContainsKey(renderer)) continue;
                states.Add(renderer, new RendererSortingState(renderer));
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = GameUIManager.BoardDuringSettlementSortingOrder;
            }
        }

        private struct RendererSortingState
        {
            public readonly int sortingLayerID;
            public readonly int sortingOrder;

            public RendererSortingState(Renderer renderer)
            {
                sortingLayerID = renderer.sortingLayerID;
                sortingOrder = renderer.sortingOrder;
            }
        }

        public void LoadNextLevel()
        {
            if (levelDataList != null && currentLevelIndex < levelDataList.Count - 1)
            {
                currentLevelIndex++;
                
                if (uiManager != null)
                {
                    uiManager.HideNextLevelPanel();
                }

                if (boardManager != null)
                {
                    if (boardManager.backgroundContainer != null) boardManager.backgroundContainer.gameObject.SetActive(true);
                    if (boardManager.elementsContainer != null) boardManager.elementsContainer.gameObject.SetActive(true);
                }
                if (uiManager != null && uiManager.centerBorderRect != null)
                {
                    uiManager.centerBorderRect.gameObject.SetActive(true);
                }

                StartGame();
            }
        }

        /// <summary>
        /// Open the Google Play Store link for the game.
        /// Can be hooked up to UI Buttons via the Unity Inspector.
        /// </summary>
        public void DownloadGame()
        {
            Luna.Unity.Playable.InstallFullGame();
        }
    }

    // Helper class to track progress during gameplay
    public class RuntimeTarget
    {
        public LevelTarget definition;
        public int remainingAmount;
    }
}

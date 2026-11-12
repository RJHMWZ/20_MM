using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Match3
{
    [System.Serializable]
    public class ConveyorCornerConfig
    {
        public Direction inDir;
        public Direction outDir;
        public float rotationAngle;
        public bool flipX;
        public bool flipY;
        public string animationName = "line_ru";
    }

    public class BoardManager : MonoBehaviour
    {
        [Header("Board Settings")]
        [Tooltip("The size of each cell in world space units")]
        public float cellSize = 1f;
        [Tooltip("Spacing between cells")]
        public float cellSpacing = 0.1f;
        [Tooltip("Background tile prefab for playable grid spots")]
        public GameObject backgroundTilePrefab;
        [Tooltip("Parent transform for all background tiles")]
        public Transform backgroundContainer;
        [Tooltip("Parent transform for all active game elements")]
        public Transform elementsContainer;
        [Tooltip("Padding for the board edges (e.g. 0.08 means 8% padding). Set to 0 if you want the board to fit exactly into the UI board area without gaps.")]
        [Range(0f, 0.5f)]
        public float boardEdgePadding = 0.0f;

        [Header("Conveyor Settings")]
        public List<ConveyorCornerConfig> cornerConfigs = new List<ConveyorCornerConfig>();

        // Internal State
        public LevelData currentLevel { get; private set; }
        private ElementDatabase elementDb;
        public ElementView[,] activeElements;
        private int[] refillSequenceIndices;
        private readonly HashSet<string> refillWarnings = new HashSet<string>();
        private Vector3 boardBottomLeftOrigin; // Used to calculate world positions
        public GameManager gameManager; // Reference to GameManager for callbacks

        public HashSet<SpecialElementView> currentlyTriggeringSpecials = new HashSet<SpecialElementView>();
        public List<int> pickedColorsThisTurn = new List<int>();
        public bool isBoardClearing = false;
        /// <summary>
        /// 当前仍在运行的链式特效协程数量。
        /// 链式特效会在飞行过程中持续清除格子,重力必须等它们全部结束后才能执行,
        /// 否则特效晚于重力清除的格子会成为永远不落下的空洞。
        /// </summary>
        private int activeSpecialChains = 0;
        private bool hasConveyorMovedThisTurn = true;

        public enum BoardState { Waiting, Animating }
        public BoardState currentState = BoardState.Waiting;

        private ElementView selectedElement;
        private Vector2 swipeStartPos;

        private Sprite cachedDefaultBgSprite;

        private float originalCellSize = -1f;
        private float originalCellSpacing = -1f;
        
        public bool pendingRefill = false;

        // ========== 魔力球聚光遮罩 ==========
        // 魔力球播放动画期间：全屏半透明黑遮罩盖住棋盘，只保留魔力球动画与粒子特效明亮。
        // 参照 GuideManager 的统一遮罩方案：遮罩 sortingOrder=25000，被点亮的物体提升到 25002。
        private const int MagicFocusMaskOrder = 25000;
        private const int MagicFocusRaisedOrder = 25002;
        private SpriteRenderer magicFocusMask;
        private int magicFocusMaskRefCount = 0;
        private readonly List<Renderer> raisedFocusRenderers = new List<Renderer>();
        private readonly List<int> raisedFocusOriginalOrders = new List<int>();

        private void Update()
        {
            if (currentState == BoardState.Waiting)
            {
                if (currentlyTriggeringSpecials.Count > 0) currentlyTriggeringSpecials.Clear();
                if (pickedColorsThisTurn.Count > 0) pickedColorsThisTurn.Clear();

                if (pendingRefill)
                {
                    pendingRefill = false;
                    currentState = BoardState.Animating;
                    StartCoroutine(ApplyGravityAndRefill());
                    return;
                }
            }

            if (currentState != BoardState.Waiting) return;
            if (gameManager != null && gameManager.isGameOver) return; // Stop interactions if game is over
            if (gameManager != null && gameManager.isAdShowing) return; // 拦截所有交互

            if (Input.GetMouseButtonDown(0))
            {
                // ========== 新增：按下第一下时，拦截检测广告配置 ==========
                if (gameManager != null && Manager.ConfigManager.Instance != null && !string.IsNullOrEmpty(Manager.ConfigManager.Instance.adTriggerStep))
                {
                    // 获取当前准备要走的这一步（还没算进playedSteps里）
                    string currentKey = $"{gameManager.currentLevelIndex + 1}_{gameManager.playedSteps}";
                    
                    if (Manager.ConfigManager.Instance.adTriggerStep.Trim() == currentKey)
                    {
                        // 触发广告，阻断玩家后续任何点击
                        gameManager.TriggerGlobalAd();
                        return;
                    }
                }
                // =========================================================

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                Vector2Int gridPos = GetGridPositionFromWorld(mouseWorldPos);

                if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
                {
                    // If tutorial hasn't shown yet or is active, block everything except the exact target
                    if (!gameManager.guideManager.IsGuideActive || (gridPos != gameManager.guideManager.CurrentTutorialFrom && gridPos != gameManager.guideManager.CurrentTutorialTo))
                    {
                        return;
                    }
                }

                if (IsValidGridPos(gridPos.x, gridPos.y) && activeElements[gridPos.x, gridPos.y] != null)
                {
                    selectedElement = activeElements[gridPos.x, gridPos.y];
                    swipeStartPos = mouseWorldPos;
                }
            }
            else if (Input.GetMouseButtonUp(0) && selectedElement != null)
            {
                // In tutorial, check if we need to block early interactions before swipe/click parsing
                if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
                {
                    if (!gameManager.guideManager.IsGuideActive)
                    {
                        selectedElement = null;
                        return;
                    }

                    Vector2Int from = gameManager.guideManager.CurrentTutorialFrom;
                    Vector2Int to = gameManager.guideManager.CurrentTutorialTo;
                    Vector2Int selPos = new Vector2Int(selectedElement.GridX, selectedElement.GridY);
                    
                    // Allow clicking the current element if it's the tutorial target
                    if (from == to)
                    {
                        if (selPos != from)
                        {
                            selectedElement = null;
                            return; // Block incorrect click during tutorial
                        }
                    }
                }

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                Vector2 swipeDelta = (Vector2)mouseWorldPos - swipeStartPos;

                if (swipeDelta.magnitude > cellSize * 0.2f)
                {
                    // Determine swipe direction
                    Vector2Int direction = Vector2Int.zero;
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        direction = swipeDelta.x > 0 ? Vector2Int.right : Vector2Int.left;
                    }
                    else
                    {
                        direction = swipeDelta.y > 0 ? Vector2Int.up : Vector2Int.down;
                    }

                    Vector2Int targetPos = new Vector2Int(selectedElement.GridX + direction.x, selectedElement.GridY + direction.y);
                    
                    if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
                    {
                        if (!gameManager.guideManager.IsGuideActive)
                        {
                            selectedElement = null;
                            return;
                        }

                        Vector2Int from = gameManager.guideManager.CurrentTutorialFrom;
                        Vector2Int to = gameManager.guideManager.CurrentTutorialTo;
                        Vector2Int selPos = new Vector2Int(selectedElement.GridX, selectedElement.GridY);
                        
                        // Prevent swiping if the tutorial step is a pure click (from == to)
                        if (from == to)
                        {
                            selectedElement = null;
                            return;
                        }

                        if (!((selPos == from && targetPos == to) || (selPos == to && targetPos == from)))
                        {
                            selectedElement = null;
                            return; // Block incorrect swipe during tutorial
                        }
                        else
                        {
                            // Correct tutorial swipe, complete the tutorial
                            gameManager.guideManager.CompleteTutorial();
                        }
                    }

                    if (IsValidGridPos(targetPos.x, targetPos.y) && activeElements[targetPos.x, targetPos.y] != null)
                    {
                        ElementView targetElement = activeElements[targetPos.x, targetPos.y];
                        
                        // Use IsElementSwappable to check if swapping is allowed
                        if (IsElementSwappable(selectedElement) && IsElementSwappable(targetElement))
                        {
                            // Swap!
                            if (AudioManager.Instance != null) AudioManager.Instance.PlaySwapSound();
                            StartCoroutine(SwapAndCheck(selectedElement, targetElement));
                        }
                    }
                }
                else
                {
                    // It's a Click
                    if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
                    {
                        if (!gameManager.guideManager.IsGuideActive)
                        {
                            selectedElement = null;
                            return;
                        }

                        Vector2Int from = gameManager.guideManager.CurrentTutorialFrom;
                        Vector2Int to = gameManager.guideManager.CurrentTutorialTo;
                        Vector2Int selPos = new Vector2Int(selectedElement.GridX, selectedElement.GridY);
                        
                        // Prevent clicking if the tutorial step is a swipe (from != to)
                        if (from != to)
                        {
                            selectedElement = null;
                            return;
                        }

                        if (selPos != from)
                        {
                            selectedElement = null;
                            return; // Block incorrect click during tutorial
                        }
                        else
                        {
                            // Correct tutorial click, complete the tutorial
                            gameManager.guideManager.CompleteTutorial();
                        }
                    }

                    if (selectedElement is SpecialElementView specialView)
                    {
                        StartCoroutine(TriggerSpecialOnClick(specialView));
                    }
                }
                selectedElement = null;
            }
        }

        private System.Collections.IEnumerator TriggerSpecialOnClick(SpecialElementView special)
        {
            currentState = BoardState.Animating;
            hasConveyorMovedThisTurn = false;
            retractedThisTurn.Clear();
            directlyHitCollectiblesThisTurn.Clear();
            damagedObstaclesThisTurn.Clear();
            triggeredGeneratorsThisTurn.Clear();
            if (gameManager != null) gameManager.OnPlayerMove();
            
            yield return StartCoroutine(ActivateSpecial(special, null));
            
            yield return StartCoroutine(ApplyGravityAndRefill());
        }

        private bool IsValidGridPos(int x, int y)
        {
            return x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height && currentLevel.GetCell(x, y) != null && currentLevel.GetCell(x, y).isPlayable;
        }

        private Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - boardBottomLeftOrigin;
            int x = Mathf.RoundToInt(localPos.x / (cellSize + cellSpacing));
            int y = Mathf.RoundToInt(localPos.y / (cellSize + cellSpacing));
            return new Vector2Int(x, y);
        }

        private System.Collections.IEnumerator SwapAndCheck(ElementView elementA, ElementView elementB)
        {
            currentState = BoardState.Animating;
            hasConveyorMovedThisTurn = false;
            retractedThisTurn.Clear();
            directlyHitCollectiblesThisTurn.Clear();
            damagedObstaclesThisTurn.Clear();
            triggeredGeneratorsThisTurn.Clear();

            int ax = elementA.GridX, ay = elementA.GridY;
            int bx = elementB.GridX, by = elementB.GridY;

            // Update array
            activeElements[ax, ay] = elementB;
            activeElements[bx, by] = elementA;

            // Animate
            bool animFinished = false;
            elementA.MoveTo(bx, by, GetWorldPosition(bx, by), 0.3f, () => animFinished = true);
            elementB.MoveTo(ax, ay, GetWorldPosition(ax, ay), 0.3f);

            yield return new WaitUntil(() => animFinished);

            // Special Interaction Check
            bool triggeredSpecial = false;
            if (elementA is SpecialElementView specA1 && elementB is SpecialElementView specB1)
            {
                triggeredSpecial = true;
                yield return StartCoroutine(ActivateSpecialCombo(specA1, specB1));
            }
            else if (elementA is SpecialElementView specA)
            {
                // Before triggering special, check if the swap itself forms a normal match
                List<MatchCluster> specialSwapClusters = FindAllMatchClusters(elementA, elementB);
                if (specialSwapClusters.Count > 0)
                {
                    if (gameManager != null) gameManager.OnPlayerMove();
                    yield return StartCoroutine(ProcessMatchesAndRefill(specialSwapClusters));
                }
                triggeredSpecial = true;
                yield return StartCoroutine(ActivateSpecial(specA, elementB));
            }
            else if (elementB is SpecialElementView specB)
            {
                // Before triggering special, check if the swap itself forms a normal match
                List<MatchCluster> specialSwapClusters = FindAllMatchClusters(elementA, elementB);
                if (specialSwapClusters.Count > 0)
                {
                    if (gameManager != null) gameManager.OnPlayerMove();
                    yield return StartCoroutine(ProcessMatchesAndRefill(specialSwapClusters));
                }
                triggeredSpecial = true;
                yield return StartCoroutine(ActivateSpecial(specB, elementA));
            }

            if (triggeredSpecial)
            {
                // After special trigger, apply gravity and refill
                if (gameManager != null) gameManager.OnPlayerMove();
                yield return StartCoroutine(ApplyGravityAndRefill());
                yield break;
            }

            // Normal Match Check
            List<MatchCluster> clusters = FindAllMatchClusters(elementA, elementB);

            if (clusters.Count > 0)
            {
                // Process Matches
                if (gameManager != null) gameManager.OnPlayerMove();
                yield return StartCoroutine(ProcessMatchesAndRefill(clusters));
            }
            else
            {
                // Swap back
                activeElements[ax, ay] = elementA;
                activeElements[bx, by] = elementB;

                animFinished = false;
                elementA.MoveTo(ax, ay, GetWorldPosition(ax, ay), 0.3f, () => animFinished = true);
                elementB.MoveTo(bx, by, GetWorldPosition(bx, by), 0.3f);

                yield return new WaitUntil(() => animFinished);
                
                if (!HasPossibleMoves())
                {
                    yield return StartCoroutine(CheckAndShuffleIfNeeded());
                }
                else
                {
                    currentState = BoardState.Waiting;
                    if (gameManager != null) gameManager.OnBoardSettled();
                }
            }
        }

        public class MatchCluster
        {
            public List<NormalElementView> elements = new List<NormalElementView>();
            public CreationCondition shape = CreationCondition.None;
            public Vector2Int intersectionPoint;
        }

        private List<MatchCluster> FindAllMatchClusters()
        {
            return FindAllMatchClusters(null, null);
        }

        private List<MatchCluster> FindAllMatchClusters(ElementView swappedA, ElementView swappedB)
        {
            List<MatchCluster> finalClusters = new List<MatchCluster>();
            bool[,] processed = new bool[currentLevel.width, currentLevel.height];

            // 1. Find raw horizontal and vertical lines
            List<List<NormalElementView>> hLines = new List<List<NormalElementView>>();
            List<List<NormalElementView>> vLines = new List<List<NormalElementView>>();

            // Horizontal
            for (int y = 0; y < currentLevel.height; y++)
            {
                for (int x = 0; x < currentLevel.width - 2; x++)
                {
                    if (activeElements[x, y] is NormalElementView norm)
                    {
                        int matchLen = 1;
                        while (x + matchLen < currentLevel.width && activeElements[x + matchLen, y] is NormalElementView next && next.NormalElementId == norm.NormalElementId)
                        {
                            matchLen++;
                        }
                        if (matchLen >= 3)
                        {
                            List<NormalElementView> line = new List<NormalElementView>();
                            for (int i = 0; i < matchLen; i++) line.Add(activeElements[x + i, y] as NormalElementView);
                            hLines.Add(line);
                            x += matchLen - 1;
                        }
                    }
                }
            }

            // Vertical
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height - 2; y++)
                {
                    if (activeElements[x, y] is NormalElementView norm)
                    {
                        int matchLen = 1;
                        while (y + matchLen < currentLevel.height && activeElements[x, y + matchLen] is NormalElementView next && next.NormalElementId == norm.NormalElementId)
                        {
                            matchLen++;
                        }
                        if (matchLen >= 3)
                        {
                            List<NormalElementView> line = new List<NormalElementView>();
                            for (int i = 0; i < matchLen; i++) line.Add(activeElements[x, y + i] as NormalElementView);
                            vLines.Add(line);
                            y += matchLen - 1;
                        }
                    }
                }
            }

            // 2x2 Squares
            List<List<NormalElementView>> squares = new List<List<NormalElementView>>();
            for (int x = 0; x < currentLevel.width - 1; x++)
            {
                for (int y = 0; y < currentLevel.height - 1; y++)
                {
                    if (activeElements[x, y] is NormalElementView norm1 &&
                        activeElements[x + 1, y] is NormalElementView norm2 &&
                        activeElements[x, y + 1] is NormalElementView norm3 &&
                        activeElements[x + 1, y + 1] is NormalElementView norm4)
                    {
                        if (norm1.NormalElementId == norm2.NormalElementId &&
                            norm1.NormalElementId == norm3.NormalElementId &&
                            norm1.NormalElementId == norm4.NormalElementId)
                        {
                            List<NormalElementView> square = new List<NormalElementView> { norm1, norm2, norm3, norm4 };
                            squares.Add(square);
                        }
                    }
                }
            }

            // 2. Merge crossing lines (L-shape, T-shape, Cross)
            List<List<NormalElementView>> allLines = new List<List<NormalElementView>>();
            allLines.AddRange(hLines);
            allLines.AddRange(vLines);
            allLines.AddRange(squares);

            List<List<NormalElementView>> mergedClusters = new List<List<NormalElementView>>();
            while (allLines.Count > 0)
            {
                List<NormalElementView> currentCluster = new List<NormalElementView>(allLines[0]);
                allLines.RemoveAt(0);

                bool merged;
                do
                {
                    merged = false;
                    for (int i = allLines.Count - 1; i >= 0; i--)
                    {
                        // Check if they share any element
                        bool share = false;
                        foreach (var el in allLines[i])
                        {
                            if (currentCluster.Contains(el))
                            {
                                share = true;
                                break;
                            }
                        }
                        if (share)
                        {
                            foreach (var el in allLines[i])
                            {
                                if (!currentCluster.Contains(el)) currentCluster.Add(el);
                            }
                            allLines.RemoveAt(i);
                            merged = true;
                        }
                    }
                } while (merged);
                
                mergedClusters.Add(currentCluster);
            }

            // 3. Evaluate Shape for each cluster
            foreach (var clusterGroup in mergedClusters)
            {
                MatchCluster cluster = new MatchCluster();
                cluster.elements = clusterGroup;
                
                int minX = 999, maxX = -1, minY = 999, maxY = -1;
                foreach(var e in clusterGroup)
                {
                    if (e.GridX < minX) minX = e.GridX;
                    if (e.GridX > maxX) maxX = e.GridX;
                    if (e.GridY < minY) minY = e.GridY;
                    if (e.GridY > maxY) maxY = e.GridY;
                }
                
                int width = maxX - minX + 1;
                int height = maxY - minY + 1;

                // Shape classification must be based on the actual matched lines, not merely
                // the cluster's bounding box. A complex cluster can contain a 2x2 square and
                // a real H/V intersection at the same time; the intersection must produce a bomb.
                bool hasLine5 = false;
                bool hasHorizontalFour = false;
                bool hasVerticalFour = false;
                bool hasCrossingLines = false;
                Vector2Int crossingPoint = Vector2Int.zero;

                foreach (var hLine in hLines)
                {
                    if (!hLine.Exists(clusterGroup.Contains)) continue;
                    if (hLine.Count >= 5) hasLine5 = true;
                    if (hLine.Count == 4) hasHorizontalFour = true;

                    foreach (var vLine in vLines)
                    {
                        if (!vLine.Exists(clusterGroup.Contains)) continue;
                        NormalElementView shared = hLine.Find(vLine.Contains);
                        if (shared != null)
                        {
                            if (!hasCrossingLines)
                            {
                                hasCrossingLines = true;
                                crossingPoint = new Vector2Int(shared.GridX, shared.GridY);
                            }
                            break;
                        }
                    }
                }

                foreach (var vLine in vLines)
                {
                    if (!vLine.Exists(clusterGroup.Contains)) continue;
                    if (vLine.Count >= 5) hasLine5 = true;
                    if (vLine.Count == 4) hasVerticalFour = true;
                }

                // Find exact intersection point
                Vector2Int intersection = new Vector2Int(clusterGroup[0].GridX, clusterGroup[0].GridY);
                if (hasCrossingLines)
                {
                    intersection = crossingPoint;
                }
                else
                {
                    // For lines, prefer the swapped element if available, else center
                    NormalElementView normA = swappedA as NormalElementView;
                    NormalElementView normB = swappedB as NormalElementView;
                    if (normA != null && clusterGroup.Contains(normA))
                    {
                        intersection = new Vector2Int(swappedA.GridX, swappedA.GridY);
                    }
                    else if (normB != null && clusterGroup.Contains(normB))
                    {
                        intersection = new Vector2Int(swappedB.GridX, swappedB.GridY);
                    }
                    else
                    {
                        intersection = new Vector2Int(minX + width/2, minY + height/2);
                    }
                }

                cluster.intersectionPoint = intersection;

                bool has2x2 = false;
                if (width >= 2 && height >= 2)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        for (int y = minY; y < maxY; y++)
                        {
                            if (clusterGroup.Exists(e => e.GridX == x && e.GridY == y) &&
                                clusterGroup.Exists(e => e.GridX == x + 1 && e.GridY == y) &&
                                clusterGroup.Exists(e => e.GridX == x && e.GridY == y + 1) &&
                                clusterGroup.Exists(e => e.GridX == x + 1 && e.GridY == y + 1))
                            {
                                has2x2 = true;
                                break;
                            }
                        }
                        if (has2x2) break;
                    }
                }

                if (hasLine5)
                {
                    cluster.shape = CreationCondition.Line5;
                }
                else if (hasCrossingLines)
                {
                    // A real horizontal/vertical match intersection (L/T/Cross) takes priority
                    // over any incidental 2x2 contained in the same merged cluster.
                    cluster.shape = CreationCondition.CrossOrT;
                }
                else if (has2x2)
                {
                    // A 2x2 without a crossing H/V match produces a bird.
                    cluster.shape = CreationCondition.Square2x2;
                }
                else if (hasHorizontalFour)
                {
                    // 4个横向排列的元素 -> 生成纵向火箭
                    cluster.shape = CreationCondition.Line4Vertical;
                }
                else if (hasVerticalFour)
                {
                    // 4个竖向排列的元素 -> 生成横向火箭
                    cluster.shape = CreationCondition.Line4Horizontal;
                }

                finalClusters.Add(cluster);
            }

            return finalClusters;
        }

        public bool IsValidTutorialMove(Vector2Int from, Vector2Int to)
        {
            if (from.x < 0 || from.x >= currentLevel.width || from.y < 0 || from.y >= currentLevel.height) return false;
            if (to.x < 0 || to.x >= currentLevel.width || to.y < 0 || to.y >= currentLevel.height) return false;

            ElementView element = activeElements[from.x, from.y];
            if (element == null) return false;

            if (from == to && element is SpecialElementView) return true;

            ElementView targetElement = activeElements[to.x, to.y];
            if (targetElement == null) return false;
            
            if (from == to) return false; // Normal elements can't be swapped with themselves

            if (!IsElementSwappable(element) || !IsElementSwappable(targetElement)) return false;

            // Allow swap if one of them is a special element (Special + Normal or Special + Special)
            if (element is SpecialElementView || targetElement is SpecialElementView)
            {
                return true;
            }

            // Check if swap makes match (Removed for tutorial)
            // activeElements[from.x, from.y] = targetElement;
            // activeElements[to.x, to.y] = element;
            // bool hasMatch = FindAllMatchClusters(targetElement, element).Count > 0;
            // activeElements[from.x, from.y] = element;
            // activeElements[to.x, to.y] = targetElement;

            return true;
        }

        private bool IsElementSwappable(ElementView element)
        {
            if (element == null) return false;
            if (element is RetractingCollectibleView) return false; // Long objects are never swappable
            if (element is ObstacleView obs && !obs.Data.isSwappable) return false;
            if (element is GeneratorView gen && !gen.Data.isSwappable) return false;
            if (element is MotherView mother && !mother.Data.isSwappable) return false;
            if (element is ChildView child && !child.Data.isSwappable) return false;
            return true;
        }

        public bool GetPossibleMove(out Vector2Int from, out Vector2Int to)
        {
            from = Vector2Int.zero;
            to = Vector2Int.zero;
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    ElementView element = activeElements[x, y];
                    if (element == null) continue;

                    if (element is SpecialElementView) 
                    {
                        from = new Vector2Int(x, y);
                        to = new Vector2Int(x, y);
                        return true; // Clicking a special element is a valid move
                    }
                    if (!IsElementSwappable(element)) continue;

                    // Try swap Right
                    if (x < currentLevel.width - 1)
                    {
                        ElementView rightElement = activeElements[x + 1, y];
                        if (rightElement != null && IsElementSwappable(rightElement))
                        {
                            activeElements[x, y] = rightElement;
                            activeElements[x + 1, y] = element;
                            bool hasMatch = FindAllMatchClusters().Count > 0;
                            activeElements[x, y] = element;
                            activeElements[x + 1, y] = rightElement;
                            if (hasMatch)
                            {
                                from = new Vector2Int(x, y);
                                to = new Vector2Int(x + 1, y);
                                return true;
                            }
                        }
                    }

                    // Try swap Up
                    if (y < currentLevel.height - 1)
                    {
                        ElementView upElement = activeElements[x, y + 1];
                        if (upElement != null && IsElementSwappable(upElement))
                        {
                            activeElements[x, y] = upElement;
                            activeElements[x, y + 1] = element;
                            bool hasMatch = FindAllMatchClusters().Count > 0;
                            activeElements[x, y] = element;
                            activeElements[x, y + 1] = upElement;
                            if (hasMatch)
                            {
                                from = new Vector2Int(x, y);
                                to = new Vector2Int(x, y + 1);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool HasPossibleMoves()
        {
            Vector2Int from, to;
            return GetPossibleMove(out from, out to);
        }

        private System.Collections.IEnumerator CheckAndShuffleIfNeeded()
        {
            int maxAttempts = 50;
            while (!HasPossibleMoves() && maxAttempts > 0)
            {
                Debug.Log("No possible moves. Shuffling board...");
                currentState = BoardState.Animating;
                yield return new WaitForSeconds(0.5f); // Pause before shuffle

                List<NormalElementView> normals = new List<NormalElementView>();
                List<Vector2Int> positions = new List<Vector2Int>();

                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] is NormalElementView normal)
                        {
                            normals.Add(normal);
                            positions.Add(new Vector2Int(x, y));
                        }
                    }
                }

                bool validShuffle = false;
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        int randomIndex = Random.Range(i, positions.Count);
                        Vector2Int temp = positions[i];
                        positions[i] = positions[randomIndex];
                        positions[randomIndex] = temp;
                    }

                    for (int i = 0; i < normals.Count; i++)
                    {
                        Vector2Int pos = positions[i];
                        activeElements[pos.x, pos.y] = normals[i];
                    }

                    if (FindAllMatchClusters().Count == 0 && HasPossibleMoves())
                    {
                        validShuffle = true;
                        break;
                    }
                }

                if (!validShuffle)
                {
                    Debug.LogWarning("Could not find a perfect shuffle. Proceeding anyway.");
                }

                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] is NormalElementView normal)
                        {
                            normal.MoveTo(x, y, GetWorldPosition(x, y), 0.5f);
                        }
                    }
                }

                yield return new WaitForSeconds(0.6f);
                maxAttempts--;
            }

            List<MatchCluster> postShuffleClusters = FindAllMatchClusters();
            if (postShuffleClusters.Count > 0)
            {
                yield return StartCoroutine(ProcessMatchesAndRefill(postShuffleClusters));
            }
            else
            {
                currentState = BoardState.Waiting;
                if (gameManager != null) gameManager.OnBoardSettled();
            }
        }

        private System.Collections.IEnumerator ProcessMatchesAndRefill(List<MatchCluster> clusters)
        {
            retractedThisTurn.Clear();
            directlyHitCollectiblesThisTurn.Clear();
            damagedObstaclesThisTurn.Clear();
            triggeredGeneratorsThisTurn.Clear();
            List<ElementView> elementsToDestroy = new List<ElementView>();
            List<SpecialSpawnInfo> specialsToSpawn = new List<SpecialSpawnInfo>();

            // 1. Analyze clusters and determine specials
            foreach (var cluster in clusters)
            {
                foreach (var el in cluster.elements)
                {
                    if (!elementsToDestroy.Contains(el)) elementsToDestroy.Add(el);
                }

                if (cluster.shape != CreationCondition.None)
                {
                    // Find which special element to spawn
                    SpecialElementData specialDataToSpawn = null;
                    foreach (var spec in elementDb.specialElements)
                    {
                        if (spec.creationCondition == cluster.shape)
                        {
                            specialDataToSpawn = spec;
                            break;
                        }
                    }

                    if (specialDataToSpawn != null)
                    {
                        specialsToSpawn.Add(new SpecialSpawnInfo { x = cluster.intersectionPoint.x, y = cluster.intersectionPoint.y, data = specialDataToSpawn });
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find SpecialElementData for CreationCondition: {cluster.shape}. Make sure it is added to the ElementDatabase!");
                    }
                }
            }
            
            if (elementsToDestroy.Count > 0 && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayNormalMatchSound();
            }

            // 2. Destroy matched elements
            foreach (var match in elementsToDestroy)
            {
                int mx = match.GridX;
                int my = match.GridY;
                
                int targetIndex = -1;
                // Only notify if it hasn't been overwritten by a special spawn yet
                if (activeElements[mx, my] == match)
                {
                    if (match is NormalElementView normal)
                    {
                        if (gameManager != null) targetIndex = gameManager.OnElementCollected(CellItemType.SpecificNormal, normal.NormalElementId);
                    }
                    activeElements[mx, my] = null;
                    CheckAdjacentCollectibles(mx, my, false);
                }

                // Create pop effect here
                DestroyElementVisual(match, targetIndex);
            }

            // 2.5 等待旧元素销毁动画彻底结束（普通缩小 0.2s / 飞向目标 0.6s / 特殊触发动画）
            // 再生成特殊元素或下落，避免新元素与尚未清理干净的旧元素在视觉上重合
            yield return StartCoroutine(WaitForElementDestroys());

            // 3. Spawn Specials
            if (specialsToSpawn.Count > 0 && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySpecialGenerateSound();
            }

            foreach (var spawn in specialsToSpawn)
            {
                if (spawn.data.prefab != null)
                {
                    // 防御：目标格若仍残留未被清理的旧元素（幽灵引用/异常路径），先销毁其视觉，
                    // 再生成特殊元素，避免同格出现两个棋子重合
                    ElementView residual = activeElements[spawn.x, spawn.y];
                    if (residual != null)
                    {
                        activeElements[spawn.x, spawn.y] = null;

                        if (residual is RetractingCollectibleView || residual is MotherView)
                        {
                            // 多格元素（蛇/母体）整体共享一个 GameObject，不能直接销毁，
                            // 否则会连带破坏整个元素。仅移除格子引用，特殊元素照常生成。
                            Debug.LogWarning($"[BoardManager] 特殊元素生成格({spawn.x},{spawn.y})被多格元素占用：{residual.name}，仅移除引用、不销毁其 GameObject");
                        }
                        else if (residual.gameObject != null)
                        {
                            Debug.LogWarning($"[BoardManager] 特殊元素生成前清理目标格({spawn.x},{spawn.y})上的残留元素：{residual.name}");
                            Destroy(residual.gameObject);
                        }
                    }

                    GameObject newSpecObj = Instantiate(spawn.data.prefab, elementsContainer);
                    var specialView = newSpecObj.GetComponent<SpecialElementView>();
                    if (specialView != null)
                    {
                        specialView.Initialize(spawn.data, spawn.x, spawn.y, GetWorldPosition(spawn.x, spawn.y), cellSize);
                        activeElements[spawn.x, spawn.y] = specialView;
                        
                        // Save the target scale that was just calculated in Initialize
                        Vector3 targetScale = newSpecObj.transform.localScale;
                        
                        // Animate creation scale
                        newSpecObj.transform.localScale = Vector3.zero;
                        newSpecObj.transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                    }
                }
            }

            // 生成特效后不再额外停顿：下落与特效弹出动画同步开始，
            // 与普通消除的下落节奏保持一致（传统消除时旧元素缩放结束后立即下落）。
            // 特殊元素的 DOScale 弹入动画为独立异步动画，不与下落互斥。
            // 4. Gravity and Refill
            yield return StartCoroutine(ApplyGravityAndRefill());
        }

        private class SpecialSpawnInfo
        {
            public int x;
            public int y;
            public SpecialElementData data;
        }

        private bool CanFall(ElementView elementToFall)
        {
            if (elementToFall == null) return false;
            if (elementToFall is RetractingCollectibleView) return false;
            if (elementToFall is ObstacleView obs && !obs.Data.isMovable) return false;
            if (elementToFall is GeneratorView gen && !gen.Data.isMovable) return false;
            if (elementToFall is MotherView mother && !mother.Data.isMovable) return false;
            return true;
        }

        private NormalElementInfo GetRandomNormalElement()
        {
            if (elementDb == null || elementDb.normalElements == null || elementDb.normalElements.Count == 0)
            {
                Debug.LogError("Cannot refill board: ElementDatabase has no normal elements.");
                return null;
            }

            int validCount = 0;
            foreach (NormalElementInfo info in elementDb.normalElements)
            {
                if (info != null) validCount++;
            }

            if (validCount == 0)
            {
                Debug.LogError("Cannot refill board: ElementDatabase contains no valid normal elements.");
                return null;
            }

            int selectedValidIndex = Random.Range(0, validCount);
            foreach (NormalElementInfo info in elementDb.normalElements)
            {
                if (info == null) continue;
                if (selectedValidIndex-- == 0) return info;
            }

            return null;
        }

        private NormalElementInfo GetNextRefillElement(int column)
        {
            if (currentLevel == null ||
                column < 0 ||
                column >= currentLevel.width ||
                refillSequenceIndices == null ||
                column >= refillSequenceIndices.Length)
            {
                return GetRandomNormalElement();
            }

            List<ColumnRefillSequence> sequences = currentLevel.columnRefillSequences;
            if (sequences == null || column >= sequences.Count)
            {
                return GetRandomNormalElement();
            }

            ColumnRefillSequence sequence = sequences[column];
            if (sequence == null || sequence.normalElementIds == null)
            {
                return GetRandomNormalElement();
            }

            while (refillSequenceIndices[column] < sequence.normalElementIds.Count)
            {
                int configuredIndex = refillSequenceIndices[column]++;
                int configuredId = sequence.normalElementIds[configuredIndex];
                NormalElementInfo info = elementDb != null ? elementDb.GetNormalElement(configuredId) : null;

                if (info != null)
                {
                    return info;
                }

                string warningKey = column + ":" + configuredIndex;
                if (refillWarnings.Add(warningKey))
                {
                    Debug.LogWarning(
                        $"Invalid refill element ID {configuredId} at column {column}, sequence index {configuredIndex}.");
                }
            }

            return GetRandomNormalElement();
        }

        /// <summary>
        /// 等待所有长条收集物的收缩动画播放完毕。
        /// 解决：蛇头空格被清除后立刻下落/填充，会把新方块掉进仍在播放收缩动画的蛇身，造成两个棋子重合。
        /// </summary>
        private System.Collections.IEnumerator WaitForCollectibleRetracts()
        {
            const float maxWait = 1.0f;
            float elapsed = 0f;
            while (elapsed < maxWait)
            {
                bool anyAnimating = false;
                for (int x = 0; x < currentLevel.width && !anyAnimating; x++)
                {
                    for (int y = 0; y < currentLevel.height && !anyAnimating; y++)
                    {
                        if (activeElements[x, y] is RetractingCollectibleView rc && rc.IsRetracting)
                        {
                            anyAnimating = true;
                            break;
                        }
                    }
                }

                if (!anyAnimating) yield break;
                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        /// <summary>
        /// 等待所有处于销毁动画中的元素（缩小/飞向目标/特殊触发）彻底销毁。
        /// 解决：特殊元素生成或新元素下落时，旧元素销毁动画尚未结束，视觉上两者重合。
        /// </summary>
        private System.Collections.IEnumerator WaitForElementDestroys()
        {
            const float maxWait = 1.5f;
            float elapsed = 0f;
            while (elapsed < maxWait)
            {
                // 清理已被销毁的失效引用（Todo：GameObject 销毁后 Component == null）
                pendingElementDestroys.RemoveWhere(el => el == null);
                if (pendingElementDestroys.Count == 0) yield break;
                yield return null;
                elapsed += Time.deltaTime;
            }
            // 超时兜底：强制清空，避免协程卡死
            pendingElementDestroys.Clear();
        }

        /// <summary>
        /// 启动一个"被跟踪"的链式特效协程：计数 +1，协程结束时 -1。
        /// 所有特效链结束后，重力才会开始下落填充，避免链式清除晚于重力造成空洞。
        /// </summary>
        private void StartTrackedSpecial(System.Func<System.Collections.IEnumerator> activation)
        {
            activeSpecialChains++;
            StartCoroutine(TrackedSpecialRoutine(activation));
        }

        private System.Collections.IEnumerator TrackedSpecialRoutine(System.Func<System.Collections.IEnumerator> activation)
        {
            // 重要：不能用 yield return StartCoroutine(activation())。
            // Playwork(Luna) WebGL 运行时的协程实现只对 StartCoroutine(直接方法调用) 做特殊处理，
            // 委托调用产生的迭代器不会被注册执行 —— 导致"转换出的特效不立马触发、联动特效不触发"
            // （编辑器正常、Web 端全部失效）。
            // 这里改为手动驱动内部迭代器（MoveNext 泵送），语义与原生嵌套协程等价：
            // 内部 yield 的 WaitForSeconds / Coroutine / WaitUntil / CustomYieldInstruction 等
            // 全部原样转发给 Unity 协程引擎等待。
            System.Collections.IEnumerator inner = activation();
            while (inner.MoveNext())
            {
                yield return inner.Current;
            }
            activeSpecialChains--;
        }

        // ========== 魔力球聚光遮罩实现 ==========

        /// <summary>
        /// 懒创建魔力球聚光遮罩：一张铺满屏幕的半透明黑 Sprite，sortingOrder=25000。
        /// 只盖住棋盘（背景、元素、特效排序都在其下），UI Canvas 排序更高不会被遮挡。
        /// </summary>
        private void EnsureMagicFocusMask()
        {
            if (magicFocusMask != null && magicFocusMask.gameObject != null) return;

            GameObject maskObj = new GameObject("MagicBallFocusMask");
            maskObj.transform.SetParent(transform);
            maskObj.transform.position = new Vector3(0, 0, -1f);

            magicFocusMask = maskObj.AddComponent<SpriteRenderer>();

            Texture2D tex = Texture2D.whiteTexture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            magicFocusMask.sprite = sprite;
            magicFocusMask.color = new Color(0, 0, 0, 0.7f);
            magicFocusMask.sortingOrder = MagicFocusMaskOrder;

            // 放大到足够覆盖整个屏幕（与 GuideManager 遮罩一致）
            float height = Camera.main.orthographicSize * 2f;
            float width = height * Camera.main.aspect;
            float maxDim = Mathf.Max(width, height) * 100f * 2f;
            maskObj.transform.localScale = new Vector3(maxDim, maxDim, 1f);

            maskObj.SetActive(false);
        }

        /// <summary>
        /// 打开魔力球聚光遮罩并把目标物体（魔力球本体/粒子特效）的所有 Renderer 提到遮罩之上。
        /// 支持魔力球链式并发：引用计数累计，最后一次 Hide 才真正关闭遮罩并还原排序。
        /// </summary>
        private void ShowMagicBallFocus(GameObject focusObject)
        {
            if (focusObject == null) return;
            EnsureMagicFocusMask();
            if (magicFocusMask == null) return;

            magicFocusMaskRefCount++;
            magicFocusMask.gameObject.SetActive(true);
            RaiseRenderersAboveFocus(focusObject);
        }

        /// <summary>
        /// 把新生成的粒子特效/拖尾等 Renderer 提到遮罩之上（用于光线发射途中生成的拖尾）。
        /// 同一 Renderer 只记录一次原始排序，避免重复提升导致还原错乱。
        /// </summary>
        private void RaiseRenderersAboveFocus(GameObject go)
        {
            if (go == null) return;
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
                if (raisedFocusRenderers.Contains(r)) continue;
                raisedFocusRenderers.Add(r);
                raisedFocusOriginalOrders.Add(r.sortingOrder);
                r.sortingOrder = MagicFocusRaisedOrder;
            }
        }

        /// <summary>
        /// 关闭魔力球聚光遮罩：引用计数减一，归零时隐藏遮罩并还原所有被提升的排序。
        /// </summary>
        private void HideMagicBallFocus()
        {
            if (magicFocusMaskRefCount > 0)
            {
                magicFocusMaskRefCount--;
                if (magicFocusMaskRefCount > 0) return;
            }

            for (int i = 0; i < raisedFocusRenderers.Count; i++)
            {
                if (raisedFocusRenderers[i] != null)
                {
                    raisedFocusRenderers[i].sortingOrder = raisedFocusOriginalOrders[i];
                }
            }
            raisedFocusRenderers.Clear();
            raisedFocusOriginalOrders.Clear();

            if (magicFocusMask != null && magicFocusMask.gameObject != null)
            {
                magicFocusMask.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 强制立即关闭聚光遮罩（如广告拦截、结算界面出现时兜底清理，防止遮罩盖住 UI）。
        /// </summary>
        public void ClearMagicFocusMask()
        {
            magicFocusMaskRefCount = 0;
            HideMagicBallFocus();
        }

        /// <summary>
        /// 魔力球聚光演出结束（遮罩撤掉）后，在原位置播放收尾粒子特效。
        /// 优先使用 finishEffectPrefab；未配置时回退到 activationEffectPrefab，保证立即可见效果。
        /// 时序要点：调用方在撤遮罩之前创建粒子，粒子留在遮罩之下（sortingOrder<25000），
        /// 遮罩熄灭的同一帧粒子恰好到达首帧喷射 —— 视觉上"聚光灯一灭粒子即爆"，零感知延迟。
        /// 注意：不对 ParticleSystem 使用 Simulate()（会把系统停在手动模拟帧导致不再播放/不显示），
        /// 仅显式 Play() 保证 playOnAwake 关闭时也能启动。
        /// 生成的特效挂在 BoardManager 下，随棋盘一起隐藏（广告/结算时不留残留）。
        /// </summary>
        private void PlayMagicBallFinishEffect(SpecialElementView ball, Vector3 worldPos)
        {
            if (ball == null || ball.Data == null) return;

            GameObject prefab = ball.Data.finishEffectPrefab != null ? ball.Data.finishEffectPrefab : ball.Data.activationEffectPrefab;
            if (prefab == null) return;

            Match3EffectPlayer.Play(prefab, worldPos, transform);
        }

        /// <summary>
        /// 射线到达转换出的特效：等待创建动画播放大部分（弹出缩放 + Spine 创建动画）后
        /// 立即触发特效（创建的动画和触发的特效都会播放）。
        /// 每个转换出的特效独立执行，互不等待。
        /// </summary>
        private System.Collections.IEnumerator ConvertedSpecialTriggerRoutine(SpecialElementView special, float creationDelay)
        {
            if (special == null || special.gameObject == null) yield break;

            // 等待创建动画播放大部分（时长已由调用方按比例缩短并封顶，保节奏）
            yield return new WaitForSeconds(creationDelay);

            if (special == null || special.gameObject == null) yield break;
            // 确认仍占据原格（可能已被其他并发特效处理）
            if (activeElements[special.GridX, special.GridY] == special)
            {
                yield return StartCoroutine(ActivateSpecial(special, null));
            }
        }

        /// <summary>
        /// 等待所有链式特效协程结束（含最长等待上限，防止极端情况软锁）。
        /// </summary>
        private System.Collections.IEnumerator WaitForActiveSpecialChains()
        {
            const float maxWait = 5.0f;
            float elapsed = 0f;
            while (activeSpecialChains > 0 && elapsed < maxWait)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }
            if (activeSpecialChains > 0)
            {
                Debug.LogWarning($"[BoardManager] 等待链式特效超时({maxWait}s)，仍有 {activeSpecialChains} 个特效链未结束，强制执行重力。");
            }
        }

        private System.Collections.IEnumerator ApplyGravityAndRefill()
        {
            // 等待所有链式特效（炸弹波及/火箭穿行/魔力球清色等）彻底结束。
            // 链式特效在飞行过程中会不断清除格子：若重力在其结束前执行，
            // 晚清除的格子将永远保持空洞（无后续下落）。这是"连续触发特效后空格子不下落"的根因。
            yield return StartCoroutine(WaitForActiveSpecialChains());

            // 等待所有长条收集物的收缩动画播放完毕再下落，
            // 防止新方块掉进正在收缩（视觉上还占着旧格子）的蛇身造成重合
            yield return StartCoroutine(WaitForCollectibleRetracts());

            // 等待所有处于销毁动画中的元素（缩小/飞向目标/特殊触发）彻底消失再下落，
            // 覆盖点击特殊触发、传送带、魔力球连击等不经过 ProcessMatchesAndRefill 的路径
            yield return StartCoroutine(WaitForElementDestroys());

            // Gravity and Refill logically
            bool needsRefill = false;
            bool boardChanged = true;
            HashSet<ElementView> movedElements = new HashSet<ElementView>();
            List<ElementView> newlySpawnedElements = new List<ElementView>();
            int[] spawnCountPerColumn = new int[currentLevel.width];

            while (boardChanged)
            {
                boardChanged = false;
                
                // Pass 1: Vertical falls only (absolute priority)
                bool verticalChanged = true;
                while (verticalChanged)
                {
                    verticalChanged = false;
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        for (int x = 0; x < currentLevel.width; x++)
                        {
                            if (!IsValidGridPos(x, y)) continue;
                            
                            // Conveyor cells act as solid obstacles for gravity; nothing falls into them.
                            if (currentLevel.GetCell(x, y).hasConveyor) continue;

                            if (activeElements[x, y] == null)
                            {
                                if (IsValidGridPos(x, y + 1) && activeElements[x, y + 1] != null && CanFall(activeElements[x, y + 1]))
                                {
                                    // Also cannot pull FROM a conveyor cell via gravity!
                                    if (currentLevel.GetCell(x, y + 1).hasConveyor) continue;

                                    ElementView el = activeElements[x, y + 1];
                                    activeElements[x, y] = el;
                                    activeElements[x, y + 1] = null;
                                    if (!newlySpawnedElements.Contains(el)) movedElements.Add(el);
                                    verticalChanged = true;
                                    boardChanged = true;
                                }
                            }
                        }
                    }
                }

                // Pass 2: Diagonal falls
                for (int y = 0; y < currentLevel.height; y++)
                {
                    for (int x = 0; x < currentLevel.width; x++)
                    {
                        if (!IsValidGridPos(x, y)) continue;
                        if (currentLevel.GetCell(x, y).hasConveyor) continue;

                        if (activeElements[x, y] == null)
                        {
                            bool isBlockedFromSky = false;
                            for (int aboveY = y + 1; aboveY < currentLevel.height; aboveY++)
                            {
                                if (!IsValidGridPos(x, aboveY)) { isBlockedFromSky = true; break; }
                                if (currentLevel.GetCell(x, aboveY).hasConveyor) { isBlockedFromSky = true; break; }
                                if (activeElements[x, aboveY] != null)
                                {
                                    if (!CanFall(activeElements[x, aboveY])) { isBlockedFromSky = true; }
                                    else { isBlockedFromSky = false; }
                                    break;
                                }
                            }

                            if (isBlockedFromSky)
                            {
                                // Try diagonal fall
                                // Make sure the path is not blocked by another un-fallable element right above the destination
                                bool canPullLeft = IsValidGridPos(x - 1, y + 1) && !currentLevel.GetCell(x - 1, y + 1).hasConveyor && activeElements[x - 1, y + 1] != null && CanFall(activeElements[x - 1, y + 1]) &&
                                                   (!IsValidGridPos(x - 1, y) || currentLevel.GetCell(x - 1, y).hasConveyor || activeElements[x - 1, y] == null || CanFall(activeElements[x - 1, y]));
                                                   
                                bool canPullRight = IsValidGridPos(x + 1, y + 1) && !currentLevel.GetCell(x + 1, y + 1).hasConveyor && activeElements[x + 1, y + 1] != null && CanFall(activeElements[x + 1, y + 1]) &&
                                                    (!IsValidGridPos(x + 1, y) || currentLevel.GetCell(x + 1, y).hasConveyor || activeElements[x + 1, y] == null || CanFall(activeElements[x + 1, y]));

                                if (canPullLeft && canPullRight)
                                {
                                    if (Random.value > 0.5f)
                                    {
                                        ElementView el = activeElements[x - 1, y + 1];
                                        activeElements[x, y] = el;
                                        activeElements[x - 1, y + 1] = null;
                                        if (!newlySpawnedElements.Contains(el)) movedElements.Add(el);
                                    }
                                    else
                                    {
                                        ElementView el = activeElements[x + 1, y + 1];
                                        activeElements[x, y] = el;
                                        activeElements[x + 1, y + 1] = null;
                                        if (!newlySpawnedElements.Contains(el)) movedElements.Add(el);
                                    }
                                    boardChanged = true;
                                }
                                else if (canPullLeft)
                                {
                                    ElementView el = activeElements[x - 1, y + 1];
                                    activeElements[x, y] = el;
                                    activeElements[x - 1, y + 1] = null;
                                    if (!newlySpawnedElements.Contains(el)) movedElements.Add(el);
                                    boardChanged = true;
                                }
                                else if (canPullRight)
                                {
                                    ElementView el = activeElements[x + 1, y + 1];
                                    activeElements[x, y] = el;
                                    activeElements[x + 1, y + 1] = null;
                                    if (!newlySpawnedElements.Contains(el)) movedElements.Add(el);
                                    boardChanged = true;
                                }
                            }
                        }
                    }
                }

                // Pass 3: Refill at the topmost valid cell only
                for (int x = 0; x < currentLevel.width; x++)
                {
                    // Find the topmost valid cell for this column
                    int topY = -1;
                    for (int y = currentLevel.height - 1; y >= 0; y--)
                    {
                        if (IsValidGridPos(x, y) && !currentLevel.GetCell(x, y).hasConveyor)
                        {
                            topY = y;
                            break;
                        }
                    }

                    // If the topmost cell is empty, spawn a new element
                    if (topY != -1 && activeElements[x, topY] == null)
                    {
                        bool isBlocked = false;
                        for (int y = topY + 1; y < currentLevel.height; y++)
                        {
                            if (IsValidGridPos(x, y) && currentLevel.GetCell(x, y).hasConveyor)
                            {
                                isBlocked = true;
                                break;
                            }
                        }
                        
                        if (!isBlocked)
                        {
                            if (elementDb == null || elementDb.unifiedNormalPrefab == null)
                            {
                                Debug.LogError("Cannot refill board: normal element prefab is missing.");
                                continue;
                            }

                            var normInfo = GetNextRefillElement(x);
                            if (normInfo == null) continue;
                            
                            GameObject newElementObj = Instantiate(elementDb.unifiedNormalPrefab, elementsContainer);
                            var normalView = newElementObj.GetComponent<NormalElementView>();
                            if (normalView == null)
                            {
                                Debug.LogError("Unified normal prefab is missing NormalElementView.");
                                Destroy(newElementObj);
                                continue;
                            }
                            
                            spawnCountPerColumn[x]++;
                            // Keep the original staggered fall distance. The refill mask clips the
                            // portion above the board until the piece enters the visible area.
                            Vector3 spawnPos = GetWorldPosition(x, currentLevel.height + spawnCountPerColumn[x]);
                            normalView.Initialize(normInfo, x, topY, spawnPos, cellSize);
                            HideRefillUntilInsideBoard(normalView);
                            
                            activeElements[x, topY] = normalView;
                            newlySpawnedElements.Add(normalView);
                            
                            boardChanged = true;
                        }
                    }
                }
            }

            // Animate moved existing elements
            foreach (var el in movedElements)
            {
                // Find the new origin of the element (in case of multi-cell elements, we must find the bottom-left most cell it occupies)
                int newX = currentLevel.width;
                int newY = currentLevel.height;
                bool found = false;

                for (int y = 0; y < currentLevel.height; y++)
                {
                    for (int x = 0; x < currentLevel.width; x++)
                    {
                        if (activeElements[x, y] == el)
                        {
                            if (x < newX) newX = x;
                            if (y < newY) newY = y;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    el.MoveTo(newX, newY, GetWorldPosition(newX, newY), 0.3f);
                    needsRefill = true;
                }
            }

            // Animate newly spawned elements
            foreach (var el in newlySpawnedElements)
            {
                int newX = currentLevel.width;
                int newY = currentLevel.height;
                bool found = false;

                for (int y = 0; y < currentLevel.height; y++)
                {
                    for (int x = 0; x < currentLevel.width; x++)
                    {
                        if (activeElements[x, y] == el)
                        {
                            if (x < newX) newX = x;
                            if (y < newY) newY = y;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    el.MoveTo(newX, newY, GetWorldPosition(newX, newY), 0.3f);
                    needsRefill = true;
                }
            }

            if (needsRefill) yield return new WaitForSeconds(0.3f); // Wait for fall animation

            // Recursive check
            List<MatchCluster> newClusters = FindAllMatchClusters();
            if (newClusters.Count > 0)
            {
                yield return StartCoroutine(ProcessMatchesAndRefill(newClusters));
            }
            else
            {
                if (!hasConveyorMovedThisTurn)
                {
                    hasConveyorMovedThisTurn = true;
                    bool moved = false;
                    yield return StartCoroutine(ProcessConveyors(result => moved = result));
                    
                    if (moved)
                    {
                        // After conveyor moves, we might have new matches or empty spaces!
                        // Let gravity and matches run again.
                        yield return StartCoroutine(ApplyGravityAndRefill());
                        yield break; 
                    }
                }

                if (!HasPossibleMoves())
                {
                    yield return StartCoroutine(CheckAndShuffleIfNeeded());
                }
                else
                {
                    currentState = BoardState.Waiting;
                    if (gameManager != null) gameManager.OnBoardSettled();
                }
            }
        }

        private void HideRefillUntilInsideBoard(NormalElementView normalView)
        {
            if (normalView == null || currentLevel == null) return;

            SpriteRenderer[] renderers = normalView.GetComponentsInChildren<SpriteRenderer>(true);
            float visualHalfHeight = cellSize * 0.475f;
            foreach (SpriteRenderer renderer in renderers)
            {
                // SpriteMask is not reliable in the PlayWorks Web runtime. Explicitly
                // remove mask interaction and reveal the piece through script instead.
                renderer.maskInteraction = SpriteMaskInteraction.None;
                if (renderer.bounds.extents.y > 0f)
                {
                    visualHalfHeight = Mathf.Max(visualHalfHeight, renderer.bounds.extents.y);
                }
                renderer.enabled = false;
            }

            float boardTop = boardBottomLeftOrigin.y
                + (currentLevel.height - 1) * (cellSize + cellSpacing)
                + cellSize * 0.5f;
            StartCoroutine(RevealRefillWhenInside(normalView, renderers, boardTop - visualHalfHeight));
        }

        private System.Collections.IEnumerator RevealRefillWhenInside(
            NormalElementView normalView,
            SpriteRenderer[] renderers,
            float revealCenterY)
        {
            while (normalView != null && normalView.transform.position.y > revealCenterY)
            {
                yield return null;
            }

            if (normalView == null) yield break;
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer != null) renderer.enabled = true;
            }
        }

        private System.Collections.IEnumerator ProcessConveyors(System.Action<bool> onComplete)
        {
            // Play spine animations
            var spineAnims = backgroundContainer.GetComponentsInChildren<Spine.Unity.SkeletonAnimation>();
            foreach (var anim in spineAnims)
            {
                anim.timeScale = 1f;
                if (anim.AnimationState != null && anim.AnimationState.GetCurrent(0) != null)
                {
                    anim.AnimationState.GetCurrent(0).TrackTime = 0f;
                }
            }

            // 1. Find all elements on conveyors
            List<ElementView> movingElements = new List<ElementView>();
            Dictionary<ElementView, Vector2Int> targetPositions = new Dictionary<ElementView, Vector2Int>();
            Dictionary<ElementView, bool> canMove = new Dictionary<ElementView, bool>();
            
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    var cell = currentLevel.GetCell(x, y);
                    if (cell != null && cell.isPlayable && cell.hasConveyor)
                    {
                        ElementView el = activeElements[x, y];
                        if (el != null)
                        {
                            // Conveyors cannot move multi-cell objects easily, but let's assume they move them if possible,
                            // or just skip Mother/Child/RetractingCollectible
                            if (el is MotherView || el is ChildView || el is RetractingCollectibleView) continue;
                            
                            Vector2Int targetPos = new Vector2Int(x, y);
                            switch (cell.conveyorDirection)
                            {
                                case Direction.Up: targetPos.y += 1; break;
                                case Direction.Down: targetPos.y -= 1; break;
                                case Direction.Left: targetPos.x -= 1; break;
                                case Direction.Right: targetPos.x += 1; break;
                            }
                            
                            movingElements.Add(el);
                            targetPositions[el] = targetPos;
                            canMove[el] = true; // Initially assume everyone can move
                        }
                    }
                }
            }
            
            if (movingElements.Count == 0)
            {
                foreach (var anim in spineAnims)
                {
                    if (anim != null) anim.timeScale = 0f;
                }
                onComplete?.Invoke(false);
                yield break;
            }
            
            // 2. Resolve blockages
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var el in movingElements)
                {
                    if (!canMove[el]) continue;
                    
                    Vector2Int tPos = targetPositions[el];
                    
                    // Out of bounds
                    if (tPos.x < 0 || tPos.x >= currentLevel.width || tPos.y < 0 || tPos.y >= currentLevel.height)
                    {
                        canMove[el] = false;
                        changed = true;
                        continue;
                    }
                    
                    var targetCell = currentLevel.GetCell(tPos.x, tPos.y);
                    if (targetCell == null || !targetCell.isPlayable)
                    {
                        canMove[el] = false;
                        changed = true;
                        continue;
                    }
                    
                    ElementView elAtTarget = activeElements[tPos.x, tPos.y];
                    if (elAtTarget != null)
                    {
                        // If the element at target is NOT moving, we are blocked
                        if (!movingElements.Contains(elAtTarget))
                        {
                            canMove[el] = false;
                            changed = true;
                        }
                        // If the element at target IS moving, but it is blocked, we are blocked
                        else if (!canMove[elAtTarget])
                        {
                            canMove[el] = false;
                            changed = true;
                        }
                    }
                    
                    // Convergence check (two conveyors pointing to same cell)
                    // If another element targets the same cell and it's already processed and can move, we block this one to prevent overlap.
                    // (Order dependent, but prevents bugs)
                    foreach (var otherEl in movingElements)
                    {
                        if (otherEl != el && canMove[otherEl] && targetPositions[otherEl] == tPos)
                        {
                            // Conflict! One must stop. Let's stop 'el'.
                            // To be fair, could use priority, but this is simple.
                            if (otherEl.GetInstanceID() < el.GetInstanceID()) // Arbitrary consistent tie-breaker
                            {
                                canMove[el] = false;
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            // 3. Execute movement
            bool anyMoved = false;
            
            // First, clear the old positions of all moving elements
            foreach (var el in movingElements)
            {
                if (canMove[el])
                {
                    activeElements[el.GridX, el.GridY] = null;
                }
            }
            
            // Then, place them in new positions and animate
            foreach (var el in movingElements)
            {
                if (canMove[el])
                {
                    Vector2Int tPos = targetPositions[el];
                    activeElements[tPos.x, tPos.y] = el;
                    el.MoveTo(tPos.x, tPos.y, GetWorldPosition(tPos.x, tPos.y), 0.3f);
                    anyMoved = true;
                }
            }
            
            if (anyMoved)
            {
                yield return new WaitForSeconds(0.3f);
            }
            
            // Stop spine animations
            foreach (var anim in spineAnims)
            {
                if (anim != null) anim.timeScale = 0f;
            }
            
            onComplete?.Invoke(anyMoved);
        }

        private System.Collections.IEnumerator ActivateSpecial(SpecialElementView special, ElementView swappedWith, bool isChainReaction = false)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySpecialTriggerSound(special.BehaviorType);

            switch (special.BehaviorType)
            {
                case SpecialBehaviorType.ColorClear:
                    yield return StartCoroutine(ActivateColorClear(special, swappedWith, isChainReaction));
                    break;
                case SpecialBehaviorType.LineClearHorizontal:
                case SpecialBehaviorType.LineClearVertical:
                    yield return StartCoroutine(ActivateLineClear(special));
                    break;
                case SpecialBehaviorType.BombExplosion:
                    yield return StartCoroutine(ActivateBomb(special));
                    break;
                case SpecialBehaviorType.TargetSeeker:
                    yield return StartCoroutine(ActivateTargetSeeker(special));
                    break;
            }
        }

        private void DestroyElementVisual(ElementView el, int targetIndex = -1)
        {
            if (el == null) return;
            
            // 确保如果对象还在 DOTween 动画中，我们先停止它的其它冲突动画（比如 DOPunchScale）
            el.transform.DOKill();
            
            // Spawn clear effect if it's a normal element
            if (el is NormalElementView normal)
            {
                var normInfo = elementDb.GetNormalElement(normal.NormalElementId);
                if (normInfo != null)
                {
                    // 如果光线到达时已经提前播放过消除特效，销毁时不再重复播放
                    if (normInfo.clearEffectPrefab != null && !normal.clearEffectAlreadyPlayed)
                    {
                        SpawnNormalClearEffect(normal);
                    }

                    if (normInfo.lightWaveEffectPrefab != null || elementDb.globalLightWaveEffectPrefab != null)
                    {
                        GameObject prefabToInstantiate = normInfo.lightWaveEffectPrefab != null ? normInfo.lightWaveEffectPrefab : elementDb.globalLightWaveEffectPrefab;
                        Match3EffectPlayer.Play(prefabToInstantiate, el.transform.position,
                            elementsContainer, cellSize * normInfo.lightWaveEffectScale);
                    }
                }
            }

            if (el is SpecialElementView special)
            {
                float duration = special.GetTriggerAnimationDuration();
                // 组合交换动画已播放时跳过 Trigger 动画，直接淡出销毁（交换表演已在 Spine 层播放）
                if (duration > 0f && !special.swapAnimationPlayed)
                {
                    // 登记销毁状态：该元素仅在触发动画播完后才真正销毁
                    pendingElementDestroys.Add(el);
                    special.PlayTriggerAnimation();
                    Destroy(el.gameObject, duration);
                    return;
                }
            }

            if (targetIndex >= 0 && gameManager != null && gameManager.uiManager != null)
            {
                // Fly to UI target
                Vector3 uiPos = gameManager.uiManager.GetTargetWorldPosition(targetIndex);
                if (uiPos != Vector3.zero)
                {
                    // Ensure rendering on top
                    var srList = el.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var sr in srList) sr.sortingOrder = 100;
                    
                    var mrList = el.GetComponentsInChildren<MeshRenderer>(true);
                    foreach (var mr in mrList) mr.sortingOrder = 100;
                    
                    if (el is MotherView)
                    {
                        for (int i = 0; i < el.transform.childCount; i++)
                        {
                            el.transform.GetChild(i).DOLocalMove(Vector3.zero, 0.6f);
                        }
                    }
                    
                    // Bezier curve to UI
                    Vector3 startPos = el.transform.position;
                    Vector3 midPoint = startPos + (uiPos - startPos) / 2f + new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), 0f);
                    
                    Vector3[] path = new Vector3[] { midPoint, uiPos };
                    
                    // 长条收集物完全收集时飞行更短：其身体已播放收缩动画（约0.2s），
                    // 若仍按 0.6s 飞行会让 WaitForElementDestroys 等待过久，造成下落/填充明显卡顿。
                    float flyDuration = el is RetractingCollectibleView ? 0.35f : 0.6f;
                    
                    // 登记销毁状态：元素飞行途中视觉上仍占着原格
                    pendingElementDestroys.Add(el);
                    el.transform.DOPath(path, flyDuration, PathType.CatmullRom)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() => {
                            pendingElementDestroys.Remove(el);
                            if (AudioManager.Instance != null) AudioManager.Instance.PlayCollectSound();
                            if (el != null && el.gameObject != null) Destroy(el.gameObject);
                        }).SetLink(el.gameObject);
                        
                    el.transform.DOScale(el.transform.localScale * 0.5f, flyDuration).SetLink(el.gameObject);
                    return;
                }
            }

            // Animate scale down
            // 登记销毁状态：缩小动画期间该元素仍视觉存在
            pendingElementDestroys.Add(el);
            el.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                pendingElementDestroys.Remove(el);
                if (el != null && el.gameObject != null)
                {
                    Destroy(el.gameObject);
                }
            }).SetLink(el.gameObject);
        }

        /// <summary>
        /// 在普通元素所在位置生成消除特效（粒子缩放与棋盘一致，兼容 Luna/Playworks 渲染）
        /// </summary>
        private void SpawnNormalClearEffect(NormalElementView normal)
        {
            if (normal == null || normal.gameObject == null) return;

            var normInfo = elementDb.GetNormalElement(normal.NormalElementId);
            if (normInfo == null || normInfo.clearEffectPrefab == null) return;

            Match3EffectPlayer.Play(normInfo.clearEffectPrefab, normal.transform.position,
                elementsContainer, cellSize * normInfo.clearEffectScale);
        }

        private void DestroyElements(List<ElementView> elements)
        {
            foreach (var el in elements)
            {
                if (el != null && activeElements[el.GridX, el.GridY] == el)
                {
                    if (el is MotherView)
                    {
                        // 母体元素不能被特效波及直接破坏，必须通过收集齐子物体来消除
                        continue;
                    }

                    if (el is ChildView child)
                    {
                        // 子物体被特效波及，同样触发收集逻辑
                        ProcessChildCollection(child);
                        continue;
                    }

                    // Special logic for long objects hit by blast
                    if (el is RetractingCollectibleView collectible)
                    {
                        // Specials damage long objects only when the effect area directly crosses them.
                        // Deduplicate because a multi-cell long object can appear several times in the area list.
                        TryRetractCollectible(collectible, directlyHitCollectiblesThisTurn);
                        continue; // Skip the standard destruction process
                    }

                    if (el is ObstacleView obstacle)
                    {
                        if (!damagedObstaclesThisTurn.Contains(obstacle))
                        {
                            damagedObstaclesThisTurn.Add(obstacle);
                            obstacle.TakeDamage();

                            if (obstacle.CurrentHealth <= 0)
                            {
                                activeElements[el.GridX, el.GridY] = null;
                                obstacle.PlayCollectEffect();
                                int tIndex = -1;
                                if (gameManager != null) tIndex = gameManager.OnElementCollected(CellItemType.Obstacle, -1, null, null, obstacle.Data);
                                DestroyElementVisual(obstacle, tIndex);
                            }
                        }
                        continue; // Skip the standard destruction process
                    }

                    if (el is GeneratorView generator)
                    {
                        if (!triggeredGeneratorsThisTurn.Contains(generator))
                        {
                            triggeredGeneratorsThisTurn.Add(generator);
                            int tIndex = -1;
                            if (gameManager != null) tIndex = gameManager.OnElementCollected(CellItemType.Generator, -1, null, null, null, generator.Data);
                            
                            Vector3 uiPos = Vector3.zero;
                            if (tIndex >= 0 && gameManager != null && gameManager.uiManager != null)
                            {
                                uiPos = gameManager.uiManager.GetTargetWorldPosition(tIndex);
                            }
                            
                            generator.TriggerGeneration(tIndex, uiPos);
                        }
                        continue; // Skip the standard destruction process
                    }

                    int targetIndex = -1;
                    if (el is SpecialElementView spec)
                    {
                        if (currentlyTriggeringSpecials.Contains(spec)) continue;
                        currentlyTriggeringSpecials.Add(spec);

                        // if special element could be a target
                        if (gameManager != null) targetIndex = gameManager.OnElementCollected(CellItemType.Special, -1, spec.Data);
                        
                        // 链式特效必须被跟踪：主流程末尾的重力会等待所有特效链结束后才执行，
                        // 否则特效晚于重力清除的格子会成为空洞
                        StartTrackedSpecial(() => ActivateSpecial(spec, null, true));
                        CheckAdjacentCollectibles(el.GridX, el.GridY, true);
                        continue; // SKIP setting activeElements to null and DestroyElementVisual! It handles its own destruction.
                    }

                    activeElements[el.GridX, el.GridY] = null;
                    
                    if (el is NormalElementView normal)
                    {
                        if (gameManager != null) targetIndex = gameManager.OnElementCollected(CellItemType.SpecificNormal, normal.NormalElementId);
                    }

                    CheckAdjacentCollectibles(el.GridX, el.GridY, true);
                    DestroyElementVisual(el, targetIndex);
                }
            }
        }

        private System.Collections.IEnumerator ActivateColorClear(SpecialElementView magicBall, ElementView targetElement, bool isChainReaction = false)
        {
            bool shouldWait = isChainReaction && isBoardClearing;
            
            // We delay setting activeElements to null if we need to wait, so gravity doesn't fall through
            if (!shouldWait)
            {
                if (activeElements[magicBall.GridX, magicBall.GridY] == magicBall)
                {
                    activeElements[magicBall.GridX, magicBall.GridY] = null;
                }
            }
            
            magicBall.PlayTriggerAnimation(true); // 循环播放触发动画

            if (shouldWait)
            {
                // 如果是被清屏触发的，等清屏演出结束再清除自己的格子。
                // 注意：不能等 currentState == Waiting —— 主流程的重力现在会等待所有链式特效
                // 结束后才执行，若在这里等 Waiting 会与"重力等特效"互相死锁。
                while (isBoardClearing)
                {
                    yield return null;
                }

                // Now we can set it to null so gravity can process it later
                if (activeElements[magicBall.GridX, magicBall.GridY] == magicBall)
                {
                    activeElements[magicBall.GridX, magicBall.GridY] = null;
                }
            }

            int targetId = -1;
            if (targetElement is NormalElementView normal)
            {
                targetId = normal.NormalElementId;
            }
            else
            {
                // 随机选取一种颜色，但要排除其他并发魔力球已经选择的颜色
                var allNormals = new List<int>();
                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] is NormalElementView n)
                        {
                            if (!allNormals.Contains(n.NormalElementId)) allNormals.Add(n.NormalElementId);
                        }
                    }
                }
                
                var availableNormals = new List<int>(allNormals);
                availableNormals.RemoveAll(c => pickedColorsThisTurn.Contains(c));
                
                if (availableNormals.Count > 0)
                {
                    targetId = availableNormals[Random.Range(0, availableNormals.Count)];
                }
                else if (allNormals.Count > 0)
                {
                    // 如果颜色不够分配，只能复用
                    targetId = allNormals[Random.Range(0, allNormals.Count)];
                }
            }

            if (targetId != -1)
            {
                pickedColorsThisTurn.Add(targetId);

                List<ElementView> toDestroy = new List<ElementView>();
                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] is NormalElementView n && n.NormalElementId == targetId)
                        {
                            toDestroy.Add(n);
                        }
                    }
                }

                GameObject trailPrefab = magicBall.Data?.trailEffectPrefab;
                float flyTime = 0.5f; // Sped up from 0.8f

                // 我们需要保存魔力球最原始的中心坐标，因为在特效发射前，魔力球的视觉元素（Sprite/Spine）
                // 可能会因为播放缩小动画而产生位置偏移，或者因为父节点被清除而丢失精确的起始点
                Vector3 magicBallStartPos = GetWorldPosition(magicBall.GridX, magicBall.GridY);

                // 按照距离从近到远排序，实现依次发射
                toDestroy.Sort((a, b) => {
                    float distA = Vector3.Distance(magicBallStartPos, a.transform.position);
                    float distB = Vector3.Distance(magicBallStartPos, b.transform.position);
                    return distA.CompareTo(distB);
                });

                foreach (var target in toDestroy)
                {
                    if (target == null || target.gameObject == null) continue;

                    if (target is NormalElementView normalTarget)
                    {
                        normalTarget.PlayChangeAnimation();
                    }

                    if (trailPrefab != null)
                    {
                        // 计算目标方向，起点绝对固定为魔力球所在的格子中心
                        Vector3 targetPos = target.transform.position;
                        Vector3 dir = (targetPos - magicBallStartPos).normalized;
                        
                        // 生成拖尾，严格从中心点发射，不加任何偏移，防止起点看起来不在魔力球上
                        GameObject trail = Instantiate(trailPrefab, magicBallStartPos, Quaternion.identity);
                        
                        // 拖尾对齐到目标方向
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        trail.transform.rotation = Quaternion.Euler(0, 0, angle);

                        // 让拖尾绝对直线飞向目标
                        // 修复：Destroy(trail, flyTime) 与 DOMove 同长竞态会吞掉 OnComplete（missing target），
                        // 导致到达时的消除特效不触发。改为 OnComplete 内销毁 + 更长延迟兜底销毁。
                        trail.transform.DOMove(targetPos, flyTime).SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                // 光线到达目标元素后，在该元素处立即生成普通元素消除特效
                                if (target != null && target.gameObject != null && target is NormalElementView hitNormal)
                                {
                                    hitNormal.clearEffectAlreadyPlayed = true;
                                    SpawnNormalClearEffect(hitNormal);
                                }
                                if (trail != null) Destroy(trail);
                            });
                        Destroy(trail, flyTime + 0.5f);
                    }
                    
                    // 按照距离远近依次（间隔不要太久）发射
                    yield return new WaitForSeconds(0.05f);
                }

                // 等待所有射线飞行到达目标（考虑最后一个射线的飞行时间）
                yield return new WaitForSeconds(flyTime);
                
                // 射线到达后，统一消除这些目标方块 (剔除那些在此期间可能已经被其他逻辑销毁的物体)
                List<ElementView> validToDestroy = new List<ElementView>();
                foreach(var target in toDestroy)
                {
                    if (target != null && target.gameObject != null)
                    {
                        validToDestroy.Add(target);
                    }
                }
                DestroyElements(validToDestroy);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }

            // 停止循环并缩小渐变消失
            float fadeOutDuration = 0.3f;

            magicBall.transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack);
            
            if (magicBall.spineAnimation != null && magicBall.spineAnimation.skeleton != null)
            {
                DOTween.To(() => magicBall.spineAnimation.skeleton.A, x => magicBall.spineAnimation.skeleton.A = x, 0f, fadeOutDuration);
            }
            else if (magicBall.spriteRenderer != null)
            {
                magicBall.spriteRenderer.DOFade(0f, fadeOutDuration);
            }

            yield return new WaitForSeconds(fadeOutDuration);

            if (magicBall != null && magicBall.gameObject != null)
            {
                Destroy(magicBall.gameObject);
            }

            // 链式触发的魔力球不再自己调用重力：
            // 所有链式特效统一由发起流程末尾的 ApplyGravityAndRefill 负责
            // （它开头的 WaitForActiveSpecialChains 会等待所有特效链结束），
            // 避免多个重力协程并发执行、以及链式清除晚于重力造成空洞。
        }

        private System.Collections.IEnumerator ActivateLineClear(SpecialElementView special)
        {
            int cx = special.GridX;
            int cy = special.GridY;
            bool isVertical = special.BehaviorType == SpecialBehaviorType.LineClearVertical;
            
            if (activeElements[cx, cy] == special) activeElements[cx, cy] = null;
            
            // 火箭：分为两个一个向上/左，一个向下/右发射（带粒子特效拖尾）离开棋盘并且沿路消除路径上的方块
            GameObject rocket1 = special.gameObject;
            GameObject rocket2 = Instantiate(rocket1, rocket1.transform.parent);
            
            SpecialElementView special2 = rocket2.GetComponent<SpecialElementView>();
            
            special.PlayTriggerAnimation();
            if (special2 != null) special2.PlayTriggerAnimation();

            // Set up directions
            Vector3 moveDir1 = isVertical ? Vector3.up : Vector3.left;
            Vector3 moveDir2 = isVertical ? Vector3.down : Vector3.right;

            // Optional rotation so the spine looks like it's pointing in the direction of movement
            // If the rocket spine defaults to facing "Up" or "Right", we can adjust rotation here.
            // Currently, assuming the visual handles it or you want it left as-is.

            GameObject trailPrefab = special.Data?.trailEffectPrefab;
            if (trailPrefab != null)
            {
                GameObject trail1 = Instantiate(trailPrefab, rocket1.transform.position, Quaternion.identity, rocket1.transform);
                float angle1 = Mathf.Atan2(moveDir1.y, moveDir1.x) * Mathf.Rad2Deg;
                trail1.transform.rotation = Quaternion.Euler(0, 0, angle1);

                GameObject trail2 = Instantiate(trailPrefab, rocket2.transform.position, Quaternion.identity, rocket2.transform);
                float angle2 = Mathf.Atan2(moveDir2.y, moveDir2.x) * Mathf.Rad2Deg;
                trail2.transform.rotation = Quaternion.Euler(0, 0, angle2);
            }
            
            float speed = 15f; 
            float duration = 1.5f;

            rocket1.transform.DOMove(rocket1.transform.position + moveDir1 * speed, duration).SetEase(Ease.Linear);
            rocket2.transform.DOMove(rocket2.transform.position + moveDir2 * speed, duration).SetEase(Ease.Linear);
            
            Destroy(rocket1, duration);
            Destroy(rocket2, duration);

            int maxDist = Mathf.Max(currentLevel.width, currentLevel.height);
            for (int dist = 1; dist <= maxDist; dist++)
            {
                List<ElementView> toDestroy = new List<ElementView>();
                
                int x1 = cx + (isVertical ? 0 : -dist);
                int y1 = cy + (isVertical ? dist : 0);
                if (x1 >= 0 && x1 < currentLevel.width && y1 >= 0 && y1 < currentLevel.height)
                {
                    if (activeElements[x1, y1] != null) toDestroy.Add(activeElements[x1, y1]);
                }

                int x2 = cx + (isVertical ? 0 : dist);
                int y2 = cy + (isVertical ? -dist : 0);
                if (x2 >= 0 && x2 < currentLevel.width && y2 >= 0 && y2 < currentLevel.height)
                {
                    if (activeElements[x2, y2] != null) toDestroy.Add(activeElements[x2, y2]);
                }

                // Also destroy center block at dist 1 (since dist 0 is rocket itself)
                if (dist == 1)
                {
                    if (activeElements[cx, cy] != null) toDestroy.Add(activeElements[cx, cy]);
                }

                if (toDestroy.Count > 0)
                {
                    DestroyElements(toDestroy);
                }
                yield return new WaitForSeconds(1f / speed);
            }
        }

        private System.Collections.IEnumerator ActivateBomb(SpecialElementView special)
        {
            int cx = special.GridX;
            int cy = special.GridY;
            
            if (activeElements[cx, cy] == special) activeElements[cx, cy] = null;
            
            // 炸弹：播放消除效果（粒子特效）
            GameObject fxPrefab = special.Data?.activationEffectPrefab;
            if (fxPrefab != null)
            {
                GameObject fx = Instantiate(fxPrefab, special.transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            DestroyElementVisual(special);

            // A 3x3 area blast
            List<ElementView> toDestroy = new List<ElementView>();
            for (int x = cx - 1; x <= cx + 1; x++)
            {
                for (int y = cy - 1; y <= cy + 1; y++)
                {
                    if (x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height)
                    {
                        if (activeElements[x, y] != null) toDestroy.Add(activeElements[x, y]);
                    }
                }
            }
            
            DestroyElements(toDestroy);
            yield return new WaitForSeconds(0.2f);
        }

        private System.Collections.IEnumerator ActivateTargetSeeker(SpecialElementView special)
        {
            int cx = special.GridX;
            int cy = special.GridY;
            
            if (activeElements[cx, cy] == special) activeElements[cx, cy] = null;
            
            // 飞鸟：播放触发动画
            special.PlayTriggerAnimation();

            // 飞鸟触发时原地爆炸挂载的粒子特效
            GameObject fxPrefab = special.Data?.activationEffectPrefab;
            if (fxPrefab != null)
            {
                GameObject fx = Instantiate(fxPrefab, special.transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            // --- 1. 原地消除以自身为中心的相邻四个格子（上、下、左、右） ---
            List<ElementView> areaToDestroy = new List<ElementView>();
            Vector2Int[] adjacentOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (Vector2Int offset in adjacentOffsets)
            {
                int x = cx + offset.x;
                int y = cy + offset.y;
                if (x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height)
                {
                    if (activeElements[x, y] != null) areaToDestroy.Add(activeElements[x, y]);
                }
            }
            
            if (areaToDestroy.Count > 0)
            {
                DestroyElements(areaToDestroy);
                yield return new WaitForSeconds(0.2f); // wait a bit after explosion
            }

            // --- 2. 挑选目标 ---
            List<ElementView> possibleTargets = new List<ElementView>();
            List<ElementView> collectibleTargets = new List<ElementView>();

            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    var el = activeElements[x, y];
                    if (el != null)
                    {
                        possibleTargets.Add(el);
                        
                        // 检查是否为收集目标
                        if (gameManager != null && gameManager.activeTargets != null)
                        {
                            bool isTarget = false;
                            foreach (var t in gameManager.activeTargets)
                            {
                                if (t.remainingAmount <= 0) continue;

                                if (t.definition.targetType == CellItemType.SpecificNormal && el is NormalElementView norm && t.definition.normalElementId == norm.NormalElementId)
                                {
                                    isTarget = true; break;
                                }
                                else if (t.definition.targetType == CellItemType.RetractingCollectible && el is RetractingCollectibleView retract && t.definition.collectibleData != null && t.definition.collectibleData.collectibleId == retract.collectibleData.collectibleId)
                                {
                                    isTarget = true; break;
                                }
                                else if (t.definition.targetType == CellItemType.Obstacle && el is ObstacleView obs && t.definition.obstacleData != null && t.definition.obstacleData.obstacleId == obs.Data.obstacleId)
                                {
                                    isTarget = true; break;
                                }
                                else if (t.definition.targetType == CellItemType.Generator && el is GeneratorView gen && t.definition.generatorData != null && t.definition.generatorData.generatorId == gen.Data.generatorId)
                                {
                                    isTarget = true; break;
                                }
                                else if (t.definition.targetType == CellItemType.Special && el is SpecialElementView spec && t.definition.specialElement != null && t.definition.specialElement.elementId == spec.Data.elementId)
                                {
                                    isTarget = true; break;
                                }
                                else if (t.definition.targetType == CellItemType.Mother && el is ChildView child && t.definition.motherData != null && t.definition.motherData.motherId == child.Data.targetMotherId)
                                {
                                    isTarget = true; break;
                                }
                            }

                            if (isTarget)
                            {
                                collectibleTargets.Add(el);
                            }
                        }
                    }
                }
            }

            ElementView target = null;
            if (collectibleTargets.Count > 0)
            {
                target = collectibleTargets[Random.Range(0, collectibleTargets.Count)];
            }
            else if (possibleTargets.Count > 0)
            {
                target = possibleTargets[Random.Range(0, possibleTargets.Count)];
            }

            if (target != null)
            {
                float flyTime = 0.5f;
                Vector3 startPos = special.transform.position; // Now use its current position after any explosion shift
                
                // Set up trail if configured
                GameObject trailPrefab = special.Data?.trailEffectPrefab;
                if (trailPrefab != null)
                {
                    Vector3 dir = (target.transform.position - startPos).normalized;
                    
                    GameObject trail = Instantiate(trailPrefab, startPos, Quaternion.identity, special.transform);
                    
                    // Rotate trail to face movement
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    trail.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                special.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.Linear);
                
                yield return new WaitForSeconds(flyTime);
                
                // Check if target is still active before destroying it
                if (target != null && target.gameObject != null)
                {
                    if (target is RetractingCollectibleView collectible)
                    {
                        // The bird directly hit the long object. This is direct special damage,
                        // not adjacency damage from another cleared cell.
                        TryRetractCollectible(collectible, directlyHitCollectiblesThisTurn);
                    }
                    else
                    {
                        DestroyElements(new List<ElementView> { target });
                    }
                }
                
                // 到达目的地消除元素时缩小淡出
                special.transform.DOScale(Vector3.zero, 0.3f);
                var skeletonAnim = special.GetComponentInChildren<Spine.Unity.SkeletonAnimation>();
                if (skeletonAnim != null && skeletonAnim.skeleton != null)
                {
                    DG.Tweening.DOTween.To(() => skeletonAnim.skeleton.A, x => skeletonAnim.skeleton.A = x, 0f, 0.3f);
                }
                
                Destroy(special.gameObject, 0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                
                special.transform.DOScale(Vector3.zero, 0.3f);
                var skeletonAnim = special.GetComponentInChildren<Spine.Unity.SkeletonAnimation>();
                if (skeletonAnim != null && skeletonAnim.skeleton != null)
                {
                    DG.Tweening.DOTween.To(() => skeletonAnim.skeleton.A, x => skeletonAnim.skeleton.A = x, 0f, 0.3f);
                }
                
                Destroy(special.gameObject, 0.3f);
            }
        }

        private List<SpecialElementView> newSpecialsThisTurn = new List<SpecialElementView>();
        private HashSet<RetractingCollectibleView> retractedThisTurn = new HashSet<RetractingCollectibleView>();
        private HashSet<RetractingCollectibleView> directlyHitCollectiblesThisTurn = new HashSet<RetractingCollectibleView>();
        private HashSet<ObstacleView> damagedObstaclesThisTurn = new HashSet<ObstacleView>();
        private HashSet<GeneratorView> triggeredGeneratorsThisTurn = new HashSet<GeneratorView>();
        private HashSet<ChildView> triggeredChildrenThisTurn = new HashSet<ChildView>();

        /// <summary>
        /// 正在播放销毁动画（缩小/飞行/触发）的元素。
        /// 生成特殊元素前需等待其全部消失，避免新元素与尚未清理干净的旧元素重合。
        /// </summary>
        private readonly HashSet<ElementView> pendingElementDestroys = new HashSet<ElementView>();

        private void TryRetractCollectible(
            RetractingCollectibleView collectible,
            HashSet<RetractingCollectibleView> hitSet)
        {
            if (collectible == null || collectible.CurrentLength <= 0 || hitSet.Contains(collectible)) return;
            hitSet.Add(collectible);

            if (AudioManager.Instance != null) AudioManager.Instance.PlayLongObjectBackSound();

            Vector2Int dirVec = Vector2Int.zero;
            switch (collectible.CurrentDirection)
            {
                case Direction.Up: dirVec = Vector2Int.up; break;
                case Direction.Down: dirVec = Vector2Int.down; break;
                case Direction.Left: dirVec = Vector2Int.left; break;
                case Direction.Right: dirVec = Vector2Int.right; break;
            }

            // Clear every old occupied cell before changing the logical length.
            for (int cx = 0; cx < currentLevel.width; cx++)
            {
                for (int cy = 0; cy < currentLevel.height; cy++)
                {
                    if (activeElements[cx, cy] == collectible) activeElements[cx, cy] = null;
                }
            }

            collectible.RetractOneCell();

            if (collectible.CurrentLength > 0)
            {
                int tailX = collectible.GridX;
                int tailY = collectible.GridY;
                for (int i = 0; i < collectible.CurrentLength; i++)
                {
                    int bx = tailX + dirVec.x * i;
                    int by = tailY + dirVec.y * i;
                    if (bx >= 0 && bx < currentLevel.width && by >= 0 && by < currentLevel.height)
                    {
                        activeElements[bx, by] = collectible;
                    }
                }
            }
            else
            {
                int targetIndex = -1;
                if (gameManager != null)
                {
                    targetIndex = gameManager.OnElementCollected(
                        CellItemType.RetractingCollectible, -1, null, collectible.collectibleData);
                }
                DestroyElementVisual(collectible, targetIndex);
            }
        }

        private void CheckAdjacentCollectibles(int x, int y, bool isFromSpecial = false)
        {
            // Simple adjacent check
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var dir in dirs)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                if (nx >= 0 && nx < currentLevel.width && ny >= 0 && ny < currentLevel.height)
                {
                    if (activeElements[nx, ny] is RetractingCollectibleView collectible)
                    {
                        // A normal match can retract an adjacent long object. A cell cleared by
                        // a special cannot: specials only damage a long object when their actual
                        // effect area directly covers one of its occupied cells.
                        if (isFromSpecial) continue;
                        TryRetractCollectible(collectible, retractedThisTurn);
                    }
                    else if (activeElements[nx, ny] is ObstacleView obstacle)
                    {
                        if (obstacle.Data != null && obstacle.Data.onlyDamagedBySpecials && !isFromSpecial) continue;

                        if (damagedObstaclesThisTurn.Contains(obstacle)) continue;
                        damagedObstaclesThisTurn.Add(obstacle);

                        obstacle.TakeDamage();

                        if (obstacle.CurrentHealth <= 0)
                        {
                            activeElements[nx, ny] = null;
                            obstacle.PlayCollectEffect();
                            int targetIndex = -1;
                            if (gameManager != null) targetIndex = gameManager.OnElementCollected(CellItemType.Obstacle, -1, null, null, obstacle.Data);
                            DestroyElementVisual(obstacle, targetIndex);
                        }
                    }
                     else if (activeElements[nx, ny] is GeneratorView generator)
                     {
                         if (triggeredGeneratorsThisTurn.Contains(generator)) continue;
                         triggeredGeneratorsThisTurn.Add(generator);

                         bool isReadyToGenerate = generator.AdvanceState();
                         if (isReadyToGenerate)
                         {
                             int targetIndex = -1;
                             if (gameManager != null) targetIndex = gameManager.OnElementCollected(CellItemType.Generator, -1, null, null, null, generator.Data);
                             
                             Vector3 uiPos = Vector3.zero;
                             if (targetIndex >= 0 && gameManager != null && gameManager.uiManager != null)
                             {
                                 uiPos = gameManager.uiManager.GetTargetWorldPosition(targetIndex);
                             }
                             
                             generator.TriggerGeneration(targetIndex, uiPos);
                         }
                     }
                    else if (activeElements[nx, ny] is ChildView child)
                    {
                        ProcessChildCollection(child);
                    }
                 }
             }
        }

        private void ProcessChildCollection(ChildView child)
        {
            if (triggeredChildrenThisTurn.Contains(child)) return;
            triggeredChildrenThisTurn.Add(child);

            // 如果此时还在数组里，就清空
            if (activeElements[child.GridX, child.GridY] == child)
            {
                activeElements[child.GridX, child.GridY] = null;
            }
            
            child.PlayCollectEffect(); // 播放子物体消除特效
            
            MotherView targetMother = null;
            for (int cx = 0; cx < currentLevel.width; cx++)
            {
                for (int cy = 0; cy < currentLevel.height; cy++)
                {
                    if (activeElements[cx, cy] is MotherView mother && mother.Data.motherId == child.Data.targetMotherId && (mother.CurrentCount - mother.PendingReceiveCount) > 0)
                    {
                        targetMother = mother;
                        break;
                    }
                }
                if (targetMother != null) break;
            }

            if (targetMother != null)
            {
                // 预先登记一个在途的子物体，防止其他子物体错误地超额飞向它
                targetMother.PendingReceiveCount++;

                if (AudioManager.Instance != null && child.Data.flySound != null)
                {
                    AudioManager.Instance.PlayContinuousSound(child.Data.flySound);
                }

                var sr = child.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 100;
                var mr = child.GetComponentInChildren<MeshRenderer>();
                if (mr != null) mr.sortingOrder = 100;

                Vector3 startPos = child.transform.position;
                // 飞向母体的视觉中心（通常是第一个子节点的位置）
                Vector3 targetPos = targetMother.transform.position;
                if (targetMother.transform.childCount > 0)
                {
                    targetPos = targetMother.transform.GetChild(0).position;
                }
                
                // 减小弧度，仅在直线垂直方向上做一个很小的偏移
                Vector3 dir = targetPos - startPos;
                Vector3 perp = new Vector3(-dir.y, dir.x, 0).normalized;
                float curveAmount = Random.Range(-0.5f, 0.5f);
                Vector3 midPoint = startPos + dir / 2f + perp * curveAmount;
                
                Vector3[] path = new Vector3[] { midPoint, targetPos };
                
                bool childDestroyed = false;
                child.transform.DOPath(path, 0.5f, PathType.CatmullRom)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => {
                        try 
                        {
                            // 飞行到达，解除在途计数
                            if (targetMother != null) targetMother.PendingReceiveCount--;
                            
                            targetMother.ReceiveChild(() => {
                                int tIndex = -1;
                                if (gameManager != null) tIndex = gameManager.OnElementCollected(CellItemType.Mother, -1, null, null, null, null, targetMother.Data);
                                
                                // 收集完毕，从棋盘彻底清除母体
                                for (int mx = 0; mx < currentLevel.width; mx++)
                                {
                                    for (int my = 0; my < currentLevel.height; my++)
                                    {
                                        if (activeElements[mx, my] == targetMother)
                                        {
                                            activeElements[mx, my] = null;
                                        }
                                    }
                                }
                                
                                DestroyElementVisual(targetMother, tIndex);
                                pendingRefill = true;
                            });
                        }
                        finally
                        {
                            // Destroy child immediately after it reaches the mother
                            if (!childDestroyed && child != null && child.gameObject != null) 
                            {
                                childDestroyed = true;
                                Destroy(child.gameObject);
                            }
                        }
                    });
                    
                child.transform.DOScale(child.transform.localScale * 0.5f, 0.5f);
            }
            else
            {
                int tIndex = -1;
                if (gameManager != null) tIndex = gameManager.OnElementCollected(CellItemType.Child, -1, null, null, null, null, null, child.Data);
                DestroyElementVisual(child, tIndex);
            }
        }

        /// <summary>
        /// Generates the board based on LevelData and ElementDatabase.
        /// Should be called by the GameManager.
        /// </summary>
        public void InitializeBoard(LevelData levelData, ElementDatabase database, RectTransform uiBoardArea)
        {
            currentLevel = levelData;
            elementDb = database;
            activeElements = new ElementView[currentLevel.width, currentLevel.height];
            refillSequenceIndices = new int[currentLevel.width];
            refillWarnings.Clear();
            pendingRefill = false;
            // 重置特效链状态，防止旧局的链式协程/清屏标志污染新局
            activeSpecialChains = 0;
            isBoardClearing = false;
            currentlyTriggeringSpecials.Clear();

            // Clear any existing board
            ClearContainer(backgroundContainer);
            ClearContainer(elementsContainer);

            CalculateBoardOrigin(uiBoardArea);
            GenerateGrid();
            
            PlayIntroAnimation();
            StartCoroutine(InitialBoardCheck());
        }

        private System.Collections.IEnumerator InitialBoardCheck()
        {
            yield return new WaitForSeconds(0.8f); // Wait for intro animation to finish
            if (!HasPossibleMoves())
            {
                yield return StartCoroutine(CheckAndShuffleIfNeeded());
            }
        }

        private void PlayIntroAnimation()
        {
            float screenWorldWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
            
            // Animate background container
            if (backgroundContainer != null)
            {
                backgroundContainer.DOKill();
                // We use LocalPosition to avoid any absolute world position conflicts
                backgroundContainer.localPosition = new Vector3(screenWorldWidth, 0, 0);
                backgroundContainer.DOLocalMoveX(0, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
            }

            // Animate elements container
            if (elementsContainer != null)
            {
                elementsContainer.DOKill();
                // We use LocalPosition to avoid any absolute world position conflicts
                elementsContainer.localPosition = new Vector3(screenWorldWidth, 0, 0);
                elementsContainer.DOLocalMoveX(0, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
            }
        }

        public void UpdateBoardLayout(RectTransform uiBoardArea)
        {
            if (currentLevel == null || activeElements == null) return;

            // If a layout update happens during the intro animation (e.g. screen resize),
            // we must snap the containers to their final positions to prevent offset calculation errors.
            if (backgroundContainer != null)
            {
                backgroundContainer.DOKill();
                backgroundContainer.localPosition = Vector3.zero;
            }
            if (elementsContainer != null)
            {
                elementsContainer.DOKill();
                elementsContainer.localPosition = Vector3.zero;
            }

            CalculateBoardOrigin(uiBoardArea);

            UpdateGlobalBackground();

            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    if (currentLevel.GetCell(x, y) == null || !currentLevel.GetCell(x, y).isPlayable) continue;

                    Vector3 worldPos = GetWorldPosition(x, y);

                    // Update background tile
                    Transform bgTile = backgroundContainer.Find($"Tile_{x}_{y}");
                    if (bgTile != null)
                    {
                        bgTile.position = worldPos;
                        SpriteRenderer bgSr = bgTile.GetComponent<SpriteRenderer>();
                        if (bgSr != null && bgSr.sprite != null && bgSr.sprite != cachedDefaultBgSprite)
                        {
                            float bgWidth = bgSr.sprite.bounds.size.x;
                            float bgHeight = bgSr.sprite.bounds.size.y;
                            float scaleX = cellSize / bgWidth;
                            float scaleY = cellSize / bgHeight;
                            bgTile.localScale = new Vector3(scaleX, scaleY, 1f);
                        }
                    }

                    // Update conveyor
                    Transform conveyorObj = backgroundContainer.Find($"Conveyor_{x}_{y}");
                    if (conveyorObj != null)
                    {
                        conveyorObj.position = worldPos;
                        SpriteRenderer bgSr = conveyorObj.GetComponent<SpriteRenderer>();
                        if (bgSr != null && bgSr.sprite != null)
                        {
                            float bgWidth = bgSr.sprite.bounds.size.x;
                            float bgHeight = bgSr.sprite.bounds.size.y;
                            float targetSize = cellSize + cellSpacing;
                            float scaleX = targetSize / bgWidth;
                            float scaleY = targetSize / bgHeight;
                            
                            // Preserve any flip that was set during generation
                            Vector3 currentScale = conveyorObj.localScale;
                            scaleX = Mathf.Abs(scaleX) * Mathf.Sign(currentScale.x);
                            scaleY = Mathf.Abs(scaleY) * Mathf.Sign(currentScale.y);
                            
                            conveyorObj.localScale = new Vector3(scaleX, scaleY, 1f);
                        }
                    }

                    // Update active element
                    if (activeElements[x, y] != null)
                    {
                        ElementView el = activeElements[x, y];
                        
                        // Process each element once based on its head position
                        if (el.GridX == x && el.GridY == y) 
                        {
                            el.SetPosition(x, y, worldPos);
                            
                            if (el is RetractingCollectibleView collectView)
                            {
                                collectView.UpdateCellSize(cellSize, cellSpacing);
                            }
                            else
                            {
                                if (el.spriteRenderer != null && el.spriteRenderer.sprite != null)
                                {
                                    el.UpdateVisuals(el.spriteRenderer.sprite, cellSize);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the bottom-left origin of the board in world space so that 
        /// the entire grid is centered in the designated UI area.
        /// </summary>
        private void CalculateBoardOrigin(RectTransform uiBoardArea)
        {
            if (originalCellSize < 0)
            {
                originalCellSize = cellSize;
                originalCellSpacing = cellSpacing;
            }
            
            // Reset to original before calculating
            cellSize = originalCellSize;
            cellSpacing = originalCellSpacing;

            Vector3 centerPosition = transform.position;
            
            if (uiBoardArea != null)
            {
                // Force Canvas update to ensure RectTransform has correct dimensions before we calculate
                Canvas.ForceUpdateCanvases();

                Vector3[] corners = new Vector3[4];
                uiBoardArea.GetWorldCorners(corners);
                
                Canvas canvas = uiBoardArea.GetComponentInParent<Canvas>();
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
                    if (cam != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            corners[i] = cam.ScreenToWorldPoint(corners[i]);
                            corners[i].z = 0;
                        }
                    }
                }

                centerPosition = (corners[0] + corners[2]) / 2f;
                centerPosition.z = 0; 
                
                float uiWorldWidth = Vector3.Distance(corners[0], corners[3]);
                float uiWorldHeight = Vector3.Distance(corners[0], corners[1]);
                
                float unscaledWidth = currentLevel.width * cellSize + (currentLevel.width - 1) * cellSpacing;
                float unscaledHeight = currentLevel.height * cellSize + (currentLevel.height - 1) * cellSpacing;
                
                if (uiWorldWidth > 0 && uiWorldHeight > 0)
                {
                    float scaleX = uiWorldWidth / unscaledWidth;
                    float scaleY = uiWorldHeight / unscaledHeight;
                    float scale = Mathf.Min(scaleX, scaleY);
                    
                    // Apply a padding so it doesn't touch the very edge
                    scale *= (1f - boardEdgePadding);
                    
                    cellSize *= scale;
                    cellSpacing *= scale;
                }
            }

            // Calculate total width and height of the board in world space with scaled size
            float totalWidth = currentLevel.width * cellSize + (currentLevel.width - 1) * cellSpacing;
            float totalHeight = currentLevel.height * cellSize + (currentLevel.height - 1) * cellSpacing;

            // Calculate bottom left corner
            boardBottomLeftOrigin = centerPosition - new Vector3(totalWidth / 2f, totalHeight / 2f, 0f);
            
            // Adjust so that cell (0,0) is exactly placed at the center of its slot
            boardBottomLeftOrigin += new Vector3(cellSize / 2f, cellSize / 2f, 0f);
        }

        private GameObject globalBgTile;

        /// <summary>
        /// Iterates through the LevelData grid and instantiates background tiles and initial elements.
        /// </summary>
        private void GenerateGrid()
        {
            // Clear lists to prevent stale references across level regenerations
            newSpecialsThisTurn.Clear();
            retractedThisTurn.Clear();
            directlyHitCollectiblesThisTurn.Clear();
            damagedObstaclesThisTurn.Clear();
            triggeredGeneratorsThisTurn.Clear();

            bool useGlobalBg = false;
            Texture2D globalBgTex = null;
            if (Manager.ConfigManager.Instance != null && Manager.ConfigManager.Instance.useCustomGridBackground && Manager.ConfigManager.Instance.GridBackgroundPic != null)
            {
                useGlobalBg = true;
                globalBgTex = Manager.ConfigManager.Instance.GridBackgroundPic;
            }

            if (useGlobalBg && globalBgTex != null)
            {
                // Create one large background for the entire grid
                globalBgTile = new GameObject("GlobalBoardBackground");
                globalBgTile.transform.SetParent(backgroundContainer);
                
                SpriteRenderer bgSr = globalBgTile.AddComponent<SpriteRenderer>();
                Sprite bgSprite = Sprite.Create(globalBgTex, new Rect(0, 0, globalBgTex.width, globalBgTex.height), new Vector2(0.5f, 0.5f), 100f);
                bgSr.sprite = bgSprite;
                bgSr.sortingOrder = -10; // ensure it stays behind elements

                UpdateGlobalBackground();
            }

            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    GridCellData cellData = currentLevel.GetCell(x, y);

                    // Skip unplayable holes
                    if (cellData == null || !cellData.isPlayable) continue;

                    Vector3 worldPos = GetWorldPosition(x, y);

                    // 1. Instantiate Background Tile (only if not using global bg)
                    if (!useGlobalBg)
                    {
                        if (backgroundTilePrefab != null)
                        {
                            GameObject bgTile = Instantiate(backgroundTilePrefab, worldPos, Quaternion.identity, backgroundContainer);
                            bgTile.name = $"Tile_{x}_{y}";
                            
                            // Automatically scale background tile to fit cell size
                            SpriteRenderer bgSr = bgTile.GetComponent<SpriteRenderer>();
                            if (bgSr != null && bgSr.sprite != null)
                            {
                                float bgWidth = bgSr.sprite.bounds.size.x;
                                float bgHeight = bgSr.sprite.bounds.size.y;
                                float scaleX = cellSize / bgWidth;
                                float scaleY = cellSize / bgHeight;
                                bgTile.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                            }
                        }
                        else
                        {
                            // Auto-generate a default background tile if none is provided
                            GameObject bgTile = new GameObject($"Tile_{x}_{y}");
                            bgTile.transform.SetParent(backgroundContainer);
                            bgTile.transform.position = worldPos;
                            
                            SpriteRenderer bgSr = bgTile.AddComponent<SpriteRenderer>();
                            
                            if (cachedDefaultBgSprite == null)
                            {
                                Texture2D tex = Texture2D.whiteTexture;
                                cachedDefaultBgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                            }
                            
                            bgSr.sprite = cachedDefaultBgSprite;
                            bgSr.color = new Color(0f, 0f, 0f, 0.3f); // Semi-transparent black
                            
                            // Scale it up to cell size
                            float bgWidth = bgSr.sprite.bounds.size.x;
                            float bgHeight = bgSr.sprite.bounds.size.y;
                            float scaleX = cellSize / bgWidth;
                            float scaleY = cellSize / bgHeight;
                            bgTile.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                        }
                    }

                    // 1.5 Instantiate Conveyor Background
                    if (cellData.hasConveyor)
                    {
                        // Calculate if it's a corner based on neighbors
                        bool isCorner = false;
                        Direction inDir = cellData.conveyorDirection;
                        
                        // To find where the conveyor is coming from, we look at neighbors pointing TO this cell
                        if (IsValidGridPos(x - 1, y) && currentLevel.GetCell(x - 1, y).hasConveyor && currentLevel.GetCell(x - 1, y).conveyorDirection == Direction.Right)
                            inDir = Direction.Right;
                        else if (IsValidGridPos(x + 1, y) && currentLevel.GetCell(x + 1, y).hasConveyor && currentLevel.GetCell(x + 1, y).conveyorDirection == Direction.Left)
                            inDir = Direction.Left;
                        else if (IsValidGridPos(x, y - 1) && currentLevel.GetCell(x, y - 1).hasConveyor && currentLevel.GetCell(x, y - 1).conveyorDirection == Direction.Up)
                            inDir = Direction.Up;
                        else if (IsValidGridPos(x, y + 1) && currentLevel.GetCell(x, y + 1).hasConveyor && currentLevel.GetCell(x, y + 1).conveyorDirection == Direction.Down)
                            inDir = Direction.Down;

                        if (inDir != cellData.conveyorDirection)
                        {
                            isCorner = true;
                        }
                        
                        // Instantiate the correct Prefab
                        string prefabName = isCorner ? "Prefabs/ConveyorCorner" : "Prefabs/ConveyorStraight";
                        GameObject prefab = Resources.Load<GameObject>(prefabName);
                        if (prefab != null)
                        {
                            GameObject conveyorObj = Instantiate(prefab, worldPos, Quaternion.identity, backgroundContainer);
                            conveyorObj.name = $"Conveyor_{x}_{y}";
                            
                            // Scale to cell size + spacing to make it continuous
                            SpriteRenderer bgSr = conveyorObj.GetComponent<SpriteRenderer>();
                            if (bgSr != null && bgSr.sprite != null)
                            {
                                float bgWidth = bgSr.sprite.bounds.size.x;
                                float bgHeight = bgSr.sprite.bounds.size.y;
                                float targetSize = cellSize + cellSpacing;
                                float scaleX = targetSize / bgWidth;
                                float scaleY = targetSize / bgHeight;
                                conveyorObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                            }
                            
                            // Calculate rotation based on out direction
                            float angle = 0f;
                            switch (cellData.conveyorDirection)
                            {
                                case Direction.Right: angle = 0f; break;
                                case Direction.Up: angle = 90f; break;
                                case Direction.Left: angle = 180f; break;
                                case Direction.Down: angle = 270f; break;
                            }
                            
                            // If it's a corner, we might need to adjust rotation or flip based on inDir
                            if (isCorner)
                            {
                                // Find if there's a custom config for this specific corner
                                ConveyorCornerConfig config = cornerConfigs.Find(c => c.inDir == inDir && c.outDir == cellData.conveyorDirection);
                                if (config != null)
                                {
                                    angle = config.rotationAngle;
                                    conveyorObj.transform.localScale = new Vector3(
                                        conveyorObj.transform.localScale.x * (config.flipX ? -1f : 1f),
                                        conveyorObj.transform.localScale.y * (config.flipY ? -1f : 1f),
                                        1f
                                    );
                                    
                                    // Update the animation name on the Spine component
                                    var arrow = conveyorObj.transform.Find("Arrow");
                                    if (arrow != null)
                                    {
                                        var anim = arrow.GetComponent<Spine.Unity.SkeletonAnimation>();
                                        if (anim != null && !string.IsNullOrEmpty(config.animationName))
                                        {
                                            anim.AnimationName = config.animationName;
                                            anim.Initialize(true);
                                        }
                                    }
                                }
                                else
                                {
                                    bool isRightTurn = false;
                                    if (inDir == Direction.Up && cellData.conveyorDirection == Direction.Right) isRightTurn = true;
                                    if (inDir == Direction.Right && cellData.conveyorDirection == Direction.Down) isRightTurn = true;
                                    if (inDir == Direction.Down && cellData.conveyorDirection == Direction.Left) isRightTurn = true;
                                    if (inDir == Direction.Left && cellData.conveyorDirection == Direction.Up) isRightTurn = true;
                                    
                                    if (!isRightTurn)
                                    {
                                        // Flip it horizontally if it's a left turn
                                        conveyorObj.transform.localScale = new Vector3(-conveyorObj.transform.localScale.x, conveyorObj.transform.localScale.y, 1f);
                                        
                                        var arrow = conveyorObj.transform.Find("Arrow");
                                        if (arrow != null)
                                        {
                                            var anim = arrow.GetComponent<Spine.Unity.SkeletonAnimation>();
                                            if (anim != null)
                                            {
                                                anim.AnimationName = "line_lu"; // Default left turn anim
                                                anim.Initialize(true);
                                            }
                                        }
                                    }
                                }
                            }

                            conveyorObj.transform.rotation = Quaternion.Euler(0, 0, angle);
                        }
                    }

                    // 2. Instantiate Initial Elements
                    SpawnElement(cellData, x, y, worldPos);
                }
            }
        }

        private void UpdateGlobalBackground()
        {
            if (globalBgTile == null) return;

            SpriteRenderer bgSr = globalBgTile.GetComponent<SpriteRenderer>();
            if (bgSr != null && bgSr.sprite != null)
            {
                // The bounds of the entire grid
                float totalWidth = currentLevel.width * cellSize + (currentLevel.width - 1) * cellSpacing;
                float totalHeight = currentLevel.height * cellSize + (currentLevel.height - 1) * cellSpacing;

                // Center of the grid
                Vector3 centerPosition = boardBottomLeftOrigin + new Vector3(totalWidth / 2f - cellSize / 2f, totalHeight / 2f - cellSize / 2f, 0f);
                globalBgTile.transform.position = centerPosition;

                float bgWidth = bgSr.sprite.bounds.size.x;
                float bgHeight = bgSr.sprite.bounds.size.y;

                // padding for global background (optional, here we add a bit of padding)
                float padding = cellSize * 0.5f; 
                float targetWidth = totalWidth + padding;
                float targetHeight = totalHeight + padding;

                float scaleX = targetWidth / bgWidth;
                float scaleY = targetHeight / bgHeight;
                float scale = Mathf.Max(scaleX, scaleY);

                globalBgTile.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        private void SpawnElement(GridCellData cellData, int x, int y, Vector3 worldPos)
        {
            if (cellData.itemType == CellItemType.None) return;
            
            // Prevent spawning if the cell is already occupied (e.g. by a long object's body generated earlier)
            if (activeElements[x, y] != null) return;

            GameObject newElementObj = null;
            ElementView elementView = null;

            // Handle Normal Elements
            if (cellData.itemType == CellItemType.RandomNormal || cellData.itemType == CellItemType.SpecificNormal)
            {
                int normalId = cellData.normalElementId;

                // If Random, pick a random ID from the database
                if (cellData.itemType == CellItemType.RandomNormal)
                {
                    int randIndex = Random.Range(0, elementDb.normalElements.Count);
                    normalId = elementDb.normalElements[randIndex].id;
                }

                NormalElementInfo normInfo = elementDb.GetNormalElement(normalId);
                if (normInfo != null && elementDb.unifiedNormalPrefab != null)
                {
                    newElementObj = Instantiate(elementDb.unifiedNormalPrefab, elementsContainer);
                    var normalView = newElementObj.GetComponent<NormalElementView>();
                    if (normalView != null)
                    {
                        normalView.Initialize(normInfo, x, y, worldPos, cellSize);
                        elementView = normalView;
                    }
                }
            }
            // Handle Special Elements
            else if (cellData.itemType == CellItemType.Special && cellData.specialElement != null)
            {
                if (cellData.specialElement.prefab != null)
                {
                    newElementObj = Instantiate(cellData.specialElement.prefab, elementsContainer);
                    var specialView = newElementObj.GetComponent<SpecialElementView>();
                    if (specialView != null)
                    {
                        specialView.Initialize(cellData.specialElement, x, y, worldPos, cellSize);
                        elementView = specialView;
                    }
                }
            }
            // Handle Obstacle Elements
            else if (cellData.itemType == CellItemType.Obstacle && cellData.obstacleData != null)
            {
                if (cellData.obstacleData.prefab != null)
                {
                    newElementObj = Instantiate(cellData.obstacleData.prefab, elementsContainer);
                    var obstacleView = newElementObj.GetComponent<ObstacleView>();
                    if (obstacleView != null)
                    {
                        obstacleView.Initialize(cellData.obstacleData, x, y, worldPos, cellSize);
                        elementView = obstacleView;
                    }
                }
            }
            // Handle Generator Elements
            else if (cellData.itemType == CellItemType.Generator && cellData.generatorData != null)
            {
                if (cellData.generatorData.prefab != null)
                {
                    newElementObj = Instantiate(cellData.generatorData.prefab, elementsContainer);
                    var generatorView = newElementObj.GetComponent<GeneratorView>();
                    if (generatorView != null)
                    {
                        generatorView.Initialize(cellData.generatorData, x, y, worldPos, cellSize);
                        elementView = generatorView;
                    }
                }
            }
            // Handle Collectible Elements (Snake/Train)
            else if (cellData.itemType == CellItemType.RetractingCollectible && cellData.collectibleData != null)
            {
                if (cellData.collectibleData.prefab != null)
                {
                    newElementObj = Instantiate(cellData.collectibleData.prefab, elementsContainer);
                    var collectView = newElementObj.GetComponent<RetractingCollectibleView>();
                    if (collectView != null)
                    {
                        // x, y is the Tail. Calculate Head.
                        int headX = x;
                        int headY = y;
                        switch (cellData.collectibleDirection)
                        {
                            case Direction.Up: headY += cellData.collectibleLength - 1; break;
                            case Direction.Down: headY -= cellData.collectibleLength - 1; break;
                            case Direction.Right: headX += cellData.collectibleLength - 1; break;
                            case Direction.Left: headX -= cellData.collectibleLength - 1; break;
                        }
                        
                        collectView.Initialize(cellData.collectibleData, x, y, headX, headY, worldPos, cellSize, cellSpacing);
                        elementView = collectView;
                    }
                }
            }
            // Handle Mother Elements
            else if (cellData.itemType == CellItemType.Mother && cellData.motherData != null)
            {
                if (cellData.motherData.prefab != null)
                {
                    newElementObj = Instantiate(cellData.motherData.prefab, elementsContainer);
                    var motherView = newElementObj.GetComponent<MotherView>();
                    if (motherView != null)
                    {
                        motherView.Initialize(cellData.motherData, x, y, worldPos, cellSize, cellSpacing);
                        elementView = motherView;
                        
                        // Occupy the grid space
                        for (int i = 0; i < cellData.motherData.gridWidth; i++)
                        {
                            for (int j = 0; j < cellData.motherData.gridHeight; j++)
                            {
                                int bx = x + i;
                                int by = y + j;
                                if (bx >= 0 && bx < currentLevel.width && by >= 0 && by < currentLevel.height)
                                {
                                    activeElements[bx, by] = motherView;
                                }
                            }
                        }
                    }
                }
            }
            // Handle Child Elements
            else if (cellData.itemType == CellItemType.Child && cellData.childData != null)
            {
                if (cellData.childData.prefab != null)
                {
                    newElementObj = Instantiate(cellData.childData.prefab, elementsContainer);
                    var childView = newElementObj.GetComponent<ChildView>();
                    if (childView != null)
                    {
                        childView.Initialize(cellData.childData, x, y, worldPos, cellSize);
                        elementView = childView;
                    }
                }
            }

            if (newElementObj != null && elementView != null)
            {
                newElementObj.name = $"Element_{x}_{y}";
                activeElements[x, y] = elementView;
                
                if (elementView is RetractingCollectibleView collectView)
                {
                    // 确保在生成后立刻应用正确的变换
                    // 这里不需要做额外的旋转操作，因为 collectView.Initialize 中已经处理了
                    Vector2Int dirVec = Vector2Int.zero;
                    switch (collectView.CurrentDirection)
                    {
                        case Direction.Up: dirVec = Vector2Int.up; break;
                        case Direction.Down: dirVec = Vector2Int.down; break;
                        case Direction.Left: dirVec = Vector2Int.left; break;
                        case Direction.Right: dirVec = Vector2Int.right; break;
                    }
                    
                    for (int i = 1; i < collectView.CurrentLength; i++)
                    {
                        int bx = x + dirVec.x * i;
                        int by = y + dirVec.y * i;
                        if (bx >= 0 && bx < currentLevel.width && by >= 0 && by < currentLevel.height)
                        {
                            activeElements[bx, by] = collectView;
                        }
                    }
                }
                else
                {
                    // For all other elements, reset rotation just in case
                    elementView.transform.rotation = Quaternion.identity;
                }
            }
        }

        /// <summary>
        /// Converts grid coordinates (x, y) to World Space position.
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            float posX = x * (cellSize + cellSpacing);
            float posY = y * (cellSize + cellSpacing);
            return boardBottomLeftOrigin + new Vector3(posX, posY, 0f);
        }

        private void ClearContainer(Transform container)
        {
            if (container == null) return;
            // Use backwards iteration for safe destruction
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(container.GetChild(i).gameObject);
                else
                    DestroyImmediate(container.GetChild(i).gameObject);
            }
        }

        private bool IsActiveLevelTarget(ElementView element)
        {
            if (element == null || gameManager == null || gameManager.activeTargets == null) return false;

            foreach (var target in gameManager.activeTargets)
            {
                if (target == null || target.remainingAmount <= 0 || target.definition == null) continue;

                if (target.definition.targetType == CellItemType.SpecificNormal &&
                    element is NormalElementView normal && target.definition.normalElementId == normal.NormalElementId) return true;
                if (target.definition.targetType == CellItemType.RetractingCollectible &&
                    element is RetractingCollectibleView retract && target.definition.collectibleData != null &&
                    target.definition.collectibleData.collectibleId == retract.collectibleData.collectibleId) return true;
                if (target.definition.targetType == CellItemType.Obstacle &&
                    element is ObstacleView obstacle && target.definition.obstacleData != null &&
                    target.definition.obstacleData.obstacleId == obstacle.Data.obstacleId) return true;
                if (target.definition.targetType == CellItemType.Generator &&
                    element is GeneratorView generator && target.definition.generatorData != null &&
                    target.definition.generatorData.generatorId == generator.Data.generatorId) return true;
                if (target.definition.targetType == CellItemType.Special &&
                    element is SpecialElementView special && target.definition.specialElement != null &&
                    target.definition.specialElement.elementId == special.Data.elementId) return true;
                if (target.definition.targetType == CellItemType.Mother &&
                    element is ChildView child && target.definition.motherData != null &&
                    target.definition.motherData.motherId == child.Data.targetMotherId) return true;
                if (target.definition.targetType == CellItemType.Child &&
                    element is ChildView directChild && target.definition.childData != null &&
                    target.definition.childData.childId == directChild.Data.childId) return true;
            }

            return false;
        }

        private List<ElementView> GetPriorityTargets(int count)
        {
            List<ElementView> targets = new List<ElementView>();
            List<ElementView> priorityPool = new List<ElementView>();
            List<ElementView> secondaryPool = new List<ElementView>();

            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    ElementView element = activeElements[x, y];
                    if (element != null)
                    {
                        if (IsActiveLevelTarget(element)) priorityPool.Add(element);
                        else secondaryPool.Add(element);
                    }
                }
            }

            // Shuffle pools
            for (int i = 0; i < priorityPool.Count; i++) { int r = Random.Range(i, priorityPool.Count); var temp = priorityPool[i]; priorityPool[i] = priorityPool[r]; priorityPool[r] = temp; }
            for (int i = 0; i < secondaryPool.Count; i++) { int r = Random.Range(i, secondaryPool.Count); var temp = secondaryPool[i]; secondaryPool[i] = secondaryPool[r]; secondaryPool[r] = temp; }

            foreach (var p in priorityPool) { if (targets.Count < count) targets.Add(p); else break; }
            foreach (var s in secondaryPool) { if (targets.Count < count) targets.Add(s); else break; }
            
            return targets;
        }

        private System.Collections.IEnumerator RotateSpecials(SpecialElementView specA, SpecialElementView specB, float duration = 0.5f)
        {
            int sortingA = 0;
            int sortingB = 0;
            if (specA.spriteRenderer != null) { sortingA = specA.spriteRenderer.sortingOrder; specA.spriteRenderer.sortingOrder = 200; }
            if (specB.spriteRenderer != null) { sortingB = specB.spriteRenderer.sortingOrder; specB.spriteRenderer.sortingOrder = 200; }

            if (specA.spineAnimation != null)
            {
                var mr = specA.spineAnimation.GetComponent<MeshRenderer>();
                if (mr != null) { sortingA = mr.sortingOrder; mr.sortingOrder = 200; }
            }
            if (specB.spineAnimation != null)
            {
                var mr = specB.spineAnimation.GetComponent<MeshRenderer>();
                if (mr != null) { sortingB = mr.sortingOrder; mr.sortingOrder = 200; }
            }

            Vector3 posA = specA.transform.position;
            Vector3 posB = specB.transform.position;
            Vector3 center = (posA + posB) / 2f;
            
            Vector3 origScaleA = specA.transform.localScale;
            Vector3 origScaleB = specB.transform.localScale;
            
            specA.transform.DOScale(origScaleA * 1.3f, 0.2f);
            specB.transform.DOScale(origScaleB * 1.3f, 0.2f);

            bool isHorizontal = Mathf.Abs(posA.x - posB.x) > Mathf.Abs(posA.y - posB.y);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float angle = t * 360f;
                
                float rad = angle * Mathf.Deg2Rad;
                Vector3 offsetA = posA - center;
                
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                
                Vector3 newOffsetA;
                if (isHorizontal)
                {
                    // Orbit in XZ plane (looks like horizontal swapping back and forth)
                    // We use Y for Z-depth simulation if needed, but since it's orthographic, we just move X and scale/sort if we wanted.
                    // Let's just do a 2D orbit but squeezed so it looks like 3D.
                    newOffsetA = new Vector3(offsetA.x * cos, offsetA.x * sin * 0.3f, 0); 
                }
                else
                {
                    // Orbit in YZ plane (looks like vertical swapping back and forth)
                    newOffsetA = new Vector3(offsetA.y * sin * 0.3f, offsetA.y * cos, 0);
                }
                
                specA.transform.position = center + newOffsetA;
                specB.transform.position = center - newOffsetA;
                
                yield return null;
            }
            
            specA.transform.position = posA;
            specB.transform.position = posB;
            
            specA.transform.DOScale(origScaleA, 0.2f);
            specB.transform.DOScale(origScaleB, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }

        private System.Collections.IEnumerator ActivateSpecialCombo(SpecialElementView specA, SpecialElementView specB)
        {
            int cx = specA.GridX;
            int cy = specA.GridY;

            if (activeElements[specA.GridX, specA.GridY] == specA) activeElements[specA.GridX, specA.GridY] = null;
            if (activeElements[specB.GridX, specB.GridY] == specB) activeElements[specB.GridX, specB.GridY] = null;

            bool isRocketA = specA.BehaviorType == SpecialBehaviorType.LineClearHorizontal || specA.BehaviorType == SpecialBehaviorType.LineClearVertical;
            bool isRocketB = specB.BehaviorType == SpecialBehaviorType.LineClearHorizontal || specB.BehaviorType == SpecialBehaviorType.LineClearVertical;
            bool isBombA = specA.BehaviorType == SpecialBehaviorType.BombExplosion;
            bool isBombB = specB.BehaviorType == SpecialBehaviorType.BombExplosion;
            bool isBirdA = specA.BehaviorType == SpecialBehaviorType.TargetSeeker;
            bool isBirdB = specB.BehaviorType == SpecialBehaviorType.TargetSeeker;
            bool isMagicA = specA.BehaviorType == SpecialBehaviorType.ColorClear;
            bool isMagicB = specB.BehaviorType == SpecialBehaviorType.ColorClear;

            List<ElementView> toDestroy = new List<ElementView>();

            // 交换 Spine 动画（接口挂载）：只播一个元素——specA 优先，未命中时由 specB 播放。
            // 在 SpecialElementView 的 swapAnimations 列表配置（支持独立骨骼 Data 的 Spine 子物体层），
            // 动画跟随元素移动；未配置的组合静默跳过。
            // 交换动画成功播放后：双方标记 swapAnimationPlayed（销毁时都跳过 Trigger 动画）、
            // 停止 Idle/静止显示，且不再执行旋转，只保留交换表演。
            bool swapPlayed = false;
            float swapDuration = 0f;
            float swapAnimStartTime = 0f; // 交换 Spine 动画开始时间（用于计算动画剩余时长）
            // 魔力球与其他特效交换时不使用 Spine 交换动画：表现改为"随机选择一种普通元素，
            // 魔力球发射射线，射线到达对应元素时该元素变身成交换的特效"（见下方 Magic + Other 分支）。
            // 仅当两个魔力球（或组合中完全没有魔力球）时才保留 Spine 交换动画。
            if (isMagicA == isMagicB)
            {
                // 两个魔力球交换：Spine 动画只播放一遍，放完停在最后一帧（不循环重复）；
                // 其他组合（炸弹+炸弹等）保持循环，销毁前动画不中断。
                bool loopSwapAnim = !(isMagicA && isMagicB);
                swapPlayed = specA.PlaySwapAnimation(specB.BehaviorType, loopSwapAnim);
                if (!swapPlayed) swapPlayed = specB.PlaySwapAnimation(specA.BehaviorType, loopSwapAnim);
            }
            if (swapPlayed)
            {
                specA.swapAnimationPlayed = true; // 双方都标记：即使只有一方实际播放交换动画
                specB.swapAnimationPlayed = true;
                specA.StopIdleVisuals();
                specB.StopIdleVisuals();
                // 记录交换动画时长：销毁元素前等待动画完整播放，避免动画被提前打断
                swapDuration = specA.GetSwapAnimationDuration(specB.BehaviorType);
                if (swapDuration <= 0f) swapDuration = specB.GetSwapAnimationDuration(specA.BehaviorType);
                swapAnimStartTime = Time.time;
            }

            bool needsRotation = false;
            // 注：isMagicA && isMagicB 不再参与旋转（RotateSpecials），
            // 两个魔力球交换时改由 Spine 交换动画（PlaySwapAnimation）表现。
            if ((isRocketA && isBombB) || (isBombA && isRocketB) || 
                (isBombA && isBombB) || 
                (isBirdA && isRocketB) || (isRocketA && isBirdB) || 
                (isBirdA && isBombB) || (isBombA && isBirdB))
            {
                needsRotation = true;
            }

            // 交换动画已播放时跳过旋转（旋转会与交换表演冲突）
            if (needsRotation && !swapPlayed)
            {
                if (isBirdA)
                {
                    specA.PlayTriggerAnimation();
                    if (specA.Data?.activationEffectPrefab != null)
                    {
                        GameObject fx = Instantiate(specA.Data.activationEffectPrefab, specA.transform.position, Quaternion.identity, specA.transform);
                        fx.transform.localPosition = Vector3.zero;
                    }
                }
                if (isBirdB)
                {
                    specB.PlayTriggerAnimation();
                    if (specB.Data?.activationEffectPrefab != null)
                    {
                        GameObject fx = Instantiate(specB.Data.activationEffectPrefab, specB.transform.position, Quaternion.identity, specB.transform);
                        fx.transform.localPosition = Vector3.zero;
                    }
                }
                yield return StartCoroutine(RotateSpecials(specA, specB, 0.5f));
            }

            // 播放组合音效（在旋转动画结束后再播放）
            if (AudioManager.Instance != null)
            {
                if (isBirdA || isBirdB)
                {
                    // 如果有飞鸟，在飞向目标时播放飞鸟的声音
                    AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.TargetSeeker);
                }
                else if (isMagicA && isMagicB)
                {
                    // 魔力球+魔力球：碰撞音效在下方分支中与“两球在棋盘中心相撞”的瞬间同步播放
                }
                else if (isBombA && isBombB)
                {
                    // 炸弹+炸弹：合并爆炸音效在下方分支中与交换动画同步播放
                }
                else
                {
                    // 其他情况，播放两者其中之一的声音
                    AudioManager.Instance.PlaySpecialTriggerSound(specA.BehaviorType);
                }
            }

            if (isRocketA && isRocketB)
            {
                // Rocket + Rocket: Cross clear with visual effects
                GameObject trailPrefabA = specA.Data?.trailEffectPrefab ?? specB.Data?.trailEffectPrefab;
                if (trailPrefabA != null)
                {
                    Vector3 startPos = GetWorldPosition(cx, cy);
                    Vector3[] dirs = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
                    foreach(var d in dirs)
                    {
                        GameObject trail = Instantiate(trailPrefabA, startPos, Quaternion.identity);
                        float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
                        trail.transform.rotation = Quaternion.Euler(0, 0, angle);
                        trail.transform.DOMove(startPos + d * 20f, 1f).SetEase(Ease.Linear);
                        Destroy(trail, 1f);
                    }
                }
                // 交换动画已播放时不再播 Trigger（主 Spine 已隐藏，避免与新交换表演冲突）
                if (!swapPlayed)
                {
                    specA.PlayTriggerAnimation();
                    specB.PlayTriggerAnimation();
                }

                for (int x = 0; x < currentLevel.width; x++) if (activeElements[x, cy] != null) toDestroy.Add(activeElements[x, cy]);
                for (int y = 0; y < currentLevel.height; y++) if (activeElements[cx, y] != null) toDestroy.Add(activeElements[cx, y]);
                
                DestroyElementVisual(specA);
                DestroyElementVisual(specB);
            }
            else if ((isRocketA && isBombB) || (isBombA && isRocketB))
            {
                // Rocket + Bomb: Clear 3 rows and 3 columns
                // 交换动画完整播放后再销毁，避免动画被提前打断
                if (swapDuration > 0f) yield return new WaitForSeconds(swapDuration);
                DestroyElementVisual(specA);
                DestroyElementVisual(specB);

                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int yOffset = -1; yOffset <= 1; yOffset++)
                    {
                        if (cy + yOffset >= 0 && cy + yOffset < currentLevel.height && activeElements[x, cy + yOffset] != null)
                            toDestroy.Add(activeElements[x, cy + yOffset]);
                    }
                }
                for (int y = 0; y < currentLevel.height; y++)
                {
                    for (int xOffset = -1; xOffset <= 1; xOffset++)
                    {
                        if (cx + xOffset >= 0 && cx + xOffset < currentLevel.width && activeElements[cx + xOffset, y] != null)
                            toDestroy.Add(activeElements[cx + xOffset, y]);
                    }
                }
            }
            else if (isBombA && isBombB)
            {
                // Bomb + Bomb: 5x5 explosion
                // 炸弹+炸弹交换：动画一开始就播放合并爆炸音效（与交换动画同步）。
                // 交换 Spine 动画在方法开头已启动且此后没有帧等待，此处即动画开始帧
                if (AudioManager.Instance != null) AudioManager.Instance.PlayBombBombMergeSound();
                // 交换动画完整播放后再销毁，避免动画被提前打断
                if (swapDuration > 0f) yield return new WaitForSeconds(swapDuration);
                DestroyElementVisual(specA);
                DestroyElementVisual(specB);

                for (int x = cx - 2; x <= cx + 2; x++)
                {
                    for (int y = cy - 2; y <= cy + 2; y++)
                    {
                        if (x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height && activeElements[x, y] != null)
                            toDestroy.Add(activeElements[x, y]);
                    }
                }
            }
            else if ((isBirdA && isBirdB))
            {
                // Bird + Bird: 3 birds fly out and each performs the bird's target-area clear.
                var targets = GetPriorityTargets(3);
                
                GameObject birdPrefab = specA.Data?.prefab ?? specB.Data?.prefab;
                GameObject fxPrefab = specA.Data?.activationEffectPrefab ?? specB.Data?.activationEffectPrefab;
                float flyTime = 0.5f;
                Vector3 centerPos = (specA.transform.position + specB.transform.position) / 2f;

                // Hide original visual elements immediately
                specA.gameObject.SetActive(false);
                specB.gameObject.SetActive(false);

                List<GameObject> flyingBirds = new List<GameObject>();

                if (targets.Count > 0)
                {
                    foreach(var target in targets)
                    {
                        if (birdPrefab != null)
                        {
                            GameObject newBirdObj = Instantiate(birdPrefab, elementsContainer);
                            newBirdObj.transform.position = centerPos;
                            
                            SpecialElementView newBirdView = newBirdObj.GetComponent<SpecialElementView>();
                            if (newBirdView != null)
                            {
                                // Initialize but force the position
                                newBirdView.Initialize(specA.Data, cx, cy, centerPos, cellSize);
                                newBirdView.PlayTriggerAnimation();
                            }
                            
                            if (fxPrefab != null)
                            {
                                GameObject fx = Instantiate(fxPrefab, newBirdObj.transform.position, Quaternion.identity, newBirdObj.transform);
                                fx.transform.localPosition = Vector3.zero;
                            }

                            newBirdObj.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.InQuad);
                            flyingBirds.Add(newBirdObj);
                        }
                        // A combined bird performs its complete hit at the destination:
                        // clear the target and its four orthogonal neighbours.
                        toDestroy.Add(target);
                        Vector2Int[] birdAreaOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                        foreach (Vector2Int offset in birdAreaOffsets)
                        {
                            int areaX = target.GridX + offset.x;
                            int areaY = target.GridY + offset.y;
                            if (areaX >= 0 && areaX < currentLevel.width && areaY >= 0 && areaY < currentLevel.height &&
                                activeElements[areaX, areaY] != null)
                            {
                                toDestroy.Add(activeElements[areaX, areaY]);
                            }
                        }
                    }
                    yield return new WaitForSeconds(flyTime);
                }

                foreach(var b in flyingBirds)
                {
                    if (b != null) Destroy(b);
                }

                DestroyElementVisual(specA);
                DestroyElementVisual(specB);
            }
            else if (isMagicA && isMagicB)
            {
                // MagicBall + MagicBall: Clear entire board
                isBoardClearing = true;
                float centerX = (currentLevel.width - 1) / 2f;
                float centerY = (currentLevel.height - 1) / 2f;
                Vector3 boardCenter = boardBottomLeftOrigin + new Vector3(centerX * (cellSize + cellSpacing), centerY * (cellSize + cellSpacing), 0f);

                // 魔力球聚光：两个魔力球交换期间点亮两个球，其余棋盘加遮罩变暗
                // 注意：两次 Show 会重复计数导致遮罩永不关闭，故只 Show 一次，第二个球单独提升排序
                ShowMagicBallFocus(specA.gameObject);
                RaiseRenderersAboveFocus(specB.gameObject);

                // 两个魔力球飞向棋盘中心（交换 Spine 动画已在方法开头统一播放，跟随元素移动）
                float rotateTime = 0.6f;
                specA.transform.DOMove(boardCenter, rotateTime).SetEase(Ease.InOutQuad);
                specB.transform.DOMove(boardCenter, rotateTime).SetEase(Ease.InOutQuad);

                yield return new WaitForSeconds(rotateTime);

                // 在中心播放粒子特效
                GameObject centerFxPrefab = specA.Data?.activationEffectPrefab ?? specB.Data?.activationEffectPrefab;
                if (centerFxPrefab != null)
                {
                    GameObject fx = Instantiate(centerFxPrefab, boardCenter, Quaternion.identity);
                    fx.transform.localScale = Vector3.one * 1.5f;
                    RaiseRenderersAboveFocus(fx); // 中心特效保持明亮
                    Destroy(fx, 2f);
                }

                // 魔力球+魔力球交换：两个魔力球在中心相撞的瞬间播放交换音效（与中心特效同步）
                if (AudioManager.Instance != null) AudioManager.Instance.PlayMagicBallSwapSound();

                // 魔力球交换后不再播放 Trigger 动画，仅保留交换动画（已在方法开头统一播放，播放一遍后停止）

                // 两个魔力球交换不再发射拖尾射线：改为交换 Spine 动画播放期间在棋盘中心
                // 播放粒子特效（挂载接口：SpecialElementView.swapParticlePrefab，PlaySwapParticleEffect 播放）。
                SpecialElementView swapFxSource = null;
                if (specA != null && specA.swapParticlePrefab != null) swapFxSource = specA;
                else if (specB != null && specB.swapParticlePrefab != null) swapFxSource = specB;
                if (swapFxSource != null)
                {
                    float swapFxDur = swapDuration > 0f ? swapDuration : 2f;
                    GameObject swapFx = swapFxSource.PlaySwapParticleEffect(boardCenter, swapFxDur);
                    if (swapFx != null) RaiseRenderersAboveFocus(swapFx); // 中心交换粒子特效保持明亮
                }

                List<ElementView> allTargets = new List<ElementView>();
                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] != null)
                        {
                            allTargets.Add(activeElements[x, y]);
                        }
                    }
                }

                // 收集所有目标元素：不再按拖尾射线逐个命中，统一在遮罩撤下时消除
                foreach (var target in allTargets)
                {
                    if (target is NormalElementView norm)
                    {
                        norm.PlayChangeAnimation();
                    }
                    toDestroy.Add(target);
                }

                // 两个魔力球交换的 Spine 动画只播放一遍：若动画还没放完，先等它放完再淡出销毁，
                // 保证完整播放一遍后停在最后一帧（不循环、不截断）。
                if (swapDuration > 0f && swapAnimStartTime > 0f)
                {
                    float remain = swapDuration - (Time.time - swapAnimStartTime);
                    if (remain > 0.05f) yield return new WaitForSeconds(remain);
                }

                // 缩小渐变消失
                float fadeOutDur = 0.3f;

                // 演出收尾：在棋盘中心播放收尾粒子特效。
                // 在撤遮罩之前先创建粒子（粒子在遮罩之下开始播放），遮罩熄灭的同一帧粒子已喷射，无感知延迟
                PlayMagicBallFinishEffect(specA != null ? specA : specB, boardCenter);

                // 聚光演出结束，撤掉遮罩（两个魔力球随后淡出销毁）
                HideMagicBallFocus();

                // 遮罩消失的同一帧立即开始清屏，不再等待两个魔力球完成 0.3 秒淡出。
                // 长条物体等多格元素会在列表中重复出现，因此先去重再交给统一销毁逻辑。
                List<ElementView> immediateClearTargets = new List<ElementView>();
                foreach (var target in toDestroy)
                {
                    if (target != null && !immediateClearTargets.Contains(target))
                    {
                        immediateClearTargets.Add(target);
                    }
                }
                if (immediateClearTargets.Count > 0)
                {
                    DestroyElements(immediateClearTargets);
                }
                toDestroy.Clear(); // 已即时处理，避免组合方法末尾再次销毁

                if (specA != null)
                {
                    specA.transform.DOScale(Vector3.zero, fadeOutDur).SetEase(Ease.InBack);
                    if (specA.spineAnimation != null && specA.spineAnimation.skeleton != null) DOTween.To(() => specA.spineAnimation.skeleton.A, x => specA.spineAnimation.skeleton.A = x, 0f, fadeOutDur);
                    else if (specA.spriteRenderer != null) specA.spriteRenderer.DOFade(0f, fadeOutDur);
                }

                if (specB != null)
                {
                    specB.transform.DOScale(Vector3.zero, fadeOutDur).SetEase(Ease.InBack);
                    if (specB.spineAnimation != null && specB.spineAnimation.skeleton != null) DOTween.To(() => specB.spineAnimation.skeleton.A, x => specB.spineAnimation.skeleton.A = x, 0f, fadeOutDur);
                    else if (specB.spriteRenderer != null) specB.spriteRenderer.DOFade(0f, fadeOutDur);
                }

                yield return new WaitForSeconds(fadeOutDur);

                if (specA != null && specA.gameObject != null) Destroy(specA.gameObject);
                if (specB != null && specB.gameObject != null) Destroy(specB.gameObject);
            }
            else if ((isBirdA && isRocketB) || (isRocketA && isBirdB))
            {
                // Bird + Rocket
                var targets = GetPriorityTargets(1);
                if (targets.Count > 0)
                {
                    ElementView target = targets[0];
                    int targetX = target.GridX;
                    int targetY = target.GridY;
                    toDestroy.Add(target);

                    float flyTime = 0.5f;
                    specA.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.InQuad);
                    specB.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.InQuad);
                    
                    yield return new WaitForSeconds(flyTime);

                    bool isHoriz = (isRocketA ? specA.BehaviorType : specB.BehaviorType) == SpecialBehaviorType.LineClearHorizontal;
                    
                    // 飞鸟到达目标后，播放另一个特殊元素（火箭）的音效
                    if (AudioManager.Instance != null) AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.LineClearHorizontal);
                    
                    // Show rocket trails from target
                    SpecialElementView rocketSpec = isRocketA ? specA : specB;
                    GameObject trailPrefab = rocketSpec.Data?.trailEffectPrefab;
                    if (trailPrefab != null)
                    {
                        Vector3[] dirs = isHoriz ? new Vector3[] { Vector3.left, Vector3.right } : new Vector3[] { Vector3.up, Vector3.down };
                        foreach(var d in dirs)
                        {
                            GameObject trail = Instantiate(trailPrefab, target.transform.position, Quaternion.identity);
                            float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
                            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
                            trail.transform.DOMove(target.transform.position + d * 20f, 1f).SetEase(Ease.Linear);
                            Destroy(trail, 1f);
                        }
                    }

                    if (isHoriz)
                    {
                        for (int x = 0; x < currentLevel.width; x++) if (activeElements[x, targetY] != null && activeElements[x, targetY] != target) toDestroy.Add(activeElements[x, targetY]);
                    }
                    else
                    {
                        for (int y = 0; y < currentLevel.height; y++) if (activeElements[targetX, y] != null && activeElements[targetX, y] != target) toDestroy.Add(activeElements[targetX, y]);
                    }
                }
                DestroyElementVisual(specA);
                DestroyElementVisual(specB);
            }
            else if ((isBirdA && isBombB) || (isBombA && isBirdB))
            {
                // Bird + Bomb
                var targets = GetPriorityTargets(1);
                if (targets.Count > 0)
                {
                    ElementView target = targets[0];
                    int targetX = target.GridX;
                    int targetY = target.GridY;
                    
                    float flyTime = 0.5f;
                    specA.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.InQuad);
                    specB.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.InQuad);
                    
                    yield return new WaitForSeconds(flyTime);

                    // 飞鸟到达目标后，播放另一个特殊元素（炸弹）的音效
                    if (AudioManager.Instance != null) AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.BombExplosion);

                    // Show bomb fx
                    SpecialElementView bombSpec = isBombA ? specA : specB;
                    GameObject fxPrefab = bombSpec.Data?.activationEffectPrefab;
                    if (fxPrefab != null)
                    {
                        GameObject fx = Instantiate(fxPrefab, target.transform.position, Quaternion.identity);
                        Destroy(fx, 2f);
                    }

                    for (int x = targetX - 1; x <= targetX + 1; x++)
                    {
                        for (int y = targetY - 1; y <= targetY + 1; y++)
                        {
                            if (x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height && activeElements[x, y] != null)
                                toDestroy.Add(activeElements[x, y]);
                        }
                    }
                }
                DestroyElementVisual(specA);
                DestroyElementVisual(specB);
            }
            else if ((isMagicA && isRocketB) || (isRocketA && isMagicB) || (isMagicA && isBombB) || (isBombA && isMagicB) || (isMagicA && isBirdB) || (isBirdA && isMagicB))
            {
                // Magic + Other Special
                SpecialElementView magicBall = isMagicA ? specA : specB;
                SpecialElementView otherSpec = isMagicA ? specB : specA;
                bool otherSpecialTriggeredDirectly = false;
                List<int> colors = new List<int>();
                for (int x = 0; x < currentLevel.width; x++)
                {
                    for (int y = 0; y < currentLevel.height; y++)
                    {
                        if (activeElements[x, y] is NormalElementView norm && !colors.Contains(norm.NormalElementId))
                            colors.Add(norm.NormalElementId);
                    }
                }
                
                if (colors.Count > 0)
                {
                    int targetColor = colors[Random.Range(0, colors.Count)];

                    // 魔力球与其它特效交换的表现：不使用 Spine 交换动画。
                    // 魔力球播放自身的触发动画（循环），随机选择一种普通元素，
                    // 向所有该颜色元素发射射线；射线到达对应元素时，该元素立即变身成交换的特效。
                    magicBall.PlayTriggerAnimation(true); // 循环播放触发动画

                    GameObject trailPrefab = magicBall.Data?.trailEffectPrefab;
                    float flyTime = 0.5f; // Sped up from 0.8f

                    // 收集目标颜色的普通元素（射线发射的目标）
                    List<ElementView> convertedElements = new List<ElementView>();
                    for (int x = 0; x < currentLevel.width; x++)
                    {
                        for (int y = 0; y < currentLevel.height; y++)
                        {
                            if (activeElements[x, y] is NormalElementView norm && norm.NormalElementId == targetColor)
                            {
                                convertedElements.Add(norm);
                            }
                        }
                    }

                    List<SpecialElementData> horizRockets = elementDb.specialElements.FindAll(e => e.behaviorType == SpecialBehaviorType.LineClearHorizontal);
                    List<SpecialElementData> vertRockets = elementDb.specialElements.FindAll(e => e.behaviorType == SpecialBehaviorType.LineClearVertical);
                    SpecialElementData bombData = elementDb.specialElements.Find(e => e.behaviorType == SpecialBehaviorType.BombExplosion);
                    SpecialElementData birdData = elementDb.specialElements.Find(e => e.behaviorType == SpecialBehaviorType.TargetSeeker);

                    // 彩球与特效交换：首个元素变身成特效的瞬间播放一次“生成特效”音效，
                    // 每波只播一次，避免多个元素同时变身时音效叠加过响
                    bool generateSoundPlayed = false;

                    // 射线到达对应元素时，将该元素变身成交换的特效（销毁普通元素并在原格生成目标特效）
                    System.Action<ElementView> convertOnRayArrival = (ElementView target) =>
                    {
                        if (target == null || target.gameObject == null) return;
                        if (activeElements[target.GridX, target.GridY] != target) return; // 已被其他逻辑处理

                        // 变身成特效时播放“生成特效”音效（与首个特效出现同步，每波只播一次）
                        if (!generateSoundPlayed && AudioManager.Instance != null)
                        {
                            generateSoundPlayed = true;
                            AudioManager.Instance.PlayMagicBallGenerateSound();
                        }

                        int mx = target.GridX;
                        int my = target.GridY;

                        // 光线到达后立即生成普通元素消除特效（销毁时不再重复播放）
                        if (target is NormalElementView hitNormal)
                        {
                            hitNormal.clearEffectAlreadyPlayed = true;
                            SpawnNormalClearEffect(hitNormal);
                        }

                        // Destroy normal visually
                        activeElements[mx, my] = null;
                        DestroyElementVisual(target);

                        SpecialElementData dataToSpawn = null;
                        if (isRocketA || isRocketB)
                        {
                            bool isHoriz = Random.value > 0.5f;
                            if (isHoriz && horizRockets.Count > 0) dataToSpawn = horizRockets[0];
                            else if (!isHoriz && vertRockets.Count > 0) dataToSpawn = vertRockets[0];
                            else if (horizRockets.Count > 0) dataToSpawn = horizRockets[0];
                            else if (vertRockets.Count > 0) dataToSpawn = vertRockets[0];
                        }
                        else if (isBombA || isBombB)
                        {
                            dataToSpawn = bombData;
                        }
                        else if (isBirdA || isBirdB)
                        {
                            dataToSpawn = birdData;
                            // 飞鸟的音效已经在上面触发过了
                        }

                        if (dataToSpawn != null && dataToSpawn.prefab != null)
                        {
                            GameObject newSpecObj = Instantiate(dataToSpawn.prefab, elementsContainer);
                            var specialView = newSpecObj.GetComponent<SpecialElementView>();
                            if (specialView != null)
                            {
                                specialView.Initialize(dataToSpawn, mx, my, GetWorldPosition(mx, my), cellSize);
                                activeElements[mx, my] = specialView;

                                // Save the target scale that was just calculated in Initialize
                                Vector3 targetScale = newSpecObj.transform.localScale;

                                // 创建动画：弹出缩放（Spine 创建动画已在 Initialize 中播放）
                                newSpecObj.transform.localScale = Vector3.zero;
                                newSpecObj.transform.DOScale(targetScale, 0.25f).SetEase(Ease.OutBack);

                                // 立即触发交换的特效：创建动画播放大部分后马上触发，避免过长停顿破坏节奏。
                                // 之前等完整创建动画（0.4s 起步）导致特效迟迟不触发；现改为按比例等待并封顶，
                                // 下限 0.2s 保证弹出缩放可见，上限 0.5s 避免创建动画过长的特效拖慢节奏。
                                float creationAnimDur = specialView.GetCreationAnimationDuration();
                                float creationDelay = Mathf.Clamp(creationAnimDur * 0.6f, 0.2f, 0.5f);
                                StartTrackedSpecial(() => ConvertedSpecialTriggerRoutine(specialView, creationDelay));
                            }
                        }
                    };

                    if (trailPrefab != null)
                    {
                        Vector3 magicBallStartPos = GetWorldPosition(magicBall.GridX, magicBall.GridY);

                        foreach (var target in convertedElements)
                        {
                            Vector3 targetPos = target.transform.position;
                            Vector3 dir = (targetPos - magicBallStartPos).normalized;

                            GameObject trail = Instantiate(trailPrefab, magicBallStartPos, Quaternion.identity);
                            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                            trail.transform.rotation = Quaternion.Euler(0, 0, angle);

                            // 射线到达对应元素时，该元素变身成交换的特效
                            // 修复：原代码 Destroy(trail, flyTime) 与 DOMove 时长相同，同一帧内若 Unity 延迟销毁先于
                            // DOTween 完成 tween 处理，Safe Mode 会捕获 "missing target" 并跳过 OnComplete，
                            // 导致 convertOnRayArrival 永不执行（射线到达后无任何反应、不生成火箭）。
                            // 改为在 OnComplete 内销毁 trail；另加更长延迟的兜底销毁，避免异常时泄漏（幂等安全）。
                            trail.transform.DOMove(targetPos, flyTime).SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    convertOnRayArrival(target);
                                    if (trail != null) Destroy(trail);
                                });
                            Destroy(trail, flyTime + 0.5f);
                        }

                        // 等待所有射线飞行到达（各自在到达时完成变身）
                        yield return new WaitForSeconds(flyTime);
                    }
                    else
                    {
                        // 未配置射线特效时，元素立即变身
                        foreach (var target in convertedElements) convertOnRayArrival(target);
                    }
                }
                else
                {
                    // No normal colour exists to convert. Do not consume the combination without an effect:
                    // trigger the swapped special once at its current grid position as a deterministic fallback.
                    otherSpecialTriggeredDirectly = true;
                    yield return StartCoroutine(ActivateSpecial(otherSpec, null, true));
                }
                
                // 停止魔力球循环并缩小渐变消失，另一个特殊元素正常销毁
                SpecialElementView mBall = magicBall;

                float fadeOutDur = 0.3f;

                if (mBall != null)
                {
                    mBall.transform.DOScale(Vector3.zero, fadeOutDur).SetEase(Ease.InBack);
                    if (mBall.spineAnimation != null && mBall.spineAnimation.skeleton != null) DOTween.To(() => mBall.spineAnimation.skeleton.A, x => mBall.spineAnimation.skeleton.A = x, 0f, fadeOutDur);
                    else if (mBall.spriteRenderer != null) mBall.spriteRenderer.DOFade(0f, fadeOutDur);
                }

                if (!otherSpecialTriggeredDirectly)
                {
                    DestroyElementVisual(otherSpec); // 其他特殊元素直接按照原来的方式销毁（如果有的话）
                }

                yield return new WaitForSeconds(fadeOutDur);
                if (mBall != null && mBall.gameObject != null) Destroy(mBall.gameObject);
            }

            // Remove duplicates
            List<ElementView> finalToDestroy = new List<ElementView>();
            foreach(var el in toDestroy)
            {
                if (!finalToDestroy.Contains(el)) finalToDestroy.Add(el);
            }

            if (finalToDestroy.Count > 0)
            {
                DestroyElements(finalToDestroy);
                yield return new WaitForSeconds(0.4f);
            }

            if (isMagicA && isMagicB)
            {
                isBoardClearing = false;
            }
        }
    }
}

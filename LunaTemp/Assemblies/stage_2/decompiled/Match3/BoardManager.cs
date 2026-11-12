using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	public class BoardManager : MonoBehaviour
	{
		public enum BoardState
		{
			Waiting,
			Animating
		}

		public class MatchCluster
		{
			public List<NormalElementView> elements = new List<NormalElementView>();

			public CreationCondition shape = CreationCondition.None;

			public Vector2Int intersectionPoint;
		}

		private class SpecialSpawnInfo
		{
			public int x;

			public int y;

			public SpecialElementData data;
		}

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
		public float boardEdgePadding = 0f;

		[Header("Conveyor Settings")]
		public List<ConveyorCornerConfig> cornerConfigs = new List<ConveyorCornerConfig>();

		private ElementDatabase elementDb;

		public ElementView[,] activeElements;

		private int[] refillSequenceIndices;

		private readonly HashSet<string> refillWarnings = new HashSet<string>();

		private Vector3 boardBottomLeftOrigin;

		public GameManager gameManager;

		public HashSet<SpecialElementView> currentlyTriggeringSpecials = new HashSet<SpecialElementView>();

		public List<int> pickedColorsThisTurn = new List<int>();

		public bool isBoardClearing = false;

		private int activeSpecialChains = 0;

		private bool hasConveyorMovedThisTurn = true;

		public BoardState currentState = BoardState.Waiting;

		private ElementView selectedElement;

		private Vector2 swipeStartPos;

		private Sprite cachedDefaultBgSprite;

		private float originalCellSize = -1f;

		private float originalCellSpacing = -1f;

		public bool pendingRefill = false;

		private const int MagicFocusMaskOrder = 25000;

		private const int MagicFocusRaisedOrder = 25002;

		private SpriteRenderer magicFocusMask;

		private int magicFocusMaskRefCount = 0;

		private readonly List<Renderer> raisedFocusRenderers = new List<Renderer>();

		private readonly List<int> raisedFocusOriginalOrders = new List<int>();

		private List<SpecialElementView> newSpecialsThisTurn = new List<SpecialElementView>();

		private HashSet<RetractingCollectibleView> retractedThisTurn = new HashSet<RetractingCollectibleView>();

		private HashSet<RetractingCollectibleView> directlyHitCollectiblesThisTurn = new HashSet<RetractingCollectibleView>();

		private HashSet<ObstacleView> damagedObstaclesThisTurn = new HashSet<ObstacleView>();

		private HashSet<GeneratorView> triggeredGeneratorsThisTurn = new HashSet<GeneratorView>();

		private HashSet<ChildView> triggeredChildrenThisTurn = new HashSet<ChildView>();

		private readonly HashSet<ElementView> pendingElementDestroys = new HashSet<ElementView>();

		private GameObject globalBgTile;

		public LevelData currentLevel { get; private set; }

		private void Update()
		{
			if (currentState == BoardState.Waiting)
			{
				if (currentlyTriggeringSpecials.Count > 0)
				{
					currentlyTriggeringSpecials.Clear();
				}
				if (pickedColorsThisTurn.Count > 0)
				{
					pickedColorsThisTurn.Clear();
				}
				if (pendingRefill)
				{
					pendingRefill = false;
					currentState = BoardState.Animating;
					StartCoroutine(ApplyGravityAndRefill());
					return;
				}
			}
			if (currentState != 0 || (gameManager != null && gameManager.isGameOver) || (gameManager != null && gameManager.isAdShowing))
			{
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				if (gameManager != null && ConfigManager.Instance != null && !string.IsNullOrEmpty(ConfigManager.Instance.adTriggerStep))
				{
					string currentKey = $"{gameManager.currentLevelIndex + 1}_{gameManager.playedSteps}";
					if (ConfigManager.Instance.adTriggerStep.Trim() == currentKey)
					{
						gameManager.TriggerGlobalAd();
						return;
					}
				}
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mouseWorldPos.z = 0f;
				Vector2Int gridPos = GetGridPositionFromWorld(mouseWorldPos);
				if ((!(gameManager != null) || !(gameManager.guideManager != null) || !gameManager.guideManager.HasPendingTutorial || (gameManager.guideManager.IsGuideActive && (!(gridPos != gameManager.guideManager.CurrentTutorialFrom) || !(gridPos != gameManager.guideManager.CurrentTutorialTo)))) && IsValidGridPos(gridPos.x, gridPos.y) && activeElements[gridPos.x, gridPos.y] != null)
				{
					selectedElement = activeElements[gridPos.x, gridPos.y];
					swipeStartPos = mouseWorldPos;
				}
			}
			else
			{
				if (!Input.GetMouseButtonUp(0) || !(selectedElement != null))
				{
					return;
				}
				if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
				{
					if (!gameManager.guideManager.IsGuideActive)
					{
						selectedElement = null;
						return;
					}
					Vector2Int from2 = gameManager.guideManager.CurrentTutorialFrom;
					Vector2Int to2 = gameManager.guideManager.CurrentTutorialTo;
					Vector2Int selPos2 = new Vector2Int(selectedElement.GridX, selectedElement.GridY);
					if (from2 == to2 && selPos2 != from2)
					{
						selectedElement = null;
						return;
					}
				}
				Vector3 mouseWorldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mouseWorldPos2.z = 0f;
				Vector2 swipeDelta = (Vector2)mouseWorldPos2 - swipeStartPos;
				if (swipeDelta.magnitude > cellSize * 0.2f)
				{
					Vector2Int direction = Vector2Int.zero;
					direction = ((!(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))) ? ((swipeDelta.y > 0f) ? Vector2Int.up : Vector2Int.down) : ((swipeDelta.x > 0f) ? Vector2Int.right : Vector2Int.left));
					Vector2Int targetPos = new Vector2Int(selectedElement.GridX + direction.x, selectedElement.GridY + direction.y);
					if (gameManager != null && gameManager.guideManager != null && gameManager.guideManager.HasPendingTutorial)
					{
						if (!gameManager.guideManager.IsGuideActive)
						{
							selectedElement = null;
							return;
						}
						Vector2Int from3 = gameManager.guideManager.CurrentTutorialFrom;
						Vector2Int to3 = gameManager.guideManager.CurrentTutorialTo;
						Vector2Int selPos3 = new Vector2Int(selectedElement.GridX, selectedElement.GridY);
						if (from3 == to3)
						{
							selectedElement = null;
							return;
						}
						if ((!(selPos3 == from3) || !(targetPos == to3)) && (!(selPos3 == to3) || !(targetPos == from3)))
						{
							selectedElement = null;
							return;
						}
						gameManager.guideManager.CompleteTutorial();
					}
					if (IsValidGridPos(targetPos.x, targetPos.y) && activeElements[targetPos.x, targetPos.y] != null)
					{
						ElementView targetElement = activeElements[targetPos.x, targetPos.y];
						if (IsElementSwappable(selectedElement) && IsElementSwappable(targetElement))
						{
							if (AudioManager.Instance != null)
							{
								AudioManager.Instance.PlaySwapSound();
							}
							StartCoroutine(SwapAndCheck(selectedElement, targetElement));
						}
					}
				}
				else
				{
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
						if (from != to)
						{
							selectedElement = null;
							return;
						}
						if (selPos != from)
						{
							selectedElement = null;
							return;
						}
						gameManager.guideManager.CompleteTutorial();
					}
					if (selectedElement is SpecialElementView specialView)
					{
						StartCoroutine(TriggerSpecialOnClick(specialView));
					}
				}
				selectedElement = null;
			}
		}

		private IEnumerator TriggerSpecialOnClick(SpecialElementView special)
		{
			currentState = BoardState.Animating;
			hasConveyorMovedThisTurn = false;
			retractedThisTurn.Clear();
			directlyHitCollectiblesThisTurn.Clear();
			damagedObstaclesThisTurn.Clear();
			triggeredGeneratorsThisTurn.Clear();
			if (gameManager != null)
			{
				gameManager.OnPlayerMove();
			}
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

		private IEnumerator SwapAndCheck(ElementView elementA, ElementView elementB)
		{
			currentState = BoardState.Animating;
			hasConveyorMovedThisTurn = false;
			retractedThisTurn.Clear();
			directlyHitCollectiblesThisTurn.Clear();
			damagedObstaclesThisTurn.Clear();
			triggeredGeneratorsThisTurn.Clear();
			int ax = elementA.GridX;
			int ay = elementA.GridY;
			int bx = elementB.GridX;
			int by = elementB.GridY;
			activeElements[ax, ay] = elementB;
			activeElements[bx, by] = elementA;
			bool animFinished = false;
			elementA.MoveTo(bx, by, GetWorldPosition(bx, by), 0.3f, delegate
			{
				animFinished = true;
			});
			elementB.MoveTo(ax, ay, GetWorldPosition(ax, ay));
			yield return new WaitUntil(() => animFinished);
			bool triggeredSpecial = false;
			SpecialElementView specA2 = elementA as SpecialElementView;
			SpecialElementView specB2 = default(SpecialElementView);
			int num;
			if ((object)specA2 != null)
			{
				specB2 = elementB as SpecialElementView;
				num = (((object)specB2 != null) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			if (num != 0)
			{
				triggeredSpecial = true;
				yield return StartCoroutine(ActivateSpecialCombo(specA2, specB2));
			}
			else if (elementA is SpecialElementView specA)
			{
				List<MatchCluster> specialSwapClusters2 = FindAllMatchClusters(elementA, elementB);
				if (specialSwapClusters2.Count > 0)
				{
					if (gameManager != null)
					{
						gameManager.OnPlayerMove();
					}
					yield return StartCoroutine(ProcessMatchesAndRefill(specialSwapClusters2));
				}
				triggeredSpecial = true;
				yield return StartCoroutine(ActivateSpecial(specA, elementB));
			}
			else if (elementB is SpecialElementView specB)
			{
				List<MatchCluster> specialSwapClusters = FindAllMatchClusters(elementA, elementB);
				if (specialSwapClusters.Count > 0)
				{
					if (gameManager != null)
					{
						gameManager.OnPlayerMove();
					}
					yield return StartCoroutine(ProcessMatchesAndRefill(specialSwapClusters));
				}
				triggeredSpecial = true;
				yield return StartCoroutine(ActivateSpecial(specB, elementA));
			}
			if (triggeredSpecial)
			{
				if (gameManager != null)
				{
					gameManager.OnPlayerMove();
				}
				yield return StartCoroutine(ApplyGravityAndRefill());
				yield break;
			}
			List<MatchCluster> clusters = FindAllMatchClusters(elementA, elementB);
			if (clusters.Count > 0)
			{
				if (gameManager != null)
				{
					gameManager.OnPlayerMove();
				}
				yield return StartCoroutine(ProcessMatchesAndRefill(clusters));
				yield break;
			}
			activeElements[ax, ay] = elementA;
			activeElements[bx, by] = elementB;
			animFinished = false;
			elementA.MoveTo(ax, ay, GetWorldPosition(ax, ay), 0.3f, delegate
			{
				animFinished = true;
			});
			elementB.MoveTo(bx, by, GetWorldPosition(bx, by));
			yield return new WaitUntil(() => animFinished);
			if (!HasPossibleMoves())
			{
				yield return StartCoroutine(CheckAndShuffleIfNeeded());
				yield break;
			}
			currentState = BoardState.Waiting;
			if (gameManager != null)
			{
				gameManager.OnBoardSettled();
			}
		}

		private List<MatchCluster> FindAllMatchClusters()
		{
			return FindAllMatchClusters(null, null);
		}

		private List<MatchCluster> FindAllMatchClusters(ElementView swappedA, ElementView swappedB)
		{
			List<MatchCluster> finalClusters = new List<MatchCluster>();
			bool[,] processed = new bool[currentLevel.width, currentLevel.height];
			List<List<NormalElementView>> hLines = new List<List<NormalElementView>>();
			List<List<NormalElementView>> vLines = new List<List<NormalElementView>>();
			for (int y3 = 0; y3 < currentLevel.height; y3++)
			{
				for (int x = 0; x < currentLevel.width - 2; x++)
				{
					if (!(activeElements[x, y3] is NormalElementView norm))
					{
						continue;
					}
					int matchLen;
					for (matchLen = 1; x + matchLen < currentLevel.width && activeElements[x + matchLen, y3] is NormalElementView next; matchLen++)
					{
						if (next.NormalElementId != norm.NormalElementId)
						{
							break;
						}
					}
					if (matchLen >= 3)
					{
						List<NormalElementView> line = new List<NormalElementView>();
						for (int i = 0; i < matchLen; i++)
						{
							line.Add(activeElements[x + i, y3] as NormalElementView);
						}
						hLines.Add(line);
						x += matchLen - 1;
					}
				}
			}
			for (int x3 = 0; x3 < currentLevel.width; x3++)
			{
				for (int y2 = 0; y2 < currentLevel.height - 2; y2++)
				{
					if (!(activeElements[x3, y2] is NormalElementView norm2))
					{
						continue;
					}
					int matchLen2;
					for (matchLen2 = 1; y2 + matchLen2 < currentLevel.height && activeElements[x3, y2 + matchLen2] is NormalElementView next2; matchLen2++)
					{
						if (next2.NormalElementId != norm2.NormalElementId)
						{
							break;
						}
					}
					if (matchLen2 >= 3)
					{
						List<NormalElementView> line2 = new List<NormalElementView>();
						for (int j = 0; j < matchLen2; j++)
						{
							line2.Add(activeElements[x3, y2 + j] as NormalElementView);
						}
						vLines.Add(line2);
						y2 += matchLen2 - 1;
					}
				}
			}
			List<List<NormalElementView>> squares = new List<List<NormalElementView>>();
			for (int x4 = 0; x4 < currentLevel.width - 1; x4++)
			{
				for (int y4 = 0; y4 < currentLevel.height - 1; y4++)
				{
					if (activeElements[x4, y4] is NormalElementView norm3 && activeElements[x4 + 1, y4] is NormalElementView norm4 && activeElements[x4, y4 + 1] is NormalElementView norm5 && activeElements[x4 + 1, y4 + 1] is NormalElementView norm6 && norm3.NormalElementId == norm4.NormalElementId && norm3.NormalElementId == norm5.NormalElementId && norm3.NormalElementId == norm6.NormalElementId)
					{
						List<NormalElementView> square = new List<NormalElementView> { norm3, norm4, norm5, norm6 };
						squares.Add(square);
					}
				}
			}
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
					for (int k = allLines.Count - 1; k >= 0; k--)
					{
						bool share = false;
						foreach (NormalElementView el2 in allLines[k])
						{
							if (currentCluster.Contains(el2))
							{
								share = true;
								break;
							}
						}
						if (share)
						{
							foreach (NormalElementView el in allLines[k])
							{
								if (!currentCluster.Contains(el))
								{
									currentCluster.Add(el);
								}
							}
							allLines.RemoveAt(k);
							merged = true;
						}
					}
				}
				while (merged);
				mergedClusters.Add(currentCluster);
			}
			foreach (List<NormalElementView> clusterGroup in mergedClusters)
			{
				MatchCluster cluster = new MatchCluster();
				cluster.elements = clusterGroup;
				int minX = 999;
				int maxX = -1;
				int minY = 999;
				int maxY = -1;
				foreach (NormalElementView e2 in clusterGroup)
				{
					if (e2.GridX < minX)
					{
						minX = e2.GridX;
					}
					if (e2.GridX > maxX)
					{
						maxX = e2.GridX;
					}
					if (e2.GridY < minY)
					{
						minY = e2.GridY;
					}
					if (e2.GridY > maxY)
					{
						maxY = e2.GridY;
					}
				}
				int width = maxX - minX + 1;
				int height = maxY - minY + 1;
				bool hasLine5 = false;
				bool hasHorizontalFour = false;
				bool hasVerticalFour = false;
				bool hasCrossingLines = false;
				Vector2Int crossingPoint = Vector2Int.zero;
				foreach (List<NormalElementView> hLine in hLines)
				{
					if (!hLine.Exists(clusterGroup.Contains))
					{
						continue;
					}
					if (hLine.Count >= 5)
					{
						hasLine5 = true;
					}
					if (hLine.Count == 4)
					{
						hasHorizontalFour = true;
					}
					foreach (List<NormalElementView> vLine2 in vLines)
					{
						if (!vLine2.Exists(clusterGroup.Contains))
						{
							continue;
						}
						NormalElementView shared = hLine.Find(vLine2.Contains);
						if (!(shared != null))
						{
							continue;
						}
						if (!hasCrossingLines)
						{
							hasCrossingLines = true;
							crossingPoint = new Vector2Int(shared.GridX, shared.GridY);
						}
						break;
					}
				}
				foreach (List<NormalElementView> vLine in vLines)
				{
					if (vLine.Exists(clusterGroup.Contains))
					{
						if (vLine.Count >= 5)
						{
							hasLine5 = true;
						}
						if (vLine.Count == 4)
						{
							hasVerticalFour = true;
						}
					}
				}
				Vector2Int intersection = new Vector2Int(clusterGroup[0].GridX, clusterGroup[0].GridY);
				if (hasCrossingLines)
				{
					intersection = crossingPoint;
				}
				else
				{
					NormalElementView normA = swappedA as NormalElementView;
					NormalElementView normB = swappedB as NormalElementView;
					intersection = ((normA != null && clusterGroup.Contains(normA)) ? new Vector2Int(swappedA.GridX, swappedA.GridY) : ((!(normB != null) || !clusterGroup.Contains(normB)) ? new Vector2Int(minX + width / 2, minY + height / 2) : new Vector2Int(swappedB.GridX, swappedB.GridY)));
				}
				cluster.intersectionPoint = intersection;
				bool has2x2 = false;
				if (width >= 2 && height >= 2)
				{
					for (int x2 = minX; x2 < maxX; x2++)
					{
						int y;
						for (y = minY; y < maxY; y++)
						{
							if (clusterGroup.Exists((NormalElementView e) => e.GridX == x2 && e.GridY == y) && clusterGroup.Exists((NormalElementView e) => e.GridX == x2 + 1 && e.GridY == y) && clusterGroup.Exists((NormalElementView e) => e.GridX == x2 && e.GridY == y + 1) && clusterGroup.Exists((NormalElementView e) => e.GridX == x2 + 1 && e.GridY == y + 1))
							{
								has2x2 = true;
								break;
							}
						}
						if (has2x2)
						{
							break;
						}
					}
				}
				if (hasLine5)
				{
					cluster.shape = CreationCondition.Line5;
				}
				else if (hasCrossingLines)
				{
					cluster.shape = CreationCondition.CrossOrT;
				}
				else if (has2x2)
				{
					cluster.shape = CreationCondition.Square2x2;
				}
				else if (hasHorizontalFour)
				{
					cluster.shape = CreationCondition.Line4Vertical;
				}
				else if (hasVerticalFour)
				{
					cluster.shape = CreationCondition.Line4Horizontal;
				}
				finalClusters.Add(cluster);
			}
			return finalClusters;
		}

		public bool IsValidTutorialMove(Vector2Int from, Vector2Int to)
		{
			if (from.x < 0 || from.x >= currentLevel.width || from.y < 0 || from.y >= currentLevel.height)
			{
				return false;
			}
			if (to.x < 0 || to.x >= currentLevel.width || to.y < 0 || to.y >= currentLevel.height)
			{
				return false;
			}
			ElementView element = activeElements[from.x, from.y];
			if (element == null)
			{
				return false;
			}
			if (from == to && element is SpecialElementView)
			{
				return true;
			}
			ElementView targetElement = activeElements[to.x, to.y];
			if (targetElement == null)
			{
				return false;
			}
			if (from == to)
			{
				return false;
			}
			if (!IsElementSwappable(element) || !IsElementSwappable(targetElement))
			{
				return false;
			}
			if (element is SpecialElementView || targetElement is SpecialElementView)
			{
				return true;
			}
			return true;
		}

		private bool IsElementSwappable(ElementView element)
		{
			if (element == null)
			{
				return false;
			}
			if (element is RetractingCollectibleView)
			{
				return false;
			}
			if (element is ObstacleView obs && !obs.Data.isSwappable)
			{
				return false;
			}
			if (element is GeneratorView gen && !gen.Data.isSwappable)
			{
				return false;
			}
			if (element is MotherView mother && !mother.Data.isSwappable)
			{
				return false;
			}
			if (element is ChildView child && !child.Data.isSwappable)
			{
				return false;
			}
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
					if (element == null)
					{
						continue;
					}
					if (element is SpecialElementView)
					{
						from = new Vector2Int(x, y);
						to = new Vector2Int(x, y);
						return true;
					}
					if (!IsElementSwappable(element))
					{
						continue;
					}
					if (x < currentLevel.width - 1)
					{
						ElementView rightElement = activeElements[x + 1, y];
						if (rightElement != null && IsElementSwappable(rightElement))
						{
							activeElements[x, y] = rightElement;
							activeElements[x + 1, y] = element;
							bool hasMatch2 = FindAllMatchClusters().Count > 0;
							activeElements[x, y] = element;
							activeElements[x + 1, y] = rightElement;
							if (hasMatch2)
							{
								from = new Vector2Int(x, y);
								to = new Vector2Int(x + 1, y);
								return true;
							}
						}
					}
					if (y >= currentLevel.height - 1)
					{
						continue;
					}
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
			return false;
		}

		private bool HasPossibleMoves()
		{
			Vector2Int from;
			Vector2Int to;
			return GetPossibleMove(out from, out to);
		}

		private IEnumerator CheckAndShuffleIfNeeded()
		{
			int maxAttempts = 50;
			while (!HasPossibleMoves() && maxAttempts > 0)
			{
				Debug.Log("No possible moves. Shuffling board...");
				currentState = BoardState.Animating;
				yield return new WaitForSeconds(0.5f);
				List<NormalElementView> normals = new List<NormalElementView>();
				List<Vector2Int> positions = new List<Vector2Int>();
				for (int x2 = 0; x2 < currentLevel.width; x2++)
				{
					for (int y = 0; y < currentLevel.height; y++)
					{
						ElementView elementView = activeElements[x2, y];
						if (elementView is NormalElementView normal)
						{
							normals.Add(normal);
							positions.Add(new Vector2Int(x2, y));
						}
					}
				}
				bool validShuffle = false;
				for (int attempt = 0; attempt < 100; attempt++)
				{
					for (int j = 0; j < positions.Count; j++)
					{
						int randomIndex = UnityEngine.Random.Range(j, positions.Count);
						Vector2Int temp = positions[j];
						positions[j] = positions[randomIndex];
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
					for (int y2 = 0; y2 < currentLevel.height; y2++)
					{
						ElementView elementView = activeElements[x, y2];
						if (elementView is NormalElementView normal2)
						{
							normal2.MoveTo(x, y2, GetWorldPosition(x, y2), 0.5f);
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
				yield break;
			}
			currentState = BoardState.Waiting;
			if (gameManager != null)
			{
				gameManager.OnBoardSettled();
			}
		}

		private IEnumerator ProcessMatchesAndRefill(List<MatchCluster> clusters)
		{
			retractedThisTurn.Clear();
			directlyHitCollectiblesThisTurn.Clear();
			damagedObstaclesThisTurn.Clear();
			triggeredGeneratorsThisTurn.Clear();
			List<ElementView> elementsToDestroy = new List<ElementView>();
			List<SpecialSpawnInfo> specialsToSpawn = new List<SpecialSpawnInfo>();
			foreach (MatchCluster cluster in clusters)
			{
				foreach (NormalElementView el in cluster.elements)
				{
					if (!elementsToDestroy.Contains(el))
					{
						elementsToDestroy.Add(el);
					}
				}
				if (cluster.shape == CreationCondition.None)
				{
					continue;
				}
				SpecialElementData specialDataToSpawn = null;
				foreach (SpecialElementData spec in elementDb.specialElements)
				{
					if (spec.creationCondition == cluster.shape)
					{
						specialDataToSpawn = spec;
						break;
					}
				}
				if (specialDataToSpawn != null)
				{
					specialsToSpawn.Add(new SpecialSpawnInfo
					{
						x = cluster.intersectionPoint.x,
						y = cluster.intersectionPoint.y,
						data = specialDataToSpawn
					});
				}
				else
				{
					Debug.LogWarning($"Could not find SpecialElementData for CreationCondition: {cluster.shape}. Make sure it is added to the ElementDatabase!");
				}
			}
			if (elementsToDestroy.Count > 0 && AudioManager.Instance != null)
			{
				AudioManager.Instance.PlayNormalMatchSound();
			}
			foreach (ElementView match in elementsToDestroy)
			{
				int mx = match.GridX;
				int my = match.GridY;
				int targetIndex = -1;
				if (activeElements[mx, my] == match)
				{
					if (match is NormalElementView normal && gameManager != null)
					{
						targetIndex = gameManager.OnElementCollected(CellItemType.SpecificNormal, normal.NormalElementId);
					}
					activeElements[mx, my] = null;
					CheckAdjacentCollectibles(mx, my);
				}
				DestroyElementVisual(match, targetIndex);
			}
			yield return StartCoroutine(WaitForElementDestroys());
			if (specialsToSpawn.Count > 0 && AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySpecialGenerateSound();
			}
			foreach (SpecialSpawnInfo spawn in specialsToSpawn)
			{
				if (!(spawn.data.prefab != null))
				{
					continue;
				}
				ElementView residual = activeElements[spawn.x, spawn.y];
				if (residual != null)
				{
					activeElements[spawn.x, spawn.y] = null;
					if (residual is RetractingCollectibleView || residual is MotherView)
					{
						Debug.LogWarning($"[BoardManager] 特殊元素生成格({spawn.x},{spawn.y})被多格元素占用：{residual.name}，仅移除引用、不销毁其 GameObject");
					}
					else if (residual.gameObject != null)
					{
						Debug.LogWarning($"[BoardManager] 特殊元素生成前清理目标格({spawn.x},{spawn.y})上的残留元素：{residual.name}");
						UnityEngine.Object.Destroy(residual.gameObject);
					}
				}
				GameObject newSpecObj = UnityEngine.Object.Instantiate(spawn.data.prefab, elementsContainer);
				SpecialElementView specialView = newSpecObj.GetComponent<SpecialElementView>();
				if (specialView != null)
				{
					specialView.Initialize(spawn.data, spawn.x, spawn.y, GetWorldPosition(spawn.x, spawn.y), cellSize);
					activeElements[spawn.x, spawn.y] = specialView;
					Vector3 targetScale = newSpecObj.transform.localScale;
					newSpecObj.transform.localScale = Vector3.zero;
					newSpecObj.transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
				}
			}
			yield return StartCoroutine(ApplyGravityAndRefill());
		}

		private bool CanFall(ElementView elementToFall)
		{
			if (elementToFall == null)
			{
				return false;
			}
			if (elementToFall is RetractingCollectibleView)
			{
				return false;
			}
			if (elementToFall is ObstacleView obs && !obs.Data.isMovable)
			{
				return false;
			}
			if (elementToFall is GeneratorView gen && !gen.Data.isMovable)
			{
				return false;
			}
			if (elementToFall is MotherView mother && !mother.Data.isMovable)
			{
				return false;
			}
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
				if (info != null)
				{
					validCount++;
				}
			}
			if (validCount == 0)
			{
				Debug.LogError("Cannot refill board: ElementDatabase contains no valid normal elements.");
				return null;
			}
			int selectedValidIndex = UnityEngine.Random.Range(0, validCount);
			foreach (NormalElementInfo info2 in elementDb.normalElements)
			{
				if (info2 == null || selectedValidIndex-- != 0)
				{
					continue;
				}
				return info2;
			}
			return null;
		}

		private NormalElementInfo GetNextRefillElement(int column)
		{
			if (currentLevel == null || column < 0 || column >= currentLevel.width || refillSequenceIndices == null || column >= refillSequenceIndices.Length)
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
				NormalElementInfo info = ((elementDb != null) ? elementDb.GetNormalElement(configuredId) : null);
				if (info != null)
				{
					return info;
				}
				string warningKey = column + ":" + configuredIndex;
				if (refillWarnings.Add(warningKey))
				{
					Debug.LogWarning($"Invalid refill element ID {configuredId} at column {column}, sequence index {configuredIndex}.");
				}
			}
			return GetRandomNormalElement();
		}

		private IEnumerator WaitForCollectibleRetracts()
		{
			for (float elapsed = 0f; elapsed < 1f; elapsed += Time.deltaTime)
			{
				bool anyAnimating = false;
				for (int x = 0; x < currentLevel.width; x++)
				{
					if (anyAnimating)
					{
						break;
					}
					for (int y = 0; y < currentLevel.height; y++)
					{
						if (anyAnimating)
						{
							break;
						}
						ElementView elementView = activeElements[x, y];
						if (elementView is RetractingCollectibleView rc && rc.IsRetracting)
						{
							anyAnimating = true;
							break;
						}
					}
				}
				if (!anyAnimating)
				{
					break;
				}
				yield return null;
			}
		}

		private IEnumerator WaitForElementDestroys()
		{
			for (float elapsed = 0f; elapsed < 1.5f; elapsed += Time.deltaTime)
			{
				pendingElementDestroys.RemoveWhere((ElementView el) => el == null);
				if (pendingElementDestroys.Count == 0)
				{
					yield break;
				}
				yield return null;
			}
			pendingElementDestroys.Clear();
		}

		private void StartTrackedSpecial(Func<IEnumerator> activation)
		{
			activeSpecialChains++;
			StartCoroutine(TrackedSpecialRoutine(activation));
		}

		private IEnumerator TrackedSpecialRoutine(Func<IEnumerator> activation)
		{
			IEnumerator inner = activation();
			while (inner.MoveNext())
			{
				yield return inner.Current;
			}
			activeSpecialChains--;
		}

		private void EnsureMagicFocusMask()
		{
			if (!(magicFocusMask != null) || !(magicFocusMask.gameObject != null))
			{
				GameObject maskObj = new GameObject("MagicBallFocusMask");
				maskObj.transform.SetParent(base.transform);
				maskObj.transform.position = new Vector3(0f, 0f, -1f);
				magicFocusMask = maskObj.AddComponent<SpriteRenderer>();
				Texture2D tex = Texture2D.whiteTexture;
				Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
				magicFocusMask.sprite = sprite;
				magicFocusMask.color = new Color(0f, 0f, 0f, 0.7f);
				magicFocusMask.sortingOrder = 25000;
				float height = Camera.main.orthographicSize * 2f;
				float width = height * Camera.main.aspect;
				float maxDim = Mathf.Max(width, height) * 100f * 2f;
				maskObj.transform.localScale = new Vector3(maxDim, maxDim, 1f);
				maskObj.SetActive(false);
			}
		}

		private void ShowMagicBallFocus(GameObject focusObject)
		{
			if (!(focusObject == null))
			{
				EnsureMagicFocusMask();
				if (!(magicFocusMask == null))
				{
					magicFocusMaskRefCount++;
					magicFocusMask.gameObject.SetActive(true);
					RaiseRenderersAboveFocus(focusObject);
				}
			}
		}

		private void RaiseRenderersAboveFocus(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
			Renderer[] array = renderers;
			foreach (Renderer r in array)
			{
				if (!(r == null) && !raisedFocusRenderers.Contains(r))
				{
					raisedFocusRenderers.Add(r);
					raisedFocusOriginalOrders.Add(r.sortingOrder);
					r.sortingOrder = 25002;
				}
			}
		}

		private void HideMagicBallFocus()
		{
			if (magicFocusMaskRefCount > 0)
			{
				magicFocusMaskRefCount--;
				if (magicFocusMaskRefCount > 0)
				{
					return;
				}
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

		public void ClearMagicFocusMask()
		{
			magicFocusMaskRefCount = 0;
			HideMagicBallFocus();
		}

		private void PlayMagicBallFinishEffect(SpecialElementView ball, Vector3 worldPos)
		{
			if (!(ball == null) && !(ball.Data == null))
			{
				GameObject prefab = ((ball.Data.finishEffectPrefab != null) ? ball.Data.finishEffectPrefab : ball.Data.activationEffectPrefab);
				if (!(prefab == null))
				{
					Match3EffectPlayer.Play(prefab, worldPos, base.transform);
				}
			}
		}

		private IEnumerator ConvertedSpecialTriggerRoutine(SpecialElementView special, float creationDelay)
		{
			if (!(special == null) && !(special.gameObject == null))
			{
				yield return new WaitForSeconds(creationDelay);
				if (!(special == null) && !(special.gameObject == null) && activeElements[special.GridX, special.GridY] == special)
				{
					yield return StartCoroutine(ActivateSpecial(special, null));
				}
			}
		}

		private IEnumerator WaitForActiveSpecialChains()
		{
			float elapsed = 0f;
			while (activeSpecialChains > 0 && elapsed < 5f)
			{
				yield return null;
				elapsed += Time.deltaTime;
			}
			if (activeSpecialChains > 0)
			{
				Debug.LogWarning($"[BoardManager] 等待链式特效超时({5f}s)，仍有 {activeSpecialChains} 个特效链未结束，强制执行重力。");
			}
		}

		private IEnumerator ApplyGravityAndRefill()
		{
			yield return StartCoroutine(WaitForActiveSpecialChains());
			yield return StartCoroutine(WaitForCollectibleRetracts());
			yield return StartCoroutine(WaitForElementDestroys());
			bool needsRefill = false;
			bool boardChanged = true;
			HashSet<ElementView> movedElements = new HashSet<ElementView>();
			List<ElementView> newlySpawnedElements = new List<ElementView>();
			int[] spawnCountPerColumn = new int[currentLevel.width];
			while (boardChanged)
			{
				boardChanged = false;
				bool verticalChanged = true;
				while (verticalChanged)
				{
					verticalChanged = false;
					for (int y = 0; y < currentLevel.height; y++)
					{
						for (int x = 0; x < currentLevel.width; x++)
						{
							if (IsValidGridPos(x, y) && !currentLevel.GetCell(x, y).hasConveyor && activeElements[x, y] == null && IsValidGridPos(x, y + 1) && activeElements[x, y + 1] != null && CanFall(activeElements[x, y + 1]) && !currentLevel.GetCell(x, y + 1).hasConveyor)
							{
								ElementView el = activeElements[x, y + 1];
								activeElements[x, y] = el;
								activeElements[x, y + 1] = null;
								if (!newlySpawnedElements.Contains(el))
								{
									movedElements.Add(el);
								}
								verticalChanged = true;
								boardChanged = true;
							}
						}
					}
				}
				for (int y4 = 0; y4 < currentLevel.height; y4++)
				{
					for (int x2 = 0; x2 < currentLevel.width; x2++)
					{
						if (!IsValidGridPos(x2, y4) || currentLevel.GetCell(x2, y4).hasConveyor || !(activeElements[x2, y4] == null))
						{
							continue;
						}
						bool isBlockedFromSky = false;
						for (int aboveY = y4 + 1; aboveY < currentLevel.height; aboveY++)
						{
							if (!IsValidGridPos(x2, aboveY))
							{
								isBlockedFromSky = true;
								break;
							}
							if (currentLevel.GetCell(x2, aboveY).hasConveyor)
							{
								isBlockedFromSky = true;
								break;
							}
							if (activeElements[x2, aboveY] != null)
							{
								isBlockedFromSky = ((!CanFall(activeElements[x2, aboveY])) ? true : false);
								break;
							}
						}
						if (!isBlockedFromSky)
						{
							continue;
						}
						bool canPullLeft = IsValidGridPos(x2 - 1, y4 + 1) && !currentLevel.GetCell(x2 - 1, y4 + 1).hasConveyor && activeElements[x2 - 1, y4 + 1] != null && CanFall(activeElements[x2 - 1, y4 + 1]) && (!IsValidGridPos(x2 - 1, y4) || currentLevel.GetCell(x2 - 1, y4).hasConveyor || activeElements[x2 - 1, y4] == null || CanFall(activeElements[x2 - 1, y4]));
						bool canPullRight = IsValidGridPos(x2 + 1, y4 + 1) && !currentLevel.GetCell(x2 + 1, y4 + 1).hasConveyor && activeElements[x2 + 1, y4 + 1] != null && CanFall(activeElements[x2 + 1, y4 + 1]) && (!IsValidGridPos(x2 + 1, y4) || currentLevel.GetCell(x2 + 1, y4).hasConveyor || activeElements[x2 + 1, y4] == null || CanFall(activeElements[x2 + 1, y4]));
						if (canPullLeft && canPullRight)
						{
							if (UnityEngine.Random.value > 0.5f)
							{
								ElementView el5 = activeElements[x2 - 1, y4 + 1];
								activeElements[x2, y4] = el5;
								activeElements[x2 - 1, y4 + 1] = null;
								if (!newlySpawnedElements.Contains(el5))
								{
									movedElements.Add(el5);
								}
							}
							else
							{
								ElementView el4 = activeElements[x2 + 1, y4 + 1];
								activeElements[x2, y4] = el4;
								activeElements[x2 + 1, y4 + 1] = null;
								if (!newlySpawnedElements.Contains(el4))
								{
									movedElements.Add(el4);
								}
							}
							boardChanged = true;
						}
						else if (canPullLeft)
						{
							ElementView el3 = activeElements[x2 - 1, y4 + 1];
							activeElements[x2, y4] = el3;
							activeElements[x2 - 1, y4 + 1] = null;
							if (!newlySpawnedElements.Contains(el3))
							{
								movedElements.Add(el3);
							}
							boardChanged = true;
						}
						else if (canPullRight)
						{
							ElementView el2 = activeElements[x2 + 1, y4 + 1];
							activeElements[x2, y4] = el2;
							activeElements[x2 + 1, y4 + 1] = null;
							if (!newlySpawnedElements.Contains(el2))
							{
								movedElements.Add(el2);
							}
							boardChanged = true;
						}
					}
				}
				for (int x3 = 0; x3 < currentLevel.width; x3++)
				{
					int topY = -1;
					for (int y3 = currentLevel.height - 1; y3 >= 0; y3--)
					{
						if (IsValidGridPos(x3, y3) && !currentLevel.GetCell(x3, y3).hasConveyor)
						{
							topY = y3;
							break;
						}
					}
					if (topY == -1 || !(activeElements[x3, topY] == null))
					{
						continue;
					}
					bool isBlocked = false;
					for (int y2 = topY + 1; y2 < currentLevel.height; y2++)
					{
						if (IsValidGridPos(x3, y2) && currentLevel.GetCell(x3, y2).hasConveyor)
						{
							isBlocked = true;
							break;
						}
					}
					if (isBlocked)
					{
						continue;
					}
					if (elementDb == null || elementDb.unifiedNormalPrefab == null)
					{
						Debug.LogError("Cannot refill board: normal element prefab is missing.");
						continue;
					}
					NormalElementInfo normInfo = GetNextRefillElement(x3);
					if (normInfo != null)
					{
						GameObject newElementObj = UnityEngine.Object.Instantiate(elementDb.unifiedNormalPrefab, elementsContainer);
						NormalElementView normalView = newElementObj.GetComponent<NormalElementView>();
						if (normalView == null)
						{
							Debug.LogError("Unified normal prefab is missing NormalElementView.");
							UnityEngine.Object.Destroy(newElementObj);
							continue;
						}
						spawnCountPerColumn[x3]++;
						Vector3 spawnPos = GetWorldPosition(x3, currentLevel.height + spawnCountPerColumn[x3]);
						normalView.Initialize(normInfo, x3, topY, spawnPos, cellSize);
						HideRefillUntilInsideBoard(normalView);
						activeElements[x3, topY] = normalView;
						newlySpawnedElements.Add(normalView);
						boardChanged = true;
					}
				}
			}
			foreach (ElementView el6 in movedElements)
			{
				int newX2 = currentLevel.width;
				int newY2 = currentLevel.height;
				bool found2 = false;
				for (int y6 = 0; y6 < currentLevel.height; y6++)
				{
					for (int x5 = 0; x5 < currentLevel.width; x5++)
					{
						if (activeElements[x5, y6] == el6)
						{
							if (x5 < newX2)
							{
								newX2 = x5;
							}
							if (y6 < newY2)
							{
								newY2 = y6;
							}
							found2 = true;
						}
					}
				}
				if (found2)
				{
					el6.MoveTo(newX2, newY2, GetWorldPosition(newX2, newY2));
					needsRefill = true;
				}
			}
			foreach (ElementView el7 in newlySpawnedElements)
			{
				int newX = currentLevel.width;
				int newY = currentLevel.height;
				bool found = false;
				for (int y5 = 0; y5 < currentLevel.height; y5++)
				{
					for (int x4 = 0; x4 < currentLevel.width; x4++)
					{
						if (activeElements[x4, y5] == el7)
						{
							if (x4 < newX)
							{
								newX = x4;
							}
							if (y5 < newY)
							{
								newY = y5;
							}
							found = true;
						}
					}
				}
				if (found)
				{
					el7.MoveTo(newX, newY, GetWorldPosition(newX, newY));
					needsRefill = true;
				}
			}
			if (needsRefill)
			{
				yield return new WaitForSeconds(0.3f);
			}
			List<MatchCluster> newClusters = FindAllMatchClusters();
			if (newClusters.Count > 0)
			{
				yield return StartCoroutine(ProcessMatchesAndRefill(newClusters));
				yield break;
			}
			if (!hasConveyorMovedThisTurn)
			{
				hasConveyorMovedThisTurn = true;
				bool moved = false;
				yield return StartCoroutine(ProcessConveyors(delegate(bool result)
				{
					moved = result;
				}));
				if (moved)
				{
					yield return StartCoroutine(ApplyGravityAndRefill());
					yield break;
				}
			}
			if (!HasPossibleMoves())
			{
				yield return StartCoroutine(CheckAndShuffleIfNeeded());
				yield break;
			}
			currentState = BoardState.Waiting;
			if (gameManager != null)
			{
				gameManager.OnBoardSettled();
			}
		}

		private void HideRefillUntilInsideBoard(NormalElementView normalView)
		{
			if (normalView == null || currentLevel == null)
			{
				return;
			}
			SpriteRenderer[] renderers = normalView.GetComponentsInChildren<SpriteRenderer>(true);
			float visualHalfHeight = cellSize * 0.475f;
			SpriteRenderer[] array = renderers;
			foreach (SpriteRenderer renderer in array)
			{
				renderer.maskInteraction = SpriteMaskInteraction.None;
				if (renderer.bounds.extents.y > 0f)
				{
					visualHalfHeight = Mathf.Max(visualHalfHeight, renderer.bounds.extents.y);
				}
				renderer.enabled = false;
			}
			float boardTop = boardBottomLeftOrigin.y + (float)(currentLevel.height - 1) * (cellSize + cellSpacing) + cellSize * 0.5f;
			StartCoroutine(RevealRefillWhenInside(normalView, renderers, boardTop - visualHalfHeight));
		}

		private IEnumerator RevealRefillWhenInside(NormalElementView normalView, SpriteRenderer[] renderers, float revealCenterY)
		{
			while (normalView != null && normalView.transform.position.y > revealCenterY)
			{
				yield return null;
			}
			if (normalView == null)
			{
				yield break;
			}
			foreach (SpriteRenderer renderer in renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = true;
				}
			}
		}

		private IEnumerator ProcessConveyors(Action<bool> onComplete)
		{
			SkeletonAnimation[] spineAnims = backgroundContainer.GetComponentsInChildren<SkeletonAnimation>();
			SkeletonAnimation[] array = spineAnims;
			foreach (SkeletonAnimation anim in array)
			{
				anim.timeScale = 1f;
				if (anim.AnimationState != null && anim.AnimationState.GetCurrent(0) != null)
				{
					anim.AnimationState.GetCurrent(0).TrackTime = 0f;
				}
			}
			List<ElementView> movingElements = new List<ElementView>();
			Dictionary<ElementView, Vector2Int> targetPositions = new Dictionary<ElementView, Vector2Int>();
			Dictionary<ElementView, bool> canMove = new Dictionary<ElementView, bool>();
			for (int x = 0; x < currentLevel.width; x++)
			{
				for (int y = 0; y < currentLevel.height; y++)
				{
					GridCellData cell = currentLevel.GetCell(x, y);
					if (cell == null || !cell.isPlayable || !cell.hasConveyor)
					{
						continue;
					}
					ElementView el = activeElements[x, y];
					if (el != null && !(el is MotherView) && !(el is ChildView) && !(el is RetractingCollectibleView))
					{
						Vector2Int targetPos = new Vector2Int(x, y);
						switch (cell.conveyorDirection)
						{
						case Direction.Up:
							targetPos.y++;
							break;
						case Direction.Down:
							targetPos.y--;
							break;
						case Direction.Left:
							targetPos.x--;
							break;
						case Direction.Right:
							targetPos.x++;
							break;
						}
						movingElements.Add(el);
						targetPositions[el] = targetPos;
						canMove[el] = true;
					}
				}
			}
			if (movingElements.Count == 0)
			{
				SkeletonAnimation[] array2 = spineAnims;
				foreach (SkeletonAnimation anim2 in array2)
				{
					if (anim2 != null)
					{
						anim2.timeScale = 0f;
					}
				}
				onComplete?.Invoke(false);
				yield break;
			}
			bool changed = true;
			while (changed)
			{
				changed = false;
				foreach (ElementView el2 in movingElements)
				{
					if (!canMove[el2])
					{
						continue;
					}
					Vector2Int tPos = targetPositions[el2];
					if (tPos.x < 0 || tPos.x >= currentLevel.width || tPos.y < 0 || tPos.y >= currentLevel.height)
					{
						canMove[el2] = false;
						changed = true;
						continue;
					}
					GridCellData targetCell = currentLevel.GetCell(tPos.x, tPos.y);
					if (targetCell == null || !targetCell.isPlayable)
					{
						canMove[el2] = false;
						changed = true;
						continue;
					}
					ElementView elAtTarget = activeElements[tPos.x, tPos.y];
					if (elAtTarget != null)
					{
						if (!movingElements.Contains(elAtTarget))
						{
							canMove[el2] = false;
							changed = true;
						}
						else if (!canMove[elAtTarget])
						{
							canMove[el2] = false;
							changed = true;
						}
					}
					foreach (ElementView otherEl in movingElements)
					{
						if (otherEl != el2 && canMove[otherEl] && targetPositions[otherEl] == tPos && otherEl.GetInstanceID() < el2.GetInstanceID())
						{
							canMove[el2] = false;
							changed = true;
							break;
						}
					}
				}
			}
			bool anyMoved = false;
			foreach (ElementView el4 in movingElements)
			{
				if (canMove[el4])
				{
					activeElements[el4.GridX, el4.GridY] = null;
				}
			}
			foreach (ElementView el3 in movingElements)
			{
				if (canMove[el3])
				{
					Vector2Int tPos2 = targetPositions[el3];
					activeElements[tPos2.x, tPos2.y] = el3;
					el3.MoveTo(tPos2.x, tPos2.y, GetWorldPosition(tPos2.x, tPos2.y));
					anyMoved = true;
				}
			}
			if (anyMoved)
			{
				yield return new WaitForSeconds(0.3f);
			}
			SkeletonAnimation[] array3 = spineAnims;
			foreach (SkeletonAnimation anim3 in array3)
			{
				if (anim3 != null)
				{
					anim3.timeScale = 0f;
				}
			}
			onComplete?.Invoke(anyMoved);
		}

		private IEnumerator ActivateSpecial(SpecialElementView special, ElementView swappedWith, bool isChainReaction = false)
		{
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySpecialTriggerSound(special.BehaviorType);
			}
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
			if (el == null)
			{
				return;
			}
			el.transform.DOKill();
			if (el is NormalElementView normal)
			{
				NormalElementInfo normInfo = elementDb.GetNormalElement(normal.NormalElementId);
				if (normInfo != null)
				{
					if (normInfo.clearEffectPrefab != null && !normal.clearEffectAlreadyPlayed)
					{
						SpawnNormalClearEffect(normal);
					}
					if (normInfo.lightWaveEffectPrefab != null || elementDb.globalLightWaveEffectPrefab != null)
					{
						GameObject prefabToInstantiate = ((normInfo.lightWaveEffectPrefab != null) ? normInfo.lightWaveEffectPrefab : elementDb.globalLightWaveEffectPrefab);
						Match3EffectPlayer.Play(prefabToInstantiate, el.transform.position, elementsContainer, cellSize * normInfo.lightWaveEffectScale);
					}
				}
			}
			if (el is SpecialElementView special)
			{
				float duration = special.GetTriggerAnimationDuration();
				if (duration > 0f && !special.swapAnimationPlayed)
				{
					pendingElementDestroys.Add(el);
					special.PlayTriggerAnimation();
					UnityEngine.Object.Destroy(el.gameObject, duration);
					return;
				}
			}
			if (targetIndex >= 0 && gameManager != null && gameManager.uiManager != null)
			{
				Vector3 uiPos = gameManager.uiManager.GetTargetWorldPosition(targetIndex);
				if (uiPos != Vector3.zero)
				{
					SpriteRenderer[] srList = el.GetComponentsInChildren<SpriteRenderer>(true);
					SpriteRenderer[] array = srList;
					foreach (SpriteRenderer sr in array)
					{
						sr.sortingOrder = 100;
					}
					MeshRenderer[] mrList = el.GetComponentsInChildren<MeshRenderer>(true);
					MeshRenderer[] array2 = mrList;
					foreach (MeshRenderer mr in array2)
					{
						mr.sortingOrder = 100;
					}
					if (el is MotherView)
					{
						for (int i = 0; i < el.transform.childCount; i++)
						{
							el.transform.GetChild(i).DOLocalMove(Vector3.zero, 0.6f);
						}
					}
					Vector3 startPos = el.transform.position;
					Vector3 midPoint = startPos + (uiPos - startPos) / 2f + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(1f, 3f), 0f);
					Vector3[] path = new Vector3[2] { midPoint, uiPos };
					float flyDuration = ((el is RetractingCollectibleView) ? 0.35f : 0.6f);
					pendingElementDestroys.Add(el);
					el.transform.DOPath(path, flyDuration, PathType.CatmullRom).SetEase(Ease.InOutQuad).OnComplete(delegate
					{
						pendingElementDestroys.Remove(el);
						if (AudioManager.Instance != null)
						{
							AudioManager.Instance.PlayCollectSound();
						}
						if (el != null && el.gameObject != null)
						{
							UnityEngine.Object.Destroy(el.gameObject);
						}
					})
						.SetLink(el.gameObject);
					el.transform.DOScale(el.transform.localScale * 0.5f, flyDuration).SetLink(el.gameObject);
					return;
				}
			}
			pendingElementDestroys.Add(el);
			el.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(delegate
			{
				pendingElementDestroys.Remove(el);
				if (el != null && el.gameObject != null)
				{
					UnityEngine.Object.Destroy(el.gameObject);
				}
			})
				.SetLink(el.gameObject);
		}

		private void SpawnNormalClearEffect(NormalElementView normal)
		{
			if (!(normal == null) && !(normal.gameObject == null))
			{
				NormalElementInfo normInfo = elementDb.GetNormalElement(normal.NormalElementId);
				if (normInfo != null && !(normInfo.clearEffectPrefab == null))
				{
					Match3EffectPlayer.Play(normInfo.clearEffectPrefab, normal.transform.position, elementsContainer, cellSize * normInfo.clearEffectScale);
				}
			}
		}

		private void DestroyElements(List<ElementView> elements)
		{
			foreach (ElementView el in elements)
			{
				if (!(el != null) || !(activeElements[el.GridX, el.GridY] == el))
				{
					continue;
				}
				if (el is MotherView)
				{
					continue;
				}
				if (el is ChildView child)
				{
					ProcessChildCollection(child);
					continue;
				}
				if (el is RetractingCollectibleView collectible)
				{
					TryRetractCollectible(collectible, directlyHitCollectiblesThisTurn);
					continue;
				}
				if (el is ObstacleView obstacle)
				{
					if (damagedObstaclesThisTurn.Contains(obstacle))
					{
						continue;
					}
					damagedObstaclesThisTurn.Add(obstacle);
					obstacle.TakeDamage();
					if (obstacle.CurrentHealth <= 0)
					{
						activeElements[el.GridX, el.GridY] = null;
						obstacle.PlayCollectEffect();
						int tIndex2 = -1;
						if (gameManager != null)
						{
							tIndex2 = gameManager.OnElementCollected(CellItemType.Obstacle, -1, null, null, obstacle.Data);
						}
						DestroyElementVisual(obstacle, tIndex2);
					}
					continue;
				}
				if (el is GeneratorView generator)
				{
					if (!triggeredGeneratorsThisTurn.Contains(generator))
					{
						triggeredGeneratorsThisTurn.Add(generator);
						int tIndex = -1;
						if (gameManager != null)
						{
							tIndex = gameManager.OnElementCollected(CellItemType.Generator, -1, null, null, null, generator.Data);
						}
						Vector3 uiPos = Vector3.zero;
						if (tIndex >= 0 && gameManager != null && gameManager.uiManager != null)
						{
							uiPos = gameManager.uiManager.GetTargetWorldPosition(tIndex);
						}
						generator.TriggerGeneration(tIndex, uiPos);
					}
					continue;
				}
				int targetIndex = -1;
				SpecialElementView spec = el as SpecialElementView;
				if ((object)spec != null)
				{
					if (!currentlyTriggeringSpecials.Contains(spec))
					{
						currentlyTriggeringSpecials.Add(spec);
						if (gameManager != null)
						{
							targetIndex = gameManager.OnElementCollected(CellItemType.Special, -1, spec.Data);
						}
						StartTrackedSpecial(() => ActivateSpecial(spec, null, true));
						CheckAdjacentCollectibles(el.GridX, el.GridY, true);
					}
				}
				else
				{
					activeElements[el.GridX, el.GridY] = null;
					if (el is NormalElementView normal && gameManager != null)
					{
						targetIndex = gameManager.OnElementCollected(CellItemType.SpecificNormal, normal.NormalElementId);
					}
					CheckAdjacentCollectibles(el.GridX, el.GridY, true);
					DestroyElementVisual(el, targetIndex);
				}
			}
		}

		private IEnumerator ActivateColorClear(SpecialElementView magicBall, ElementView targetElement, bool isChainReaction = false)
		{
			bool shouldWait = isChainReaction && isBoardClearing;
			if (!shouldWait && activeElements[magicBall.GridX, magicBall.GridY] == magicBall)
			{
				activeElements[magicBall.GridX, magicBall.GridY] = null;
			}
			magicBall.PlayTriggerAnimation(true);
			if (shouldWait)
			{
				while (isBoardClearing)
				{
					yield return null;
				}
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
				List<int> allNormals = new List<int>();
				for (int x2 = 0; x2 < currentLevel.width; x2++)
				{
					for (int y = 0; y < currentLevel.height; y++)
					{
						ElementView elementView = activeElements[x2, y];
						if (elementView is NormalElementView i && !allNormals.Contains(i.NormalElementId))
						{
							allNormals.Add(i.NormalElementId);
						}
					}
				}
				List<int> availableNormals = new List<int>(allNormals);
				availableNormals.RemoveAll((int c) => pickedColorsThisTurn.Contains(c));
				if (availableNormals.Count > 0)
				{
					targetId = availableNormals[UnityEngine.Random.Range(0, availableNormals.Count)];
				}
				else if (allNormals.Count > 0)
				{
					targetId = allNormals[UnityEngine.Random.Range(0, allNormals.Count)];
				}
			}
			if (targetId != -1)
			{
				pickedColorsThisTurn.Add(targetId);
				List<ElementView> toDestroy = new List<ElementView>();
				for (int x3 = 0; x3 < currentLevel.width; x3++)
				{
					for (int y2 = 0; y2 < currentLevel.height; y2++)
					{
						ElementView elementView = activeElements[x3, y2];
						NormalElementView j = elementView as NormalElementView;
						if ((object)j != null && j.NormalElementId == targetId)
						{
							toDestroy.Add(j);
						}
					}
				}
				GameObject trailPrefab = magicBall.Data?.trailEffectPrefab;
				float flyTime = 0.5f;
				Vector3 magicBallStartPos = GetWorldPosition(magicBall.GridX, magicBall.GridY);
				toDestroy.Sort(delegate(ElementView a, ElementView b)
				{
					float num = Vector3.Distance(magicBallStartPos, a.transform.position);
					float value = Vector3.Distance(magicBallStartPos, b.transform.position);
					return num.CompareTo(value);
				});
				foreach (ElementView target in toDestroy)
				{
					if (target == null || target.gameObject == null)
					{
						continue;
					}
					if (target is NormalElementView normalTarget)
					{
						normalTarget.PlayChangeAnimation();
					}
					if (trailPrefab != null)
					{
						Vector3 targetPos = target.transform.position;
						Vector3 dir = (targetPos - magicBallStartPos).normalized;
						GameObject trail = UnityEngine.Object.Instantiate(trailPrefab, magicBallStartPos, Quaternion.identity);
						float angle = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
						trail.transform.rotation = Quaternion.Euler(0f, 0f, angle);
						trail.transform.DOMove(targetPos, flyTime).SetEase(Ease.Linear).OnComplete(delegate
						{
							if (target != null && target.gameObject != null && target is NormalElementView normalElementView)
							{
								normalElementView.clearEffectAlreadyPlayed = true;
								SpawnNormalClearEffect(normalElementView);
							}
							if (trail != null)
							{
								UnityEngine.Object.Destroy(trail);
							}
						});
						UnityEngine.Object.Destroy(trail, flyTime + 0.5f);
					}
					yield return new WaitForSeconds(0.05f);
				}
				yield return new WaitForSeconds(flyTime);
				List<ElementView> validToDestroy = new List<ElementView>();
				foreach (ElementView target2 in toDestroy)
				{
					if (target2 != null && target2.gameObject != null)
					{
						validToDestroy.Add(target2);
					}
				}
				DestroyElements(validToDestroy);
			}
			else
			{
				yield return new WaitForSeconds(0.3f);
			}
			float fadeOutDuration = 0.3f;
			magicBall.transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack);
			if (magicBall.spineAnimation != null && magicBall.spineAnimation.skeleton != null)
			{
				DOTween.To(() => magicBall.spineAnimation.skeleton.A, delegate(float x)
				{
					magicBall.spineAnimation.skeleton.A = x;
				}, 0f, fadeOutDuration);
			}
			else if (magicBall.spriteRenderer != null)
			{
				magicBall.spriteRenderer.DOFade(0f, fadeOutDuration);
			}
			yield return new WaitForSeconds(fadeOutDuration);
			if (magicBall != null && magicBall.gameObject != null)
			{
				UnityEngine.Object.Destroy(magicBall.gameObject);
			}
		}

		private IEnumerator ActivateLineClear(SpecialElementView special)
		{
			int cx = special.GridX;
			int cy = special.GridY;
			bool isVertical = special.BehaviorType == SpecialBehaviorType.LineClearVertical;
			if (activeElements[cx, cy] == special)
			{
				activeElements[cx, cy] = null;
			}
			GameObject rocket1 = special.gameObject;
			GameObject rocket2 = UnityEngine.Object.Instantiate(rocket1, rocket1.transform.parent);
			SpecialElementView special2 = rocket2.GetComponent<SpecialElementView>();
			special.PlayTriggerAnimation();
			if (special2 != null)
			{
				special2.PlayTriggerAnimation();
			}
			Vector3 moveDir1 = (isVertical ? Vector3.up : Vector3.left);
			Vector3 moveDir2 = (isVertical ? Vector3.down : Vector3.right);
			GameObject trailPrefab = special.Data?.trailEffectPrefab;
			if (trailPrefab != null)
			{
				GameObject trail1 = UnityEngine.Object.Instantiate(trailPrefab, rocket1.transform.position, Quaternion.identity, rocket1.transform);
				float angle1 = Mathf.Atan2(moveDir1.y, moveDir1.x) * 57.29578f;
				trail1.transform.rotation = Quaternion.Euler(0f, 0f, angle1);
				GameObject trail2 = UnityEngine.Object.Instantiate(trailPrefab, rocket2.transform.position, Quaternion.identity, rocket2.transform);
				float angle2 = Mathf.Atan2(moveDir2.y, moveDir2.x) * 57.29578f;
				trail2.transform.rotation = Quaternion.Euler(0f, 0f, angle2);
			}
			float speed = 15f;
			float duration = 1.5f;
			rocket1.transform.DOMove(rocket1.transform.position + moveDir1 * speed, duration).SetEase(Ease.Linear);
			rocket2.transform.DOMove(rocket2.transform.position + moveDir2 * speed, duration).SetEase(Ease.Linear);
			UnityEngine.Object.Destroy(rocket1, duration);
			UnityEngine.Object.Destroy(rocket2, duration);
			int maxDist = Mathf.Max(currentLevel.width, currentLevel.height);
			for (int dist = 1; dist <= maxDist; dist++)
			{
				List<ElementView> toDestroy = new List<ElementView>();
				int x1 = cx + ((!isVertical) ? (-dist) : 0);
				int y1 = cy + (isVertical ? dist : 0);
				if (x1 >= 0 && x1 < currentLevel.width && y1 >= 0 && y1 < currentLevel.height && activeElements[x1, y1] != null)
				{
					toDestroy.Add(activeElements[x1, y1]);
				}
				int x2 = cx + ((!isVertical) ? dist : 0);
				int y2 = cy + (isVertical ? (-dist) : 0);
				if (x2 >= 0 && x2 < currentLevel.width && y2 >= 0 && y2 < currentLevel.height && activeElements[x2, y2] != null)
				{
					toDestroy.Add(activeElements[x2, y2]);
				}
				if (dist == 1 && activeElements[cx, cy] != null)
				{
					toDestroy.Add(activeElements[cx, cy]);
				}
				if (toDestroy.Count > 0)
				{
					DestroyElements(toDestroy);
				}
				yield return new WaitForSeconds(1f / speed);
			}
		}

		private IEnumerator ActivateBomb(SpecialElementView special)
		{
			int cx = special.GridX;
			int cy = special.GridY;
			if (activeElements[cx, cy] == special)
			{
				activeElements[cx, cy] = null;
			}
			GameObject fxPrefab = special.Data?.activationEffectPrefab;
			if (fxPrefab != null)
			{
				GameObject fx = UnityEngine.Object.Instantiate(fxPrefab, special.transform.position, Quaternion.identity);
				UnityEngine.Object.Destroy(fx, 2f);
			}
			DestroyElementVisual(special);
			List<ElementView> toDestroy = new List<ElementView>();
			for (int x = cx - 1; x <= cx + 1; x++)
			{
				for (int y = cy - 1; y <= cy + 1; y++)
				{
					if (x >= 0 && x < currentLevel.width && y >= 0 && y < currentLevel.height && activeElements[x, y] != null)
					{
						toDestroy.Add(activeElements[x, y]);
					}
				}
			}
			DestroyElements(toDestroy);
			yield return new WaitForSeconds(0.2f);
		}

		private IEnumerator ActivateTargetSeeker(SpecialElementView special)
		{
			int cx = special.GridX;
			int cy = special.GridY;
			if (activeElements[cx, cy] == special)
			{
				activeElements[cx, cy] = null;
			}
			special.PlayTriggerAnimation();
			GameObject fxPrefab = special.Data?.activationEffectPrefab;
			if (fxPrefab != null)
			{
				GameObject fx = UnityEngine.Object.Instantiate(fxPrefab, special.transform.position, Quaternion.identity);
				UnityEngine.Object.Destroy(fx, 2f);
			}
			List<ElementView> areaToDestroy = new List<ElementView>();
			Vector2Int[] adjacentOffsets = new Vector2Int[4]
			{
				Vector2Int.up,
				Vector2Int.down,
				Vector2Int.left,
				Vector2Int.right
			};
			Vector2Int[] array = adjacentOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				Vector2Int offset = array[i];
				int x2 = cx + offset.x;
				int y = cy + offset.y;
				if (x2 >= 0 && x2 < currentLevel.width && y >= 0 && y < currentLevel.height && activeElements[x2, y] != null)
				{
					areaToDestroy.Add(activeElements[x2, y]);
				}
			}
			if (areaToDestroy.Count > 0)
			{
				DestroyElements(areaToDestroy);
				yield return new WaitForSeconds(0.2f);
			}
			List<ElementView> possibleTargets = new List<ElementView>();
			List<ElementView> collectibleTargets = new List<ElementView>();
			for (int x3 = 0; x3 < currentLevel.width; x3++)
			{
				for (int y2 = 0; y2 < currentLevel.height; y2++)
				{
					ElementView el = activeElements[x3, y2];
					if (!(el != null))
					{
						continue;
					}
					possibleTargets.Add(el);
					if (!(gameManager != null) || gameManager.activeTargets == null)
					{
						continue;
					}
					bool isTarget = false;
					foreach (RuntimeTarget t in gameManager.activeTargets)
					{
						if (t.remainingAmount > 0)
						{
							if (t.definition.targetType == CellItemType.SpecificNormal && el is NormalElementView norm && t.definition.normalElementId == norm.NormalElementId)
							{
								isTarget = true;
								break;
							}
							if (t.definition.targetType == CellItemType.RetractingCollectible && el is RetractingCollectibleView retract && t.definition.collectibleData != null && t.definition.collectibleData.collectibleId == retract.collectibleData.collectibleId)
							{
								isTarget = true;
								break;
							}
							if (t.definition.targetType == CellItemType.Obstacle && el is ObstacleView obs && t.definition.obstacleData != null && t.definition.obstacleData.obstacleId == obs.Data.obstacleId)
							{
								isTarget = true;
								break;
							}
							if (t.definition.targetType == CellItemType.Generator && el is GeneratorView gen && t.definition.generatorData != null && t.definition.generatorData.generatorId == gen.Data.generatorId)
							{
								isTarget = true;
								break;
							}
							if (t.definition.targetType == CellItemType.Special && el is SpecialElementView spec && t.definition.specialElement != null && t.definition.specialElement.elementId == spec.Data.elementId)
							{
								isTarget = true;
								break;
							}
							if (t.definition.targetType == CellItemType.Mother && el is ChildView child && t.definition.motherData != null && t.definition.motherData.motherId == child.Data.targetMotherId)
							{
								isTarget = true;
								break;
							}
						}
					}
					if (isTarget)
					{
						collectibleTargets.Add(el);
					}
				}
			}
			ElementView target = null;
			if (collectibleTargets.Count > 0)
			{
				target = collectibleTargets[UnityEngine.Random.Range(0, collectibleTargets.Count)];
			}
			else if (possibleTargets.Count > 0)
			{
				target = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];
			}
			if (target != null)
			{
				float flyTime = 0.5f;
				Vector3 startPos = special.transform.position;
				GameObject trailPrefab = special.Data?.trailEffectPrefab;
				if (trailPrefab != null)
				{
					Vector3 dir = (target.transform.position - startPos).normalized;
					GameObject trail = UnityEngine.Object.Instantiate(trailPrefab, startPos, Quaternion.identity, special.transform);
					float angle = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
					trail.transform.rotation = Quaternion.Euler(0f, 0f, angle);
				}
				special.transform.DOMove(target.transform.position, flyTime).SetEase(Ease.Linear);
				yield return new WaitForSeconds(flyTime);
				if (target != null && target.gameObject != null)
				{
					if (target is RetractingCollectibleView collectible)
					{
						TryRetractCollectible(collectible, directlyHitCollectiblesThisTurn);
					}
					else
					{
						DestroyElements(new List<ElementView> { target });
					}
				}
				special.transform.DOScale(Vector3.zero, 0.3f);
				SkeletonAnimation skeletonAnim2 = special.GetComponentInChildren<SkeletonAnimation>();
				if (skeletonAnim2 != null && skeletonAnim2.skeleton != null)
				{
					DOTween.To(() => skeletonAnim2.skeleton.A, delegate(float x)
					{
						skeletonAnim2.skeleton.A = x;
					}, 0f, 0.3f);
				}
				UnityEngine.Object.Destroy(special.gameObject, 0.3f);
				yield break;
			}
			yield return new WaitForSeconds(0.2f);
			special.transform.DOScale(Vector3.zero, 0.3f);
			SkeletonAnimation skeletonAnim = special.GetComponentInChildren<SkeletonAnimation>();
			if (skeletonAnim != null && skeletonAnim.skeleton != null)
			{
				DOTween.To(() => skeletonAnim.skeleton.A, delegate(float x)
				{
					skeletonAnim.skeleton.A = x;
				}, 0f, 0.3f);
			}
			UnityEngine.Object.Destroy(special.gameObject, 0.3f);
		}

		private void TryRetractCollectible(RetractingCollectibleView collectible, HashSet<RetractingCollectibleView> hitSet)
		{
			if (collectible == null || collectible.CurrentLength <= 0 || hitSet.Contains(collectible))
			{
				return;
			}
			hitSet.Add(collectible);
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlayLongObjectBackSound();
			}
			Vector2Int dirVec = Vector2Int.zero;
			switch (collectible.CurrentDirection)
			{
			case Direction.Up:
				dirVec = Vector2Int.up;
				break;
			case Direction.Down:
				dirVec = Vector2Int.down;
				break;
			case Direction.Left:
				dirVec = Vector2Int.left;
				break;
			case Direction.Right:
				dirVec = Vector2Int.right;
				break;
			}
			for (int cx = 0; cx < currentLevel.width; cx++)
			{
				for (int cy = 0; cy < currentLevel.height; cy++)
				{
					if (activeElements[cx, cy] == collectible)
					{
						activeElements[cx, cy] = null;
					}
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
					targetIndex = gameManager.OnElementCollected(CellItemType.RetractingCollectible, -1, null, collectible.collectibleData);
				}
				DestroyElementVisual(collectible, targetIndex);
			}
		}

		private void CheckAdjacentCollectibles(int x, int y, bool isFromSpecial = false)
		{
			Vector2Int[] dirs = new Vector2Int[4]
			{
				Vector2Int.up,
				Vector2Int.down,
				Vector2Int.left,
				Vector2Int.right
			};
			Vector2Int[] array = dirs;
			for (int i = 0; i < array.Length; i++)
			{
				Vector2Int dir = array[i];
				int nx = x + dir.x;
				int ny = y + dir.y;
				if (nx < 0 || nx >= currentLevel.width || ny < 0 || ny >= currentLevel.height)
				{
					continue;
				}
				if (activeElements[nx, ny] is RetractingCollectibleView collectible)
				{
					if (!isFromSpecial)
					{
						TryRetractCollectible(collectible, retractedThisTurn);
					}
				}
				else if (activeElements[nx, ny] is ObstacleView obstacle)
				{
					if ((obstacle.Data != null && obstacle.Data.onlyDamagedBySpecials && !isFromSpecial) || damagedObstaclesThisTurn.Contains(obstacle))
					{
						continue;
					}
					damagedObstaclesThisTurn.Add(obstacle);
					obstacle.TakeDamage();
					if (obstacle.CurrentHealth <= 0)
					{
						activeElements[nx, ny] = null;
						obstacle.PlayCollectEffect();
						int targetIndex2 = -1;
						if (gameManager != null)
						{
							targetIndex2 = gameManager.OnElementCollected(CellItemType.Obstacle, -1, null, null, obstacle.Data);
						}
						DestroyElementVisual(obstacle, targetIndex2);
					}
				}
				else if (activeElements[nx, ny] is GeneratorView generator)
				{
					if (triggeredGeneratorsThisTurn.Contains(generator))
					{
						continue;
					}
					triggeredGeneratorsThisTurn.Add(generator);
					if (generator.AdvanceState())
					{
						int targetIndex = -1;
						if (gameManager != null)
						{
							targetIndex = gameManager.OnElementCollected(CellItemType.Generator, -1, null, null, null, generator.Data);
						}
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

		private void ProcessChildCollection(ChildView child)
		{
			if (triggeredChildrenThisTurn.Contains(child))
			{
				return;
			}
			triggeredChildrenThisTurn.Add(child);
			if (activeElements[child.GridX, child.GridY] == child)
			{
				activeElements[child.GridX, child.GridY] = null;
			}
			child.PlayCollectEffect();
			MotherView targetMother = null;
			for (int cx = 0; cx < currentLevel.width; cx++)
			{
				for (int cy = 0; cy < currentLevel.height; cy++)
				{
					if (activeElements[cx, cy] is MotherView mother && mother.Data.motherId == child.Data.targetMotherId && mother.CurrentCount - mother.PendingReceiveCount > 0)
					{
						targetMother = mother;
						break;
					}
				}
				if (targetMother != null)
				{
					break;
				}
			}
			if (targetMother != null)
			{
				targetMother.PendingReceiveCount++;
				if (AudioManager.Instance != null && child.Data.flySound != null)
				{
					AudioManager.Instance.PlayContinuousSound(child.Data.flySound);
				}
				SpriteRenderer sr = child.GetComponentInChildren<SpriteRenderer>();
				if (sr != null)
				{
					sr.sortingOrder = 100;
				}
				MeshRenderer mr = child.GetComponentInChildren<MeshRenderer>();
				if (mr != null)
				{
					mr.sortingOrder = 100;
				}
				Vector3 startPos = child.transform.position;
				Vector3 targetPos = targetMother.transform.position;
				if (targetMother.transform.childCount > 0)
				{
					targetPos = targetMother.transform.GetChild(0).position;
				}
				Vector3 dir = targetPos - startPos;
				Vector3 perp = new Vector3(0f - dir.y, dir.x, 0f).normalized;
				float curveAmount = UnityEngine.Random.Range(-0.5f, 0.5f);
				Vector3 midPoint = startPos + dir / 2f + perp * curveAmount;
				Vector3[] path = new Vector3[2] { midPoint, targetPos };
				bool childDestroyed = false;
				child.transform.DOPath(path, 0.5f, PathType.CatmullRom).SetEase(Ease.InOutQuad).OnComplete(delegate
				{
					try
					{
						if (targetMother != null)
						{
							targetMother.PendingReceiveCount--;
						}
						targetMother.ReceiveChild(delegate
						{
							int targetIndex = -1;
							if (gameManager != null)
							{
								targetIndex = gameManager.OnElementCollected(CellItemType.Mother, -1, null, null, null, null, targetMother.Data);
							}
							for (int i = 0; i < currentLevel.width; i++)
							{
								for (int j = 0; j < currentLevel.height; j++)
								{
									if (activeElements[i, j] == targetMother)
									{
										activeElements[i, j] = null;
									}
								}
							}
							DestroyElementVisual(targetMother, targetIndex);
							pendingRefill = true;
						});
					}
					finally
					{
						if (!childDestroyed && child != null && child.gameObject != null)
						{
							childDestroyed = true;
							UnityEngine.Object.Destroy(child.gameObject);
						}
					}
				});
				child.transform.DOScale(child.transform.localScale * 0.5f, 0.5f);
			}
			else
			{
				int tIndex = -1;
				if (gameManager != null)
				{
					tIndex = gameManager.OnElementCollected(CellItemType.Child, -1, null, null, null, null, null, child.Data);
				}
				DestroyElementVisual(child, tIndex);
			}
		}

		public void InitializeBoard(LevelData levelData, ElementDatabase database, RectTransform uiBoardArea)
		{
			currentLevel = levelData;
			elementDb = database;
			activeElements = new ElementView[currentLevel.width, currentLevel.height];
			refillSequenceIndices = new int[currentLevel.width];
			refillWarnings.Clear();
			pendingRefill = false;
			activeSpecialChains = 0;
			isBoardClearing = false;
			currentlyTriggeringSpecials.Clear();
			ClearContainer(backgroundContainer);
			ClearContainer(elementsContainer);
			CalculateBoardOrigin(uiBoardArea);
			GenerateGrid();
			PlayIntroAnimation();
			StartCoroutine(InitialBoardCheck());
		}

		private IEnumerator InitialBoardCheck()
		{
			yield return new WaitForSeconds(0.8f);
			if (!HasPossibleMoves())
			{
				yield return StartCoroutine(CheckAndShuffleIfNeeded());
			}
		}

		private void PlayIntroAnimation()
		{
			float screenWorldWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
			if (backgroundContainer != null)
			{
				backgroundContainer.DOKill();
				backgroundContainer.localPosition = new Vector3(screenWorldWidth, 0f, 0f);
				backgroundContainer.DOLocalMoveX(0f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
			}
			if (elementsContainer != null)
			{
				elementsContainer.DOKill();
				elementsContainer.localPosition = new Vector3(screenWorldWidth, 0f, 0f);
				elementsContainer.DOLocalMoveX(0f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.2f);
			}
		}

		public void UpdateBoardLayout(RectTransform uiBoardArea)
		{
			if (currentLevel == null || activeElements == null)
			{
				return;
			}
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
					if (currentLevel.GetCell(x, y) == null || !currentLevel.GetCell(x, y).isPlayable)
					{
						continue;
					}
					Vector3 worldPos = GetWorldPosition(x, y);
					Transform bgTile = backgroundContainer.Find($"Tile_{x}_{y}");
					if (bgTile != null)
					{
						bgTile.position = worldPos;
						SpriteRenderer bgSr = bgTile.GetComponent<SpriteRenderer>();
						if (bgSr != null && bgSr.sprite != null && bgSr.sprite != cachedDefaultBgSprite)
						{
							float bgWidth2 = bgSr.sprite.bounds.size.x;
							float bgHeight2 = bgSr.sprite.bounds.size.y;
							float scaleX2 = cellSize / bgWidth2;
							float scaleY2 = cellSize / bgHeight2;
							bgTile.localScale = new Vector3(scaleX2, scaleY2, 1f);
						}
					}
					Transform conveyorObj = backgroundContainer.Find($"Conveyor_{x}_{y}");
					if (conveyorObj != null)
					{
						conveyorObj.position = worldPos;
						SpriteRenderer bgSr2 = conveyorObj.GetComponent<SpriteRenderer>();
						if (bgSr2 != null && bgSr2.sprite != null)
						{
							float bgWidth = bgSr2.sprite.bounds.size.x;
							float bgHeight = bgSr2.sprite.bounds.size.y;
							float targetSize = cellSize + cellSpacing;
							float scaleX = targetSize / bgWidth;
							float scaleY = targetSize / bgHeight;
							Vector3 currentScale = conveyorObj.localScale;
							scaleX = Mathf.Abs(scaleX) * Mathf.Sign(currentScale.x);
							scaleY = Mathf.Abs(scaleY) * Mathf.Sign(currentScale.y);
							conveyorObj.localScale = new Vector3(scaleX, scaleY, 1f);
						}
					}
					if (!(activeElements[x, y] != null))
					{
						continue;
					}
					ElementView el = activeElements[x, y];
					if (el.GridX == x && el.GridY == y)
					{
						el.SetPosition(x, y, worldPos);
						if (el is RetractingCollectibleView collectView)
						{
							collectView.UpdateCellSize(cellSize, cellSpacing);
						}
						else if (el.spriteRenderer != null && el.spriteRenderer.sprite != null)
						{
							el.UpdateVisuals(el.spriteRenderer.sprite, cellSize);
						}
					}
				}
			}
		}

		private void CalculateBoardOrigin(RectTransform uiBoardArea)
		{
			if (originalCellSize < 0f)
			{
				originalCellSize = cellSize;
				originalCellSpacing = cellSpacing;
			}
			cellSize = originalCellSize;
			cellSpacing = originalCellSpacing;
			Vector3 centerPosition = base.transform.position;
			if (uiBoardArea != null)
			{
				Canvas.ForceUpdateCanvases();
				Vector3[] corners = new Vector3[4];
				uiBoardArea.GetWorldCorners(corners);
				Canvas canvas = uiBoardArea.GetComponentInParent<Canvas>();
				if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					Camera cam = ((canvas.worldCamera != null) ? canvas.worldCamera : Camera.main);
					if (cam != null)
					{
						for (int i = 0; i < 4; i++)
						{
							corners[i] = cam.ScreenToWorldPoint(corners[i]);
							corners[i].z = 0f;
						}
					}
				}
				centerPosition = (corners[0] + corners[2]) / 2f;
				centerPosition.z = 0f;
				float uiWorldWidth = Vector3.Distance(corners[0], corners[3]);
				float uiWorldHeight = Vector3.Distance(corners[0], corners[1]);
				float unscaledWidth = (float)currentLevel.width * cellSize + (float)(currentLevel.width - 1) * cellSpacing;
				float unscaledHeight = (float)currentLevel.height * cellSize + (float)(currentLevel.height - 1) * cellSpacing;
				if (uiWorldWidth > 0f && uiWorldHeight > 0f)
				{
					float scaleX = uiWorldWidth / unscaledWidth;
					float scaleY = uiWorldHeight / unscaledHeight;
					float scale = Mathf.Min(scaleX, scaleY);
					scale *= 1f - boardEdgePadding;
					cellSize *= scale;
					cellSpacing *= scale;
				}
			}
			float totalWidth = (float)currentLevel.width * cellSize + (float)(currentLevel.width - 1) * cellSpacing;
			float totalHeight = (float)currentLevel.height * cellSize + (float)(currentLevel.height - 1) * cellSpacing;
			boardBottomLeftOrigin = centerPosition - new Vector3(totalWidth / 2f, totalHeight / 2f, 0f);
			boardBottomLeftOrigin += new Vector3(cellSize / 2f, cellSize / 2f, 0f);
		}

		private void GenerateGrid()
		{
			newSpecialsThisTurn.Clear();
			retractedThisTurn.Clear();
			directlyHitCollectiblesThisTurn.Clear();
			damagedObstaclesThisTurn.Clear();
			triggeredGeneratorsThisTurn.Clear();
			bool useGlobalBg = false;
			Texture2D globalBgTex = null;
			if (ConfigManager.Instance != null && ConfigManager.Instance.useCustomGridBackground && ConfigManager.Instance.GridBackgroundPic != null)
			{
				useGlobalBg = true;
				globalBgTex = ConfigManager.Instance.GridBackgroundPic;
			}
			if (useGlobalBg && globalBgTex != null)
			{
				globalBgTile = new GameObject("GlobalBoardBackground");
				globalBgTile.transform.SetParent(backgroundContainer);
				SpriteRenderer bgSr = globalBgTile.AddComponent<SpriteRenderer>();
				Sprite bgSprite = (bgSr.sprite = Sprite.Create(globalBgTex, new Rect(0f, 0f, globalBgTex.width, globalBgTex.height), new Vector2(0.5f, 0.5f), 100f));
				bgSr.sortingOrder = -10;
				UpdateGlobalBackground();
			}
			for (int x = 0; x < currentLevel.width; x++)
			{
				for (int y = 0; y < currentLevel.height; y++)
				{
					GridCellData cellData = currentLevel.GetCell(x, y);
					if (cellData == null || !cellData.isPlayable)
					{
						continue;
					}
					Vector3 worldPos = GetWorldPosition(x, y);
					if (!useGlobalBg)
					{
						if (backgroundTilePrefab != null)
						{
							GameObject bgTile = UnityEngine.Object.Instantiate(backgroundTilePrefab, worldPos, Quaternion.identity, backgroundContainer);
							bgTile.name = $"Tile_{x}_{y}";
							SpriteRenderer bgSr2 = bgTile.GetComponent<SpriteRenderer>();
							if (bgSr2 != null && bgSr2.sprite != null)
							{
								float bgWidth2 = bgSr2.sprite.bounds.size.x;
								float bgHeight2 = bgSr2.sprite.bounds.size.y;
								float scaleX2 = cellSize / bgWidth2;
								float scaleY2 = cellSize / bgHeight2;
								bgTile.transform.localScale = new Vector3(scaleX2, scaleY2, 1f);
							}
						}
						else
						{
							GameObject bgTile2 = new GameObject($"Tile_{x}_{y}");
							bgTile2.transform.SetParent(backgroundContainer);
							bgTile2.transform.position = worldPos;
							SpriteRenderer bgSr4 = bgTile2.AddComponent<SpriteRenderer>();
							if (cachedDefaultBgSprite == null)
							{
								Texture2D tex = Texture2D.whiteTexture;
								cachedDefaultBgSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
							}
							bgSr4.sprite = cachedDefaultBgSprite;
							bgSr4.color = new Color(0f, 0f, 0f, 0.3f);
							float bgWidth3 = bgSr4.sprite.bounds.size.x;
							float bgHeight3 = bgSr4.sprite.bounds.size.y;
							float scaleX3 = cellSize / bgWidth3;
							float scaleY3 = cellSize / bgHeight3;
							bgTile2.transform.localScale = new Vector3(scaleX3, scaleY3, 1f);
						}
					}
					if (cellData.hasConveyor)
					{
						bool isCorner = false;
						Direction inDir = cellData.conveyorDirection;
						if (IsValidGridPos(x - 1, y) && currentLevel.GetCell(x - 1, y).hasConveyor && currentLevel.GetCell(x - 1, y).conveyorDirection == Direction.Right)
						{
							inDir = Direction.Right;
						}
						else if (IsValidGridPos(x + 1, y) && currentLevel.GetCell(x + 1, y).hasConveyor && currentLevel.GetCell(x + 1, y).conveyorDirection == Direction.Left)
						{
							inDir = Direction.Left;
						}
						else if (IsValidGridPos(x, y - 1) && currentLevel.GetCell(x, y - 1).hasConveyor && currentLevel.GetCell(x, y - 1).conveyorDirection == Direction.Up)
						{
							inDir = Direction.Up;
						}
						else if (IsValidGridPos(x, y + 1) && currentLevel.GetCell(x, y + 1).hasConveyor && currentLevel.GetCell(x, y + 1).conveyorDirection == Direction.Down)
						{
							inDir = Direction.Down;
						}
						if (inDir != cellData.conveyorDirection)
						{
							isCorner = true;
						}
						string prefabName = (isCorner ? "Prefabs/ConveyorCorner" : "Prefabs/ConveyorStraight");
						GameObject prefab = Resources.Load<GameObject>(prefabName);
						if (prefab != null)
						{
							GameObject conveyorObj = UnityEngine.Object.Instantiate(prefab, worldPos, Quaternion.identity, backgroundContainer);
							conveyorObj.name = $"Conveyor_{x}_{y}";
							SpriteRenderer bgSr3 = conveyorObj.GetComponent<SpriteRenderer>();
							if (bgSr3 != null && bgSr3.sprite != null)
							{
								float bgWidth = bgSr3.sprite.bounds.size.x;
								float bgHeight = bgSr3.sprite.bounds.size.y;
								float targetSize = cellSize + cellSpacing;
								float scaleX = targetSize / bgWidth;
								float scaleY = targetSize / bgHeight;
								conveyorObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
							}
							float angle = 0f;
							switch (cellData.conveyorDirection)
							{
							case Direction.Right:
								angle = 0f;
								break;
							case Direction.Up:
								angle = 90f;
								break;
							case Direction.Left:
								angle = 180f;
								break;
							case Direction.Down:
								angle = 270f;
								break;
							}
							if (isCorner)
							{
								ConveyorCornerConfig config = cornerConfigs.Find((ConveyorCornerConfig c) => c.inDir == inDir && c.outDir == cellData.conveyorDirection);
								if (config != null)
								{
									angle = config.rotationAngle;
									conveyorObj.transform.localScale = new Vector3(conveyorObj.transform.localScale.x * (config.flipX ? (-1f) : 1f), conveyorObj.transform.localScale.y * (config.flipY ? (-1f) : 1f), 1f);
									Transform arrow2 = conveyorObj.transform.Find("Arrow");
									if (arrow2 != null)
									{
										SkeletonAnimation anim2 = arrow2.GetComponent<SkeletonAnimation>();
										if (anim2 != null && !string.IsNullOrEmpty(config.animationName))
										{
											anim2.AnimationName = config.animationName;
											anim2.Initialize(true);
										}
									}
								}
								else
								{
									bool isRightTurn = false;
									if (inDir == Direction.Up && cellData.conveyorDirection == Direction.Right)
									{
										isRightTurn = true;
									}
									if (inDir == Direction.Right && cellData.conveyorDirection == Direction.Down)
									{
										isRightTurn = true;
									}
									if (inDir == Direction.Down && cellData.conveyorDirection == Direction.Left)
									{
										isRightTurn = true;
									}
									if (inDir == Direction.Left && cellData.conveyorDirection == Direction.Up)
									{
										isRightTurn = true;
									}
									if (!isRightTurn)
									{
										conveyorObj.transform.localScale = new Vector3(0f - conveyorObj.transform.localScale.x, conveyorObj.transform.localScale.y, 1f);
										Transform arrow = conveyorObj.transform.Find("Arrow");
										if (arrow != null)
										{
											SkeletonAnimation anim = arrow.GetComponent<SkeletonAnimation>();
											if (anim != null)
											{
												anim.AnimationName = "line_lu";
												anim.Initialize(true);
											}
										}
									}
								}
							}
							conveyorObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
						}
					}
					SpawnElement(cellData, x, y, worldPos);
				}
			}
		}

		private void UpdateGlobalBackground()
		{
			if (!(globalBgTile == null))
			{
				SpriteRenderer bgSr = globalBgTile.GetComponent<SpriteRenderer>();
				if (bgSr != null && bgSr.sprite != null)
				{
					float totalWidth = (float)currentLevel.width * cellSize + (float)(currentLevel.width - 1) * cellSpacing;
					float totalHeight = (float)currentLevel.height * cellSize + (float)(currentLevel.height - 1) * cellSpacing;
					Vector3 centerPosition = boardBottomLeftOrigin + new Vector3(totalWidth / 2f - cellSize / 2f, totalHeight / 2f - cellSize / 2f, 0f);
					globalBgTile.transform.position = centerPosition;
					float bgWidth = bgSr.sprite.bounds.size.x;
					float bgHeight = bgSr.sprite.bounds.size.y;
					float padding = cellSize * 0.5f;
					float targetWidth = totalWidth + padding;
					float targetHeight = totalHeight + padding;
					float scaleX = targetWidth / bgWidth;
					float scaleY = targetHeight / bgHeight;
					float scale = Mathf.Max(scaleX, scaleY);
					globalBgTile.transform.localScale = new Vector3(scale, scale, 1f);
				}
			}
		}

		private void SpawnElement(GridCellData cellData, int x, int y, Vector3 worldPos)
		{
			if (cellData.itemType == CellItemType.None || activeElements[x, y] != null)
			{
				return;
			}
			GameObject newElementObj = null;
			ElementView elementView = null;
			if (cellData.itemType == CellItemType.RandomNormal || cellData.itemType == CellItemType.SpecificNormal)
			{
				int normalId = cellData.normalElementId;
				if (cellData.itemType == CellItemType.RandomNormal)
				{
					int randIndex = UnityEngine.Random.Range(0, elementDb.normalElements.Count);
					normalId = elementDb.normalElements[randIndex].id;
				}
				NormalElementInfo normInfo = elementDb.GetNormalElement(normalId);
				if (normInfo != null && elementDb.unifiedNormalPrefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(elementDb.unifiedNormalPrefab, elementsContainer);
					NormalElementView normalView = newElementObj.GetComponent<NormalElementView>();
					if (normalView != null)
					{
						normalView.Initialize(normInfo, x, y, worldPos, cellSize);
						elementView = normalView;
					}
				}
			}
			else if (cellData.itemType == CellItemType.Special && cellData.specialElement != null)
			{
				if (cellData.specialElement.prefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(cellData.specialElement.prefab, elementsContainer);
					SpecialElementView specialView = newElementObj.GetComponent<SpecialElementView>();
					if (specialView != null)
					{
						specialView.Initialize(cellData.specialElement, x, y, worldPos, cellSize);
						elementView = specialView;
					}
				}
			}
			else if (cellData.itemType == CellItemType.Obstacle && cellData.obstacleData != null)
			{
				if (cellData.obstacleData.prefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(cellData.obstacleData.prefab, elementsContainer);
					ObstacleView obstacleView = newElementObj.GetComponent<ObstacleView>();
					if (obstacleView != null)
					{
						obstacleView.Initialize(cellData.obstacleData, x, y, worldPos, cellSize);
						elementView = obstacleView;
					}
				}
			}
			else if (cellData.itemType == CellItemType.Generator && cellData.generatorData != null)
			{
				if (cellData.generatorData.prefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(cellData.generatorData.prefab, elementsContainer);
					GeneratorView generatorView = newElementObj.GetComponent<GeneratorView>();
					if (generatorView != null)
					{
						generatorView.Initialize(cellData.generatorData, x, y, worldPos, cellSize);
						elementView = generatorView;
					}
				}
			}
			else if (cellData.itemType == CellItemType.RetractingCollectible && cellData.collectibleData != null)
			{
				if (cellData.collectibleData.prefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(cellData.collectibleData.prefab, elementsContainer);
					RetractingCollectibleView collectView2 = newElementObj.GetComponent<RetractingCollectibleView>();
					if (collectView2 != null)
					{
						int headX = x;
						int headY = y;
						switch (cellData.collectibleDirection)
						{
						case Direction.Up:
							headY += cellData.collectibleLength - 1;
							break;
						case Direction.Down:
							headY -= cellData.collectibleLength - 1;
							break;
						case Direction.Right:
							headX += cellData.collectibleLength - 1;
							break;
						case Direction.Left:
							headX -= cellData.collectibleLength - 1;
							break;
						}
						collectView2.Initialize(cellData.collectibleData, x, y, headX, headY, worldPos, cellSize, cellSpacing);
						elementView = collectView2;
					}
				}
			}
			else if (cellData.itemType == CellItemType.Mother && cellData.motherData != null)
			{
				if (cellData.motherData.prefab != null)
				{
					newElementObj = UnityEngine.Object.Instantiate(cellData.motherData.prefab, elementsContainer);
					MotherView motherView = newElementObj.GetComponent<MotherView>();
					if (motherView != null)
					{
						motherView.Initialize(cellData.motherData, x, y, worldPos, cellSize, cellSpacing);
						elementView = motherView;
						for (int j = 0; j < cellData.motherData.gridWidth; j++)
						{
							for (int k = 0; k < cellData.motherData.gridHeight; k++)
							{
								int bx2 = x + j;
								int by2 = y + k;
								if (bx2 >= 0 && bx2 < currentLevel.width && by2 >= 0 && by2 < currentLevel.height)
								{
									activeElements[bx2, by2] = motherView;
								}
							}
						}
					}
				}
			}
			else if (cellData.itemType == CellItemType.Child && cellData.childData != null && cellData.childData.prefab != null)
			{
				newElementObj = UnityEngine.Object.Instantiate(cellData.childData.prefab, elementsContainer);
				ChildView childView = newElementObj.GetComponent<ChildView>();
				if (childView != null)
				{
					childView.Initialize(cellData.childData, x, y, worldPos, cellSize);
					elementView = childView;
				}
			}
			if (!(newElementObj != null) || !(elementView != null))
			{
				return;
			}
			newElementObj.name = $"Element_{x}_{y}";
			activeElements[x, y] = elementView;
			if (elementView is RetractingCollectibleView collectView)
			{
				Vector2Int dirVec = Vector2Int.zero;
				switch (collectView.CurrentDirection)
				{
				case Direction.Up:
					dirVec = Vector2Int.up;
					break;
				case Direction.Down:
					dirVec = Vector2Int.down;
					break;
				case Direction.Left:
					dirVec = Vector2Int.left;
					break;
				case Direction.Right:
					dirVec = Vector2Int.right;
					break;
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
				elementView.transform.rotation = Quaternion.identity;
			}
		}

		public Vector3 GetWorldPosition(int x, int y)
		{
			float posX = (float)x * (cellSize + cellSpacing);
			float posY = (float)y * (cellSize + cellSpacing);
			return boardBottomLeftOrigin + new Vector3(posX, posY, 0f);
		}

		private void ClearContainer(Transform container)
		{
			if (container == null)
			{
				return;
			}
			for (int i = container.childCount - 1; i >= 0; i--)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(container.GetChild(i).gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(container.GetChild(i).gameObject);
				}
			}
		}

		private bool IsActiveLevelTarget(ElementView element)
		{
			if (element == null || gameManager == null || gameManager.activeTargets == null)
			{
				return false;
			}
			foreach (RuntimeTarget target in gameManager.activeTargets)
			{
				if (target != null && target.remainingAmount > 0 && target.definition != null)
				{
					if (target.definition.targetType == CellItemType.SpecificNormal && element is NormalElementView normal && target.definition.normalElementId == normal.NormalElementId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.RetractingCollectible && element is RetractingCollectibleView retract && target.definition.collectibleData != null && target.definition.collectibleData.collectibleId == retract.collectibleData.collectibleId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.Obstacle && element is ObstacleView obstacle && target.definition.obstacleData != null && target.definition.obstacleData.obstacleId == obstacle.Data.obstacleId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.Generator && element is GeneratorView generator && target.definition.generatorData != null && target.definition.generatorData.generatorId == generator.Data.generatorId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.Special && element is SpecialElementView special && target.definition.specialElement != null && target.definition.specialElement.elementId == special.Data.elementId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.Mother && element is ChildView child && target.definition.motherData != null && target.definition.motherData.motherId == child.Data.targetMotherId)
					{
						return true;
					}
					if (target.definition.targetType == CellItemType.Child && element is ChildView directChild && target.definition.childData != null && target.definition.childData.childId == directChild.Data.childId)
					{
						return true;
					}
				}
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
						if (IsActiveLevelTarget(element))
						{
							priorityPool.Add(element);
						}
						else
						{
							secondaryPool.Add(element);
						}
					}
				}
			}
			for (int j = 0; j < priorityPool.Count; j++)
			{
				int r = UnityEngine.Random.Range(j, priorityPool.Count);
				ElementView temp = priorityPool[j];
				priorityPool[j] = priorityPool[r];
				priorityPool[r] = temp;
			}
			for (int i = 0; i < secondaryPool.Count; i++)
			{
				int r2 = UnityEngine.Random.Range(i, secondaryPool.Count);
				ElementView temp2 = secondaryPool[i];
				secondaryPool[i] = secondaryPool[r2];
				secondaryPool[r2] = temp2;
			}
			foreach (ElementView p in priorityPool)
			{
				if (targets.Count < count)
				{
					targets.Add(p);
					continue;
				}
				break;
			}
			foreach (ElementView s in secondaryPool)
			{
				if (targets.Count < count)
				{
					targets.Add(s);
					continue;
				}
				break;
			}
			return targets;
		}

		private IEnumerator RotateSpecials(SpecialElementView specA, SpecialElementView specB, float duration = 0.5f)
		{
			if (specA.spriteRenderer != null)
			{
				_ = specA.spriteRenderer.sortingOrder;
				specA.spriteRenderer.sortingOrder = 200;
			}
			if (specB.spriteRenderer != null)
			{
				_ = specB.spriteRenderer.sortingOrder;
				specB.spriteRenderer.sortingOrder = 200;
			}
			if (specA.spineAnimation != null)
			{
				MeshRenderer mr2 = specA.spineAnimation.GetComponent<MeshRenderer>();
				if (mr2 != null)
				{
					_ = mr2.sortingOrder;
					mr2.sortingOrder = 200;
				}
			}
			if (specB.spineAnimation != null)
			{
				MeshRenderer mr = specB.spineAnimation.GetComponent<MeshRenderer>();
				if (mr != null)
				{
					_ = mr.sortingOrder;
					mr.sortingOrder = 200;
				}
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
				float rad = angle * (3.14159265f / 180f);
				Vector3 offsetA = posA - center;
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);
				Vector3 newOffsetA = ((!isHorizontal) ? new Vector3(offsetA.y * sin * 0.3f, offsetA.y * cos, 0f) : new Vector3(offsetA.x * cos, offsetA.x * sin * 0.3f, 0f));
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

		private IEnumerator ActivateSpecialCombo(SpecialElementView specA, SpecialElementView specB)
		{
			int cx = specA.GridX;
			int cy = specA.GridY;
			if (activeElements[specA.GridX, specA.GridY] == specA)
			{
				activeElements[specA.GridX, specA.GridY] = null;
			}
			if (activeElements[specB.GridX, specB.GridY] == specB)
			{
				activeElements[specB.GridX, specB.GridY] = null;
			}
			bool isRocketA = specA.BehaviorType == SpecialBehaviorType.LineClearHorizontal || specA.BehaviorType == SpecialBehaviorType.LineClearVertical;
			bool isRocketB = specB.BehaviorType == SpecialBehaviorType.LineClearHorizontal || specB.BehaviorType == SpecialBehaviorType.LineClearVertical;
			bool isBombA = specA.BehaviorType == SpecialBehaviorType.BombExplosion;
			bool isBombB = specB.BehaviorType == SpecialBehaviorType.BombExplosion;
			bool isBirdA = specA.BehaviorType == SpecialBehaviorType.TargetSeeker;
			bool isBirdB = specB.BehaviorType == SpecialBehaviorType.TargetSeeker;
			bool isMagicA = specA.BehaviorType == SpecialBehaviorType.ColorClear;
			bool isMagicB = specB.BehaviorType == SpecialBehaviorType.ColorClear;
			List<ElementView> toDestroy = new List<ElementView>();
			bool swapPlayed = false;
			float swapDuration = 0f;
			float swapAnimStartTime = 0f;
			if (isMagicA == isMagicB)
			{
				bool loopSwapAnim = !(isMagicA && isMagicB);
				swapPlayed = specA.PlaySwapAnimation(specB.BehaviorType, loopSwapAnim);
				if (!swapPlayed)
				{
					swapPlayed = specB.PlaySwapAnimation(specA.BehaviorType, loopSwapAnim);
				}
			}
			if (swapPlayed)
			{
				specA.swapAnimationPlayed = true;
				specB.swapAnimationPlayed = true;
				specA.StopIdleVisuals();
				specB.StopIdleVisuals();
				swapDuration = specA.GetSwapAnimationDuration(specB.BehaviorType);
				if (swapDuration <= 0f)
				{
					swapDuration = specB.GetSwapAnimationDuration(specA.BehaviorType);
				}
				swapAnimStartTime = Time.time;
			}
			bool needsRotation = false;
			if ((isRocketA && isBombB) || (isBombA && isRocketB) || (isBombA && isBombB) || (isBirdA && isRocketB) || (isRocketA && isBirdB) || (isBirdA && isBombB) || (isBombA && isBirdB))
			{
				needsRotation = true;
			}
			if (needsRotation && !swapPlayed)
			{
				if (isBirdA)
				{
					specA.PlayTriggerAnimation();
					if (specA.Data?.activationEffectPrefab != null)
					{
						GameObject fx = UnityEngine.Object.Instantiate(specA.Data.activationEffectPrefab, specA.transform.position, Quaternion.identity, specA.transform);
						fx.transform.localPosition = Vector3.zero;
					}
				}
				if (isBirdB)
				{
					specB.PlayTriggerAnimation();
					if (specB.Data?.activationEffectPrefab != null)
					{
						GameObject fx2 = UnityEngine.Object.Instantiate(specB.Data.activationEffectPrefab, specB.transform.position, Quaternion.identity, specB.transform);
						fx2.transform.localPosition = Vector3.zero;
					}
				}
				yield return StartCoroutine(RotateSpecials(specA, specB));
			}
			if (AudioManager.Instance != null)
			{
				if (isBirdA || isBirdB)
				{
					AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.TargetSeeker);
				}
				else if (!(isMagicA && isMagicB) && !(isBombA && isBombB))
				{
					AudioManager.Instance.PlaySpecialTriggerSound(specA.BehaviorType);
				}
			}
			if (isRocketA && isRocketB)
			{
				GameObject trailPrefabA = specA.Data?.trailEffectPrefab ?? specB.Data?.trailEffectPrefab;
				if (trailPrefabA != null)
				{
					Vector3 startPos = GetWorldPosition(cx, cy);
					Vector3[] dirs = new Vector3[4]
					{
						Vector3.up,
						Vector3.down,
						Vector3.left,
						Vector3.right
					};
					Vector3[] array = dirs;
					for (int i = 0; i < array.Length; i++)
					{
						Vector3 d = array[i];
						GameObject trail2 = UnityEngine.Object.Instantiate(trailPrefabA, startPos, Quaternion.identity);
						float angle = Mathf.Atan2(d.y, d.x) * 57.29578f;
						trail2.transform.rotation = Quaternion.Euler(0f, 0f, angle);
						trail2.transform.DOMove(startPos + d * 20f, 1f).SetEase(Ease.Linear);
						UnityEngine.Object.Destroy(trail2, 1f);
					}
				}
				if (!swapPlayed)
				{
					specA.PlayTriggerAnimation();
					specB.PlayTriggerAnimation();
				}
				for (int x2 = 0; x2 < currentLevel.width; x2++)
				{
					if (activeElements[x2, cy] != null)
					{
						toDestroy.Add(activeElements[x2, cy]);
					}
				}
				for (int y3 = 0; y3 < currentLevel.height; y3++)
				{
					if (activeElements[cx, y3] != null)
					{
						toDestroy.Add(activeElements[cx, y3]);
					}
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
			}
			else if ((isRocketA && isBombB) || (isBombA && isRocketB))
			{
				if (swapDuration > 0f)
				{
					yield return new WaitForSeconds(swapDuration);
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
				for (int x3 = 0; x3 < currentLevel.width; x3++)
				{
					for (int yOffset = -1; yOffset <= 1; yOffset++)
					{
						if (cy + yOffset >= 0 && cy + yOffset < currentLevel.height && activeElements[x3, cy + yOffset] != null)
						{
							toDestroy.Add(activeElements[x3, cy + yOffset]);
						}
					}
				}
				for (int y4 = 0; y4 < currentLevel.height; y4++)
				{
					for (int xOffset = -1; xOffset <= 1; xOffset++)
					{
						if (cx + xOffset >= 0 && cx + xOffset < currentLevel.width && activeElements[cx + xOffset, y4] != null)
						{
							toDestroy.Add(activeElements[cx + xOffset, y4]);
						}
					}
				}
			}
			else if (isBombA && isBombB)
			{
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlayBombBombMergeSound();
				}
				if (swapDuration > 0f)
				{
					yield return new WaitForSeconds(swapDuration);
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
				for (int x4 = cx - 2; x4 <= cx + 2; x4++)
				{
					for (int y5 = cy - 2; y5 <= cy + 2; y5++)
					{
						if (x4 >= 0 && x4 < currentLevel.width && y5 >= 0 && y5 < currentLevel.height && activeElements[x4, y5] != null)
						{
							toDestroy.Add(activeElements[x4, y5]);
						}
					}
				}
			}
			else if (isBirdA && isBirdB)
			{
				List<ElementView> targets = GetPriorityTargets(3);
				GameObject birdPrefab = specA.Data?.prefab ?? specB.Data?.prefab;
				GameObject fxPrefab = specA.Data?.activationEffectPrefab ?? specB.Data?.activationEffectPrefab;
				float flyTime = 0.5f;
				Vector3 centerPos = (specA.transform.position + specB.transform.position) / 2f;
				specA.gameObject.SetActive(false);
				specB.gameObject.SetActive(false);
				List<GameObject> flyingBirds = new List<GameObject>();
				if (targets.Count > 0)
				{
					foreach (ElementView target4 in targets)
					{
						if (birdPrefab != null)
						{
							GameObject newBirdObj = UnityEngine.Object.Instantiate(birdPrefab, elementsContainer);
							newBirdObj.transform.position = centerPos;
							SpecialElementView newBirdView = newBirdObj.GetComponent<SpecialElementView>();
							if (newBirdView != null)
							{
								newBirdView.Initialize(specA.Data, cx, cy, centerPos, cellSize);
								newBirdView.PlayTriggerAnimation();
							}
							if (fxPrefab != null)
							{
								GameObject fx3 = UnityEngine.Object.Instantiate(fxPrefab, newBirdObj.transform.position, Quaternion.identity, newBirdObj.transform);
								fx3.transform.localPosition = Vector3.zero;
							}
							newBirdObj.transform.DOMove(target4.transform.position, flyTime).SetEase(Ease.InQuad);
							flyingBirds.Add(newBirdObj);
						}
						toDestroy.Add(target4);
						Vector2Int[] birdAreaOffsets = new Vector2Int[4]
						{
							Vector2Int.up,
							Vector2Int.down,
							Vector2Int.left,
							Vector2Int.right
						};
						Vector2Int[] array2 = birdAreaOffsets;
						for (int j = 0; j < array2.Length; j++)
						{
							Vector2Int offset = array2[j];
							int areaX = target4.GridX + offset.x;
							int areaY = target4.GridY + offset.y;
							if (areaX >= 0 && areaX < currentLevel.width && areaY >= 0 && areaY < currentLevel.height && activeElements[areaX, areaY] != null)
							{
								toDestroy.Add(activeElements[areaX, areaY]);
							}
						}
					}
					yield return new WaitForSeconds(flyTime);
				}
				foreach (GameObject b in flyingBirds)
				{
					if (b != null)
					{
						UnityEngine.Object.Destroy(b);
					}
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
			}
			else if (isMagicA && isMagicB)
			{
				isBoardClearing = true;
				float centerX = (float)(currentLevel.width - 1) / 2f;
				float centerY = (float)(currentLevel.height - 1) / 2f;
				Vector3 boardCenter = boardBottomLeftOrigin + new Vector3(centerX * (cellSize + cellSpacing), centerY * (cellSize + cellSpacing), 0f);
				ShowMagicBallFocus(specA.gameObject);
				RaiseRenderersAboveFocus(specB.gameObject);
				float rotateTime = 0.6f;
				specA.transform.DOMove(boardCenter, rotateTime).SetEase(Ease.InOutQuad);
				specB.transform.DOMove(boardCenter, rotateTime).SetEase(Ease.InOutQuad);
				yield return new WaitForSeconds(rotateTime);
				GameObject centerFxPrefab = specA.Data?.activationEffectPrefab ?? specB.Data?.activationEffectPrefab;
				if (centerFxPrefab != null)
				{
					GameObject fx4 = UnityEngine.Object.Instantiate(centerFxPrefab, boardCenter, Quaternion.identity);
					fx4.transform.localScale = Vector3.one * 1.5f;
					RaiseRenderersAboveFocus(fx4);
					UnityEngine.Object.Destroy(fx4, 2f);
				}
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlayMagicBallSwapSound();
				}
				SpecialElementView swapFxSource = null;
				if (specA != null && specA.swapParticlePrefab != null)
				{
					swapFxSource = specA;
				}
				else if (specB != null && specB.swapParticlePrefab != null)
				{
					swapFxSource = specB;
				}
				if (swapFxSource != null)
				{
					float swapFxDur = ((swapDuration > 0f) ? swapDuration : 2f);
					GameObject swapFx = swapFxSource.PlaySwapParticleEffect(boardCenter, swapFxDur);
					if (swapFx != null)
					{
						RaiseRenderersAboveFocus(swapFx);
					}
				}
				List<ElementView> allTargets = new List<ElementView>();
				for (int x9 = 0; x9 < currentLevel.width; x9++)
				{
					for (int y8 = 0; y8 < currentLevel.height; y8++)
					{
						if (activeElements[x9, y8] != null)
						{
							allTargets.Add(activeElements[x9, y8]);
						}
					}
				}
				foreach (ElementView target8 in allTargets)
				{
					if (target8 is NormalElementView norm3)
					{
						norm3.PlayChangeAnimation();
					}
					toDestroy.Add(target8);
				}
				if (swapDuration > 0f && swapAnimStartTime > 0f)
				{
					float remain = swapDuration - (Time.time - swapAnimStartTime);
					if (remain > 0.05f)
					{
						yield return new WaitForSeconds(remain);
					}
				}
				float fadeOutDur2 = 0.3f;
				PlayMagicBallFinishEffect((specA != null) ? specA : specB, boardCenter);
				HideMagicBallFocus();
				List<ElementView> immediateClearTargets = new List<ElementView>();
				foreach (ElementView target7 in toDestroy)
				{
					if (target7 != null && !immediateClearTargets.Contains(target7))
					{
						immediateClearTargets.Add(target7);
					}
				}
				if (immediateClearTargets.Count > 0)
				{
					DestroyElements(immediateClearTargets);
				}
				toDestroy.Clear();
				if (specA != null)
				{
					specA.transform.DOScale(Vector3.zero, fadeOutDur2).SetEase(Ease.InBack);
					if (specA.spineAnimation != null && specA.spineAnimation.skeleton != null)
					{
						DOTween.To(() => specA.spineAnimation.skeleton.A, delegate(float x)
						{
							specA.spineAnimation.skeleton.A = x;
						}, 0f, fadeOutDur2);
					}
					else if (specA.spriteRenderer != null)
					{
						specA.spriteRenderer.DOFade(0f, fadeOutDur2);
					}
				}
				if (specB != null)
				{
					specB.transform.DOScale(Vector3.zero, fadeOutDur2).SetEase(Ease.InBack);
					if (specB.spineAnimation != null && specB.spineAnimation.skeleton != null)
					{
						DOTween.To(() => specB.spineAnimation.skeleton.A, delegate(float x)
						{
							specB.spineAnimation.skeleton.A = x;
						}, 0f, fadeOutDur2);
					}
					else if (specB.spriteRenderer != null)
					{
						specB.spriteRenderer.DOFade(0f, fadeOutDur2);
					}
				}
				yield return new WaitForSeconds(fadeOutDur2);
				if (specA != null && specA.gameObject != null)
				{
					UnityEngine.Object.Destroy(specA.gameObject);
				}
				if (specB != null && specB.gameObject != null)
				{
					UnityEngine.Object.Destroy(specB.gameObject);
				}
			}
			else if ((isBirdA && isRocketB) || (isRocketA && isBirdB))
			{
				List<ElementView> targets3 = GetPriorityTargets(1);
				if (targets3.Count > 0)
				{
					ElementView target6 = targets3[0];
					int targetX2 = target6.GridX;
					int targetY2 = target6.GridY;
					toDestroy.Add(target6);
					float flyTime4 = 0.5f;
					specA.transform.DOMove(target6.transform.position, flyTime4).SetEase(Ease.InQuad);
					specB.transform.DOMove(target6.transform.position, flyTime4).SetEase(Ease.InQuad);
					yield return new WaitForSeconds(flyTime4);
					bool isHoriz = (isRocketA ? specA.BehaviorType : specB.BehaviorType) == SpecialBehaviorType.LineClearHorizontal;
					if (AudioManager.Instance != null)
					{
						AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.LineClearHorizontal);
					}
					SpecialElementView rocketSpec = (isRocketA ? specA : specB);
					GameObject trailPrefab2 = rocketSpec.Data?.trailEffectPrefab;
					if (trailPrefab2 != null)
					{
						Vector3[] dirs2 = ((!isHoriz) ? new Vector3[2]
						{
							Vector3.up,
							Vector3.down
						} : new Vector3[2]
						{
							Vector3.left,
							Vector3.right
						});
						Vector3[] array3 = dirs2;
						for (int k = 0; k < array3.Length; k++)
						{
							Vector3 d2 = array3[k];
							GameObject trail3 = UnityEngine.Object.Instantiate(trailPrefab2, target6.transform.position, Quaternion.identity);
							float angle3 = Mathf.Atan2(d2.y, d2.x) * 57.29578f;
							trail3.transform.rotation = Quaternion.Euler(0f, 0f, angle3);
							trail3.transform.DOMove(target6.transform.position + d2 * 20f, 1f).SetEase(Ease.Linear);
							UnityEngine.Object.Destroy(trail3, 1f);
						}
					}
					if (isHoriz)
					{
						for (int x8 = 0; x8 < currentLevel.width; x8++)
						{
							if (activeElements[x8, targetY2] != null && activeElements[x8, targetY2] != target6)
							{
								toDestroy.Add(activeElements[x8, targetY2]);
							}
						}
					}
					else
					{
						for (int y7 = 0; y7 < currentLevel.height; y7++)
						{
							if (activeElements[targetX2, y7] != null && activeElements[targetX2, y7] != target6)
							{
								toDestroy.Add(activeElements[targetX2, y7]);
							}
						}
					}
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
			}
			else if ((isBirdA && isBombB) || (isBombA && isBirdB))
			{
				List<ElementView> targets2 = GetPriorityTargets(1);
				if (targets2.Count > 0)
				{
					ElementView target5 = targets2[0];
					int targetX = target5.GridX;
					int targetY = target5.GridY;
					float flyTime3 = 0.5f;
					specA.transform.DOMove(target5.transform.position, flyTime3).SetEase(Ease.InQuad);
					specB.transform.DOMove(target5.transform.position, flyTime3).SetEase(Ease.InQuad);
					yield return new WaitForSeconds(flyTime3);
					if (AudioManager.Instance != null)
					{
						AudioManager.Instance.PlaySpecialTriggerSound(SpecialBehaviorType.BombExplosion);
					}
					SpecialElementView bombSpec = (isBombA ? specA : specB);
					GameObject fxPrefab2 = bombSpec.Data?.activationEffectPrefab;
					if (fxPrefab2 != null)
					{
						GameObject fx5 = UnityEngine.Object.Instantiate(fxPrefab2, target5.transform.position, Quaternion.identity);
						UnityEngine.Object.Destroy(fx5, 2f);
					}
					for (int x7 = targetX - 1; x7 <= targetX + 1; x7++)
					{
						for (int y6 = targetY - 1; y6 <= targetY + 1; y6++)
						{
							if (x7 >= 0 && x7 < currentLevel.width && y6 >= 0 && y6 < currentLevel.height && activeElements[x7, y6] != null)
							{
								toDestroy.Add(activeElements[x7, y6]);
							}
						}
					}
				}
				DestroyElementVisual(specA);
				DestroyElementVisual(specB);
			}
			else if ((isMagicA && isRocketB) || (isRocketA && isMagicB) || (isMagicA && isBombB) || (isBombA && isMagicB) || (isMagicA && isBirdB) || (isBirdA && isMagicB))
			{
				SpecialElementView magicBall = (isMagicA ? specA : specB);
				SpecialElementView otherSpec = (isMagicA ? specB : specA);
				bool otherSpecialTriggeredDirectly = false;
				List<int> colors = new List<int>();
				for (int x6 = 0; x6 < currentLevel.width; x6++)
				{
					for (int y = 0; y < currentLevel.height; y++)
					{
						ElementView elementView = activeElements[x6, y];
						NormalElementView norm = elementView as NormalElementView;
						if ((object)norm != null && !colors.Contains(norm.NormalElementId))
						{
							colors.Add(norm.NormalElementId);
						}
					}
				}
				if (colors.Count > 0)
				{
					int targetColor = colors[UnityEngine.Random.Range(0, colors.Count)];
					magicBall.PlayTriggerAnimation(true);
					GameObject trailPrefab = magicBall.Data?.trailEffectPrefab;
					float flyTime2 = 0.5f;
					List<ElementView> convertedElements = new List<ElementView>();
					for (int x5 = 0; x5 < currentLevel.width; x5++)
					{
						for (int y2 = 0; y2 < currentLevel.height; y2++)
						{
							ElementView elementView = activeElements[x5, y2];
							NormalElementView norm2 = elementView as NormalElementView;
							if ((object)norm2 != null && norm2.NormalElementId == targetColor)
							{
								convertedElements.Add(norm2);
							}
						}
					}
					List<SpecialElementData> horizRockets = elementDb.specialElements.FindAll((SpecialElementData e) => e.behaviorType == SpecialBehaviorType.LineClearHorizontal);
					List<SpecialElementData> vertRockets = elementDb.specialElements.FindAll((SpecialElementData e) => e.behaviorType == SpecialBehaviorType.LineClearVertical);
					SpecialElementData bombData = elementDb.specialElements.Find((SpecialElementData e) => e.behaviorType == SpecialBehaviorType.BombExplosion);
					SpecialElementData birdData = elementDb.specialElements.Find((SpecialElementData e) => e.behaviorType == SpecialBehaviorType.TargetSeeker);
					bool generateSoundPlayed = false;
					Action<ElementView> convertOnRayArrival = delegate(ElementView target)
					{
						if (!(target == null) && !(target.gameObject == null) && !(activeElements[target.GridX, target.GridY] != target))
						{
							if (!generateSoundPlayed && AudioManager.Instance != null)
							{
								generateSoundPlayed = true;
								AudioManager.Instance.PlayMagicBallGenerateSound();
							}
							int gridX = target.GridX;
							int gridY = target.GridY;
							if (target is NormalElementView normalElementView)
							{
								normalElementView.clearEffectAlreadyPlayed = true;
								SpawnNormalClearEffect(normalElementView);
							}
							activeElements[gridX, gridY] = null;
							DestroyElementVisual(target);
							SpecialElementData specialElementData = null;
							if (isRocketA || isRocketB)
							{
								bool flag = UnityEngine.Random.value > 0.5f;
								if (flag && horizRockets.Count > 0)
								{
									specialElementData = horizRockets[0];
								}
								else if (!flag && vertRockets.Count > 0)
								{
									specialElementData = vertRockets[0];
								}
								else if (horizRockets.Count > 0)
								{
									specialElementData = horizRockets[0];
								}
								else if (vertRockets.Count > 0)
								{
									specialElementData = vertRockets[0];
								}
							}
							else if (isBombA || isBombB)
							{
								specialElementData = bombData;
							}
							else if (isBirdA || isBirdB)
							{
								specialElementData = birdData;
							}
							if (specialElementData != null && specialElementData.prefab != null)
							{
								GameObject gameObject = UnityEngine.Object.Instantiate(specialElementData.prefab, elementsContainer);
								SpecialElementView specialView = gameObject.GetComponent<SpecialElementView>();
								if (specialView != null)
								{
									specialView.Initialize(specialElementData, gridX, gridY, GetWorldPosition(gridX, gridY), cellSize);
									activeElements[gridX, gridY] = specialView;
									Vector3 localScale = gameObject.transform.localScale;
									gameObject.transform.localScale = Vector3.zero;
									gameObject.transform.DOScale(localScale, 0.25f).SetEase(Ease.OutBack);
									float creationAnimationDuration = specialView.GetCreationAnimationDuration();
									float creationDelay = Mathf.Clamp(creationAnimationDuration * 0.6f, 0.2f, 0.5f);
									StartTrackedSpecial(() => ConvertedSpecialTriggerRoutine(specialView, creationDelay));
								}
							}
						}
					};
					if (trailPrefab != null)
					{
						Vector3 magicBallStartPos = GetWorldPosition(magicBall.GridX, magicBall.GridY);
						foreach (ElementView target2 in convertedElements)
						{
							Vector3 targetPos = target2.transform.position;
							Vector3 dir = (targetPos - magicBallStartPos).normalized;
							GameObject trail = UnityEngine.Object.Instantiate(trailPrefab, magicBallStartPos, Quaternion.identity);
							float angle2 = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
							trail.transform.rotation = Quaternion.Euler(0f, 0f, angle2);
							trail.transform.DOMove(targetPos, flyTime2).SetEase(Ease.Linear).OnComplete(delegate
							{
								convertOnRayArrival(target2);
								if (trail != null)
								{
									UnityEngine.Object.Destroy(trail);
								}
							});
							UnityEngine.Object.Destroy(trail, flyTime2 + 0.5f);
						}
						yield return new WaitForSeconds(flyTime2);
					}
					else
					{
						foreach (ElementView target3 in convertedElements)
						{
							convertOnRayArrival(target3);
						}
					}
				}
				else
				{
					otherSpecialTriggeredDirectly = true;
					yield return StartCoroutine(ActivateSpecial(otherSpec, null, true));
				}
				SpecialElementView mBall = magicBall;
				float fadeOutDur = 0.3f;
				if (mBall != null)
				{
					mBall.transform.DOScale(Vector3.zero, fadeOutDur).SetEase(Ease.InBack);
					if (mBall.spineAnimation != null && mBall.spineAnimation.skeleton != null)
					{
						DOTween.To(() => mBall.spineAnimation.skeleton.A, delegate(float x)
						{
							mBall.spineAnimation.skeleton.A = x;
						}, 0f, fadeOutDur);
					}
					else if (mBall.spriteRenderer != null)
					{
						mBall.spriteRenderer.DOFade(0f, fadeOutDur);
					}
				}
				if (!otherSpecialTriggeredDirectly)
				{
					DestroyElementVisual(otherSpec);
				}
				yield return new WaitForSeconds(fadeOutDur);
				if (mBall != null && mBall.gameObject != null)
				{
					UnityEngine.Object.Destroy(mBall.gameObject);
				}
			}
			List<ElementView> finalToDestroy = new List<ElementView>();
			foreach (ElementView el in toDestroy)
			{
				if (!finalToDestroy.Contains(el))
				{
					finalToDestroy.Add(el);
				}
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

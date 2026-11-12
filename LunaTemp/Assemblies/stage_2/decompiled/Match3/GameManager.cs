using System.Collections;
using System.Collections.Generic;
using Luna.Unity;
using Manager;
using Match3.UI;
using UnityEngine;

namespace Match3
{
	public class GameManager : MonoBehaviour
	{
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

		[Header("Data References")]
		public List<LevelData> levelDataList = new List<LevelData>();

		public ElementDatabase elementDatabase;

		[Header("Scene References")]
		public GameUIManager uiManager;

		public BoardManager boardManager;

		[Header("Tutorial References")]
		public Sprite tutorialFingerSprite;

		[Header("Startup Loading Screen")]
		[SerializeField]
		[Min(0f)]
		private float simulatedLoadingDuration = 1.6f;

		private int currentSteps;

		public int playedSteps = 0;

		public bool isAdShowing = false;

		public List<RuntimeTarget> activeTargets = new List<RuntimeTarget>();

		public GuideManager guideManager;

		public LevelData currentLevelData => (levelDataList != null && levelDataList.Count > 0 && currentLevelIndex < levelDataList.Count) ? levelDataList[currentLevelIndex] : null;

		public int currentLevelIndex { get; private set; } = 0;


		public bool isGameOver { get; private set; } = false;


		private IEnumerator Start()
		{
			RuntimeLoadingScreen loadingScreen = RuntimeLoadingScreen.Create();
			if (levelDataList != null && levelDataList.Count > 0 && uiManager != null && elementDatabase != null)
			{
				guideManager = base.gameObject.AddComponent<GuideManager>();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				float elapsed = 0f;
				while (elapsed < simulatedLoadingDuration)
				{
					elapsed += Time.unscaledDeltaTime;
					float normalizedProgress = ((simulatedLoadingDuration <= 0f) ? 1f : Mathf.Clamp01(elapsed / simulatedLoadingDuration));
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
			if (AudioManager.Instance != null && AudioManager.Instance.bgmClip != null && AudioManager.Instance.bgmSource != null && (!AudioManager.Instance.bgmSource.isPlaying || AudioManager.Instance.bgmSource.clip != AudioManager.Instance.bgmClip))
			{
				AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmClip);
			}
			currentSteps = currentLevelData.maxSteps;
			playedSteps = 0;
			activeTargets.Clear();
			List<TargetData> uiTargets = new List<TargetData>();
			foreach (LevelTarget target in currentLevelData.levelTargets)
			{
				RuntimeTarget rt = new RuntimeTarget();
				rt.definition = target;
				rt.remainingAmount = target.requiredAmount;
				activeTargets.Add(rt);
				Sprite iconSprite = null;
				if (target.targetType == CellItemType.SpecificNormal)
				{
					NormalElementInfo norm = elementDatabase.GetNormalElement(target.normalElementId);
					if (norm != null)
					{
						iconSprite = norm.sprite;
					}
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
						SpriteRenderer sr2 = target.obstacleData.prefab.GetComponentInChildren<SpriteRenderer>();
						if (sr2 != null)
						{
							iconSprite = sr2.sprite;
						}
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
				else if (target.targetType == CellItemType.Child && target.childData != null && target.childData.prefab != null)
				{
					SpriteRenderer sr = target.childData.prefab.GetComponentInChildren<SpriteRenderer>();
					if (sr != null)
					{
						iconSprite = sr.sprite;
					}
				}
				uiTargets.Add(new TargetData
				{
					icon = iconSprite,
					requiredAmount = target.requiredAmount
				});
			}
			uiManager.InitializeUI("Download Now for more levels!", currentSteps);
			uiManager.SetupTargets(uiTargets);
			if (boardManager != null)
			{
				boardManager.gameManager = this;
				boardManager.InitializeBoard(currentLevelData, elementDatabase, uiManager.boardAreaRect);
				if (guideManager == null)
				{
					guideManager = base.gameObject.AddComponent<GuideManager>();
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

		public void OnPlayerMove()
		{
			if (!isGameOver)
			{
				currentSteps--;
				playedSteps++;
				uiManager.UpdateSteps(currentSteps);
				CheckWinCondition();
			}
		}

		public void TriggerGlobalAd()
		{
			if (isAdShowing)
			{
				return;
			}
			isAdShowing = true;
			if (boardManager != null)
			{
				boardManager.ClearMagicFocusMask();
				if (boardManager.backgroundContainer != null)
				{
					boardManager.backgroundContainer.gameObject.SetActive(false);
				}
				if (boardManager.elementsContainer != null)
				{
					boardManager.elementsContainer.gameObject.SetActive(false);
				}
			}
			if (uiManager != null && uiManager.centerBorderRect != null)
			{
				uiManager.centerBorderRect.gameObject.SetActive(false);
			}
			if (uiManager != null)
			{
				if (ConfigManager.Instance != null && ConfigManager.Instance.adTriggerType == AdTriggerType.DownloadPanel)
				{
					uiManager.ShowDownloadPanel();
				}
				else
				{
					uiManager.ShowGlobalAdOverlay();
				}
			}
		}

		public int OnElementCollected(CellItemType type, int normalId = -1, SpecialElementData special = null, CollectibleElementData collectible = null, ObstacleData obstacle = null, GeneratorData generator = null, MotherData mother = null, ChildData child = null)
		{
			if (isGameOver)
			{
				return -1;
			}
			bool UIneedsUpdate = false;
			int collectedTargetIndex = -1;
			for (int i = 0; i < activeTargets.Count; i++)
			{
				RuntimeTarget rt = activeTargets[i];
				if (rt.remainingAmount <= 0)
				{
					continue;
				}
				bool isMatch = false;
				if (rt.definition.targetType == type)
				{
					if (type == CellItemType.SpecificNormal && rt.definition.normalElementId == normalId)
					{
						isMatch = true;
					}
					else if (type == CellItemType.Special && rt.definition.specialElement == special)
					{
						isMatch = true;
					}
					else if (type == CellItemType.RetractingCollectible && rt.definition.collectibleData == collectible)
					{
						isMatch = true;
					}
					else if (type == CellItemType.Obstacle && rt.definition.obstacleData == obstacle)
					{
						isMatch = true;
					}
					else if (type == CellItemType.Generator && rt.definition.generatorData == generator)
					{
						isMatch = true;
					}
					else if (type == CellItemType.Mother && rt.definition.motherData == mother)
					{
						isMatch = true;
					}
					else if (type == CellItemType.Child && rt.definition.childData == child)
					{
						isMatch = true;
					}
				}
				if (isMatch)
				{
					rt.remainingAmount--;
					uiManager.UpdateTarget(i, rt.remainingAmount);
					UIneedsUpdate = true;
					collectedTargetIndex = i;
					break;
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
			if (isGameOver)
			{
				return;
			}
			bool allTargetsMet = true;
			foreach (RuntimeTarget rt in activeTargets)
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
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlayVictorySound();
				}
				bool hasNextLevel = levelDataList != null && currentLevelIndex < levelDataList.Count - 1;
				StartCoroutine(ShowSettlementUIWithDelay(true, hasNextLevel));
			}
			else if (currentSteps <= 0 && (!(boardManager != null) || boardManager.currentState == BoardManager.BoardState.Waiting))
			{
				isGameOver = true;
				Debug.Log("Out of Steps! You Lose!");
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlayDefeatSound();
				}
				StartCoroutine(ShowSettlementUIWithDelay(false));
			}
		}

		private IEnumerator ShowSettlementUIWithDelay(bool isWin, bool hasNextLevel = false)
		{
			yield return new WaitForSeconds(1f);
			if (boardManager != null)
			{
				yield return new WaitUntil(() => boardManager.currentState == BoardManager.BoardState.Waiting);
			}
			if (isWin && hasNextLevel && ConfigManager.Instance.levelTransitionType == LevelTransitionType.Fade)
			{
				if (uiManager != null)
				{
					PlaySettlementSound();
					yield return StartCoroutine(PlayVictorySettlementAboveBoard());
				}
				if (uiManager != null)
				{
					yield return StartCoroutine(uiManager.FadeOutCoroutine(0.5f));
				}
				currentLevelIndex++;
				StartGame();
				if (uiManager != null)
				{
					yield return StartCoroutine(uiManager.FadeInCoroutine(0.5f));
				}
				yield break;
			}
			if (isWin && uiManager != null)
			{
				PlaySettlementSound();
				yield return StartCoroutine(PlayVictorySettlementAboveBoard());
			}
			if (boardManager != null)
			{
				if (boardManager.backgroundContainer != null)
				{
					boardManager.backgroundContainer.gameObject.SetActive(false);
				}
				if (boardManager.elementsContainer != null)
				{
					boardManager.elementsContainer.gameObject.SetActive(false);
				}
			}
			if (uiManager != null && uiManager.centerBorderRect != null)
			{
				uiManager.centerBorderRect.gameObject.SetActive(false);
			}
			if (!(uiManager != null))
			{
				yield break;
			}
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

		private void PlaySettlementSound()
		{
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySettlementSound();
			}
		}

		private IEnumerator PlayVictorySettlementAboveBoard()
		{
			if (uiManager == null)
			{
				yield break;
			}
			if (guideManager != null)
			{
				guideManager.ClearGuideInstantly();
				uiManager.SetTutorialUIMask(false);
			}
			if (boardManager != null)
			{
				boardManager.ClearMagicFocusMask();
			}
			Dictionary<Renderer, RendererSortingState> boardRendererStates = new Dictionary<Renderer, RendererSortingState>();
			SetBoardSettlementSorting((boardManager != null) ? boardManager.backgroundContainer : null, boardRendererStates);
			SetBoardSettlementSorting((boardManager != null) ? boardManager.elementsContainer : null, boardRendererStates);
			yield return StartCoroutine(uiManager.PlayVictorySettlement());
			foreach (KeyValuePair<Renderer, RendererSortingState> entry in boardRendererStates)
			{
				if (!(entry.Key == null))
				{
					entry.Key.sortingLayerID = entry.Value.sortingLayerID;
					entry.Key.sortingOrder = entry.Value.sortingOrder;
				}
			}
		}

		private static void SetBoardSettlementSorting(Transform root, Dictionary<Renderer, RendererSortingState> states)
		{
			if (root == null)
			{
				return;
			}
			Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (!(renderer == null) && !states.ContainsKey(renderer))
				{
					states.Add(renderer, new RendererSortingState(renderer));
					renderer.sortingLayerName = "Default";
					renderer.sortingOrder = 30000;
				}
			}
		}

		public void LoadNextLevel()
		{
			if (levelDataList == null || currentLevelIndex >= levelDataList.Count - 1)
			{
				return;
			}
			currentLevelIndex++;
			if (uiManager != null)
			{
				uiManager.HideNextLevelPanel();
			}
			if (boardManager != null)
			{
				if (boardManager.backgroundContainer != null)
				{
					boardManager.backgroundContainer.gameObject.SetActive(true);
				}
				if (boardManager.elementsContainer != null)
				{
					boardManager.elementsContainer.gameObject.SetActive(true);
				}
			}
			if (uiManager != null && uiManager.centerBorderRect != null)
			{
				uiManager.centerBorderRect.gameObject.SetActive(true);
			}
			StartGame();
		}

		public void DownloadGame()
		{
			Playable.InstallFullGame();
		}
	}
}

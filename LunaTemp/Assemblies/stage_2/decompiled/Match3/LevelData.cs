using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "NewLevelData", menuName = "Match3/Level Data")]
	public class LevelData : ScriptableObject
	{
		[Header("Game Rules")]
		public int maxSteps = 30;

		public List<LevelTarget> levelTargets = new List<LevelTarget>();

		[Header("Tutorial Settings")]
		public bool hasTutorial = false;

		public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

		[HideInInspector]
		public Vector2Int tutorialSwipeFrom;

		[HideInInspector]
		public Vector2Int tutorialSwipeTo;

		[HideInInspector]
		public List<Vector2Int> tutorialHighlightCells = new List<Vector2Int>();

		[HideInInspector]
		public float tutorialFingerScale = 1f;

		[HideInInspector]
		public Vector2 tutorialFingerOffset = Vector2.zero;

		[Header("Grid Size")]
		public int width = 8;

		public int height = 8;

		[Header("Grid Cells")]
		public List<GridCellData> cells = new List<GridCellData>();

		[Header("Column Refill Sequences")]
		[Tooltip("The list index is the column index. Random normal elements are used after a sequence is exhausted.")]
		public List<ColumnRefillSequence> columnRefillSequences = new List<ColumnRefillSequence>();

		public GridCellData GetCell(int x, int y)
		{
			return cells.Find((GridCellData c) => c.x == x && c.y == y);
		}

		public void ValidateGrid()
		{
			List<GridCellData> newCells = new List<GridCellData>();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					GridCellData existing = GetCell(x, y);
					if (existing != null)
					{
						newCells.Add(existing);
						continue;
					}
					newCells.Add(new GridCellData
					{
						x = x,
						y = y,
						isPlayable = true
					});
				}
			}
			cells = newCells;
		}
	}
}

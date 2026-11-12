using UnityEngine;
using System.Collections.Generic;

namespace Match3
{
    public enum CellItemType
    {
        None,
        RandomNormal,
        SpecificNormal,
        Special,
        RetractingCollectible,
        Obstacle,
        Generator,
        Mother,
        Child
    }

    [System.Serializable]
    public class GridCellData
    {
        public int x;
        public int y;
        public bool isPlayable = true; // False for holes/empty spaces
        
        public CellItemType itemType = CellItemType.None;
        
        // Used if itemType == SpecificNormal
        public int normalElementId = -1; 
        
        // Used if itemType == Special
        public SpecialElementData specialElement; 
        
        // Used if itemType == RetractingCollectible
        public CollectibleElementData collectibleData;
        public Direction collectibleDirection = Direction.Up;
        public int collectibleLength = 3;

        // Used if itemType == Obstacle
        public ObstacleData obstacleData;
        
        // Used if itemType == Generator
        public GeneratorData generatorData;
        
        // Used if itemType == Mother
        public MotherData motherData;
        
        // Used if itemType == Child
        public ChildData childData;

        // Conveyor Settings
        public bool hasConveyor = false;
        public Direction conveyorDirection = Direction.Up;
    }

    [System.Serializable]
    public class LevelTarget
    {
        public CellItemType targetType;
        // Used if targetType == SpecificNormal
        public int normalElementId = -1;
        // Used if targetType == Special
        public SpecialElementData specialElement;
        // Used if targetType == RetractingCollectible
        public CollectibleElementData collectibleData;
        // Used if targetType == Obstacle
        public ObstacleData obstacleData;
        // Used if targetType == Generator
        public GeneratorData generatorData;
        // Used if targetType == Mother
        public MotherData motherData;
        // Used if targetType == Child
        public ChildData childData;
        
        public int requiredAmount;
    }

    [System.Serializable]
    public class TutorialStep
    {
        public Vector2Int tutorialSwipeFrom;
        public Vector2Int tutorialSwipeTo;
        // List of grid coordinates that should be highlighted during the tutorial
        public List<Vector2Int> tutorialHighlightCells = new List<Vector2Int>();
        // Scale for the tutorial finger
        public float tutorialFingerScale = 1f;
        // Offset for the tutorial finger position
        public Vector2 tutorialFingerOffset = Vector2.zero;
        // Whether to show dark mask
        public bool showMask = true;
        // Whether to show a popup reminder
        public bool showPopup = false;
        // The prefab to instantiate for the popup
        public GameObject popupPrefab;
        // Offset for the popup position
        public Vector2 popupOffset = Vector2.zero;
    }

    [System.Serializable]
    public class ColumnRefillSequence
    {
        [Tooltip("Normal element IDs spawned in order for this column")]
        public List<int> normalElementIds = new List<int>();
    }

    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Match3/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Game Rules")]
        public int maxSteps = 30;
        public List<LevelTarget> levelTargets = new List<LevelTarget>();

        [Header("Tutorial Settings")]
        public bool hasTutorial = false;
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

        [HideInInspector] public Vector2Int tutorialSwipeFrom;
        [HideInInspector] public Vector2Int tutorialSwipeTo;
        [HideInInspector] public List<Vector2Int> tutorialHighlightCells = new List<Vector2Int>();
        [HideInInspector] public float tutorialFingerScale = 1f;
        [HideInInspector] public Vector2 tutorialFingerOffset = Vector2.zero;

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
            return cells.Find(c => c.x == x && c.y == y);
        }

        public void ValidateGrid()
        {
            List<GridCellData> newCells = new List<GridCellData>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var existing = GetCell(x, y);
                    if (existing != null)
                    {
                        newCells.Add(existing);
                    }
                    else
                    {
                        newCells.Add(new GridCellData { x = x, y = y, isPlayable = true });
                    }
                }
            }
            cells = newCells;
        }
    }
}

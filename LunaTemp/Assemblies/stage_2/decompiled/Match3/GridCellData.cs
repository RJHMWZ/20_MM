using System;

namespace Match3
{
	[Serializable]
	public class GridCellData
	{
		public int x;

		public int y;

		public bool isPlayable = true;

		public CellItemType itemType = CellItemType.None;

		public int normalElementId = -1;

		public SpecialElementData specialElement;

		public CollectibleElementData collectibleData;

		public Direction collectibleDirection = Direction.Up;

		public int collectibleLength = 3;

		public ObstacleData obstacleData;

		public GeneratorData generatorData;

		public MotherData motherData;

		public ChildData childData;

		public bool hasConveyor = false;

		public Direction conveyorDirection = Direction.Up;
	}
}

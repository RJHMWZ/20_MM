using System;

namespace Match3
{
	[Serializable]
	public class LevelTarget
	{
		public CellItemType targetType;

		public int normalElementId = -1;

		public SpecialElementData specialElement;

		public CollectibleElementData collectibleData;

		public ObstacleData obstacleData;

		public GeneratorData generatorData;

		public MotherData motherData;

		public ChildData childData;

		public int requiredAmount;
	}
}

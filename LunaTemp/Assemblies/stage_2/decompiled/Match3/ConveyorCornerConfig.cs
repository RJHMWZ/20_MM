using System;

namespace Match3
{
	[Serializable]
	public class ConveyorCornerConfig
	{
		public Direction inDir;

		public Direction outDir;

		public float rotationAngle;

		public bool flipX;

		public bool flipY;

		public string animationName = "line_ru";
	}
}

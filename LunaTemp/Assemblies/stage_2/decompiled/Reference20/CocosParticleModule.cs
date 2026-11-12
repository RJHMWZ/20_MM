using System;

namespace Reference20
{
	[Serializable]
	public sealed class CocosParticleModule
	{
		public bool separateAxes;

		public int numTilesX = 1;

		public int numTilesY = 1;

		public int animation;

		public int rowIndex;

		public bool randomRow;

		public float dampen;

		public float cycleCount = 1f;

		public float flipU;

		public float flipV;

		public CocosParticleRange size;

		public CocosParticleRange x;

		public CocosParticleRange y;

		public CocosParticleRange z;

		public CocosParticleRange limit;

		public CocosParticleRange limitX;

		public CocosParticleRange limitY;

		public CocosParticleRange limitZ;

		public CocosParticleRange frameOverTime;

		public CocosParticleRange startFrame;

		public CocosParticleGradient color;
	}
}

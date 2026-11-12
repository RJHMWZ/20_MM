using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CocosParticleRange
	{
		public int mode;

		public float[] min;

		public float[] max;

		public float Evaluate(float t, float random)
		{
			if (min == null || min.Length == 0)
			{
				return 0f;
			}
			float frame = Mathf.Clamp01(t) * (float)(min.Length - 1);
			int a = (int)frame;
			int b = Mathf.Min(a + 1, min.Length - 1);
			return Mathf.LerpUnclamped(Mathf.LerpUnclamped(min[a], min[b], frame - (float)a), Mathf.LerpUnclamped(max[a], max[b], frame - (float)a), random);
		}
	}
}

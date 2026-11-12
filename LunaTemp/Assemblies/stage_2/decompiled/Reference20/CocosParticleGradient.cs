using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CocosParticleGradient
	{
		public int mode;

		public float[] times;

		public float[][] min;

		public float[][] max;

		public float[][] randomColors;

		private static Color ColorAt(float[] c)
		{
			return new Color(c[0], c[1], c[2], c[3]);
		}

		public Color Evaluate(float t, float random)
		{
			if (times == null || times.Length == 0)
			{
				return Color.white;
			}
			if (mode == 4 && randomColors != null && randomColors.Length != 0)
			{
				return ColorAt(randomColors[Mathf.Min(randomColors.Length - 1, (int)(random * (float)randomColors.Length))]);
			}
			int a;
			for (a = 0; a + 1 < times.Length && times[a + 1] <= t; a++)
			{
			}
			int b = Mathf.Min(a + 1, times.Length - 1);
			float f = ((times[b] > times[a]) ? Mathf.Clamp01((t - times[a]) / (times[b] - times[a])) : 0f);
			return Color.LerpUnclamped(Color.LerpUnclamped(ColorAt(min[a]), ColorAt(min[b]), f), Color.LerpUnclamped(ColorAt(max[a]), ColorAt(max[b]), f), random);
		}
	}
}

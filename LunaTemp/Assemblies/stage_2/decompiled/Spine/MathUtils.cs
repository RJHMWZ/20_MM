using System;

namespace Spine
{
	public static class MathUtils
	{
		public const float PI = 3.14159265f;

		public const float PI2 = 3.14159265f * 2f;

		public const float RadDeg = 180f / 3.14159265f;

		public const float DegRad = 3.14159265f / 180f;

		private static Random random = new Random();

		public static float Sin(float radians)
		{
			return (float)Math.Sin(radians);
		}

		public static float Cos(float radians)
		{
			return (float)Math.Cos(radians);
		}

		public static float SinDeg(float degrees)
		{
			return (float)Math.Sin(degrees * (3.14159265f / 180f));
		}

		public static float CosDeg(float degrees)
		{
			return (float)Math.Cos(degrees * (3.14159265f / 180f));
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2(y, x);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static float RandomTriangle(float min, float max)
		{
			return RandomTriangle(min, max, (min + max) * 0.5f);
		}

		public static float RandomTriangle(float min, float max, float mode)
		{
			float u = (float)random.NextDouble();
			float d = max - min;
			if (u <= (mode - min) / d)
			{
				return min + (float)Math.Sqrt(u * d * (mode - min));
			}
			return max - (float)Math.Sqrt((1f - u) * d * (max - mode));
		}
	}
}

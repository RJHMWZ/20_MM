using System;

namespace Spine
{
	public abstract class CurveTimeline : Timeline
	{
		protected const float LINEAR = 0f;

		protected const float STEPPED = 1f;

		protected const float BEZIER = 2f;

		protected const int BEZIER_SIZE = 19;

		internal float[] curves;

		public int FrameCount => curves.Length / 19 + 1;

		public abstract int PropertyId { get; }

		public CurveTimeline(int frameCount)
		{
			if (frameCount <= 0)
			{
				throw new ArgumentOutOfRangeException("frameCount must be > 0: ");
			}
			curves = new float[(frameCount - 1) * 19];
		}

		public abstract void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction);

		public void SetLinear(int frameIndex)
		{
			curves[frameIndex * 19] = 0f;
		}

		public void SetStepped(int frameIndex)
		{
			curves[frameIndex * 19] = 1f;
		}

		public float GetCurveType(int frameIndex)
		{
			int index = frameIndex * 19;
			if (index == curves.Length)
			{
				return 0f;
			}
			float type = curves[index];
			if (type == 0f)
			{
				return 0f;
			}
			if (type == 1f)
			{
				return 1f;
			}
			return 2f;
		}

		public void SetCurve(int frameIndex, float cx1, float cy1, float cx2, float cy2)
		{
			float tmpx = ((0f - cx1) * 2f + cx2) * 0.03f;
			float tmpy = ((0f - cy1) * 2f + cy2) * 0.03f;
			float dddfx = ((cx1 - cx2) * 3f + 1f) * 0.006f;
			float dddfy = ((cy1 - cy2) * 3f + 1f) * 0.006f;
			float ddfx = tmpx * 2f + dddfx;
			float ddfy = tmpy * 2f + dddfy;
			float dfx = cx1 * 0.3f + tmpx + dddfx * (1f / 6f);
			float dfy = cy1 * 0.3f + tmpy + dddfy * (1f / 6f);
			int i = frameIndex * 19;
			float[] curves = this.curves;
			curves[i++] = 2f;
			float x = dfx;
			float y = dfy;
			for (int j = i + 19 - 1; i < j; i += 2)
			{
				curves[i] = x;
				curves[i + 1] = y;
				dfx += ddfx;
				dfy += ddfy;
				ddfx += dddfx;
				ddfy += dddfy;
				x += dfx;
				y += dfy;
			}
		}

		public float GetCurvePercent(int frameIndex, float percent)
		{
			percent = MathUtils.Clamp(percent, 0f, 1f);
			float[] curves = this.curves;
			int i = frameIndex * 19;
			float type = curves[i];
			if (type == 0f)
			{
				return percent;
			}
			if (type == 1f)
			{
				return 0f;
			}
			i++;
			float x = 0f;
			int start = i;
			for (int j = i + 19 - 1; i < j; i += 2)
			{
				x = curves[i];
				if (x >= percent)
				{
					if (i == start)
					{
						return curves[i + 1] * percent / x;
					}
					float prevX = curves[i - 2];
					float prevY = curves[i - 1];
					return prevY + (curves[i + 1] - prevY) * (percent - prevX) / (x - prevX);
				}
			}
			float y = curves[i - 1];
			return y + (1f - y) * (percent - x) / (1f - x);
		}
	}
}

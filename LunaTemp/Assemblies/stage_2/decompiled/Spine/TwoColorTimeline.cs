using System;

namespace Spine
{
	public class TwoColorTimeline : CurveTimeline, ISlotTimeline
	{
		public const int ENTRIES = 8;

		protected const int PREV_TIME = -8;

		protected const int PREV_R = -7;

		protected const int PREV_G = -6;

		protected const int PREV_B = -5;

		protected const int PREV_A = -4;

		protected const int PREV_R2 = -3;

		protected const int PREV_G2 = -2;

		protected const int PREV_B2 = -1;

		protected const int R = 1;

		protected const int G = 2;

		protected const int B = 3;

		protected const int A = 4;

		protected const int R2 = 5;

		protected const int G2 = 6;

		protected const int B2 = 7;

		internal int slotIndex;

		internal float[] frames;

		public override int PropertyId => 234881024 + slotIndex;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				slotIndex = value;
			}
		}

		public float[] Frames => frames;

		public TwoColorTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 8];
		}

		public void SetFrame(int frameIndex, float time, float r, float g, float b, float a, float r2, float g2, float b2)
		{
			frameIndex *= 8;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = r;
			frames[frameIndex + 2] = g;
			frames[frameIndex + 3] = b;
			frames[frameIndex + 4] = a;
			frames[frameIndex + 5] = r2;
			frames[frameIndex + 6] = g2;
			frames[frameIndex + 7] = b2;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (!slot.bone.active)
			{
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				SlotData slotData = slot.data;
				switch (blend)
				{
				case MixBlend.Setup:
					slot.r = slotData.r;
					slot.g = slotData.g;
					slot.b = slotData.b;
					slot.a = slotData.a;
					slot.ClampColor();
					slot.r2 = slotData.r2;
					slot.g2 = slotData.g2;
					slot.b2 = slotData.b2;
					slot.ClampSecondColor();
					break;
				case MixBlend.First:
					slot.r += (slot.r - slotData.r) * alpha;
					slot.g += (slot.g - slotData.g) * alpha;
					slot.b += (slot.b - slotData.b) * alpha;
					slot.a += (slot.a - slotData.a) * alpha;
					slot.ClampColor();
					slot.r2 += (slot.r2 - slotData.r2) * alpha;
					slot.g2 += (slot.g2 - slotData.g2) * alpha;
					slot.b2 += (slot.b2 - slotData.b2) * alpha;
					slot.ClampSecondColor();
					break;
				}
				return;
			}
			float r;
			float g;
			float b;
			float a;
			float r2;
			float g2;
			float b2;
			if (time >= frames[frames.Length - 8])
			{
				int i = frames.Length;
				r = frames[i + -7];
				g = frames[i + -6];
				b = frames[i + -5];
				a = frames[i + -4];
				r2 = frames[i + -3];
				g2 = frames[i + -2];
				b2 = frames[i + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 8);
				r = frames[frame + -7];
				g = frames[frame + -6];
				b = frames[frame + -5];
				a = frames[frame + -4];
				r2 = frames[frame + -3];
				g2 = frames[frame + -2];
				b2 = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 8 - 1, 1f - (time - frameTime) / (frames[frame + -8] - frameTime));
				r += (frames[frame + 1] - r) * percent;
				g += (frames[frame + 2] - g) * percent;
				b += (frames[frame + 3] - b) * percent;
				a += (frames[frame + 4] - a) * percent;
				r2 += (frames[frame + 5] - r2) * percent;
				g2 += (frames[frame + 6] - g2) * percent;
				b2 += (frames[frame + 7] - b2) * percent;
			}
			if (alpha == 1f)
			{
				slot.r = r;
				slot.g = g;
				slot.b = b;
				slot.a = a;
				slot.ClampColor();
				slot.r2 = r2;
				slot.g2 = g2;
				slot.b2 = b2;
				slot.ClampSecondColor();
				return;
			}
			float br;
			float bg;
			float bb;
			float ba;
			float br2;
			float bg2;
			float bb2;
			if (blend == MixBlend.Setup)
			{
				br = slot.data.r;
				bg = slot.data.g;
				bb = slot.data.b;
				ba = slot.data.a;
				br2 = slot.data.r2;
				bg2 = slot.data.g2;
				bb2 = slot.data.b2;
			}
			else
			{
				br = slot.r;
				bg = slot.g;
				bb = slot.b;
				ba = slot.a;
				br2 = slot.r2;
				bg2 = slot.g2;
				bb2 = slot.b2;
			}
			slot.r = br + (r - br) * alpha;
			slot.g = bg + (g - bg) * alpha;
			slot.b = bb + (b - bb) * alpha;
			slot.a = ba + (a - ba) * alpha;
			slot.ClampColor();
			slot.r2 = br2 + (r2 - br2) * alpha;
			slot.g2 = bg2 + (g2 - bg2) * alpha;
			slot.b2 = bb2 + (b2 - bb2) * alpha;
			slot.ClampSecondColor();
		}
	}
}

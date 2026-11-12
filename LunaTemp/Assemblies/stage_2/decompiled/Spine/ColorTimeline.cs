using System;

namespace Spine
{
	public class ColorTimeline : CurveTimeline, ISlotTimeline
	{
		public const int ENTRIES = 5;

		protected const int PREV_TIME = -5;

		protected const int PREV_R = -4;

		protected const int PREV_G = -3;

		protected const int PREV_B = -2;

		protected const int PREV_A = -1;

		protected const int R = 1;

		protected const int G = 2;

		protected const int B = 3;

		protected const int A = 4;

		internal int slotIndex;

		internal float[] frames;

		public override int PropertyId => 83886080 + slotIndex;

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

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public ColorTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 5];
		}

		public void SetFrame(int frameIndex, float time, float r, float g, float b, float a)
		{
			frameIndex *= 5;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = r;
			frames[frameIndex + 2] = g;
			frames[frameIndex + 3] = b;
			frames[frameIndex + 4] = a;
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
					break;
				case MixBlend.First:
					slot.r += (slotData.r - slot.r) * alpha;
					slot.g += (slotData.g - slot.g) * alpha;
					slot.b += (slotData.b - slot.b) * alpha;
					slot.a += (slotData.a - slot.a) * alpha;
					slot.ClampColor();
					break;
				}
				return;
			}
			float r;
			float g;
			float b;
			float a;
			if (time >= frames[frames.Length - 5])
			{
				int i = frames.Length;
				r = frames[i + -4];
				g = frames[i + -3];
				b = frames[i + -2];
				a = frames[i + -1];
			}
			else
			{
				int frame = Animation.BinarySearch(frames, time, 5);
				r = frames[frame + -4];
				g = frames[frame + -3];
				b = frames[frame + -2];
				a = frames[frame + -1];
				float frameTime = frames[frame];
				float percent = GetCurvePercent(frame / 5 - 1, 1f - (time - frameTime) / (frames[frame + -5] - frameTime));
				r += (frames[frame + 1] - r) * percent;
				g += (frames[frame + 2] - g) * percent;
				b += (frames[frame + 3] - b) * percent;
				a += (frames[frame + 4] - a) * percent;
			}
			if (alpha == 1f)
			{
				slot.r = r;
				slot.g = g;
				slot.b = b;
				slot.a = a;
				slot.ClampColor();
				return;
			}
			float br;
			float bg;
			float bb;
			float ba;
			if (blend == MixBlend.Setup)
			{
				br = slot.data.r;
				bg = slot.data.g;
				bb = slot.data.b;
				ba = slot.data.a;
			}
			else
			{
				br = slot.r;
				bg = slot.g;
				bb = slot.b;
				ba = slot.a;
			}
			slot.r = br + (r - br) * alpha;
			slot.g = bg + (g - bg) * alpha;
			slot.b = bb + (b - bb) * alpha;
			slot.a = ba + (a - ba) * alpha;
			slot.ClampColor();
		}
	}
}

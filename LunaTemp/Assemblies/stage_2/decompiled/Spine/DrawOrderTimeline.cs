using System;

namespace Spine
{
	public class DrawOrderTimeline : Timeline
	{
		internal float[] frames;

		private int[][] drawOrders;

		public int PropertyId => 134217728;

		public int FrameCount => frames.Length;

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

		public int[][] DrawOrders
		{
			get
			{
				return drawOrders;
			}
			set
			{
				drawOrders = value;
			}
		}

		public DrawOrderTimeline(int frameCount)
		{
			frames = new float[frameCount];
			drawOrders = new int[frameCount][];
		}

		public void SetFrame(int frameIndex, float time, int[] drawOrder)
		{
			frames[frameIndex] = time;
			drawOrders[frameIndex] = drawOrder;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			ExposedList<Slot> slots = skeleton.slots;
			if (direction == MixDirection.Out)
			{
				if (blend == MixBlend.Setup)
				{
					Array.Copy(slots.Items, 0, drawOrder.Items, 0, slots.Count);
				}
				return;
			}
			float[] frames = this.frames;
			if (time < frames[0])
			{
				if (blend == MixBlend.Setup || blend == MixBlend.First)
				{
					Array.Copy(slots.Items, 0, drawOrder.Items, 0, slots.Count);
				}
				return;
			}
			int frame = ((!(time >= frames[frames.Length - 1])) ? (Animation.BinarySearch(frames, time) - 1) : (frames.Length - 1));
			int[] drawOrderToSetupIndex = drawOrders[frame];
			if (drawOrderToSetupIndex == null)
			{
				Array.Copy(slots.Items, 0, drawOrder.Items, 0, slots.Count);
				return;
			}
			Slot[] drawOrderItems = drawOrder.Items;
			Slot[] slotsItems = slots.Items;
			int i = 0;
			for (int j = drawOrderToSetupIndex.Length; i < j; i++)
			{
				drawOrderItems[i] = slotsItems[drawOrderToSetupIndex[i]];
			}
		}
	}
}

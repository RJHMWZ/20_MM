using System;

namespace Spine
{
	public class DeformTimeline : CurveTimeline, ISlotTimeline
	{
		internal int slotIndex;

		internal VertexAttachment attachment;

		internal float[] frames;

		internal float[][] frameVertices;

		public override int PropertyId => 805306368 + attachment.id + slotIndex;

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

		public VertexAttachment Attachment
		{
			get
			{
				return attachment;
			}
			set
			{
				attachment = value;
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

		public float[][] Vertices
		{
			get
			{
				return frameVertices;
			}
			set
			{
				frameVertices = value;
			}
		}

		public DeformTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount];
			frameVertices = new float[frameCount][];
		}

		public void SetFrame(int frameIndex, float time, float[] vertices)
		{
			frames[frameIndex] = time;
			frameVertices[frameIndex] = vertices;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (!slot.bone.active || !(slot.attachment is VertexAttachment vertexAttachment) || vertexAttachment.DeformAttachment != attachment)
			{
				return;
			}
			ExposedList<float> deformArray = slot.Deform;
			if (deformArray.Count == 0)
			{
				blend = MixBlend.Setup;
			}
			float[][] frameVertices = this.frameVertices;
			int vertexCount = frameVertices[0].Length;
			float[] frames = this.frames;
			float[] deform;
			if (time < frames[0])
			{
				switch (blend)
				{
				case MixBlend.Setup:
					deformArray.Clear();
					break;
				case MixBlend.First:
					if (alpha == 1f)
					{
						deformArray.Clear();
						break;
					}
					if (deformArray.Capacity < vertexCount)
					{
						deformArray.Capacity = vertexCount;
					}
					deformArray.Count = vertexCount;
					deform = deformArray.Items;
					if (vertexAttachment.bones == null)
					{
						float[] setupVertices3 = vertexAttachment.vertices;
						for (int i2 = 0; i2 < vertexCount; i2++)
						{
							deform[i2] += (setupVertices3[i2] - deform[i2]) * alpha;
						}
					}
					else
					{
						alpha = 1f - alpha;
						for (int i4 = 0; i4 < vertexCount; i4++)
						{
							deform[i4] *= alpha;
						}
					}
					break;
				}
				return;
			}
			if (deformArray.Capacity < vertexCount)
			{
				deformArray.Capacity = vertexCount;
			}
			deformArray.Count = vertexCount;
			deform = deformArray.Items;
			if (time >= frames[frames.Length - 1])
			{
				float[] lastVertices = frameVertices[frames.Length - 1];
				if (alpha == 1f)
				{
					if (blend == MixBlend.Add)
					{
						if (vertexAttachment.bones == null)
						{
							float[] setupVertices7 = vertexAttachment.vertices;
							for (int i12 = 0; i12 < vertexCount; i12++)
							{
								deform[i12] += lastVertices[i12] - setupVertices7[i12];
							}
						}
						else
						{
							for (int i11 = 0; i11 < vertexCount; i11++)
							{
								deform[i11] += lastVertices[i11];
							}
						}
					}
					else
					{
						Array.Copy(lastVertices, 0, deform, 0, vertexCount);
					}
					return;
				}
				switch (blend)
				{
				case MixBlend.Setup:
					if (vertexAttachment.bones == null)
					{
						float[] setupVertices5 = vertexAttachment.vertices;
						for (int i7 = 0; i7 < vertexCount; i7++)
						{
							float setup2 = setupVertices5[i7];
							deform[i7] = setup2 + (lastVertices[i7] - setup2) * alpha;
						}
					}
					else
					{
						for (int i6 = 0; i6 < vertexCount; i6++)
						{
							deform[i6] = lastVertices[i6] * alpha;
						}
					}
					break;
				case MixBlend.First:
				case MixBlend.Replace:
				{
					for (int i8 = 0; i8 < vertexCount; i8++)
					{
						deform[i8] += (lastVertices[i8] - deform[i8]) * alpha;
					}
					break;
				}
				case MixBlend.Add:
					if (vertexAttachment.bones == null)
					{
						float[] setupVertices6 = vertexAttachment.vertices;
						for (int i10 = 0; i10 < vertexCount; i10++)
						{
							deform[i10] += (lastVertices[i10] - setupVertices6[i10]) * alpha;
						}
					}
					else
					{
						for (int i9 = 0; i9 < vertexCount; i9++)
						{
							deform[i9] += lastVertices[i9] * alpha;
						}
					}
					break;
				}
				return;
			}
			int frame = Animation.BinarySearch(frames, time);
			float[] prevVertices = frameVertices[frame - 1];
			float[] nextVertices = frameVertices[frame];
			float frameTime = frames[frame];
			float percent = GetCurvePercent(frame - 1, 1f - (time - frameTime) / (frames[frame - 1] - frameTime));
			if (alpha == 1f)
			{
				if (blend == MixBlend.Add)
				{
					if (vertexAttachment.bones == null)
					{
						float[] setupVertices4 = vertexAttachment.vertices;
						for (int i5 = 0; i5 < vertexCount; i5++)
						{
							float prev8 = prevVertices[i5];
							deform[i5] += prev8 + (nextVertices[i5] - prev8) * percent - setupVertices4[i5];
						}
					}
					else
					{
						for (int i3 = 0; i3 < vertexCount; i3++)
						{
							float prev7 = prevVertices[i3];
							deform[i3] += prev7 + (nextVertices[i3] - prev7) * percent;
						}
					}
				}
				else
				{
					for (int n = 0; n < vertexCount; n++)
					{
						float prev6 = prevVertices[n];
						deform[n] = prev6 + (nextVertices[n] - prev6) * percent;
					}
				}
				return;
			}
			switch (blend)
			{
			case MixBlend.Setup:
				if (vertexAttachment.bones == null)
				{
					float[] setupVertices = vertexAttachment.vertices;
					for (int j = 0; j < vertexCount; j++)
					{
						float prev2 = prevVertices[j];
						float setup = setupVertices[j];
						deform[j] = setup + (prev2 + (nextVertices[j] - prev2) * percent - setup) * alpha;
					}
				}
				else
				{
					for (int i = 0; i < vertexCount; i++)
					{
						float prev = prevVertices[i];
						deform[i] = (prev + (nextVertices[i] - prev) * percent) * alpha;
					}
				}
				break;
			case MixBlend.First:
			case MixBlend.Replace:
			{
				for (int k = 0; k < vertexCount; k++)
				{
					float prev3 = prevVertices[k];
					deform[k] += (prev3 + (nextVertices[k] - prev3) * percent - deform[k]) * alpha;
				}
				break;
			}
			case MixBlend.Add:
				if (vertexAttachment.bones == null)
				{
					float[] setupVertices2 = vertexAttachment.vertices;
					for (int m = 0; m < vertexCount; m++)
					{
						float prev5 = prevVertices[m];
						deform[m] += (prev5 + (nextVertices[m] - prev5) * percent - setupVertices2[m]) * alpha;
					}
				}
				else
				{
					for (int l = 0; l < vertexCount; l++)
					{
						float prev4 = prevVertices[l];
						deform[l] += (prev4 + (nextVertices[l] - prev4) * percent) * alpha;
					}
				}
				break;
			}
		}
	}
}

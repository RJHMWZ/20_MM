using System;

namespace Spine
{
	public abstract class VertexAttachment : Attachment
	{
		private static int nextID = 0;

		private static readonly object nextIdLock = new object();

		internal readonly int id;

		internal int[] bones;

		internal float[] vertices;

		internal int worldVerticesLength;

		internal VertexAttachment deformAttachment;

		public int Id => id;

		public int[] Bones
		{
			get
			{
				return bones;
			}
			set
			{
				bones = value;
			}
		}

		public float[] Vertices
		{
			get
			{
				return vertices;
			}
			set
			{
				vertices = value;
			}
		}

		public int WorldVerticesLength
		{
			get
			{
				return worldVerticesLength;
			}
			set
			{
				worldVerticesLength = value;
			}
		}

		public VertexAttachment DeformAttachment
		{
			get
			{
				return deformAttachment;
			}
			set
			{
				deformAttachment = value;
			}
		}

		public VertexAttachment(string name)
			: base(name)
		{
			deformAttachment = this;
			lock (nextIdLock)
			{
				id = (nextID++ & 0xFFFF) << 11;
			}
		}

		public void ComputeWorldVertices(Slot slot, float[] worldVertices)
		{
			ComputeWorldVertices(slot, 0, worldVerticesLength, worldVertices, 0);
		}

		public void ComputeWorldVertices(Slot slot, int start, int count, float[] worldVertices, int offset, int stride = 2)
		{
			count = offset + (count >> 1) * stride;
			Skeleton skeleton = slot.bone.skeleton;
			ExposedList<float> deformArray = slot.deform;
			float[] vertices = this.vertices;
			int[] bones = this.bones;
			if (bones == null)
			{
				if (deformArray.Count > 0)
				{
					vertices = deformArray.Items;
				}
				Bone bone3 = slot.bone;
				float x = bone3.worldX;
				float y = bone3.worldY;
				float a = bone3.a;
				float b3 = bone3.b;
				float c = bone3.c;
				float d = bone3.d;
				int vv = start;
				for (int w3 = offset; w3 < count; w3 += stride)
				{
					float vx3 = vertices[vv];
					float vy3 = vertices[vv + 1];
					worldVertices[w3] = vx3 * a + vy3 * b3 + x;
					worldVertices[w3 + 1] = vx3 * c + vy3 * d + y;
					vv += 2;
				}
				return;
			}
			int v = 0;
			int skip = 0;
			for (int i = 0; i < start; i += 2)
			{
				int j = bones[v];
				v += j + 1;
				skip += j;
			}
			Bone[] skeletonBones = skeleton.bones.Items;
			if (deformArray.Count == 0)
			{
				int w2 = offset;
				int b2 = skip * 3;
				for (; w2 < count; w2 += stride)
				{
					float wx2 = 0f;
					float wy2 = 0f;
					int l = bones[v++];
					l += v;
					while (v < l)
					{
						Bone bone2 = skeletonBones[bones[v]];
						float vx2 = vertices[b2];
						float vy2 = vertices[b2 + 1];
						float weight2 = vertices[b2 + 2];
						wx2 += (vx2 * bone2.a + vy2 * bone2.b + bone2.worldX) * weight2;
						wy2 += (vx2 * bone2.c + vy2 * bone2.d + bone2.worldY) * weight2;
						v++;
						b2 += 3;
					}
					worldVertices[w2] = wx2;
					worldVertices[w2 + 1] = wy2;
				}
				return;
			}
			float[] deform = deformArray.Items;
			int w = offset;
			int b = skip * 3;
			int f = skip << 1;
			for (; w < count; w += stride)
			{
				float wx = 0f;
				float wy = 0f;
				int k = bones[v++];
				k += v;
				while (v < k)
				{
					Bone bone = skeletonBones[bones[v]];
					float vx = vertices[b] + deform[f];
					float vy = vertices[b + 1] + deform[f + 1];
					float weight = vertices[b + 2];
					wx += (vx * bone.a + vy * bone.b + bone.worldX) * weight;
					wy += (vx * bone.c + vy * bone.d + bone.worldY) * weight;
					v++;
					b += 3;
					f += 2;
				}
				worldVertices[w] = wx;
				worldVertices[w + 1] = wy;
			}
		}

		internal void CopyTo(VertexAttachment attachment)
		{
			if (bones != null)
			{
				attachment.bones = new int[bones.Length];
				Array.Copy(bones, 0, attachment.bones, 0, bones.Length);
			}
			else
			{
				attachment.bones = null;
			}
			if (vertices != null)
			{
				attachment.vertices = new float[vertices.Length];
				Array.Copy(vertices, 0, attachment.vertices, 0, vertices.Length);
			}
			else
			{
				attachment.vertices = null;
			}
			attachment.worldVerticesLength = worldVerticesLength;
			attachment.deformAttachment = deformAttachment;
		}
	}
}

using System;

namespace Spine
{
	public class MeshAttachment : VertexAttachment, IHasRendererObject
	{
		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		private MeshAttachment parentMesh;

		internal float[] uvs;

		internal float[] regionUVs;

		internal int[] triangles;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal int hulllength;

		public int HullLength
		{
			get
			{
				return hulllength;
			}
			set
			{
				hulllength = value;
			}
		}

		public float[] RegionUVs
		{
			get
			{
				return regionUVs;
			}
			set
			{
				regionUVs = value;
			}
		}

		public float[] UVs
		{
			get
			{
				return uvs;
			}
			set
			{
				uvs = value;
			}
		}

		public int[] Triangles
		{
			get
			{
				return triangles;
			}
			set
			{
				triangles = value;
			}
		}

		public float R
		{
			get
			{
				return r;
			}
			set
			{
				r = value;
			}
		}

		public float G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public float B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		public float A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		public string Path { get; set; }

		public object RendererObject { get; set; }

		public float RegionU { get; set; }

		public float RegionV { get; set; }

		public float RegionU2 { get; set; }

		public float RegionV2 { get; set; }

		public bool RegionRotate { get; set; }

		public int RegionDegrees { get; set; }

		public float RegionOffsetX
		{
			get
			{
				return regionOffsetX;
			}
			set
			{
				regionOffsetX = value;
			}
		}

		public float RegionOffsetY
		{
			get
			{
				return regionOffsetY;
			}
			set
			{
				regionOffsetY = value;
			}
		}

		public float RegionWidth
		{
			get
			{
				return regionWidth;
			}
			set
			{
				regionWidth = value;
			}
		}

		public float RegionHeight
		{
			get
			{
				return regionHeight;
			}
			set
			{
				regionHeight = value;
			}
		}

		public float RegionOriginalWidth
		{
			get
			{
				return regionOriginalWidth;
			}
			set
			{
				regionOriginalWidth = value;
			}
		}

		public float RegionOriginalHeight
		{
			get
			{
				return regionOriginalHeight;
			}
			set
			{
				regionOriginalHeight = value;
			}
		}

		public MeshAttachment ParentMesh
		{
			get
			{
				return parentMesh;
			}
			set
			{
				parentMesh = value;
				if (value != null)
				{
					bones = value.bones;
					vertices = value.vertices;
					worldVerticesLength = value.worldVerticesLength;
					regionUVs = value.regionUVs;
					triangles = value.triangles;
					HullLength = value.HullLength;
					Edges = value.Edges;
					Width = value.Width;
					Height = value.Height;
				}
			}
		}

		public int[] Edges { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		public MeshAttachment(string name)
			: base(name)
		{
		}

		public void UpdateUVs()
		{
			float[] regionUVs = this.regionUVs;
			if (this.uvs == null || this.uvs.Length != regionUVs.Length)
			{
				this.uvs = new float[regionUVs.Length];
			}
			float[] uvs = this.uvs;
			float u = RegionU;
			float v = RegionV;
			float width = 0f;
			float height = 0f;
			if (RegionDegrees == 90)
			{
				float textureHeight4 = regionWidth / (RegionV2 - RegionV);
				float textureWidth4 = regionHeight / (RegionU2 - RegionU);
				u -= (RegionOriginalHeight - RegionOffsetY - RegionHeight) / textureWidth4;
				v -= (RegionOriginalWidth - RegionOffsetX - RegionWidth) / textureHeight4;
				width = RegionOriginalHeight / textureWidth4;
				height = RegionOriginalWidth / textureHeight4;
				int l = 0;
				for (int n3 = uvs.Length; l < n3; l += 2)
				{
					uvs[l] = u + regionUVs[l + 1] * width;
					uvs[l + 1] = v + (1f - regionUVs[l]) * height;
				}
			}
			else if (RegionDegrees == 180)
			{
				float textureWidth3 = regionWidth / (RegionU2 - RegionU);
				float textureHeight3 = regionHeight / (RegionV2 - RegionV);
				u -= (RegionOriginalWidth - RegionOffsetX - RegionWidth) / textureWidth3;
				v -= RegionOffsetY / textureHeight3;
				width = RegionOriginalWidth / textureWidth3;
				height = RegionOriginalHeight / textureHeight3;
				int k = 0;
				for (int n2 = uvs.Length; k < n2; k += 2)
				{
					uvs[k] = u + (1f - regionUVs[k]) * width;
					uvs[k + 1] = v + (1f - regionUVs[k + 1]) * height;
				}
			}
			else if (RegionDegrees == 270)
			{
				float textureWidth2 = regionWidth / (RegionU2 - RegionU);
				float textureHeight2 = regionHeight / (RegionV2 - RegionV);
				u -= RegionOffsetY / textureWidth2;
				v -= RegionOffsetX / textureHeight2;
				width = RegionOriginalHeight / textureWidth2;
				height = RegionOriginalWidth / textureHeight2;
				int j = 0;
				for (int n = uvs.Length; j < n; j += 2)
				{
					uvs[j] = u + (1f - regionUVs[j + 1]) * width;
					uvs[j + 1] = v + regionUVs[j] * height;
				}
			}
			else
			{
				float textureWidth = regionWidth / (RegionU2 - RegionU);
				float textureHeight = regionHeight / (RegionV2 - RegionV);
				u -= RegionOffsetX / textureWidth;
				v -= (RegionOriginalHeight - RegionOffsetY - RegionHeight) / textureHeight;
				width = RegionOriginalWidth / textureWidth;
				height = RegionOriginalHeight / textureHeight;
				int i = 0;
				for (int m = uvs.Length; i < m; i += 2)
				{
					uvs[i] = u + regionUVs[i] * width;
					uvs[i + 1] = v + regionUVs[i + 1] * height;
				}
			}
		}

		public override Attachment Copy()
		{
			if (parentMesh != null)
			{
				return NewLinkedMesh();
			}
			MeshAttachment copy = new MeshAttachment(base.Name);
			copy.RendererObject = RendererObject;
			copy.regionOffsetX = regionOffsetX;
			copy.regionOffsetY = regionOffsetY;
			copy.regionWidth = regionWidth;
			copy.regionHeight = regionHeight;
			copy.regionOriginalWidth = regionOriginalWidth;
			copy.regionOriginalHeight = regionOriginalHeight;
			copy.RegionRotate = RegionRotate;
			copy.RegionDegrees = RegionDegrees;
			copy.RegionU = RegionU;
			copy.RegionV = RegionV;
			copy.RegionU2 = RegionU2;
			copy.RegionV2 = RegionV2;
			copy.Path = Path;
			copy.r = r;
			copy.g = g;
			copy.b = b;
			copy.a = a;
			CopyTo(copy);
			copy.regionUVs = new float[regionUVs.Length];
			Array.Copy(regionUVs, 0, copy.regionUVs, 0, regionUVs.Length);
			copy.uvs = new float[uvs.Length];
			Array.Copy(uvs, 0, copy.uvs, 0, uvs.Length);
			copy.triangles = new int[triangles.Length];
			Array.Copy(triangles, 0, copy.triangles, 0, triangles.Length);
			copy.HullLength = HullLength;
			if (Edges != null)
			{
				copy.Edges = new int[Edges.Length];
				Array.Copy(Edges, 0, copy.Edges, 0, Edges.Length);
			}
			copy.Width = Width;
			copy.Height = Height;
			return copy;
		}

		public MeshAttachment NewLinkedMesh()
		{
			MeshAttachment mesh = new MeshAttachment(base.Name);
			mesh.RendererObject = RendererObject;
			mesh.regionOffsetX = regionOffsetX;
			mesh.regionOffsetY = regionOffsetY;
			mesh.regionWidth = regionWidth;
			mesh.regionHeight = regionHeight;
			mesh.regionOriginalWidth = regionOriginalWidth;
			mesh.regionOriginalHeight = regionOriginalHeight;
			mesh.RegionDegrees = RegionDegrees;
			mesh.RegionRotate = RegionRotate;
			mesh.RegionU = RegionU;
			mesh.RegionV = RegionV;
			mesh.RegionU2 = RegionU2;
			mesh.RegionV2 = RegionV2;
			mesh.Path = Path;
			mesh.r = r;
			mesh.g = g;
			mesh.b = b;
			mesh.a = a;
			mesh.deformAttachment = deformAttachment;
			mesh.ParentMesh = ((parentMesh != null) ? parentMesh : this);
			mesh.UpdateUVs();
			return mesh;
		}
	}
}

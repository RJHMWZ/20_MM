using System;

namespace Spine
{
	public class RegionAttachment : Attachment, IHasRendererObject
	{
		public const int BLX = 0;

		public const int BLY = 1;

		public const int ULX = 2;

		public const int ULY = 3;

		public const int URX = 4;

		public const int URY = 5;

		public const int BRX = 6;

		public const int BRY = 7;

		internal float x;

		internal float y;

		internal float rotation;

		internal float scaleX = 1f;

		internal float scaleY = 1f;

		internal float width;

		internal float height;

		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		internal float[] offset = new float[8];

		internal float[] uvs = new float[8];

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public float Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public float ScaleX
		{
			get
			{
				return scaleX;
			}
			set
			{
				scaleX = value;
			}
		}

		public float ScaleY
		{
			get
			{
				return scaleY;
			}
			set
			{
				scaleY = value;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
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

		public float[] Offset => offset;

		public float[] UVs => uvs;

		public RegionAttachment(string name)
			: base(name)
		{
		}

		public void UpdateOffset()
		{
			float width = this.width;
			float height = this.height;
			float localX2 = width * 0.5f;
			float localY2 = height * 0.5f;
			float localX = 0f - localX2;
			float localY = 0f - localY2;
			if (regionOriginalWidth != 0f)
			{
				localX += regionOffsetX / regionOriginalWidth * width;
				localY += regionOffsetY / regionOriginalHeight * height;
				localX2 -= (regionOriginalWidth - regionOffsetX - regionWidth) / regionOriginalWidth * width;
				localY2 -= (regionOriginalHeight - regionOffsetY - regionHeight) / regionOriginalHeight * height;
			}
			float scaleX = this.scaleX;
			float scaleY = this.scaleY;
			localX *= scaleX;
			localY *= scaleY;
			localX2 *= scaleX;
			localY2 *= scaleY;
			float rotation = this.rotation;
			float cos = MathUtils.CosDeg(rotation);
			float sin = MathUtils.SinDeg(rotation);
			float x = this.x;
			float y = this.y;
			float localXCos = localX * cos + x;
			float localXSin = localX * sin;
			float localYCos = localY * cos + y;
			float localYSin = localY * sin;
			float localX2Cos = localX2 * cos + x;
			float localX2Sin = localX2 * sin;
			float localY2Cos = localY2 * cos + y;
			float localY2Sin = localY2 * sin;
			float[] offset = this.offset;
			offset[0] = localXCos - localYSin;
			offset[1] = localYCos + localXSin;
			offset[2] = localXCos - localY2Sin;
			offset[3] = localY2Cos + localXSin;
			offset[4] = localX2Cos - localY2Sin;
			offset[5] = localY2Cos + localX2Sin;
			offset[6] = localX2Cos - localYSin;
			offset[7] = localYCos + localX2Sin;
		}

		public void SetUVs(float u, float v, float u2, float v2, bool rotate)
		{
			float[] uvs = this.uvs;
			if (rotate)
			{
				uvs[4] = u;
				uvs[5] = v2;
				uvs[6] = u;
				uvs[7] = v;
				uvs[0] = u2;
				uvs[1] = v;
				uvs[2] = u2;
				uvs[3] = v2;
			}
			else
			{
				uvs[2] = u;
				uvs[3] = v2;
				uvs[4] = u;
				uvs[5] = v;
				uvs[6] = u2;
				uvs[7] = v;
				uvs[0] = u2;
				uvs[1] = v2;
			}
		}

		public void ComputeWorldVertices(Bone bone, float[] worldVertices, int offset, int stride = 2)
		{
			float[] vertexOffset = this.offset;
			float bwx = bone.worldX;
			float bwy = bone.worldY;
			float a = bone.a;
			float b = bone.b;
			float c = bone.c;
			float d = bone.d;
			float offsetX = vertexOffset[6];
			float offsetY = vertexOffset[7];
			worldVertices[offset] = offsetX * a + offsetY * b + bwx;
			worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
			offset += stride;
			offsetX = vertexOffset[0];
			offsetY = vertexOffset[1];
			worldVertices[offset] = offsetX * a + offsetY * b + bwx;
			worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
			offset += stride;
			offsetX = vertexOffset[2];
			offsetY = vertexOffset[3];
			worldVertices[offset] = offsetX * a + offsetY * b + bwx;
			worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
			offset += stride;
			offsetX = vertexOffset[4];
			offsetY = vertexOffset[5];
			worldVertices[offset] = offsetX * a + offsetY * b + bwx;
			worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
		}

		public override Attachment Copy()
		{
			RegionAttachment copy = new RegionAttachment(base.Name);
			copy.RendererObject = RendererObject;
			copy.regionOffsetX = regionOffsetX;
			copy.regionOffsetY = regionOffsetY;
			copy.regionWidth = regionWidth;
			copy.regionHeight = regionHeight;
			copy.regionOriginalWidth = regionOriginalWidth;
			copy.regionOriginalHeight = regionOriginalHeight;
			copy.Path = Path;
			copy.x = x;
			copy.y = y;
			copy.scaleX = scaleX;
			copy.scaleY = scaleY;
			copy.rotation = rotation;
			copy.width = width;
			copy.height = height;
			Array.Copy(uvs, 0, copy.uvs, 0, 8);
			Array.Copy(offset, 0, copy.offset, 0, 8);
			copy.r = r;
			copy.g = g;
			copy.b = b;
			copy.a = a;
			return copy;
		}
	}
}

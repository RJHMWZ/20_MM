using System;

namespace Spine
{
	public class PointAttachment : Attachment
	{
		internal float x;

		internal float y;

		internal float rotation;

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

		public PointAttachment(string name)
			: base(name)
		{
		}

		public void ComputeWorldPosition(Bone bone, out float ox, out float oy)
		{
			bone.LocalToWorld(x, y, out ox, out oy);
		}

		public float ComputeWorldRotation(Bone bone)
		{
			float cos = MathUtils.CosDeg(rotation);
			float sin = MathUtils.SinDeg(rotation);
			float ix = cos * bone.a + sin * bone.b;
			float iy = cos * bone.c + sin * bone.d;
			return MathUtils.Atan2(iy, ix) * (180f / 3.14159265f);
		}

		public override Attachment Copy()
		{
			PointAttachment copy = new PointAttachment(base.Name);
			copy.x = x;
			copy.y = y;
			copy.rotation = rotation;
			return copy;
		}
	}
}

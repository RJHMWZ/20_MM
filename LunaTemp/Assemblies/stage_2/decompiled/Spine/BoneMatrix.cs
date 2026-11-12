using System;

namespace Spine
{
	public struct BoneMatrix
	{
		public float a;

		public float b;

		public float c;

		public float d;

		public float x;

		public float y;

		public static BoneMatrix CalculateSetupWorld(BoneData boneData)
		{
			if (boneData == null)
			{
				return default(BoneMatrix);
			}
			if (boneData.parent == null)
			{
				return GetInheritedInternal(boneData, default(BoneMatrix));
			}
			BoneMatrix result = CalculateSetupWorld(boneData.parent);
			return GetInheritedInternal(boneData, result);
		}

		private static BoneMatrix GetInheritedInternal(BoneData boneData, BoneMatrix parentMatrix)
		{
			BoneData parent = boneData.parent;
			if (parent == null)
			{
				return new BoneMatrix(boneData);
			}
			float pa = parentMatrix.a;
			float pb = parentMatrix.b;
			float pc = parentMatrix.c;
			float pd = parentMatrix.d;
			BoneMatrix result = default(BoneMatrix);
			result.x = pa * boneData.x + pb * boneData.y + parentMatrix.x;
			result.y = pc * boneData.x + pd * boneData.y + parentMatrix.y;
			switch (boneData.transformMode)
			{
			case TransformMode.Normal:
			{
				float rotationY = boneData.rotation + 90f + boneData.shearY;
				float la = MathUtils.CosDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				float lb = MathUtils.CosDeg(rotationY) * boneData.scaleY;
				float lc = MathUtils.SinDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				float ld = MathUtils.SinDeg(rotationY) * boneData.scaleY;
				result.a = pa * la + pb * lc;
				result.b = pa * lb + pb * ld;
				result.c = pc * la + pd * lc;
				result.d = pc * lb + pd * ld;
				break;
			}
			case TransformMode.OnlyTranslation:
			{
				float rotationY2 = boneData.rotation + 90f + boneData.shearY;
				result.a = MathUtils.CosDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				result.b = MathUtils.CosDeg(rotationY2) * boneData.scaleY;
				result.c = MathUtils.SinDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				result.d = MathUtils.SinDeg(rotationY2) * boneData.scaleY;
				break;
			}
			case TransformMode.NoRotationOrReflection:
			{
				float s = pa * pa + pc * pc;
				float prx;
				if (s > 0.0001f)
				{
					s = Math.Abs(pa * pd - pb * pc) / s;
					pb = pc * s;
					pd = pa * s;
					prx = MathUtils.Atan2(pc, pa) * (180f / 3.14159265f);
				}
				else
				{
					pa = 0f;
					pc = 0f;
					prx = 90f - MathUtils.Atan2(pd, pb) * (180f / 3.14159265f);
				}
				float rx = boneData.rotation + boneData.shearX - prx;
				float ry = boneData.rotation + boneData.shearY - prx + 90f;
				float la2 = MathUtils.CosDeg(rx) * boneData.scaleX;
				float lb2 = MathUtils.CosDeg(ry) * boneData.scaleY;
				float lc2 = MathUtils.SinDeg(rx) * boneData.scaleX;
				float ld2 = MathUtils.SinDeg(ry) * boneData.scaleY;
				result.a = pa * la2 - pb * lc2;
				result.b = pa * lb2 - pb * ld2;
				result.c = pc * la2 + pd * lc2;
				result.d = pc * lb2 + pd * ld2;
				break;
			}
			case TransformMode.NoScale:
			case TransformMode.NoScaleOrReflection:
			{
				float cos = MathUtils.CosDeg(boneData.rotation);
				float sin = MathUtils.SinDeg(boneData.rotation);
				float za = pa * cos + pb * sin;
				float zc = pc * cos + pd * sin;
				float s2 = (float)Math.Sqrt(za * za + zc * zc);
				if (s2 > 1E-05f)
				{
					s2 = 1f / s2;
				}
				za *= s2;
				zc *= s2;
				s2 = (float)Math.Sqrt(za * za + zc * zc);
				float r = 3.14159265f / 2f + MathUtils.Atan2(zc, za);
				float zb = MathUtils.Cos(r) * s2;
				float zd = MathUtils.Sin(r) * s2;
				float la3 = MathUtils.CosDeg(boneData.shearX) * boneData.scaleX;
				float lb3 = MathUtils.CosDeg(90f + boneData.shearY) * boneData.scaleY;
				float lc3 = MathUtils.SinDeg(boneData.shearX) * boneData.scaleX;
				float ld3 = MathUtils.SinDeg(90f + boneData.shearY) * boneData.scaleY;
				if (boneData.transformMode != TransformMode.NoScaleOrReflection && pa * pd - pb * pc < 0f)
				{
					zb = 0f - zb;
					zd = 0f - zd;
				}
				result.a = za * la3 + zb * lc3;
				result.b = za * lb3 + zb * ld3;
				result.c = zc * la3 + zd * lc3;
				result.d = zc * lb3 + zd * ld3;
				break;
			}
			}
			return result;
		}

		public BoneMatrix(BoneData boneData)
		{
			float rotationY = boneData.rotation + 90f + boneData.shearY;
			float rotationX = boneData.rotation + boneData.shearX;
			a = MathUtils.CosDeg(rotationX) * boneData.scaleX;
			c = MathUtils.SinDeg(rotationX) * boneData.scaleX;
			b = MathUtils.CosDeg(rotationY) * boneData.scaleY;
			d = MathUtils.SinDeg(rotationY) * boneData.scaleY;
			x = boneData.x;
			y = boneData.y;
		}

		public BoneMatrix(Bone bone)
		{
			float rotationY = bone.rotation + 90f + bone.shearY;
			float rotationX = bone.rotation + bone.shearX;
			a = MathUtils.CosDeg(rotationX) * bone.scaleX;
			c = MathUtils.SinDeg(rotationX) * bone.scaleX;
			b = MathUtils.CosDeg(rotationY) * bone.scaleY;
			d = MathUtils.SinDeg(rotationY) * bone.scaleY;
			x = bone.x;
			y = bone.y;
		}

		public BoneMatrix TransformMatrix(BoneMatrix local)
		{
			BoneMatrix result = default(BoneMatrix);
			result.a = a * local.a + b * local.c;
			result.b = a * local.b + b * local.d;
			result.c = c * local.a + d * local.c;
			result.d = c * local.b + d * local.d;
			result.x = a * local.x + b * local.y + x;
			result.y = c * local.x + d * local.y + y;
			return result;
		}
	}
}

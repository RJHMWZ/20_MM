using System;

namespace Spine
{
	public class Bone : IUpdatable
	{
		public static bool yDown;

		internal BoneData data;

		internal Skeleton skeleton;

		internal Bone parent;

		internal ExposedList<Bone> children = new ExposedList<Bone>();

		internal float x;

		internal float y;

		internal float rotation;

		internal float scaleX;

		internal float scaleY;

		internal float shearX;

		internal float shearY;

		internal float ax;

		internal float ay;

		internal float arotation;

		internal float ascaleX;

		internal float ascaleY;

		internal float ashearX;

		internal float ashearY;

		internal bool appliedValid;

		internal float a;

		internal float b;

		internal float worldX;

		internal float c;

		internal float d;

		internal float worldY;

		internal bool sorted;

		internal bool active;

		public BoneData Data => data;

		public Skeleton Skeleton => skeleton;

		public Bone Parent => parent;

		public ExposedList<Bone> Children => children;

		public bool Active => active;

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

		public float ShearX
		{
			get
			{
				return shearX;
			}
			set
			{
				shearX = value;
			}
		}

		public float ShearY
		{
			get
			{
				return shearY;
			}
			set
			{
				shearY = value;
			}
		}

		public float AppliedRotation
		{
			get
			{
				return arotation;
			}
			set
			{
				arotation = value;
			}
		}

		public float AX
		{
			get
			{
				return ax;
			}
			set
			{
				ax = value;
			}
		}

		public float AY
		{
			get
			{
				return ay;
			}
			set
			{
				ay = value;
			}
		}

		public float AScaleX
		{
			get
			{
				return ascaleX;
			}
			set
			{
				ascaleX = value;
			}
		}

		public float AScaleY
		{
			get
			{
				return ascaleY;
			}
			set
			{
				ascaleY = value;
			}
		}

		public float AShearX
		{
			get
			{
				return ashearX;
			}
			set
			{
				ashearX = value;
			}
		}

		public float AShearY
		{
			get
			{
				return ashearY;
			}
			set
			{
				ashearY = value;
			}
		}

		public float A => a;

		public float B => b;

		public float C => c;

		public float D => d;

		public float WorldX => worldX;

		public float WorldY => worldY;

		public float WorldRotationX => MathUtils.Atan2(c, a) * (180f / 3.14159265f);

		public float WorldRotationY => MathUtils.Atan2(d, b) * (180f / 3.14159265f);

		public float WorldScaleX => (float)Math.Sqrt(a * a + c * c);

		public float WorldScaleY => (float)Math.Sqrt(b * b + d * d);

		public float WorldToLocalRotationX
		{
			get
			{
				Bone parent = this.parent;
				if (parent == null)
				{
					return arotation;
				}
				float pa = parent.a;
				float pb = parent.b;
				float pc = parent.c;
				float pd = parent.d;
				float a = this.a;
				float c = this.c;
				return MathUtils.Atan2(pa * c - pc * a, pd * a - pb * c) * (180f / 3.14159265f);
			}
		}

		public float WorldToLocalRotationY
		{
			get
			{
				Bone parent = this.parent;
				if (parent == null)
				{
					return arotation;
				}
				float pa = parent.a;
				float pb = parent.b;
				float pc = parent.c;
				float pd = parent.d;
				float b = this.b;
				float d = this.d;
				return MathUtils.Atan2(pa * d - pc * b, pd * b - pb * d) * (180f / 3.14159265f);
			}
		}

		public Bone(BoneData data, Skeleton skeleton, Bone parent)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			this.data = data;
			this.skeleton = skeleton;
			this.parent = parent;
			SetToSetupPose();
		}

		public void Update()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
		}

		public void UpdateWorldTransform()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
		}

		public void UpdateWorldTransform(float x, float y, float rotation, float scaleX, float scaleY, float shearX, float shearY)
		{
			ax = x;
			ay = y;
			arotation = rotation;
			ascaleX = scaleX;
			ascaleY = scaleY;
			ashearX = shearX;
			ashearY = shearY;
			appliedValid = true;
			Skeleton skeleton = this.skeleton;
			Bone parent = this.parent;
			if (parent == null)
			{
				float rotationY3 = rotation + 90f + shearY;
				float sx = skeleton.ScaleX;
				float sy = skeleton.ScaleY;
				a = MathUtils.CosDeg(rotation + shearX) * scaleX * sx;
				b = MathUtils.CosDeg(rotationY3) * scaleY * sx;
				c = MathUtils.SinDeg(rotation + shearX) * scaleX * sy;
				d = MathUtils.SinDeg(rotationY3) * scaleY * sy;
				worldX = x * sx + skeleton.x;
				worldY = y * sy + skeleton.y;
				return;
			}
			float pa = parent.a;
			float pb = parent.b;
			float pc = parent.c;
			float pd = parent.d;
			worldX = pa * x + pb * y + parent.worldX;
			worldY = pc * x + pd * y + parent.worldY;
			switch (data.transformMode)
			{
			case TransformMode.Normal:
			{
				float rotationY = rotation + 90f + shearY;
				float la = MathUtils.CosDeg(rotation + shearX) * scaleX;
				float lb = MathUtils.CosDeg(rotationY) * scaleY;
				float lc = MathUtils.SinDeg(rotation + shearX) * scaleX;
				float ld = MathUtils.SinDeg(rotationY) * scaleY;
				a = pa * la + pb * lc;
				b = pa * lb + pb * ld;
				c = pc * la + pd * lc;
				d = pc * lb + pd * ld;
				return;
			}
			case TransformMode.OnlyTranslation:
			{
				float rotationY2 = rotation + 90f + shearY;
				a = MathUtils.CosDeg(rotation + shearX) * scaleX;
				b = MathUtils.CosDeg(rotationY2) * scaleY;
				c = MathUtils.SinDeg(rotation + shearX) * scaleX;
				d = MathUtils.SinDeg(rotationY2) * scaleY;
				break;
			}
			case TransformMode.NoRotationOrReflection:
			{
				float s = pa * pa + pc * pc;
				float prx;
				if (s > 0.0001f)
				{
					s = Math.Abs(pa * pd - pb * pc) / s;
					pa /= skeleton.ScaleX;
					pc /= skeleton.ScaleY;
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
				float rx = rotation + shearX - prx;
				float ry = rotation + shearY - prx + 90f;
				float la2 = MathUtils.CosDeg(rx) * scaleX;
				float lb2 = MathUtils.CosDeg(ry) * scaleY;
				float lc2 = MathUtils.SinDeg(rx) * scaleX;
				float ld2 = MathUtils.SinDeg(ry) * scaleY;
				a = pa * la2 - pb * lc2;
				b = pa * lb2 - pb * ld2;
				c = pc * la2 + pd * lc2;
				d = pc * lb2 + pd * ld2;
				break;
			}
			case TransformMode.NoScale:
			case TransformMode.NoScaleOrReflection:
			{
				float cos = MathUtils.CosDeg(rotation);
				float sin = MathUtils.SinDeg(rotation);
				float za = (pa * cos + pb * sin) / skeleton.ScaleX;
				float zc = (pc * cos + pd * sin) / skeleton.ScaleY;
				float s2 = (float)Math.Sqrt(za * za + zc * zc);
				if (s2 > 1E-05f)
				{
					s2 = 1f / s2;
				}
				za *= s2;
				zc *= s2;
				s2 = (float)Math.Sqrt(za * za + zc * zc);
				if (data.transformMode == TransformMode.NoScale && pa * pd - pb * pc < 0f != (skeleton.ScaleX < 0f != skeleton.ScaleY < 0f))
				{
					s2 = 0f - s2;
				}
				float r = 3.14159265f / 2f + MathUtils.Atan2(zc, za);
				float zb = MathUtils.Cos(r) * s2;
				float zd = MathUtils.Sin(r) * s2;
				float la3 = MathUtils.CosDeg(shearX) * scaleX;
				float lb3 = MathUtils.CosDeg(90f + shearY) * scaleY;
				float lc3 = MathUtils.SinDeg(shearX) * scaleX;
				float ld3 = MathUtils.SinDeg(90f + shearY) * scaleY;
				a = za * la3 + zb * lc3;
				b = za * lb3 + zb * ld3;
				c = zc * la3 + zd * lc3;
				d = zc * lb3 + zd * ld3;
				break;
			}
			}
			a *= skeleton.ScaleX;
			b *= skeleton.ScaleX;
			c *= skeleton.ScaleY;
			d *= skeleton.ScaleY;
		}

		public void SetToSetupPose()
		{
			BoneData data = this.data;
			x = data.x;
			y = data.y;
			rotation = data.rotation;
			scaleX = data.scaleX;
			scaleY = data.scaleY;
			shearX = data.shearX;
			shearY = data.shearY;
		}

		internal void UpdateAppliedTransform()
		{
			appliedValid = true;
			Bone parent = this.parent;
			if (parent == null)
			{
				ax = worldX;
				ay = worldY;
				arotation = MathUtils.Atan2(c, a) * (180f / 3.14159265f);
				ascaleX = (float)Math.Sqrt(a * a + c * c);
				ascaleY = (float)Math.Sqrt(b * b + d * d);
				ashearX = 0f;
				ashearY = MathUtils.Atan2(a * b + c * d, a * d - b * c) * (180f / 3.14159265f);
				return;
			}
			float pa = parent.a;
			float pb = parent.b;
			float pc = parent.c;
			float pd = parent.d;
			float pid = 1f / (pa * pd - pb * pc);
			float dx = worldX - parent.worldX;
			float dy = worldY - parent.worldY;
			ax = dx * pd * pid - dy * pb * pid;
			ay = dy * pa * pid - dx * pc * pid;
			float ia = pid * pd;
			float id = pid * pa;
			float ib = pid * pb;
			float ic = pid * pc;
			float ra = ia * a - ib * c;
			float rb = ia * b - ib * d;
			float rc = id * c - ic * a;
			float rd = id * d - ic * b;
			ashearX = 0f;
			ascaleX = (float)Math.Sqrt(ra * ra + rc * rc);
			if (ascaleX > 0.0001f)
			{
				float det = ra * rd - rb * rc;
				ascaleY = det / ascaleX;
				ashearY = MathUtils.Atan2(ra * rb + rc * rd, det) * (180f / 3.14159265f);
				arotation = MathUtils.Atan2(rc, ra) * (180f / 3.14159265f);
			}
			else
			{
				ascaleX = 0f;
				ascaleY = (float)Math.Sqrt(rb * rb + rd * rd);
				ashearY = 0f;
				arotation = 90f - MathUtils.Atan2(rd, rb) * (180f / 3.14159265f);
			}
		}

		public void WorldToLocal(float worldX, float worldY, out float localX, out float localY)
		{
			float a = this.a;
			float b = this.b;
			float c = this.c;
			float d = this.d;
			float invDet = 1f / (a * d - b * c);
			float x = worldX - this.worldX;
			float y = worldY - this.worldY;
			localX = x * d * invDet - y * b * invDet;
			localY = y * a * invDet - x * c * invDet;
		}

		public void LocalToWorld(float localX, float localY, out float worldX, out float worldY)
		{
			worldX = localX * a + localY * b + this.worldX;
			worldY = localX * c + localY * d + this.worldY;
		}

		public float WorldToLocalRotation(float worldRotation)
		{
			float sin = MathUtils.SinDeg(worldRotation);
			float cos = MathUtils.CosDeg(worldRotation);
			return MathUtils.Atan2(a * sin - c * cos, d * cos - b * sin) * (180f / 3.14159265f) + rotation - shearX;
		}

		public float LocalToWorldRotation(float localRotation)
		{
			localRotation -= rotation - shearX;
			float sin = MathUtils.SinDeg(localRotation);
			float cos = MathUtils.CosDeg(localRotation);
			return MathUtils.Atan2(cos * c + sin * d, cos * a + sin * b) * (180f / 3.14159265f);
		}

		public void RotateWorld(float degrees)
		{
			float a = this.a;
			float b = this.b;
			float c = this.c;
			float d = this.d;
			float cos = MathUtils.CosDeg(degrees);
			float sin = MathUtils.SinDeg(degrees);
			this.a = cos * a - sin * c;
			this.b = cos * b - sin * d;
			this.c = sin * a + cos * c;
			this.d = sin * b + cos * d;
			appliedValid = false;
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}

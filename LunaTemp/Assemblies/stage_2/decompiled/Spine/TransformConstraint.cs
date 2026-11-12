using System;

namespace Spine
{
	public class TransformConstraint : IUpdatable
	{
		internal TransformConstraintData data;

		internal ExposedList<Bone> bones;

		internal Bone target;

		internal float rotateMix;

		internal float translateMix;

		internal float scaleMix;

		internal float shearMix;

		internal bool active;

		public ExposedList<Bone> Bones => bones;

		public Bone Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public float RotateMix
		{
			get
			{
				return rotateMix;
			}
			set
			{
				rotateMix = value;
			}
		}

		public float TranslateMix
		{
			get
			{
				return translateMix;
			}
			set
			{
				translateMix = value;
			}
		}

		public float ScaleMix
		{
			get
			{
				return scaleMix;
			}
			set
			{
				scaleMix = value;
			}
		}

		public float ShearMix
		{
			get
			{
				return shearMix;
			}
			set
			{
				shearMix = value;
			}
		}

		public bool Active => active;

		public TransformConstraintData Data => data;

		public TransformConstraint(TransformConstraintData data, Skeleton skeleton)
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
			rotateMix = data.rotateMix;
			translateMix = data.translateMix;
			scaleMix = data.scaleMix;
			shearMix = data.shearMix;
			bones = new ExposedList<Bone>();
			foreach (BoneData boneData in data.bones)
			{
				bones.Add(skeleton.FindBone(boneData.name));
			}
			target = skeleton.FindBone(data.target.name);
		}

		public TransformConstraint(TransformConstraint constraint, Skeleton skeleton)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException("constraint cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			data = constraint.data;
			bones = new ExposedList<Bone>(constraint.Bones.Count);
			foreach (Bone bone in constraint.Bones)
			{
				bones.Add(skeleton.Bones.Items[bone.data.index]);
			}
			target = skeleton.Bones.Items[constraint.target.data.index];
			rotateMix = constraint.rotateMix;
			translateMix = constraint.translateMix;
			scaleMix = constraint.scaleMix;
			shearMix = constraint.shearMix;
		}

		public void Apply()
		{
			Update();
		}

		public void Update()
		{
			if (data.local)
			{
				if (data.relative)
				{
					ApplyRelativeLocal();
				}
				else
				{
					ApplyAbsoluteLocal();
				}
			}
			else if (data.relative)
			{
				ApplyRelativeWorld();
			}
			else
			{
				ApplyAbsoluteWorld();
			}
		}

		private void ApplyAbsoluteWorld()
		{
			float rotateMix = this.rotateMix;
			float translateMix = this.translateMix;
			float scaleMix = this.scaleMix;
			float shearMix = this.shearMix;
			Bone target = this.target;
			float ta = target.a;
			float tb = target.b;
			float tc = target.c;
			float td = target.d;
			float degRadReflect = ((ta * td - tb * tc > 0f) ? (3.14159265f / 180f) : (-3.14159265f / 180f));
			float offsetRotation = data.offsetRotation * degRadReflect;
			float offsetShearY = data.offsetShearY * degRadReflect;
			ExposedList<Bone> bones = this.bones;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bones.Items[i];
				bool modified = false;
				if (rotateMix != 0f)
				{
					float a = bone.a;
					float b2 = bone.b;
					float c = bone.c;
					float d2 = bone.d;
					float r2 = MathUtils.Atan2(tc, ta) - MathUtils.Atan2(c, a) + offsetRotation;
					if (r2 > 3.14159265f)
					{
						r2 -= 3.14159265f * 2f;
					}
					else if (r2 < -3.14159265f)
					{
						r2 += 3.14159265f * 2f;
					}
					r2 *= rotateMix;
					float cos = MathUtils.Cos(r2);
					float sin = MathUtils.Sin(r2);
					bone.a = cos * a - sin * c;
					bone.b = cos * b2 - sin * d2;
					bone.c = sin * a + cos * c;
					bone.d = sin * b2 + cos * d2;
					modified = true;
				}
				if (translateMix != 0f)
				{
					target.LocalToWorld(data.offsetX, data.offsetY, out var tx, out var ty);
					bone.worldX += (tx - bone.worldX) * translateMix;
					bone.worldY += (ty - bone.worldY) * translateMix;
					modified = true;
				}
				if (scaleMix > 0f)
				{
					float s2 = (float)Math.Sqrt(bone.a * bone.a + bone.c * bone.c);
					if (s2 != 0f)
					{
						s2 = (s2 + ((float)Math.Sqrt(ta * ta + tc * tc) - s2 + data.offsetScaleX) * scaleMix) / s2;
					}
					bone.a *= s2;
					bone.c *= s2;
					s2 = (float)Math.Sqrt(bone.b * bone.b + bone.d * bone.d);
					if (s2 != 0f)
					{
						s2 = (s2 + ((float)Math.Sqrt(tb * tb + td * td) - s2 + data.offsetScaleY) * scaleMix) / s2;
					}
					bone.b *= s2;
					bone.d *= s2;
					modified = true;
				}
				if (shearMix > 0f)
				{
					float b = bone.b;
					float d = bone.d;
					float by = MathUtils.Atan2(d, b);
					float r = MathUtils.Atan2(td, tb) - MathUtils.Atan2(tc, ta) - (by - MathUtils.Atan2(bone.c, bone.a));
					if (r > 3.14159265f)
					{
						r -= 3.14159265f * 2f;
					}
					else if (r < -3.14159265f)
					{
						r += 3.14159265f * 2f;
					}
					r = by + (r + offsetShearY) * shearMix;
					float s = (float)Math.Sqrt(b * b + d * d);
					bone.b = MathUtils.Cos(r) * s;
					bone.d = MathUtils.Sin(r) * s;
					modified = true;
				}
				if (modified)
				{
					bone.appliedValid = false;
				}
			}
		}

		private void ApplyRelativeWorld()
		{
			float rotateMix = this.rotateMix;
			float translateMix = this.translateMix;
			float scaleMix = this.scaleMix;
			float shearMix = this.shearMix;
			Bone target = this.target;
			float ta = target.a;
			float tb = target.b;
			float tc = target.c;
			float td = target.d;
			float degRadReflect = ((ta * td - tb * tc > 0f) ? (3.14159265f / 180f) : (-3.14159265f / 180f));
			float offsetRotation = data.offsetRotation * degRadReflect;
			float offsetShearY = data.offsetShearY * degRadReflect;
			ExposedList<Bone> bones = this.bones;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bones.Items[i];
				bool modified = false;
				if (rotateMix != 0f)
				{
					float a = bone.a;
					float b2 = bone.b;
					float c = bone.c;
					float d2 = bone.d;
					float r2 = MathUtils.Atan2(tc, ta) + offsetRotation;
					if (r2 > 3.14159265f)
					{
						r2 -= 3.14159265f * 2f;
					}
					else if (r2 < -3.14159265f)
					{
						r2 += 3.14159265f * 2f;
					}
					r2 *= rotateMix;
					float cos = MathUtils.Cos(r2);
					float sin = MathUtils.Sin(r2);
					bone.a = cos * a - sin * c;
					bone.b = cos * b2 - sin * d2;
					bone.c = sin * a + cos * c;
					bone.d = sin * b2 + cos * d2;
					modified = true;
				}
				if (translateMix != 0f)
				{
					target.LocalToWorld(data.offsetX, data.offsetY, out var tx, out var ty);
					bone.worldX += tx * translateMix;
					bone.worldY += ty * translateMix;
					modified = true;
				}
				if (scaleMix > 0f)
				{
					float s2 = ((float)Math.Sqrt(ta * ta + tc * tc) - 1f + data.offsetScaleX) * scaleMix + 1f;
					bone.a *= s2;
					bone.c *= s2;
					s2 = ((float)Math.Sqrt(tb * tb + td * td) - 1f + data.offsetScaleY) * scaleMix + 1f;
					bone.b *= s2;
					bone.d *= s2;
					modified = true;
				}
				if (shearMix > 0f)
				{
					float r = MathUtils.Atan2(td, tb) - MathUtils.Atan2(tc, ta);
					if (r > 3.14159265f)
					{
						r -= 3.14159265f * 2f;
					}
					else if (r < -3.14159265f)
					{
						r += 3.14159265f * 2f;
					}
					float b = bone.b;
					float d = bone.d;
					r = MathUtils.Atan2(d, b) + (r - 3.14159265f / 2f + offsetShearY) * shearMix;
					float s = (float)Math.Sqrt(b * b + d * d);
					bone.b = MathUtils.Cos(r) * s;
					bone.d = MathUtils.Sin(r) * s;
					modified = true;
				}
				if (modified)
				{
					bone.appliedValid = false;
				}
			}
		}

		private void ApplyAbsoluteLocal()
		{
			float rotateMix = this.rotateMix;
			float translateMix = this.translateMix;
			float scaleMix = this.scaleMix;
			float shearMix = this.shearMix;
			Bone target = this.target;
			if (!target.appliedValid)
			{
				target.UpdateAppliedTransform();
			}
			Bone[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bonesItems[i];
				if (!bone.appliedValid)
				{
					bone.UpdateAppliedTransform();
				}
				float rotation = bone.arotation;
				if (rotateMix != 0f)
				{
					float r2 = target.arotation - rotation + data.offsetRotation;
					r2 -= (float)((16384 - (int)(16384.499999999996 - (double)(r2 / 360f))) * 360);
					rotation += r2 * rotateMix;
				}
				float x = bone.ax;
				float y = bone.ay;
				if (translateMix != 0f)
				{
					x += (target.ax - x + data.offsetX) * translateMix;
					y += (target.ay - y + data.offsetY) * translateMix;
				}
				float scaleX = bone.ascaleX;
				float scaleY = bone.ascaleY;
				if (scaleMix != 0f)
				{
					if (scaleX != 0f)
					{
						scaleX = (scaleX + (target.ascaleX - scaleX + data.offsetScaleX) * scaleMix) / scaleX;
					}
					if (scaleY != 0f)
					{
						scaleY = (scaleY + (target.ascaleY - scaleY + data.offsetScaleY) * scaleMix) / scaleY;
					}
				}
				float shearY = bone.ashearY;
				if (shearMix != 0f)
				{
					float r = target.ashearY - shearY + data.offsetShearY;
					r -= (float)((16384 - (int)(16384.499999999996 - (double)(r / 360f))) * 360);
					shearY += r * shearMix;
				}
				bone.UpdateWorldTransform(x, y, rotation, scaleX, scaleY, bone.ashearX, shearY);
			}
		}

		private void ApplyRelativeLocal()
		{
			float rotateMix = this.rotateMix;
			float translateMix = this.translateMix;
			float scaleMix = this.scaleMix;
			float shearMix = this.shearMix;
			Bone target = this.target;
			if (!target.appliedValid)
			{
				target.UpdateAppliedTransform();
			}
			Bone[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bonesItems[i];
				if (!bone.appliedValid)
				{
					bone.UpdateAppliedTransform();
				}
				float rotation = bone.arotation;
				if (rotateMix != 0f)
				{
					rotation += (target.arotation + data.offsetRotation) * rotateMix;
				}
				float x = bone.ax;
				float y = bone.ay;
				if (translateMix != 0f)
				{
					x += (target.ax + data.offsetX) * translateMix;
					y += (target.ay + data.offsetY) * translateMix;
				}
				float scaleX = bone.ascaleX;
				float scaleY = bone.ascaleY;
				if (scaleMix != 0f)
				{
					scaleX *= (target.ascaleX - 1f + data.offsetScaleX) * scaleMix + 1f;
					scaleY *= (target.ascaleY - 1f + data.offsetScaleY) * scaleMix + 1f;
				}
				float shearY = bone.ashearY;
				if (shearMix != 0f)
				{
					shearY += (target.ashearY + data.offsetShearY) * shearMix;
				}
				bone.UpdateWorldTransform(x, y, rotation, scaleX, scaleY, bone.ashearX, shearY);
			}
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}

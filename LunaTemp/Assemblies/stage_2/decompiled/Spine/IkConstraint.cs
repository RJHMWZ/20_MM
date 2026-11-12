using System;

namespace Spine
{
	public class IkConstraint : IUpdatable
	{
		internal IkConstraintData data;

		internal ExposedList<Bone> bones = new ExposedList<Bone>();

		internal Bone target;

		internal int bendDirection;

		internal bool compress;

		internal bool stretch;

		internal float mix = 1f;

		internal float softness;

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

		public float Mix
		{
			get
			{
				return mix;
			}
			set
			{
				mix = value;
			}
		}

		public float Softness
		{
			get
			{
				return softness;
			}
			set
			{
				softness = value;
			}
		}

		public int BendDirection
		{
			get
			{
				return bendDirection;
			}
			set
			{
				bendDirection = value;
			}
		}

		public bool Compress
		{
			get
			{
				return compress;
			}
			set
			{
				compress = value;
			}
		}

		public bool Stretch
		{
			get
			{
				return stretch;
			}
			set
			{
				stretch = value;
			}
		}

		public bool Active => active;

		public IkConstraintData Data => data;

		public IkConstraint(IkConstraintData data, Skeleton skeleton)
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
			mix = data.mix;
			softness = data.softness;
			bendDirection = data.bendDirection;
			compress = data.compress;
			stretch = data.stretch;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData boneData in data.bones)
			{
				bones.Add(skeleton.FindBone(boneData.name));
			}
			target = skeleton.FindBone(data.target.name);
		}

		public IkConstraint(IkConstraint constraint, Skeleton skeleton)
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
			mix = constraint.mix;
			softness = constraint.softness;
			bendDirection = constraint.bendDirection;
			compress = constraint.compress;
			stretch = constraint.stretch;
		}

		public void Apply()
		{
			Update();
		}

		public void Update()
		{
			Bone target = this.target;
			ExposedList<Bone> bones = this.bones;
			switch (bones.Count)
			{
			case 1:
				Apply(bones.Items[0], target.worldX, target.worldY, compress, stretch, data.uniform, mix);
				break;
			case 2:
				Apply(bones.Items[0], bones.Items[1], target.worldX, target.worldY, bendDirection, stretch, softness, mix);
				break;
			}
		}

		public override string ToString()
		{
			return data.name;
		}

		public static void Apply(Bone bone, float targetX, float targetY, bool compress, bool stretch, bool uniform, float alpha)
		{
			if (!bone.appliedValid)
			{
				bone.UpdateAppliedTransform();
			}
			Bone p = bone.parent;
			float pa = p.a;
			float pb = p.b;
			float pc = p.c;
			float pd = p.d;
			float rotationIK = 0f - bone.ashearX - bone.arotation;
			float tx = 0f;
			float ty = 0f;
			TransformMode transformMode = bone.data.transformMode;
			TransformMode transformMode2 = transformMode;
			if (transformMode2 != TransformMode.NoRotationOrReflection)
			{
				if (transformMode2 == TransformMode.OnlyTranslation)
				{
					tx = targetX - bone.worldX;
					ty = targetY - bone.worldY;
					goto IL_0153;
				}
			}
			else
			{
				float s2 = Math.Abs(pa * pd - pb * pc) / (pa * pa + pc * pc);
				float sa = pa / bone.skeleton.ScaleX;
				float sc = pc / bone.skeleton.ScaleY;
				pb = (0f - sc) * s2 * bone.skeleton.ScaleX;
				pd = sa * s2 * bone.skeleton.ScaleY;
				rotationIK += (float)Math.Atan2(pc, pa) * (180f / 3.14159265f);
			}
			float x = targetX - p.worldX;
			float y = targetY - p.worldY;
			float d = pa * pd - pb * pc;
			tx = (x * pd - y * pb) / d - bone.ax;
			ty = (y * pa - x * pc) / d - bone.ay;
			goto IL_0153;
			IL_0153:
			rotationIK += (float)Math.Atan2(ty, tx) * (180f / 3.14159265f);
			if (bone.ascaleX < 0f)
			{
				rotationIK += 180f;
			}
			if (rotationIK > 180f)
			{
				rotationIK -= 360f;
			}
			else if (rotationIK < -180f)
			{
				rotationIK += 360f;
			}
			float sx = bone.ascaleX;
			float sy = bone.ascaleY;
			if (compress || stretch)
			{
				switch (bone.data.transformMode)
				{
				case TransformMode.NoScale:
					tx = targetX - bone.worldX;
					ty = targetY - bone.worldY;
					break;
				case TransformMode.NoScaleOrReflection:
					tx = targetX - bone.worldX;
					ty = targetY - bone.worldY;
					break;
				}
				float b = bone.data.length * sx;
				float dd = (float)Math.Sqrt(tx * tx + ty * ty);
				if ((compress && dd < b) || (stretch && dd > b && b > 0.0001f))
				{
					float s = (dd / b - 1f) * alpha + 1f;
					sx *= s;
					if (uniform)
					{
						sy *= s;
					}
				}
			}
			bone.UpdateWorldTransform(bone.ax, bone.ay, bone.arotation + rotationIK * alpha, sx, sy, bone.ashearX, bone.ashearY);
		}

		public static void Apply(Bone parent, Bone child, float targetX, float targetY, int bendDir, bool stretch, float softness, float alpha)
		{
			if (alpha == 0f)
			{
				child.UpdateWorldTransform();
				return;
			}
			if (!parent.appliedValid)
			{
				parent.UpdateAppliedTransform();
			}
			if (!child.appliedValid)
			{
				child.UpdateAppliedTransform();
			}
			float px = parent.ax;
			float py = parent.ay;
			float psx = parent.ascaleX;
			float sx = psx;
			float psy = parent.ascaleY;
			float csx = child.ascaleX;
			int os2;
			int s2;
			if (psx < 0f)
			{
				psx = 0f - psx;
				os2 = 180;
				s2 = -1;
			}
			else
			{
				os2 = 0;
				s2 = 1;
			}
			if (psy < 0f)
			{
				psy = 0f - psy;
				s2 = -s2;
			}
			int os3;
			if (csx < 0f)
			{
				csx = 0f - csx;
				os3 = 180;
			}
			else
			{
				os3 = 0;
			}
			float cx = child.ax;
			float a = parent.a;
			float b = parent.b;
			float c = parent.c;
			float d = parent.d;
			bool u = Math.Abs(psx - psy) <= 0.0001f;
			float cy;
			float cwx;
			float cwy;
			if (!u)
			{
				cy = 0f;
				cwx = a * cx + parent.worldX;
				cwy = c * cx + parent.worldY;
			}
			else
			{
				cy = child.ay;
				cwx = a * cx + b * cy + parent.worldX;
				cwy = c * cx + d * cy + parent.worldY;
			}
			Bone pp = parent.parent;
			a = pp.a;
			b = pp.b;
			c = pp.c;
			d = pp.d;
			float id = 1f / (a * d - b * c);
			float x = cwx - pp.worldX;
			float y = cwy - pp.worldY;
			float dx = (x * d - y * b) * id - px;
			float dy = (y * a - x * c) * id - py;
			float l1 = (float)Math.Sqrt(dx * dx + dy * dy);
			float l2 = child.data.length * csx;
			if (l1 < 0.0001f)
			{
				Apply(parent, targetX, targetY, false, stretch, false, alpha);
				child.UpdateWorldTransform(cx, cy, 0f, child.ascaleX, child.ascaleY, child.ashearX, child.ashearY);
				return;
			}
			x = targetX - pp.worldX;
			y = targetY - pp.worldY;
			float tx = (x * d - y * b) * id - px;
			float ty = (y * a - x * c) * id - py;
			float dd = tx * tx + ty * ty;
			if (softness != 0f)
			{
				softness *= psx * (csx + 1f) / 2f;
				float td = (float)Math.Sqrt(dd);
				float sd = td - l1 - l2 * psx + softness;
				if (sd > 0f)
				{
					float p = Math.Min(1f, sd / (softness * 2f)) - 1f;
					p = (sd - softness * (1f - p * p)) / td;
					tx -= p * tx;
					ty -= p * ty;
					dd = tx * tx + ty * ty;
				}
			}
			float a2;
			float a3;
			if (u)
			{
				l2 *= psx;
				float cos = (dd - l1 * l1 - l2 * l2) / (2f * l1 * l2);
				if (cos < -1f)
				{
					cos = -1f;
				}
				else if (cos > 1f)
				{
					cos = 1f;
					if (stretch)
					{
						sx *= ((float)Math.Sqrt(dd) / (l1 + l2) - 1f) * alpha + 1f;
					}
				}
				a3 = (float)Math.Acos(cos) * (float)bendDir;
				a = l1 + l2 * cos;
				b = l2 * (float)Math.Sin(a3);
				a2 = (float)Math.Atan2(ty * a - tx * b, tx * a + ty * b);
			}
			else
			{
				a = psx * l2;
				b = psy * l2;
				float aa = a * a;
				float bb = b * b;
				float ta = (float)Math.Atan2(ty, tx);
				c = bb * l1 * l1 + aa * dd - aa * bb;
				float c2 = -2f * bb * l1;
				float c3 = bb - aa;
				d = c2 * c2 - 4f * c3 * c;
				if (d >= 0f)
				{
					float q = (float)Math.Sqrt(d);
					if (c2 < 0f)
					{
						q = 0f - q;
					}
					q = (0f - (c2 + q)) / 2f;
					float r2 = q / c3;
					float r3 = c / q;
					float r = ((Math.Abs(r2) < Math.Abs(r3)) ? r2 : r3);
					if (r * r <= dd)
					{
						y = (float)Math.Sqrt(dd - r * r) * (float)bendDir;
						a2 = ta - (float)Math.Atan2(y, r);
						a3 = (float)Math.Atan2(y / psy, (r - l1) / psx);
						goto IL_067d;
					}
				}
				float minAngle = 3.14159265f;
				float minX = l1 - a;
				float minDist = minX * minX;
				float minY = 0f;
				float maxAngle = 0f;
				float maxX = l1 + a;
				float maxDist = maxX * maxX;
				float maxY = 0f;
				c = (0f - a) * l1 / (aa - bb);
				if (c >= -1f && c <= 1f)
				{
					c = (float)Math.Acos(c);
					x = a * (float)Math.Cos(c) + l1;
					y = b * (float)Math.Sin(c);
					d = x * x + y * y;
					if (d < minDist)
					{
						minAngle = c;
						minDist = d;
						minX = x;
						minY = y;
					}
					if (d > maxDist)
					{
						maxAngle = c;
						maxDist = d;
						maxX = x;
						maxY = y;
					}
				}
				if (dd <= (minDist + maxDist) / 2f)
				{
					a2 = ta - (float)Math.Atan2(minY * (float)bendDir, minX);
					a3 = minAngle * (float)bendDir;
				}
				else
				{
					a2 = ta - (float)Math.Atan2(maxY * (float)bendDir, maxX);
					a3 = maxAngle * (float)bendDir;
				}
			}
			goto IL_067d;
			IL_067d:
			float os = (float)Math.Atan2(cy, cx) * (float)s2;
			float rotation = parent.arotation;
			a2 = (a2 - os) * (180f / 3.14159265f) + (float)os2 - rotation;
			if (a2 > 180f)
			{
				a2 -= 360f;
			}
			else if (a2 < -180f)
			{
				a2 += 360f;
			}
			parent.UpdateWorldTransform(px, py, rotation + a2 * alpha, sx, parent.ascaleY, 0f, 0f);
			rotation = child.arotation;
			a3 = ((a3 + os) * (180f / 3.14159265f) - child.ashearX) * (float)s2 + (float)os3 - rotation;
			if (a3 > 180f)
			{
				a3 -= 360f;
			}
			else if (a3 < -180f)
			{
				a3 += 360f;
			}
			child.UpdateWorldTransform(cx, cy, rotation + a3 * alpha, child.ascaleX, child.ascaleY, child.ashearX, child.ashearY);
		}
	}
}

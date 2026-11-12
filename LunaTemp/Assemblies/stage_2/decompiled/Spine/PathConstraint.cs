using System;

namespace Spine
{
	public class PathConstraint : IUpdatable
	{
		private const int NONE = -1;

		private const int BEFORE = -2;

		private const int AFTER = -3;

		private const float Epsilon = 1E-05f;

		internal PathConstraintData data;

		internal ExposedList<Bone> bones;

		internal Slot target;

		internal float position;

		internal float spacing;

		internal float rotateMix;

		internal float translateMix;

		internal bool active;

		internal ExposedList<float> spaces = new ExposedList<float>();

		internal ExposedList<float> positions = new ExposedList<float>();

		internal ExposedList<float> world = new ExposedList<float>();

		internal ExposedList<float> curves = new ExposedList<float>();

		internal ExposedList<float> lengths = new ExposedList<float>();

		internal float[] segments = new float[10];

		public float Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public float Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
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

		public ExposedList<Bone> Bones => bones;

		public Slot Target
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

		public bool Active => active;

		public PathConstraintData Data => data;

		public PathConstraint(PathConstraintData data, Skeleton skeleton)
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
			bones = new ExposedList<Bone>(data.Bones.Count);
			foreach (BoneData boneData in data.bones)
			{
				bones.Add(skeleton.FindBone(boneData.name));
			}
			target = skeleton.FindSlot(data.target.name);
			position = data.position;
			spacing = data.spacing;
			rotateMix = data.rotateMix;
			translateMix = data.translateMix;
		}

		public PathConstraint(PathConstraint constraint, Skeleton skeleton)
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
			target = skeleton.slots.Items[constraint.target.data.index];
			position = constraint.position;
			spacing = constraint.spacing;
			rotateMix = constraint.rotateMix;
			translateMix = constraint.translateMix;
		}

		public void Apply()
		{
			Update();
		}

		public void Update()
		{
			if (!(target.Attachment is PathAttachment attachment))
			{
				return;
			}
			float rotateMix = this.rotateMix;
			float translateMix = this.translateMix;
			bool translate = translateMix > 0f;
			bool rotate = rotateMix > 0f;
			if (!translate && !rotate)
			{
				return;
			}
			PathConstraintData data = this.data;
			bool percentSpacing = data.spacingMode == SpacingMode.Percent;
			RotateMode rotateMode = data.rotateMode;
			bool tangents = rotateMode == RotateMode.Tangent;
			bool scale = rotateMode == RotateMode.ChainScale;
			int boneCount = bones.Count;
			int spacesCount = (tangents ? boneCount : (boneCount + 1));
			Bone[] bonesItems = bones.Items;
			ExposedList<float> spaces = this.spaces.Resize(spacesCount);
			ExposedList<float> lengths = null;
			float spacing = this.spacing;
			if (scale || !percentSpacing)
			{
				if (scale)
				{
					lengths = this.lengths.Resize(boneCount);
				}
				bool lengthSpacing = data.spacingMode == SpacingMode.Length;
				int j = 0;
				int l = spacesCount - 1;
				while (j < l)
				{
					Bone bone = bonesItems[j];
					float setupLength = bone.data.length;
					if (setupLength < 1E-05f)
					{
						if (scale)
						{
							lengths.Items[j] = 0f;
						}
						spaces.Items[++j] = 0f;
					}
					else if (percentSpacing)
					{
						if (scale)
						{
							float x2 = setupLength * bone.a;
							float y2 = setupLength * bone.c;
							float length2 = (float)Math.Sqrt(x2 * x2 + y2 * y2);
							lengths.Items[j] = length2;
						}
						spaces.Items[++j] = spacing;
					}
					else
					{
						float x = setupLength * bone.a;
						float y = setupLength * bone.c;
						float length = (float)Math.Sqrt(x * x + y * y);
						if (scale)
						{
							lengths.Items[j] = length;
						}
						spaces.Items[++j] = (lengthSpacing ? (setupLength + spacing) : spacing) * length / setupLength;
					}
				}
			}
			else
			{
				for (int k = 1; k < spacesCount; k++)
				{
					spaces.Items[k] = spacing;
				}
			}
			float[] positions = ComputeWorldPositions(attachment, spacesCount, tangents, data.positionMode == PositionMode.Percent, percentSpacing);
			float boneX = positions[0];
			float boneY = positions[1];
			float offsetRotation = data.offsetRotation;
			bool tip;
			if (offsetRotation == 0f)
			{
				tip = rotateMode == RotateMode.Chain;
			}
			else
			{
				tip = false;
				Bone p2 = target.bone;
				offsetRotation *= ((p2.a * p2.d - p2.b * p2.c > 0f) ? (3.14159265f / 180f) : (-3.14159265f / 180f));
			}
			int i = 0;
			int p = 3;
			while (i < boneCount)
			{
				Bone bone2 = bonesItems[i];
				bone2.worldX += (boneX - bone2.worldX) * translateMix;
				bone2.worldY += (boneY - bone2.worldY) * translateMix;
				float x3 = positions[p];
				float y3 = positions[p + 1];
				float dx = x3 - boneX;
				float dy = y3 - boneY;
				if (scale)
				{
					float length4 = lengths.Items[i];
					if (length4 >= 1E-05f)
					{
						float s = ((float)Math.Sqrt(dx * dx + dy * dy) / length4 - 1f) * rotateMix + 1f;
						bone2.a *= s;
						bone2.c *= s;
					}
				}
				boneX = x3;
				boneY = y3;
				if (rotate)
				{
					float a = bone2.a;
					float b = bone2.b;
					float c = bone2.c;
					float d = bone2.d;
					float r = (tangents ? positions[p - 1] : ((!(spaces.Items[i + 1] < 1E-05f)) ? MathUtils.Atan2(dy, dx) : positions[p + 2]));
					r -= MathUtils.Atan2(c, a);
					float cos;
					float sin;
					if (tip)
					{
						cos = MathUtils.Cos(r);
						sin = MathUtils.Sin(r);
						float length3 = bone2.data.length;
						boneX += (length3 * (cos * a - sin * c) - dx) * rotateMix;
						boneY += (length3 * (sin * a + cos * c) - dy) * rotateMix;
					}
					else
					{
						r += offsetRotation;
					}
					if (r > 3.14159265f)
					{
						r -= 3.14159265f * 2f;
					}
					else if (r < -3.14159265f)
					{
						r += 3.14159265f * 2f;
					}
					r *= rotateMix;
					cos = MathUtils.Cos(r);
					sin = MathUtils.Sin(r);
					bone2.a = cos * a - sin * c;
					bone2.b = cos * b - sin * d;
					bone2.c = sin * a + cos * c;
					bone2.d = sin * b + cos * d;
				}
				bone2.appliedValid = false;
				i++;
				p += 3;
			}
		}

		private float[] ComputeWorldPositions(PathAttachment path, int spacesCount, bool tangents, bool percentPosition, bool percentSpacing)
		{
			Slot target = this.target;
			float position = this.position;
			float[] spacesItems = spaces.Items;
			float[] output = positions.Resize(spacesCount * 3 + 2).Items;
			bool closed = path.Closed;
			int verticesLength = path.WorldVerticesLength;
			int curveCount = verticesLength / 6;
			int prevCurve = -1;
			float pathLength = 0f;
			float[] world;
			if (!path.ConstantSpeed)
			{
				float[] lengths = path.Lengths;
				curveCount -= (closed ? 1 : 2);
				pathLength = lengths[curveCount];
				if (percentPosition)
				{
					position *= pathLength;
				}
				if (percentSpacing)
				{
					for (int m = 1; m < spacesCount; m++)
					{
						spacesItems[m] *= pathLength;
					}
				}
				world = this.world.Resize(8).Items;
				int l = 0;
				int o2 = 0;
				int curve2 = 0;
				for (; l < spacesCount; l++, o2 += 3)
				{
					float space2 = spacesItems[l];
					position += space2;
					float p2 = position;
					if (closed)
					{
						p2 %= pathLength;
						if (p2 < 0f)
						{
							p2 += pathLength;
						}
						curve2 = 0;
					}
					else
					{
						if (p2 < 0f)
						{
							if (prevCurve != -2)
							{
								prevCurve = -2;
								path.ComputeWorldVertices(target, 2, 4, world, 0);
							}
							AddBeforePosition(p2, world, 0, output, o2);
							continue;
						}
						if (p2 > pathLength)
						{
							if (prevCurve != -3)
							{
								prevCurve = -3;
								path.ComputeWorldVertices(target, verticesLength - 6, 4, world, 0);
							}
							AddAfterPosition(p2 - pathLength, world, 0, output, o2);
							continue;
						}
					}
					float length3;
					while (true)
					{
						length3 = lengths[curve2];
						if (!(p2 > length3))
						{
							break;
						}
						curve2++;
					}
					if (curve2 == 0)
					{
						p2 /= length3;
					}
					else
					{
						float prev3 = lengths[curve2 - 1];
						p2 = (p2 - prev3) / (length3 - prev3);
					}
					if (curve2 != prevCurve)
					{
						prevCurve = curve2;
						if (closed && curve2 == curveCount)
						{
							path.ComputeWorldVertices(target, verticesLength - 4, 4, world, 0);
							path.ComputeWorldVertices(target, 0, 4, world, 4);
						}
						else
						{
							path.ComputeWorldVertices(target, curve2 * 6 + 2, 8, world, 0);
						}
					}
					AddCurvePosition(p2, world[0], world[1], world[2], world[3], world[4], world[5], world[6], world[7], output, o2, tangents || (l > 0 && space2 < 1E-05f));
				}
				return output;
			}
			if (closed)
			{
				verticesLength += 2;
				world = this.world.Resize(verticesLength).Items;
				path.ComputeWorldVertices(target, 2, verticesLength - 4, world, 0);
				path.ComputeWorldVertices(target, 0, 2, world, verticesLength - 4);
				world[verticesLength - 2] = world[0];
				world[verticesLength - 1] = world[1];
			}
			else
			{
				curveCount--;
				verticesLength -= 4;
				world = this.world.Resize(verticesLength).Items;
				path.ComputeWorldVertices(target, 2, verticesLength, world, 0);
			}
			float[] curves = this.curves.Resize(curveCount).Items;
			pathLength = 0f;
			float x1 = world[0];
			float y1 = world[1];
			float cx1 = 0f;
			float cy1 = 0f;
			float cx2 = 0f;
			float cy2 = 0f;
			float x2 = 0f;
			float y2 = 0f;
			int k = 0;
			int w = 2;
			while (k < curveCount)
			{
				cx1 = world[w];
				cy1 = world[w + 1];
				cx2 = world[w + 2];
				cy2 = world[w + 3];
				x2 = world[w + 4];
				y2 = world[w + 5];
				float tmpx = (x1 - cx1 * 2f + cx2) * 0.1875f;
				float tmpy = (y1 - cy1 * 2f + cy2) * 0.1875f;
				float dddfx = ((cx1 - cx2) * 3f - x1 + x2) * (3f / 32f);
				float dddfy = ((cy1 - cy2) * 3f - y1 + y2) * (3f / 32f);
				float ddfx = tmpx * 2f + dddfx;
				float ddfy = tmpy * 2f + dddfy;
				float dfx = (cx1 - x1) * 0.75f + tmpx + dddfx * (1f / 6f);
				float dfy = (cy1 - y1) * 0.75f + tmpy + dddfy * (1f / 6f);
				pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
				dfx += ddfx;
				dfy += ddfy;
				ddfx += dddfx;
				ddfy += dddfy;
				pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
				dfx += ddfx;
				dfy += ddfy;
				pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
				dfx += ddfx + dddfx;
				dfy += ddfy + dddfy;
				pathLength = (curves[k] = pathLength + (float)Math.Sqrt(dfx * dfx + dfy * dfy));
				x1 = x2;
				y1 = y2;
				k++;
				w += 6;
			}
			position = ((!percentPosition) ? (position * (pathLength / path.lengths[curveCount - 1])) : (position * pathLength));
			if (percentSpacing)
			{
				for (int j = 1; j < spacesCount; j++)
				{
					spacesItems[j] *= pathLength;
				}
			}
			float[] segments = this.segments;
			float curveLength = 0f;
			int i = 0;
			int o = 0;
			int curve = 0;
			int segment = 0;
			for (; i < spacesCount; i++, o += 3)
			{
				float space = spacesItems[i];
				position += space;
				float p = position;
				if (closed)
				{
					p %= pathLength;
					if (p < 0f)
					{
						p += pathLength;
					}
					curve = 0;
				}
				else
				{
					if (p < 0f)
					{
						AddBeforePosition(p, world, 0, output, o);
						continue;
					}
					if (p > pathLength)
					{
						AddAfterPosition(p - pathLength, world, verticesLength - 4, output, o);
						continue;
					}
				}
				float length2;
				while (true)
				{
					length2 = curves[curve];
					if (!(p > length2))
					{
						break;
					}
					curve++;
				}
				if (curve == 0)
				{
					p /= length2;
				}
				else
				{
					float prev2 = curves[curve - 1];
					p = (p - prev2) / (length2 - prev2);
				}
				if (curve != prevCurve)
				{
					prevCurve = curve;
					int ii = curve * 6;
					x1 = world[ii];
					y1 = world[ii + 1];
					cx1 = world[ii + 2];
					cy1 = world[ii + 3];
					cx2 = world[ii + 4];
					cy2 = world[ii + 5];
					x2 = world[ii + 6];
					y2 = world[ii + 7];
					float tmpx = (x1 - cx1 * 2f + cx2) * 0.03f;
					float tmpy = (y1 - cy1 * 2f + cy2) * 0.03f;
					float dddfx = ((cx1 - cx2) * 3f - x1 + x2) * 0.006f;
					float dddfy = ((cy1 - cy2) * 3f - y1 + y2) * 0.006f;
					float ddfx = tmpx * 2f + dddfx;
					float ddfy = tmpy * 2f + dddfy;
					float dfx = (cx1 - x1) * 0.3f + tmpx + dddfx * (1f / 6f);
					float dfy = (cy1 - y1) * 0.3f + tmpy + dddfy * (1f / 6f);
					curveLength = (segments[0] = (float)Math.Sqrt(dfx * dfx + dfy * dfy));
					for (ii = 1; ii < 8; ii++)
					{
						dfx += ddfx;
						dfy += ddfy;
						ddfx += dddfx;
						ddfy += dddfy;
						curveLength = (segments[ii] = curveLength + (float)Math.Sqrt(dfx * dfx + dfy * dfy));
					}
					dfx += ddfx;
					dfy += ddfy;
					curveLength = (segments[8] = curveLength + (float)Math.Sqrt(dfx * dfx + dfy * dfy));
					dfx += ddfx + dddfx;
					dfy += ddfy + dddfy;
					curveLength = (segments[9] = curveLength + (float)Math.Sqrt(dfx * dfx + dfy * dfy));
					segment = 0;
				}
				p *= curveLength;
				float length;
				while (true)
				{
					length = segments[segment];
					if (!(p > length))
					{
						break;
					}
					segment++;
				}
				if (segment == 0)
				{
					p /= length;
				}
				else
				{
					float prev = segments[segment - 1];
					p = (float)segment + (p - prev) / (length - prev);
				}
				AddCurvePosition(p * 0.1f, x1, y1, cx1, cy1, cx2, cy2, x2, y2, output, o, tangents || (i > 0 && space < 1E-05f));
			}
			return output;
		}

		private static void AddBeforePosition(float p, float[] temp, int i, float[] output, int o)
		{
			float x1 = temp[i];
			float y1 = temp[i + 1];
			float dx = temp[i + 2] - x1;
			float dy = temp[i + 3] - y1;
			float r = MathUtils.Atan2(dy, dx);
			output[o] = x1 + p * MathUtils.Cos(r);
			output[o + 1] = y1 + p * MathUtils.Sin(r);
			output[o + 2] = r;
		}

		private static void AddAfterPosition(float p, float[] temp, int i, float[] output, int o)
		{
			float x1 = temp[i + 2];
			float y1 = temp[i + 3];
			float dx = x1 - temp[i];
			float dy = y1 - temp[i + 1];
			float r = MathUtils.Atan2(dy, dx);
			output[o] = x1 + p * MathUtils.Cos(r);
			output[o + 1] = y1 + p * MathUtils.Sin(r);
			output[o + 2] = r;
		}

		private static void AddCurvePosition(float p, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2, float[] output, int o, bool tangents)
		{
			if (p < 1E-05f || float.IsNaN(p))
			{
				output[o] = x1;
				output[o + 1] = y1;
				output[o + 2] = (float)Math.Atan2(cy1 - y1, cx1 - x1);
				return;
			}
			float tt = p * p;
			float ttt = tt * p;
			float u = 1f - p;
			float uu = u * u;
			float uuu = uu * u;
			float ut = u * p;
			float ut2 = ut * 3f;
			float uut3 = u * ut2;
			float utt3 = ut2 * p;
			float x3 = x1 * uuu + cx1 * uut3 + cx2 * utt3 + x2 * ttt;
			float y3 = y1 * uuu + cy1 * uut3 + cy2 * utt3 + y2 * ttt;
			output[o] = x3;
			output[o + 1] = y3;
			if (tangents)
			{
				if (p < 0.001f)
				{
					output[o + 2] = (float)Math.Atan2(cy1 - y1, cx1 - x1);
				}
				else
				{
					output[o + 2] = (float)Math.Atan2(y3 - (y1 * uu + cy1 * ut * 2f + cy2 * tt), x3 - (x1 * uu + cx1 * ut * 2f + cx2 * tt));
				}
			}
		}
	}
}

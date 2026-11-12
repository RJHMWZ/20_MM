using System;

namespace Spine
{
	public class Skeleton
	{
		internal SkeletonData data;

		internal ExposedList<Bone> bones;

		internal ExposedList<Slot> slots;

		internal ExposedList<Slot> drawOrder;

		internal ExposedList<IkConstraint> ikConstraints;

		internal ExposedList<TransformConstraint> transformConstraints;

		internal ExposedList<PathConstraint> pathConstraints;

		internal ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();

		internal ExposedList<Bone> updateCacheReset = new ExposedList<Bone>();

		internal Skin skin;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal float time;

		private float scaleX = 1f;

		private float scaleY = 1f;

		internal float x;

		internal float y;

		public SkeletonData Data => data;

		public ExposedList<Bone> Bones => bones;

		public ExposedList<IUpdatable> UpdateCacheList => updateCache;

		public ExposedList<Slot> Slots => slots;

		public ExposedList<Slot> DrawOrder => drawOrder;

		public ExposedList<IkConstraint> IkConstraints => ikConstraints;

		public ExposedList<PathConstraint> PathConstraints => pathConstraints;

		public ExposedList<TransformConstraint> TransformConstraints => transformConstraints;

		public Skin Skin
		{
			get
			{
				return skin;
			}
			set
			{
				SetSkin(value);
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

		public float Time
		{
			get
			{
				return time;
			}
			set
			{
				time = value;
			}
		}

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
				return scaleY * (float)((!Bone.yDown) ? 1 : (-1));
			}
			set
			{
				scaleY = value;
			}
		}

		[Obsolete("Use ScaleX instead. FlipX is when ScaleX is negative.")]
		public bool FlipX
		{
			get
			{
				return scaleX < 0f;
			}
			set
			{
				scaleX = (value ? (-1f) : 1f);
			}
		}

		[Obsolete("Use ScaleY instead. FlipY is when ScaleY is negative.")]
		public bool FlipY
		{
			get
			{
				return scaleY < 0f;
			}
			set
			{
				scaleY = (value ? (-1f) : 1f);
			}
		}

		public Bone RootBone => (bones.Count == 0) ? null : bones.Items[0];

		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			this.data = data;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData boneData in data.bones)
			{
				Bone bone2;
				if (boneData.parent == null)
				{
					bone2 = new Bone(boneData, this, null);
				}
				else
				{
					Bone parent = bones.Items[boneData.parent.index];
					bone2 = new Bone(boneData, this, parent);
					parent.children.Add(bone2);
				}
				bones.Add(bone2);
			}
			slots = new ExposedList<Slot>(data.slots.Count);
			drawOrder = new ExposedList<Slot>(data.slots.Count);
			foreach (SlotData slotData in data.slots)
			{
				Bone bone = bones.Items[slotData.boneData.index];
				Slot slot = new Slot(slotData, bone);
				slots.Add(slot);
				drawOrder.Add(slot);
			}
			ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
			foreach (IkConstraintData ikConstraintData in data.ikConstraints)
			{
				ikConstraints.Add(new IkConstraint(ikConstraintData, this));
			}
			transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
			foreach (TransformConstraintData transformConstraintData in data.transformConstraints)
			{
				transformConstraints.Add(new TransformConstraint(transformConstraintData, this));
			}
			pathConstraints = new ExposedList<PathConstraint>(data.pathConstraints.Count);
			foreach (PathConstraintData pathConstraintData in data.pathConstraints)
			{
				pathConstraints.Add(new PathConstraint(pathConstraintData, this));
			}
			UpdateCache();
			UpdateWorldTransform();
		}

		public void UpdateCache()
		{
			ExposedList<IUpdatable> updateCache = this.updateCache;
			updateCache.Clear();
			updateCacheReset.Clear();
			int boneCount = this.bones.Items.Length;
			ExposedList<Bone> bones = this.bones;
			for (int l = 0; l < boneCount; l++)
			{
				Bone bone = bones.Items[l];
				bone.sorted = bone.data.skinRequired;
				bone.active = !bone.sorted;
			}
			if (skin != null)
			{
				object[] items = skin.bones.Items;
				object[] skinBones = items;
				int k = 0;
				for (int m = skin.bones.Count; k < m; k++)
				{
					Bone bone2 = bones.Items[((BoneData)skinBones[k]).index];
					do
					{
						bone2.sorted = false;
						bone2.active = true;
						bone2 = bone2.parent;
					}
					while (bone2 != null);
				}
			}
			int ikCount = this.ikConstraints.Count;
			int transformCount = this.transformConstraints.Count;
			int pathCount = this.pathConstraints.Count;
			ExposedList<IkConstraint> ikConstraints = this.ikConstraints;
			ExposedList<TransformConstraint> transformConstraints = this.transformConstraints;
			ExposedList<PathConstraint> pathConstraints = this.pathConstraints;
			int constraintCount = ikCount + transformCount + pathCount;
			for (int j = 0; j < constraintCount; j++)
			{
				int ii3 = 0;
				while (true)
				{
					if (ii3 < ikCount)
					{
						IkConstraint constraint = ikConstraints.Items[ii3];
						if (constraint.data.order == j)
						{
							SortIkConstraint(constraint);
							break;
						}
						ii3++;
						continue;
					}
					int ii2 = 0;
					while (true)
					{
						if (ii2 < transformCount)
						{
							TransformConstraint constraint2 = transformConstraints.Items[ii2];
							if (constraint2.data.order == j)
							{
								SortTransformConstraint(constraint2);
								break;
							}
							ii2++;
							continue;
						}
						for (int ii = 0; ii < pathCount; ii++)
						{
							PathConstraint constraint3 = pathConstraints.Items[ii];
							if (constraint3.data.order == j)
							{
								SortPathConstraint(constraint3);
								break;
							}
						}
						break;
					}
					break;
				}
			}
			for (int i = 0; i < boneCount; i++)
			{
				SortBone(bones.Items[i]);
			}
		}

		private void SortIkConstraint(IkConstraint constraint)
		{
			constraint.active = constraint.target.active && (!constraint.data.skinRequired || (skin != null && skin.constraints.Contains(constraint.data)));
			if (!constraint.active)
			{
				return;
			}
			Bone target = constraint.target;
			SortBone(target);
			ExposedList<Bone> constrained = constraint.bones;
			Bone parent = constrained.Items[0];
			SortBone(parent);
			if (constrained.Count > 1)
			{
				Bone child = constrained.Items[constrained.Count - 1];
				if (!updateCache.Contains(child))
				{
					updateCacheReset.Add(child);
				}
			}
			updateCache.Add(constraint);
			SortReset(parent.children);
			constrained.Items[constrained.Count - 1].sorted = true;
		}

		private void SortPathConstraint(PathConstraint constraint)
		{
			constraint.active = constraint.target.bone.active && (!constraint.data.skinRequired || (skin != null && skin.constraints.Contains(constraint.data)));
			if (constraint.active)
			{
				Slot slot = constraint.target;
				int slotIndex = slot.data.index;
				Bone slotBone = slot.bone;
				if (skin != null)
				{
					SortPathConstraintAttachment(skin, slotIndex, slotBone);
				}
				if (data.defaultSkin != null && data.defaultSkin != skin)
				{
					SortPathConstraintAttachment(data.defaultSkin, slotIndex, slotBone);
				}
				Attachment attachment = slot.attachment;
				if (attachment is PathAttachment)
				{
					SortPathConstraintAttachment(attachment, slotBone);
				}
				ExposedList<Bone> constrained = constraint.bones;
				int boneCount = constrained.Count;
				for (int k = 0; k < boneCount; k++)
				{
					SortBone(constrained.Items[k]);
				}
				updateCache.Add(constraint);
				for (int j = 0; j < boneCount; j++)
				{
					SortReset(constrained.Items[j].children);
				}
				for (int i = 0; i < boneCount; i++)
				{
					constrained.Items[i].sorted = true;
				}
			}
		}

		private void SortTransformConstraint(TransformConstraint constraint)
		{
			constraint.active = constraint.target.active && (!constraint.data.skinRequired || (skin != null && skin.constraints.Contains(constraint.data)));
			if (!constraint.active)
			{
				return;
			}
			SortBone(constraint.target);
			ExposedList<Bone> constrained = constraint.bones;
			int boneCount = constrained.Count;
			if (constraint.data.local)
			{
				for (int l = 0; l < boneCount; l++)
				{
					Bone child = constrained.Items[l];
					SortBone(child.parent);
					if (!updateCache.Contains(child))
					{
						updateCacheReset.Add(child);
					}
				}
			}
			else
			{
				for (int k = 0; k < boneCount; k++)
				{
					SortBone(constrained.Items[k]);
				}
			}
			updateCache.Add(constraint);
			for (int j = 0; j < boneCount; j++)
			{
				SortReset(constrained.Items[j].children);
			}
			for (int i = 0; i < boneCount; i++)
			{
				constrained.Items[i].sorted = true;
			}
		}

		private void SortPathConstraintAttachment(Skin skin, int slotIndex, Bone slotBone)
		{
			foreach (Skin.SkinEntry entryObj in skin.Attachments.Keys)
			{
				Skin.SkinEntry entry = entryObj;
				if (entry.SlotIndex == slotIndex)
				{
					SortPathConstraintAttachment(entry.Attachment, slotBone);
				}
			}
		}

		private void SortPathConstraintAttachment(Attachment attachment, Bone slotBone)
		{
			if (!(attachment is PathAttachment))
			{
				return;
			}
			int[] pathBones = ((PathAttachment)attachment).bones;
			if (pathBones == null)
			{
				SortBone(slotBone);
				return;
			}
			ExposedList<Bone> bones = this.bones;
			int i = 0;
			int j = pathBones.Length;
			while (i < j)
			{
				int nn = pathBones[i++];
				nn += i;
				while (i < nn)
				{
					SortBone(bones.Items[pathBones[i++]]);
				}
			}
		}

		private void SortBone(Bone bone)
		{
			if (!bone.sorted)
			{
				Bone parent = bone.parent;
				if (parent != null)
				{
					SortBone(parent);
				}
				bone.sorted = true;
				updateCache.Add(bone);
			}
		}

		private static void SortReset(ExposedList<Bone> bones)
		{
			Bone[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bonesItems[i];
				if (bone.active)
				{
					if (bone.sorted)
					{
						SortReset(bone.children);
					}
					bone.sorted = false;
				}
			}
		}

		public void UpdateWorldTransform()
		{
			ExposedList<Bone> updateCacheReset = this.updateCacheReset;
			Bone[] updateCacheResetItems = updateCacheReset.Items;
			int j = 0;
			for (int l = updateCacheReset.Count; j < l; j++)
			{
				Bone bone = updateCacheResetItems[j];
				bone.ax = bone.x;
				bone.ay = bone.y;
				bone.arotation = bone.rotation;
				bone.ascaleX = bone.scaleX;
				bone.ascaleY = bone.scaleY;
				bone.ashearX = bone.shearX;
				bone.ashearY = bone.shearY;
				bone.appliedValid = true;
			}
			IUpdatable[] updateItems = updateCache.Items;
			int i = 0;
			for (int k = updateCache.Count; i < k; i++)
			{
				updateItems[i].Update();
			}
		}

		public void UpdateWorldTransform(Bone parent)
		{
			ExposedList<Bone> updateCacheReset = this.updateCacheReset;
			Bone[] updateCacheResetItems = updateCacheReset.Items;
			int j = 0;
			for (int l = updateCacheReset.Count; j < l; j++)
			{
				Bone bone = updateCacheResetItems[j];
				bone.ax = bone.x;
				bone.ay = bone.y;
				bone.arotation = bone.rotation;
				bone.ascaleX = bone.scaleX;
				bone.ascaleY = bone.scaleY;
				bone.ashearX = bone.shearX;
				bone.ashearY = bone.shearY;
				bone.appliedValid = true;
			}
			Bone rootBone = RootBone;
			float pa = parent.a;
			float pb = parent.b;
			float pc = parent.c;
			float pd = parent.d;
			rootBone.worldX = pa * x + pb * y + parent.worldX;
			rootBone.worldY = pc * x + pd * y + parent.worldY;
			float rotationY = rootBone.rotation + 90f + rootBone.shearY;
			float la = MathUtils.CosDeg(rootBone.rotation + rootBone.shearX) * rootBone.scaleX;
			float lb = MathUtils.CosDeg(rotationY) * rootBone.scaleY;
			float lc = MathUtils.SinDeg(rootBone.rotation + rootBone.shearX) * rootBone.scaleX;
			float ld = MathUtils.SinDeg(rotationY) * rootBone.scaleY;
			rootBone.a = (pa * la + pb * lc) * scaleX;
			rootBone.b = (pa * lb + pb * ld) * scaleX;
			rootBone.c = (pc * la + pd * lc) * scaleY;
			rootBone.d = (pc * lb + pd * ld) * scaleY;
			ExposedList<IUpdatable> updateCache = this.updateCache;
			IUpdatable[] updateCacheItems = updateCache.Items;
			int i = 0;
			for (int k = updateCache.Count; i < k; i++)
			{
				IUpdatable updatable = updateCacheItems[i];
				if (updatable != rootBone)
				{
					updatable.Update();
				}
			}
		}

		public void SetToSetupPose()
		{
			SetBonesToSetupPose();
			SetSlotsToSetupPose();
		}

		public void SetBonesToSetupPose()
		{
			Bone[] bonesItems = bones.Items;
			int l = 0;
			for (int n3 = bones.Count; l < n3; l++)
			{
				bonesItems[l].SetToSetupPose();
			}
			IkConstraint[] ikConstraintsItems = ikConstraints.Items;
			int k = 0;
			for (int n2 = ikConstraints.Count; k < n2; k++)
			{
				IkConstraint constraint = ikConstraintsItems[k];
				constraint.mix = constraint.data.mix;
				constraint.softness = constraint.data.softness;
				constraint.bendDirection = constraint.data.bendDirection;
				constraint.compress = constraint.data.compress;
				constraint.stretch = constraint.data.stretch;
			}
			TransformConstraint[] transformConstraintsItems = transformConstraints.Items;
			int j = 0;
			for (int n = transformConstraints.Count; j < n; j++)
			{
				TransformConstraint constraint2 = transformConstraintsItems[j];
				TransformConstraintData constraintData = constraint2.data;
				constraint2.rotateMix = constraintData.rotateMix;
				constraint2.translateMix = constraintData.translateMix;
				constraint2.scaleMix = constraintData.scaleMix;
				constraint2.shearMix = constraintData.shearMix;
			}
			PathConstraint[] pathConstraintItems = pathConstraints.Items;
			int i = 0;
			for (int m = pathConstraints.Count; i < m; i++)
			{
				PathConstraint constraint3 = pathConstraintItems[i];
				PathConstraintData constraintData2 = constraint3.data;
				constraint3.position = constraintData2.position;
				constraint3.spacing = constraintData2.spacing;
				constraint3.rotateMix = constraintData2.rotateMix;
				constraint3.translateMix = constraintData2.translateMix;
			}
		}

		public void SetSlotsToSetupPose()
		{
			ExposedList<Slot> slots = this.slots;
			Slot[] slotsItems = slots.Items;
			drawOrder.Clear();
			int j = 0;
			for (int l = slots.Count; j < l; j++)
			{
				drawOrder.Add(slotsItems[j]);
			}
			int i = 0;
			for (int k = slots.Count; i < k; i++)
			{
				slotsItems[i].SetToSetupPose();
			}
		}

		public Bone FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<Bone> bones = this.bones;
			Bone[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				Bone bone = bonesItems[i];
				if (bone.data.name == boneName)
				{
					return bone;
				}
			}
			return null;
		}

		public int FindBoneIndex(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<Bone> bones = this.bones;
			Bone[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				if (bonesItems[i].data.name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> slots = this.slots;
			Slot[] slotsItems = slots.Items;
			int i = 0;
			for (int j = slots.Count; i < j; i++)
			{
				Slot slot = slotsItems[i];
				if (slot.data.name == slotName)
				{
					return slot;
				}
			}
			return null;
		}

		public int FindSlotIndex(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> slots = this.slots;
			Slot[] slotsItems = slots.Items;
			int i = 0;
			for (int j = slots.Count; i < j; i++)
			{
				if (slotsItems[i].data.name.Equals(slotName))
				{
					return i;
				}
			}
			return -1;
		}

		public void SetSkin(string skinName)
		{
			Skin foundSkin = data.FindSkin(skinName);
			if (foundSkin == null)
			{
				throw new ArgumentException("Skin not found: " + skinName, "skinName");
			}
			SetSkin(foundSkin);
		}

		public void SetSkin(Skin newSkin)
		{
			if (newSkin == skin)
			{
				return;
			}
			if (newSkin != null)
			{
				if (skin != null)
				{
					newSkin.AttachAll(this, skin);
				}
				else
				{
					ExposedList<Slot> slots = this.slots;
					int i = 0;
					for (int j = slots.Count; i < j; i++)
					{
						Slot slot = slots.Items[i];
						string name = slot.data.attachmentName;
						if (name != null)
						{
							Attachment attachment = newSkin.GetAttachment(i, name);
							if (attachment != null)
							{
								slot.Attachment = attachment;
							}
						}
					}
				}
			}
			skin = newSkin;
			UpdateCache();
		}

		public Attachment GetAttachment(string slotName, string attachmentName)
		{
			return GetAttachment(data.FindSlotIndex(slotName), attachmentName);
		}

		public Attachment GetAttachment(int slotIndex, string attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentNullException("attachmentName", "attachmentName cannot be null.");
			}
			if (skin != null)
			{
				Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			return (data.defaultSkin != null) ? data.defaultSkin.GetAttachment(slotIndex, attachmentName) : null;
		}

		public void SetAttachment(string slotName, string attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> slots = this.slots;
			int i = 0;
			for (int j = slots.Count; i < j; i++)
			{
				Slot slot = slots.Items[i];
				if (!(slot.data.name == slotName))
				{
					continue;
				}
				Attachment attachment = null;
				if (attachmentName != null)
				{
					attachment = GetAttachment(i, attachmentName);
					if (attachment == null)
					{
						throw new Exception("Attachment not found: " + attachmentName + ", for slot: " + slotName);
					}
				}
				slot.Attachment = attachment;
				return;
			}
			throw new Exception("Slot not found: " + slotName);
		}

		public IkConstraint FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<IkConstraint> ikConstraints = this.ikConstraints;
			int i = 0;
			for (int j = ikConstraints.Count; i < j; i++)
			{
				IkConstraint ikConstraint = ikConstraints.Items[i];
				if (ikConstraint.data.name == constraintName)
				{
					return ikConstraint;
				}
			}
			return null;
		}

		public TransformConstraint FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<TransformConstraint> transformConstraints = this.transformConstraints;
			int i = 0;
			for (int j = transformConstraints.Count; i < j; i++)
			{
				TransformConstraint transformConstraint = transformConstraints.Items[i];
				if (transformConstraint.data.Name == constraintName)
				{
					return transformConstraint;
				}
			}
			return null;
		}

		public PathConstraint FindPathConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<PathConstraint> pathConstraints = this.pathConstraints;
			int i = 0;
			for (int j = pathConstraints.Count; i < j; i++)
			{
				PathConstraint constraint = pathConstraints.Items[i];
				if (constraint.data.Name.Equals(constraintName))
				{
					return constraint;
				}
			}
			return null;
		}

		public void Update(float delta)
		{
			time += delta;
		}

		public void GetBounds(out float x, out float y, out float width, out float height, ref float[] vertexBuffer)
		{
			float[] temp = vertexBuffer;
			temp = temp ?? new float[8];
			Slot[] drawOrderItems = drawOrder.Items;
			float minX = 2.1474836E+09f;
			float minY = 2.1474836E+09f;
			float maxX = -2.1474836E+09f;
			float maxY = -2.1474836E+09f;
			int i = 0;
			for (int j = drawOrderItems.Length; i < j; i++)
			{
				Slot slot = drawOrderItems[i];
				if (!slot.bone.active)
				{
					continue;
				}
				int verticesLength = 0;
				float[] vertices = null;
				Attachment attachment = slot.attachment;
				if (attachment is RegionAttachment regionAttachment)
				{
					verticesLength = 8;
					vertices = temp;
					if (vertices.Length < 8)
					{
						vertices = (temp = new float[8]);
					}
					regionAttachment.ComputeWorldVertices(slot.bone, temp, 0);
				}
				else if (attachment is MeshAttachment meshAttachment)
				{
					MeshAttachment mesh = meshAttachment;
					verticesLength = mesh.WorldVerticesLength;
					vertices = temp;
					if (vertices.Length < verticesLength)
					{
						vertices = (temp = new float[verticesLength]);
					}
					mesh.ComputeWorldVertices(slot, 0, verticesLength, temp, 0);
				}
				if (vertices != null)
				{
					for (int ii = 0; ii < verticesLength; ii += 2)
					{
						float vx = vertices[ii];
						float vy = vertices[ii + 1];
						minX = Math.Min(minX, vx);
						minY = Math.Min(minY, vy);
						maxX = Math.Max(maxX, vx);
						maxY = Math.Max(maxY, vy);
					}
				}
			}
			x = minX;
			y = minY;
			width = maxX - minX;
			height = maxY - minY;
			vertexBuffer = temp;
		}
	}
}

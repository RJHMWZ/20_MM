using System;

namespace Spine
{
	public class SkeletonData
	{
		internal string name;

		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal ExposedList<SlotData> slots = new ExposedList<SlotData>();

		internal ExposedList<Skin> skins = new ExposedList<Skin>();

		internal Skin defaultSkin;

		internal ExposedList<EventData> events = new ExposedList<EventData>();

		internal ExposedList<Animation> animations = new ExposedList<Animation>();

		internal ExposedList<IkConstraintData> ikConstraints = new ExposedList<IkConstraintData>();

		internal ExposedList<TransformConstraintData> transformConstraints = new ExposedList<TransformConstraintData>();

		internal ExposedList<PathConstraintData> pathConstraints = new ExposedList<PathConstraintData>();

		internal float x;

		internal float y;

		internal float width;

		internal float height;

		internal string version;

		internal string hash;

		internal float fps;

		internal string imagesPath;

		internal string audioPath;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public ExposedList<BoneData> Bones => bones;

		public ExposedList<SlotData> Slots => slots;

		public ExposedList<Skin> Skins
		{
			get
			{
				return skins;
			}
			set
			{
				skins = value;
			}
		}

		public Skin DefaultSkin
		{
			get
			{
				return defaultSkin;
			}
			set
			{
				defaultSkin = value;
			}
		}

		public ExposedList<EventData> Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		public ExposedList<Animation> Animations
		{
			get
			{
				return animations;
			}
			set
			{
				animations = value;
			}
		}

		public ExposedList<IkConstraintData> IkConstraints
		{
			get
			{
				return ikConstraints;
			}
			set
			{
				ikConstraints = value;
			}
		}

		public ExposedList<TransformConstraintData> TransformConstraints
		{
			get
			{
				return transformConstraints;
			}
			set
			{
				transformConstraints = value;
			}
		}

		public ExposedList<PathConstraintData> PathConstraints
		{
			get
			{
				return pathConstraints;
			}
			set
			{
				pathConstraints = value;
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

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public string Hash
		{
			get
			{
				return hash;
			}
			set
			{
				hash = value;
			}
		}

		public string ImagesPath
		{
			get
			{
				return imagesPath;
			}
			set
			{
				imagesPath = value;
			}
		}

		public string AudioPath
		{
			get
			{
				return audioPath;
			}
			set
			{
				audioPath = value;
			}
		}

		public float Fps
		{
			get
			{
				return fps;
			}
			set
			{
				fps = value;
			}
		}

		public BoneData FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<BoneData> bones = this.bones;
			BoneData[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				BoneData bone = bonesItems[i];
				if (bone.name == boneName)
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
			ExposedList<BoneData> bones = this.bones;
			BoneData[] bonesItems = bones.Items;
			int i = 0;
			for (int j = bones.Count; i < j; i++)
			{
				if (bonesItems[i].name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public SlotData FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<SlotData> slots = this.slots;
			int i = 0;
			for (int j = slots.Count; i < j; i++)
			{
				SlotData slot = slots.Items[i];
				if (slot.name == slotName)
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
			ExposedList<SlotData> slots = this.slots;
			int i = 0;
			for (int j = slots.Count; i < j; i++)
			{
				if (slots.Items[i].name == slotName)
				{
					return i;
				}
			}
			return -1;
		}

		public Skin FindSkin(string skinName)
		{
			if (skinName == null)
			{
				throw new ArgumentNullException("skinName", "skinName cannot be null.");
			}
			foreach (Skin skin in skins)
			{
				if (skin.name == skinName)
				{
					return skin;
				}
			}
			return null;
		}

		public EventData FindEvent(string eventDataName)
		{
			if (eventDataName == null)
			{
				throw new ArgumentNullException("eventDataName", "eventDataName cannot be null.");
			}
			foreach (EventData eventData in events)
			{
				if (eventData.name == eventDataName)
				{
					return eventData;
				}
			}
			return null;
		}

		public Animation FindAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName", "animationName cannot be null.");
			}
			ExposedList<Animation> animations = this.animations;
			int i = 0;
			for (int j = animations.Count; i < j; i++)
			{
				Animation animation = animations.Items[i];
				if (animation.name == animationName)
				{
					return animation;
				}
			}
			return null;
		}

		public IkConstraintData FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<IkConstraintData> ikConstraints = this.ikConstraints;
			int i = 0;
			for (int j = ikConstraints.Count; i < j; i++)
			{
				IkConstraintData ikConstraint = ikConstraints.Items[i];
				if (ikConstraint.name == constraintName)
				{
					return ikConstraint;
				}
			}
			return null;
		}

		public TransformConstraintData FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<TransformConstraintData> transformConstraints = this.transformConstraints;
			int i = 0;
			for (int j = transformConstraints.Count; i < j; i++)
			{
				TransformConstraintData transformConstraint = transformConstraints.Items[i];
				if (transformConstraint.name == constraintName)
				{
					return transformConstraint;
				}
			}
			return null;
		}

		public PathConstraintData FindPathConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<PathConstraintData> pathConstraints = this.pathConstraints;
			int i = 0;
			for (int j = pathConstraints.Count; i < j; i++)
			{
				PathConstraintData constraint = pathConstraints.Items[i];
				if (constraint.name.Equals(constraintName))
				{
					return constraint;
				}
			}
			return null;
		}

		public int FindPathConstraintIndex(string pathConstraintName)
		{
			if (pathConstraintName == null)
			{
				throw new ArgumentNullException("pathConstraintName", "pathConstraintName cannot be null.");
			}
			ExposedList<PathConstraintData> pathConstraints = this.pathConstraints;
			int i = 0;
			for (int j = pathConstraints.Count; i < j; i++)
			{
				if (pathConstraints.Items[i].name.Equals(pathConstraintName))
				{
					return i;
				}
			}
			return -1;
		}

		public override string ToString()
		{
			return name ?? base.ToString();
		}
	}
}

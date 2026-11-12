using System;

namespace Spine
{
	public static class SpineSkeletonExtensions
	{
		public static bool IsWeighted(this VertexAttachment va)
		{
			return va.bones != null && va.bones.Length != 0;
		}

		public static bool IsRenderable(this Attachment a)
		{
			return a is IHasRendererObject;
		}

		public static bool InheritsRotation(this TransformMode mode)
		{
			return ((ulong)mode & 1uL) == 0;
		}

		public static bool InheritsScale(this TransformMode mode)
		{
			return ((ulong)mode & 2uL) == 0;
		}

		internal static void SetPropertyToSetupPose(this Skeleton skeleton, int propertyID)
		{
			int tt = propertyID >> 24;
			TimelineType timelineType = (TimelineType)tt;
			int i = propertyID - (tt << 24);
			switch (timelineType)
			{
			case TimelineType.Rotate:
			{
				Bone bone = skeleton.bones.Items[i];
				bone.rotation = bone.data.rotation;
				break;
			}
			case TimelineType.Translate:
			{
				Bone bone = skeleton.bones.Items[i];
				bone.x = bone.data.x;
				bone.y = bone.data.y;
				break;
			}
			case TimelineType.Scale:
			{
				Bone bone = skeleton.bones.Items[i];
				bone.scaleX = bone.data.scaleX;
				bone.scaleY = bone.data.scaleY;
				break;
			}
			case TimelineType.Shear:
			{
				Bone bone = skeleton.bones.Items[i];
				bone.shearX = bone.data.shearX;
				bone.shearY = bone.data.shearY;
				break;
			}
			case TimelineType.Attachment:
				skeleton.SetSlotAttachmentToSetupPose(i);
				break;
			case TimelineType.Color:
				skeleton.slots.Items[i].SetColorToSetupPose();
				break;
			case TimelineType.TwoColor:
				skeleton.slots.Items[i].SetColorToSetupPose();
				break;
			case TimelineType.Deform:
				skeleton.slots.Items[i].Deform.Clear();
				break;
			case TimelineType.DrawOrder:
				skeleton.SetDrawOrderToSetupPose();
				break;
			case TimelineType.IkConstraint:
			{
				IkConstraint ikc = skeleton.ikConstraints.Items[i];
				ikc.mix = ikc.data.mix;
				ikc.softness = ikc.data.softness;
				ikc.bendDirection = ikc.data.bendDirection;
				ikc.stretch = ikc.data.stretch;
				break;
			}
			case TimelineType.TransformConstraint:
			{
				TransformConstraint tc = skeleton.transformConstraints.Items[i];
				TransformConstraintData tcData = tc.data;
				tc.rotateMix = tcData.rotateMix;
				tc.translateMix = tcData.translateMix;
				tc.scaleMix = tcData.scaleMix;
				tc.shearMix = tcData.shearMix;
				break;
			}
			case TimelineType.PathConstraintPosition:
			{
				PathConstraint pc = skeleton.pathConstraints.Items[i];
				pc.position = pc.data.position;
				break;
			}
			case TimelineType.PathConstraintSpacing:
			{
				PathConstraint pc = skeleton.pathConstraints.Items[i];
				pc.spacing = pc.data.spacing;
				break;
			}
			case TimelineType.PathConstraintMix:
			{
				PathConstraint pc = skeleton.pathConstraints.Items[i];
				pc.rotateMix = pc.data.rotateMix;
				pc.translateMix = pc.data.translateMix;
				break;
			}
			case TimelineType.Event:
				break;
			}
		}

		public static void SetDrawOrderToSetupPose(this Skeleton skeleton)
		{
			Slot[] slotsItems = skeleton.slots.Items;
			int i = skeleton.slots.Count;
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			drawOrder.Clear(false);
			drawOrder.EnsureCapacity(i);
			drawOrder.Count = i;
			Array.Copy(slotsItems, drawOrder.Items, i);
		}

		public static void SetSlotAttachmentsToSetupPose(this Skeleton skeleton)
		{
			Slot[] slotsItems = skeleton.slots.Items;
			for (int i = 0; i < skeleton.slots.Count; i++)
			{
				Slot slot = slotsItems[i];
				string attachmentName = slot.data.attachmentName;
				slot.Attachment = (string.IsNullOrEmpty(attachmentName) ? null : skeleton.GetAttachment(i, attachmentName));
			}
		}

		public static void SetColorToSetupPose(this Slot slot)
		{
			slot.r = slot.data.r;
			slot.g = slot.data.g;
			slot.b = slot.data.b;
			slot.a = slot.data.a;
			slot.r2 = slot.data.r2;
			slot.g2 = slot.data.g2;
			slot.b2 = slot.data.b2;
		}

		public static void SetAttachmentToSetupPose(this Slot slot)
		{
			SlotData slotData = slot.data;
			slot.Attachment = slot.bone.skeleton.GetAttachment(slotData.name, slotData.attachmentName);
		}

		public static void SetSlotAttachmentToSetupPose(this Skeleton skeleton, int slotIndex)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			string attachmentName = slot.data.attachmentName;
			if (string.IsNullOrEmpty(attachmentName))
			{
				slot.Attachment = null;
			}
			else
			{
				Attachment attachment = (slot.Attachment = skeleton.GetAttachment(slotIndex, attachmentName));
			}
		}

		public static void SetKeyedItemsToSetupPose(this Animation animation, Skeleton skeleton)
		{
			animation.Apply(skeleton, 0f, 0f, false, null, 0f, MixBlend.Setup, MixDirection.Out);
		}

		public static void AllowImmediateQueue(this TrackEntry trackEntry)
		{
			if (trackEntry.nextTrackLast < 0f)
			{
				trackEntry.nextTrackLast = 0f;
			}
		}
	}
}

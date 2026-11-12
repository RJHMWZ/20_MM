using System;
using System.Collections.Generic;
using Spine.Collections;

namespace Spine.Unity.AttachmentTools
{
	public static class SkinUtilities
	{
		public static Skin UnshareSkin(this Skeleton skeleton, bool includeDefaultSkin, bool unshareAttachments, AnimationState state = null)
		{
			Skin newSkin = skeleton.GetClonedSkin("cloned skin", includeDefaultSkin, unshareAttachments);
			skeleton.SetSkin(newSkin);
			if (state != null)
			{
				skeleton.SetToSetupPose();
				state.Apply(skeleton);
			}
			return newSkin;
		}

		public static Skin GetClonedSkin(this Skeleton skeleton, string newSkinName, bool includeDefaultSkin = false, bool cloneAttachments = false, bool cloneMeshesAsLinked = true)
		{
			Skin newSkin = new Skin(newSkinName);
			Skin defaultSkin = skeleton.data.DefaultSkin;
			Skin activeSkin = skeleton.skin;
			if (includeDefaultSkin)
			{
				defaultSkin.CopyTo(newSkin, true, cloneAttachments, cloneMeshesAsLinked);
			}
			activeSkin?.CopyTo(newSkin, true, cloneAttachments, cloneMeshesAsLinked);
			return newSkin;
		}

		public static Skin GetClone(this Skin original)
		{
			Skin newSkin = new Skin(original.name + " clone");
			OrderedDictionary<Skin.SkinEntry, Attachment> newSkinAttachments = newSkin.Attachments;
			ExposedList<BoneData> newSkinBones = newSkin.Bones;
			ExposedList<ConstraintData> newSkinConstraints = newSkin.Constraints;
			foreach (KeyValuePair<Skin.SkinEntry, Attachment> a in original.Attachments)
			{
				newSkinAttachments[a.Key] = a.Value;
			}
			newSkinBones.AddRange(original.bones);
			newSkinConstraints.AddRange(original.constraints);
			return newSkin;
		}

		public static void SetAttachment(this Skin skin, string slotName, string keyName, Attachment attachment, Skeleton skeleton)
		{
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (slotIndex == -1)
			{
				throw new ArgumentException($"Slot '{slotName}' does not exist in skeleton.", "slotName");
			}
			skin.SetAttachment(slotIndex, keyName, attachment);
		}

		public static void AddAttachments(this Skin skin, Skin otherSkin)
		{
			otherSkin?.CopyTo(skin, true, false);
		}

		public static Attachment GetAttachment(this Skin skin, string slotName, string keyName, Skeleton skeleton)
		{
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (slotIndex == -1)
			{
				throw new ArgumentException($"Slot '{slotName}' does not exist in skeleton.", "slotName");
			}
			return skin.GetAttachment(slotIndex, keyName);
		}

		public static void SetAttachment(this Skin skin, int slotIndex, string keyName, Attachment attachment)
		{
			skin.SetAttachment(slotIndex, keyName, attachment);
		}

		public static void RemoveAttachment(this Skin skin, string slotName, string keyName, SkeletonData skeletonData)
		{
			int slotIndex = skeletonData.FindSlotIndex(slotName);
			if (skeletonData == null)
			{
				throw new ArgumentNullException("skeletonData", "skeletonData cannot be null.");
			}
			if (slotIndex == -1)
			{
				throw new ArgumentException($"Slot '{slotName}' does not exist in skeleton.", "slotName");
			}
			skin.RemoveAttachment(slotIndex, keyName);
		}

		public static void Clear(this Skin skin)
		{
			skin.Attachments.Clear();
		}

		public static void Append(this Skin destination, Skin source)
		{
			source.CopyTo(destination, true, false);
		}

		public static void CopyTo(this Skin source, Skin destination, bool overwrite, bool cloneAttachments, bool cloneMeshesAsLinked = true)
		{
			OrderedDictionary<Skin.SkinEntry, Attachment> sourceAttachments = source.Attachments;
			OrderedDictionary<Skin.SkinEntry, Attachment> destinationAttachments = destination.Attachments;
			ExposedList<BoneData> destinationBones = destination.Bones;
			ExposedList<ConstraintData> destinationConstraints = destination.Constraints;
			if (cloneAttachments)
			{
				if (overwrite)
				{
					foreach (KeyValuePair<Skin.SkinEntry, Attachment> e4 in sourceAttachments)
					{
						Attachment clonedAttachment2 = e4.Value.GetCopy(cloneMeshesAsLinked);
						destinationAttachments[new Skin.SkinEntry(e4.Key.SlotIndex, e4.Key.Name, clonedAttachment2)] = clonedAttachment2;
					}
				}
				else
				{
					foreach (KeyValuePair<Skin.SkinEntry, Attachment> e3 in sourceAttachments)
					{
						if (!destinationAttachments.ContainsKey(e3.Key))
						{
							Attachment clonedAttachment = e3.Value.GetCopy(cloneMeshesAsLinked);
							destinationAttachments.Add(new Skin.SkinEntry(e3.Key.SlotIndex, e3.Key.Name, clonedAttachment), clonedAttachment);
						}
					}
				}
			}
			else if (overwrite)
			{
				foreach (KeyValuePair<Skin.SkinEntry, Attachment> e2 in sourceAttachments)
				{
					destinationAttachments[e2.Key] = e2.Value;
				}
			}
			else
			{
				foreach (KeyValuePair<Skin.SkinEntry, Attachment> e in sourceAttachments)
				{
					if (!destinationAttachments.ContainsKey(e.Key))
					{
						destinationAttachments.Add(e.Key, e.Value);
					}
				}
			}
			foreach (BoneData data2 in source.bones)
			{
				if (!destinationBones.Contains(data2))
				{
					destinationBones.Add(data2);
				}
			}
			foreach (ConstraintData data in source.constraints)
			{
				if (!destinationConstraints.Contains(data))
				{
					destinationConstraints.Add(data);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Spine.Collections;

namespace Spine
{
	public class Skin
	{
		public struct SkinEntry
		{
			private readonly int slotIndex;

			private readonly string name;

			private readonly Attachment attachment;

			internal readonly int hashCode;

			public int SlotIndex => slotIndex;

			public string Name => name;

			public Attachment Attachment => attachment;

			public SkinEntry(int slotIndex, string name, Attachment attachment)
			{
				this.slotIndex = slotIndex;
				this.name = name;
				this.attachment = attachment;
				hashCode = this.name.GetHashCode() + this.slotIndex * 37;
			}
		}

		private class SkinEntryComparer : IEqualityComparer<SkinEntry>
		{
			internal static readonly SkinEntryComparer Instance = new SkinEntryComparer();

			bool IEqualityComparer<SkinEntry>.Equals(SkinEntry e1, SkinEntry e2)
			{
				if (e1.SlotIndex != e2.SlotIndex)
				{
					return false;
				}
				if (!string.Equals(e1.Name, e2.Name, StringComparison.Ordinal))
				{
					return false;
				}
				return true;
			}

			int IEqualityComparer<SkinEntry>.GetHashCode(SkinEntry e)
			{
				return e.Name.GetHashCode() + e.SlotIndex * 37;
			}
		}

		internal string name;

		private OrderedDictionary<SkinEntry, Attachment> attachments = new OrderedDictionary<SkinEntry, Attachment>(SkinEntryComparer.Instance);

		internal readonly ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal readonly ExposedList<ConstraintData> constraints = new ExposedList<ConstraintData>();

		public string Name => name;

		public OrderedDictionary<SkinEntry, Attachment> Attachments => attachments;

		public ExposedList<BoneData> Bones => bones;

		public ExposedList<ConstraintData> Constraints => constraints;

		public Skin(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			this.name = name;
		}

		public void SetAttachment(int slotIndex, string name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment", "attachment cannot be null.");
			}
			if (slotIndex < 0)
			{
				throw new ArgumentNullException("slotIndex", "slotIndex must be >= 0.");
			}
			attachments[new SkinEntry(slotIndex, name, attachment)] = attachment;
		}

		public void AddSkin(Skin skin)
		{
			foreach (BoneData data2 in skin.bones)
			{
				if (!bones.Contains(data2))
				{
					bones.Add(data2);
				}
			}
			foreach (ConstraintData data in skin.constraints)
			{
				if (!constraints.Contains(data))
				{
					constraints.Add(data);
				}
			}
			foreach (SkinEntry entry in skin.attachments.Keys)
			{
				SetAttachment(entry.SlotIndex, entry.Name, entry.Attachment);
			}
		}

		public void CopySkin(Skin skin)
		{
			foreach (BoneData data in skin.bones)
			{
				if (!bones.Contains(data))
				{
					bones.Add(data);
				}
			}
			foreach (ConstraintData data2 in skin.constraints)
			{
				if (!constraints.Contains(data2))
				{
					constraints.Add(data2);
				}
			}
			foreach (SkinEntry entry in skin.attachments.Keys)
			{
				if (entry.Attachment is MeshAttachment)
				{
					SetAttachment(entry.SlotIndex, entry.Name, (entry.Attachment != null) ? ((MeshAttachment)entry.Attachment).NewLinkedMesh() : null);
				}
				else
				{
					SetAttachment(entry.SlotIndex, entry.Name, (entry.Attachment != null) ? entry.Attachment.Copy() : null);
				}
			}
		}

		public Attachment GetAttachment(int slotIndex, string name)
		{
			SkinEntry lookup = new SkinEntry(slotIndex, name, null);
			Attachment attachment = null;
			return attachments.TryGetValue(lookup, out attachment) ? attachment : null;
		}

		public void RemoveAttachment(int slotIndex, string name)
		{
			if (slotIndex < 0)
			{
				throw new ArgumentOutOfRangeException("slotIndex", "slotIndex must be >= 0");
			}
			SkinEntry lookup = new SkinEntry(slotIndex, name, null);
			attachments.Remove(lookup);
		}

		public ICollection<SkinEntry> GetAttachments()
		{
			return attachments.Keys;
		}

		public void GetAttachments(int slotIndex, List<SkinEntry> attachments)
		{
			foreach (SkinEntry entry in this.attachments.Keys)
			{
				if (entry.SlotIndex == slotIndex)
				{
					attachments.Add(entry);
				}
			}
		}

		public void Clear()
		{
			attachments.Clear();
			bones.Clear();
			constraints.Clear();
		}

		public override string ToString()
		{
			return name;
		}

		internal void AttachAll(Skeleton skeleton, Skin oldSkin)
		{
			foreach (SkinEntry entry in oldSkin.attachments.Keys)
			{
				int slotIndex = entry.SlotIndex;
				Slot slot = skeleton.slots.Items[slotIndex];
				if (slot.Attachment == entry.Attachment)
				{
					Attachment attachment = GetAttachment(slotIndex, entry.Name);
					if (attachment != null)
					{
						slot.Attachment = attachment;
					}
				}
			}
		}
	}
}

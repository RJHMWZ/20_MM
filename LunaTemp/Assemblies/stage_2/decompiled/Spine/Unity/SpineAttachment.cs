using System;

namespace Spine.Unity
{
	public class SpineAttachment : SpineAttributeBase
	{
		public struct Hierarchy
		{
			public string skin;

			public string slot;

			public string name;

			public Hierarchy(string fullPath)
			{
				string[] chunks = fullPath.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				if (chunks.Length == 0)
				{
					skin = "";
					slot = "";
					name = "";
					return;
				}
				if (chunks.Length < 2)
				{
					throw new Exception("Cannot generate Attachment Hierarchy from string! Not enough components! [" + fullPath + "]");
				}
				skin = chunks[0];
				slot = chunks[1];
				name = "";
				for (int i = 2; i < chunks.Length; i++)
				{
					name += chunks[i];
				}
			}
		}

		public bool returnAttachmentPath = false;

		public bool currentSkinOnly = false;

		public bool placeholdersOnly = false;

		public string skinField = "";

		public string slotField = "";

		public SpineAttachment(bool currentSkinOnly = true, bool returnAttachmentPath = false, bool placeholdersOnly = false, string slotField = "", string dataField = "", string skinField = "", bool includeNone = true, bool fallbackToTextField = false)
		{
			this.currentSkinOnly = currentSkinOnly;
			this.returnAttachmentPath = returnAttachmentPath;
			this.placeholdersOnly = placeholdersOnly;
			this.slotField = slotField;
			base.dataField = dataField;
			this.skinField = skinField;
			base.includeNone = includeNone;
			base.fallbackToTextField = fallbackToTextField;
		}

		public static Hierarchy GetHierarchy(string fullPath)
		{
			return new Hierarchy(fullPath);
		}

		public static Attachment GetAttachment(string attachmentPath, SkeletonData skeletonData)
		{
			Hierarchy hierarchy = GetHierarchy(attachmentPath);
			return string.IsNullOrEmpty(hierarchy.name) ? null : skeletonData.FindSkin(hierarchy.skin).GetAttachment(skeletonData.FindSlotIndex(hierarchy.slot), hierarchy.name);
		}

		public static Attachment GetAttachment(string attachmentPath, SkeletonDataAsset skeletonDataAsset)
		{
			return GetAttachment(attachmentPath, skeletonDataAsset.GetSkeletonData(true));
		}
	}
}

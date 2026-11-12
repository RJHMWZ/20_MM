using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[Serializable]
	public class BlendModeMaterials
	{
		[Serializable]
		public class ReplacementMaterial
		{
			public string pageName;

			public Material material;
		}

		[SerializeField]
		[HideInInspector]
		protected bool requiresBlendModeMaterials = false;

		public bool applyAdditiveMaterial = false;

		public List<ReplacementMaterial> additiveMaterials = new List<ReplacementMaterial>();

		public List<ReplacementMaterial> multiplyMaterials = new List<ReplacementMaterial>();

		public List<ReplacementMaterial> screenMaterials = new List<ReplacementMaterial>();

		public bool RequiresBlendModeMaterials
		{
			get
			{
				return requiresBlendModeMaterials;
			}
			set
			{
				requiresBlendModeMaterials = value;
			}
		}

		public void ApplyMaterials(SkeletonData skeletonData)
		{
			if (skeletonData == null)
			{
				throw new ArgumentNullException("skeletonData");
			}
			if (!requiresBlendModeMaterials)
			{
				return;
			}
			List<Skin.SkinEntry> skinEntries = new List<Skin.SkinEntry>();
			SlotData[] slotsItems = skeletonData.Slots.Items;
			int slotIndex = 0;
			for (int slotCount = skeletonData.Slots.Count; slotIndex < slotCount; slotIndex++)
			{
				SlotData slot = slotsItems[slotIndex];
				if (slot.blendMode == BlendMode.Normal || (!applyAdditiveMaterial && slot.blendMode == BlendMode.Additive))
				{
					continue;
				}
				List<ReplacementMaterial> replacementMaterials = null;
				switch (slot.blendMode)
				{
				case BlendMode.Multiply:
					replacementMaterials = multiplyMaterials;
					break;
				case BlendMode.Screen:
					replacementMaterials = screenMaterials;
					break;
				case BlendMode.Additive:
					replacementMaterials = additiveMaterials;
					break;
				}
				if (replacementMaterials == null)
				{
					continue;
				}
				skinEntries.Clear();
				foreach (Skin skin in skeletonData.Skins)
				{
					skin.GetAttachments(slotIndex, skinEntries);
				}
				foreach (Skin.SkinEntry item in skinEntries)
				{
					if (item.Attachment is IHasRendererObject renderableAttachment)
					{
						renderableAttachment.RendererObject = CloneAtlasRegionWithMaterial((AtlasRegion)renderableAttachment.RendererObject, replacementMaterials);
					}
				}
			}
		}

		protected AtlasRegion CloneAtlasRegionWithMaterial(AtlasRegion originalRegion, List<ReplacementMaterial> replacementMaterials)
		{
			AtlasRegion newRegion = originalRegion.Clone();
			Material material = null;
			foreach (ReplacementMaterial replacement in replacementMaterials)
			{
				if (replacement.pageName == originalRegion.page.name)
				{
					material = replacement.material;
					break;
				}
			}
			AtlasPage originalPage = originalRegion.page;
			AtlasPage newPage = originalPage.Clone();
			newPage.rendererObject = material;
			newRegion.page = newPage;
			return newRegion;
		}
	}
}

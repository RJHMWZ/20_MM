using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[CreateAssetMenu(menuName = "Spine/SkeletonData Modifiers/Blend Mode Materials", order = 200)]
	public class BlendModeMaterialsAsset : SkeletonDataModifierAsset
	{
		private class AtlasMaterialCache : IDisposable
		{
			private readonly Dictionary<KeyValuePair<AtlasPage, Material>, AtlasPage> cache = new Dictionary<KeyValuePair<AtlasPage, Material>, AtlasPage>();

			public AtlasRegion CloneAtlasRegionWithMaterial(AtlasRegion originalRegion, Material materialTemplate)
			{
				AtlasRegion newRegion = originalRegion.Clone();
				newRegion.page = GetAtlasPageWithMaterial(originalRegion.page, materialTemplate);
				return newRegion;
			}

			private AtlasPage GetAtlasPageWithMaterial(AtlasPage originalPage, Material materialTemplate)
			{
				if (originalPage == null)
				{
					throw new ArgumentNullException("originalPage");
				}
				AtlasPage newPage = null;
				KeyValuePair<AtlasPage, Material> key = new KeyValuePair<AtlasPage, Material>(originalPage, materialTemplate);
				cache.TryGetValue(key, out newPage);
				if (newPage == null)
				{
					newPage = originalPage.Clone();
					Material originalMaterial = originalPage.rendererObject as Material;
					newPage.rendererObject = new Material(materialTemplate)
					{
						name = originalMaterial.name + " " + materialTemplate.name,
						mainTexture = originalMaterial.mainTexture
					};
					cache.Add(key, newPage);
				}
				return newPage;
			}

			public void Dispose()
			{
				cache.Clear();
			}
		}

		public Material multiplyMaterialTemplate;

		public Material screenMaterialTemplate;

		public Material additiveMaterialTemplate;

		public bool applyAdditiveMaterial = true;

		public override void Apply(SkeletonData skeletonData)
		{
			ApplyMaterials(skeletonData, multiplyMaterialTemplate, screenMaterialTemplate, additiveMaterialTemplate, applyAdditiveMaterial);
		}

		public static void ApplyMaterials(SkeletonData skeletonData, Material multiplyTemplate, Material screenTemplate, Material additiveTemplate, bool includeAdditiveSlots)
		{
			if (skeletonData == null)
			{
				throw new ArgumentNullException("skeletonData");
			}
			using (AtlasMaterialCache materialCache = new AtlasMaterialCache())
			{
				List<Skin.SkinEntry> entryBuffer = new List<Skin.SkinEntry>();
				SlotData[] slotsItems = skeletonData.Slots.Items;
				int slotIndex = 0;
				for (int slotCount = skeletonData.Slots.Count; slotIndex < slotCount; slotIndex++)
				{
					SlotData slot = slotsItems[slotIndex];
					if (slot.blendMode == BlendMode.Normal || (!includeAdditiveSlots && slot.blendMode == BlendMode.Additive))
					{
						continue;
					}
					entryBuffer.Clear();
					foreach (Skin skin in skeletonData.Skins)
					{
						skin.GetAttachments(slotIndex, entryBuffer);
					}
					Material templateMaterial = null;
					switch (slot.blendMode)
					{
					case BlendMode.Multiply:
						templateMaterial = multiplyTemplate;
						break;
					case BlendMode.Screen:
						templateMaterial = screenTemplate;
						break;
					case BlendMode.Additive:
						templateMaterial = additiveTemplate;
						break;
					}
					if (templateMaterial == null)
					{
						continue;
					}
					foreach (Skin.SkinEntry item in entryBuffer)
					{
						if (item.Attachment is IHasRendererObject renderableAttachment)
						{
							renderableAttachment.RendererObject = materialCache.CloneAtlasRegionWithMaterial((AtlasRegion)renderableAttachment.RendererObject, templateMaterial);
						}
					}
				}
			}
		}
	}
}

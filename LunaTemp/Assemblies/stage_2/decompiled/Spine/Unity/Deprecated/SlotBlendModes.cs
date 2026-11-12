using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Deprecated
{
	[Obsolete("The spine-unity 3.7 runtime introduced SkeletonDataModifierAssets BlendModeMaterials which replaced SlotBlendModes. Will be removed in spine-unity 3.9.", false)]
	[DisallowMultipleComponent]
	public class SlotBlendModes : MonoBehaviour
	{
		public struct MaterialTexturePair
		{
			public Texture2D texture2D;

			public Material material;
		}

		internal class MaterialWithRefcount
		{
			public Material materialClone;

			public int refcount = 1;

			public MaterialWithRefcount(Material mat)
			{
				materialClone = mat;
			}
		}

		internal struct SlotMaterialTextureTuple
		{
			public Slot slot;

			public Texture2D texture2D;

			public Material material;

			public SlotMaterialTextureTuple(Slot slot, Material material, Texture2D texture)
			{
				this.slot = slot;
				this.material = material;
				texture2D = texture;
			}
		}

		private static Dictionary<MaterialTexturePair, MaterialWithRefcount> materialTable;

		public Material multiplyMaterialSource;

		public Material screenMaterialSource;

		private Texture2D texture;

		private SlotMaterialTextureTuple[] slotsWithCustomMaterial = new SlotMaterialTextureTuple[0];

		internal static Dictionary<MaterialTexturePair, MaterialWithRefcount> MaterialTable
		{
			get
			{
				if (materialTable == null)
				{
					materialTable = new Dictionary<MaterialTexturePair, MaterialWithRefcount>();
				}
				return materialTable;
			}
		}

		public bool Applied { get; private set; }

		internal static Material GetOrAddMaterialFor(Material materialSource, Texture2D texture)
		{
			if (materialSource == null || texture == null)
			{
				return null;
			}
			Dictionary<MaterialTexturePair, MaterialWithRefcount> mt = MaterialTable;
			MaterialTexturePair materialTexturePair = default(MaterialTexturePair);
			materialTexturePair.material = materialSource;
			materialTexturePair.texture2D = texture;
			MaterialTexturePair key = materialTexturePair;
			if (!mt.TryGetValue(key, out var matWithRefcount))
			{
				matWithRefcount = new MaterialWithRefcount(new Material(materialSource));
				Material i = matWithRefcount.materialClone;
				i.name = "(Clone)" + texture.name + "-" + materialSource.name;
				i.mainTexture = texture;
				mt[key] = matWithRefcount;
			}
			else
			{
				matWithRefcount.refcount++;
			}
			return matWithRefcount.materialClone;
		}

		internal static MaterialWithRefcount GetExistingMaterialFor(Material materialSource, Texture2D texture)
		{
			if (materialSource == null || texture == null)
			{
				return null;
			}
			Dictionary<MaterialTexturePair, MaterialWithRefcount> mt = MaterialTable;
			MaterialTexturePair materialTexturePair = default(MaterialTexturePair);
			materialTexturePair.material = materialSource;
			materialTexturePair.texture2D = texture;
			MaterialTexturePair key = materialTexturePair;
			if (!mt.TryGetValue(key, out var matWithRefcount))
			{
				return null;
			}
			return matWithRefcount;
		}

		internal static void RemoveMaterialFromTable(Material materialSource, Texture2D texture)
		{
			Dictionary<MaterialTexturePair, MaterialWithRefcount> mt = MaterialTable;
			MaterialTexturePair materialTexturePair = default(MaterialTexturePair);
			materialTexturePair.material = materialSource;
			materialTexturePair.texture2D = texture;
			MaterialTexturePair key = materialTexturePair;
			mt.Remove(key);
		}

		private void Start()
		{
			if (!Applied)
			{
				Apply();
			}
		}

		private void OnDestroy()
		{
			if (Applied)
			{
				Remove();
			}
		}

		public void Apply()
		{
			GetTexture();
			if (texture == null)
			{
				return;
			}
			SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
			if (skeletonRenderer == null)
			{
				return;
			}
			Dictionary<Slot, Material> slotMaterials = skeletonRenderer.CustomSlotMaterials;
			int numSlotsWithCustomMaterial = 0;
			foreach (Slot s2 in skeletonRenderer.Skeleton.Slots)
			{
				switch (s2.data.blendMode)
				{
				case BlendMode.Multiply:
					if (multiplyMaterialSource != null)
					{
						slotMaterials[s2] = GetOrAddMaterialFor(multiplyMaterialSource, texture);
						numSlotsWithCustomMaterial++;
					}
					break;
				case BlendMode.Screen:
					if (screenMaterialSource != null)
					{
						slotMaterials[s2] = GetOrAddMaterialFor(screenMaterialSource, texture);
						numSlotsWithCustomMaterial++;
					}
					break;
				}
			}
			slotsWithCustomMaterial = new SlotMaterialTextureTuple[numSlotsWithCustomMaterial];
			int storedSlotIndex = 0;
			foreach (Slot s in skeletonRenderer.Skeleton.Slots)
			{
				switch (s.data.blendMode)
				{
				case BlendMode.Multiply:
					if (multiplyMaterialSource != null)
					{
						slotsWithCustomMaterial[storedSlotIndex++] = new SlotMaterialTextureTuple(s, multiplyMaterialSource, texture);
					}
					break;
				case BlendMode.Screen:
					if (screenMaterialSource != null)
					{
						slotsWithCustomMaterial[storedSlotIndex++] = new SlotMaterialTextureTuple(s, screenMaterialSource, texture);
					}
					break;
				}
			}
			Applied = true;
			skeletonRenderer.LateUpdate();
		}

		public void Remove()
		{
			GetTexture();
			if (texture == null)
			{
				return;
			}
			SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
			if (skeletonRenderer == null)
			{
				return;
			}
			Dictionary<Slot, Material> slotMaterials = skeletonRenderer.CustomSlotMaterials;
			SlotMaterialTextureTuple[] array = slotsWithCustomMaterial;
			for (int j = 0; j < array.Length; j++)
			{
				SlotMaterialTextureTuple slotWithCustomMat = array[j];
				Slot s = slotWithCustomMat.slot;
				Material storedMaterialSource = slotWithCustomMat.material;
				Texture2D storedTexture = slotWithCustomMat.texture2D;
				MaterialWithRefcount matWithRefcount = GetExistingMaterialFor(storedMaterialSource, storedTexture);
				if (--matWithRefcount.refcount == 0)
				{
					RemoveMaterialFromTable(storedMaterialSource, storedTexture);
				}
				if (slotMaterials.TryGetValue(s, out var i))
				{
					Material existingMat = matWithRefcount?.materialClone;
					if ((object)i == existingMat)
					{
						slotMaterials.Remove(s);
					}
				}
			}
			slotsWithCustomMaterial = null;
			Applied = false;
			if (skeletonRenderer.valid)
			{
				skeletonRenderer.LateUpdate();
			}
		}

		public void GetTexture()
		{
			if (!(texture == null))
			{
				return;
			}
			SkeletonRenderer sr = GetComponent<SkeletonRenderer>();
			if (sr == null)
			{
				return;
			}
			SkeletonDataAsset sda = sr.skeletonDataAsset;
			if (sda == null)
			{
				return;
			}
			AtlasAssetBase aa = sda.atlasAssets[0];
			if (!(aa == null))
			{
				Material am = aa.PrimaryMaterial;
				if (!(am == null))
				{
					texture = am.mainTexture as Texture2D;
				}
			}
		}
	}
}

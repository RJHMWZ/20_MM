using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonGraphicCustomMaterials")]
	public class SkeletonGraphicCustomMaterials : MonoBehaviour
	{
		[Serializable]
		public struct AtlasMaterialOverride : IEquatable<AtlasMaterialOverride>
		{
			public bool overrideEnabled;

			public Texture originalTexture;

			public Material replacementMaterial;

			public bool Equals(AtlasMaterialOverride other)
			{
				return overrideEnabled == other.overrideEnabled && originalTexture == other.originalTexture && replacementMaterial == other.replacementMaterial;
			}
		}

		[Serializable]
		public struct AtlasTextureOverride : IEquatable<AtlasTextureOverride>
		{
			public bool overrideEnabled;

			public Texture originalTexture;

			public Texture replacementTexture;

			public bool Equals(AtlasTextureOverride other)
			{
				return overrideEnabled == other.overrideEnabled && originalTexture == other.originalTexture && replacementTexture == other.replacementTexture;
			}
		}

		public SkeletonGraphic skeletonGraphic;

		[SerializeField]
		protected List<AtlasMaterialOverride> customMaterialOverrides = new List<AtlasMaterialOverride>();

		[SerializeField]
		protected List<AtlasTextureOverride> customTextureOverrides = new List<AtlasTextureOverride>();

		private void SetCustomMaterialOverrides()
		{
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (atlasMaterialOverride.overrideEnabled)
				{
					skeletonGraphic.CustomMaterialOverride[atlasMaterialOverride.originalTexture] = atlasMaterialOverride.replacementMaterial;
				}
			}
		}

		private void RemoveCustomMaterialOverrides()
		{
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (skeletonGraphic.CustomMaterialOverride.TryGetValue(atlasMaterialOverride.originalTexture, out var currentMaterial) && !(currentMaterial != atlasMaterialOverride.replacementMaterial))
				{
					skeletonGraphic.CustomMaterialOverride.Remove(atlasMaterialOverride.originalTexture);
				}
			}
		}

		private void SetCustomTextureOverrides()
		{
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			for (int i = 0; i < customTextureOverrides.Count; i++)
			{
				AtlasTextureOverride atlasTextureOverride = customTextureOverrides[i];
				if (atlasTextureOverride.overrideEnabled)
				{
					skeletonGraphic.CustomTextureOverride[atlasTextureOverride.originalTexture] = atlasTextureOverride.replacementTexture;
				}
			}
		}

		private void RemoveCustomTextureOverrides()
		{
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			for (int i = 0; i < customTextureOverrides.Count; i++)
			{
				AtlasTextureOverride atlasTextureOverride = customTextureOverrides[i];
				if (skeletonGraphic.CustomTextureOverride.TryGetValue(atlasTextureOverride.originalTexture, out var currentTexture) && !(currentTexture != atlasTextureOverride.replacementTexture))
				{
					skeletonGraphic.CustomTextureOverride.Remove(atlasTextureOverride.originalTexture);
				}
			}
		}

		private void OnEnable()
		{
			if (skeletonGraphic == null)
			{
				skeletonGraphic = GetComponent<SkeletonGraphic>();
			}
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			skeletonGraphic.Initialize(false);
			SetCustomMaterialOverrides();
			SetCustomTextureOverrides();
		}

		private void OnDisable()
		{
			if (skeletonGraphic == null)
			{
				Debug.LogError("skeletonGraphic == null");
				return;
			}
			RemoveCustomMaterialOverrides();
			RemoveCustomTextureOverrides();
		}
	}
}

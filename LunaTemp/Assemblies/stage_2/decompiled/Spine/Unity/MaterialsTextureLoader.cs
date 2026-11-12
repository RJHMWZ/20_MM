using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	public class MaterialsTextureLoader : TextureLoader
	{
		private SpineAtlasAsset atlasAsset;

		public MaterialsTextureLoader(SpineAtlasAsset atlasAsset)
		{
			this.atlasAsset = atlasAsset;
		}

		public void Load(AtlasPage page, string path)
		{
			string name = Path.GetFileNameWithoutExtension(path);
			Material material = null;
			Material[] materials = atlasAsset.materials;
			foreach (Material other in materials)
			{
				if (other.mainTexture == null)
				{
					Debug.LogError("Material is missing texture: " + other.name, other);
					return;
				}
				if (other.mainTexture.name == name)
				{
					material = other;
					break;
				}
			}
			if (material == null)
			{
				Debug.LogError("Material with texture name \"" + name + "\" not found for atlas asset: " + atlasAsset.name, atlasAsset);
				return;
			}
			page.rendererObject = material;
			if (page.width == 0 || page.height == 0)
			{
				page.width = material.mainTexture.width;
				page.height = material.mainTexture.height;
			}
		}

		public void Unload(object texture)
		{
		}
	}
}

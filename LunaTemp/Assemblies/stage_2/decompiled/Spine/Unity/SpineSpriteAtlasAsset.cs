using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Spine.Unity
{
	[CreateAssetMenu(fileName = "New Spine SpriteAtlas Asset", menuName = "Spine/Spine SpriteAtlas Asset")]
	public class SpineSpriteAtlasAsset : AtlasAssetBase
	{
		[Serializable]
		protected class SavedRegionInfo
		{
			public float x;

			public float y;

			public float width;

			public float height;

			public SpritePackingRotation packingRotation;
		}

		public SpriteAtlas spriteAtlasFile;

		public Material[] materials;

		protected Atlas atlas;

		public bool updateRegionsInPlayMode;

		[SerializeField]
		protected SavedRegionInfo[] savedRegions;

		public override bool IsLoaded => atlas != null;

		public override IEnumerable<Material> Materials => materials;

		public override int MaterialCount => (materials != null) ? materials.Length : 0;

		public override Material PrimaryMaterial => materials[0];

		public static SpineSpriteAtlasAsset CreateRuntimeInstance(SpriteAtlas spriteAtlasFile, Material[] materials, bool initialize)
		{
			SpineSpriteAtlasAsset atlasAsset = ScriptableObject.CreateInstance<SpineSpriteAtlasAsset>();
			atlasAsset.Reset();
			atlasAsset.spriteAtlasFile = spriteAtlasFile;
			atlasAsset.materials = materials;
			if (initialize)
			{
				atlasAsset.GetAtlas();
			}
			return atlasAsset;
		}

		private void Reset()
		{
			Clear();
		}

		public override void Clear()
		{
			atlas = null;
		}

		public override Atlas GetAtlas()
		{
			if (spriteAtlasFile == null)
			{
				Debug.LogError("SpriteAtlas file not set for SpineSpriteAtlasAsset: " + base.name, this);
				Clear();
				return null;
			}
			if (materials == null || materials.Length == 0)
			{
				Debug.LogError("Materials not set for SpineSpriteAtlasAsset: " + base.name, this);
				Clear();
				return null;
			}
			if (atlas != null)
			{
				return atlas;
			}
			try
			{
				atlas = LoadAtlas(spriteAtlasFile);
				return atlas;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error analyzing SpriteAtlas for SpineSpriteAtlasAsset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
				return null;
			}
		}

		protected void AssignRegionsFromSavedRegions(Sprite[] sprites, Atlas usedAtlas)
		{
			if (savedRegions == null || savedRegions.Length != sprites.Length)
			{
				return;
			}
			int i = 0;
			foreach (AtlasRegion region in usedAtlas)
			{
				SavedRegionInfo savedRegion = savedRegions[i];
				AtlasPage page = region.page;
				region.degrees = ((savedRegion.packingRotation != 0) ? 90 : 0);
				region.rotate = region.degrees != 0;
				float x = savedRegion.x;
				float y = savedRegion.y;
				float width = savedRegion.width;
				float height = savedRegion.height;
				region.u = x / (float)page.width;
				region.v = y / (float)page.height;
				if (region.rotate)
				{
					region.u2 = (x + height) / (float)page.width;
					region.v2 = (y + width) / (float)page.height;
				}
				else
				{
					region.u2 = (x + width) / (float)page.width;
					region.v2 = (y + height) / (float)page.height;
				}
				region.x = (int)x;
				region.y = (int)y;
				region.width = Math.Abs((int)width);
				region.height = Math.Abs((int)height);
				float temp = region.v;
				region.v = region.v2;
				region.v2 = temp;
				region.originalWidth = (int)width;
				region.originalHeight = (int)height;
				region.offsetX = 0f;
				region.offsetY = 0f;
				i++;
			}
		}

		private Atlas LoadAtlas(SpriteAtlas spriteAtlas)
		{
			List<AtlasPage> pages = new List<AtlasPage>();
			List<AtlasRegion> regions = new List<AtlasRegion>();
			Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
			spriteAtlas.GetSprites(sprites);
			if (sprites.Length == 0)
			{
				return new Atlas(pages, regions);
			}
			Texture2D texture = null;
			texture = AccessPackedTexture(sprites);
			Material material = materials[0];
			material.mainTexture = texture;
			AtlasPage page = new AtlasPage();
			page.name = spriteAtlas.name;
			page.width = texture.width;
			page.height = texture.height;
			page.format = Format.RGBA8888;
			page.minFilter = TextureFilter.Linear;
			page.magFilter = TextureFilter.Linear;
			page.uWrap = TextureWrap.ClampToEdge;
			page.vWrap = TextureWrap.ClampToEdge;
			page.rendererObject = material;
			pages.Add(page);
			sprites = AccessPackedSprites(spriteAtlas);
			for (int i = 0; i < sprites.Length; i++)
			{
				Sprite sprite = sprites[i];
				AtlasRegion region = new AtlasRegion();
				region.name = sprite.name.Replace("(Clone)", "");
				region.page = page;
				region.degrees = ((sprite.packingRotation != 0) ? 90 : 0);
				region.rotate = region.degrees != 0;
				region.u2 = 1f;
				region.v2 = 1f;
				region.width = page.width;
				region.height = page.height;
				region.originalWidth = page.width;
				region.originalHeight = page.height;
				region.index = i;
				regions.Add(region);
			}
			Atlas atlas = new Atlas(pages, regions);
			AssignRegionsFromSavedRegions(sprites, atlas);
			return atlas;
		}

		public static Texture2D AccessPackedTexture(Sprite[] sprites)
		{
			return sprites[0].texture;
		}

		public static Sprite[] AccessPackedSprites(SpriteAtlas spriteAtlas)
		{
			Sprite[] sprites = null;
			if (sprites == null)
			{
				sprites = new Sprite[spriteAtlas.spriteCount];
				spriteAtlas.GetSprites(sprites);
				if (sprites.Length == 0)
				{
					return null;
				}
			}
			return sprites;
		}
	}
}

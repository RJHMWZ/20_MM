using System;
using System.Collections.Generic;
using Spine.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Spine.Unity.AttachmentTools
{
	public static class AtlasUtilities
	{
		private struct IntAndAtlasRegionKey
		{
			private int i;

			private AtlasRegion region;

			public IntAndAtlasRegionKey(int i, AtlasRegion region)
			{
				this.i = i;
				this.region = region;
			}

			public override int GetHashCode()
			{
				return (i.GetHashCode() * 23) ^ region.GetHashCode();
			}
		}

		internal const TextureFormat SpineTextureFormat = TextureFormat.RGBA32;

		internal const float DefaultMipmapBias = -0.5f;

		internal const bool UseMipMaps = false;

		internal const float DefaultScale = 0.01f;

		private const int NonrenderingRegion = -1;

		private static readonly Dictionary<AtlasRegion, int> existingRegions = new Dictionary<AtlasRegion, int>();

		private static readonly List<int> regionIndices = new List<int>();

		private static readonly List<Texture2D> texturesToPack = new List<Texture2D>();

		private static readonly List<AtlasRegion> originalRegions = new List<AtlasRegion>();

		private static readonly List<AtlasRegion> repackedRegions = new List<AtlasRegion>();

		private static readonly List<Attachment> repackedAttachments = new List<Attachment>();

		private static List<Texture2D>[] texturesToPackAtParam = new List<Texture2D>[1];

		private static Dictionary<IntAndAtlasRegionKey, Texture2D> CachedRegionTextures = new Dictionary<IntAndAtlasRegionKey, Texture2D>();

		private static List<Texture2D> CachedRegionTexturesList = new List<Texture2D>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			ClearCache();
		}

		public static AtlasRegion ToAtlasRegion(this Texture2D t, Material materialPropertySource, float scale = 0.01f)
		{
			return t.ToAtlasRegion(materialPropertySource.shader, scale, materialPropertySource);
		}

		public static AtlasRegion ToAtlasRegion(this Texture2D t, Shader shader, float scale = 0.01f, Material materialPropertySource = null)
		{
			Material material = new Material(shader);
			if (materialPropertySource != null)
			{
				material.CopyPropertiesFromMaterial(materialPropertySource);
				material.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			material.mainTexture = t;
			AtlasPage page = material.ToSpineAtlasPage();
			float width = t.width;
			float height = t.height;
			AtlasRegion region = new AtlasRegion();
			region.name = t.name;
			region.index = -1;
			region.rotate = false;
			Vector2 boundsMin = Vector2.zero;
			Vector2 boundsMax = new Vector2(width, height) * scale;
			region.width = (int)width;
			region.originalWidth = (int)width;
			region.height = (int)height;
			region.originalHeight = (int)height;
			region.offsetX = width * (0.5f - InverseLerp(boundsMin.x, boundsMax.x, 0f));
			region.offsetY = height * (0.5f - InverseLerp(boundsMin.y, boundsMax.y, 0f));
			region.u = 0f;
			region.v = 1f;
			region.u2 = 1f;
			region.v2 = 0f;
			region.x = 0;
			region.y = 0;
			region.page = page;
			return region;
		}

		public static AtlasRegion ToAtlasRegionPMAClone(this Texture2D t, Material materialPropertySource, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false)
		{
			return t.ToAtlasRegionPMAClone(materialPropertySource.shader, textureFormat, mipmaps, materialPropertySource);
		}

		public static AtlasRegion ToAtlasRegionPMAClone(this Texture2D t, Shader shader, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, Material materialPropertySource = null)
		{
			Material material = new Material(shader);
			if (materialPropertySource != null)
			{
				material.CopyPropertiesFromMaterial(materialPropertySource);
				material.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			Texture2D newTexture = t.GetClone(textureFormat, mipmaps, false, true);
			newTexture.name = t.name + "-pma-";
			material.name = t.name + shader.name;
			material.mainTexture = newTexture;
			AtlasPage page = material.ToSpineAtlasPage();
			AtlasRegion region = newTexture.ToAtlasRegion(shader);
			region.page = page;
			return region;
		}

		public static AtlasPage ToSpineAtlasPage(this Material m)
		{
			AtlasPage newPage = new AtlasPage
			{
				rendererObject = m,
				name = m.name
			};
			Texture t = m.mainTexture;
			if (t != null)
			{
				newPage.width = t.width;
				newPage.height = t.height;
			}
			return newPage;
		}

		public static AtlasRegion ToAtlasRegion(this Sprite s, AtlasPage page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page", "page cannot be null. AtlasPage determines which texture region belongs and how it should be rendered. You can use material.ToSpineAtlasPage() to get a shareable AtlasPage from a Material, or use the sprite.ToAtlasRegion(material) overload.");
			}
			AtlasRegion region = s.ToAtlasRegion();
			region.page = page;
			return region;
		}

		public static AtlasRegion ToAtlasRegion(this Sprite s, Material material)
		{
			AtlasRegion region = s.ToAtlasRegion();
			region.page = material.ToSpineAtlasPage();
			return region;
		}

		public static AtlasRegion ToAtlasRegionPMAClone(this Sprite s, Material materialPropertySource, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false)
		{
			return s.ToAtlasRegionPMAClone(materialPropertySource.shader, textureFormat, mipmaps, materialPropertySource);
		}

		public static AtlasRegion ToAtlasRegionPMAClone(this Sprite s, Shader shader, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, Material materialPropertySource = null)
		{
			Material material = new Material(shader);
			if (materialPropertySource != null)
			{
				material.CopyPropertiesFromMaterial(materialPropertySource);
				material.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			Texture2D tex = s.ToTexture(textureFormat, mipmaps, false, true);
			tex.name = s.name + "-pma-";
			material.name = tex.name + shader.name;
			material.mainTexture = tex;
			AtlasPage page = material.ToSpineAtlasPage();
			AtlasRegion region = s.ToAtlasRegion(true);
			region.page = page;
			return region;
		}

		internal static AtlasRegion ToAtlasRegion(this Sprite s, bool isolatedTexture = false)
		{
			AtlasRegion region = new AtlasRegion();
			region.name = s.name;
			region.index = -1;
			region.rotate = s.packed && s.packingRotation != SpritePackingRotation.None;
			Bounds bounds = s.bounds;
			Vector2 boundsMin = bounds.min;
			Vector2 boundsMax = bounds.max;
			Rect spineRect = s.rect.SpineUnityFlipRect(s.texture.height);
			region.width = (int)spineRect.width;
			region.originalWidth = (int)spineRect.width;
			region.height = (int)spineRect.height;
			region.originalHeight = (int)spineRect.height;
			region.offsetX = spineRect.width * (0.5f - InverseLerp(boundsMin.x, boundsMax.x, 0f));
			region.offsetY = spineRect.height * (0.5f - InverseLerp(boundsMin.y, boundsMax.y, 0f));
			if (isolatedTexture)
			{
				region.u = 0f;
				region.v = 1f;
				region.u2 = 1f;
				region.v2 = 0f;
				region.x = 0;
				region.y = 0;
			}
			else
			{
				Texture2D tex = s.texture;
				Rect uvRect = TextureRectToUVRect(s.textureRect, tex.width, tex.height);
				region.u = uvRect.xMin;
				region.v = uvRect.yMax;
				region.u2 = uvRect.xMax;
				region.v2 = uvRect.yMin;
				region.x = (int)spineRect.x;
				region.y = (int)spineRect.y;
			}
			return region;
		}

		public static void GetRepackedAttachments(List<Attachment> sourceAttachments, List<Attachment> outputAttachments, Material materialPropertySource, out Material outputMaterial, out Texture2D outputTexture, int maxAtlasSize = 1024, int padding = 2, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, string newAssetName = "Repacked Attachments", bool clearCache = false, bool useOriginalNonrenderables = true)
		{
			if (sourceAttachments == null)
			{
				throw new ArgumentNullException("sourceAttachments");
			}
			if (outputAttachments == null)
			{
				throw new ArgumentNullException("outputAttachments");
			}
			existingRegions.Clear();
			regionIndices.Clear();
			texturesToPack.Clear();
			originalRegions.Clear();
			outputAttachments.Clear();
			outputAttachments.AddRange(sourceAttachments);
			int newRegionIndex = 0;
			int i = 0;
			for (int l = sourceAttachments.Count; i < l; i++)
			{
				Attachment originalAttachment = sourceAttachments[i];
				if (IsRenderable(originalAttachment))
				{
					Attachment newAttachment = originalAttachment.GetCopy(true);
					AtlasRegion region = newAttachment.GetRegion();
					if (existingRegions.TryGetValue(region, out var existingIndex))
					{
						regionIndices.Add(existingIndex);
					}
					else
					{
						originalRegions.Add(region);
						texturesToPack.Add(region.ToTexture(textureFormat, mipmaps));
						existingRegions.Add(region, newRegionIndex);
						regionIndices.Add(newRegionIndex);
						newRegionIndex++;
					}
					outputAttachments[i] = newAttachment;
				}
				else
				{
					outputAttachments[i] = (useOriginalNonrenderables ? originalAttachment : originalAttachment.GetCopy(true));
					regionIndices.Add(-1);
				}
			}
			Texture2D newTexture = new Texture2D(maxAtlasSize, maxAtlasSize, textureFormat, mipmaps);
			newTexture.mipMapBias = -0.5f;
			newTexture.name = newAssetName;
			if (texturesToPack.Count > 0)
			{
				Texture2D sourceTexture = texturesToPack[0];
				newTexture.CopyTextureAttributesFrom(sourceTexture);
			}
			Rect[] rects = newTexture.PackTextures(texturesToPack.ToArray(), padding, maxAtlasSize);
			Shader shader = ((materialPropertySource == null) ? Shader.Find("Spine/Skeleton") : materialPropertySource.shader);
			Material newMaterial = new Material(shader);
			if (materialPropertySource != null)
			{
				newMaterial.CopyPropertiesFromMaterial(materialPropertySource);
				newMaterial.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			newMaterial.name = newAssetName;
			newMaterial.mainTexture = newTexture;
			AtlasPage page = newMaterial.ToSpineAtlasPage();
			page.name = newAssetName;
			repackedRegions.Clear();
			int k = 0;
			for (int n = originalRegions.Count; k < n; k++)
			{
				AtlasRegion oldRegion = originalRegions[k];
				AtlasRegion newRegion = UVRectToAtlasRegion(rects[k], oldRegion, page);
				repackedRegions.Add(newRegion);
			}
			int j = 0;
			for (int m = outputAttachments.Count; j < m; j++)
			{
				Attachment a = outputAttachments[j];
				if (IsRenderable(a))
				{
					a.SetRegion(repackedRegions[regionIndices[j]]);
				}
			}
			if (clearCache)
			{
				ClearCache();
			}
			outputTexture = newTexture;
			outputMaterial = newMaterial;
		}

		public static Skin GetRepackedSkin(this Skin o, string newName, Material materialPropertySource, out Material outputMaterial, out Texture2D outputTexture, int maxAtlasSize = 1024, int padding = 2, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, bool useOriginalNonrenderables = true, bool clearCache = false, int[] additionalTexturePropertyIDsToCopy = null, Texture2D[] additionalOutputTextures = null, TextureFormat[] additionalTextureFormats = null, bool[] additionalTextureIsLinear = null)
		{
			return o.GetRepackedSkin(newName, materialPropertySource.shader, out outputMaterial, out outputTexture, maxAtlasSize, padding, textureFormat, mipmaps, materialPropertySource, clearCache, useOriginalNonrenderables, additionalTexturePropertyIDsToCopy, additionalOutputTextures, additionalTextureFormats, additionalTextureIsLinear);
		}

		public static Skin GetRepackedSkin(this Skin o, string newName, Shader shader, out Material outputMaterial, out Texture2D outputTexture, int maxAtlasSize = 1024, int padding = 2, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, Material materialPropertySource = null, bool clearCache = false, bool useOriginalNonrenderables = true, int[] additionalTexturePropertyIDsToCopy = null, Texture2D[] additionalOutputTextures = null, TextureFormat[] additionalTextureFormats = null, bool[] additionalTextureIsLinear = null)
		{
			outputTexture = null;
			if (additionalTexturePropertyIDsToCopy != null && additionalTextureIsLinear == null)
			{
				additionalTextureIsLinear = new bool[additionalTexturePropertyIDsToCopy.Length];
				for (int i = 0; i < additionalTextureIsLinear.Length; i++)
				{
					additionalTextureIsLinear[i] = true;
				}
			}
			if (o == null)
			{
				throw new NullReferenceException("Skin was null");
			}
			OrderedDictionary<Skin.SkinEntry, Attachment> skinAttachments = o.Attachments;
			Skin newSkin = new Skin(newName);
			newSkin.bones.AddRange(o.bones);
			newSkin.constraints.AddRange(o.constraints);
			existingRegions.Clear();
			regionIndices.Clear();
			repackedAttachments.Clear();
			int numTextureParamsToRepack = 1 + ((additionalTexturePropertyIDsToCopy != null) ? additionalTexturePropertyIDsToCopy.Length : 0);
			additionalOutputTextures = ((additionalTexturePropertyIDsToCopy == null) ? null : new Texture2D[additionalTexturePropertyIDsToCopy.Length]);
			if (texturesToPackAtParam.Length < numTextureParamsToRepack)
			{
				Array.Resize(ref texturesToPackAtParam, numTextureParamsToRepack);
			}
			for (int j = 0; j < numTextureParamsToRepack; j++)
			{
				if (texturesToPackAtParam[j] != null)
				{
					texturesToPackAtParam[j].Clear();
				}
				else
				{
					texturesToPackAtParam[j] = new List<Texture2D>();
				}
			}
			originalRegions.Clear();
			int newRegionIndex = 0;
			foreach (KeyValuePair<Skin.SkinEntry, Attachment> skinEntry in skinAttachments)
			{
				Skin.SkinEntry originalKey = skinEntry.Key;
				Attachment originalAttachment = skinEntry.Value;
				if (IsRenderable(originalAttachment))
				{
					Attachment newAttachment = originalAttachment.GetCopy(true);
					AtlasRegion region = newAttachment.GetRegion();
					if (existingRegions.TryGetValue(region, out var existingIndex))
					{
						regionIndices.Add(existingIndex);
					}
					else
					{
						originalRegions.Add(region);
						for (int n = 0; n < numTextureParamsToRepack; n++)
						{
							Texture2D regionTexture = ((n == 0) ? region.ToTexture(textureFormat, mipmaps) : region.ToTexture((additionalTextureFormats != null && n - 1 < additionalTextureFormats.Length) ? additionalTextureFormats[n - 1] : textureFormat, mipmaps, additionalTexturePropertyIDsToCopy[n - 1], additionalTextureIsLinear[n - 1]));
							texturesToPackAtParam[n].Add(regionTexture);
						}
						existingRegions.Add(region, newRegionIndex);
						regionIndices.Add(newRegionIndex);
						newRegionIndex++;
					}
					repackedAttachments.Add(newAttachment);
					newSkin.SetAttachment(originalKey.SlotIndex, originalKey.Name, newAttachment);
				}
				else
				{
					newSkin.SetAttachment(originalKey.SlotIndex, originalKey.Name, useOriginalNonrenderables ? originalAttachment : originalAttachment.GetCopy(true));
				}
			}
			Material newMaterial = new Material(shader);
			if (materialPropertySource != null)
			{
				newMaterial.CopyPropertiesFromMaterial(materialPropertySource);
				newMaterial.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			newMaterial.name = newName;
			Rect[] rects = null;
			for (int m = 0; m < numTextureParamsToRepack; m++)
			{
				Texture2D newTexture = new Texture2D(maxAtlasSize, maxAtlasSize, (m > 0 && additionalTextureFormats != null && m - 1 < additionalTextureFormats.Length) ? additionalTextureFormats[m - 1] : textureFormat, mipmaps, m > 0 && additionalTextureIsLinear[m - 1]);
				newTexture.mipMapBias = -0.5f;
				List<Texture2D> texturesToPack = texturesToPackAtParam[m];
				if (texturesToPack.Count > 0)
				{
					Texture2D sourceTexture = texturesToPack[0];
					newTexture.CopyTextureAttributesFrom(sourceTexture);
				}
				newTexture.name = newName;
				Rect[] rectsForTexParam = newTexture.PackTextures(texturesToPack.ToArray(), padding, maxAtlasSize);
				if (m == 0)
				{
					rects = rectsForTexParam;
					newMaterial.mainTexture = newTexture;
					outputTexture = newTexture;
				}
				else
				{
					newMaterial.SetTexture(additionalTexturePropertyIDsToCopy[m - 1], newTexture);
					additionalOutputTextures[m - 1] = newTexture;
				}
			}
			AtlasPage page = newMaterial.ToSpineAtlasPage();
			page.name = newName;
			repackedRegions.Clear();
			int l = 0;
			for (int n3 = originalRegions.Count; l < n3; l++)
			{
				AtlasRegion oldRegion = originalRegions[l];
				AtlasRegion newRegion = UVRectToAtlasRegion(rects[l], oldRegion, page);
				repackedRegions.Add(newRegion);
			}
			int k = 0;
			for (int n2 = repackedAttachments.Count; k < n2; k++)
			{
				Attachment a = repackedAttachments[k];
				if (IsRenderable(a))
				{
					a.SetRegion(repackedRegions[regionIndices[k]]);
				}
			}
			if (clearCache)
			{
				ClearCache();
			}
			outputMaterial = newMaterial;
			return newSkin;
		}

		public static Sprite ToSprite(this AtlasRegion ar, float pixelsPerUnit = 100f)
		{
			return Sprite.Create(ar.GetMainTexture(), ar.GetUnityRect(), new Vector2(0.5f, 0.5f), pixelsPerUnit);
		}

		public static void ClearCache()
		{
			foreach (Texture2D t in CachedRegionTexturesList)
			{
				UnityEngine.Object.Destroy(t);
			}
			CachedRegionTextures.Clear();
			CachedRegionTexturesList.Clear();
		}

		public static Texture2D ToTexture(this AtlasRegion ar, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, int texturePropertyId = 0, bool linear = false, bool applyPMA = false)
		{
			IntAndAtlasRegionKey cacheKey = new IntAndAtlasRegionKey(texturePropertyId, ar);
			CachedRegionTextures.TryGetValue(cacheKey, out var output);
			if (output == null)
			{
				Texture2D sourceTexture = ((texturePropertyId == 0) ? ar.GetMainTexture() : ar.GetTexture(texturePropertyId));
				Rect r = ar.GetUnityRect();
				if (sourceTexture.width < ar.page.width)
				{
					float scaleX = (float)sourceTexture.width / (float)ar.page.width;
					float scaleY = (float)sourceTexture.height / (float)ar.page.height;
					Vector2 scale = new Vector2(scaleX, scaleY);
					r = new Rect(Vector2.Scale(r.position, scale), Vector2.Scale(r.size, scale));
				}
				int width = (int)r.width;
				int height = (int)r.height;
				output = new Texture2D(width, height, textureFormat, mipmaps, linear)
				{
					name = ar.name
				};
				output.CopyTextureAttributesFrom(sourceTexture);
				if (applyPMA)
				{
					CopyTextureApplyPMA(sourceTexture, r, output);
				}
				else
				{
					CopyTexture(sourceTexture, r, output);
				}
				CachedRegionTextures.Add(cacheKey, output);
				CachedRegionTexturesList.Add(output);
			}
			return output;
		}

		private static Texture2D ToTexture(this Sprite s, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, bool linear = false, bool applyPMA = false)
		{
			Texture2D spriteTexture = s.texture;
			Rect r;
			if (!s.packed || s.packingMode == SpritePackingMode.Rectangle)
			{
				r = s.textureRect;
			}
			else
			{
				r = default(Rect);
				r.xMin = Math.Min(s.uv[0].x, s.uv[1].x) * (float)spriteTexture.width;
				r.xMax = Math.Max(s.uv[0].x, s.uv[1].x) * (float)spriteTexture.width;
				r.yMin = Math.Min(s.uv[0].y, s.uv[2].y) * (float)spriteTexture.height;
				r.yMax = Math.Max(s.uv[0].y, s.uv[2].y) * (float)spriteTexture.height;
			}
			Texture2D newTexture = new Texture2D((int)r.width, (int)r.height, textureFormat, mipmaps, linear);
			newTexture.CopyTextureAttributesFrom(spriteTexture);
			if (applyPMA)
			{
				CopyTextureApplyPMA(spriteTexture, r, newTexture);
			}
			else
			{
				CopyTexture(spriteTexture, r, newTexture);
			}
			return newTexture;
		}

		private static Texture2D GetClone(this Texture2D t, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipmaps = false, bool linear = false, bool applyPMA = false)
		{
			Texture2D newTexture = new Texture2D(t.width, t.height, textureFormat, mipmaps, linear);
			newTexture.CopyTextureAttributesFrom(t);
			if (applyPMA)
			{
				CopyTextureApplyPMA(t, new Rect(0f, 0f, t.width, t.height), newTexture);
			}
			else
			{
				CopyTexture(t, new Rect(0f, 0f, t.width, t.height), newTexture);
			}
			return newTexture;
		}

		private static void CopyTexture(Texture2D source, Rect sourceRect, Texture2D destination)
		{
			if (SystemInfo.copyTextureSupport == CopyTextureSupport.None)
			{
				Color[] pixelBuffer = source.GetPixels((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
				destination.SetPixels(pixelBuffer);
				destination.Apply();
			}
			else
			{
				Graphics.CopyTexture(source, 0, 0, (int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height, destination, 0, 0, 0, 0);
			}
		}

		private static void CopyTextureApplyPMA(Texture2D source, Rect sourceRect, Texture2D destination)
		{
			Color[] pixelBuffer = source.GetPixels((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
			int i = 0;
			for (int j = pixelBuffer.Length; i < j; i++)
			{
				Color p = pixelBuffer[i];
				float a = p.a;
				p.r *= a;
				p.g *= a;
				p.b *= a;
				pixelBuffer[i] = p;
			}
			destination.SetPixels(pixelBuffer);
			destination.Apply();
		}

		private static bool IsRenderable(Attachment a)
		{
			return a is IHasRendererObject;
		}

		private static Rect SpineUnityFlipRect(this Rect rect, int textureHeight)
		{
			rect.y = (float)textureHeight - rect.y - rect.height;
			return rect;
		}

		private static Rect GetUnityRect(this AtlasRegion region)
		{
			return region.GetSpineAtlasRect().SpineUnityFlipRect(region.page.height);
		}

		private static Rect GetUnityRect(this AtlasRegion region, int textureHeight)
		{
			return region.GetSpineAtlasRect().SpineUnityFlipRect(textureHeight);
		}

		private static Rect GetSpineAtlasRect(this AtlasRegion region, bool includeRotate = true)
		{
			if (includeRotate && (region.degrees == 90 || region.degrees == 270))
			{
				return new Rect(region.x, region.y, region.height, region.width);
			}
			return new Rect(region.x, region.y, region.width, region.height);
		}

		private static Rect UVRectToTextureRect(Rect uvRect, int texWidth, int texHeight)
		{
			uvRect.x *= texWidth;
			uvRect.width *= texWidth;
			uvRect.y *= texHeight;
			uvRect.height *= texHeight;
			return uvRect;
		}

		private static Rect TextureRectToUVRect(Rect textureRect, int texWidth, int texHeight)
		{
			textureRect.x = Mathf.InverseLerp(0f, texWidth, textureRect.x);
			textureRect.y = Mathf.InverseLerp(0f, texHeight, textureRect.y);
			textureRect.width = Mathf.InverseLerp(0f, texWidth, textureRect.width);
			textureRect.height = Mathf.InverseLerp(0f, texHeight, textureRect.height);
			return textureRect;
		}

		private static AtlasRegion UVRectToAtlasRegion(Rect uvRect, AtlasRegion referenceRegion, AtlasPage page)
		{
			Rect tr = UVRectToTextureRect(uvRect, page.width, page.height);
			Rect rr = tr.SpineUnityFlipRect(page.height);
			int x = (int)rr.x;
			int y = (int)rr.y;
			int w;
			int h;
			if (referenceRegion.degrees == 90 || referenceRegion.degrees == 270)
			{
				w = (int)rr.height;
				h = (int)rr.width;
			}
			else
			{
				w = (int)rr.width;
				h = (int)rr.height;
			}
			int originalW = Mathf.RoundToInt((float)w * ((float)referenceRegion.originalWidth / (float)referenceRegion.width));
			int originalH = Mathf.RoundToInt((float)h * ((float)referenceRegion.originalHeight / (float)referenceRegion.height));
			int offsetX = Mathf.RoundToInt(referenceRegion.offsetX * ((float)w / (float)referenceRegion.width));
			int offsetY = Mathf.RoundToInt(referenceRegion.offsetY * ((float)h / (float)referenceRegion.height));
			if (referenceRegion.degrees == 270)
			{
				w = (int)rr.width;
				h = (int)rr.height;
			}
			float u = uvRect.xMin;
			float u2 = uvRect.xMax;
			float v = uvRect.yMax;
			float v2 = uvRect.yMin;
			return new AtlasRegion
			{
				page = page,
				name = referenceRegion.name,
				u = u,
				u2 = u2,
				v = v,
				v2 = v2,
				index = -1,
				width = w,
				originalWidth = originalW,
				height = h,
				originalHeight = originalH,
				offsetX = offsetX,
				offsetY = offsetY,
				x = x,
				y = y,
				rotate = referenceRegion.rotate,
				degrees = referenceRegion.degrees
			};
		}

		private static Texture2D GetMainTexture(this AtlasRegion region)
		{
			Material material = region.page.rendererObject as Material;
			return material.mainTexture as Texture2D;
		}

		private static Texture2D GetTexture(this AtlasRegion region, string texturePropertyName)
		{
			Material material = region.page.rendererObject as Material;
			return material.GetTexture(texturePropertyName) as Texture2D;
		}

		private static Texture2D GetTexture(this AtlasRegion region, int texturePropertyId)
		{
			Material material = region.page.rendererObject as Material;
			return material.GetTexture(texturePropertyId) as Texture2D;
		}

		private static void CopyTextureAttributesFrom(this Texture2D destination, Texture2D source)
		{
			destination.filterMode = source.filterMode;
			destination.anisoLevel = source.anisoLevel;
			destination.wrapModeU = source.wrapModeU;
			destination.wrapModeV = source.wrapModeV;
			destination.wrapModeW = source.wrapModeW;
		}

		private static float InverseLerp(float a, float b, float value)
		{
			return (value - a) / (b - a);
		}
	}
}

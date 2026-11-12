using System;
using UnityEngine;

namespace Spine.Unity.AttachmentTools
{
	public static class AttachmentCloneExtensions
	{
		public static Attachment GetCopy(this Attachment o, bool cloneMeshesAsLinked)
		{
			MeshAttachment meshAttachment = o as MeshAttachment;
			if (meshAttachment != null && cloneMeshesAsLinked)
			{
				return meshAttachment.NewLinkedMesh();
			}
			return o.Copy();
		}

		public static MeshAttachment GetLinkedMesh(this MeshAttachment o, string newLinkedMeshName, AtlasRegion region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			MeshAttachment mesh = o.NewLinkedMesh();
			mesh.SetRegion(region, false);
			return mesh;
		}

		public static MeshAttachment GetLinkedMesh(this MeshAttachment o, Sprite sprite, Shader shader, Material materialPropertySource = null)
		{
			Material i = new Material(shader);
			if (materialPropertySource != null)
			{
				i.CopyPropertiesFromMaterial(materialPropertySource);
				i.shaderKeywords = materialPropertySource.shaderKeywords;
			}
			return o.GetLinkedMesh(sprite.name, sprite.ToAtlasRegion());
		}

		public static MeshAttachment GetLinkedMesh(this MeshAttachment o, Sprite sprite, Material materialPropertySource)
		{
			return o.GetLinkedMesh(sprite, materialPropertySource.shader, materialPropertySource);
		}

		public static Attachment GetRemappedClone(this Attachment o, Sprite sprite, Material sourceMaterial, bool premultiplyAlpha = true, bool cloneMeshAsLinked = true, bool useOriginalRegionSize = false, bool pivotShiftsMeshUVCoords = true, bool useOriginalRegionScale = false)
		{
			AtlasRegion atlasRegion = (premultiplyAlpha ? sprite.ToAtlasRegionPMAClone(sourceMaterial) : sprite.ToAtlasRegion(new Material(sourceMaterial)
			{
				mainTexture = sprite.texture
			}));
			if (!pivotShiftsMeshUVCoords && o is MeshAttachment)
			{
				atlasRegion.offsetX = 0f;
				atlasRegion.offsetY = 0f;
			}
			float scale = 1f / sprite.pixelsPerUnit;
			if (useOriginalRegionScale && o is RegionAttachment regionAttachment)
			{
				scale = regionAttachment.width / regionAttachment.regionOriginalWidth;
			}
			return o.GetRemappedClone(atlasRegion, cloneMeshAsLinked, useOriginalRegionSize, scale);
		}

		public static Attachment GetRemappedClone(this Attachment o, AtlasRegion atlasRegion, bool cloneMeshAsLinked = true, bool useOriginalRegionSize = false, float scale = 0.01f)
		{
			if (o is RegionAttachment regionAttachment)
			{
				RegionAttachment newAttachment = (RegionAttachment)regionAttachment.Copy();
				newAttachment.SetRegion(atlasRegion, false);
				if (!useOriginalRegionSize)
				{
					newAttachment.width = (float)atlasRegion.width * scale;
					newAttachment.height = (float)atlasRegion.height * scale;
				}
				newAttachment.UpdateOffset();
				return newAttachment;
			}
			if (o is MeshAttachment meshAttachment)
			{
				MeshAttachment newAttachment2 = (cloneMeshAsLinked ? meshAttachment.NewLinkedMesh() : ((MeshAttachment)meshAttachment.Copy()));
				newAttachment2.SetRegion(atlasRegion);
				return newAttachment2;
			}
			return o.GetCopy(true);
		}
	}
}

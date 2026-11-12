using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	[CreateAssetMenu(fileName = "New Spine Atlas Asset", menuName = "Spine/Spine Atlas Asset")]
	public class SpineAtlasAsset : AtlasAssetBase
	{
		public TextAsset atlasFile;

		public Material[] materials;

		protected Atlas atlas;

		public override bool IsLoaded => atlas != null;

		public override IEnumerable<Material> Materials => materials;

		public override int MaterialCount => (materials != null) ? materials.Length : 0;

		public override Material PrimaryMaterial => materials[0];

		public static SpineAtlasAsset CreateRuntimeInstance(TextAsset atlasText, Material[] materials, bool initialize)
		{
			SpineAtlasAsset atlasAsset = ScriptableObject.CreateInstance<SpineAtlasAsset>();
			atlasAsset.Reset();
			atlasAsset.atlasFile = atlasText;
			atlasAsset.materials = materials;
			if (initialize)
			{
				atlasAsset.GetAtlas();
			}
			return atlasAsset;
		}

		public static SpineAtlasAsset CreateRuntimeInstance(TextAsset atlasText, Texture2D[] textures, Material materialPropertySource, bool initialize)
		{
			string atlasString = atlasText.text;
			atlasString = atlasString.Replace("\r", "");
			string[] atlasLines = atlasString.Split('\n');
			List<string> pages = new List<string>();
			for (int j = 0; j < atlasLines.Length - 1; j++)
			{
				if (atlasLines[j].Trim().Length == 0)
				{
					pages.Add(atlasLines[j + 1].Trim().Replace(".png", ""));
				}
			}
			Material[] materials = new Material[pages.Count];
			int i = 0;
			for (int m = pages.Count; i < m; i++)
			{
				Material mat = null;
				string pageName = pages[i];
				int k = 0;
				for (int l = textures.Length; k < l; k++)
				{
					if (string.Equals(pageName, textures[k].name, StringComparison.OrdinalIgnoreCase))
					{
						mat = new Material(materialPropertySource);
						mat.mainTexture = textures[k];
						break;
					}
				}
				if (mat != null)
				{
					materials[i] = mat;
					continue;
				}
				throw new ArgumentException("Could not find matching atlas page in the texture array.");
			}
			return CreateRuntimeInstance(atlasText, materials, initialize);
		}

		public static SpineAtlasAsset CreateRuntimeInstance(TextAsset atlasText, Texture2D[] textures, Shader shader, bool initialize)
		{
			if (shader == null)
			{
				shader = Shader.Find("Spine/Skeleton");
			}
			Material materialProperySource = new Material(shader);
			return CreateRuntimeInstance(atlasText, textures, materialProperySource, initialize);
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
			if (atlasFile == null)
			{
				Debug.LogError("Atlas file not set for atlas asset: " + base.name, this);
				Clear();
				return null;
			}
			if (materials == null || materials.Length == 0)
			{
				Debug.LogError("Materials not set for atlas asset: " + base.name, this);
				Clear();
				return null;
			}
			if (atlas != null)
			{
				return atlas;
			}
			try
			{
				atlas = new Atlas(new StringReader(atlasFile.text), "", new MaterialsTextureLoader(this));
				atlas.FlipV();
				return atlas;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error reading atlas file for atlas asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
				return null;
			}
		}

		public Mesh GenerateMesh(string name, Mesh mesh, out Material material, float scale = 0.01f)
		{
			AtlasRegion region = atlas.FindRegion(name);
			material = null;
			if (region != null)
			{
				if (mesh == null)
				{
					mesh = new Mesh();
					mesh.name = name;
				}
				Vector3[] verts = new Vector3[4];
				Vector2[] uvs = new Vector2[4];
				Color[] colors = new Color[4]
				{
					Color.white,
					Color.white,
					Color.white,
					Color.white
				};
				int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
				float left = (float)region.width / -2f;
				float right = left * -1f;
				float top = (float)region.height / 2f;
				float bottom = top * -1f;
				verts[0] = new Vector3(left, bottom, 0f) * scale;
				verts[1] = new Vector3(left, top, 0f) * scale;
				verts[2] = new Vector3(right, top, 0f) * scale;
				verts[3] = new Vector3(right, bottom, 0f) * scale;
				float u = region.u;
				float v = region.v;
				float u2 = region.u2;
				float v2 = region.v2;
				if (!region.rotate)
				{
					uvs[0] = new Vector2(u, v2);
					uvs[1] = new Vector2(u, v);
					uvs[2] = new Vector2(u2, v);
					uvs[3] = new Vector2(u2, v2);
				}
				else
				{
					uvs[0] = new Vector2(u2, v2);
					uvs[1] = new Vector2(u, v2);
					uvs[2] = new Vector2(u, v);
					uvs[3] = new Vector2(u2, v);
				}
				mesh.triangles = new int[0];
				mesh.vertices = verts;
				mesh.uv = uvs;
				mesh.colors = colors;
				mesh.triangles = triangles;
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				material = (Material)region.page.rendererObject;
			}
			else
			{
				mesh = null;
			}
			return mesh;
		}
	}
}

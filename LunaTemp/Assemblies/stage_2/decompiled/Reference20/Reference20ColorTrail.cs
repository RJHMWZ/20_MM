using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[RequireComponent(typeof(CanvasRenderer))]
	public sealed class Reference20ColorTrail : MaskableGraphic
	{
		private static HtmlTrailData source;

		private HtmlTrailLayer layer;

		private Texture2D texture;

		private float elapsed;

		private float length;

		public override Texture mainTexture => texture ? texture : Texture2D.whiteTexture;

		public Reference20ColorTrail()
		{
			base.useLegacyMeshGeneration = false;
		}

		public static void Play(RectTransform stage, Vector2 from, Vector2 to)
		{
			if (source == null)
			{
				source = JsonConvert.DeserializeObject<HtmlTrailData>(Resources.Load<TextAsset>("Reference20/Company/Particles/ColorTrail").text);
			}
			RectTransform root = Reference20Props.Rect("HtmlColorTrail", Reference20Native.Overlay(stage));
			Vector2 vector2 = (root.anchorMin = (root.anchorMax = Vector2.zero));
			root.anchoredPosition = from;
			Vector2 delta = to - from;
			root.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * 57.29578f);
			HtmlTrailLayer[] layers = source.layers;
			foreach (HtmlTrailLayer data in layers)
			{
				RectTransform rt = Reference20Props.Rect(data.name, root);
				Reference20ColorTrail g = rt.gameObject.AddComponent<Reference20ColorTrail>();
				g.layer = data;
				g.length = delta.magnitude;
				g.texture = Resources.Load<Texture2D>("Reference20/Company/Particles/" + data.texture);
				if (!data.material)
				{
					data.material = new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));
					float boost = ((data.shader == null || !data.shader.StartsWith("Legacy")) ? 1 : 2);
					data.material.SetColor("_Color", new Color(data.tint[0], data.tint[1], data.tint[2], data.tint[3]) * boost);
					data.material.SetFloat("_SrcBlend", 5f);
					data.material.SetFloat("_DstBlend", data.blend);
				}
				g.material = data.material;
				g.raycastTarget = false;
				g.maskable = false;
			}
			Object.Destroy(root.gameObject, 0.5f);
		}

		private void Update()
		{
			elapsed += Time.deltaTime;
			SetVerticesDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (layer == null)
			{
				Empty(vh);
				return;
			}
			HtmlTrailFrame frame = null;
			HtmlTrailFrame[] frames = layer.frames;
			foreach (HtmlTrailFrame f in frames)
			{
				if (f.time > elapsed)
				{
					break;
				}
				frame = f;
			}
			if (frame == null || frame.vertices == null)
			{
				Empty(vh);
				return;
			}
			float[][] vertices = frame.vertices;
			foreach (float[] v in vertices)
			{
				if (v.Length < 8)
				{
					continue;
				}
				Color c = new Color(v[4], v[5], v[6], v[7]);
				Vector2 uv = new Vector2(v[2] * layer.tiling[0] + layer.tiling[2], v[3] * layer.tiling[1] + layer.tiling[3]);
				if (layer.flipbook != null && layer.flipbook.Length == 4)
				{
					float[] fb = layer.flipbook;
					float t = Mathf.LerpUnclamped(Time.time, layer.frame - 1f, layer.switchValue);
					float index = Mathf.Round((t * fb[2] + fb[3] - 1f) % (fb[0] * fb[1]));
					if (index < 0f)
					{
						index += fb[0] * fb[1];
					}
					float column = Mathf.Round(index % fb[0]);
					float row = Mathf.Round((index - column) / fb[0] % fb[1]);
					uv = new Vector2((uv.x + Time.time * layer.uvSpeed[0] + column) / fb[0], (uv.y + Time.time * layer.uvSpeed[1] + fb[1] - 1f - row) / fb[1]);
				}
				vh.AddVert(new Vector3(v[0] * length, v[1] / 0.52734375f, 0f), c, uv);
			}
			for (int i = 0; i + 2 < frame.indices.Length; i += 3)
			{
				int a = frame.indices[i];
				int b = frame.indices[i + 1];
				int c2 = frame.indices[i + 2];
				if (a >= 0 && b >= 0 && c2 >= 0 && a < vh.currentVertCount && b < vh.currentVertCount && c2 < vh.currentVertCount)
				{
					vh.AddTriangle(a, b, c2);
				}
			}
			if (vh.currentVertCount == 0)
			{
				vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
				vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
				vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
				vh.AddTriangle(0, 1, 2);
			}
		}

		private static void Empty(VertexHelper vh)
		{
			vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
			vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
			vh.AddVert(Vector3.zero, Color.clear, Vector2.zero);
			vh.AddTriangle(0, 1, 2);
		}
	}
}

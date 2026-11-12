using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Reference20
{
	public static class Reference20CompanyParticles
	{
		private static readonly Dictionary<string, CompanyParticleEffect> definitions = new Dictionary<string, CompanyParticleEffect>();

		private static readonly List<Reference20CompanyParticleHandle> active = new List<Reference20CompanyParticleHandle>(128);

		private static readonly Stack<Reference20CompanyParticleHandle> pool = new Stack<Reference20CompanyParticleHandle>(128);

		private static readonly Dictionary<string, Reference20CompanyParticleBatch> batches = new Dictionary<string, Reference20CompanyParticleBatch>();

		private static readonly Dictionary<string, Material> sourceMaterials = new Dictionary<string, Material>();

		private static RectTransform overlay;

		public static List<Reference20CompanyParticleHandle> Active => active;

		public static void Load()
		{
			if (definitions.Count > 0)
			{
				return;
			}
			string[] array = new string[8] { "NormalBlue", "NormalGold", "Wave", "RocketTrail", "ColorCharge", "ColorFinish", "Bomb", "BeeImpact" };
			foreach (string name in array)
			{
				CompanyParticleEffect data = JsonConvert.DeserializeObject<CompanyParticleEffect>(Resources.Load<TextAsset>("Reference20/Company/Particles/" + name).text);
				CompanyParticleLayer[] layers = data.layers;
				foreach (CompanyParticleLayer layer in layers)
				{
					layer.image = Resources.Load<Texture2D>("Reference20/Company/Particles/" + layer.texture);
					layer.additive = layer.shader.Contains("Additive") || layer.shader.Contains("AddUV");
					float boost = ((!layer.shader.StartsWith("Legacy") && !layer.shader.StartsWith("Mobile") && !(layer.shader == "UI/AlphaBlended")) ? 1 : 2);
					layer.color = new Color(layer.tint[0], layer.tint[1], layer.tint[2], layer.tint[3]) * boost;
					string key = layer.shader + ":" + string.Join(",", layer.tint);
					if (!sourceMaterials.TryGetValue(key, out var mat))
					{
						mat = new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));
						mat.name = "HtmlParticle_" + key;
						mat.SetFloat("_SrcBlend", 5f);
						mat.SetFloat("_DstBlend", layer.additive ? 1 : 10);
						mat.SetColor("_Color", layer.color);
						sourceMaterials[key] = mat;
					}
					layer.material = mat;
					layer.batchKey = layer.texture + ":" + key;
				}
				definitions[name] = data;
			}
		}

		private static Reference20CompanyParticleHandle Create()
		{
			RectTransform root = Reference20Props.Rect("CompanyParticles", overlay);
			Vector2 vector2 = (root.anchorMin = (root.anchorMax = Vector2.zero));
			Reference20CompanyParticleHandle h = root.gameObject.AddComponent<Reference20CompanyParticleHandle>();
			root.gameObject.SetActive(false);
			return h;
		}

		public static void Prewarm(RectTransform stage)
		{
			Load();
			RectTransform target = Reference20Native.Overlay(stage);
			if (overlay == target && batches.Count > 0)
			{
				return;
			}
			overlay = target;
			active.Clear();
			pool.Clear();
			batches.Clear();
			foreach (CompanyParticleEffect effect in definitions.Values)
			{
				CompanyParticleLayer[] layers = effect.layers;
				foreach (CompanyParticleLayer layer in layers)
				{
					string key = layer.batchKey;
					if (!batches.ContainsKey(key))
					{
						RectTransform rt = Reference20Props.Rect("CompanyBatch_" + layer.texture, overlay);
						Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
						Reference20CompanyParticleBatch b = rt.gameObject.AddComponent<Reference20CompanyParticleBatch>();
						b.Configure(layer);
						batches[key] = b;
					}
				}
			}
			for (int i = 0; i < 128; i++)
			{
				pool.Push(Create());
			}
		}

		public static void Reset()
		{
			active.Clear();
			pool.Clear();
			batches.Clear();
			overlay = null;
		}

		public static void RaiseAboveFocus()
		{
			foreach (Reference20CompanyParticleBatch batch in batches.Values)
			{
				if ((bool)batch)
				{
					batch.transform.SetAsLastSibling();
				}
			}
		}

		public static RectTransform Play(string name, RectTransform stage, Vector2 position, float scale = 1f, bool loop = false)
		{
			Prewarm(stage);
			if (!definitions.TryGetValue(name, out var definition))
			{
				return null;
			}
			Reference20CompanyParticleHandle h = null;
			while (pool.Count > 0 && !h)
			{
				h = pool.Pop();
			}
			if (!h)
			{
				h = Create();
			}
			RectTransform rt = (RectTransform)h.transform;
			rt.SetParent(overlay, false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = position;
			rt.localRotation = Quaternion.identity;
			Vector3 one = Vector3.one;
			float num;
			switch (name)
			{
			default:
				num = 1.8962963f;
				break;
			case "NormalBlue":
			case "NormalGold":
			case "Wave":
				num = 1f;
				break;
			}
			rt.localScale = one * (scale * num);
			h.Play(definition, loop, name == "ColorCombo");
			active.Add(h);
			return rt;
		}

		public static void Release(Reference20CompanyParticleHandle h)
		{
			active.Remove(h);
			if ((bool)overlay)
			{
				h.transform.SetParent(overlay, false);
			}
			pool.Push(h);
		}

		public static void MovingTrail(RectTransform stage, Vector2 from, Vector2 direction, float duration)
		{
			RectTransform rt = Play("RocketTrail", stage, from);
			Reference20CompanyParticleHandle h = rt.GetComponent<Reference20CompanyParticleHandle>();
			h.Velocity = Vector2.right * (1704f / duration);
			rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * 57.29578f);
		}

		public static void StopChildren(Transform root)
		{
			Reference20CompanyParticleHandle[] componentsInChildren = root.GetComponentsInChildren<Reference20CompanyParticleHandle>(true);
			foreach (Reference20CompanyParticleHandle h in componentsInChildren)
			{
				h.Stop();
			}
		}
	}
}

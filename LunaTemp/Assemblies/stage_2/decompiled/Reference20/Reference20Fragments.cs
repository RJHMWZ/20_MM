using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Reference20
{
	public static class Reference20Fragments
	{
		[Serializable]
		public sealed class Region
		{
			public int x;

			public int y;

			public int w;

			public int h;
		}

		[Serializable]
		public sealed class Sheet
		{
			public string name;

			public Region[] regions;
		}

		[Serializable]
		private sealed class Manifest
		{
			public Sheet[] sheets;
		}

		private static readonly Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();

		private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

		private static readonly Dictionary<string, Stack<Reference20FragmentBurst>> pools = new Dictionary<string, Stack<Reference20FragmentBurst>>();

		private static void Load()
		{
			if (sheets.Count <= 0)
			{
				Manifest manifest = JsonConvert.DeserializeObject<Manifest>(Resources.Load<TextAsset>("Reference20/Props/Fragments/regions").text);
				Sheet[] array = manifest.sheets;
				foreach (Sheet sheet in array)
				{
					sheets[sheet.name] = sheet;
					textures[sheet.name] = Resources.Load<Texture2D>("Reference20/Props/Fragments/" + sheet.name);
				}
			}
		}

		public static void Prewarm()
		{
			Load();
		}

		public static void Reset()
		{
			pools.Clear();
		}

		public static void Burst(RectTransform stage, Vector2 position, string name, int count = 12, float scale = 1f)
		{
			Load();
			if (sheets.TryGetValue(name, out var sheet) && sheet.regions.Length != 0)
			{
				if (!pools.TryGetValue(name, out var pool))
				{
					pool = new Stack<Reference20FragmentBurst>();
					pools[name] = pool;
				}
				Reference20FragmentBurst burst = null;
				while (pool.Count > 0 && !burst)
				{
					burst = pool.Pop();
				}
				if (!burst)
				{
					RectTransform r = Reference20Props.Rect("Fragments_" + name, Reference20Native.Overlay(stage));
					burst = r.gameObject.AddComponent<Reference20FragmentBurst>();
				}
				RectTransform root = burst.rectTransform;
				root.SetParent(Reference20Native.Overlay(stage), false);
				Vector2 vector2 = (root.anchorMin = (root.anchorMax = Vector2.zero));
				root.anchoredPosition = position;
				root.localScale = Vector3.one * (0.8875f * scale);
				root.SetAsLastSibling();
				burst.Play(textures[name], sheet.regions, count, delegate
				{
					pool.Push(burst);
				});
			}
		}
	}
}

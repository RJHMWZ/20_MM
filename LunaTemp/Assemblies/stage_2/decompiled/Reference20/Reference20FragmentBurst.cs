using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[RequireComponent(typeof(CanvasRenderer))]
	public sealed class Reference20FragmentBurst : MaskableGraphic
	{
		private struct Fragment
		{
			public Reference20Fragments.Region region;

			public Vector2 start;

			public Vector2 velocity;

			public float angle;

			public float spin;

			public float size;

			public float life;
		}

		private readonly List<Fragment> parts = new List<Fragment>();

		private readonly UIVertex[] quad = new UIVertex[4];

		private Texture2D texture;

		private float elapsed;

		private Action complete;

		public override Texture mainTexture => texture ? texture : Texture2D.whiteTexture;

		public void Play(Texture2D atlas, Reference20Fragments.Region[] regions, int count, Action done)
		{
			texture = atlas;
			complete = done;
			elapsed = 0f;
			parts.Clear();
			raycastTarget = false;
			for (int i = 0; i < count; i++)
			{
				Reference20Fragments.Region region = regions[UnityEngine.Random.Range(0, regions.Length)];
				float angle = UnityEngine.Random.Range(0f, 3.14159265f * 2f);
				parts.Add(new Fragment
				{
					region = region,
					start = UnityEngine.Random.insideUnitCircle * 16f,
					velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * UnityEngine.Random.Range(80f, 260f) + Vector2.up * 90f,
					angle = UnityEngine.Random.Range(-3.14159265f, 3.14159265f),
					spin = UnityEngine.Random.Range(-8f, 8f),
					size = UnityEngine.Random.Range(0.5f, 0.95f) * Mathf.Min(1f, 40f / (float)Mathf.Max(region.w, region.h)),
					life = UnityEngine.Random.Range(0.45f, 0.75f)
				});
			}
			base.gameObject.SetActive(true);
			SetMaterialDirty();
			SetVerticesDirty();
		}

		private void Update()
		{
			if (complete != null)
			{
				elapsed += Time.deltaTime;
				if (elapsed >= 0.8f)
				{
					Action done = complete;
					complete = null;
					base.gameObject.SetActive(false);
					done();
				}
				else
				{
					SetVerticesDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (!texture)
			{
				return;
			}
			foreach (Fragment p in parts)
			{
				if (!(elapsed > p.life))
				{
					Reference20Fragments.Region r = p.region;
					float angle = p.angle + p.spin * elapsed;
					Vector2 right = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r.w * p.size * 0.5f;
					Vector2 up = new Vector2(0f - Mathf.Sin(angle), Mathf.Cos(angle)) * r.h * p.size * 0.5f;
					Vector2 pos = p.start + p.velocity * elapsed + Vector2.down * 450f * elapsed * elapsed;
					Color tint = new Color(1f, 1f, 1f, 1f - Mathf.InverseLerp(p.life * 0.6f, p.life, elapsed));
					for (int i = 0; i < 4; i++)
					{
						quad[i] = UIVertex.simpleVert;
						quad[i].color = tint;
					}
					quad[0].position = pos - right - up;
					quad[1].position = pos - right + up;
					quad[2].position = pos + right + up;
					quad[3].position = pos + right - up;
					float x = (float)r.x / (float)texture.width;
					float y = (float)r.y / (float)texture.height;
					float x2 = (float)(r.x + r.w) / (float)texture.width;
					float y2 = (float)(r.y + r.h) / (float)texture.height;
					quad[0].uv0 = new Vector2(x, y);
					quad[1].uv0 = new Vector2(x, y2);
					quad[2].uv0 = new Vector2(x2, y2);
					quad[3].uv0 = new Vector2(x2, y);
					vh.AddUIVertexQuad(quad);
				}
			}
		}
	}
}

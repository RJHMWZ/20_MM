using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[RequireComponent(typeof(CanvasRenderer))]
	public sealed class Reference20CompanyParticleBatch : MaskableGraphic
	{
		private Texture2D texture;

		private string batchKey;

		private bool wasActive;

		private readonly UIVertex[] quad = new UIVertex[4];

		private readonly Dictionary<CompanyParticleEffect, CompanyParticleLayer[]> layersByEffect = new Dictionary<CompanyParticleEffect, CompanyParticleLayer[]>();

		public override Texture mainTexture => texture ? texture : Texture2D.whiteTexture;

		public Reference20CompanyParticleBatch()
		{
			base.useLegacyMeshGeneration = false;
		}

		public void Configure(CompanyParticleLayer layer)
		{
			texture = layer.image;
			batchKey = layer.batchKey;
			layersByEffect.Clear();
			material = layer.material;
			raycastTarget = false;
			base.maskable = false;
		}

		public CompanyParticleLayer[] Prepare(CompanyParticleEffect effect)
		{
			if (layersByEffect.TryGetValue(effect, out var layers))
			{
				return layers;
			}
			List<CompanyParticleLayer> matches = new List<CompanyParticleLayer>();
			CompanyParticleLayer[] layers2 = effect.layers;
			foreach (CompanyParticleLayer layer in layers2)
			{
				if (layer.batchKey == batchKey)
				{
					matches.Add(layer);
				}
			}
			layers = matches.ToArray();
			layersByEffect[effect] = layers;
			return layers;
		}

		private void LateUpdate()
		{
			bool any = false;
			foreach (Reference20CompanyParticleHandle h in Reference20CompanyParticles.Active)
			{
				if ((bool)h && h.gameObject.activeInHierarchy && Prepare(h.Definition).Length != 0)
				{
					any = true;
					break;
				}
			}
			if (any || wasActive)
			{
				SetVerticesDirty();
			}
			wasActive = any;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (!texture)
			{
				return;
			}
			foreach (Reference20CompanyParticleHandle h in Reference20CompanyParticles.Active)
			{
				if (!h || !h.gameObject.activeInHierarchy)
				{
					continue;
				}
				CompanyParticleLayer[] layers = Prepare(h.Definition);
				if (layers.Length == 0)
				{
					continue;
				}
				RectTransform emitter = (RectTransform)h.transform;
				Vector2 origin = emitter.anchoredPosition - base.rectTransform.anchoredPosition;
				Quaternion rotation = emitter.localRotation;
				Vector3 scale = emitter.localScale;
				Vector3 right = rotation * new Vector3(scale.x, 0f, 0f);
				Vector3 up = rotation * new Vector3(0f, scale.y, 0f);
				CompanyParticleLayer[] array = layers;
				foreach (CompanyParticleLayer layer in array)
				{
					int count = layer.tracks.Length;
					for (int particle = 0; particle < count; particle++)
					{
						CompanyParticleTrack track = layer.tracks[particle];
						float frame = (h.Elapsed - track.start) * h.SampleRate;
						if (frame < 0f || frame > (float)(track.samples.Length - 1))
						{
							continue;
						}
						int a = (int)frame;
						int b = Mathf.Min(a + 1, track.samples.Length - 1);
						float f = frame - (float)a;
						float[] va = track.samples[a];
						float[] vb = track.samples[b];
						if (!track.ValidSample(a) || !track.ValidSample(b))
						{
							continue;
						}
						Color tint = new Color(Mathf.LerpUnclamped(va[0], vb[0], f), Mathf.LerpUnclamped(va[1], vb[1], f), Mathf.LerpUnclamped(va[2], vb[2], f), Mathf.LerpUnclamped(va[3], vb[3], f));
						if (tint.a < 0.002f)
						{
							continue;
						}
						int vertices = (va.Length - 4) / 4;
						if (vertices == 4 && vh.currentVertCount <= 64000)
						{
							for (int k = 0; k < 4; k++)
							{
								int l = 4 + k * 4;
								quad[k] = UIVertex.simpleVert;
								float x = Mathf.LerpUnclamped(va[l], vb[l], f) + h.Velocity.x * track.start;
								float y = Mathf.LerpUnclamped(va[l + 1], vb[l + 1], f) + h.Velocity.y * track.start;
								quad[k].position = new Vector3(origin.x + right.x * x + up.x * y, origin.y + right.y * x + up.y * y, right.z * x + up.z * y);
								quad[k].uv0 = new Vector2(va[l + 2] * layer.tiling[0] + layer.tiling[2], va[l + 3] * layer.tiling[1] + layer.tiling[3]);
								quad[k].color = tint;
							}
							int offset = vh.currentVertCount;
							for (int j = 0; j < 4; j++)
							{
								vh.AddVert(quad[j]);
							}
							for (int i = 0; i + 2 < layer.indices.Length; i += 3)
							{
								vh.AddTriangle(offset + layer.indices[i], offset + layer.indices[i + 1], offset + layer.indices[i + 2]);
							}
						}
					}
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
	}
}

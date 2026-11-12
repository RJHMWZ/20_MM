using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[RequireComponent(typeof(CanvasRenderer))]
	public sealed class Reference20ParticleGraphic : MaskableGraphic
	{
		private struct Particle
		{
			public Vector2 position;

			public Vector2 velocity;

			public Vector2 origin;

			public float age;

			public float life;

			public float startSize;

			public float endSize;

			public float startSpin;

			public float endSpin;

			public float radial;

			public float tangential;

			public float angle;

			public float radius;

			public float endRadius;

			public float angular;

			public Color startColor;

			public Color endColor;
		}

		private Reference20ParticleData data;

		private Sprite sprite;

		private readonly List<Particle> particles = new List<Particle>();

		private static int seedCounter;

		private readonly System.Random random = new System.Random(Environment.TickCount * 397 + ++seedCounter);

		private float elapsed;

		private float emitted;

		private readonly UIVertex[] quad = new UIVertex[4];

		public override Texture mainTexture => sprite ? sprite.texture : Texture2D.whiteTexture;

		public void Configure(Reference20ParticleData value, Sprite texture)
		{
			data = value;
			sprite = texture;
			raycastTarget = false;
			particles.Capacity = Mathf.Max(particles.Capacity, value.totalParticles);
			Restart();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Restart();
		}

		public void Restart()
		{
			particles.Clear();
			elapsed = (emitted = 0f);
			SetVerticesDirty();
		}

		private float V(float value, float variance)
		{
			return value + ((float)random.NextDouble() * 2f - 1f) * variance;
		}

		private Color C(float[] value, float[] variance)
		{
			return new Color(Mathf.Clamp01(V(value[0], variance[0])), Mathf.Clamp01(V(value[1], variance[1])), Mathf.Clamp01(V(value[2], variance[2])), Mathf.Clamp01(V(value[3], variance[3])));
		}

		private void Emit()
		{
			float life = Mathf.Max(0.001f, V(data.life, data.lifeVar));
			float angle = V(data.angle, data.angleVar) * (3.14159265f / 180f);
			Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * V(data.speed, data.speedVar);
			Vector2 origin = data.sourcePos + new Vector2(V(0f, data.posVar.x), V(0f, data.posVar.y));
			float size = Mathf.Max(0f, V(data.startSize, data.startSizeVar));
			float radius = V(data.startRadius, data.startRadiusVar);
			particles.Add(new Particle
			{
				life = life,
				position = origin,
				origin = origin,
				velocity = velocity,
				startSize = size,
				endSize = ((data.endSize == -1f) ? size : Mathf.Max(0f, V(data.endSize, data.endSizeVar))),
				startSpin = (data.rotationIsDir ? ((0f - angle) * 57.29578f) : V(data.startSpin, data.startSpinVar)),
				endSpin = V(data.endSpin, data.endSpinVar),
				radial = V(data.radialAccel, data.radialAccelVar),
				tangential = V(data.tangentialAccel, data.tangentialAccelVar),
				startColor = C(data.startColor, data.startColorVar),
				endColor = C(data.endColor, data.endColorVar),
				angle = angle,
				radius = radius,
				endRadius = ((data.endRadius == -1f) ? radius : V(data.endRadius, data.endRadiusVar)),
				angular = V(data.rotatePerS, data.rotatePerSVar) * (3.14159265f / 180f)
			});
		}

		private void Update()
		{
			if (data == null)
			{
				return;
			}
			float dt = Time.deltaTime;
			float emissionTime = ((data.duration < 0f) ? dt : Mathf.Min(dt, Mathf.Max(0f, data.duration - elapsed)));
			elapsed += dt;
			emitted += emissionTime * data.emissionRate;
			while (emitted >= 1f)
			{
				emitted -= 1f;
				if (particles.Count < data.totalParticles)
				{
					Emit();
				}
			}
			for (int i = particles.Count - 1; i >= 0; i--)
			{
				Particle p = particles[i];
				p.age += dt;
				if (p.age >= p.life)
				{
					particles.RemoveAt(i);
				}
				else
				{
					if (data.emitterMode == 0)
					{
						Vector2 radial = (p.position - p.origin).normalized;
						Vector2 tangential = new Vector2(0f - radial.y, radial.x);
						p.velocity += (data.gravity + radial * p.radial + tangential * p.tangential) * dt;
						p.position += p.velocity * dt;
					}
					else
					{
						p.angle += p.angular * dt;
						float radius = Mathf.Lerp(p.radius, p.endRadius, p.age / p.life);
						p.position = p.origin - new Vector2(Mathf.Cos(p.angle), Mathf.Sin(p.angle)) * radius;
					}
					particles[i] = p;
				}
			}
			SetVerticesDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (data == null || !sprite)
			{
				return;
			}
			Rect rect = sprite.textureRect;
			Vector4 uv = new Vector4(rect.xMin / (float)mainTexture.width, rect.yMin / (float)mainTexture.height, rect.xMax / (float)mainTexture.width, rect.yMax / (float)mainTexture.height);
			foreach (Particle p in particles)
			{
				float t = p.age / p.life;
				float size = Mathf.Lerp(p.startSize, p.endSize, t) * 0.5f;
				float spin = (0f - Mathf.Lerp(p.startSpin, p.endSpin, t)) * (3.14159265f / 180f);
				Vector2 right = new Vector2(Mathf.Cos(spin), Mathf.Sin(spin)) * size;
				Vector2 up = new Vector2(0f - Mathf.Sin(spin), Mathf.Cos(spin)) * size;
				Color32 tint = Color.Lerp(p.startColor, p.endColor, t) * color;
				for (int i = 0; i < 4; i++)
				{
					quad[i] = UIVertex.simpleVert;
					quad[i].color = tint;
				}
				quad[0].position = p.position - right - up;
				quad[1].position = p.position - right + up;
				quad[2].position = p.position + right + up;
				quad[3].position = p.position + right - up;
				quad[0].uv0 = new Vector2(uv.x, uv.y);
				quad[1].uv0 = new Vector2(uv.x, uv.w);
				quad[2].uv0 = new Vector2(uv.z, uv.w);
				quad[3].uv0 = new Vector2(uv.z, uv.y);
				vh.AddUIVertexQuad(quad);
			}
		}
	}
}

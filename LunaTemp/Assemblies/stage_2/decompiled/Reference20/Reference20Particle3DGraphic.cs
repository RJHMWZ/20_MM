using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[RequireComponent(typeof(CanvasRenderer))]
	public sealed class Reference20Particle3DGraphic : MaskableGraphic
	{
		private struct Particle
		{
			public Vector3 position;

			public Vector3 velocity;

			public Vector3 actualVelocity;

			public float age;

			public float life;

			public float size;

			public float rotation;

			public float random;

			public Color color;
		}

		private readonly List<Particle> particles = new List<Particle>();

		private static int seedCounter;

		private readonly System.Random random = new System.Random(Environment.TickCount * 397 + ++seedCounter);

		private readonly UIVertex[] quad = new UIVertex[4];

		private CocosParticle3DData data;

		private Sprite sprite;

		private float time;

		private float emission;

		private float startDelay;

		private Vector3 lastPosition;

		public override Texture mainTexture => sprite ? sprite.texture : Texture2D.whiteTexture;

		private float Random01()
		{
			return (float)random.NextDouble();
		}

		public void Configure(CocosParticle3DData value, Sprite image)
		{
			data = value;
			sprite = image;
			raycastTarget = false;
			particles.Capacity = Mathf.Max(particles.Capacity, value.capacity);
			Restart();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Restart();
		}

		public void Restart()
		{
			time = (emission = 0f);
			particles.Clear();
			lastPosition = base.transform.position;
			if (data != null)
			{
				startDelay = data.startDelay.Evaluate(0f, Random01());
				if (data.prewarm && data.loop)
				{
					int steps = Mathf.Min(120, Mathf.CeilToInt(data.duration * 30f));
					for (int i = 0; i < steps; i++)
					{
						Advance(1f / 30f);
					}
				}
			}
			SetVerticesDirty();
		}

		private void Emit(float normalizedTime)
		{
			if (particles.Count < data.capacity)
			{
				float r = Random01();
				Vector3 position = Vector3.zero;
				Vector3 direction = Vector3.forward;
				if (data.samples != null && data.samples.Length != 0)
				{
					float[] sample = data.samples[random.Next(data.samples.Length)];
					position = new Vector3(sample[0], sample[1], sample[2]);
					direction = new Vector3(sample[3], sample[4], sample[5]);
				}
				particles.Add(new Particle
				{
					position = position,
					velocity = direction * data.startSpeed.Evaluate(normalizedTime, r),
					life = Mathf.Max(0.001f, data.startLifetime.Evaluate(normalizedTime, r)),
					size = Mathf.Max(0f, data.startSize.Evaluate(normalizedTime, r)),
					rotation = data.startRotation.Evaluate(normalizedTime, r),
					random = r,
					color = data.color.Evaluate(normalizedTime, r)
				});
			}
		}

		private void Update()
		{
			if (data != null)
			{
				Advance(Time.deltaTime * data.speed);
				SetVerticesDirty();
			}
		}

		private void Advance(float dt)
		{
			float before = time - startDelay;
			time += dt;
			float after = time - startDelay;
			float duration = Mathf.Max(0.001f, data.duration);
			if (after >= 0f && (data.loop || before < duration))
			{
				float end = (data.loop ? after : Mathf.Min(after, duration));
				float begin = Mathf.Max(0f, before);
				float normalized = (data.loop ? (Mathf.Repeat(end, duration) / duration) : Mathf.Clamp01(end / duration));
				emission += Mathf.Max(0f, end - begin) * data.rateOverTime.Evaluate(normalized, Random01());
				if (data.rateOverDistance != null)
				{
					emission += Vector3.Distance(lastPosition, base.transform.position) * data.rateOverDistance.Evaluate(normalized, Random01());
				}
				lastPosition = base.transform.position;
				while (emission >= 1f)
				{
					emission -= 1f;
					Emit(normalized);
				}
				if (data.bursts != null)
				{
					CocosParticleBurst[] bursts = data.bursts;
					foreach (CocosParticleBurst burst in bursts)
					{
						int first = (data.loop ? Mathf.Max(0, Mathf.FloorToInt(begin / duration)) : 0);
						int last = (data.loop ? Mathf.FloorToInt(end / duration) : 0);
						for (int cycle = first; cycle <= last; cycle++)
						{
							for (int repeat = 0; repeat < Mathf.Max(1, burst.repeat); repeat++)
							{
								float at = (float)cycle * duration + burst.time + (float)repeat * burst.interval;
								if (at >= Mathf.Max(0f, before) && at < after)
								{
									int count = Mathf.Max(0, Mathf.RoundToInt(burst.count.Evaluate(normalized, Random01())));
									for (int j = 0; j < count; j++)
									{
										Emit(normalized);
									}
								}
							}
						}
					}
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
					float life = p.age / p.life;
					p.velocity += Vector3.down * (9.8f * data.gravityModifier.Evaluate(life, p.random) * dt);
					CocosParticleModule limit = data.limitVelocityOvertime;
					Vector3 velocity = p.velocity;
					if (limit != null)
					{
						if (limit.separateAxes)
						{
							velocity.x = Dampen(velocity.x, limit.limitX.Evaluate(life, p.random), limit.dampen);
							velocity.y = Dampen(velocity.y, limit.limitY.Evaluate(life, p.random), limit.dampen);
							velocity.z = Dampen(velocity.z, limit.limitZ.Evaluate(life, p.random), limit.dampen);
						}
						else
						{
							float max = ((limit.limit == null) ? float.PositiveInfinity : Mathf.Max(0f, limit.limit.Evaluate(life, p.random)));
							if (velocity.magnitude > max)
							{
								velocity = Vector3.LerpUnclamped(velocity, velocity.normalized * max, limit.dampen);
							}
						}
					}
					p.actualVelocity = velocity;
					p.position += velocity * dt;
					CocosParticleModule rotation = data.rotationOvertime;
					if (rotation != null && rotation.z != null)
					{
						p.rotation += rotation.z.Evaluate(life, p.random) * dt;
					}
					particles[i] = p;
				}
			}
		}

		private static float Dampen(float value, float max, float dampen)
		{
			return (Mathf.Abs(value) > max) ? Mathf.LerpUnclamped(value, Mathf.Sign(value) * max, dampen) : value;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (data == null || !sprite)
			{
				return;
			}
			CocosParticleModule sheet = data.textureAnimation;
			int columns = ((sheet == null) ? 1 : Mathf.Max(1, sheet.numTilesX));
			int rows = ((sheet == null) ? 1 : Mathf.Max(1, sheet.numTilesY));
			Rect rect = sprite.textureRect;
			foreach (Particle p in particles)
			{
				float life = p.age / p.life;
				Vector2 size = Vector2.one * p.size;
				CocosParticleModule sizes = data.sizeOvertime;
				if (sizes != null)
				{
					if (sizes.separateAxes)
					{
						size = new Vector2(size.x * sizes.x.Evaluate(life, p.random), size.y * sizes.y.Evaluate(life, p.random));
					}
					else
					{
						size *= sizes.size.Evaluate(life, p.random);
					}
				}
				float angle = p.rotation;
				Vector3 position = p.position;
				Vector3 scale = base.transform.lossyScale;
				Vector3 worldVelocity = base.transform.TransformVector(p.actualVelocity);
				if (data.renderMode == 1)
				{
					Vector3 actualVelocity = p.actualVelocity;
					if (actualVelocity.sqrMagnitude > 1E-05f)
					{
						angle = Mathf.Atan2(worldVelocity.y, worldVelocity.x) - 3.14159265f / 2f;
						float num = size.y * data.lengthScale;
						actualVelocity = p.actualVelocity;
						size.y = num + actualVelocity.magnitude * data.velocityScale;
					}
				}
				Vector3 right = base.transform.InverseTransformVector(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * size.x * 0.5f * Mathf.Abs(scale.x));
				Vector3 up = base.transform.InverseTransformVector(new Vector3(0f - Mathf.Sin(angle), Mathf.Cos(angle), 0f) * size.y * 0.5f * Mathf.Abs(scale.y));
				Color tint = p.color * color;
				if (data.colorOverLifetime != null)
				{
					tint *= data.colorOverLifetime.color.Evaluate(life, p.random);
				}
				int frame = 0;
				if (sheet != null && sheet.frameOverTime != null)
				{
					float value = sheet.frameOverTime.Evaluate(life, p.random) * sheet.cycleCount;
					if (sheet.startFrame != null)
					{
						value += sheet.startFrame.Evaluate(0f, p.random) / (float)(columns * rows);
					}
					frame = Mathf.FloorToInt(Mathf.Repeat(value, 1f) * (float)columns * (float)rows);
				}
				int col = frame % columns;
				int row = frame / columns;
				Vector4 uv = new Vector4((rect.x + (float)col * rect.width / (float)columns) / (float)mainTexture.width, (rect.y + (float)(rows - row - 1) * rect.height / (float)rows) / (float)mainTexture.height, (rect.x + (float)(col + 1) * rect.width / (float)columns) / (float)mainTexture.width, (rect.y + (float)(rows - row) * rect.height / (float)rows) / (float)mainTexture.height);
				for (int i = 0; i < 4; i++)
				{
					quad[i] = UIVertex.simpleVert;
					quad[i].color = tint;
				}
				quad[0].position = position - right - up;
				quad[1].position = position - right + up;
				quad[2].position = position + right + up;
				quad[3].position = position + right - up;
				quad[0].uv0 = new Vector2(uv.x, uv.y);
				quad[1].uv0 = new Vector2(uv.x, uv.w);
				quad[2].uv0 = new Vector2(uv.z, uv.w);
				quad[3].uv0 = new Vector2(uv.z, uv.y);
				vh.AddUIVertexQuad(quad);
			}
		}
	}
}

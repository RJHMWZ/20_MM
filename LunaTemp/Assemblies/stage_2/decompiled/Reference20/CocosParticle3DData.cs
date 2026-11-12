using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CocosParticle3DData
	{
		public float duration;

		public float speed = 1f;

		public float velocityScale;

		public float lengthScale;

		public int capacity;

		public int renderMode;

		public int src = 770;

		public int dst = 771;

		public bool loop;

		public bool prewarm;

		public string sprite;

		public float[] tint;

		public float[] tiling;

		public Vector3 rotation;

		public float[][] samples;

		public CocosParticleBurst[] bursts;

		public CocosParticleRange startDelay;

		public CocosParticleRange startLifetime;

		public CocosParticleRange startSize;

		public CocosParticleRange startSpeed;

		public CocosParticleRange startRotation;

		public CocosParticleRange gravityModifier;

		public CocosParticleRange rateOverTime;

		public CocosParticleRange rateOverDistance;

		public CocosParticleGradient color;

		public CocosParticleModule colorOverLifetime;

		public CocosParticleModule sizeOvertime;

		public CocosParticleModule limitVelocityOvertime;

		public CocosParticleModule rotationOvertime;

		public CocosParticleModule textureAnimation;
	}
}

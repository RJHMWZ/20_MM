using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class Reference20ParticleData
	{
		public int totalParticles;

		public int emitterMode;

		public int positionType;

		public float duration;

		public float emissionRate;

		public float life;

		public float lifeVar;

		public float angle;

		public float angleVar;

		public float startSize;

		public float startSizeVar;

		public float endSize;

		public float endSizeVar;

		public float startSpin;

		public float startSpinVar;

		public float endSpin;

		public float endSpinVar;

		public float speed;

		public float speedVar;

		public float radialAccel;

		public float radialAccelVar;

		public float tangentialAccel;

		public float tangentialAccelVar;

		public float startRadius;

		public float startRadiusVar;

		public float endRadius;

		public float endRadiusVar;

		public float rotatePerS;

		public float rotatePerSVar;

		public bool rotationIsDir;

		public Vector2 posVar;

		public Vector2 sourcePos;

		public Vector2 gravity;

		public float[] startColor;

		public float[] startColorVar;

		public float[] endColor;

		public float[] endColorVar;

		public string sprite;
	}
}

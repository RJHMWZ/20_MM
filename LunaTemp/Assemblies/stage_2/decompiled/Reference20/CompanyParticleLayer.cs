using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CompanyParticleLayer
	{
		public string name;

		public string shader;

		public int texture;

		public int[] indices;

		public float[] tint;

		public float[] tiling;

		public float[][] vertices;

		public CompanyParticleTrack[] tracks;

		[NonSerialized]
		public Texture2D image;

		[NonSerialized]
		public bool additive;

		[NonSerialized]
		public Color color;

		[NonSerialized]
		public Material material;

		[NonSerialized]
		public string batchKey;
	}
}

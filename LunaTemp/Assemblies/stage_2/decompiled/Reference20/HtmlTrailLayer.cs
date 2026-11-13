using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class HtmlTrailLayer
	{
		public string name;

		public string shader;

		public int texture;

		public int blend = 1;

		public float[] tint;

		public float[] tiling;

		public float[] flipbook;

		public float[] uvSpeed;

		public float switchValue;

		public float frame = 1f;

		public HtmlTrailFrame[] frames;

		[NonSerialized]
		public Material material;

		[NonSerialized]
		public Texture2D image;
	}
}

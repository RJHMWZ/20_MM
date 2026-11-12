using System;

namespace Reference20
{
	[Serializable]
	public class NativeNode
	{
		public int src = 770;

		public int dst = 771;

		public int spriteSizeMode;

		public string name;

		public string sprite;

		public string skeleton;

		public string anim;

		public string skin;

		public float x;

		public float y;

		public float sx = 1f;

		public float sy = 1f;

		public float angle;

		public float w;

		public float h;

		public float ax = 0.5f;

		public float ay = 0.5f;

		public float opacity = 255f;

		public float timeScale = 1f;

		public float fill;

		public float[] color;

		public NativeSlot[] slots;

		public bool active = true;

		public bool loop;

		public bool autoplay;

		public bool mask;

		public bool inverted;

		public NativeNode[] children;

		public string[] animations;

		public NativeClip[] clips;

		public Reference20ParticleData particle;

		public CocosParticle3DData particle3d;
	}
}

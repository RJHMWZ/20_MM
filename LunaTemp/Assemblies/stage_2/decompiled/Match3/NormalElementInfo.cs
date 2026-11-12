using System;
using UnityEngine;

namespace Match3
{
	[Serializable]
	public class NormalElementInfo
	{
		[Tooltip("Unique ID for this normal element (e.g., 1 for Red, 2 for Blue)")]
		public int id;

		public Sprite sprite;

		[Tooltip("Prefab for the particle effect when this element is cleared")]
		public GameObject clearEffectPrefab;

		[Tooltip("Size multiplier for the clear particle effect (1 = matches board cell size)")]
		public float clearEffectScale = 1f;

		[Tooltip("Optional: Prefab for the light wave effect when this element is cleared")]
		public GameObject lightWaveEffectPrefab;

		[Tooltip("Size multiplier for the light wave effect (1 = matches board cell size)")]
		public float lightWaveEffectScale = 1f;

		[Tooltip("Spine skin name for this element color")]
		public string spineSkinName;
	}
}

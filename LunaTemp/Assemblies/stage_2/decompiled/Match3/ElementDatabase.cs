using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "ElementDatabase", menuName = "Match3/Element Database")]
	public class ElementDatabase : ScriptableObject
	{
		[Header("Unified Normal Element Prefab")]
		[Tooltip("All normal elements will use this single prefab. Their visuals will be swapped at runtime based on the sprite config.")]
		public GameObject unifiedNormalPrefab;

		[Header("Global Effects")]
		[Tooltip("Optional: Global light wave effect spawned when ANY normal element is cleared")]
		public GameObject globalLightWaveEffectPrefab;

		[Header("Normal Elements Config")]
		public List<NormalElementInfo> normalElements = new List<NormalElementInfo>();

		[Header("Special Elements Config")]
		public List<SpecialElementData> specialElements = new List<SpecialElementData>();

		[Header("Collectibles")]
		public List<CollectibleElementData> collectibles = new List<CollectibleElementData>();

		[Header("Obstacles")]
		public List<ObstacleData> obstacles = new List<ObstacleData>();

		[Header("Generators")]
		public List<GeneratorData> generators = new List<GeneratorData>();

		[Header("Mother & Child")]
		public List<MotherData> mothers = new List<MotherData>();

		public List<ChildData> children = new List<ChildData>();

		public NormalElementInfo GetNormalElement(int id)
		{
			return normalElements.Find((NormalElementInfo e) => e.id == id);
		}
	}
}

using UnityEngine;
using System.Collections.Generic;

namespace Match3
{
    [System.Serializable]
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
            return normalElements.Find(e => e.id == id);
        }
    }
}

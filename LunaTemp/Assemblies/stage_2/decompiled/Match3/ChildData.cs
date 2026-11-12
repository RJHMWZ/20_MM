using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "NewChildElement", menuName = "Match3/Child Element Data")]
	public class ChildData : ScriptableObject
	{
		public string childId = "Apple";

		[Tooltip("Must match the motherId of the target MotherElement")]
		public string targetMotherId = "Basket";

		public GameObject prefab;

		[Tooltip("If true, this child element can be swapped with normal elements.")]
		public bool isSwappable = false;

		[Header("Audio & Effects")]
		public AudioClip flySound;

		public GameObject collectEffectPrefab;
	}
}

using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "NewSpecialElement", menuName = "Match3/Special Element Data")]
	public class SpecialElementData : ScriptableObject
	{
		[Header("Basic Info")]
		public string elementId;

		[Header("Visuals")]
		public Sprite sprite;

		public GameObject prefab;

		[Header("Mechanics")]
		[Tooltip("Behavior Type")]
		public SpecialBehaviorType behaviorType;

		[Tooltip("Creation Condition")]
		public CreationCondition creationCondition;

		[Header("Effects")]
		[Tooltip("Optional: Effect spawned when this special element activates (e.g. Bomb explosion)")]
		public GameObject activationEffectPrefab;

		[Tooltip("Optional: Trail effect for moving elements (e.g. Rocket trail, Color ball beam)")]
		public GameObject trailEffectPrefab;

		[Tooltip("Optional: Final particle effect spawned when the performance ends (after the dim mask is removed, e.g. Magic ball finish burst)")]
		public GameObject finishEffectPrefab;
	}
}

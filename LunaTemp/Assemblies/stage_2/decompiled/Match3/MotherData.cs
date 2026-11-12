using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "NewMotherElement", menuName = "Match3/Mother Element Data")]
	public class MotherData : ScriptableObject
	{
		public string motherId = "Basket";

		public GameObject prefab;

		[Tooltip("Initial number required to collect")]
		public int requiredCount = 5;

		[Header("Grid Size")]
		[Tooltip("How many cells wide the Mother element is")]
		public int gridWidth = 3;

		[Tooltip("How many cells high the Mother element is")]
		public int gridHeight = 3;

		[Tooltip("If true, this mother element will fall down when there is empty space below it.")]
		public bool isMovable = false;

		[Tooltip("If true, this mother element can be swapped with normal elements. (Only recommended for 1x1 size)")]
		public bool isSwappable = false;

		[Header("Visual Offset")]
		[Tooltip("Fine-tune the local position offset of the visual elements (Spine, Sprites) relative to the calculated center")]
		public Vector2 visualOffset = Vector2.zero;

		[Header("Spine Animations")]
		public string idleAnimation = "idle";

		public string receiveAnimation = "receive";

		public string completeAnimation = "complete";

		[Header("Audio")]
		public AudioClip receiveSound;

		public AudioClip completeSound;

		[Tooltip("Icon used for level target UI")]
		public Sprite targetIcon;
	}
}

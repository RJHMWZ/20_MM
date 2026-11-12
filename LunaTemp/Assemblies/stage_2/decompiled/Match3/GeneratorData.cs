using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
	[CreateAssetMenu(fileName = "NewGenerator", menuName = "Match3/Generator Data")]
	public class GeneratorData : ScriptableObject
	{
		public string generatorId = "Typewriter";

		public GameObject prefab;

		[Tooltip("If true, this generator will fall down when there is empty space below it.")]
		public bool isMovable = false;

		[Tooltip("If true, this generator can be swapped with normal elements.")]
		public bool isSwappable = false;

		[Header("Visual Offset")]
		[Tooltip("Fine-tune the local position offset of the visual elements (Spine, Sprites) relative to the cell center")]
		public Vector2 visualOffset = Vector2.zero;

		[Header("Animations")]
		[Tooltip("The Spine animation name for the idle state")]
		public string idleAnimation = "idle";

		[Tooltip("The Spine animation name for the generating state (e.g. printing paper)")]
		public string workingAnimation = "work";

		[Header("Generated Item (Paper)")]
		[Tooltip("The item that pops out when triggered. Should contain its own Spine animations if needed.")]
		public GameObject generatedItemPrefab;

		[Tooltip("Icon used for level target UI and Level Editor preview")]
		public Sprite targetIcon;

		[Header("Audio")]
		[Tooltip("Sound played when the generator is triggered and pops out an item")]
		public AudioClip generateSound;

		[Header("Limit & Empty State")]
		[Tooltip("Maximum number of items this generator can produce. Set to -1 for infinite.")]
		public int maxGenerations = -1;

		[Tooltip("The Spine animation name for the empty/depleted state (if it has limited generations)")]
		public string emptyAnimation = "";

		[Header("Multi-State Progression")]
		[Tooltip("How many triggers/hits it takes to generate an item. Default is 1.")]
		public int hitsToGenerate = 1;

		[Tooltip("Spine skins for each state (from 0 hits to hitsToGenerate-1)")]
		public List<string> stateSkins;

		[Tooltip("Spine idle animations for each state. Index 0 = 0 hits, etc. Overrides default idleAnimation if set.")]
		public List<string> stateAnimations;

		[Tooltip("Animations to play when hit but not generating yet. Index 0 = transition from state 0 to 1, etc.")]
		public List<string> transitionAnimations;

		[Tooltip("Sound played when hit but not generating yet")]
		public AudioClip hitSound;
	}
}

using UnityEngine;
using System.Collections.Generic;

namespace Match3
{
    [CreateAssetMenu(fileName = "NewObstacle", menuName = "Match3/Obstacle Data")]
    public class ObstacleData : ScriptableObject
    {
        public string obstacleId = "WoodenBox";
        public GameObject prefab;
        
        [Tooltip("Number of hits required to destroy/collect the obstacle.")]
        public int maxHealth = 1;

        [Tooltip("If true, this obstacle can only be damaged by special effects (like Rockets, Bombs) and not by normal adjacent matches.")]
        public bool onlyDamagedBySpecials = false;
        
        [Tooltip("If true, this obstacle will fall down when there is empty space below it.")]
        public bool isMovable = false;

        [Tooltip("If true, this obstacle can be swapped with normal elements.")]
        public bool isSwappable = false;

        [Header("Visual Offset")]
        [Tooltip("Fine-tune the local position offset of the visual elements (Spine, Sprites) relative to the cell center")]
        public Vector2 visualOffset = Vector2.zero;

        [Header("State Effects")]
        [Tooltip("Effects spawned when the obstacle takes damage. Size should match maxHealth - 1")]
        public List<GameObject> damageEffects;
        
        [Tooltip("Effect spawned when the obstacle is fully destroyed/collected")]
        public GameObject collectEffect;
        
        [Header("Audio")]
        [Tooltip("Sound played when the obstacle takes damage but is not destroyed")]
        public AudioClip damageSound;
        
        [Tooltip("Sound played when the obstacle is fully destroyed/collected")]
        public AudioClip destroySound;

        [Header("Spine Animations (Optional)")]
        [Tooltip("Spine skin names for each health state. Index 0 = Full Health, Index 1 = 1 Hit Taken, etc. Use this instead of stateAnimations if the state changes visually via Skins.")]
        public List<string> stateSkins;

        [Tooltip("Spine animation names for each health state. Index 0 = Full Health, Index 1 = 1 Hit Taken, etc. Use this if the state changes visually via Animations.")]
        public List<string> stateAnimations;

        [Tooltip("Spine animation names for the transition when taking damage. Size should match maxHealth - 1")]
        public List<string> transitionAnimations;
    }
}

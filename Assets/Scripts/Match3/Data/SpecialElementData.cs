using UnityEngine;

namespace Match3
{
    public enum SpecialBehaviorType
    {
        LineClearHorizontal = 0, 
        BombExplosion = 1,       
        TargetSeeker = 2,        
        ColorClear = 3,          
        LineClearVertical = 4    
    }

    public enum CreationCondition
    {
        None = 0,
        Line4Horizontal = 1,
        CrossOrT = 2,   
        LShape4 = 3,    
        Line5 = 4,      
        Line4Vertical = 5,
        Square2x2 = 6,
        LShape6 = 7
    }

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
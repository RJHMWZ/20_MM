using UnityEngine;

namespace Match3
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [CreateAssetMenu(fileName = "NewCollectible", menuName = "Match3/Collectible Data")]
    public class CollectibleElementData : ScriptableObject
    {
        public string collectibleId = "Snake";
        public GameObject prefab;

        [Header("Visuals")]
        [Tooltip("显示在最前端的头部图片")]
        public Sprite headSprite;
        [Tooltip("中间身体的图片")]
        public Sprite bodySprite;
        [Tooltip("最后尾部的图片")]
        public Sprite tailSprite;
    }
}
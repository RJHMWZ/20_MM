using UnityEngine;
using UnityEditor;
using Match3;
using System.IO;

namespace Match3.EditorScripts
{
    public class SpecialElementGenerator
    {
        [MenuItem("Tools/Match3/Generate Default Special Elements")]
        public static void GenerateDefaults()
        {
            string absolutePath = EditorUtility.OpenFolderPanel("Select folder to save Special Elements", "Assets", "");
            if (string.IsNullOrEmpty(absolutePath)) return; // User canceled

            string folderPath = "Assets";
            if (absolutePath.StartsWith(Application.dataPath))
            {
                folderPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogWarning("Please select a folder inside the Assets folder.");
                return;
            }

            CreateSpecialElement(folderPath, "LineClear_Rocket_Horizontal", "火箭 (横向消除)", SpecialBehaviorType.LineClearHorizontal, CreationCondition.Line4Horizontal);
            CreateSpecialElement(folderPath, "LineClear_Rocket_Vertical", "火箭 (纵向消除)", SpecialBehaviorType.LineClearVertical, CreationCondition.Line4Vertical);
            CreateSpecialElement(folderPath, "Bomb_Explosion", "炸弹 (3x3爆炸)", SpecialBehaviorType.BombExplosion, CreationCondition.CrossOrT);
            CreateSpecialElement(folderPath, "TargetSeeker_Bird", "飞鸟 (追踪目标)", SpecialBehaviorType.TargetSeeker, CreationCondition.LShape4);
            CreateSpecialElement(folderPath, "ColorClear_MagicBall", "魔力球 (同色全消)", SpecialBehaviorType.ColorClear, CreationCondition.Line5);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("4 Default Special Elements generated at: " + folderPath);
        }

        private static void CreateSpecialElement(string folderPath, string assetName, string displayName, SpecialBehaviorType behavior, CreationCondition condition)
        {
            string path = $"{folderPath}/{assetName}.asset";
            SpecialElementData data = AssetDatabase.LoadAssetAtPath<SpecialElementData>(path);
            
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SpecialElementData>();
                data.elementId = displayName;
                data.behaviorType = behavior;
                data.creationCondition = condition;
                
                AssetDatabase.CreateAsset(data, path);
            }
            else
            {
                data.elementId = displayName;
                data.behaviorType = behavior;
                data.creationCondition = condition;
                EditorUtility.SetDirty(data);
            }
        }
    }
}
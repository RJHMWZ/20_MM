using UnityEngine;
using UnityEditor;
using BlockPuzzle.Views; // 引入原脚本的命名空间

namespace BlockPuzzle.EditorScripts
{
    [CustomEditor(typeof(OrientationActiveSwitcher))]
    public class OrientationActiveSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认的 Inspector 面板（显示 layouts 数组等属性）
            base.OnInspectorGUI();

            OrientationActiveSwitcher switcher = (OrientationActiveSwitcher)target;

            GUILayout.Space(15);
            EditorGUILayout.LabelField("🔧 布局记录与预览工具", EditorStyles.boldLabel);

            // ================= 记录功能 =================
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("⏺ 记录当前为【横屏】", GUILayout.Height(30)))
            {
                // 确保对数据的修改走的是序列化流程
                serializedObject.Update();
                
                switcher.RecordAsLandscape();
                
                // 将对象修改同步回序列化系统并标记变脏
                EditorUtility.SetDirty(switcher);
                PrefabUtility.RecordPrefabInstancePropertyModifications(switcher);
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("⏺ 记录当前为【竖屏】", GUILayout.Height(30)))
            {
                serializedObject.Update();
                
                switcher.RecordAsPortrait();
                
                EditorUtility.SetDirty(switcher);
                PrefabUtility.RecordPrefabInstancePropertyModifications(switcher);
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // ================= 预览功能 =================
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("▶ 预览【横屏】", GUILayout.Height(30)))
            {
                RecordTargetsUndo("Preview Landscape");
                switcher.PreviewLandscape();
            }

            if (GUILayout.Button("▶ 预览【竖屏】", GUILayout.Height(30)))
            {
                RecordTargetsUndo("Preview Portrait");
                switcher.PreviewPortrait();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 为所有目标物体记录 Undo 状态，以便预览后可以按 Ctrl+Z 恢复原状
        /// </summary>
        private void RecordTargetsUndo(string undoMessage)
        {
            // 通过序列化对象获取私有变量 layouts
            SerializedProperty layoutsProp = serializedObject.FindProperty("layouts");
            if (layoutsProp == null) return;

            for (int i = 0; i < layoutsProp.arraySize; i++)
            {
                SerializedProperty layoutItem = layoutsProp.GetArrayElementAtIndex(i);
                SerializedProperty targetProp = layoutItem.FindPropertyRelative("Target");
                
                if (targetProp != null && targetProp.objectReferenceValue != null)
                {
                    GameObject go = targetProp.objectReferenceValue as GameObject;
                    if (go != null)
                    {
                        // 记录 GameObject 激活状态和 Transform/RectTransform 状态
                        Undo.RecordObject(go, undoMessage);
                        Undo.RecordObject(go.transform, undoMessage);
                    }
                }
            }
        }
    }
}

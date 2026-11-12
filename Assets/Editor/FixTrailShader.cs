using UnityEngine;
using UnityEditor;
public class FixTrailShader {
    public static void Fix() {
        Shader newShader = Shader.Find("Particles/Standard Unlit");
        if (newShader == null) {
            Debug.LogError("Shader not found!");
            return;
        }
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Particles/Materials/trail1.mat");
        if (mat != null) {
            mat.shader = newShader;
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();
            Debug.Log("Material shader updated successfully.");
        } else {
            Debug.LogError("Material not found.");
        }
    }
}

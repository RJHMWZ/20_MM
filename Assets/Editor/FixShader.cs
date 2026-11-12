using UnityEngine;
using UnityEditor;
public class FixShader {
    public static void Fix() {
        Shader newShader = Shader.Find("XR/AddUV_Builtin");
        if (newShader == null) {
            Debug.LogError("Shader not found!");
            return;
        }
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Particles/Materials/FX_Teil1_uvadd_uvadd.mat");
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

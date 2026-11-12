using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PhotoPic : EditorWindow
{
    string path = "Assets/Resources/Screenshot/";
    string picName = "preview.jpg";
    [MenuItem("Tools/PhotoPic")]

    public static void ShowPic()
    {
        GetWindow<PhotoPic>("PhotoPic");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("截图"))
        {
            // 设置保存路径
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = path + picName;
            
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log("Screenshot saved to: " + filePath);
            
            CheckScreenshotSaved(filePath);
        }
    }

    private void CheckScreenshotSaved(string filePath)
    {
        EditorApplication.CallbackFunction checkFile = null;
        checkFile = () =>
        {
            if (File.Exists(filePath))
            {
                EditorApplication.update -= checkFile;
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("截图成功", "您已经截图成功！" + filePath, "确定");
            }
        };
        EditorApplication.update += checkFile;
    }
}

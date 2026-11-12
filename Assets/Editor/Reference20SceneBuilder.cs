using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.IO;
using Spine.Unity;
using System.Collections.Generic;
namespace Reference20 {
public static class Reference20SceneBuilder {
 [MenuItem("Reference20/Create Replica Scene")]
 public static void Create(){
  if(EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=false;EditorApplication.delayCall+=Create;return;}
  foreach(var path in AssetDatabase.FindAssets("",new[]{"Assets/Resources/Reference20"}).Select(AssetDatabase.GUIDToAssetPath)){
   var imp=AssetImporter.GetAtPath(path) as TextureImporter;if(imp==null)continue;
   imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.maxTextureSize=4096;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
  }
  string native="Assets/Resources/Reference20/Native/";
  var graphic=new Material(Shader.Find("Spine/SkeletonGraphic"));graphic.EnableKeyword("_STRAIGHT_ALPHA_INPUT");graphic.SetFloat("_StraightAlphaInput",1);
  if(!AssetDatabase.LoadAssetAtPath<Material>(native+"SpineGraphic.mat"))AssetDatabase.CreateAsset(graphic,native+"SpineGraphic.mat");
  foreach(var skeletonFile in Directory.GetFiles(native,"skeleton_*.json")){
   var name=Path.GetFileNameWithoutExtension(skeletonFile);
   var materials=new List<Material>();for(int i=0;i<8;i++){
    var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(native+name+"_tex"+i+".png");if(!tex)break;
    string path=native+name+"_tex"+i+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
    if(!mat){mat=new Material(Shader.Find("Spine/Skeleton"));AssetDatabase.CreateAsset(mat,path);}mat.name=name+"_tex"+i;mat.mainTexture=tex;EditorUtility.SetDirty(mat);materials.Add(mat);
   }
   string atlasPath=native+name+"_atlas.asset";var atlas=AssetDatabase.LoadAssetAtPath<SpineAtlasAsset>(atlasPath);
   if(!atlas){atlas=ScriptableObject.CreateInstance<SpineAtlasAsset>();AssetDatabase.CreateAsset(atlas,atlasPath);}
   atlas.atlasFile=AssetDatabase.LoadAssetAtPath<TextAsset>(native+name+".atlas.txt");atlas.materials=materials.ToArray();atlas.Clear();EditorUtility.SetDirty(atlas);
   string dataPath=native+name+"_data.asset";var data=AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(dataPath);
   if(!data){data=ScriptableObject.CreateInstance<SkeletonDataAsset>();AssetDatabase.CreateAsset(data,dataPath);}
   data.skeletonJSON=AssetDatabase.LoadAssetAtPath<TextAsset>(skeletonFile.Replace('\\','/'));data.atlasAssets=new AtlasAssetBase[]{atlas};data.scale=.01f*Reference20Native.CocosScale(name);data.Clear();EditorUtility.SetDirty(data);
  }

  EditorSceneManager.SaveOpenScenes();
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var camera=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener));camera.tag="MainCamera";var cam=camera.GetComponent<Camera>();cam.orthographic=true;cam.orthographicSize=9.6f;camera.transform.position=new Vector3(0,0,-10);cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.3f,.75f,.7f);
  var light=new GameObject("Directional Light",typeof(Light));light.GetComponent<Light>().type=LightType.Directional;
  var cg=new GameObject("Reference20Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));cg.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
  var scaler=cg.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
  var stage=new GameObject("Stage",typeof(RectTransform)).GetComponent<RectTransform>();stage.SetParent(cg.transform,false);stage.anchorMin=stage.anchorMax=new Vector2(.5f,.5f);stage.sizeDelta=new Vector2(1080,1920);
  new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
  var gm=new GameObject("Reference20Game");var template=gm.AddComponent<Match3.GameManager>();template.enabled=false;var controller=gm.AddComponent<Reference20Game>();controller.stage=stage;controller.templateDownload=template;
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.AutoRotation;
  PlayerSettings.allowedAutorotateToPortrait=true;PlayerSettings.allowedAutorotateToLandscapeLeft=true;PlayerSettings.allowedAutorotateToLandscapeRight=true;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
  Canvas.ForceUpdateCanvases();controller.Build();Canvas.ForceUpdateCanvases();cg.GetComponent<Reference20ResponsiveLayout>().Refresh();
  EditorSceneManager.SaveScene(scene,"Assets/Scenes/Reference20.unity");
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Reference20.unity",true)};
  AssetDatabase.SaveAssets();Debug.Log("Reference20 scene created. Reference layout: 9 columns, 12 rows, 9 moves. Download uses Match3.GameManager.DownloadGame.");
 }
}
}


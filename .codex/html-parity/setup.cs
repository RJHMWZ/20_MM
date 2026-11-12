var path="Assets/Resources/Reference20/Company/UI/";
foreach(var name in new[]{"Loading_Logo_en","Level_PopUp_1","Leve_Queen","EndDownloadLabel","Level_Easyl_PopUp_2","Background"}) {
 var imp=UnityEditor.AssetImporter.GetAtPath(path+name+".png") as UnityEditor.TextureImporter;
 imp.textureType=UnityEditor.TextureImporterType.Sprite;imp.spriteImportMode=UnityEditor.SpriteImportMode.Single;imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;
 if(name=="Level_Easyl_PopUp_2")imp.spriteBorder=new UnityEngine.Vector4(23,30,22,22);
 if(name=="Background")imp.spriteBorder=new UnityEngine.Vector4(9.973249f,9.948683f,10.026751f,10.051317f);
 imp.SaveAndReimport();
}
var game=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();
game.maxMoves=25;game.Build();
UnityEditor.EditorUtility.SetDirty(game);
UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
UnityEditor.AssetDatabase.SaveAssets();
UnityEngine.Debug.Log("HTML parity UI imported and scene rebuilt");
return "UI and scene saved";

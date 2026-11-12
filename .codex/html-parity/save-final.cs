var game=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();game.maxMoves=25;game.Build();
UnityEditor.EditorUtility.SetDirty(game);UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();UnityEditor.AssetDatabase.SaveAssets();
var errors=new System.Collections.Generic.List<string>();foreach(var r in Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(System.IO.File.ReadAllText(".codex/runtime-roots.json")))if(UnityEditor.AssetDatabase.LoadMainAssetAtPath(r)==null)errors.Add(r);
return new {missingResources=errors,game.maxMoves};

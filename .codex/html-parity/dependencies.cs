var roots=Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(System.IO.File.ReadAllText(".codex/runtime-roots.json"));
var closure=UnityEditor.AssetDatabase.GetDependencies(roots,true);
System.IO.File.WriteAllText(".codex/asset-keep.json",Newtonsoft.Json.JsonConvert.SerializeObject(closure));
var missing=new System.Collections.Generic.List<string>();foreach(var go in UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.GameObject>())if(go.scene.IsValid()&&UnityEditor.GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go)>0)missing.Add(go.name);
return new {closure=closure.Length,missing};

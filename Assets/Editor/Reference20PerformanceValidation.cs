using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Reference20;

[InitializeOnLoad]
public static class Reference20PerformanceValidation {
 const string DirectoryPath=".codex/performance";
 const string Request=DirectoryPath+"/particles.request";
 static Reference20PerformanceValidation(){EditorApplication.update+=Tick;}
 static void Tick(){if(EditorApplication.isCompiling||!File.Exists(Request))return;var mode=File.ReadAllText(Request).Trim();File.Delete(Request);Run(mode);}
 [Serializable] sealed class Snapshot {public string batch;public float time;public Vector3[] vertices;public Vector2[] uv;public Color32[] colors;public int[] triangles;}
 [Serializable] sealed class SnapshotSet {public Snapshot[] items;}
 public static void Run(string mode){
  Directory.CreateDirectory(DirectoryPath);var errors=new List<string>();var snapshots=new List<Snapshot>();double milliseconds=0;int builds=0;
  var root=new GameObject("ParticlePerformanceValidation",typeof(RectTransform),typeof(Canvas));root.hideFlags=HideFlags.HideAndDontSave;root.GetComponent<Canvas>().referencePixelsPerUnit=100;
  var mesh=new Mesh();
  try{
   Reference20CompanyParticles.Reset();var stage=(RectTransform)root.transform;stage.sizeDelta=new Vector2(1080,1920);
   string[] names={"NormalBlue","NormalGold","Wave","RocketTrail","ColorCharge","ColorFinish","Bomb","BeeImpact"};
   for(int i=0;i<24;i++){
    var rt=Reference20CompanyParticles.Play(names[i%names.Length],stage,new Vector2(85.6f+(i%9)*113.6f,1584.8f-(i%12)*113.6f));
    rt.localRotation=Quaternion.Euler(0,0,(i%4)*45);rt.localScale*=.7f+(i%3)*.2f;
   }
   var field=typeof(Reference20CompanyParticleBatch).GetField("batchKey",BindingFlags.Instance|BindingFlags.NonPublic);
   var batches=root.GetComponentsInChildren<Reference20CompanyParticleBatch>(true).OrderBy(b=>(string)field.GetValue(b),StringComparer.Ordinal).ToArray();
   var populate=typeof(Reference20CompanyParticleBatch).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null);
   foreach(float time in new[]{.1f,.3f,.6f,.9f}){
    foreach(var h in Reference20CompanyParticles.Active)h.Elapsed=time;
    foreach(var batch in batches){using(var vh=new VertexHelper()){populate.Invoke(batch,new object[]{vh});vh.FillMesh(mesh);}}
    var watch=Stopwatch.StartNew();
    for(int repeat=0;repeat<4;repeat++)foreach(var batch in batches){using(var vh=new VertexHelper()){populate.Invoke(batch,new object[]{vh});vh.FillMesh(mesh);builds++;}}
    watch.Stop();milliseconds+=watch.Elapsed.TotalMilliseconds;
    foreach(var batch in batches){
     using(var vh=new VertexHelper()){populate.Invoke(batch,new object[]{vh});vh.FillMesh(mesh);}
     snapshots.Add(new Snapshot{batch=(string)field.GetValue(batch),time=time,vertices=mesh.vertices,uv=mesh.uv,colors=mesh.colors32,triangles=mesh.triangles});
    }
   }
   var baselinePath=DirectoryPath+"/particle-baseline.json";
   if(mode=="baseline")File.WriteAllText(baselinePath,JsonUtility.ToJson(new SnapshotSet{items=snapshots.ToArray()}));
   else{
    var before=JsonUtility.FromJson<SnapshotSet>(File.ReadAllText(baselinePath)).items;
    if(before.Length!=snapshots.Count)errors.Add("Batch count changed");
    for(int i=0;i<Math.Min(before.Length,snapshots.Count);i++){
     var a=before[i];var b=snapshots[i];string label=b.batch+" @"+b.time;
     if(a.batch!=b.batch||a.time!=b.time||a.vertices.Length!=b.vertices.Length||!a.triangles.SequenceEqual(b.triangles)||!a.colors.SequenceEqual(b.colors)||a.uv.Length!=b.uv.Length){errors.Add("Geometry/color/topology changed: "+label);continue;}
     for(int j=0;j<a.vertices.Length;j++)if(Vector3.Distance(a.vertices[j],b.vertices[j])>.0003f){errors.Add("Vertex changed: "+label);break;}
     for(int j=0;j<a.uv.Length;j++)if(Vector2.Distance(a.uv[j],b.uv[j])>.000001f){errors.Add("UV changed: "+label);break;}
    }
   }
  }catch(Exception e){errors.Add(e.ToString());}finally{
   Reference20CompanyParticles.Reset();UnityEngine.Object.DestroyImmediate(mesh);UnityEngine.Object.DestroyImmediate(root);
  }
  if(mode!="baseline"){CheckTrailReuse(errors);CheckEventOrder(errors);CheckBatchActivity(errors);}
  File.WriteAllText(DirectoryPath+"/"+mode+".json",Newtonsoft.Json.JsonConvert.SerializeObject(new{mode,builds,milliseconds,snapshots=snapshots.Count,vertices=snapshots.Sum(s=>s.vertices.Length),errors}));
 }
 static void CheckTrailReuse(List<string> errors){
  var root=new GameObject("TrailReuseValidation",typeof(RectTransform),typeof(Canvas));root.hideFlags=HideFlags.HideAndDontSave;root.GetComponent<Canvas>().referencePixelsPerUnit=100;var mesh=new Mesh();
  try{
   Reference20ColorTrail.Reset();var stage=(RectTransform)root.transform;stage.sizeDelta=new Vector2(1080,1920);
   var from=new Vector2(312.8f,790.4f);var to=new Vector2(880.8f,1130.4f);
   Reference20ColorTrail.Play(stage,from,to);var first=root.GetComponentInChildren<Reference20ColorTrailLease>();
   var elapsed=typeof(Reference20ColorTrail).GetField("elapsed",BindingFlags.NonPublic|BindingFlags.Instance);
   var populate=typeof(Reference20ColorTrail).GetMethod("OnPopulateMesh",BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{typeof(VertexHelper)},null);
   var before=new List<Snapshot>();
   foreach(var layer in first.Layers){elapsed.SetValue(layer,.2f);using(var vh=new VertexHelper()){populate.Invoke(layer,new object[]{vh});vh.FillMesh(mesh);}before.Add(new Snapshot{vertices=mesh.vertices,uv=mesh.uv,colors=mesh.colors32,triangles=mesh.triangles});}
   int count=root.GetComponentsInChildren<Transform>(true).Length;first.Complete();
   Reference20ColorTrail.Play(stage,from,to);var second=root.GetComponentInChildren<Reference20ColorTrailLease>();
   if(first!=second)errors.Add("Trail did not reuse its renderers");
   for(int i=0;i<second.Layers.Length;i++){
    var layer=second.Layers[i];if((float)elapsed.GetValue(layer)!=0)errors.Add("Reused trail animation did not restart");
    elapsed.SetValue(layer,.2f);using(var vh=new VertexHelper()){populate.Invoke(layer,new object[]{vh});vh.FillMesh(mesh);}
    var a=before[i];if(!a.vertices.SequenceEqual(mesh.vertices)||!a.uv.SequenceEqual(mesh.uv)||!a.colors.SequenceEqual(mesh.colors32)||!a.triangles.SequenceEqual(mesh.triangles))errors.Add("Reused trail geometry changed");
   }
   second.Complete();
   for(int i=0;i<50;i++){Reference20ColorTrail.Play(stage,from+Vector2.one*i,to+Vector2.one*i);var lease=root.GetComponentInChildren<Reference20ColorTrailLease>();if(lease!=first)errors.Add("Sequential trails allocated new objects");lease.Complete();}
   if(root.GetComponentsInChildren<Transform>(true).Length!=count)errors.Add("Trail hierarchy grew during reuse");
  }catch(Exception e){errors.Add(e.ToString());}finally{Reference20ColorTrail.Reset();UnityEngine.Object.DestroyImmediate(mesh);UnityEngine.Object.DestroyImmediate(root);}
 }
 static void CheckEventOrder(List<string> errors){
  var root=new GameObject("EventOrderValidation");root.hideFlags=HideFlags.HideAndDontSave;
  try{
   var game=root.AddComponent<Reference20Game>();game.enabled=false;
   var flags=BindingFlags.Instance|BindingFlags.NonPublic;
   var after=typeof(Reference20Game).GetMethod("After",flags);var pop=typeof(Reference20Game).GetMethod("PopEvent",flags);
   var random=new System.Random(7);var scheduled=new List<KeyValuePair<float,int>>();var actual=new List<int>();
   for(int i=0;i<2048;i++){int ordinal=i;float delay=random.Next(0,64)*.05f;scheduled.Add(new KeyValuePair<float,int>(delay,i));after.Invoke(game,new object[]{delay,(Action)(()=>actual.Add(ordinal))});}
   for(int i=0;i<scheduled.Count;i++){var e=pop.Invoke(game,null);((Action)e.GetType().GetField("action").GetValue(e))();}
   var expected=scheduled.OrderBy(e=>e.Key).ThenBy(e=>e.Value).Select(e=>e.Value);
   if(!expected.SequenceEqual(actual))errors.Add("Scheduled effect order changed");
   // A callback may enqueue another event at the same time during dispatch.
   actual.Clear();after.Invoke(game,new object[]{0f,(Action)(()=>{actual.Add(1);after.Invoke(game,new object[]{0f,(Action)(()=>actual.Add(3))});})});after.Invoke(game,new object[]{0f,(Action)(()=>actual.Add(2))});
   for(int i=0;i<3;i++){var e=pop.Invoke(game,null);((Action)e.GetType().GetField("action").GetValue(e))();}
   if(!actual.SequenceEqual(new[]{1,2,3}))errors.Add("Nested effect callback order changed");
  }catch(Exception e){errors.Add(e.ToString());}finally{UnityEngine.Object.DestroyImmediate(root);}
 }
 static void CheckBatchActivity(List<string> errors){
  var root=new GameObject("BatchActivityValidation",typeof(RectTransform),typeof(Canvas));root.hideFlags=HideFlags.HideAndDontSave;
  try{
   Reference20CompanyParticles.Reset();var stage=(RectTransform)root.transform;
   var effect=Reference20CompanyParticles.Play("NormalBlue",stage,new Vector2(880.8f,1130.4f)).GetComponent<Reference20CompanyParticleHandle>();
   var batches=root.GetComponentsInChildren<Reference20CompanyParticleBatch>();var flags=BindingFlags.Instance|BindingFlags.NonPublic;
   var update=typeof(Reference20CompanyParticleBatch).GetMethod("LateUpdate",flags);var active=typeof(Reference20CompanyParticleBatch).GetField("wasActive",flags);
   int count=0;foreach(var batch in batches){update.Invoke(batch,null);if((bool)active.GetValue(batch))count++;}
   if(count!=effect.Definition.layers.Select(l=>l.batchKey).Distinct().Count()||count>=batches.Length)errors.Add("Unrelated particle batches still rebuild");
   effect.Stop();foreach(var batch in batches){update.Invoke(batch,null);if((bool)active.GetValue(batch))errors.Add("Inactive particle batch remains dirty");}
  }catch(Exception e){errors.Add(e.ToString());}finally{Reference20CompanyParticles.Reset();UnityEngine.Object.DestroyImmediate(root);}
 }
}

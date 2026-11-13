using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Match3.UI;
using Reference20;

// Checks production layout and pointer conversion using Unity's transform system.
[InitializeOnLoad]
public static class Reference20WebLayoutValidation {
 const string Request=".codex/html-parity/web-layout-check.request";
 static Reference20WebLayoutValidation(){EditorApplication.update+=Tick;}
 static void Tick(){if(EditorApplication.isCompiling||!File.Exists(Request))return;File.Delete(Request);Run();}
 [MenuItem("Reference20/Validate Web Layout")]
 public static void Run(){
  var errors=new List<string>();int checks=0;
  var root=new GameObject("LayoutValidation",typeof(RectTransform));root.hideFlags=HideFlags.HideAndDontSave;
  try{
   var canvas=(RectTransform)root.transform;canvas.pivot=Vector2.one*.5f;
   var board=new GameObject("Board",typeof(RectTransform)).GetComponent<RectTransform>();board.SetParent(canvas,false);
   var layout=root.AddComponent<Reference20ResponsiveLayout>();layout.board=board;
   foreach(var size in new[]{new Vector2(320,568),new Vector2(375,667),new Vector2(390,844),new Vector2(768,1024),new Vector2(844,390),new Vector2(1920,1080)}){
    canvas.sizeDelta=size;layout.Refresh();
    for(int y=0;y<12;y++)for(int x=0;x<9;x++){
     var expected=new Vector2(85.6f+x*113.6f,1584.8f-y*113.6f);
     var world=board.TransformPoint(expected-board.sizeDelta*.5f);
     Vector2 canvasPoint=canvas.InverseTransformPoint(world);
     var screen=new Vector2((canvasPoint.x+size.x*.5f)*Screen.width/size.x,(canvasPoint.y+size.y*.5f)*Screen.height/size.y);
     Vector2 actual;
     if(!layout.TryScreenToBoard(screen,out actual)||Vector2.Distance(expected,actual)>.01f)errors.Add("Pointer mismatch "+size+" cell "+x+","+y);
     checks++;
    }
   }
  }catch(Exception e){errors.Add(e.ToString());}finally{UnityEngine.Object.DestroyImmediate(root);}
  // Reproduce Luna's (0,0) default anchors, then exercise the real hint factory.
  // The old code's animated image shifted half a tile while the tile's root stayed put.
  var tile=new GameObject("HintValidation",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));tile.hideFlags=HideFlags.HideAndDontSave;
  try{
   var rt=(RectTransform)tile.transform;rt.pivot=Vector2.one*.5f;rt.sizeDelta=new Vector2(128,128);rt.localScale=Vector3.one*.8875f;
   tile.GetComponent<Image>().sprite=Resources.Load<Sprite>("Reference20/Company/Elements/brick2_0");
   var legacy=new GameObject("LegacyPresentation",typeof(RectTransform)).GetComponent<RectTransform>();legacy.SetParent(rt,false);legacy.anchorMin=legacy.anchorMax=Vector2.zero;legacy.sizeDelta=rt.sizeDelta;
   var oldOffset=rt.InverseTransformPoint(legacy.position);
   if(Vector2.Distance(oldOffset,new Vector2(-64,-64))>.01f)errors.Add("Luna anchor reproduction failed");checks++;
   UnityEngine.Object.DestroyImmediate(legacy.gameObject);
   var clip=Reference20Native.Hint(rt,"match_tip");var art=rt.Find("Presentation") as RectTransform;
   if(!clip||!art)errors.Add("Hint visual missing");
   else{
    foreach(var target in new[]{new Vector2(85.6f,1584.8f),new Vector2(880.8f,1130.4f),new Vector2(994.4f,1130.4f)}){
     rt.anchoredPosition=target;
     if(Vector2.Distance(rt.InverseTransformPoint(art.position),Vector2.zero)>.01f)errors.Add("Hint image offset after move: "+target);checks++;
    }
    clip.Stop();
    Reference20Native.Hint(rt,"match_tip_right");clip.Stop();
    if(rt.childCount!=1||Vector2.Distance(rt.InverseTransformPoint(art.position),Vector2.zero)>.01f)errors.Add("Repeated hint changed tile center");checks++;
   }
  }catch(Exception e){errors.Add(e.ToString());}finally{UnityEngine.Object.DestroyImmediate(tile);}
  RuntimeLoadingScreen loading=null;GameObject loadingRoot=null;
  try{
   loading=RuntimeLoadingScreen.Create();loadingRoot=(GameObject)typeof(RuntimeLoadingScreen).GetField("root",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(loading);
   loadingRoot.hideFlags=HideFlags.HideAndDontSave;Canvas.ForceUpdateCanvases();
   var logo=(RectTransform)loadingRoot.transform.Find("Loading_Logo_en");var track=(RectTransform)loadingRoot.transform.Find("ProgressTrack");
   float gap=logo.anchoredPosition.y-logo.rect.height*.5f-(track.anchoredPosition.y+track.rect.height*.5f);
   if(gap<23.9f)errors.Add("Loading logo/track gap: "+gap);checks++;
  }catch(Exception e){errors.Add(e.ToString());}finally{if(loadingRoot)UnityEngine.Object.DestroyImmediate(loadingRoot);}
  CheckAnimatedMeshesAndEffects(errors,ref checks);
  Directory.CreateDirectory(".codex/html-parity");
  File.WriteAllText(".codex/html-parity/web-layout-check.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{checks,errors,screen=new{Screen.width,Screen.height}}));
  Debug.Log("Web layout checks: "+checks+", errors: "+errors.Count);
 }
 static void CheckAnimatedMeshesAndEffects(List<string> errors,ref int checks){
  var root=new GameObject("EffectsValidation",typeof(RectTransform),typeof(Canvas));root.hideFlags=HideFlags.HideAndDontSave;
  var output=new Mesh();Reference20CompanyParticleHandle emitter=null;
  try{
   var stage=(RectTransform)root.transform;stage.sizeDelta=new Vector2(1080,1920);
   var geometry=typeof(Graphic).GetMethod("UpdateGeometry",BindingFlags.Instance|BindingFlags.NonPublic);
   var controlRect=Reference20Props.Rect("OriginalSpineGeometry",stage);var control=controlRect.gameObject.AddComponent<SkeletonGraphic>();
   control.skeletonDataAsset=Resources.Load<SkeletonDataAsset>("Reference20/Props/Bomb_SkeletonData");control.material=Resources.Load<Material>("Reference20/Native/SpineGraphic");control.Initialize(true);
   control.Skeleton.SetSkin("brick0_heng");control.Skeleton.SetSlotsToSetupPose();control.AnimationState.SetAnimation(0,"Bomb_hint",true);control.Update(.1f);control.LateUpdate();
   var controlMesh=control.canvasRenderer.GetMesh();float before=controlMesh?controlMesh.bounds.size.sqrMagnitude:0;
   geometry.Invoke(control,null);controlMesh=control.canvasRenderer.GetMesh();
   if(before<.001f||(controlMesh&&controlMesh.bounds.size.sqrMagnitude>.001f))errors.Add("Original UI geometry overwrite was not reproduced");checks++;
   UnityEngine.Object.DestroyImmediate(controlRect.gameObject);
   // Activation uses a dynamically-created nested Canvas, unlike idle pieces.
   // Match Luna's zero default to reproduce the invisible discharge animation.
   var overlay=Reference20Native.Overlay(stage);var effectCanvas=overlay.GetComponent<Canvas>();
   if(effectCanvas.referencePixelsPerUnit<=0)errors.Add("Activation canvas has zero pixels per unit");checks++;
   float rootDensity=root.GetComponent<Canvas>().referencePixelsPerUnit;
   effectCanvas.referencePixelsPerUnit=0;
   var discharge=Reference20Native.Effect("SameColorEffect",stage,new Vector2(880.8f,1130.4f));
   var dischargeRig=discharge.GetComponentInChildren<SkeletonGraphic>();dischargeRig.Update(.1f);dischargeRig.LateUpdate();
   var collapsed=dischargeRig.canvasRenderer.GetMesh();
   if(!collapsed||collapsed.bounds.size.sqrMagnitude>.001f)errors.Add("Luna zero-scale discharge was not reproduced");checks++;
   // Unity's native nested Canvas setter also affects the root. Restore the
   // parent separately; Luna's zero field is local to the new child Canvas.
   root.GetComponent<Canvas>().referencePixelsPerUnit=rootDensity;
   Reference20Native.SortingCanvas(overlay,200);
   foreach(float delta in new[]{.1f,.3f,.7f}){
    dischargeRig.Update(delta);dischargeRig.LateUpdate();geometry.Invoke(dischargeRig,null);var mesh=dischargeRig.canvasRenderer.GetMesh();
    if(!mesh||mesh.bounds.size.sqrMagnitude<.001f)errors.Add("Five-match activation animation collapsed; canvas="+dischargeRig.canvas.name+" ppu="+dischargeRig.canvas.referencePixelsPerUnit+" root="+root.GetComponent<Canvas>().referencePixelsPerUnit+" nested="+effectCanvas.referencePixelsPerUnit);checks++;
   }
   Reference20Native.FinishEffect(discharge);
   foreach(string effect in new[]{"SameColorEffect","SameColorChangeEffect","AreaAreaEffect"}){
    var activation=Reference20Native.Effect(effect,stage,new Vector2(880.8f,1130.4f));
    foreach(var sg in activation.GetComponentsInChildren<SkeletonGraphic>()){
     foreach(float delta in new[]{.1f,.3f,.7f}){
      sg.Update(delta);sg.LateUpdate();geometry.Invoke(sg,null);var mesh=sg.canvasRenderer.GetMesh();
      if(!mesh||mesh.bounds.size.sqrMagnitude<.001f)errors.Add("Invisible activation effect: "+effect);checks++;
     }
    }
    Reference20Native.FinishEffect(activation);
   }
   foreach(int id in new[]{11,12,13,14,15}){
    var visual=Reference20Props.Visual(id,stage,false);var actor=visual.GetComponent<Reference20PropVisual>();
    foreach(string animation in new[]{"Bomb_hint","Bomb_hint_r","falling_d","Bomb_dropped_d",Reference20Props.Creation(id)}){
     actor.Rig.AnimationState.ClearTracks();actor.Rig.Skeleton.SetToSetupPose();actor.Rig.AnimationState.SetAnimation(0,animation,false);
     foreach(float delta in new[]{.1f,.3f,.7f}){
      actor.Rig.Update(delta);actor.Rig.LateUpdate();geometry.Invoke(actor.Rig,null);
      var rendered=actor.Rig.canvasRenderer.GetMesh();
      if(!rendered||rendered.vertexCount==0||rendered.bounds.size.sqrMagnitude<.001f)errors.Add("UI rebuild erased special mesh: "+id+"/"+animation);checks++;
     }
    }
    UnityEngine.Object.DestroyImmediate(visual.gameObject);
   }
   var batchRect=Reference20Props.Rect("Batch",stage);batchRect.anchorMin=batchRect.anchorMax=Vector2.zero;
   var batch=batchRect.gameObject.AddComponent<Reference20CompanyParticleBatch>();
   var layer=new CompanyParticleLayer{batchKey="validation",image=Texture2D.whiteTexture,tiling=new float[]{1,1,0,0},indices=new[]{0,1,2,2,3,0},tracks=new[]{new CompanyParticleTrack{samples=new[]{new float[]{1,1,1,1,-1,-1,0,0,-1,1,0,1,1,1,1,1,1,-1,1,0}}}}};
   batch.Configure(layer);
   var emitterRect=Reference20Props.Rect("Emitter",stage);emitterRect.anchorMin=emitterRect.anchorMax=Vector2.zero;
   emitter=emitterRect.gameObject.AddComponent<Reference20CompanyParticleHandle>();emitter.Definition=new CompanyParticleEffect{layers=new[]{layer}};
   Reference20CompanyParticles.Active.Add(emitter);
   var populate=typeof(Reference20CompanyParticleBatch).GetMethod("OnPopulateMesh",BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{typeof(VertexHelper)},null);
   foreach(var position in new[]{new Vector2(85.6f,1584.8f),new Vector2(880.8f,1130.4f),new Vector2(994.4f,335.2f)}){
    emitterRect.anchoredPosition=position;emitterRect.localScale=new Vector3(.7f,1.3f,1);emitterRect.localRotation=Quaternion.Euler(0,0,45);
    using(var vh=new VertexHelper()){populate.Invoke(batch,new object[]{vh});vh.FillMesh(output);}
    Vector3 center=Vector3.zero;foreach(var vertex in output.vertices)center+=vertex;center/=output.vertexCount;
    if(Vector2.Distance(center,position)>.01f)errors.Add("Particle center differs from match: "+position+" actual "+center);checks++;
   }
   var fragments=Reference20Props.Rect("FragmentMeshValidation",stage).gameObject.AddComponent<Reference20FragmentBurst>();
   fragments.Play(Texture2D.whiteTexture,new[]{new Reference20Fragments.Region{x=0,y=0,w=1,h=1}},8,()=>{});
   geometry.Invoke(fragments,null);var fragmentMesh=fragments.canvasRenderer.GetMesh();
   if(!fragmentMesh||fragmentMesh.vertexCount!=32)errors.Add("Fragment UI rebuild used the default quad");checks++;
  }catch(Exception e){errors.Add(e.ToString());}finally{
   if(emitter)Reference20CompanyParticles.Active.Remove(emitter);
   UnityEngine.Object.DestroyImmediate(output);UnityEngine.Object.DestroyImmediate(root);
  }
 }
}

using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Reference20 {
public static class Reference20Props {
 // The HTML's board uses this world-space cell size in both orientations.
 public const float SourceCellSize=.5273437648f;
 public static float VisualScale(int id){return 1.28f*(id==13?.75f:.8f);}
 public static float CreationDuration(int id){var d=Resources.Load<SkeletonDataAsset>("Reference20/Props/Bomb_SkeletonData");var a=d.GetSkeletonData(true).FindAnimation(Creation(id));return a==null?0:a.Duration;}
 public static string Creation(int id){return id==13?"Boom_cross_creat_1":id==14?"Boom_square_creat":id==15?"Boom_color_creat_1":id==12?"Bomb_shu_creat_1":"Bomb_heng_creat_1";}
 static readonly Dictionary<string,SkeletonDataAsset> data=new Dictionary<string,SkeletonDataAsset>();
 static readonly Dictionary<int,Stack<Reference20PropVisual>> visuals=new Dictionary<int,Stack<Reference20PropVisual>>();
 static readonly Dictionary<string,Stack<Reference20EffectInstance>> effects=new Dictionary<string,Stack<Reference20EffectInstance>>();
 static readonly List<Reference20PropVisual> hints=new List<Reference20PropVisual>();
 static Material spineMaterial;
 public static RectTransform Rect(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=Vector2.one*.5f;r.pivot=Vector2.one*.5f;r.sizeDelta=Vector2.zero;r.anchoredPosition=Vector2.zero;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;return r;}
 public static SkeletonGraphic Rig(string asset,Transform parent,string skin,string animation){
  SkeletonDataAsset d;if(!data.TryGetValue(asset,out d)){d=Resources.Load<SkeletonDataAsset>("Reference20/Props/"+asset+"_SkeletonData");data[asset]=d;}
  var r=Rect(asset,parent);var sg=r.gameObject.AddComponent<Reference20SkeletonGraphic>();sg.skeletonDataAsset=d;
  if(!spineMaterial)spineMaterial=Resources.Load<Material>("Reference20/Native/SpineGraphic");sg.material=spineMaterial;sg.raycastTarget=false;sg.Initialize(true);
  sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();if(animation!=null)sg.AnimationState.SetAnimation(0,animation,false);sg.Update(0);sg.LateUpdate();return sg;
 }
 public static int Id(string name){return name=="LineBlock"?11:name=="VerticalLineBlock"?12:name=="AreaBlock"?13:name=="SquareBlock"?14:name=="SameColorBlock"?15:0;}
 static Reference20PropVisual CreateVisual(int id,Transform parent){var root=Rect("Prop_"+id,parent);var dynamic=Rect("Dynamic",Rect("ArtRoot",Rect("Root",root)));var v=root.gameObject.AddComponent<Reference20PropVisual>();v.Kind=id;v.Rig=Rig("Bomb",dynamic,id==13?"bomb_cross":id==14?"bomb_square":id==15?"brick_color":"brick0_heng","Bomb_still");return v;}
 public static RectTransform Visual(int id,Transform parent,bool born=true){
  Stack<Reference20PropVisual> pool;if(!visuals.TryGetValue(id,out pool)){pool=new Stack<Reference20PropVisual>();visuals[id]=pool;}
  Reference20PropVisual v=null;while(pool.Count>0&&!v)v=pool.Pop();if(!v)v=CreateVisual(id,parent);
  var root=v.transform as RectTransform;root.SetParent(parent,false);root.anchoredPosition=Vector2.zero;root.localScale=Vector3.one*VisualScale(id);root.localRotation=Quaternion.identity;root.Find("Root/ArtRoot/Dynamic").localRotation=Quaternion.identity;root.gameObject.SetActive(true);v.ResetPose(born);return root;
 }
 public static bool ReleaseVisual(Transform child,RectTransform stage){var v=child.GetComponent<Reference20PropVisual>();if(!v)return false;v.StopHint();child.gameObject.SetActive(false);child.SetParent(Reference20Native.Overlay(stage),false);Stack<Reference20PropVisual> pool;if(!visuals.TryGetValue(v.Kind,out pool)){pool=new Stack<Reference20PropVisual>();visuals[v.Kind]=pool;}pool.Push(v);return true;}
 public static void Warm(RectTransform stage){foreach(string asset in new[]{"Bomb","bomb_bomb","ColorBomb_ColorBomb"})Resources.Load<SkeletonDataAsset>("Reference20/Props/"+asset+"_SkeletonData").GetSkeletonData(true);foreach(int id in new[]{11,12,13,14,15})for(int i=0;i<8;i++){var v=CreateVisual(id,Reference20Native.Overlay(stage));ReleaseVisual(v.transform,stage);}Reference20CompanyParticles.Prewarm(stage);}
 public static bool Hint(RectTransform root,string name){var v=root.GetComponentInChildren<Reference20PropVisual>();if(!v)return false;v.Hint(name);if(!hints.Contains(v))hints.Add(v);return true;}
 public static void StopHints(){foreach(var v in hints)if(v)v.StopHint();hints.Clear();}
 public static void Reset(){visuals.Clear();effects.Clear();hints.Clear();}
 static RectTransform EffectRoot(string key,RectTransform stage,Vector2 position,float duration,Action<RectTransform> create,Action<RectTransform> play){
  Stack<Reference20EffectInstance> pool;if(!effects.TryGetValue(key,out pool)){pool=new Stack<Reference20EffectInstance>();effects[key]=pool;}
  Reference20EffectInstance instance=null;while(pool.Count>0&&!instance)instance=pool.Pop();
  if(!instance){var root=Rect(key,Reference20Native.Overlay(stage));create(root);instance=root.gameObject.AddComponent<Reference20EffectInstance>();instance.Capture();}else instance.Restore();
  var rt=instance.transform as RectTransform;rt.SetParent(Reference20Native.Overlay(stage),false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=position;rt.localScale=Vector3.one*.8875f;rt.localRotation=Quaternion.identity;rt.SetAsLastSibling();play(rt);instance.Lease(duration,i=>pool.Push(i));return rt;
 }
 public static Vector2 ColorComboCenter(Vector2 position){return new Vector2(540,960);}
 public static RectTransform SpineEffect(string key,RectTransform stage,Vector2 position,string asset,string skin,string animation,float life,float speed=1){return EffectRoot(key,stage,position,life,r=>{Rig(asset,r,skin,null);},r=>{var sg=r.GetComponentInChildren<SkeletonGraphic>(true);sg.timeScale=speed;sg.AnimationState.ClearTracks();sg.Skeleton.SetToSetupPose();sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();sg.AnimationState.SetAnimation(0,animation,key=="SameColorEffect");sg.Update(0);sg.LateUpdate();});}
 public static bool TryEffect(string name,RectTransform stage,Vector2 position,float scale,float life,out RectTransform root){
  root=null;
  if(name=="AreaAreaEffect")root=SpineEffect(name,stage,position,"bomb_bomb","default","Boom_bomb_1",1.7f);
  else if(name=="SameColorChangeEffect")root=SpineEffect(name,stage,position,"ColorBomb_ColorBomb","default","huixuanzhuan",2.5f);
  else if(name=="SameColorEffect")root=SpineEffect(name,stage,position,"Bomb","brick_color","Boom_color_start",life>0?life:3600);
  else if(name=="AreaEffect"){root=ParticleHalo(stage,position,false,1);return true;}
  else if(name=="SquareEffect"||name=="SquareBigEffect"){root=Reference20CompanyEffects.Play("BeeImpact",stage,position);return true;}
  if(!root)return false;root.localScale=Vector3.one*(name=="SameColorEffect"?VisualScale(15)*.8875f:name=="SameColorChangeEffect"?.6f*1.136f:1.136f);return true;
 }
 public static void LineBlast(RectTransform stage,Vector2 position,bool horizontal,float duration){
  string skin=horizontal?"brick0_heng":"brick0_shu";
  var root=EffectRoot(horizontal?"CompanyLineH":"CompanyLineV",stage,position,duration,r=>{Rig("Bomb",r,skin,null);Rig("Bomb",r,skin,null);},r=>{
   r.localScale=Vector3.one;int direction=-1;
   foreach(var sg in r.GetComponentsInChildren<SkeletonGraphic>()){
    sg.Skeleton.SetToSetupPose();sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();sg.AnimationState.ClearTracks();sg.AnimationState.SetAnimation(0,"Bomb_dropped_d",true);sg.Update(0);sg.LateUpdate();
    var t=sg.rectTransform;t.anchoredPosition=Vector2.zero;t.localRotation=Quaternion.identity;t.localScale=Vector3.one*(VisualScale(11)*.8875f);
    Reference20CompanyParticles.MovingTrail(stage,position,(horizontal?Vector2.right:Vector2.up)*direction,duration);
    t.DOAnchorPos((horizontal?Vector2.right:Vector2.up)*direction*(113.6f*15/SourceCellSize),duration).SetEase(Ease.Linear);direction=1;
   }
  });
 }
 public static RectTransform BeeFlight(RectTransform stage,Vector2 position,float duration,int payload,bool reverse){
  var root=SpineEffect("HtmlBee"+payload,stage,position,"Bomb","bomb_square","Bomb_fly1_1",duration+.32f);
  root.localScale=Vector3.one*(VisualScale(14)*.8875f);
  if(payload>0&&!root.Find("Payload")){var companion=Visual(payload,root,false);companion.name="Payload";companion.localScale=Vector3.one*(VisualScale(payload)/VisualScale(14));}
  return root;
 }
 public static RectTransform ParticleHalo(RectTransform stage,Vector2 position,bool color,float scale=1){return Reference20CompanyEffects.Play(color?"ColorFinish":"Bomb",stage,position,scale);}
 public static void ChangeNormal(RectTransform root,int id){
  if(root.Find("element"))return;
  var image=root.GetComponent<UnityEngine.UI.Image>();if(image)image.enabled=false;
  var sg=Rig("element",root,id==3?"brick2_0":"brick4_0","shake");sg.rectTransform.localScale=Vector3.one*(.95f/1.28f*SourceCellSize*2.5f*1.136f);
 }
}
}

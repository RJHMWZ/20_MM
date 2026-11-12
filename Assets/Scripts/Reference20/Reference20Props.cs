using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Reference20 {
public static class Reference20Props {
 public const float VisualScale=.90f;
 static readonly Dictionary<string,SkeletonDataAsset> data=new Dictionary<string,SkeletonDataAsset>();
 static readonly Dictionary<int,Stack<Reference20PropVisual>> visuals=new Dictionary<int,Stack<Reference20PropVisual>>();
 static readonly Dictionary<string,Stack<Reference20EffectInstance>> effects=new Dictionary<string,Stack<Reference20EffectInstance>>();
 static readonly List<Reference20PropVisual> hints=new List<Reference20PropVisual>();
 static Material spineMaterial;
 public static RectTransform Rect(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=Vector2.one*.5f;r.sizeDelta=Vector2.zero;return r;}
 public static SkeletonGraphic Rig(string asset,Transform parent,string skin,string animation){
  SkeletonDataAsset d;if(!data.TryGetValue(asset,out d)){d=Resources.Load<SkeletonDataAsset>("Reference20/Props/"+asset+"_SkeletonData");data[asset]=d;}
  var r=Rect(asset,parent);var sg=r.gameObject.AddComponent<SkeletonGraphic>();sg.skeletonDataAsset=d;
  if(!spineMaterial)spineMaterial=Resources.Load<Material>("Reference20/Native/SpineGraphic");sg.material=spineMaterial;sg.raycastTarget=false;sg.Initialize(true);
  sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();if(animation!=null)sg.AnimationState.SetAnimation(0,animation,false);sg.Update(0);sg.LateUpdate();return sg;
 }
 public static int Id(string name){return name=="LineBlock"?11:name=="AreaBlock"?13:name=="SquareBlock"?14:name=="SameColorBlock"?15:0;}
 static Reference20PropVisual CreateVisual(int id,Transform parent){var root=Rect("Prop_"+id,parent);var dynamic=Rect("Dynamic",Rect("ArtRoot",Rect("Root",root)));var v=root.gameObject.AddComponent<Reference20PropVisual>();v.Kind=id;v.Rig=Rig("Bomb",dynamic,id==13?"bomb_cross":id==14?"bomb_square":id==15?"brick_color":"brick0_heng","Bomb_still");return v;}
 public static RectTransform Visual(int id,Transform parent,bool born=true){
  Stack<Reference20PropVisual> pool;if(!visuals.TryGetValue(id,out pool)){pool=new Stack<Reference20PropVisual>();visuals[id]=pool;}
  Reference20PropVisual v=null;while(pool.Count>0&&!v)v=pool.Pop();if(!v)v=CreateVisual(id,parent);
  var root=v.transform as RectTransform;root.SetParent(parent,false);root.anchoredPosition=Vector2.zero;root.localScale=Vector3.one*VisualScale;root.localRotation=Quaternion.identity;root.Find("Root/ArtRoot/Dynamic").localRotation=Quaternion.identity;root.gameObject.SetActive(true);v.ResetPose(born);return root;
 }
 public static bool ReleaseVisual(Transform child,RectTransform stage){var v=child.GetComponent<Reference20PropVisual>();if(!v)return false;v.StopHint();child.gameObject.SetActive(false);child.SetParent(Reference20Native.Overlay(stage),false);Stack<Reference20PropVisual> pool;if(!visuals.TryGetValue(v.Kind,out pool)){pool=new Stack<Reference20PropVisual>();visuals[v.Kind]=pool;}pool.Push(v);return true;}
 public static void Warm(RectTransform stage){foreach(string asset in new[]{"Bomb","Boom_fly_double_b","bomb_bomb","ColorBomb_ColorBomb"})Resources.Load<SkeletonDataAsset>("Reference20/Props/"+asset+"_SkeletonData").GetSkeletonData(true);foreach(int id in new[]{11,13,14,15})for(int i=0;i<8;i++){var v=CreateVisual(id,Reference20Native.Overlay(stage));ReleaseVisual(v.transform,stage);}Reference20CompanyEffects.Prewarm();}
 public static bool Hint(RectTransform root,string name){var v=root.GetComponentInChildren<Reference20PropVisual>();if(!v)return false;v.Hint(name);if(!hints.Contains(v))hints.Add(v);return true;}
 public static void StopHints(){foreach(var v in hints)if(v)v.StopHint();hints.Clear();}
 public static void Reset(){visuals.Clear();effects.Clear();hints.Clear();}
 static RectTransform EffectRoot(string key,RectTransform stage,Vector2 position,float duration,Action<RectTransform> create,Action<RectTransform> play){
  Stack<Reference20EffectInstance> pool;if(!effects.TryGetValue(key,out pool)){pool=new Stack<Reference20EffectInstance>();effects[key]=pool;}
  Reference20EffectInstance instance=null;while(pool.Count>0&&!instance)instance=pool.Pop();
  if(!instance){var root=Rect(key,Reference20Native.Overlay(stage));create(root);instance=root.gameObject.AddComponent<Reference20EffectInstance>();instance.Capture();}else instance.Restore();
  var rt=instance.transform as RectTransform;rt.SetParent(Reference20Native.Overlay(stage),false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=position;rt.localScale=Vector3.one*.8875f;rt.localRotation=Quaternion.identity;rt.SetAsLastSibling();play(rt);instance.Lease(duration,i=>pool.Push(i));return rt;
 }
 public static Vector2 ColorComboCenter(Vector2 position){return new Vector2(540,Mathf.Clamp(position.y,700,1220));}
 public static RectTransform SpineEffect(string key,RectTransform stage,Vector2 position,string asset,string skin,string animation,float life,float speed=1){return EffectRoot(key,stage,position,life,r=>{Rig(asset,r,skin,null);},r=>{var sg=r.GetComponentInChildren<SkeletonGraphic>(true);sg.timeScale=speed;sg.AnimationState.ClearTracks();sg.Skeleton.SetToSetupPose();sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();sg.AnimationState.SetAnimation(0,animation,key=="SameColorEffect");sg.Update(0);sg.LateUpdate();});}
 public static bool TryEffect(string name,RectTransform stage,Vector2 position,float scale,float life,out RectTransform root){
  root=null;
  if(name=="AreaAreaEffect")root=SpineEffect(name,stage,position,"bomb_bomb","default","Boom_bomb_1",2.02f,1.6667f/2);
  else if(name=="SameColorChangeEffect")root=SpineEffect(name,stage,position,"ColorBomb_ColorBomb","default","huixuanzhuan",2.2f,2.1667f/1.8f);
  else if(name=="SameColorEffect"){root=SpineEffect(name,stage,position,"Bomb","brick_color","Boom_color_start",life>0?life:3600);var charge=Reference20CompanyEffects.Play("ColorCharge",stage,position,1,true);if(charge){charge.SetParent(root,false);charge.anchorMin=charge.anchorMax=Vector2.one*.5f;charge.anchoredPosition=Vector2.zero;}}
  else if(name=="AreaEffect"){root=SpineEffect(name,stage,position,"Bomb","bomb_cross","Boom_bomb_2",.24f,1.6667f/.23f);ParticleHalo(stage,position,false,1);}
  else if(name=="SquareEffect"||name=="SquareBigEffect")root=SpineEffect(name,stage,position,"Bomb","bomb_square","Boom_fly_start",.25f);
  if(!root)return false;root.localScale=Vector3.one*scale;if(name=="SameColorChangeEffect")root.DOAnchorPos(ColorComboCenter(position),.25f).SetEase(Ease.OutQuad);return true;
 }
 public static void LineBlast(RectTransform stage,Vector2 position,bool horizontal,float duration){
  var root=EffectRoot(horizontal?"OwnLineH":"OwnLineV",stage,position,duration,r=>{Rig("Bomb",r,"brick0_heng",null);Rig("Bomb",r,"brick0_heng",null);},r=>{int direction=-1;foreach(var sg in r.GetComponentsInChildren<SkeletonGraphic>()){sg.Skeleton.SetToSetupPose();sg.Skeleton.SetSkin("brick0_heng");sg.Skeleton.SetSlotsToSetupPose();sg.AnimationState.ClearTracks();sg.AnimationState.SetAnimation(0,"Bomb_still",false);var t=sg.rectTransform;t.anchoredPosition=Vector2.zero;t.localRotation=Quaternion.Euler(0,0,horizontal?0:90);t.localScale=Vector3.one*VisualScale;var trail=Reference20CompanyEffects.Play("RocketTrail",stage,Vector2.zero,.6f);if(trail){trail.SetParent(t,false);trail.anchorMin=trail.anchorMax=Vector2.one*.5f;trail.anchoredPosition=Vector2.zero;trail.localRotation=Quaternion.Euler(0,0,horizontal?direction>0?180:0:direction>0?90:-90);}t.DOAnchorPos((horizontal?Vector2.right:Vector2.up)*direction*2500,duration).SetEase(Ease.Linear);direction=1;}});
 }
 public static RectTransform BeeFlight(RectTransform stage,Vector2 position,float duration,int payload,bool reverse){
  bool combo=payload>0;string asset=combo?"Boom_fly_double_b":"Bomb",skin=combo?(payload==13?"bomb_square_cross":payload==11?"bomb_square_heng":"bomb_square_shu"):"bomb_square";
  var root=SpineEffect("OwnBee"+payload,stage,position,asset,skin,combo?"Boom_fly_double_show":"Boom_fly_start",duration+.22f);
  var sg=root.GetComponentInChildren<SkeletonGraphic>();sg.rectTransform.localScale=Vector3.one*VisualScale;
  if(combo){sg.AnimationState.AddAnimation(0,reverse?"Boom_fly_doublek_l":"Boom_fly_doublek_r",true,0);}else sg.AnimationState.AddAnimation(0,"Bomb_fly1",true,0);
  return root;
 }
 public static RectTransform ParticleHalo(RectTransform stage,Vector2 position,bool color,float scale=1){return Reference20CompanyEffects.Play(color?"ColorFinish":"Bomb",stage,position,scale);}
}
}

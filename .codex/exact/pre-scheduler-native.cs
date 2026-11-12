using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
namespace Reference20 {
[Serializable] public class NativeNode {
 public string name,sprite,skeleton,anim,skin; public float x,y,sx=1,sy=1,angle,w,h,ax=.5f,ay=.5f,opacity=255,timeScale=1,fill;public float[] color;public NativeSlot[] slots;public bool active=true,loop,autoplay,mask,inverted;public NativeNode[] children;public string[] animations;public NativeClip[] clips;
}
[Serializable] public class NativeSlot {public string name,attachment;public float r=1,g=1,b=1,a=1;}
[Serializable] public class NativeClip {public string name;public float duration;public int wrap;public JObject data;}
public class NativeGraph {public NativeNode[] pieces;public NativeNode guideHand,button;public Dictionary<string,NativeNode> effects;}
public class NativeEndGraph {public NativeNode end;}
public class Reference20Native : MonoBehaviour {
 static NativeGraph graph;static Dictionary<string,SkeletonDataAsset> skeletons=new Dictionary<string,SkeletonDataAsset>();static Material graphicMaterial;
 public static void ResetCaches(){graph=null;skeletons.Clear();}
 public static NativeGraph Graph {get {if(graph==null)graph=JsonConvert.DeserializeObject<NativeGraph>(Resources.Load<TextAsset>("Reference20/Native/graph").text);return graph;}}
 public static RectTransform Piece(string name,RectTransform stage){var n=Array.Find(Graph.pieces,x=>x.name==name);if(n==null)return null;var rt=Node(n,stage);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=new Vector2(540+(n.x-1024)*.8875f,960+(1088+n.y)*.8875f);rt.localScale=new Vector3(n.sx,n.sy,1)*.8875f;rt.gameObject.AddComponent<Reference20Native>().id=int.Parse(name.Substring(name.LastIndexOf('_')+1));return rt;}
 public static RectTransform Visual(string name,Transform parent){NativeNode n;if(!Graph.effects.TryGetValue(name,out n))return null;var rt=Node(n,parent);rt.anchoredPosition=Vector2.zero;rt.gameObject.SetActive(true);if(name=="SquareBlock"||name=="AreaBlock"||name=="LineBlock"||name=="SameColorBlock")foreach(var s in rt.GetComponentsInChildren<SkeletonGraphic>(true)){if(s.Skeleton.Data.FindAnimation("synth")!=null)s.AnimationState.SetAnimation(0,"synth",false);}return rt;}
 public static RectTransform GuideHand(RectTransform parent){var n=Graph.guideHand;if(n==null)return null;var rt=Node(n,parent);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=new Vector2(n.x,n.y);foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true)){int index=Array.FindIndex(clip.clips,c=>c.name=="common_hand");if(index>=0)clip.Play(index);}return rt;}
 public static RectTransform DownloadArtwork(RectTransform parent){var n=Graph.button;if(n==null)return null;var rt=Node(n,parent);rt.anchorMin=rt.anchorMax=new Vector2(.5f,.5f);rt.anchoredPosition=Vector2.zero;foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(0);return rt;}
 public static RectTransform EndArtwork(RectTransform parent){var asset=Resources.Load<TextAsset>("Reference20/Native/end");if(!asset)return null;var data=JsonConvert.DeserializeObject<NativeEndGraph>(asset.text);var rt=Node(data.end,parent);rt.anchoredPosition=Vector2.zero;return rt;}
 public int id;int health,animationRevision;Coroutine idleRoutine;
 static SkeletonDataAsset Skeleton(string name){
  SkeletonDataAsset result;if(skeletons.TryGetValue(name,out result))return result;
  result=Resources.Load<SkeletonDataAsset>("Reference20/Native/"+name+"_data");if(result){skeletons[name]=result;return result;}
  var text=Resources.Load<TextAsset>("Reference20/Native/"+name+".atlas");var mats=new List<Material>();
  for(int i=0;i<8;i++){var tex=Resources.Load<Texture2D>("Reference20/Native/"+name+"_tex"+i);if(tex==null)break;var mat=new Material(Shader.Find("Spine/Skeleton"));mat.name=name+"_tex"+i;mat.mainTexture=tex;mats.Add(mat);}
  var atlas=SpineAtlasAsset.CreateRuntimeInstance(text,mats.ToArray(),true);
  result=SkeletonDataAsset.CreateRuntimeInstance(Resources.Load<TextAsset>("Reference20/Native/"+name),atlas,true,.01f);skeletons[name]=result;return result;
 }
 static RectTransform Node(NativeNode n,Transform parent){
  var go=new GameObject(n.name,typeof(RectTransform));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);var pr=parent as RectTransform;rt.anchorMin=rt.anchorMax=pr?pr.pivot:new Vector2(.5f,.5f);rt.pivot=new Vector2(n.ax,n.ay);rt.sizeDelta=new Vector2(n.w,n.h);rt.anchoredPosition=new Vector2(n.x,n.y);rt.localScale=new Vector3(n.sx,n.sy,1);rt.localRotation=Quaternion.Euler(0,0,n.angle);
  Color color=n.color!=null&&n.color.Length>=3?new Color(n.color[0],n.color[1],n.color[2],1):Color.white;
  if(!string.IsNullOrEmpty(n.sprite)||n.mask){var im=go.AddComponent<Image>();im.sprite=Resources.Load<Sprite>("Reference20/Native/"+n.sprite);if(n.mask&&!im.sprite)im.sprite=Resources.Load<Sprite>("Reference20/white");im.color=color;im.raycastTarget=false;if(n.mask){var mask=go.AddComponent<Mask>();mask.showMaskGraphic=false;}}
  if(!string.IsNullOrEmpty(n.skeleton)){
   if(!graphicMaterial){graphicMaterial=Resources.Load<Material>("Reference20/Native/SpineGraphic");if(!graphicMaterial){graphicMaterial=new Material(Shader.Find("Spine/SkeletonGraphic"));graphicMaterial.EnableKeyword("_STRAIGHT_ALPHA_INPUT");}}
   var sg=go.AddComponent<SkeletonGraphic>();sg.skeletonDataAsset=Skeleton(n.skeleton);sg.material=graphicMaterial;sg.raycastTarget=false;sg.Initialize(true);sg.color=color;
   if(!string.IsNullOrEmpty(n.skin)&&sg.Skeleton.Data.FindSkin(n.skin)!=null){sg.Skeleton.SetSkin(n.skin);sg.Skeleton.SetSlotsToSetupPose();}
   if(n.slots!=null)foreach(var state in n.slots){var slot=sg.Skeleton.FindSlot(state.name);if(slot!=null){slot.R=state.r;slot.G=state.g;slot.B=state.b;slot.A=state.a;}}
   if(!string.IsNullOrEmpty(n.anim)&&sg.Skeleton.Data.FindAnimation(n.anim)!=null)sg.AnimationState.SetAnimation(0,n.anim,n.loop||n.anim.Contains("idle")||n.anim.Contains("\u5f85\u673a"));else if(string.IsNullOrEmpty(n.anim)){if(sg.Skeleton.Data.FindAnimation("normal")!=null)sg.AnimationState.SetAnimation(0,"normal",true);else if(sg.Skeleton.Data.FindAnimation("idle_1")!=null)sg.AnimationState.SetAnimation(0,"idle_1",true);}sg.timeScale=n.timeScale;
  }
  if(n.opacity<255){var group=go.GetComponent<CanvasGroup>();if(!group)group=go.AddComponent<CanvasGroup>();group.alpha=n.opacity/255;}
  // The original corn uses an inverted image stencil. Its complement is a right-facing
  // clip rectangle; preserve the children's world positions when moving that rectangle.
  if(n.mask&&n.inverted)rt.anchoredPosition+=new Vector2(n.w,0);
  if(n.children!=null)foreach(var c in n.children){var child=Node(c,rt);if(n.mask&&n.inverted)child.anchoredPosition-=new Vector2(n.w,0);}
  if(n.clips!=null&&n.clips.Length>0){var animation=go.AddComponent<Reference20CocosClip>();animation.clips=n.clips;if(n.autoplay)animation.Play(0);}
  go.SetActive(n.active);return rt;
 }
 public static void Effect(string name,RectTransform stage,Vector2 position,float scale=.8875f){NativeNode n;if(!Graph.effects.TryGetValue(name,out n))return;var rt=Node(n,stage);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=position;rt.localScale*=scale;rt.gameObject.SetActive(true);foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(0);UnityEngine.Object.Destroy(rt.gameObject,2.3f);}
 public void PlayAnimation(string name,bool loop=false){foreach(var s in GetComponentsInChildren<SkeletonGraphic>(true))if(s.Skeleton.Data.FindAnimation(name)!=null){s.gameObject.SetActive(true);s.AnimationState.SetAnimation(0,name,loop);}}
 public void Configure(int type,int hp){
  id=type;health=hp;animationRevision++;
  ApplyIdlePose();if(Application.isPlaying){if(idleRoutine!=null)StopCoroutine(idleRoutine);idleRoutine=StartCoroutine(IdleLoop(animationRevision));}
 }
 void ApplyIdlePose(){
  string pose=id==100016?(health>=3?"egg_idle":"idle"):id==100015?(health>=3?"egg_idle":health==2?"egg2_idle":"idle"):id==100042?(health>=3?"\u52a8\u65481-\u5f85\u673a":health==2?"\u52a8\u65483-\u5f85\u673a":"\u52a8\u65484-\u5f00\u5c4f\u540e\u5f85\u673a"):id==100032?"\u5de6\u53f3\u770b":"idle";
  PlayAnimation(pose,true);
 }
 IEnumerator IdleLoop(int revision){
  int index=0;
  while(revision==animationRevision&&health>0){
   string animation;float duration;
   if(id==100042){animation=health>=3?"\u52a8\u65481-\u5f85\u673a":health==2?"\u52a8\u65483-\u5f85\u673a":"\u52a8\u65484-\u5f00\u5c4f\u540e\u5f85\u673a";duration=8;}
   else if(id==100032){animation=index%2==0?"\u5de6\u53f3\u770b":"\u539f\u5730\u6323\u624e";duration=index%2==0?7.0667f:1.83f;}
   else if(id==100016){animation=health>=3?"egg_idle":"idle";duration=3;}
   else if(id==100015&&health>1){animation=health==3?"egg_idle":"egg2_idle";duration=3;}
   else {animation=index%3==0?"idle":index%3==1?"idle2":"idle3";duration=index%3==0?2:3.2f;}
   PlayAnimation(animation,false);yield return new WaitForSeconds(duration);index++;
  }
 }
 public float AnimateDamage(int before,int after){
  health=after;int revision=++animationRevision;if(idleRoutine!=null){StopCoroutine(idleRoutine);idleRoutine=null;}
  float duration=id==100042?(after>0?1:.33f):id==100016?(before==3?1.7f:.25f):id==100032?(before%2==0?.8f:.867f):before==3?.8f:before==2?.5f:.4f;
  if(Application.isPlaying)StartCoroutine(DamageSequence(before,after,duration,revision));return duration;
 }
 IEnumerator DamageSequence(int before,int after,float duration,int revision){
  if(id==100042)PlayAnimation(before==3?"\u52a8\u65482-\u6d88\u9664":before==2?"\u52a8\u65484-\u6d88\u9664":"\u52a8\u65485-\u6d88\u9664",false);
  else if(id==100016&&before==3){var eggs=GetComponentsInChildren<SkeletonGraphic>(true);for(int i=0;i<eggs.Length;i++)StartCoroutine(Hatch(eggs[i],i*.2f,revision));}
  else if(id==100015)PlayAnimation(before==3?"egg_open":before==2?"egg_open2":"collect",false);
  else if(id==100032)PlayAnimation(before%2==0?"\u6536\u7f29\u7ef3\u5b50\uff0c\u516c\u9e21\u6324\u538b":"\u7ef3\u5b50\u677e\u7ed1",false);
  yield return new WaitForSeconds(duration);
  if(after>0&&revision==animationRevision){ApplyIdlePose();idleRoutine=StartCoroutine(IdleLoop(revision));}
 }
 IEnumerator Hatch(SkeletonGraphic egg,float delay,int revision){
  yield return new WaitForSeconds(delay);if(!egg||revision!=animationRevision)yield break;
  Animate(egg,"egg_open",false);yield return new WaitForSeconds(.6f);
  if(!egg||revision!=animationRevision)yield break;Animate(egg,"egg_open2",false);yield return new WaitForSeconds(Mathf.Max(0,.6f-delay));
  if(!egg||revision!=animationRevision)yield break;Animate(egg,"collect",false);yield return new WaitForSeconds(.5f);
  if(egg&&revision==animationRevision)Animate(egg,"idle",true);
 }
 void Animate(SkeletonGraphic skeleton,string animation,bool loop){if(skeleton.Skeleton.Data.FindAnimation(animation)!=null)skeleton.AnimationState.SetAnimation(0,animation,loop);}
 public void Hit(int remaining){AnimateDamage(health,remaining);}
 public void PlayCollected(){animationRevision++;PlayAnimation(id==100042?"\u52a8\u65486-\u6d88\u9664":id==100032?"\u6536\u96c6":"collect_2",true);}
 public void UseChickData(SkeletonDataAsset data,string skin){
  if(!data)return;foreach(var skeleton in GetComponentsInChildren<SkeletonGraphic>(true)){
   skeleton.skeletonDataAsset=data;skeleton.Initialize(true);if(!string.IsNullOrEmpty(skin)&&skeleton.Skeleton.Data.FindSkin(skin)!=null){skeleton.Skeleton.SetSkin(skin);skeleton.Skeleton.SetSlotsToSetupPose();}
  }
 }
 public RectTransform DetachNestChick(RectTransform stage){
  var eggs=GetComponentsInChildren<SkeletonGraphic>(false);foreach(var egg in eggs){if(!egg.gameObject.activeInHierarchy)continue;return Detach(egg,stage,"collect_2");}return null;
 }
 public RectTransform DetachCornHead(RectTransform stage){foreach(var head in GetComponentsInChildren<SkeletonGraphic>(false))if(head.name=="head")return Detach(head,stage,"collect_2");return null;}
 RectTransform Detach(SkeletonGraphic skeleton,RectTransform stage,string animation){
  var rect=skeleton.rectTransform;Vector3 position=rect.position;rect.SetParent(stage,true);rect.anchorMin=rect.anchorMax=Vector2.zero;rect.anchoredPosition=(Vector2)stage.InverseTransformPoint(position)+stage.sizeDelta*.5f;rect.localRotation=Quaternion.identity;Animate(skeleton,animation,true);return rect;
 }
 public void Eat(int remaining){
  health=remaining;int revision=++animationRevision;if(Application.isPlaying)StartCoroutine(EatSequence(revision));
  foreach(var rect in GetComponentsInChildren<RectTransform>(true)){
   if(rect.name=="headNode")rect.anchoredPosition+=new Vector2(128,0);
   if(rect.name=="Mask"){rect.anchoredPosition+=new Vector2(128,0);for(int i=0;i<rect.childCount;i++){var child=rect.GetChild(i) as RectTransform;if(child)child.anchoredPosition-=new Vector2(128,0);}}
  }
 }
 IEnumerator EatSequence(int revision){PlayAnimation("eat",true);yield return new WaitForSeconds(.55f);if(revision!=animationRevision)yield break;PlayAnimation("eat2",false);yield return new WaitForSeconds(.4f);if(revision==animationRevision){ApplyIdlePose();idleRoutine=StartCoroutine(IdleLoop(revision));}}
}
}

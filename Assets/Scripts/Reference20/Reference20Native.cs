using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;
namespace Reference20 {
[Serializable] public class NativeNode {
 public int src=770,dst=771,spriteSizeMode;public string name,sprite,skeleton,anim,skin; public float x,y,sx=1,sy=1,angle,w,h,ax=.5f,ay=.5f,opacity=255,timeScale=1,fill;public float[] color;public NativeSlot[] slots;public bool active=true,loop,autoplay,mask,inverted;public NativeNode[] children;public string[] animations;public NativeClip[] clips;public Reference20ParticleData particle;public CocosParticle3DData particle3d;
}
[Serializable] public class NativeSlot {public string name,attachment;public float r=1,g=1,b=1,a=1;}
[Serializable] public class NativeClip {public string name;public float duration;public int wrap;public NativeClipData data;}
public class NativeGraph {public NativeNode[] pieces;public NativeNode guideHand,button;public NativeClip[] basicHintClips;public Dictionary<string,NativeNode> effects;}
public class Reference20Native : MonoBehaviour {
 static NativeGraph graph;static Dictionary<string,SkeletonDataAsset> skeletons=new Dictionary<string,SkeletonDataAsset>();static Material graphicMaterial;
 static readonly HashSet<int> trimmedSprites=new HashSet<int>();
 static readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();
 static readonly Dictionary<int,Sprite[]> basicFragments=new Dictionary<int,Sprite[]>();
 static Dictionary<string,float> dataScales;
 static readonly Dictionary<string,Stack<Reference20EffectInstance>> effectPools=new Dictionary<string,Stack<Reference20EffectInstance>>();
 static NativeClip[] basicHintClips;static RectTransform overlayRoot;
 static readonly Dictionary<int,Material> spriteMaterials=new Dictionary<int,Material>();
 static int Blend(int value){switch(value){case 0:return 0;case 1:return 1;case 768:return 3;case 769:return 6;case 770:return 5;case 771:return 10;case 772:return 7;case 773:return 8;case 774:return 2;case 775:return 4;case 776:return 9;default:return 10;}}
 static Material SpriteMaterial(int src,int dst){if(src==770&&dst==771)return null;int key=src*1024+dst;Material mat;if(spriteMaterials.TryGetValue(key,out mat))return mat;mat=new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));mat.name="CocosSprite_"+src+"_"+dst;mat.SetFloat("_SrcBlend",Blend(src));mat.SetFloat("_DstBlend",Blend(dst));spriteMaterials[key]=mat;return mat;}

 static readonly Dictionary<string,Material> particleMaterials=new Dictionary<string,Material>();
 static Material ParticleMaterial(CocosParticle3DData data){var tint=data.tint;string key=data.src+"_"+data.dst+"_"+(tint==null?"white":string.Join(",",tint));Material material;if(particleMaterials.TryGetValue(key,out material))return material;material=new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));material.name="CocosParticle_"+key;material.SetFloat("_SrcBlend",Blend(data.src));material.SetFloat("_DstBlend",Blend(data.dst));material.SetColor("_Color",tint!=null&&tint.Length==4?new Color(tint[0]*2,tint[1]*2,tint[2]*2,tint[3]*2):Color.white);particleMaterials[key]=material;return material;}
 public static Reference20CocosClip Hint(RectTransform root,string name){if(Reference20Props.Hint(root,name))return null;foreach(var clip in root.GetComponentsInChildren<Reference20CocosClip>(true))if(clip.Play(name))return clip;var image=root.GetComponent<Image>();if(!image||!image.sprite)return null;if(basicHintClips==null)basicHintClips=Graph.basicHintClips;var art=new GameObject("Presentation",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<RectTransform>();art.SetParent(root,false);art.sizeDelta=root.sizeDelta;var graphic=art.GetComponent<Image>();graphic.sprite=image.sprite;graphic.color=image.color;graphic.raycastTarget=false;image.enabled=false;var animation=art.gameObject.AddComponent<Reference20CocosClip>();animation.clips=basicHintClips;animation.Play(name);return animation;}
 public static float CocosScale(string name){if(dataScales==null){var asset=Resources.Load<TextAsset>("Reference20/Native/spine-scales");dataScales=asset?JsonConvert.DeserializeObject<Dictionary<string,float>>(asset.text):new Dictionary<string,float>();}float scale;return dataScales.TryGetValue(name,out scale)?scale:1;}
 internal static Sprite Sprite(string name){if(string.IsNullOrEmpty(name))return null;Sprite result;if(!sprites.TryGetValue(name,out result)){result=Resources.Load<Sprite>(name=="sprite_0"?"Reference20/Company/Elements/brick2_0":name=="sprite_1"?"Reference20/Company/Elements/brick4_0":"Reference20/Native/"+name);sprites[name]=result;}return result;}
 public static void Prewarm(){Reference20ReplacementVisual.Prewarm();Reference20Fragments.Prewarm();}
 public static void WarmEffects(RectTransform stage){}
 public static RectTransform Overlay(RectTransform stage){if(overlayRoot&&overlayRoot.parent==stage&&overlayRoot.gameObject.activeSelf)return overlayRoot;for(int i=stage.childCount-1;i>=0;i--){var child=stage.GetChild(i) as RectTransform;if(child&&child.name=="TransientEffects"&&child.gameObject.activeSelf){overlayRoot=child;return child;}}var rt=new GameObject("TransientEffects",typeof(RectTransform),typeof(Canvas)).GetComponent<RectTransform>();rt.SetParent(stage,false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.pivot=Vector2.zero;rt.sizeDelta=Vector2.zero;var canvas=rt.GetComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=200;overlayRoot=rt;return rt;}
 static RectTransform CollectionLayer(RectTransform stage){for(int i=stage.childCount-1;i>=0;i--){var child=stage.GetChild(i) as RectTransform;if(child&&child.name=="CollectedBirds"&&child.gameObject.activeSelf)return child;}var rt=new GameObject("CollectedBirds",typeof(RectTransform),typeof(Canvas)).GetComponent<RectTransform>();rt.SetParent(stage,false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.pivot=Vector2.zero;rt.sizeDelta=Vector2.zero;var canvas=rt.GetComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=300;return rt;}
 public static void Raise(RectTransform rt,RectTransform stage){var position=rt.position;rt.SetParent(CollectionLayer(stage),true);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=(Vector2)stage.InverseTransformPoint(position)+stage.sizeDelta*.5f;var canvas=rt.GetComponent<Canvas>();if(canvas)canvas.overrideSorting=false;rt.SetAsLastSibling();}
 public static void Fly(RectTransform rt,Vector2 from,Vector2 to,float duration){var controlPoint=new Vector2((from.x+to.x)*.5f,Mathf.Max(from.y,to.y)+200*.8875f);CubicFlight(rt,from,controlPoint,controlPoint,to,duration);}
 static void CubicFlight(RectTransform rt,Vector2 from,Vector2 startHandle,Vector2 endHandle,Vector2 to,float duration){rt.DOKill();DOTween.To(()=>0f,t=>{if(rt){float r=1-t;rt.anchoredPosition=r*r*r*from+3*r*r*t*startHandle+3*r*t*t*endHandle+t*t*t*to;}},1,duration).SetEase(Ease.Linear).SetTarget(rt);}
 public static void ResetCaches(){Reference20Props.Reset();Reference20Fragments.Reset();Reference20CompanyEffects.Reset();trimmedSprites.Clear();graph=null;overlayRoot=null;skeletons.Clear();sprites.Clear();effectPools.Clear();}
 static RectTransform LeaseEffect(string name,RectTransform stage,Vector2 position,float duration,float scale=.8875f){NativeNode node;if(!Graph.effects.TryGetValue(name,out node))return null;Stack<Reference20EffectInstance> pool;if(!effectPools.TryGetValue(name,out pool)){pool=new Stack<Reference20EffectInstance>();effectPools[name]=pool;}Reference20EffectInstance instance=null;while(pool.Count>0&&!instance)instance=pool.Pop();if(!instance){var root=Node(node,Overlay(stage));instance=root.gameObject.AddComponent<Reference20EffectInstance>();instance.Capture();}else instance.Restore();var rt=instance.transform as RectTransform;rt.SetParent(Overlay(stage),false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=position;rt.localScale=new Vector3(node.sx,node.sy,1)*scale;rt.SetAsLastSibling();instance.Lease(duration,i=>pool.Push(i));return rt;}
 public static NativeGraph Graph {get {if(graph==null)graph=JsonConvert.DeserializeObject<NativeGraph>(Resources.Load<TextAsset>("Reference20/Native/graph").text);return graph;}}
 public static RectTransform Piece(string name,RectTransform stage){var n=Array.Find(Graph.pieces,x=>x.name==name);if(n==null)return null;int id=int.Parse(name.Substring(name.LastIndexOf('_')+1));var rt=id>=11&&id<=15?Reference20Props.Rect(name,stage):Reference20ReplacementVisual.Supports(id)?Reference20ReplacementVisual.Create(id,name,stage):Node(n,stage);if(id>=11&&id<=15){var prop=Reference20Props.Visual(id,rt,false);}rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=new Vector2(540+(n.x-1024)*.8875f,960+(1088+n.y)*.8875f);rt.localScale=new Vector3(n.sx,n.sy,1)*.8875f;if(id>=100)rt.gameObject.AddComponent<Reference20Native>().id=id;return rt;}
 public static void WarmVisuals(RectTransform stage){Reference20Props.Warm(stage);}
 public static bool ReleaseVisual(Transform child,RectTransform stage){return Reference20Props.ReleaseVisual(child,stage);}
 public static RectTransform Visual(string name,Transform parent){int id=Reference20Props.Id(name);if(id>0)return Reference20Props.Visual(id,parent);NativeNode n;if(!Graph.effects.TryGetValue(name,out n))return null;var rt=Node(n,parent);rt.localScale=new Vector3(n.sx,n.sy,1);rt.localRotation=Quaternion.Euler(0,0,n.angle);rt.anchoredPosition=Vector2.zero;rt.gameObject.SetActive(true);return rt;}
 public static RectTransform GuideHand(RectTransform parent){var n=Graph.guideHand;if(n==null)return null;var rt=Node(n,parent);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=new Vector2(n.x,n.y);foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true)){int index=Array.FindIndex(clip.clips,c=>c.name=="common_hand");if(index>=0)clip.Play(index);}return rt;}
 public static RectTransform DownloadArtwork(RectTransform parent){var n=Graph.button;if(n==null)return null;var rt=Node(n,parent);rt.anchorMin=rt.anchorMax=new Vector2(.5f,.5f);rt.anchoredPosition=Vector2.zero;foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(0);return rt;}
 public int id;int health,animationRevision;Reference20ReplacementVisual replacement;Coroutine idleRoutine;
 readonly Dictionary<RectTransform,float> cornTargets=new Dictionary<RectTransform,float>();float cornMotionEnd;Transform cornEffect;Reference20CocosClip cornEffectClip;bool cornEffectActive;
 public float CornMotionRemaining {get{return replacement?replacement.MotionRemaining:Mathf.Max(0,cornMotionEnd-Time.time);}}
 static SkeletonDataAsset Skeleton(string name){
  SkeletonDataAsset result;if(skeletons.TryGetValue(name,out result))return result;
  result=Resources.Load<SkeletonDataAsset>("Reference20/Native/"+name+"_data");if(result){skeletons[name]=result;return result;}
  var text=Resources.Load<TextAsset>("Reference20/Native/"+name+".atlas");var mats=new List<Material>();
  for(int i=0;i<8;i++){var tex=Resources.Load<Texture2D>("Reference20/Native/"+name+"_tex"+i);if(tex==null)break;var mat=new Material(Shader.Find("Spine/Skeleton"));mat.name=name+"_tex"+i;mat.mainTexture=tex;mats.Add(mat);}
  var atlas=SpineAtlasAsset.CreateRuntimeInstance(text,mats.ToArray(),true);
  result=SkeletonDataAsset.CreateRuntimeInstance(Resources.Load<TextAsset>("Reference20/Native/"+name),atlas,true,.01f*CocosScale(name));skeletons[name]=result;return result;
 }
 static RectTransform Node(NativeNode n,Transform parent){
  var go=new GameObject(n.name,typeof(RectTransform));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);var pr=parent as RectTransform;rt.anchorMin=rt.anchorMax=pr?pr.pivot:new Vector2(.5f,.5f);rt.pivot=new Vector2(n.ax,n.ay);rt.sizeDelta=new Vector2(n.w,n.h);rt.anchoredPosition=new Vector2(n.x,n.y);rt.localScale=new Vector3(n.sx,n.sy,1);rt.localRotation=Quaternion.Euler(0,0,n.angle);
  if(n.particle!=null){var emitter=go.AddComponent<Reference20ParticleGraphic>();emitter.Configure(n.particle,Sprite(n.particle.sprite));emitter.material=SpriteMaterial(n.src,n.dst);}
  if(n.particle3d!=null){var emitter=go.AddComponent<Reference20Particle3DGraphic>();emitter.Configure(n.particle3d,Sprite(n.particle3d.sprite));emitter.material=ParticleMaterial(n.particle3d);rt.localRotation=Quaternion.Euler(n.particle3d.rotation);}
  Color color=n.color!=null&&n.color.Length>=3?new Color(n.color[0],n.color[1],n.color[2],1):Color.white;
  if(!string.IsNullOrEmpty(n.sprite)||n.mask){var im=go.AddComponent<Image>();im.sprite=Sprite(n.sprite);if(n.spriteSizeMode==1)trimmedSprites.Add(im.GetInstanceID());if(n.mask&&!im.sprite)im.sprite=Resources.Load<Sprite>("Reference20/white");im.color=color;im.raycastTarget=false;im.material=SpriteMaterial(n.src,n.dst);if(n.mask){var mask=go.AddComponent<Mask>();mask.showMaskGraphic=false;}}
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
  if(n.clips!=null&&n.clips.Length>0){var animation=go.AddComponent<Reference20CocosClip>();animation.clips=n.clips;if(n.autoplay&&n.clips[0].name.StartsWith("ele_anim_"))animation.Play(0);}
  go.SetActive(n.active);return rt;
 }
 public static RectTransform Effect(string name,RectTransform stage,Vector2 position,float scale=.8875f,float lifetime=-1){RectTransform own;if(name=="CommonBornPropsEffect")return Reference20CompanyEffects.Play("Wave",stage,position,.55f);if(Reference20Props.TryEffect(name,stage,position,scale,lifetime,out own))return own;var rt=LeaseEffect(name,stage,position,2,scale);if(!rt)return null;float duration=name=="AreaAreaEffect"||name=="SameColorChangeEffect"?2:name=="SquareBigEffect"?1.5f:name=="SquareEffect"||name=="HenHouseDestroy"?1:.6f;foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true)){clip.Play(0);duration=Mathf.Max(duration,clip.clips[0].duration);}rt.GetComponent<Reference20EffectInstance>().Lease(lifetime>0?lifetime:duration+.02f,i=>effectPools[name].Push(i));return rt;}
 public static void FinishEffect(RectTransform rt){if(!rt)return;Reference20CompanyParticles.StopChildren(rt);var instance=rt.GetComponent<Reference20EffectInstance>();if(instance)instance.Complete();}
 public static void HideEffect(RectTransform rt){if(!rt)return;var alpha=rt.GetComponent<CanvasGroup>();if(!alpha)alpha=rt.gameObject.AddComponent<CanvasGroup>();alpha.alpha=0;}
 static Sprite[] BasicFragments(int color){Sprite[] result;if(basicFragments.TryGetValue(color,out result))return result;var source=Sprite(color==4?"sprite_1":"sprite_0");var rect=source.rect;result=new Sprite[4];for(int i=0;i<4;i++){var part=new Rect(rect.x+(i%2)*rect.width*.5f,rect.y+(i/2)*rect.height*.5f,rect.width*.5f,rect.height*.5f);result[i]=UnityEngine.Sprite.Create(source.texture,part,Vector2.one*.5f,source.pixelsPerUnit);result[i].name=source.name+"_fragment_"+i;}basicFragments[color]=result;return result;}
 public static void BaseBurst(RectTransform stage,Vector2 position,int color){Reference20CompanyEffects.Play(color==4?"NormalGold":"NormalBlue",stage,position);}
 public static void ApplySpriteFrame(Image image,Sprite sprite){if(!sprite||image.sprite==sprite)return;image.sprite=sprite;if(trimmedSprites.Contains(image.GetInstanceID()))image.SetNativeSize();}
 public static void LineBlast(RectTransform stage,Vector2 position,bool horizontal,float duration){Reference20Props.LineBlast(stage,position,horizontal,duration);}
 public static void ColorBeam(RectTransform stage,Vector2 from,Vector2 to,float delay){DOTween.Sequence().SetTarget(stage).AppendInterval(delay).AppendCallback(()=>Reference20CompanyEffects.Beam(stage,from,to));}
 static void PlayClips(RectTransform rt,string name){foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(name);}
 static void PlayFlightSpine(RectTransform rt,string name,bool loop){foreach(var skeleton in rt.GetComponentsInChildren<SkeletonGraphic>(true))if(skeleton.Skeleton.Data.FindAnimation(name)!=null)skeleton.AnimationState.SetAnimation(0,name,loop);}
 public static void BeeFlight(RectTransform stage,Vector2 from,Vector2 to,float duration,int payload=0,bool reverse=false,Vector2? payloadOffset=null){
  bool combo=payload>0;var rt=Reference20Props.BeeFlight(stage,from,duration,payload,reverse);if(!rt)return;
  float launchDelay=Mathf.Max(0,duration-.5f);DOTween.Sequence().SetTarget(rt).AppendInterval(launchDelay).Append(rt.DOAnchorPos(to,.5f).SetEase(payload>0?Ease.InQuad:Ease.Linear));
  var payloadRoot=rt.Find("Payload") as RectTransform;if(payloadRoot){payloadRoot.anchoredPosition=(payloadOffset??Vector2.zero)/rt.localScale.x;DOTween.Sequence().SetTarget(rt).AppendInterval(launchDelay).Append(payloadRoot.DOAnchorPos(Vector2.zero,.5f).SetEase(Ease.InQuad));}
  var fade=rt.GetComponent<CanvasGroup>();if(!fade)fade=rt.gameObject.AddComponent<CanvasGroup>();
  DOTween.Sequence().SetTarget(rt).AppendInterval(duration).Append(rt.DOScale(Vector3.zero,.3f)).Join(fade.DOFade(0,.3f));

 }
 public void PlayAnimation(string name,bool loop=false){foreach(var s in GetComponentsInChildren<SkeletonGraphic>(true))if(s.Skeleton.Data.FindAnimation(name)!=null){s.gameObject.SetActive(true);s.AnimationState.SetAnimation(0,name,loop);}}
 public void Configure(int type,int hp){
  id=type;health=hp;animationRevision++;replacement=GetComponent<Reference20ReplacementVisual>();if(replacement){replacement.Configure(hp);return;}
  if(id==100059){cornTargets.Clear();cornMotionEnd=Time.time;cornEffect=transform.Find("Root/ArtRoot/Dynamic/headNode/Effect/Root");if(cornEffect){cornEffectClip=cornEffect.GetComponentInChildren<Reference20CocosClip>(true);if(cornEffectClip){cornEffectClip.Play("ele_anim_bread_effect");cornEffectClip.Stop();}cornEffect.gameObject.SetActive(false);cornEffectActive=false;}}

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
 public void NestFeedback(){var art=transform.Find("Root/ArtRoot");var clip=art?art.GetComponent<Reference20CocosClip>():null;if(clip)clip.Play(0);}
 public float DamageDuration(int before,int after){return replacement?replacement.DamageDuration(before,after):id==100042?(after>0?1:.33f):id==100016?(before==3?1.7f:.25f):id==100032?(before%2==0?.8f:.867f):before==3?.8f:before==2?.5f:.4f;}
 public float AnimateDamage(int before,int after){
  health=after;if(replacement){replacement.Damage(before,after);return replacement.DamageDuration(before,after);}if(id==100016)NestFeedback();int revision=++animationRevision;if(idleRoutine!=null){StopCoroutine(idleRoutine);idleRoutine=null;}
  float duration=DamageDuration(before,after);
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
 public void PlayCollected(){animationRevision++;if(replacement){replacement.Collected();return;}PlayAnimation(id==100042?"\u52a8\u65486-\u6d88\u9664":id==100032?"\u6536\u96c6":"collect_2",true);}
 public void UseChickData(SkeletonDataAsset data,string skin){
  if(!data)return;foreach(var skeleton in GetComponentsInChildren<SkeletonGraphic>(true)){
   skeleton.skeletonDataAsset=data;skeleton.Initialize(true);if(!string.IsNullOrEmpty(skin)&&skeleton.Skeleton.Data.FindSkin(skin)!=null){skeleton.Skeleton.SetSkin(skin);skeleton.Skeleton.SetSlotsToSetupPose();}
  }
 }
 public RectTransform DetachNestChick(RectTransform stage){if(replacement)return replacement.DetachPayload(stage);var eggs=GetComponentsInChildren<SkeletonGraphic>(false);if(eggs.Length==0)return null;return Detach(eggs[UnityEngine.Random.Range(0,eggs.Length)],stage,null);}
 public RectTransform DetachCornHead(RectTransform stage){if(replacement)return null;foreach(var head in GetComponentsInChildren<SkeletonGraphic>(false))if(head.name=="head")return Detach(head,stage,"collect_2");return null;}
 RectTransform Detach(SkeletonGraphic skeleton,RectTransform stage,string animation){
  var rect=skeleton.rectTransform;Raise(rect,stage);rect.localRotation=Quaternion.identity;if(!string.IsNullOrEmpty(animation))Animate(skeleton,animation,true);return rect;
 }
 float MoveCorn(RectTransform rect,float delta){float target;if(!cornTargets.TryGetValue(rect,out target))target=rect.anchoredPosition.x;target+=delta;cornTargets[rect]=target;float duration=Mathf.Abs(target-rect.anchoredPosition.x)/(128/.55f);rect.DOKill();rect.DOAnchorPosX(target,duration).SetEase(Ease.Linear);return duration;}
 void CornEffect(bool active){if(!cornEffect)return;if(active&&!cornEffectActive){cornEffect.gameObject.SetActive(true);if(cornEffectClip)cornEffectClip.Play("ele_anim_bread_effect");}else if(!active){if(cornEffectClip)cornEffectClip.Stop();cornEffect.gameObject.SetActive(false);}cornEffectActive=active;}
 public void Eat(int remaining){
  if(replacement){replacement.Feed();return;}health=remaining;int revision=++animationRevision;float duration=.55f;
  foreach(var rect in GetComponentsInChildren<RectTransform>(true)){
   if(rect.name=="headNode")duration=Mathf.Max(duration,MoveCorn(rect,128));
   if(rect.name=="Mask"){duration=Mathf.Max(duration,MoveCorn(rect,128));for(int i=0;i<rect.childCount;i++){var child=rect.GetChild(i) as RectTransform;if(child)duration=Mathf.Max(duration,MoveCorn(child,-128));}}
  }
  cornMotionEnd=Time.time+duration;CornEffect(true);if(Application.isPlaying)StartCoroutine(EatSequence(revision,duration));
 }
 IEnumerator EatSequence(int revision,float duration){PlayAnimation("eat",true);yield return new WaitForSeconds(duration);if(revision!=animationRevision)yield break;CornEffect(false);PlayAnimation("eat2",false);yield return new WaitForSeconds(.4f);if(revision==animationRevision){ApplyIdlePose();idleRoutine=StartCoroutine(IdleLoop(revision));}}

}
}

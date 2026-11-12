using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

namespace Reference20 {
public sealed class Reference20ReplacementVisual : MonoBehaviour {
 public int Kind {get;private set;}
 SkeletonGraphic rig;RectTransform contents;int feedLength;bool feeding;float feedEnd;readonly Queue<int> feeds=new Queue<int>();
 readonly List<RectTransform> payloads=new List<RectTransform>();
 static Material graphicMaterial;static Sprite leafSprite;static readonly Dictionary<string,Sprite> cabinetSprites=new Dictionary<string,Sprite>();
 public float MotionRemaining {get{return Mathf.Max(0,feedEnd-Time.time)+feeds.Count*.55f;}}
 public static bool Supports(int id){return id==100016||id==100042||id==100059||id==Reference20Game.BearId||id==3||id==4;}
 static RectTransform Rect(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=Vector2.one*.5f;r.pivot=Vector2.one*.5f;return r;}
 static Image Picture(string name,Transform parent,Sprite sprite,Vector2 position,Vector2 size){var r=Rect(name,parent);r.anchoredPosition=position;r.sizeDelta=size;var image=r.gameObject.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=false;return image;}
 static Sprite CabinetSprite(string name){Sprite sprite;if(!cabinetSprites.TryGetValue(name,out sprite)){sprite=Resources.Load<Sprite>("Reference20/Skins/Cabinet/"+name);cabinetSprites[name]=sprite;}return sprite;}
 public static void Prewarm(){for(int i=1;i<=14;i++)CabinetSprite("part_guizi_"+i.ToString("00"));}
 static SkeletonGraphic Spine(string name,Transform parent,string skin){var r=Rect(name,parent);r.sizeDelta=Vector2.zero;var sg=r.gameObject.AddComponent<SkeletonGraphic>();sg.skeletonDataAsset=Resources.Load<SkeletonDataAsset>("Reference20/Skins/"+name+"_SkeletonData");if(!graphicMaterial)graphicMaterial=Resources.Load<Material>("Reference20/Native/SpineGraphic");sg.material=graphicMaterial;sg.raycastTarget=false;sg.Initialize(true);sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();return sg;}
 public static RectTransform Create(int id,string name,Transform parent){var root=Rect(name,parent);root.sizeDelta=Vector2.zero;
  if(id==3||id==4){var sprite=Reference20Native.Sprite(id==3?"sprite_0":"sprite_1");root.sizeDelta=sprite.rect.size;var image=root.gameObject.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=false;return root;}
  var visual=root.gameObject.AddComponent<Reference20ReplacementVisual>();visual.Kind=id;
  if(id==Reference20Game.BearId){visual.rig=Spine("chandelier",root,"default");visual.rig.rectTransform.localScale=Vector3.one*(128/130.1544f);visual.Play("still",false);}
  else if(id==100042){visual.rig=Spine("bush",root,"bush_5");visual.rig.rectTransform.localScale=Vector3.one*(256/268.4032f);visual.Play("still",false);}
  else if(id==100059){int column=int.Parse(name.Split('_')[1])-4;visual.feedLength=9-column*2;visual.rig=Spine("rockchipper",root,"wood_1");visual.rig.rectTransform.localRotation=Quaternion.Euler(0,0,90);visual.rig.rectTransform.localScale=new Vector3(128/133.3f,128/150f,1);visual.Play("still"+visual.feedLength,false);}
  else if(id==100016){visual.contents=Rect("Cabinet",root);visual.contents.localScale=Vector3.one*(256/268f);var side=CabinetSprite("elements_sp_chugui_55");Picture("LeftFrame",visual.contents,side,new Vector2(-67,0),new Vector2(134,270));var right=Picture("RightFrame",visual.contents,side,new Vector2(67,0),new Vector2(134,270));right.rectTransform.localScale=new Vector3(-1,1,1);foreach(int x in new[]{-1,1})foreach(int y in new[]{1,-1}){var group=CanStack(visual.contents);group.anchoredPosition=new Vector2(x*55,y*53-5);visual.payloads.Add(group);}}
  return root;
 }
 static RectTransform CanStack(Transform parent){var root=Rect("CanStack",parent);root.sizeDelta=new Vector2(81,113);var sprite=CabinetSprite("elements_sp_chugui_2");Picture("CanTop",root,sprite,new Vector2(0,25),new Vector2(81,63));Picture("CanBottom",root,sprite,new Vector2(0,-25),new Vector2(81,63));return root;}
 public static RectTransform CreateCan(Transform stage){var root=CanStack(stage);root.gameObject.AddComponent<Reference20ReplacementVisual>().Kind=100015;return root;}
 public void Configure(int hp){if(Kind==100042)SetBushSkin(hp);}
 void SetBushSkin(int hp){string skin="bush_"+(hp>=3?5:hp==2?4:3);rig.Skeleton.SetSkin(skin);rig.Skeleton.SetSlotsToSetupPose();Play("still",false);}
 void Play(string name,bool loop){if(rig&&rig.Skeleton.Data.FindAnimation(name)!=null)rig.AnimationState.SetAnimation(0,name,loop);}
 public float DamageDuration(int before,int after){return Kind==100042?.8333333f:Kind==100016?.3f:.4f;}
 public void Damage(int before,int after){if(Kind==100042){var stage=transform.parent as RectTransform;Reference20Fragments.Burst(stage,((RectTransform)transform).anchoredPosition,"FX_Grass_sui01",16,1);if(before==3)Reference20Fragments.Burst(stage,((RectTransform)transform).anchoredPosition,"FX_Bush_01",7,.65f);if(after==0)Reference20Fragments.Burst(stage,((RectTransform)transform).anchoredPosition,"FX_Bush_guizi_01",10,1);SetBushSkin(before);var entry=rig.AnimationState.SetAnimation(0,"click",false);entry.Complete+=finished=>{if(this&&rig&&after>0&&rig.AnimationState.GetCurrent(0)==finished)SetBushSkin(after);};}else if(Kind==100016){if(contents){contents.DOKill();contents.DOShakeAnchorPos(.45f,new Vector2(7,3),14,70,false,true);}foreach(var p in payloads)if(p){p.DOKill();p.DOPunchRotation(new Vector3(0,0,7),.5f,8,.5f);}}else if(Kind==100015){transform.DOPunchScale(Vector3.one*.13f,.3f,7,.5f);}}
 public RectTransform DetachPayload(RectTransform stage){payloads.RemoveAll(p=>!p);if(payloads.Count==0)return null;int index=UnityEngine.Random.Range(0,payloads.Count);var p=payloads[index];payloads.RemoveAt(index);p.DOKill();Reference20Native.Raise(p,stage);p.localRotation=Quaternion.identity;return p;}
 public void Feed(){if(feedLength<=0)return;feeds.Enqueue(feedLength--);if(!feeding&&Application.isPlaying)StartCoroutine(FeedLoop());}
 IEnumerator FeedLoop(){feeding=true;while(feeds.Count>0){int count=feeds.Dequeue();rig.timeScale=.5f/.55f;Play("strike_"+count,false);feedEnd=Time.time+.55f;Reference20Fragments.Burst(transform.parent as RectTransform,((RectTransform)transform).anchoredPosition+Vector2.up*55,"FX_RockChipper_suishi",8,.75f);if(count==1)Reference20Fragments.Burst(transform.parent as RectTransform,((RectTransform)transform).anchoredPosition,"FX_RockChipper_suikuai",12,.8f);yield return new WaitForSeconds(.55f);if(count>1){rig.timeScale=1;Play("still"+(count-1),false);}}feeding=false;feedEnd=Time.time;}
 public void BearFalling(){if(Kind==Reference20Game.BearId)Play("falling_d",false);}
 public void BearLanded(){if(Kind!=Reference20Game.BearId)return;Play("dropped_d",false);rig.AnimationState.AddAnimation(0,"still",false,0);}
 public void BearArrived(){if(Kind==Reference20Game.BearId)Play("arrived",false);}
 public void BearFlying(){if(Kind==Reference20Game.BearId){Play("move3_u",false);rig.AnimationState.AddAnimation(0,"falling_u",false,0);}}
 public void Collected(){if(Kind==100015){transform.DORotate(new Vector3(0,0,360),1.2f,RotateMode.FastBeyond360).SetEase(Ease.InQuad);}}
 public void BreakCabinet(RectTransform stage){if(Kind==100016)Reference20Fragments.Burst(stage,((RectTransform)transform).anchoredPosition,"FX_guizi_01",18,.8f);}
 public static RectTransform Leaf(RectTransform parent){var root=Rect("BushLeaf",parent);root.sizeDelta=new Vector2(62,56);if(!leafSprite){var atlas=Resources.Load<Texture2D>("Reference20/Skins/bush");leafSprite=Sprite.Create(atlas,new Rect(1689,atlas.height-227-56,62,56),new Vector2(.5f,.5f),100);}var image=root.gameObject.AddComponent<Image>();image.sprite=leafSprite;image.raycastTarget=false;root.localScale=Vector3.one*.8875f;return root;}
}
}

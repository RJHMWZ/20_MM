using System;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class CompanySprite {public string name;}
[Serializable] public sealed class CompanyImage {public int type;public bool preserveAspect;public Color color=Color.white;public CompanySprite sprite;}
[Serializable] public sealed class CompanyText {public string text;public float fontSize;public Color color=Color.white;}
[Serializable] public sealed class CompanyNode {
 public string name;public Vector2 position,size,pivot,anchorMin=Vector2.one*.5f,anchorMax=Vector2.one*.5f;public Vector3 scale=Vector3.one,rotation;
 public CompanyImage image;public CompanyText text;public CompanyNode[] children;public Color renderedColor;
}
public partial class Reference20Game {
 bool loadingUIActive,startupBuild;
 Match3.UI.RuntimeLoadingScreen companyLoading;
 IEnumerator ShowCompanyLoading(){
  loadingUIActive=true;
  var loading=companyLoading=Match3.UI.RuntimeLoadingScreen.Create();
  yield return null;
  startupBuild=true;Build();startupBuild=false;
  float elapsed=0;
  while(elapsed<1.6f){elapsed+=Time.unscaledDeltaTime;loading.SetProgress(1-Mathf.Pow(1-Mathf.Clamp01(elapsed/1.6f),2));yield return null;}
  loading.SetProgress(1);yield return new WaitForSecondsRealtime(.12f);
  loading.Close();companyLoading=null;loadingUIActive=false;
 }
 RectTransform CompanyEndArtwork(RectTransform parent,bool won){
  var data=JsonConvert.DeserializeObject<CompanyNode>(Resources.Load<TextAsset>(won?"Reference20/Company/end-layout":"Reference20/Company/end-defeat-layout").text);
  var root=CreateCompanyNode(data,parent);root.name="CompanyEndCard";root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.sizeDelta=data.size;
  var blocker=root.GetComponent<Image>();if(blocker){blocker.raycastTarget=true;blocker.sprite=Resources.Load<Sprite>("Reference20/white");blocker.type=Image.Type.Simple;var color=data.renderedColor;blocker.color=color.maxColorComponent>0?new Color(color.r,color.g,color.b,1):new Color32(38,13,51,255);}
  if(Application.isPlaying){root.localScale=Vector3.zero;root.DOScale(Vector3.one,.5f).SetEase(Ease.OutBack);}
  return root;
 }
 RectTransform CreateCompanyNode(CompanyNode node,RectTransform parent){
  var rt=Reference20Props.Rect(node.name,parent);rt.anchorMin=node.anchorMin;rt.anchorMax=node.anchorMax;rt.anchoredPosition=node.position;rt.sizeDelta=node.size;rt.pivot=node.pivot;rt.localScale=node.scale;rt.localRotation=Quaternion.Euler(node.rotation);
  if(node.image!=null&&node.image.sprite!=null){var im=rt.gameObject.AddComponent<Image>();im.sprite=Resources.Load<Sprite>("Reference20/Company/UI/"+node.image.sprite.name);im.color=node.image.color;im.type=(Image.Type)node.image.type;im.preserveAspect=node.image.preserveAspect;im.raycastTarget=false;}
  if(node.text!=null){rt.sizeDelta=new Vector2(Mathf.Max(500,rt.sizeDelta.x),Mathf.Max(100,rt.sizeDelta.y));var label=rt.gameObject.AddComponent<Text>();label.font=Resources.Load<Font>("Reference20/Company/UI/CompanyFont");label.text=node.text.text;label.fontSize=Mathf.RoundToInt(node.text.fontSize);label.fontStyle=FontStyle.Bold;label.color=node.text.color;label.alignment=TextAnchor.MiddleCenter;label.raycastTarget=false;var outline=rt.gameObject.AddComponent<Outline>();outline.effectColor=Color.black;outline.effectDistance=new Vector2(3,-3);}
  if(node.name=="download"){var im=rt.GetComponent<Image>();if(im)im.raycastTarget=true;rt.gameObject.AddComponent<Button>().onClick.AddListener(Download);}
  if(node.children!=null)foreach(var child in node.children)CreateCompanyNode(child,rt);
  return rt;
 }
}
}

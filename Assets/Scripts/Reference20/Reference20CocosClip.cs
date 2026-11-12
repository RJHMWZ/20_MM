using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Reference20 {
public class Reference20CocosClip : MonoBehaviour {
 public NativeClip[] clips;NativeClip current;float time;
 public void Play(int index){if(clips==null||clips.Length==0)return;current=clips[Mathf.Clamp(index,0,clips.Length-1)];time=0;Apply(current.data,transform,0);}
 public bool Play(string name){int index=System.Array.FindIndex(clips,c=>c.name==name);if(index<0)return false;Play(index);return true;}
 public void Stop(bool reset=true){if(reset&&current!=null)Apply(current.data,transform,0);current=null;time=0;}
 void Update(){if(current==null)return;time+=Time.deltaTime;float t=time;if(t>current.duration){if((current.wrap&16)!=0)t=Mathf.PingPong(t,Mathf.Max(.01f,current.duration));else if(current.wrap>=2)t%=Mathf.Max(.01f,current.duration);else t=current.duration;}Apply(current.data,transform,t);if(time>current.duration&&current.wrap<2)current=null;}
 void Apply(NativeClipData data,Transform root,float t){if(data==null)return;Properties(data.props,root,t);if(data.comps!=null)foreach(var c in data.comps)Properties(c.Value,root,t);if(data.paths!=null)foreach(var p in data.paths){var target=root.Find(p.Key);if(target)Apply(p.Value,target,t);}}
 void Properties(Dictionary<string,NativeCurveKey[]> props,Transform tr,float t){if(props==null)return;foreach(var prop in props){var keys=prop.Value;if(keys==null||keys.Length==0)continue;var a=keys[0];var b=a;for(int i=0;i<keys.Length;i++){if(keys[i].frame<=t)a=keys[i];else {b=keys[i];break;}b=a;}float f=b.frame>a.frame?Mathf.Clamp01((t-a.frame)/(b.frame-a.frame)):0;var rt=tr as RectTransform;
  if(prop.Key=="spriteFrame"){var im=tr.GetComponent<Image>();if(im&&a.sprite!=null)Reference20Native.ApplySpriteFrame(im,Reference20Native.Sprite(a.sprite));}
  else if(prop.Key=="position"&&a.vector!=null){var v=Vector3.Lerp(Vec(a.vector),Vec(b.vector),f);if(rt)rt.anchoredPosition=v;else tr.localPosition=v;}
  else if(prop.Key=="scale"&&a.vector!=null){var scale=Vector3.Lerp(Vec(a.vector),Vec(b.vector),f);scale.z=1;tr.localScale=scale;}
  else if(prop.Key=="angle")tr.localRotation=Quaternion.Euler(0,0,Mathf.Lerp(a.number,b.number,f));
  else if(prop.Key=="scaleX"||prop.Key=="scaleY"){var v=tr.localScale;float z=Mathf.Lerp(a.number,b.number,f);if(prop.Key=="scaleX")v.x=z;else v.y=z;tr.localScale=v;}
  else if(prop.Key=="opacity"){var group=tr.GetComponent<CanvasGroup>();if(!group)group=tr.gameObject.AddComponent<CanvasGroup>();group.alpha=Mathf.Lerp(a.number,b.number,f)/255;}
  else if(prop.Key=="active")tr.gameObject.SetActive(a.active);
 }}
 Vector3 Vec(float[] v){return new Vector3(v[0],v[1],v.Length>2?v[2]:1);}
}
}

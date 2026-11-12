using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class CompanyEffectDefinition {
 public string name,texture;public int columns,rows,frames;public float fps,cells;public float[] times;
 public float Duration {get{return times!=null&&times.Length>0?times[times.Length-1]+1/fps:frames/fps;}}
}
[Serializable] public sealed class CompanyEffectManifest {public CompanyEffectDefinition[] effects;}
public static class Reference20CompanyEffects {
 static readonly Dictionary<string,CompanyEffectDefinition> definitions=new Dictionary<string,CompanyEffectDefinition>();
 static readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
 static readonly Stack<Reference20CompanyFlipbook> pool=new Stack<Reference20CompanyFlipbook>();
 public static void Prewarm(){
  if(definitions.Count>0)return;
  var manifest=JsonConvert.DeserializeObject<CompanyEffectManifest>(Resources.Load<TextAsset>("Reference20/Company/FX/manifest").text);
  foreach(var effect in manifest.effects){definitions[effect.name]=effect;string texture=string.IsNullOrEmpty(effect.texture)?effect.name:effect.texture;if(!textures.ContainsKey(texture))textures[texture]=Resources.Load<Texture2D>("Reference20/Company/FX/"+texture);}
 }
 public static void Reset(){pool.Clear();}
 public static RectTransform Play(string name,RectTransform stage,Vector2 position,float scale=1,bool loop=false){
  Prewarm();CompanyEffectDefinition definition;if(!definitions.TryGetValue(name,out definition))return null;
  Reference20CompanyFlipbook effect=null;while(pool.Count>0&&!effect)effect=pool.Pop();
  if(!effect){var rt=Reference20Props.Rect("CompanyFX_"+name,Reference20Native.Overlay(stage));rt.gameObject.AddComponent<RawImage>().raycastTarget=false;effect=rt.gameObject.AddComponent<Reference20CompanyFlipbook>();}
  var root=(RectTransform)effect.transform;root.SetParent(Reference20Native.Overlay(stage),false);root.anchorMin=root.anchorMax=Vector2.zero;root.pivot=Vector2.one*.5f;root.anchoredPosition=position;root.sizeDelta=Vector2.one*(113.6f*definition.cells*scale);root.localScale=Vector3.one;root.localRotation=Quaternion.identity;root.SetAsLastSibling();
  effect.Play(definition,textures[string.IsNullOrEmpty(definition.texture)?name:definition.texture],loop,item=>{item.transform.SetParent(Reference20Native.Overlay(stage),false);pool.Push(item);});return root;
 }
 public static void Beam(RectTransform stage,Vector2 from,Vector2 to){
  var rt=Play("ColorTrail",stage,(from+to)*.5f);if(!rt)return;var delta=to-from;rt.sizeDelta=new Vector2(delta.magnitude*(4/2.8f),113.6f*1.5f);rt.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
 }
}
// Rendered from the source HTML's original particle systems. A single UI quad
// preserves their meshes, sparkle, glow and alpha across Unity and Playworks.
public sealed class Reference20CompanyFlipbook : MonoBehaviour {
 CompanyEffectDefinition definition;RawImage image;float elapsed;bool loop;Action<Reference20CompanyFlipbook> release;
 public void Play(CompanyEffectDefinition value,Texture2D atlas,bool repeat,Action<Reference20CompanyFlipbook> done){definition=value;loop=repeat;release=done;elapsed=0;image=GetComponent<RawImage>();image.texture=atlas;image.color=Color.white;gameObject.SetActive(true);ApplyFrame(0);}
 void Update(){if(release==null)return;elapsed+=Time.deltaTime;float time=elapsed;if(time>=definition.Duration){if(!loop){Stop();return;}time%=definition.Duration;}int frame=0;if(definition.times!=null&&definition.times.Length>0){while(frame+1<definition.frames&&time>=definition.times[frame+1])frame++;}else frame=Mathf.Min(definition.frames-1,(int)(time*definition.fps));ApplyFrame(frame);}
 void ApplyFrame(int frame){int x=frame%definition.columns,y=frame/definition.columns;float u=.5f/image.texture.width,v=.5f/image.texture.height;image.uvRect=new Rect((float)x/definition.columns+u,1f-(float)(y+1)/definition.rows+v,1f/definition.columns-2*u,1f/definition.rows-2*v);}
 public void Stop(){if(release==null)return;var done=release;release=null;gameObject.SetActive(false);done(this);}
}
}

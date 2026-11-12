using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class CompanyParticleTrack {public float start;public float[][] samples;}
[Serializable] public sealed class CompanyParticleLayer {
 public string name,shader;public int texture;public int[] indices;public float[] tint,tiling;public float[][] vertices;public CompanyParticleTrack[] tracks;
 [NonSerialized] public Texture2D image;[NonSerialized] public bool additive;[NonSerialized] public Color color;[NonSerialized] public Material material;[NonSerialized] public string batchKey;
}
[Serializable] public sealed class CompanyParticleEffect {public string name;public float duration;public CompanyParticleLayer[] layers;}
// The source HTML's particle positions, sizes, rotations, UVs and alpha curves are
// imported at 60 Hz. Geometry is interpolated at the display rate, sharing batches
// by texture instead of allocating one UI object for every fragment.
public static class Reference20CompanyParticles {
 static readonly Dictionary<string,CompanyParticleEffect> definitions=new Dictionary<string,CompanyParticleEffect>();
 static readonly List<Reference20CompanyParticleHandle> active=new List<Reference20CompanyParticleHandle>(128);
 static readonly Stack<Reference20CompanyParticleHandle> pool=new Stack<Reference20CompanyParticleHandle>(128);
 static readonly Dictionary<string,Reference20CompanyParticleBatch> batches=new Dictionary<string,Reference20CompanyParticleBatch>();
 static readonly Dictionary<string,Material> sourceMaterials=new Dictionary<string,Material>();
 static RectTransform overlay;
 public static List<Reference20CompanyParticleHandle> Active {get{return active;}}
 public static void Load(){if(definitions.Count>0)return;foreach(string name in new[]{"NormalBlue","NormalGold","Wave","RocketTrail","ColorCharge","ColorFinish","Bomb","BeeImpact"}){
  var data=JsonConvert.DeserializeObject<CompanyParticleEffect>(Resources.Load<TextAsset>("Reference20/Company/Particles/"+name).text);
  foreach(var layer in data.layers){layer.image=Resources.Load<Texture2D>("Reference20/Company/Particles/"+layer.texture);layer.additive=layer.shader.Contains("Additive")||layer.shader.Contains("AddUV");float boost=layer.shader.StartsWith("Legacy")||layer.shader.StartsWith("Mobile")||layer.shader=="UI/AlphaBlended"?2:1;layer.color=new Color(layer.tint[0],layer.tint[1],layer.tint[2],layer.tint[3])*boost;string key=layer.shader+":"+string.Join(",",layer.tint);Material mat;if(!sourceMaterials.TryGetValue(key,out mat)){mat=new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));mat.name="HtmlParticle_"+key;mat.SetFloat("_SrcBlend",5);mat.SetFloat("_DstBlend",layer.additive?1:10);mat.SetColor("_Color",layer.color);sourceMaterials[key]=mat;}layer.material=mat;layer.batchKey=layer.texture+":"+key;}
  definitions[name]=data;
 }}
 static Reference20CompanyParticleHandle Create(){var root=Reference20Props.Rect("CompanyParticles",overlay);root.anchorMin=root.anchorMax=Vector2.zero;var h=root.gameObject.AddComponent<Reference20CompanyParticleHandle>();root.gameObject.SetActive(false);return h;}
 public static void Prewarm(RectTransform stage){Load();var target=Reference20Native.Overlay(stage);if(overlay==target&&batches.Count>0)return;overlay=target;active.Clear();pool.Clear();batches.Clear();foreach(var effect in definitions.Values)foreach(var layer in effect.layers){string key=layer.batchKey;if(batches.ContainsKey(key))continue;var rt=Reference20Props.Rect("CompanyBatch_"+layer.texture,overlay);rt.anchorMin=rt.anchorMax=Vector2.zero;var b=rt.gameObject.AddComponent<Reference20CompanyParticleBatch>();b.Configure(layer);batches[key]=b;}for(int i=0;i<128;i++)pool.Push(Create());}
 public static void Reset(){active.Clear();pool.Clear();batches.Clear();overlay=null;}
 public static void RaiseAboveFocus(){foreach(var batch in batches.Values)if(batch)batch.transform.SetAsLastSibling();}
 public static RectTransform Play(string name,RectTransform stage,Vector2 position,float scale=1,bool loop=false){
  Prewarm(stage);CompanyParticleEffect definition;if(!definitions.TryGetValue(name,out definition))return null;
  Reference20CompanyParticleHandle h=null;while(pool.Count>0&&!h)h=pool.Pop();if(!h)h=Create();var rt=(RectTransform)h.transform;rt.SetParent(overlay,false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=position;rt.localRotation=Quaternion.identity;rt.localScale=Vector3.one*(scale*(name=="NormalBlue"||name=="NormalGold"||name=="Wave"?1:1/Reference20Props.SourceCellSize));h.Play(definition,loop,name=="ColorCombo");active.Add(h);return rt;
 }
 public static void Release(Reference20CompanyParticleHandle h){active.Remove(h);if(overlay)h.transform.SetParent(overlay,false);pool.Push(h);}
 public static void MovingTrail(RectTransform stage,Vector2 from,Vector2 direction,float duration){
  var rt=Play("RocketTrail",stage,from);var h=rt.GetComponent<Reference20CompanyParticleHandle>();h.Velocity=Vector2.right*(113.6f*15/duration);rt.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg);
 }
 public static void StopChildren(Transform root){foreach(var h in root.GetComponentsInChildren<Reference20CompanyParticleHandle>(true))h.Stop();}

}
public sealed class Reference20CompanyParticleHandle : MonoBehaviour {
 public CompanyParticleEffect Definition;public float Elapsed,SampleRate=60;public bool Combo;public Vector2 Velocity;bool loop,leased;
 public void Play(CompanyParticleEffect effect,bool repeat,bool combo){enabled=true;Definition=effect;Elapsed=0;SampleRate=60;Velocity=Vector2.zero;loop=repeat;Combo=combo;leased=true;gameObject.SetActive(true);}
 void Update(){if(!leased)return;Elapsed+=Time.deltaTime;if(Elapsed<Definition.duration)return;if(loop)Elapsed%=Definition.duration;else Stop();}
 public void Stop(){if(!leased)return;leased=false;gameObject.SetActive(false);Reference20CompanyParticles.Release(this);}
}
[RequireComponent(typeof(CanvasRenderer))]
public sealed class Reference20CompanyParticleBatch : MaskableGraphic {
 public Reference20CompanyParticleBatch(){useLegacyMeshGeneration=false;}
 Texture2D texture;string batchKey;bool wasActive;readonly UIVertex[] quad=new UIVertex[4];
 public override Texture mainTexture {get{return texture?texture:Texture2D.whiteTexture;}}
 public void Configure(CompanyParticleLayer layer){texture=layer.image;batchKey=layer.batchKey;material=layer.material;raycastTarget=false;maskable=false;}
 void LateUpdate(){bool any=Reference20CompanyParticles.Active.Count>0;if(any||wasActive)SetVerticesDirty();wasActive=any;}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(!texture)return;foreach(var h in Reference20CompanyParticles.Active){if(!h||!h.gameObject.activeInHierarchy)continue;var matrix=transform.worldToLocalMatrix*h.transform.localToWorldMatrix;foreach(var layer in h.Definition.layers){if(layer.batchKey!=batchKey)continue;int count=layer.tracks.Length;for(int particle=0;particle<count;particle++){
   var track=layer.tracks[particle];float frame=(h.Elapsed-track.start)*h.SampleRate;if(frame<0||frame>track.samples.Length-1)continue;int a=(int)frame,b=Mathf.Min(a+1,track.samples.Length-1);float f=frame-a;var va=track.samples[a];var vb=track.samples[b];bool valid=true;for(int j=0;j<va.Length;j++)if(float.IsNaN(va[j])||float.IsInfinity(va[j])||float.IsNaN(vb[j])||float.IsInfinity(vb[j])){valid=false;break;}if(!valid)continue;var tint=new Color(Mathf.LerpUnclamped(va[0],vb[0],f),Mathf.LerpUnclamped(va[1],vb[1],f),Mathf.LerpUnclamped(va[2],vb[2],f),Mathf.LerpUnclamped(va[3],vb[3],f));if(tint.a<.002f)continue;int vertices=(va.Length-4)/4;if(vertices!=4||vh.currentVertCount>64000)continue;
   for(int i=0;i<4;i++){int j=4+i*4;quad[i]=UIVertex.simpleVert;quad[i].position=matrix.MultiplyPoint3x4(new Vector3(Mathf.LerpUnclamped(va[j],vb[j],f)+h.Velocity.x*track.start,Mathf.LerpUnclamped(va[j+1],vb[j+1],f)+h.Velocity.y*track.start,0));quad[i].uv0=new Vector2(va[j+2]*layer.tiling[0]+layer.tiling[2],va[j+3]*layer.tiling[1]+layer.tiling[3]);quad[i].color=tint;}
   int offset=vh.currentVertCount;for(int i=0;i<4;i++)vh.AddVert(quad[i]);for(int i=0;i+2<layer.indices.Length;i+=3)vh.AddTriangle(offset+layer.indices[i],offset+layer.indices[i+1],offset+layer.indices[i+2]);
  }}}
  // Keep finite mesh bounds while a shared texture batch has no live particles.
  if(vh.currentVertCount==0){vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddTriangle(0,1,2);}
 }
}
}

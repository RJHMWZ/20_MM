using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
public static class Reference20Fragments {
 [Serializable] public sealed class Region {public int x,y,w,h;}
 [Serializable] public sealed class Sheet {public string name;public Region[] regions;}
 [Serializable] sealed class Manifest {public Sheet[] sheets;}
 static readonly Dictionary<string,Sheet> sheets=new Dictionary<string,Sheet>();
 static readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
 static readonly Dictionary<string,Stack<Reference20FragmentBurst>> pools=new Dictionary<string,Stack<Reference20FragmentBurst>>();
 static void Load(){if(sheets.Count>0)return;var manifest=Newtonsoft.Json.JsonConvert.DeserializeObject<Manifest>(Resources.Load<TextAsset>("Reference20/Props/Fragments/regions").text);foreach(var sheet in manifest.sheets){sheets[sheet.name]=sheet;textures[sheet.name]=Resources.Load<Texture2D>("Reference20/Props/Fragments/"+sheet.name);}}
 public static void Prewarm(){Load();}
 public static void Reset(){pools.Clear();}
 public static void Burst(RectTransform stage,Vector2 position,string name,int count=12,float scale=1){
  Load();Sheet sheet;if(!sheets.TryGetValue(name,out sheet)||sheet.regions.Length==0)return;Stack<Reference20FragmentBurst> pool;if(!pools.TryGetValue(name,out pool)){pool=new Stack<Reference20FragmentBurst>();pools[name]=pool;}
  Reference20FragmentBurst burst=null;while(pool.Count>0&&!burst)burst=pool.Pop();if(!burst){var r=Reference20Props.Rect("Fragments_"+name,Reference20Native.Overlay(stage));burst=r.gameObject.AddComponent<Reference20FragmentBurst>();}
  var root=burst.rectTransform;root.SetParent(Reference20Native.Overlay(stage),false);root.anchorMin=root.anchorMax=Vector2.zero;root.anchoredPosition=position;root.localScale=Vector3.one*(.8875f*scale);root.SetAsLastSibling();burst.Play(textures[name],sheet.regions,count,()=>pool.Push(burst));
 }
}
[RequireComponent(typeof(CanvasRenderer))]
public sealed class Reference20FragmentBurst : MaskableGraphic {
 public Reference20FragmentBurst(){useLegacyMeshGeneration=false;}
 struct Fragment {public Reference20Fragments.Region region;public Vector2 start,velocity;public float angle,spin,size,life;}
 readonly List<Fragment> parts=new List<Fragment>();readonly UIVertex[] quad=new UIVertex[4];Texture2D texture;float elapsed;Action complete;
 public override Texture mainTexture {get{return texture?texture:Texture2D.whiteTexture;}}
 public void Play(Texture2D atlas,Reference20Fragments.Region[] regions,int count,Action done){texture=atlas;complete=done;elapsed=0;parts.Clear();raycastTarget=false;for(int i=0;i<count;i++){var region=regions[UnityEngine.Random.Range(0,regions.Length)];float angle=UnityEngine.Random.Range(0,Mathf.PI*2);parts.Add(new Fragment{region=region,start=UnityEngine.Random.insideUnitCircle*16,velocity=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*UnityEngine.Random.Range(80f,260f)+Vector2.up*90,angle=UnityEngine.Random.Range(-Mathf.PI,Mathf.PI),spin=UnityEngine.Random.Range(-8f,8f),size=UnityEngine.Random.Range(.5f,.95f)*Mathf.Min(1,40f/Mathf.Max(region.w,region.h)),life=UnityEngine.Random.Range(.45f,.75f)});}gameObject.SetActive(true);SetMaterialDirty();SetVerticesDirty();}
 void Update(){if(complete==null)return;elapsed+=Time.deltaTime;if(elapsed>=.8f){var done=complete;complete=null;gameObject.SetActive(false);done();return;}SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(!texture)return;foreach(var p in parts){if(elapsed>p.life)continue;var r=p.region;float angle=p.angle+p.spin*elapsed;Vector2 right=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*r.w*p.size*.5f;Vector2 up=new Vector2(-Mathf.Sin(angle),Mathf.Cos(angle))*r.h*p.size*.5f;var pos=p.start+p.velocity*elapsed+Vector2.down*450*elapsed*elapsed;Color tint=new Color(1,1,1,1-Mathf.InverseLerp(p.life*.6f,p.life,elapsed));for(int i=0;i<4;i++){quad[i]=UIVertex.simpleVert;quad[i].color=tint;}quad[0].position=pos-right-up;quad[1].position=pos-right+up;quad[2].position=pos+right+up;quad[3].position=pos+right-up;float x=(float)r.x/texture.width,y=(float)r.y/texture.height,x1=(float)(r.x+r.w)/texture.width,y1=(float)(r.y+r.h)/texture.height;quad[0].uv0=new Vector2(x,y);quad[1].uv0=new Vector2(x,y1);quad[2].uv0=new Vector2(x1,y1);quad[3].uv0=new Vector2(x1,y);vh.AddUIVertexQuad(quad);}}
}
}

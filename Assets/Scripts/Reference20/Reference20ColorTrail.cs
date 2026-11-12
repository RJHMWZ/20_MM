using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class HtmlTrailFrame {public float time;public float[][] vertices;public int[] indices;}
[Serializable] public sealed class HtmlTrailLayer {public string name,shader;public int texture,blend=1;public float[] tint,tiling,flipbook,uvSpeed;public float switchValue,frame=1;public HtmlTrailFrame[] frames;[NonSerialized] public Material material;}
[Serializable] public sealed class HtmlTrailData {public float duration;public HtmlTrailLayer[] layers;}

// Geometry, vertex colours, widths and UVs come from the reference TrailRenderers.
// Only the longitudinal coordinate changes with the distance to the target.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class Reference20ColorTrail : MaskableGraphic {
 public Reference20ColorTrail(){useLegacyMeshGeneration=false;}
 static HtmlTrailData source;
 HtmlTrailLayer layer;Texture2D texture;float elapsed,length;
 public override Texture mainTexture {get{return texture?texture:Texture2D.whiteTexture;}}
 public static void Play(RectTransform stage,Vector2 from,Vector2 to){
  if(source==null)source=JsonConvert.DeserializeObject<HtmlTrailData>(Resources.Load<TextAsset>("Reference20/Company/Particles/ColorTrail").text);
  var root=Reference20Props.Rect("HtmlColorTrail",Reference20Native.Overlay(stage));root.anchorMin=root.anchorMax=Vector2.zero;root.anchoredPosition=from;var delta=to-from;root.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
  foreach(var data in source.layers){var rt=Reference20Props.Rect(data.name,root);var g=rt.gameObject.AddComponent<Reference20ColorTrail>();g.layer=data;g.length=delta.magnitude;g.texture=Resources.Load<Texture2D>("Reference20/Company/Particles/"+data.texture);if(!data.material){data.material=new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));float boost=data.shader!=null&&data.shader.StartsWith("Legacy")?2:1;data.material.SetColor("_Color",new Color(data.tint[0],data.tint[1],data.tint[2],data.tint[3])*boost);data.material.SetFloat("_SrcBlend",5);data.material.SetFloat("_DstBlend",data.blend);}g.material=data.material;g.raycastTarget=false;g.maskable=false;}
  UnityEngine.Object.Destroy(root.gameObject,.5f);
 }
 void Update(){elapsed+=Time.deltaTime;SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){
  vh.Clear();if(layer==null){Empty(vh);return;}HtmlTrailFrame frame=null;foreach(var f in layer.frames){if(f.time>elapsed)break;frame=f;}if(frame==null||frame.vertices==null){Empty(vh);return;}
  foreach(var v in frame.vertices){if(v.Length<8)continue;var c=new Color(v[4],v[5],v[6],v[7]);var uv=new Vector2(v[2]*layer.tiling[0]+layer.tiling[2],v[3]*layer.tiling[1]+layer.tiling[3]);if(layer.flipbook!=null&&layer.flipbook.Length==4){var fb=layer.flipbook;float t=Mathf.LerpUnclamped(Time.time,layer.frame-1,layer.switchValue);float index=Mathf.Round((t*fb[2]+fb[3]-1)%(fb[0]*fb[1]));if(index<0)index+=fb[0]*fb[1];float column=Mathf.Round(index%fb[0]),row=Mathf.Round(((index-column)/fb[0])%fb[1]);uv=new Vector2((uv.x+Time.time*layer.uvSpeed[0]+column)/fb[0],(uv.y+Time.time*layer.uvSpeed[1]+fb[1]-1-row)/fb[1]);}vh.AddVert(new Vector3(v[0]*length,v[1]/Reference20Props.SourceCellSize,0),c,uv);}
  for(int i=0;i+2<frame.indices.Length;i+=3){int a=frame.indices[i],b=frame.indices[i+1],c=frame.indices[i+2];if(a>=0&&b>=0&&c>=0&&a<vh.currentVertCount&&b<vh.currentVertCount&&c<vh.currentVertCount)vh.AddTriangle(a,b,c);}
  if(vh.currentVertCount==0){vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddTriangle(0,1,2);}
 }
 static void Empty(VertexHelper vh){vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddTriangle(0,1,2);}
}
}

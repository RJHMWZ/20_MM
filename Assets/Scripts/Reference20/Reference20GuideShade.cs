using UnityEngine;
using UnityEngine.UI;
namespace Reference20 {
[ExecuteAlways]
public class Reference20GuideShade : MaskableGraphic {
 public Reference20GuideShade(){useLegacyMeshGeneration=false;}
 Vector2 lastSize;Vector3 lastScale;
 protected override void OnPopulateMesh(VertexHelper vh){
  vh.Clear();if(!canvas)return;
  var bounds=canvas.rootCanvas.transform as RectTransform;var corners=new Vector3[4];bounds.GetWorldCorners(corners);
  var min=rectTransform.InverseTransformPoint(corners[0]);var max=rectTransform.InverseTransformPoint(corners[2]);
  // Original two-cell guide opening, expressed in the board's pixel coordinate space.
  float left=284f,right=511.2f,bottom=113.6f,top=227.2f;
  Quad(vh,min.x,min.y,max.x,bottom);Quad(vh,min.x,top,max.x,max.y);
  Quad(vh,min.x,bottom,left,top);Quad(vh,right,bottom,max.x,top);
 }
 void Quad(VertexHelper vh,float x0,float y0,float x1,float y1){if(x1<=x0||y1<=y0)return;int i=vh.currentVertCount;var c=(Color32)color;vh.AddVert(new Vector3(x0,y0),c,Vector2.zero);vh.AddVert(new Vector3(x0,y1),c,Vector2.zero);vh.AddVert(new Vector3(x1,y1),c,Vector2.zero);vh.AddVert(new Vector3(x1,y0),c,Vector2.zero);vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);}
 void LateUpdate(){if(!canvas)return;var size=(canvas.rootCanvas.transform as RectTransform).rect.size;var scale=rectTransform.lossyScale;if(size!=lastSize||scale!=lastScale){lastSize=size;lastScale=scale;SetVerticesDirty();}}
}
}

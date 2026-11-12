using UnityEngine;
using UnityEngine.UI;
namespace Reference20 {
[ExecuteAlways]
public class Reference20ResponsiveLayout : MonoBehaviour {
 public RectTransform board,background,badge,download,endFrame;
 RectTransform canvasRect;
 void OnEnable(){canvasRect=transform as RectTransform;}
 void LateUpdate(){Refresh();}
 public void Refresh(){
  if(!canvasRect)canvasRect=transform as RectTransform;
  if(!canvasRect||!board)return;
  Vector2 size=canvasRect.rect.size;if(size.x<=0||size.y<=0)return;
  Rect safe=Screen.safeArea;
  if(Screen.width<=0||Screen.height<=0)safe=new Rect(0,0,size.x,size.y);
  float rx=size.x/Mathf.Max(1,Screen.width),ry=size.y/Mathf.Max(1,Screen.height);
  var area=new Rect(safe.x*rx,safe.y*ry,safe.width*rx,safe.height*ry);
  if(area.width<=0||area.height<=0)area=new Rect(0,0,size.x,size.y);
  bool landscape=size.x>size.y;
  // Exact Game.Adpater formula from the reference: 9 columns x 12 rows, 128 pixels.
  float mapScale=Mathf.Max(.01f,Mathf.Min(area.width/(9*128f),area.height/(12*128f))-.05f);
  float scale=mapScale/.8875f;
  Vector2 center=area.center;
  board.anchorMin=board.anchorMax=new Vector2(.5f,.5f);board.pivot=new Vector2(.5f,.5f);board.sizeDelta=new Vector2(1080,1920);
  board.localScale=Vector3.one*scale;board.anchoredPosition=center-size*.5f;
  if(background){
   background.anchorMin=background.anchorMax=new Vector2(.5f,.5f);background.anchoredPosition=Vector2.zero;background.localScale=Vector3.one;
   float k=Mathf.Max(size.x/1079,size.y/1920);background.sizeDelta=new Vector2(1079,1920)*k;
  }
  if(badge){badge.anchorMin=badge.anchorMax=Vector2.zero;badge.pivot=new Vector2(0,1);badge.anchoredPosition=new Vector2(area.x,area.yMax);badge.sizeDelta=new Vector2(350,350);badge.localScale=Vector3.one;}
  if(download){download.gameObject.SetActive(!landscape);download.anchorMin=download.anchorMax=Vector2.zero;download.pivot=new Vector2(.5f,.5f);download.localScale=Vector3.one;
   download.sizeDelta=new Vector2(423,159);download.anchoredPosition=new Vector2(area.center.x,area.y+139.776f);
  }
  if(endFrame){endFrame.anchorMin=endFrame.anchorMax=Vector2.zero;endFrame.pivot=new Vector2(.5f,.5f);endFrame.anchoredPosition=area.center;endFrame.sizeDelta=new Vector2(1080,1920);endFrame.localScale=Vector3.one*Mathf.Min(area.width/1080,area.height/1920);}
 }
}
}

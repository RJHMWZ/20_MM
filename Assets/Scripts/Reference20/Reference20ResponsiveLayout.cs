using UnityEngine;
using UnityEngine.UI;
namespace Reference20 {
[ExecuteAlways]
public class Reference20ResponsiveLayout : MonoBehaviour {
 public RectTransform board,background,badge,download,endFrame;
 RectTransform canvasRect;
 bool layoutValid;Vector2 lastCanvasSize;Rect lastSafeArea;int lastScreenWidth,lastScreenHeight;
 RectTransform lastBoard,lastBackground,lastBadge,lastDownload,lastEndFrame;
 void OnEnable(){canvasRect=transform as RectTransform;layoutValid=false;}
 void LateUpdate(){Refresh();}
 public bool TryScreenToBoard(Vector2 screenPoint,out Vector2 point){
  point=Vector2.zero;if(!canvasRect)canvasRect=transform as RectTransform;
  if(!canvasRect||!board||Screen.width<=0||Screen.height<=0)return false;
  var size=canvasRect.rect.size;var scale=board.localScale;
  if(size.x<=0||size.y<=0||Mathf.Abs(scale.x)<.00001f||Mathf.Abs(scale.y)<.00001f)return false;
  // The board is centered in the overlay canvas. Work in its bottom-left pixel
  // coordinates, matching CellPosition, regardless of canvas scaling or aspect.
  var center=size*.5f+board.anchoredPosition;
  point=new Vector2((screenPoint.x*size.x/Screen.width-center.x)/scale.x+board.sizeDelta.x*.5f,(screenPoint.y*size.y/Screen.height-center.y)/scale.y+board.sizeDelta.y*.5f);
  return true;
 }
 public void Refresh(){
  if(!canvasRect)canvasRect=transform as RectTransform;
  if(!canvasRect||!board)return;
  Vector2 size=canvasRect.rect.size;if(size.x<=0||size.y<=0)return;
  Rect safe=Screen.safeArea;
  // Luna dirties the complete UI hierarchy on transform setters, even when the
  // assigned value is unchanged. Only apply layout when its inputs change.
  if(layoutValid&&size==lastCanvasSize&&safe==lastSafeArea&&Screen.width==lastScreenWidth&&Screen.height==lastScreenHeight&&board==lastBoard&&background==lastBackground&&badge==lastBadge&&download==lastDownload&&endFrame==lastEndFrame)return;
  layoutValid=true;lastCanvasSize=size;lastSafeArea=safe;lastScreenWidth=Screen.width;lastScreenHeight=Screen.height;
  lastBoard=board;lastBackground=background;lastBadge=badge;lastDownload=download;lastEndFrame=endFrame;
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
  if(endFrame){endFrame.anchorMin=Vector2.zero;endFrame.anchorMax=Vector2.one;endFrame.pivot=new Vector2(.5f,.5f);endFrame.anchoredPosition=Vector2.zero;endFrame.sizeDelta=Vector2.zero;endFrame.localScale=Vector3.one;}
 }
}
}

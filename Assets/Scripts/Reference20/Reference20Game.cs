using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Reference20 {
[Serializable] public class Picture { public string asset; public float left,top,width,height; }
[Serializable] public class PieceRecord : Picture { public string name; public int id; public float x,y; }
[Serializable] public class Layout { public Picture[] layers; public PieceRecord[] pieces; }
public partial class Reference20Game : MonoBehaviour {
 public RectTransform stage;
 public Match3.GameManager templateDownload;
 public int maxMoves=25;
 [NonSerialized] public bool InputEnabled=true;
 public int MovesPlayed {get;private set;}
 public bool IsBusy {get;private set;}
 public bool IsFinished {get;private set;}
 bool settlementPending;
 public int ClearedCount {get;private set;}
 class Piece {
  public int x,y,w=1,h=1,id,hp=1;public RectTransform root;public Image image;public Vector2 initial;public int originalHeight;public float settledAt;public bool falling,dying,reserved;public float fallTime,lockedUntil,markedUntil,spawnedAt=-1;public Vector2 simPosition,previousPosition,visualOffset;public bool paused,colorConverted,switching;public int cornQueued,cornBlockedRow=-1;public bool cornCycle;
  public bool special {get{return id>=11&&id<=15;}}public bool normal {get{return id<100;}}public bool basic {get{return id==3||id==4;}}public bool bear {get{return id==BearId;}}public bool movable {get{return normal||id==100015||bear;}}
  public Spine.Unity.SkeletonDataAsset chickData;public string chickSkin;
 }
 class MatchGroup {public List<Piece> members=new List<Piece>();public int skill;public Piece origin;}
 readonly List<Piece> pendingFlights=new List<Piece>();
 readonly Dictionary<string,AudioClip> sounds=new Dictionary<string,AudioClip>();
 int[] entryRows=new int[9];int pendingCollections;

 readonly List<Piece> pieces=new List<Piece>();
 readonly Dictionary<int,Sprite> normalSprites=new Dictionary<int,Sprite>();
 bool[,] playable=new bool[9,12];
 Piece[,] grid=new Piece[9,12];
 int randomSeed=6;
 readonly int[] lastDropColor=new int[9];
 int RandomRange(int min,int max){randomSeed=(int)((9301L*randomSeed+49297L)%233280L);return min+(int)Math.Floor((randomSeed/233280d)*(max-min)+.5d);}
 RectTransform guide,endPanel,chrome;
 Reference20ResponsiveLayout responsive;
 Vector2 press;
 Piece selected;
 AudioSource audioSource,standbySource;int standbyCount;
 readonly List<AudioSource> flightAudio=new List<AudioSource>();readonly Stack<AudioSource> availableFlightAudio=new Stack<AudioSource>();
 Vector2 cellOrigin=new Vector2(85.6f,1584.8f);
 const float Cell=113.6f;
 Font font;
 IEnumerator Start(){Application.runInBackground=true;Application.targetFrameRate=60;yield return ShowCompanyLoading();}
 public void Build(){
  stage.gameObject.SetActive(true);
  if(Application.isPlaying&&!startupBuild)StopAllCoroutines();if(!startupBuild){if(companyLoading!=null)companyLoading.Close();companyLoading=null;loadingUIActive=false;}if(standbySource)standbySource.Stop();standbyCount=0;availableFlightAudio.Clear();foreach(var source in flightAudio){source.Stop();source.clip=null;availableFlightAudio.Push(source);}ResetRuntime();Reference20Native.ResetCaches();pendingFlights.Clear();pendingCollections=0;randomSeed=6;Array.Clear(lastDropColor,0,lastDropColor.Length);
  for(int i=stage.childCount-1;i>=0;i--){var child=stage.GetChild(i);foreach(var rt in child.GetComponentsInChildren<RectTransform>(true))rt.DOKill();child.gameObject.SetActive(false);if(Application.isPlaying)Destroy(child.gameObject);else DestroyImmediate(child.gameObject);}
  StopHints();Phase=TurnPhase.Ready;selected=null;pieces.Clear();normalSprites.Clear();playable=new bool[9,12];MovesPlayed=0;IsFinished=false;settlementPending=false;IsBusy=false;ClearedCount=0;
  font=Resources.Load<Font>("Reference20/Company/UI/CompanyFont");
  var layout=Newtonsoft.Json.JsonConvert.DeserializeObject<Layout>(Resources.Load<TextAsset>("Reference20/layout").text);
  audioSource=GetComponent<AudioSource>();if(!audioSource)audioSource=gameObject.AddComponent<AudioSource>();audioSource.playOnAwake=false;
  sounds.Clear();foreach(var clip in Resources.LoadAll<AudioClip>("Reference20/Company/Audio")){sounds[clip.name]=clip;}EnsureAudio();
  // Preserve the reference's screen-space layout while keeping every game piece interactive.
  normalSprites[3]=Reference20Native.Sprite("sprite_0");normalSprites[4]=Reference20Native.Sprite("sprite_1");
  foreach(var l in layout.layers) if(l.asset!="decoration"&&l.asset!="background") AddPicture(l,stage);
  foreach(var r in layout.pieces){
   string[] a=r.name.Split('_');int col=int.Parse(a[1])-4,row=int.Parse(a[0])-3;
   var p=new Piece {id=r.id,x=col,y=row};
   if(p.id==100016||p.id==100042){p.w=2;p.h=2;p.hp=p.id==100016?2:3;}
   if(p.id==100032){CreateNormal(col,row,col%2==0?3:4,false);CreateBear(col,row+1);continue;}
   if(p.id==100059){p.h=10-col*2;p.y=12-p.h;p.originalHeight=p.h;}
   p.root=Reference20Native.Piece(r.name,stage);
   if(p.root==null)p.root=AddPicture(r,stage);
   p.image=p.root.GetComponent<Image>();if(!p.image){p.image=p.root.gameObject.AddComponent<Image>();p.image.enabled=false;}
   p.initial=p.root.anchoredPosition;p.visualOffset=p.initial-CellPosition(p.x,p.y);p.simPosition=p.initial;p.image.raycastTarget=false;
   var actor=p.root.GetComponent<Reference20Native>();if(actor)actor.Configure(p.id,p.hp);
   if(p.id==100016){var egg=p.root.GetComponentInChildren<Spine.Unity.SkeletonGraphic>(true);if(egg){p.chickData=egg.skeletonDataAsset;p.chickSkin=egg.Skeleton.Skin==null?"default":egg.Skeleton.Skin.Name;}}
   pieces.Add(p);
   if(p.normal&&!normalSprites.ContainsKey(p.id))normalSprites[p.id]=Reference20Native.Sprite(p.id==3?"sprite_0":"sprite_1");
   for(int x=p.x;x<p.x+p.w;x++)for(int y=p.y;y<p.y+p.h;y++)if(Inside(x,y))playable[x,y]=true;
  }
  Reindex();ApplySourceTopology();
  BuildChrome();BuildGuide();Reference20Native.Prewarm();if(Application.isPlaying){WarmNormalPool();Reference20Native.WarmEffects(stage);Reference20Native.WarmVisuals(stage);}
 }
 void ClearChrome(string name){var old=stage.parent.Find(name);if(!old)return;foreach(var rt in old.GetComponentsInChildren<RectTransform>(true))rt.DOKill();old.gameObject.SetActive(false);if(Application.isPlaying)Destroy(old.gameObject);else DestroyImmediate(old.gameObject);}
 void BuildChrome(){
  ClearChrome("Reference20Backdrop");ClearChrome("Reference20Chrome");
  var canvas=stage.parent as RectTransform;
  var bg=Box("Reference20Backdrop",canvas,Vector2.zero,new Vector2(1079,1920),Color.white);bg.GetComponent<Image>().sprite=Resources.Load<Sprite>("Reference20/background");bg.GetComponent<Image>().raycastTarget=false;bg.SetAsFirstSibling();
  chrome=new GameObject("Reference20Chrome",typeof(RectTransform)).GetComponent<RectTransform>();chrome.SetParent(canvas,false);chrome.anchorMin=Vector2.zero;chrome.anchorMax=Vector2.one;chrome.offsetMin=chrome.offsetMax=Vector2.zero;chrome.SetAsLastSibling();
  var badge=Box("Ribbon",chrome,Vector2.zero,new Vector2(350,350),Color.white);badge.GetComponent<Image>().sprite=Resources.Load<Sprite>("Reference20/ribbon");badge.GetComponent<Image>().raycastTarget=false;
  var button=Box("DownloadButton",chrome,Vector2.zero,new Vector2(440,170),Color.white);button.GetComponent<Image>().color=new Color(1,1,1,0);Reference20Native.DownloadArtwork(button);button.gameObject.AddComponent<Button>().onClick.AddListener(Download);
  responsive=canvas.GetComponent<Reference20ResponsiveLayout>();if(!responsive)responsive=canvas.gameObject.AddComponent<Reference20ResponsiveLayout>();responsive.board=stage;responsive.background=bg;responsive.badge=badge;responsive.download=button;responsive.endFrame=null;responsive.Refresh();
 }

 RectTransform AddPicture(Picture p,Transform parent){
  var rt=Box(p.asset,parent,new Vector2(p.left+p.width/2,1920-p.top-p.height/2),new Vector2(p.width,p.height),Color.white);
  var im=rt.GetComponent<Image>();im.sprite=Resources.Load<Sprite>("Reference20/"+p.asset);im.raycastTarget=false;return rt;
 }
 RectTransform Box(string name,Transform parent,Vector2 pos,Vector2 size,Color color){
  var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;go.GetComponent<Image>().color=color;go.GetComponent<Image>().sprite=Resources.Load<Sprite>("Reference20/white");return rt;
 }
 Text Label(string name,Transform parent,string value,Vector2 pos,Vector2 size,int fontSize){
  var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);rt.anchorMin=rt.anchorMax=Vector2.zero;rt.anchoredPosition=pos;rt.sizeDelta=size;var t=go.GetComponent<Text>();t.text=value;t.font=font;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.raycastTarget=false;return t;
 }
 void BuildGuide(){
  var shadeObject=new GameObject("TutorialShade",typeof(RectTransform),typeof(CanvasRenderer),typeof(Reference20GuideShade));guide=shadeObject.GetComponent<RectTransform>();guide.SetParent(stage,false);guide.anchorMin=guide.anchorMax=Vector2.one*.5f;guide.sizeDelta=new Vector2(1080,1920);var shade=shadeObject.GetComponent<Reference20GuideShade>();shade.color=new Color(0,0,0,.55f);shade.raycastTarget=false;
  var top=guide.gameObject.AddComponent<Canvas>();top.overrideSorting=true;top.sortingOrder=500;
  var hand=Reference20Native.GuideHand(stage);if(hand)hand.SetParent(guide,true);
  Hint(grid[7,4]);Hint(grid[8,4]);
 }
 void RemoveGuide(){StopHints();if(guide){guide.DOKill();foreach(var t in guide.GetComponentsInChildren<RectTransform>())t.DOKill();Destroy(guide.gameObject);guide=null;}}
 void EnsureAudio(){
  audioSource.spatialBlend=0;audioSource.volume=1;audioSource.mute=false;
  if(!FindObjectOfType<AudioListener>()){var camera=Camera.main;if(camera)camera.gameObject.AddComponent<AudioListener>();else gameObject.AddComponent<AudioListener>();}
 }
 void Sound(string name,float volume=.8f){
  name=Reference20CompanyAudio.Name(name);if(name==null||!Application.isPlaying||!audioSource)return;AudioClip clip;
  if(!sounds.TryGetValue(name,out clip)){clip=Resources.Load<AudioClip>("Reference20/Company/Audio/"+name);sounds[name]=clip;}
  if(clip)audioSource.PlayOneShot(clip,volume);
 }
 void Update(){
  AdvanceRuntime(Time.deltaTime);
  if(!InputEnabled)return;
  if(!CanAcceptInput){selected=null;return;}if(selected!=null&&!CanManipulate(selected))selected=null;
  IdleFeedback();
  if(Input.GetMouseButtonDown(0)){
   StopHints();
   Vector2 point;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,Input.mousePosition,null,out point);press=point+new Vector2(540,960);
   var piece=AtPoint(press);
   if(selected!=null&&piece!=null&&piece!=selected&&TryMove(selected.x,selected.y,piece.x,piece.y)){selected=null;return;}
   selected=CanManipulate(piece)?piece:null;Hint(selected);
  }
  if(Input.GetMouseButton(0)&&selected!=null){Vector2 point;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,Input.mousePosition,null,out point);DragSelected(point+new Vector2(540,960));}
  if(Input.GetMouseButtonUp(0)&&selected!=null){
   Vector2 point;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,Input.mousePosition,null,out point);var delta=point+new Vector2(540,960)-press;
   if(delta.magnitude>Cell*.22f){int dx=0,dy=0;if(Mathf.Abs(delta.x)>Mathf.Abs(delta.y))dx=delta.x>0?1:-1;else dy=delta.y>0?-1:1;TryMove(selected.x,selected.y,selected.x+dx,selected.y+dy);selected=null;}
   else if(selected.special){TryActivate(selected.x,selected.y);selected=null;}
  }
 }
 Piece AtPoint(Vector2 point){int x=Mathf.RoundToInt((point.x-cellOrigin.x)/Cell),y=Mathf.RoundToInt((cellOrigin.y-point.y)/Cell);return Inside(x,y)?grid[x,y]:null;}
 bool Inside(int x,int y){return x>=0&&x<9&&y>=0&&y<12;}
 Vector2 CellPosition(int x,int y){return cellOrigin+new Vector2(x*Cell,-y*Cell);}
 void Reindex(){grid=new Piece[9,12];foreach(var p in pieces)Put(p);}
 void Erase(Piece p){for(int x=0;x<9;x++)for(int y=0;y<12;y++)if(grid[x,y]==p)grid[x,y]=null;}
 void Put(Piece p){for(int x=p.x;x<p.x+p.w;x++)for(int y=p.y;y<p.y+p.h;y++)if(Inside(x,y))grid[x,y]=p;}
 bool CanPlace(Piece p,int x,int y){
  int w=p==null?1:p.w,h=p==null?1:p.h;
  for(int xx=x;xx<x+w;xx++)for(int yy=y;yy<y+h;yy++)if(!Inside(xx,yy)||!playable[xx,yy]||(grid[xx,yy]!=null&&grid[xx,yy]!=p))return false;return true;
 }
 public void ShowEnd(bool won=true){
  if(IsFinished)return;IsFinished=true;selected=null;RemoveGuide();
  stage.gameObject.SetActive(false);foreach(var child in chrome.GetComponentsInChildren<Graphic>())child.gameObject.SetActive(false);
  var frame=new GameObject("EndFrame",typeof(RectTransform)).GetComponent<RectTransform>();frame.SetParent(chrome,false);frame.sizeDelta=new Vector2(1080,1920);var top=frame.gameObject.AddComponent<Canvas>();top.overrideSorting=true;top.sortingOrder=1000;frame.gameObject.AddComponent<GraphicRaycaster>();responsive.endFrame=frame;responsive.Refresh();
  endPanel=CompanyEndArtwork(frame,won);

 }
 public void Download(){Sound("btn_click");templateDownload.DownloadGame();}
 void OnDestroy(){if(stage)foreach(var t in stage.GetComponentsInChildren<RectTransform>())t.DOKill();}
}
}


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
public class Reference20Game : MonoBehaviour {
 public RectTransform stage;
 public Match3.GameManager templateDownload;
 public int maxMoves=9;
 public int MovesPlayed {get;private set;}
 public bool IsBusy {get;private set;}
 public bool IsFinished {get;private set;}
 public int ClearedCount {get;private set;}
 class Piece {
  public int x,y,w=1,h=1,id,hp=1;public RectTransform root;public Image image;public Vector2 initial;public int originalHeight;public float settledAt;
  public bool normal {get{return id<100;}}public bool basic {get{return id==3||id==4;}}public bool movable {get{return normal||id==100015||id==100032;}}
  public Spine.Unity.SkeletonDataAsset chickData;public string chickSkin;
 }
 class MatchGroup {public HashSet<Piece> members=new HashSet<Piece>();public int horizontal,vertical;public bool square;}
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
 AudioSource audioSource;
 AudioClip pop,wrong;
 Vector2 cellOrigin=new Vector2(85.6f,1584.8f);
 const float Cell=113.6f;
 Font font;
 void Start(){Application.runInBackground=true;Application.targetFrameRate=60;Build();}
 public void Build(){
  if(Application.isPlaying)StopAllCoroutines();Reference20Native.ResetCaches();pendingFlights.Clear();pendingCollections=0;randomSeed=6;Array.Clear(lastDropColor,0,lastDropColor.Length);
  for(int i=stage.childCount-1;i>=0;i--){var child=stage.GetChild(i);foreach(var rt in child.GetComponentsInChildren<RectTransform>(true))rt.DOKill();child.gameObject.SetActive(false);if(Application.isPlaying)Destroy(child.gameObject);else DestroyImmediate(child.gameObject);}
  pieces.Clear();normalSprites.Clear();playable=new bool[9,12];MovesPlayed=0;IsFinished=false;IsBusy=false;ClearedCount=0;
  font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var layout=JsonUtility.FromJson<Layout>(Resources.Load<TextAsset>("Reference20/layout").text);
  audioSource=GetComponent<AudioSource>();if(!audioSource)audioSource=gameObject.AddComponent<AudioSource>();audioSource.playOnAwake=false;
  pop=Resources.Load<AudioClip>("Reference20/match");wrong=Resources.Load<AudioClip>("Reference20/wrong");EnsureAudio();
  // Preserve the reference's screen-space layout while keeping every game piece interactive.
  foreach(var l in layout.layers) if(l.asset!="decoration"&&l.asset!="background") AddPicture(l,stage);
  foreach(var r in layout.pieces){
   string[] a=r.name.Split('_');int col=int.Parse(a[1])-4,row=int.Parse(a[0])-3;
   var p=new Piece {id=r.id,x=col,y=row};
   if(p.id==100016||p.id==100042){p.w=2;p.h=2;p.hp=3;}
   if(p.id==100032){p.h=2;p.hp=2;}
   if(p.id==100059){p.h=10-col*2;p.y=12-p.h;p.originalHeight=p.h;}
   p.root=Reference20Native.Piece(r.name,stage);
   if(p.root==null)p.root=AddPicture(r,stage);
   p.image=p.root.GetComponent<Image>();if(!p.image){p.image=p.root.gameObject.AddComponent<Image>();p.image.enabled=false;}
   p.initial=p.root.anchoredPosition;p.image.raycastTarget=false;
   var actor=p.root.GetComponent<Reference20Native>();if(actor)actor.Configure(p.id,p.hp);
   if(p.id==100016){var egg=p.root.GetComponentInChildren<Spine.Unity.SkeletonGraphic>(true);if(egg){p.chickData=egg.skeletonDataAsset;p.chickSkin=egg.Skeleton.Skin==null?"default":egg.Skeleton.Skin.Name;}}
   pieces.Add(p);
   if(p.normal&&!normalSprites.ContainsKey(p.id))normalSprites[p.id]=Resources.Load<Sprite>("Reference20/"+r.asset);
   for(int x=p.x;x<p.x+p.w;x++)for(int y=p.y;y<p.y+p.h;y++)if(Inside(x,y))playable[x,y]=true;
  }
  Reindex();for(int x=0;x<9;x++){entryRows[x]=-1;for(int y=0;y<12;y++)if(playable[x,y]){entryRows[x]=y;break;}}
  BuildChrome();BuildGuide();
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
 }
 void RemoveGuide(){if(guide){guide.DOKill();foreach(var t in guide.GetComponentsInChildren<RectTransform>())t.DOKill();Destroy(guide.gameObject);guide=null;}}
 void EnsureAudio(){
  audioSource.spatialBlend=0;audioSource.volume=1;audioSource.mute=false;audioSource.ignoreListenerPause=false;
  if(!FindObjectOfType<AudioListener>()){var camera=Camera.main;if(camera)camera.gameObject.AddComponent<AudioListener>();else gameObject.AddComponent<AudioListener>();}
 }
 void Sound(string name,float volume=.8f){
  if(!Application.isPlaying||!audioSource)return;AudioClip clip;
  if(!sounds.TryGetValue(name,out clip)){clip=Resources.Load<AudioClip>("Reference20/Native/Audio/"+name);sounds[name]=clip;}
  if(clip)audioSource.PlayOneShot(clip,volume);
 }
 void Update(){
  if(IsBusy||IsFinished){selected=null;return;}
  if(Input.GetMouseButtonDown(0)){
   Vector2 point;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,Input.mousePosition,null,out point);press=point+new Vector2(540,960);
   var piece=AtPoint(press);
   if(selected!=null&&piece!=null&&piece!=selected&&TryMove(selected.x,selected.y,piece.x,piece.y)){selected=null;return;}
   selected=piece;
  }
  if(Input.GetMouseButtonUp(0)&&selected!=null){
   Vector2 point;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,Input.mousePosition,null,out point);var delta=point+new Vector2(540,960)-press;
   if(delta.magnitude>Cell*.22f){int dx=0,dy=0;if(Mathf.Abs(delta.x)>Mathf.Abs(delta.y))dx=delta.x>0?1:-1;else dy=delta.y>0?-1:1;TryMove(selected.x,selected.y,selected.x+dx,selected.y+dy);selected=null;}
   else if(selected.normal&&!selected.basic){StartCoroutine(PowerTurn(selected));selected=null;}
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
 void Move(Piece p,int x,int y,float duration){
  var target=CellPosition(x,y);if(p.id==100032)target+=new Vector2(0,-Cell*.5f);Erase(p);p.x=x;p.y=y;Put(p);p.settledAt=Time.time+duration;p.root.DOKill();p.root.DOAnchorPos(target,duration).SetEase(Ease.InQuad);
 }
 void Swap(Piece a,Piece b){
  int ax=a.x,ay=a.y,bx=b.x,by=b.y;Erase(a);Erase(b);a.x=bx;a.y=by;b.x=ax;b.y=ay;Put(a);Put(b);
  a.root.DOKill();b.root.DOKill();a.root.DOAnchorPos(CellPosition(a.x,a.y),.16f).SetEase(Ease.InOutSine);b.root.DOAnchorPos(CellPosition(b.x,b.y),.16f).SetEase(Ease.InOutSine);
 }
 public bool TryMove(int x,int y,int tx,int ty){
  if(IsBusy||IsFinished||!Inside(x,y)||!Inside(tx,ty)||Mathf.Abs(x-tx)+Mathf.Abs(y-ty)!=1)return false;
  var a=grid[x,y];var b=grid[tx,ty];if(a==null||b==null||!a.normal||!b.normal)return false;
  if(guide&&!((x==7&&y==4&&tx==8&&ty==4)||(x==8&&y==4&&tx==7&&ty==4)))return false;
  IsBusy=true;StartCoroutine(SwapTurn(a,b));return true;
 }
 IEnumerator SwapTurn(Piece a,Piece b){
  try{
   Sound("btn_click",.35f);Swap(a,b);yield return new WaitForSeconds(.17f);
   var groups=FindGroups();bool powers=!a.basic||!b.basic;
   bool matches=groups.Exists(g=>g.members.Contains(a)||g.members.Contains(b));
   if(!powers&&!matches){Swap(a,b);Sound("Audio_Match_WrongMatch");yield return new WaitForSeconds(.17f);yield break;}
   RemoveGuide();MovesPlayed++;
   var triggered=new HashSet<Piece>();if(!a.basic)triggered.Add(a);if(!b.basic)triggered.Add(b);
   int color=a.basic?a.id:b.basic?b.id:0;
   yield return Resolve(groups,triggered,b,color,powers?(a.basic?a:b.basic?b:null):null);
   yield return CompleteTurn();
  }finally{IsBusy=false;}
 }
 IEnumerator PowerTurn(Piece p){
  if(IsBusy||IsFinished||guide)yield break;IsBusy=true;
  try{MovesPlayed++;yield return Resolve(FindGroups(),new HashSet<Piece>{p},null,0);yield return CompleteTurn();}finally{IsBusy=false;}
 }
 int MatchId(int x,int y){var p=grid[x,y];return p!=null&&p.basic&&Time.time+.001f>=p.settledAt?p.id:0;}
 List<MatchGroup> FindGroups(){
  var runs=new List<MatchGroup>();
  for(int y=0;y<12;y++)for(int x=0;x<9;x++){
   var p=grid[x,y];if(MatchId(x,y)==0)continue;
   if(x==0||MatchId(x-1,y)!=p.id){int end=x+1;while(end<9&&MatchId(end,y)==p.id)end++;if(end-x>=3){var g=new MatchGroup{horizontal=end-x};for(int i=x;i<end;i++)g.members.Add(grid[i,y]);runs.Add(g);}}
   if(y==0||MatchId(x,y-1)!=p.id){int end=y+1;while(end<12&&MatchId(x,end)==p.id)end++;if(end-y>=3){var g=new MatchGroup{vertical=end-y};for(int i=y;i<end;i++)g.members.Add(grid[x,i]);runs.Add(g);}}
   if(x<8&&y<11&&MatchId(x+1,y)==p.id&&MatchId(x,y+1)==p.id&&MatchId(x+1,y+1)==p.id){var g=new MatchGroup{square=true};g.members.UnionWith(new[]{p,grid[x+1,y],grid[x,y+1],grid[x+1,y+1]});runs.Add(g);}
  }
  for(int i=0;i<runs.Count;i++)for(int j=i+1;j<runs.Count;j++)if(runs[i].members.Overlaps(runs[j].members)){
   var a=runs[i];var b=runs[j];a.members.UnionWith(b.members);a.horizontal=Mathf.Max(a.horizontal,b.horizontal);a.vertical=Mathf.Max(a.vertical,b.vertical);a.square|=b.square;runs.RemoveAt(j);j=i;
  }
  return runs;
 }
 IEnumerator Resolve(List<MatchGroup> groups,HashSet<Piece> triggered,Piece preferred,int selectedColor,Piece exchanged=null){
  int cascade=0;
  while(groups.Count>0||triggered.Count>0){
   cascade++;Sound("Audio_Match_Base"+Mathf.Min(cascade,5));
   var clear=new HashSet<Piece>(triggered);var damage=new HashSet<Piece>();var damageHits=new Dictionary<Piece,int>();var upgrades=new Dictionary<Piece,int>();
   if(exchanged!=null){clear.Add(exchanged);exchanged=null;}
   foreach(var group in groups){
    Piece keep=null;int skill=group.horizontal>=5||group.vertical>=5?15:group.horizontal>=3&&group.vertical>=3?13:group.horizontal>=4?12:group.vertical>=4?11:group.square?14:0;
    if(skill!=0){keep=preferred!=null&&group.members.Contains(preferred)?preferred:null;if(keep==null)foreach(var p in group.members){keep=p;break;}upgrades[keep]=skill;}
    var groupDamage=new HashSet<Piece>();foreach(var p in group.members){if(p!=keep)clear.Add(p);foreach(var d in Neighbors){int x=p.x+d.x,y=p.y+d.y;if(Inside(x,y)&&grid[x,y]!=null&&!grid[x,y].normal)groupDamage.Add(grid[x,y]);}}
    foreach(var p in groupDamage){damage.Add(p);if(!damageHits.ContainsKey(p))damageHits[p]=0;damageHits[p]++;}
   }
   var queue=new Queue<Piece>(triggered);var seen=new HashSet<Piece>();
   if(triggered.Count==2){
    var pair=new List<Piece>(triggered);var a=pair[0];var b=pair[1];var center=preferred??b;
    if(a.id==15||b.id==15){
     var other=a.id==15?b:a;int color=selectedColor;
     if(color==0){int blue=0,yellow=0;foreach(var p in pieces){if(p.id==3)blue++;if(p.id==4)yellow++;}color=blue>=yellow?3:4;}
     queue.Clear();seen.Add(a);seen.Add(b);
     foreach(var p in pieces.ToArray()){
      if(a.id==15&&b.id==15){if(p.normal){clear.Add(p);if(!p.basic&&p!=a&&p!=b)queue.Enqueue(p);}else damage.Add(p);}
      else if(p.basic&&p.id==color){ReplaceBasic(p,other.id==11||other.id==12?RandomRange(0,1)==0?11:12:other.id);clear.Add(p);queue.Enqueue(p);}
     }
     Reference20Native.Effect("SameColorEffect",stage,center.root.anchoredPosition);Sound("Audio_Match_SameColorCrusherExplode");
    }else if(a.id==14||b.id==14){
     queue.Clear();seen.Add(a);seen.Add(b);int other=a.id==14?b.id:a.id;int count=other==14?3:1;
     for(int i=0;i<count;i++){
      Piece target=null;foreach(var p in pieces)if(!p.normal&&!damage.Contains(p)&&(target==null||p.x>target.x))target=p;
      if(target==null)break;damage.Add(target);int cx=target.x,cy=target.y;
      foreach(var p in pieces){bool hit=false;for(int x=p.x;x<p.x+p.w;x++)for(int y=p.y;y<p.y+p.h;y++)hit|=other==11?y==cy:other==12?x==cx:other==13?Mathf.Abs(x-cx)<=2&&Mathf.Abs(y-cy)<=2:x==cx&&y==cy;
       if(!hit)continue;if(p.normal){clear.Add(p);if(!p.basic&&p!=a&&p!=b)queue.Enqueue(p);}else damage.Add(p);
      }
      Reference20Native.Effect(other==13?"AreaEffect":other==14?"SquareEffect":"LineMoveEffect",stage,CellPosition(cx,cy));
     }
     Sound("Audio_Match_Rocket_Flying");
    }else{
     queue.Clear();seen.Add(a);seen.Add(b);bool bombs=a.id==13&&b.id==13;bool mixed=a.id==13||b.id==13;
     foreach(var p in pieces){bool hit=false;for(int x=p.x;x<p.x+p.w;x++)for(int y=p.y;y<p.y+p.h;y++){
      int dx=Mathf.Abs(x-center.x),dy=Mathf.Abs(y-center.y);hit|=bombs?dx<=3&&dy<=3&&!(dx==3&&dy==3):mixed?dx<=1||dy<=1:dx==0||dy==0;
     }if(!hit)continue;if(p.normal){clear.Add(p);if(!p.basic&&p!=a&&p!=b)queue.Enqueue(p);}else damage.Add(p);}
     Reference20Native.Effect(bombs?"AreaAreaEffect":mixed?"SquareBigEffect":"LineMoveEffect",stage,center.root.anchoredPosition);Sound(bombs?"Audio_Match_BigBomb_Shake":mixed?"Audio_Match_LineAndAreaCrusher":"Audio_Match_LineCrusher");
    }
   }
   while(queue.Count>0){var p=queue.Dequeue();if(!seen.Add(p)||!pieces.Contains(p))continue;string effect=p.id==15?"SameColorEffect":p.id==13?"SquareBigEffect":p.id==14?"SquareFlyEffect":"LineMoveEffect";Reference20Native.Effect(effect,stage,p.root.anchoredPosition);
    Sound(p.id==15?"Audio_Match_SameColorCrusherExplode":p.id==13?"Audio_Match_AreaCrusher":p.id==14?"Audio_Match_Rocket_Flying":"Audio_Match_LineCrusher");
    if(p.id==15&&selectedColor==0)selectedColor=RandomRange(0,1)==0?3:4;
    if(p.id==14){Piece target=null;foreach(var candidate in pieces)if(!candidate.normal&&(target==null||candidate.x>target.x))target=candidate;if(target!=null)damage.Add(target);}
    for(int x=0;x<9;x++)for(int y=0;y<12;y++){
     bool hit=p.id==11?y==p.y:p.id==12?x==p.x:p.id==13?Mathf.Abs(x-p.x)<=2&&Mathf.Abs(y-p.y)<=2:p.id==15?grid[x,y]!=null&&grid[x,y].id==selectedColor:Mathf.Abs(x-p.x)+Mathf.Abs(y-p.y)<=1;
     if(!hit)continue;var q=grid[x,y];if(q==null)continue;if(q.normal){if(clear.Add(q)&&!q.basic)queue.Enqueue(q);}else damage.Add(q);
    }
   }
   foreach(var p in clear)if(pieces.Contains(p)){upgrades.Remove(p);Reference20Native.Effect("BaseBlockDestroy",stage,p.root.anchoredPosition);RemoveNormal(p);}
   foreach(var upgrade in upgrades)if(pieces.Contains(upgrade.Key)){ReplaceBasic(upgrade.Key,upgrade.Value);Reference20Native.Effect("CommonBornPropsEffect",stage,upgrade.Key.root.anchoredPosition);Sound(upgrade.Value==15?"Audio_Match_SameColorCrusherAppear":upgrade.Value==13?"Audio_Match_BombAppear":"Audio_Match_LineCrushAppear");}
   float wait=.2f;var collected=new List<Piece>();pendingFlights.Clear();
   foreach(var p in damage)if(pieces.Contains(p)){
    int before=p.hp;var actor=p.root.GetComponent<Reference20Native>();
    if(p.id==100059){Erase(p);p.y++;p.h--;if(actor)actor.Eat(p.h);if(p.h<=1){pieces.Remove(p);collected.Add(p);ClearedCount++;}else{Put(p);}Sound("Audio_Match_RedChicken0");wait=Mathf.Max(wait,.95f);continue;}
    int hits;damageHits.TryGetValue(p,out hits);
    p.hp=Mathf.Max(0,p.hp-Mathf.Max(1,hits));if(actor)wait=Mathf.Max(wait,actor.AnimateDamage(before,p.hp));
    if(p.id==100016){Sound("Audio_Match_HenEgg0");if(before<=2)pendingFlights.Add(p);}
    else if(p.id==100042){Sound(p.hp==0?"shrubPeacock_end":"shrubPeacock_break_1");Reference20Native.Effect("ShrubPeacockEffect",stage,p.root.anchoredPosition);}
    else if(p.id==100032)Sound(before==2?"screamChicken_break2_0":"screamChicken_break1");
    else Sound("Audio_Match_HenEgg0");
    if(p.hp<=0){Erase(p);pieces.Remove(p);collected.Add(p);ClearedCount++;}
   }
   // Obstacle animation and gravity advance together; the board does not wait
   // for an unrelated hatch/collection animation before starting to fall.
   yield return new WaitForSeconds(.16f);
   bool fallen=false;StartCoroutine(FallAndNotify(()=>fallen=true));
   float readyAt=Time.time+Mathf.Max(0,wait-.16f);
   while(!fallen||Time.time<readyAt)yield return null;
   int flights=pendingFlights.Count;foreach(var nest in pendingFlights)StartCoroutine(FlyAndNotify(nest,()=>flights--));
   while(flights>0)yield return null;
   foreach(var p in collected)Release(p);
   pendingFlights.Clear();
   yield return FallAndRefill();
   groups=FindGroups();triggered=new HashSet<Piece>();preferred=null;selectedColor=0;
  }
 }
 static readonly Vector2Int[] Neighbors={Vector2Int.left,Vector2Int.right,Vector2Int.up,Vector2Int.down};
 IEnumerator FallAndNotify(Action complete){yield return FallAndRefill();complete();}
 IEnumerator FlyAndNotify(Piece nest,Action complete){yield return FlyChicks(nest);complete();}
 void RemoveNormal(Piece p){Erase(p);pieces.Remove(p);ClearedCount++;p.root.DOKill();p.root.DOScale(0,.16f).OnComplete(()=>{if(p.root)Destroy(p.root.gameObject);});}
 void Release(Piece p){
  if(!p.root)return;p.root.DOKill();var actor=p.root.GetComponent<Reference20Native>();
  if(p.id==100016){Reference20Native.Effect("HenHouseDestroy",stage,p.root.anchoredPosition);p.root.DOScale(0,.15f).OnComplete(()=>{if(p.root)Destroy(p.root.gameObject);});return;}
  if(p.id==100059&&actor){var head=actor.DetachCornHead(stage);var body=p.root;body.gameObject.SetActive(false);Destroy(body.gameObject);if(!head)return;p.root=head;actor=null;}
  if(actor)actor.PlayCollected();
  var sorting=p.root.GetComponent<Canvas>();if(!sorting)sorting=p.root.gameObject.AddComponent<Canvas>();sorting.overrideSorting=true;sorting.sortingOrder=100;
  pendingCollections++;var current=p.root.anchoredPosition;var tween=DOTween.Sequence();tween.Append(p.root.DOAnchorPos(current+new Vector2((540-current.x)*.2f,-200),.7f).SetEase(Ease.InQuad));tween.AppendInterval(.15f);tween.Append(p.root.DOAnchorPos(new Vector2(540,2200),.8f).SetEase(Ease.InQuad));tween.OnComplete(()=>{pendingCollections=Mathf.Max(0,pendingCollections-1);if(p.root)Destroy(p.root.gameObject);});
  Sound(p.id==100032?"screamChicken_end":"flyout_duang");
 }
 IEnumerator FlyChicks(Piece nest){
  if(!nest.root)yield break;
  var reserved=new HashSet<Piece>();
  for(int i=0;i<2;i++){
   var targets=pieces.FindAll(p=>p.basic&&!reserved.Contains(p));if(targets.Count==0)yield break;var target=targets[RandomRange(0,targets.Count-1)];reserved.Add(target);
   var actor=nest.root.GetComponent<Reference20Native>();var chick=actor?actor.DetachNestChick(stage):null;if(!chick)yield break;
   var start=chick.anchoredPosition;var dest=CellPosition(target.x,target.y);Sound("henHouse_duang");
   var sequence=DOTween.Sequence();sequence.Append(chick.DOAnchorPos((start+dest)*.5f+new Vector2(0,160),.25f).SetEase(Ease.OutQuad));sequence.Append(chick.DOAnchorPos(dest,.25f).SetEase(Ease.InQuad));yield return sequence.WaitForCompletion();
   if(chick)Destroy(chick.gameObject);
   if(pieces.Contains(target)&&target.basic)ReplaceWithChick(target,nest.chickData,nest.chickSkin);
  }
 }
 void ReplaceWithChick(Piece p,Spine.Unity.SkeletonDataAsset data,string skin){
  var old=p.root;old.DOKill();old.gameObject.SetActive(false);Destroy(old.gameObject);
  p.id=100015;p.hp=1;p.w=p.h=1;p.chickData=data;p.chickSkin=skin;
  var root=Reference20Native.Visual("HenEggBlock",stage);p.root=root;root.anchorMin=root.anchorMax=Vector2.zero;root.anchoredPosition=CellPosition(p.x,p.y);root.localScale=Vector3.one*.8875f;
  var native=root.gameObject.AddComponent<Reference20Native>();native.UseChickData(data,skin);native.Configure(p.id,p.hp);p.image=root.GetComponent<Image>();
 }
 float FallDuration(int cells){return (Mathf.Sqrt(700f*700f+2*2800f*128f*cells)-700f)/2800f;}
 IEnumerator FallAndRefill(){
  while(true){
   bool changed=false,moving=false;
   var movers=pieces.FindAll(p=>p.movable);movers.Sort((a,b)=>b.y!=a.y?b.y.CompareTo(a.y):b.x.CompareTo(a.x));
   foreach(var p in movers){
    if(Time.time+.001f<p.settledAt){moving=true;continue;}
    int bottom=p.y;while(CanPlace(p,p.x,bottom+1))bottom++;
    if(bottom>p.y){Move(p,p.x,bottom,FallDuration(bottom-p.y));changed=true;continue;}
    if(p.w!=1||p.h!=1)continue;
    foreach(int dx in new[]{-1,1}){
     int x=p.x+dx,y=p.y+1;if(!CanPlace(p,x,y)||!Inside(x,p.y))continue;
     var above=grid[x,p.y];if(playable[x,p.y]&&(above==null||above.movable))continue;
     Move(p,x,y,FallDuration(1));changed=true;break;
    }
   }
   for(int x=8;x>=0;x--){
    int top=entryRows[x];if(top<0||grid[x,top]!=null)continue;
    int bottom=top;while(CanPlace(null,x,bottom+1))bottom++;
    var p=CreateNormal(x,bottom,ChooseRefill(x,bottom));float duration=FallDuration(bottom-top+1);
    p.root.anchoredPosition=CellPosition(x,top)+new Vector2(0,Cell);p.settledAt=Time.time+duration;
    p.root.DOAnchorPos(CellPosition(x,bottom),duration).SetEase(Ease.InQuad);changed=true;
   }
   if(FindGroups().Count>0)yield break;
   if(!changed&&!moving)yield break;
   yield return new WaitForSeconds(.05f);
  }
 }
 int ChooseRefill(int x,int y){
  int last=lastDropColor[x];int index=RandomRange(0,last==0?1:0);
  int id=last==3?4:last==4?3:index==0?3:4;
  lastDropColor[x]=RandomRange(1,10)<=5?id:0;return id;
 }
 Piece CreateNormal(int x,int y,int id){
  var p=new Piece{x=x,y=y,id=id};var sample=normalSprites[id];p.root=Box("Normal_"+x+"_"+y,stage,CellPosition(x,y),sample.rect.size,Color.white);p.image=p.root.GetComponent<Image>();p.image.sprite=sample;p.image.raycastTarget=false;pieces.Add(p);Put(p);return p;
 }
 void ReplaceBasic(Piece p,int id){
  p.root.DOKill();for(int i=p.root.childCount-1;i>=0;i--){var child=p.root.GetChild(i);child.gameObject.SetActive(false);Destroy(child.gameObject);}
  p.id=id;if(!p.image)p.image=p.root.gameObject.AddComponent<Image>();p.image.raycastTarget=false;
  if(p.basic){p.image.enabled=true;p.image.color=Color.white;p.image.sprite=normalSprites[id];p.root.sizeDelta=normalSprites[id].rect.size;p.root.localScale=Vector3.one;}
  else{p.image.enabled=false;var visual=Reference20Native.Visual(id==15?"SameColorBlock":id==13?"AreaBlock":id==14?"SquareBlock":"LineBlock",p.root);if(visual){visual.localScale*=.8875f/Mathf.Abs(p.root.localScale.x);if(id==12)visual.localRotation=Quaternion.Euler(0,0,90);}}
 }
 public bool TryGetValidMove(out Vector2Int from,out Vector2Int to){
  from=to=Vector2Int.zero;
  foreach(var p in pieces)if(p.normal)foreach(var d in new[]{Vector2Int.right,Vector2Int.down}){int x=p.x+d.x,y=p.y+d.y;if(!Inside(x,y))continue;var q=grid[x,y];if(q==null||!q.normal)continue;
   if(!p.basic||!q.basic){from=new Vector2Int(p.x,p.y);to=new Vector2Int(x,y);return true;}
   int id=p.id;p.id=q.id;q.id=id;bool valid=FindGroups().Exists(g=>g.members.Contains(p)||g.members.Contains(q));q.id=p.id;p.id=id;
   if(valid){from=new Vector2Int(p.x,p.y);to=new Vector2Int(x,y);return true;}
  }return false;
 }
 IEnumerator CompleteTurn(){if(MovesPlayed>=maxMoves)while(pendingCollections>0)yield return null;FinishTurn();}
 void FinishTurn(){if(MovesPlayed>=maxMoves){ShowEnd();return;}Vector2Int from,to;if(!TryGetValidMove(out from,out to))Shuffle();}
 void Shuffle(){
  int parity=RandomRange(0,1);foreach(var p in pieces)if(p.basic)ReplaceBasic(p,3+((p.x+p.y+parity)&1));
  Vector2Int from,to;if(!TryGetValidMove(out from,out to)){var p=pieces.Find(x=>x.basic);if(p!=null)ReplaceBasic(p,14);}
 }
 public void ShowEnd(){
  if(IsFinished)return;IsFinished=true;RemoveGuide();
  var frame=new GameObject("EndFrame",typeof(RectTransform)).GetComponent<RectTransform>();frame.SetParent(chrome,false);frame.sizeDelta=new Vector2(1080,1920);var top=frame.gameObject.AddComponent<Canvas>();top.overrideSorting=true;top.sortingOrder=1000;frame.gameObject.AddComponent<GraphicRaycaster>();responsive.endFrame=frame;responsive.Refresh();
  Sound("Audio_UI_Win");
  endPanel=Reference20Native.EndArtwork(frame);
  if(endPanel){var target=endPanel.Find("Root/DownloadBtn") as RectTransform;if(target){var im=target.GetComponent<Image>();if(!im)im=target.gameObject.AddComponent<Image>();im.raycastTarget=true;var button=target.gameObject.AddComponent<Button>();button.onClick.AddListener(Download);foreach(var clip in target.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(0);}return;}
  endPanel=Box("EndCard",frame,new Vector2(540,960),new Vector2(1080,1920),Color.white);
  endPanel.GetComponent<Image>().sprite=Resources.Load<Sprite>("Reference20/endcard");
  var btn=Box("Play",endPanel,new Vector2(540,375),new Vector2(550,220),new Color(1,1,1,0));btn.gameObject.AddComponent<Button>().onClick.AddListener(Download);

 }
 public void Download(){Sound("btn_click");templateDownload.DownloadGame();}
 void OnDestroy(){if(stage)foreach(var t in stage.GetComponentsInChildren<RectTransform>())t.DOKill();}
}
}


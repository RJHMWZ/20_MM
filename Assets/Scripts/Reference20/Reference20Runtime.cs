using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
public partial class Reference20Game {
 sealed class BoardEvent {public float at;public long order;public Action action;}
 readonly List<BoardEvent> boardEvents=new List<BoardEvent>();
 readonly List<Piece> landed=new List<Piece>();
 readonly List<Piece> motionOrder=new List<Piece>();
 readonly List<Piece> renderOrder=new List<Piece>();
 readonly HashSet<Vector2Int> beeTargets=new HashSet<Vector2Int>();
 readonly Stack<RectTransform> normalPool=new Stack<RectTransform>();
 readonly Stack<RectTransform> composePool=new Stack<RectTransform>();
 readonly float[] entryReady=new float[9];
 readonly float[,] pullReady=new float[9,12];
 readonly int[,] cellLocks=new int[9,12];
 float clock,accumulator;long eventOrder;bool resolving,boardMoving;int comboCount,fallingPauseCount,matchingPauseCount;
 const float Tick=1f/59f;
 public Action<string,int,int,int,float> TraceEvent;
 void Trace(string kind,Piece p){if(TraceEvent!=null)TraceEvent(kind,p.x,p.y,p.id,clock);}
 static readonly int[] Signs={-1,1},SquareYSigns={1,-1};
 static readonly Vector2Int[] FloodDirections={Vector2Int.left,Vector2Int.right,Vector2Int.down,Vector2Int.up};
 static readonly Vector2Int[] MoveDirections={Vector2Int.right,Vector2Int.down};
 static readonly Vector2Int[] Neighbors={Vector2Int.left,Vector2Int.right,Vector2Int.up,Vector2Int.down};

 void ResetRuntime(){magicFocus=null;magicFocusLeases=0;boardEvents.Clear();landed.Clear();motionOrder.Clear();renderOrder.Clear();beeTargets.Clear();emptyCells.Clear();cachedCells.Clear();fallWait.Clear();normalPool.Clear();composePool.Clear();Array.Clear(entryReady,0,9);for(int x=0;x<9;x++)for(int y=0;y<12;y++){pullReady[x,y]=0;cellLocks[x,y]=0;}clock=accumulator=0;eventOrder=0;comboCount=fallingPauseCount=matchingPauseCount=0;resolving=boardMoving=false;activeOperations=0;settleRoutine=null;}
 void RecycleNormal(RectTransform rt){if(!rt)return;rt.DOKill();for(int i=rt.childCount-1;i>=0;i--){var child=rt.GetChild(i);if(Reference20Native.ReleaseVisual(child,stage))continue;foreach(var rect in child.GetComponentsInChildren<RectTransform>(true))rect.DOKill();child.gameObject.SetActive(false);Destroy(child.gameObject);}rt.gameObject.SetActive(false);if(rt.name.StartsWith("Normal_"))normalPool.Push(rt);else Destroy(rt.gameObject);}
 void WarmNormalPool(){if(!Application.isPlaying)return;for(int i=0;i<128;i++){var rt=Box("Normal_Pooled",stage,Vector2.zero,Vector2.one*Cell,Color.white);rt.GetComponent<Image>().raycastTarget=false;rt.gameObject.SetActive(false);normalPool.Push(rt);}}
 RectTransform ComposeTile(Piece p){RectTransform rt=null;while(composePool.Count>0&&!rt)rt=composePool.Pop();var sprite=normalSprites[p.id];if(!rt)rt=Box("ComposeTile",Reference20Native.Overlay(stage),p.root.anchoredPosition,sprite.rect.size,Color.white);rt.DOKill();rt.anchoredPosition=p.root.anchoredPosition;rt.sizeDelta=sprite.rect.size*.8875f;rt.localScale=Vector3.one;var image=rt.GetComponent<Image>();image.sprite=sprite;image.raycastTarget=false;image.color=Color.white;rt.gameObject.SetActive(true);rt.SetAsLastSibling();return rt;}
 void ReturnCompose(RectTransform rt){if(!rt)return;rt.gameObject.SetActive(false);composePool.Push(rt);}
 static bool EventBefore(BoardEvent a,BoardEvent b){return a.at<b.at||a.at==b.at&&a.order<b.order;}
 void After(float seconds,Action action){
  var next=new BoardEvent{at=clock+seconds,order=eventOrder++,action=action};int index=boardEvents.Count;boardEvents.Add(next);
  while(index>0){int parent=(index-1)/2;if(!EventBefore(next,boardEvents[parent]))break;boardEvents[index]=boardEvents[parent];index=parent;}boardEvents[index]=next;
 }
 BoardEvent PopEvent(){
  var first=boardEvents[0];int last=boardEvents.Count-1;var tail=boardEvents[last];boardEvents.RemoveAt(last);if(last==0)return first;
  int index=0;while(true){int child=index*2+1;if(child>=last)break;if(child+1<last&&EventBefore(boardEvents[child+1],boardEvents[child]))child++;if(!EventBefore(boardEvents[child],tail))break;boardEvents[index]=boardEvents[child];index=child;}boardEvents[index]=tail;return first;
 }
 void AdvanceRuntime(float dt){
  accumulator+=Mathf.Max(0,dt);
  while(accumulator>=Tick){accumulator-=Tick;clock+=Tick;
   int guard=0;while(boardEvents.Count>0&&guard++<4096){if(boardEvents[0].at>clock+.00001f)break;var e=PopEvent();e.action();}
   if(resolving){TickGravity(Tick);bool due=false;foreach(var p in landed)if(!p.falling&&!p.dying&&!p.reserved&&clock+.00001f>=p.settledAt){due=true;break;}if(due&&matchingPauseCount==0){var matches=FindGroups();if(matches.Count>0)ApplyMatches(matches);landed.RemoveAll(p=>!pieces.Contains(p)||p.dying||!p.falling&&!p.reserved&&clock+.00001f>=p.settledAt);}Phase=activeOperations>0?TurnPhase.Swapping:boardEvents.Count>0?TurnPhase.Resolving:TurnPhase.Settling;}
  }
  for(int i=renderOrder.Count-1;i>=0;i--){var p=renderOrder[i];if(!pieces.Contains(p)||p.dying||!p.root){renderOrder.RemoveAt(i);continue;}if(!p.reserved)p.root.anchoredPosition=Vector2.LerpUnclamped(p.previousPosition,p.simPosition,accumulator/Tick);if(!p.falling&&!p.paused&&(p.previousPosition-p.simPosition).sqrMagnitude<.001f)renderOrder.RemoveAt(i);}
 }
 Vector2 Position(Piece p){return CellPosition(p.x,p.y)+p.visualOffset;}
 bool Free(Piece p,int x,int y){if(!CanPlace(p,x,y))return false;int w=p==null?1:p.w,h=p==null?1:p.h;for(int xx=x;xx<x+w;xx++)for(int yy=y;yy<y+h;yy++)if(cellLocks[xx,yy]>0)return false;return true;}
 bool Matchable(Piece p){return p!=null&&p.basic&&!p.dying&&!p.reserved&&!p.falling&&!p.switching&&clock+.00001f>=p.settledAt;}
 Piece Same(int x,int y,int id){if(!Inside(x,y))return null;var p=grid[x,y];return p!=null&&p.basic&&!p.dying&&!p.reserved&&!p.falling&&!p.switching&&p.id==id?p:null;}
 bool MatchingFallPending(Piece p){for(int y=p.y-1;y>=entryRows[p.x]&&y>=0;y--){var above=grid[p.x,y];if(above==null||above.dying||above.reserved||above.switching||above.id!=p.id)break;if(above.falling)return true;}foreach(int dx in Signs){int x=p.x+dx;if(!Inside(x,p.y))continue;var side=grid[x,p.y];if(side!=null&&side.id==p.id&&!side.dying&&!side.reserved&&side.falling&&!Free(side,x,p.y+1))return true;}return false;}
 List<Piece> Run(Piece p,bool vertical){var result=new List<Piece>{p};foreach(int sign in Signs)for(int i=1;;i++){var q=Same(p.x+(vertical?0:i*sign),p.y+(vertical?i*sign:0),p.id);if(q==null)break;result.Add(q);}return result;}
 List<Piece> Square(Piece p){foreach(int dx in Signs)foreach(int dy in SquareYSigns){var a=Same(p.x+dx,p.y,p.id);var b=Same(p.x,p.y+dy,p.id);var c=Same(p.x+dx,p.y+dy,p.id);if(a!=null&&b!=null&&c!=null)return new List<Piece>{p,a,b,c};}return new List<Piece>{p};}
 void Flood(Piece p,List<Piece> result){if(result.Contains(p))return;result.Add(p);foreach(var d in FloodDirections){var q=Same(p.x+d.x,p.y+d.y,p.id);if(q!=null)Flood(q,result);}}
 static void Union(List<Piece> result,List<Piece> other){foreach(var p in other)if(!result.Contains(p))result.Add(p);}
 static int Rank(int skill){return skill==15?0:skill==13?1:skill==12?2:skill==11?3:skill==14?4:5;}
 List<MatchGroup> FindGroups(Piece first=null,Piece second=null){
  var origins=new List<Piece>();if(first!=null)origins.Add(first);if(second!=null&&!origins.Contains(second))origins.Add(second);for(int i=landed.Count-1;i>=0;i--){var p=landed[i];if(pieces.Contains(p)&&!origins.Contains(p))origins.Add(p);}
  for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid[x,y];if(p!=null&&p.basic&&!origins.Contains(p))origins.Add(p);}
  var consumed=new HashSet<Piece>();var result=new List<MatchGroup>();
  foreach(var origin in origins){if(!Matchable(origin)||consumed.Contains(origin))continue;var component=new List<Piece>();Flood(origin,component);if(component.Count<3)continue;bool pending=false;foreach(var p in component)if(MatchingFallPending(p)){pending=true;break;}if(pending)continue;
   MatchGroup best=null;var checkedPieces=new HashSet<Piece>();foreach(var p in component){if(checkedPieces.Contains(p))continue;var column=Run(p,true);List<Piece> row=null;foreach(var c in column){var r=Run(c,false);if(row==null||r.Count>row.Count)row=r;}var square=Square(p);var members=new List<Piece>();int skill=-1;
    if(column.Count>=5||row.Count>=5){skill=15;if(column.Count>=3)Union(members,column);if(row.Count>=5||column.Count>=5&&row.Count>=3)Union(members,row);}
    else if(column.Count>=3&&row.Count>=3){skill=13;Union(members,column);Union(members,row);}
    else if(column.Count>=4){skill=11;Union(members,column);}
    else if(row.Count>=4){skill=12;Union(members,row);}
    else if(square.Count==4){skill=14;Union(members,square);if(column.Count>=3)Union(members,column);if(row.Count>=3)Union(members,row);}
    else if(column.Count>=3){skill=0;Union(members,column);}
    else if(row.Count>=3){skill=0;Union(members,row);}
    if(skill<0)continue;foreach(var c in members)checkedPieces.Add(c);if(best==null||Rank(skill)<Rank(best.skill)||Rank(skill)==Rank(best.skill)&&members.Count>best.members.Count)best=new MatchGroup{skill=skill,members=members,origin=origin};
   }
   if(best==null)continue;if(best.skill>0&&best.skill!=14){var extra=Square(origin);if(extra.Count==4)Union(best.members,extra);}bool overlap=false;foreach(var p in best.members)if(consumed.Contains(p))overlap=true;if(overlap)continue;foreach(var p in best.members)consumed.Add(p);result.Add(best);
  }
  return result;
 }
 void ApplyMatches(List<MatchGroup> groups){foreach(var g in groups){if(TraceEvent!=null)TraceEvent("match:"+g.skill,g.origin.x,g.origin.y,g.origin.id,clock);var damaged=new HashSet<Piece>();Piece keep=g.skill>0?(g.members.Contains(g.origin)?g.origin:g.members[0]):null;foreach(var p in g.members){if(!pieces.Contains(p))continue;foreach(var d in Neighbors){int x=p.x+d.x,y=p.y+d.y;if(Inside(x,y)){var q=grid[x,y];if(q!=null&&!q.normal)Damage(q,damaged,x,y);}}}
   if(keep!=null){var destination=keep.root.anchoredPosition;foreach(var p in g.members)if(p!=keep&&pieces.Contains(p)){var ghost=ComposeTile(p);ghost.DOAnchorPos(destination,.1f).SetEase(Ease.Linear).OnComplete(()=>ReturnCompose(ghost));ClearNormal(p);}keep.reserved=true;keep.root.gameObject.SetActive(false);After(.1f,()=>{if(pieces.Contains(keep)){Trace("clear",keep);keep.root.gameObject.SetActive(true);ReplaceBasic(keep,g.skill);Sound(g.skill==15?"Audio_Match_SameColorCrusherAppear":g.skill==13?"Audio_Match_BombAppear":g.skill==14?"Audio_Match_RocketAppear":"Audio_Match_LineCrushAppear");keep.reserved=false;}});}
   else foreach(var p in g.members)ClearNormal(p);
   comboCount++;Sound("Audio_Match_Base"+Mathf.Min(comboCount,5));
  }landed.RemoveAll(p=>!pieces.Contains(p)||!p.basic);
 }
 void ClearNormal(Piece p,bool particles=true){if(p==null||!pieces.Contains(p)||!p.normal||p.dying)return;p.dying=true;Trace("hide",p);motionOrder.Remove(p);landed.Remove(p);if(particles&&p.basic)Reference20Native.BaseBurst(stage,p.root.anchoredPosition,p.id);p.root.DOKill();p.root.gameObject.SetActive(false);After(p.id==15?.06f:.16f,()=>{Trace("clear",p);Vacate(p);pieces.Remove(p);ClearedCount++;RecycleNormal(p.root);});}
 void Damage(Piece p,HashSet<Piece> hit,int hitX=-1,int hitY=-1){if(p==null||p.normal||p.bear||p.dying||p.switching||p.reserved&&p.id!=100059||!pieces.Contains(p))return;if(p.id==100059&&(hitY==p.cornBlockedRow&&hitY>=0||p.h-1-p.cornQueued<=0))return;if(!hit.Add(p))return;
  p.markedUntil=clock+.16f;var actor=p.root.GetComponent<Reference20Native>();if(p.id==100059){p.cornQueued++;p.reserved=true;if(actor)actor.Eat(p.h-1-p.cornQueued);if(!p.cornCycle)StartCornCycle(p);Sound("Audio_Match_ElementsCracked");return;}

  int before=p.hp;p.hp=Mathf.Max(0,p.hp-1);int after=p.hp;
  // Each distinct hit releases two stacks. Animation must never make a later hit disappear.
  if(p.id==100016){if(actor)actor.AnimateDamage(before,after);Sound("match3_element_dead_cupboard");Trace("cabinet-hit",p);if(after==0)p.dying=true;After(.16f,()=>{if(pieces.Contains(p))CollectCabinetCans(p);});if(after==0)After(.46f,()=>{if(pieces.Contains(p)){Vacate(p);pieces.Remove(p);ClearedCount++;Release(p);}});return;}
  float duration=actor?actor.DamageDuration(before,after):.16f;float begin=Mathf.Max(clock+(p.id==100042?.16f:0),p.lockedUntil);p.lockedUntil=begin+duration;if(actor){if(begin<=clock)actor.AnimateDamage(before,after);else After(begin-clock,()=>{if(actor)actor.AnimateDamage(before,after);});}
  if(p.id==100032)Sound(before==2?"screamChicken_break2_0":"screamChicken_break1");else if(p.id!=100042)Sound("Audio_Match_HenEgg0");
  if(p.hp>0)return;p.dying=true;After(p.lockedUntil-clock+(p.id==100042?0:.16f),()=>{if(!pieces.Contains(p))return;if(p.id==100042)LaunchPeacock(p);Vacate(p);pieces.Remove(p);motionOrder.Remove(p);ClearedCount++;Release(p);});
 }
 void StartCornCycle(Piece p){if(!pieces.Contains(p))return;p.cornCycle=true;p.cornBlockedRow=p.y;After(.332f,()=>{if(!pieces.Contains(p))return;Vacate(p);p.y++;p.h--;p.cornQueued=Mathf.Max(0,p.cornQueued-1);p.cornBlockedRow=-1;if(p.h<=1){pieces.Remove(p);ClearedCount++;var actor=p.root?p.root.GetComponent<Reference20Native>():null;After(Mathf.Max(.218f,actor?actor.CornMotionRemaining:0),()=>Release(p));}else Put(p);});After(.55f,()=>{if(!pieces.Contains(p))return;if(p.cornQueued>0)StartCornCycle(p);else{p.cornCycle=false;p.reserved=false;p.cornBlockedRow=-1;}});}
 void Release(Piece p){if(!p.root)return;if(p.id==100059)Sound("match3_element_dead_woodchipper_end");p.root.DOKill();var actor=p.root.GetComponent<Reference20Native>();if(p.id==100016||p.id==100042){var replacement=p.root.GetComponent<Reference20ReplacementVisual>();if(replacement&&p.id==100016)replacement.BreakCabinet(stage);p.root.gameObject.SetActive(false);Destroy(p.root.gameObject);return;}if(p.id==100059&&actor){var head=actor.DetachCornHead(stage);Destroy(p.root.gameObject);if(!head)return;p.root=head;actor=null;}
  Reference20Native.Raise(p.root,stage);if(actor)actor.PlayCollected();pendingCollections++;var current=p.root.anchoredPosition;DOTween.Sequence().Append(p.root.DOAnchorPos(current+new Vector2((540-current.x)*.2f,-200),.35f).SetEase(Ease.InQuad)).Append(p.root.DOAnchorPos(new Vector2(540,2200),.85f).SetEase(Ease.InQuad)).OnComplete(()=>{pendingCollections=Mathf.Max(0,pendingCollections-1);if(p.root)Destroy(p.root.gameObject);});Sound(p.id==100032?"screamChicken_end":"flyout_duang");
 }
 int ChooseRefill(int x,int y){int last=lastDropColor[x];int index=RandomRange(0,last==0?1:0);int id=last==3?4:last==4?3:index==0?3:4;lastDropColor[x]=RandomRange(1,10)<=5?id:0;return id;}
 Piece CreateNormal(int x,int y,int id,bool trace=true){var p=new Piece{x=x,y=y,id=id};var sprite=normalSprites[id];while(normalPool.Count>0&&!p.root)p.root=normalPool.Pop();if(!p.root)p.root=Box("Normal_"+x+"_"+y,stage,CellPosition(x,y),sprite.rect.size,Color.white);p.root.name="Normal_"+x+"_"+y;p.root.anchorMin=p.root.anchorMax=Vector2.zero;p.root.pivot=Vector2.one*.5f;p.root.anchoredPosition=CellPosition(x,y);p.root.sizeDelta=sprite.rect.size*.8875f;p.root.localScale=Vector3.one;p.root.localRotation=Quaternion.identity;p.root.gameObject.SetActive(true);p.image=p.root.GetComponent<Image>();p.image.enabled=true;p.image.color=Color.white;p.image.sprite=sprite;p.image.raycastTarget=false;p.simPosition=Position(p);pieces.Add(p);Put(p);if(trace)Trace("spawn",p);return p;}
 void ReplaceBasic(Piece p,int id){p.root.DOKill();for(int i=p.root.childCount-1;i>=0;i--){var child=p.root.GetChild(i);if(Reference20Native.ReleaseVisual(child,stage))continue;child.gameObject.SetActive(false);Destroy(child.gameObject);}p.id=id;p.visualOffset=Vector2.zero;p.root.anchoredPosition=CellPosition(p.x,p.y);p.simPosition=p.root.anchoredPosition;if(!p.image)p.image=p.root.gameObject.AddComponent<Image>();p.image.raycastTarget=false;p.root.localScale=Vector3.one;
  if(p.basic){p.image.enabled=true;p.image.color=Color.white;p.image.sprite=normalSprites[id];p.root.sizeDelta=normalSprites[id].rect.size*.8875f;}else{p.image.enabled=false;p.root.sizeDelta=Vector2.zero;var visual=Reference20Native.Visual(id==15?"SameColorBlock":id==13?"AreaBlock":id==14?"SquareBlock":id==12?"VerticalLineBlock":"LineBlock",p.root);visual.localScale*=.8875f;Reference20CompanyEffects.Play("Wave",stage,Position(p),.4f);}}

 void LockCells(List<Vector2Int> cells,float duration){foreach(var c in cells)if(Inside(c.x,c.y))cellLocks[c.x,c.y]++;After(duration,()=>{foreach(var c in cells)if(Inside(c.x,c.y)){cellLocks[c.x,c.y]=Mathf.Max(0,cellLocks[c.x,c.y]-1);if(cellLocks[c.x,c.y]==0)PushEmpty(c.x,c.y);}});}
 void HitCell(int x,int y,HashSet<Piece> damaged){if(!Inside(x,y))return;var p=grid[x,y];if(p==null||p.dying||p.switching||p.reserved&&!p.colorConverted&&p.id!=100059)return;if(p.normal){if(p.basic)ClearNormal(p);else Activate(p,0);}else Damage(p,damaged,x,y);}
 IEnumerator CompleteTurn(){bool won=!pieces.Exists(p=>p.bear);if(MovesPlayed<maxMoves&&!won)yield break;settlementPending=true;Sound(won?"Audio_UI_Win":"Audio_UI_Fail");yield return new WaitForSeconds(1);while(pendingCollections>0||activeOperations>0||boardEvents.Count>0||boardMoving)yield return ResolveUntilStable();ShowEnd(won);}
}
}

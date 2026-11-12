using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Reference20 {
public partial class Reference20Game {
 int activeOperations;
 Coroutine settleRoutine;
 public bool CanAcceptInput {get{return Application.isPlaying&&!loadingUIActive&&!IsFinished&&!settlementPending&&MovesPlayed<maxMoves&&matchingPauseCount==0;}}
 bool CanManipulate(Piece p){return p!=null&&(p.normal||p.id==100015)&&pieces.Contains(p)&&p.root&&p.root.gameObject.activeInHierarchy&&!p.dying&&!p.reserved&&!p.falling&&!p.paused&&!p.switching;}
 void EnsureResolution(){resolving=true;IsBusy=true;if(settleRoutine==null)settleRoutine=StartCoroutine(SettleBoard());}
 IEnumerator SettleBoard(){try{yield return ResolveUntilStable();yield return CompleteTurn();}finally{settleRoutine=null;resolving=false;IsBusy=false;Phase=IsFinished?TurnPhase.Completed:TurnPhase.Ready;}}
 void HoldForExchange(Piece p){p.switching=true;motionOrder.Remove(p);renderOrder.Remove(p);landed.Remove(p);p.root.DOKill();}
 void ReleaseExchange(Piece p){p.switching=false;if(!pieces.Contains(p)||p.dying||!p.root)return;p.previousPosition=p.simPosition=p.root.anchoredPosition;p.settledAt=clock+.05f;if(!landed.Contains(p))landed.Add(p);PushEmpty(p.x,p.y);PushEmpty(p.x,p.y+p.h);}
 void Swap(Piece a,Piece b){int ax=a.x,ay=a.y;Erase(a);Erase(b);a.x=b.x;a.y=b.y;b.x=ax;b.y=ay;Put(a);Put(b);a.root.DOKill();b.root.DOKill();a.root.DOAnchorPos(Position(a),.15f).SetEase(Ease.Linear);if(!(a.special&&b.special))b.root.DOAnchorPos(Position(b),.15f).SetEase(Ease.Linear);}
 public bool TryMove(int x,int y,int tx,int ty){
  if(!CanAcceptInput||!Inside(x,y)||!Inside(tx,ty)||Mathf.Abs(x-tx)+Mathf.Abs(y-ty)!=1)return false;
  var a=grid[x,y];var b=grid[tx,ty];if(!CanManipulate(a)||!CanManipulate(b))return false;
  if(guide&&!((x==7&&y==4&&tx==8&&ty==4)||(x==8&&y==4&&tx==7&&ty==4)))return false;
  StopHints();if(guide)RemoveGuide();HoldForExchange(a);HoldForExchange(b);activeOperations++;Phase=TurnPhase.Swapping;EnsureResolution();StartCoroutine(SwapTurn(a,b));return true;
 }
 IEnumerator SwapTurn(Piece a,Piece b){
  try{
   comboCount=0;Sound("btn_click",.35f);Swap(a,b);yield return new WaitForSeconds(.15f);
   if(!pieces.Contains(a)||!pieces.Contains(b)||a.dying||b.dying)yield break;
   a.switching=b.switching=false;
   var first=a.y==b.y?(a.x<b.x?a:b):b;var groups=FindGroups(first,first==a?b:a);bool powers=a.special||b.special;
   if(!powers&&!groups.Exists(g=>g.origin==a||g.origin==b||g.members.Contains(a)||g.members.Contains(b))){a.switching=b.switching=true;Swap(a,b);Sound("Audio_Match_WrongMatch");yield return new WaitForSeconds(.15f);yield break;}
   MovesPlayed++;
   if(powers){if(a.special&&b.special)Combine(a,b);else{var power=a.special?a:b;var plain=a.special?b:a;int color=plain.basic?plain.id:-2;if(power.id==15)ColorBlast(Position(power),color,0,power,plain);else{ApplyMatches(groups.FindAll(g=>g.members.Contains(plain)));Activate(power,color);}}}
   else ApplyMatches(groups);
  }finally{ReleaseExchange(a);ReleaseExchange(b);activeOperations=Mathf.Max(0,activeOperations-1);}
 }
 public bool TryActivate(int x,int y){
  if(!CanAcceptInput||guide||!Inside(x,y))return false;var p=grid[x,y];if(!CanManipulate(p)||!p.special)return false;
  StopHints();comboCount=0;MovesPlayed++;EnsureResolution();Activate(p,0);return true;
 }
 IEnumerator PowerTurn(Piece p){if(p!=null)TryActivate(p.x,p.y);yield break;}
 IEnumerator ResolveUntilStable(){float stableSince=-1;while(true){yield return null;bool waiting=activeOperations>0||pieces.Exists(p=>p.falling||p.reserved||p.dying||p.switching||p.normal&&p.settledAt>clock);if(!waiting&&!boardMoving&&boardEvents.Count==0&&motionOrder.Count==0){if(QueueMissedVacancies()){stableSince=-1;continue;}if(stableSince<0)stableSince=clock;if(clock-stableSince>=.1f)yield break;}else stableSince=-1;}}
}
}

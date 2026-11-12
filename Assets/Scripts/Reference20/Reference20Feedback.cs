using System.Collections.Generic;
using UnityEngine;
namespace Reference20 {
public partial class Reference20Game {
 public enum TurnPhase {Ready,Swapping,Resolving,Settling,Completed}
 public TurnPhase Phase {get;private set;}
 readonly List<Reference20CocosClip> hintClips=new List<Reference20CocosClip>();
 float idleStart;bool hintActive;
 void StopHints(){Reference20Props.StopHints();foreach(var clip in hintClips)if(clip)clip.Stop();hintClips.Clear();hintActive=false;idleStart=clock;}
 void Hint(Piece p,string name="match_tip"){if(p==null||!p.root||p.falling||p.dying)return;var clip=Reference20Native.Hint(p.root,name);if(clip)hintClips.Add(clip);}
 void IdleFeedback(){if(IsBusy||IsFinished||guide||selected!=null){idleStart=clock;return;}if(hintActive||clock-idleStart<1)return;var move=FindHintMove(true);if(move==null){var power=pieces.Find(p=>p.normal&&!p.basic&&!p.dying&&!p.reserved&&!p.falling);if(power!=null){Hint(power);hintActive=true;}return;}var mover=grid[move.from.x,move.from.y];string name=move.to.x>move.from.x?"match_tip_right":move.to.x<move.from.x?"match_tip_left":move.to.y>move.from.y?"match_tip_down":"match_tip_up";foreach(var p in move.members)Hint(p,p==mover?name:"match_tip");hintActive=true;}
 bool DragSelected(Vector2 point){if(selected==null)return false;var delta=point-press;if(Mathf.Abs(delta.x)<=25&&Mathf.Abs(delta.y)<=30)return false;int dx=0,dy=0;if(Mathf.Abs(delta.x)>Mathf.Abs(delta.y))dx=delta.x>0?1:-1;else dy=delta.y>0?-1:1;bool moved=TryMove(selected.x,selected.y,selected.x+dx,selected.y+dy);StopHints();selected=null;return moved;}
}
}

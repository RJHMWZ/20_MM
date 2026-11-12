using System.Collections.Generic;
using UnityEngine;

namespace Reference20 {
public partial class Reference20Game {
 // The source manager drains a stack of vacancies, not every column in a
 // blanket pass. A source tile/entry has its own 50 ms release interval.
 readonly List<Vector2Int> emptyCells=new List<Vector2Int>();
 readonly List<Vector2Int> cachedCells=new List<Vector2Int>();
 readonly Dictionary<int,float> fallWait=new Dictionary<int,float>();
 int CellKey(int x,int y){return (y+1)*9+x;}
 void PushEmpty(int x,int y){if(!Inside(x,y)||!playable[x,y])return;var cell=new Vector2Int(x,y);if(!emptyCells.Contains(cell))emptyCells.Add(cell);}
 void Vacate(Piece p){Erase(p);for(int y=p.y;y<p.y+p.h;y++)for(int x=p.x;x<p.x+p.w;x++){PushEmpty(x,y);if(Inside(x,y+1)&&grid[x,y+1]==null)PushEmpty(x,y+1);}}
 bool WaitReady(int x,int y){int key=CellKey(x,y);float began;if(!fallWait.TryGetValue(key,out began))return true;if(clock-began<=.05f+.00001f)return false;fallWait.Remove(key);return true;}
 void WaitFor(int x,int y){int key=CellKey(x,y);if(!fallWait.ContainsKey(key))fallWait[key]=clock;}
 void ReserveMove(Piece p,int x,int y){Trace("fall",p);Vacate(p);p.x=x;p.y=y;Put(p);if(!p.falling){var start=p.paused?p.simPosition:p.root.anchoredPosition;p.paused=false;p.fallTime=0;p.simPosition=start;p.previousPosition=p.simPosition;p.falling=true;if(p.bear)p.root.GetComponent<Reference20ReplacementVisual>().BearFalling();if(p.special){var visual=p.root.GetComponentInChildren<Reference20PropVisual>();if(visual)visual.Falling();}if(!renderOrder.Contains(p))renderOrder.Add(p);if(!motionOrder.Contains(p))motionOrder.Add(p);}p.settledAt=float.PositiveInfinity;}
 bool ConnectedAbove(int x,int y){for(int yy=y-1;yy>=entryRows[x]&&yy>=0;yy--){if(!playable[x,yy])return false;var p=grid[x,yy];if(p!=null&&!p.movable)return false;if(p!=null&&p.movable&&!p.dying)return true;}return y>=entryRows[x]&&entryRows[x]>=0;}
 Piece PreviousSource(int x,int y){if(y<=entryRows[x])return null;for(int yy=y-1;yy>=entryRows[x];yy--){if(!Inside(x,yy)||!playable[x,yy]||(cellLocks[x,yy]>0))return null;var p=grid[x,yy];if(p==null)continue;if(!p.movable||p.dying||p.reserved||p.switching||p.falling||clock<p.lockedUntil)return null;if(!Free(p,p.x,p.y+1))return null;return p;}return null;}
 bool SlantInto(int x,int y){if(y<=entryRows[x]||ConnectedAbove(x,y))return false;foreach(int dx in Signs){int sx=x+dx,sy=y-1;if(!Inside(sx,sy))continue;var p=grid[sx,sy];if(p==null||!p.movable||p.w!=1||p.h!=1||p.falling||p.dying||p.reserved||p.switching||clock<p.lockedUntil||!Free(p,x,y))continue;int px=p.x,py=p.y;ReserveMove(p,x,y);PushEmpty(px,py);return true;}return false;}
 bool WaitingSourceAbove(int x,int y){for(int yy=y-1;yy>=entryRows[x]&&yy>=0;yy--){if(!playable[x,yy])return false;var p=grid[x,yy];if(p==null)continue;return p.movable&&!p.dying&&(p.reserved||p.switching||clock<p.lockedUntil);}return false;}
 bool CanFillVacancy(int x,int y){if(!Inside(x,y)||!playable[x,y]||grid[x,y]!=null||cellLocks[x,y]>0)return false;if(y==entryRows[x]||PreviousSource(x,y)!=null||WaitingSourceAbove(x,y))return true;if(y<=entryRows[x]||ConnectedAbove(x,y))return false;foreach(int dx in Signs){int sx=x+dx,sy=y-1;if(!Inside(sx,sy))continue;var p=grid[sx,sy];if(p!=null&&p.movable&&p.w==1&&p.h==1&&!p.dying&&!p.reserved&&!p.switching&&!p.falling&&clock>=p.lockedUntil&&Free(p,x,y))return true;}return false;}
 bool QueueMissedVacancies(){bool found=false;for(int y=11;y>=0;y--)for(int x=0;x<9;x++)if(CanFillVacancy(x,y)){PushEmpty(x,y);found=true;}return found;}
 void StartFromVacancy(Vector2Int cell){int x=cell.x,y=cell.y;
  // PopEmptyTiled in the original climbs from an occupied vacancy to its
  // first empty predecessor; this keeps the entry stream alive.
  while(Inside(x,y)&&playable[x,y]&&grid[x,y]!=null){var occupied=grid[x,y];if(!occupied.movable||occupied.reserved||occupied.switching||occupied.dying)return;y--;}
  if(!Inside(x,y)||!playable[x,y]||(cellLocks[x,y]>0))return;
  if(WaitingSourceAbove(x,y)){cachedCells.Add(new Vector2Int(x,y));return;}
  var source=PreviousSource(x,y);if(source!=null){int sx=source.x,sy=source.y;if(!WaitReady(sx,sy)){cachedCells.Add(new Vector2Int(x,y));return;}ReserveMove(source,sx,sy+1);PushEmpty(sx,sy);if(sy>entryRows[sx]){var above=grid[sx,sy-1];if(above!=null&&above.movable&&!above.dying&&!above.reserved&&!above.switching)WaitFor(sx,sy-1);}else fallWait[CellKey(sx,entryRows[sx]-1)]=clock;return;}
  if(y==entryRows[x]){if(Free(null,x,y)&&WaitReady(x,y-1)){var p=CreateNormal(x,y,ChooseRefill(x,y));p.simPosition=CellPosition(x,y-1);p.root.anchoredPosition=p.simPosition;p.previousPosition=p.simPosition;renderOrder.Add(p);p.falling=true;p.spawnedAt=clock;p.settledAt=float.PositiveInfinity;motionOrder.Add(p);WaitFor(x,y-1);}cachedCells.Add(new Vector2Int(x,y));return;}
  var previous=grid[x,y-1];if(previous!=null&&(previous.falling||previous.dying||previous.reserved||previous.switching)){if(previous.h>1)cachedCells.Add(new Vector2Int(x,y));return;}
  if(!ConnectedAbove(x,y))SlantInto(x,y);
 }
 bool ContinueFall(Piece p){if(p.dying||p.reserved||p.switching||clock<p.lockedUntil)return false;if(Free(p,p.x,p.y+1)){ReserveMove(p,p.x,p.y+1);return true;}if(p.w!=1||p.h!=1)return false;foreach(int dx in Signs){int x=p.x+dx,y=p.y+1;if(!Inside(x,y)||y<=entryRows[x]||!Free(p,x,y)||ConnectedAbove(x,y)||PreviousSource(x,y)!=null)continue;ReserveMove(p,x,y);return true;}return false;}
 void TickGravity(float dt){
  foreach(var p in renderOrder)p.previousPosition=p.simPosition;
  foreach(var c in cachedCells)PushEmpty(c.x,c.y);cachedCells.Clear();int guard=0;
  while(emptyCells.Count>0&&guard++<2048){int i=emptyCells.Count-1;var cell=emptyCells[i];emptyCells.RemoveAt(i);StartFromVacancy(cell);}
  for(int i=0;i<motionOrder.Count;i++){var p=motionOrder[i];if(!pieces.Contains(p)||p.dying||!p.root){motionOrder.RemoveAt(i--);continue;}if(p.reserved||clock<=p.spawnedAt+.00001f)continue;if(p.paused){if(fallingPauseCount>0)continue;p.paused=false;p.falling=true;}var dest=Position(p);
   if((p.simPosition-dest).sqrMagnitude>.001f){p.fallTime+=dt;float speed=Mathf.Min(12000,700+2800*p.fallTime);p.simPosition=Vector2.MoveTowards(p.simPosition,dest,speed*dt);p.root.anchoredPosition=p.simPosition;}
   if((p.simPosition-dest).sqrMagnitude>.001f)continue;if(fallingPauseCount>0){p.paused=true;p.falling=false;continue;}if(ContinueFall(p))continue;
   if(Inside(p.x,p.y+p.h)){var below=grid[p.x,p.y+p.h];if(below!=null&&below!=p&&below.falling&&!below.reserved)continue;}
   p.falling=false;p.settledAt=clock+.05f;p.fallTime=0;if(p.bear)p.root.GetComponent<Reference20ReplacementVisual>().BearLanded();if(p.special){var visual=p.root.GetComponentInChildren<Reference20PropVisual>();if(visual)visual.Landed();}landed.Add(p);motionOrder.RemoveAt(i--);
  }
  for(int i=pieces.Count-1;i>=0;i--)if(pieces[i].bear)TryCollectBear(pieces[i]);
  // An occupied cached entry is no longer a pending vacancy. It will be
  // queued again when its current tile advances to the next cell.
  cachedCells.RemoveAll(c=>grid[c.x,c.y]!=null&&!grid[c.x,c.y].dying);
  boardMoving=motionOrder.Count>0||emptyCells.Count>0||cachedCells.Count>0;
  if(!boardMoving&&boardEvents.Count==0)fallWait.Clear();
 }
}
}

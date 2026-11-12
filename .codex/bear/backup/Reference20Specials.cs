using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reference20 {
public partial class Reference20Game {
 AudioSource BeginFlightSound(string name){AudioClip clip;if(!sounds.TryGetValue(name,out clip)||!clip)return null;AudioSource source;if(availableFlightAudio.Count>0)source=availableFlightAudio.Pop();else{source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.loop=true;flightAudio.Add(source);}source.clip=clip;source.volume=.8f;source.Play();return source;}
 void EndFlightSound(AudioSource source){if(!source)return;source.Stop();source.clip=null;availableFlightAudio.Push(source);}
 void BeginStandby(){standbyCount++;if(standbyCount>1)return;if(!standbySource){standbySource=gameObject.AddComponent<AudioSource>();standbySource.playOnAwake=false;standbySource.loop=true;standbySource.spatialBlend=0;}AudioClip clip;if(sounds.TryGetValue("Audio_Match_SameColorCrusherStandby",out clip)){standbySource.clip=clip;standbySource.volume=.8f;standbySource.Play();}}
 void EndStandby(){standbyCount=Mathf.Max(0,standbyCount-1);if(standbyCount==0&&standbySource)standbySource.Stop();}
 void Line(int x,int y,bool horizontal,HashSet<Piece> damaged=null,Vector2? endpoint1=null,Vector2? endpoint2=null){
  if(damaged==null)damaged=new HashSet<Piece>();var hits=damaged;var cells=new List<Vector2Int>();
  if(horizontal){for(int xx=x-1;xx>=0;xx--)if(playable[xx,y])cells.Add(new Vector2Int(xx,y));for(int xx=x;xx<9;xx++)if(playable[xx,y])cells.Add(new Vector2Int(xx,y));}
  else {for(int yy=y;yy<12;yy++)if(playable[x,yy])cells.Add(new Vector2Int(x,yy));for(int yy=y-1;yy>=0;yy--)if(playable[x,yy])cells.Add(new Vector2Int(x,yy));}
  // Source endpoints include the inactive border of its 18 x 18 map.
  var origin=CellPosition(x,y);var end1=endpoint1??(horizontal?CellPosition(-4,y):CellPosition(x,14));var end2=endpoint2??(horizontal?CellPosition(13,y):CellPosition(x,-3));float distance=Mathf.Max(Vector2.Distance(origin,end1),Vector2.Distance(origin,end2));
  float duration=(Mathf.Ceil(distance/2000/Tick)+1)*Tick;LockCells(cells,duration);
  float effectDuration=(Mathf.Ceil(2500f/2000f/Tick)+1)*Tick;Reference20Native.LineBlast(stage,CellPosition(x,y),horizontal,effectDuration);After(effectDuration,()=>{});HitCell(x,y,hits);
  float interval=Mathf.Ceil(.02f/Tick)*Tick;for(int i=1;i<(horizontal?9:12);i++){int offset=i;After(interval*i,()=>{if(horizontal){HitCell(x+offset,y,hits);HitCell(x-offset,y,hits);}else{HitCell(x,y+offset,hits);HitCell(x,y-offset,hits);}});}Sound("Audio_Match_LineCrusher");
 }
 void Bomb(int x,int y,int radius=2,float wait=-1){
  var cells=new List<Vector2Int>();for(int yy=y-radius;yy<=y+radius;yy++)for(int xx=x-radius;xx<=x+radius;xx++)if(Inside(xx,yy)&&!(radius==3&&Mathf.Abs(xx-x)==3&&Mathf.Abs(yy-y)==3))cells.Add(new Vector2Int(xx,yy));
  float delay=wait>=0?wait:radius==3?2:.23f;LockCells(cells,delay);Reference20Native.Effect(radius==3?"AreaAreaEffect":"AreaEffect",stage,CellPosition(x,y));
  if(radius==3){After(1.9f,()=>Reference20Native.Effect("AreaEffect",stage,CellPosition(x,y),.8875f*2));Sound("Audio_Match_BigBomb_Shake");}else Sound("Audio_Match_AreaCrusher");
  After(delay,()=>{var hit=new HashSet<Piece>();foreach(var c in cells)HitCell(c.x,c.y,hit);});
 }
 Vector2Int? BeeTarget(){var obstacles=new List<Vector2Int>();var others=new List<Vector2Int>();var fallback=new List<Vector2Int>();for(int y=0;y<12;y++)for(int x=0;x<9;x++){if(!playable[x,y])continue;var cell=new Vector2Int(x,y);fallback.Add(cell);var p=grid[x,y];if(beeTargets.Contains(cell)||p==null||p.dying||p.reserved||p.falling||clock<p.markedUntil)continue;if(!p.normal)obstacles.Add(cell);else others.Add(cell);}var candidates=obstacles.Count>0?obstacles:others.Count>0?others:fallback;return candidates.Count==0?(Vector2Int?)null:candidates[RandomRange(0,candidates.Count-1)];}
 void Bee(Vector2 from,int payload=0,int ordinal=0){
  var target=BeeTarget();if(!target.HasValue)return;var cell=target.Value;beeTargets.Add(cell);bool reverse=RandomRange(0,1)==0;Vector2 end=CellPosition(cell.x,cell.y);float duration=Mathf.Clamp(.95f*Vector2.Distance(from,end)/(4*Cell),.84f,1.1f)+ordinal*.1f;
  Reference20Native.BeeFlight(stage,from,end,duration,payload,reverse);var flight=BeginFlightSound(payload==0?"Audio_Match_Rocket_Flying":"Audio_Match_Rocket_FlyRotate");After(duration,()=>EndFlightSound(flight));After(duration+(payload==0?.2f:0),()=>{beeTargets.Remove(cell);int x=cell.x,y=cell.y;if(payload==11||payload==12)Line(x,y,payload==11);else if(payload==13)Bomb(x,y,2,0);else HitCell(x,y,new HashSet<Piece>());Sound("Audio_Match_Rocket_Explode");});
 }
 void BeeLaunch(int x,int y,Vector2 position,int payload=0,int count=1){
  var hit=new HashSet<Piece>();foreach(var d in Neighbors)HitCell(x+d.x,y+d.y,hit);
  if(count==3)foreach(int dx in Signs)foreach(int dy in Signs)HitCell(x+dx,y+dy,hit);
  Reference20Native.Effect(count==3?"SquareBigEffect":"SquareEffect",stage,position);for(int i=0;i<count;i++)Bee(position,payload,i);
 }
 void Activate(Piece p,int color){
  if(p==null||!pieces.Contains(p)||p.reserved&&!p.colorConverted||p.dying||p.switching)return;if(p.colorConverted){p.reserved=false;p.colorConverted=false;}int id=p.id,x=p.x,y=p.y;var position=Position(p);
  if(id==15){ColorBlast(position,color,0,p,null);return;}if(id==13){p.reserved=true;Bomb(x,y);After(.23f,()=>{p.reserved=false;ClearNormal(p,false);});return;}ClearNormal(p,false);
  if(id==11||id==12)Line(x,y,id==11);else if(id==13)Bomb(x,y);else if(id==14)BeeLaunch(x,y,position);
 }
 void ColorBlast(Vector2 position,int color,int payload,Piece source=null,Piece companion=null,bool swapOrigin=false){
  if(color==0){int blue=0,yellow=0;foreach(var p in pieces)if(!p.falling&&!p.dying&&!p.reserved&&!p.switching){if(p.id==3)blue++;if(p.id==4)yellow++;}color=blue==0&&yellow==0?-1:blue>=yellow?3:4;}
  if(source!=null){source.reserved=true;source.root.gameObject.SetActive(false);}if(companion!=null){companion.reserved=true;if(payload>0)companion.root.gameObject.SetActive(false);}
  matchingPauseCount++;BeginStandby();var ready=Reference20Native.Effect("SameColorEffect",stage,position,.8875f,3600);var targets=new List<Piece>();var shakes=new List<RectTransform>();
  Action finish=()=>{
   matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);EndStandby();Reference20Native.FinishEffect(ready);
   if(swapOrigin&&source!=null&&companion!=null&&pieces.Contains(source)&&pieces.Contains(companion)){int sx=source.x,sy=source.y;Erase(source);Erase(companion);source.x=companion.x;source.y=companion.y;companion.x=sx;companion.y=sy;Put(source);Put(companion);companion.simPosition=companion.root.anchoredPosition=Position(companion);}
   if(source!=null){source.reserved=false;ClearNormal(source,false);}StopHints();
   if(payload==0){if(companion!=null){companion.reserved=false;if(companion.normal)ClearNormal(companion);else Damage(companion,new HashSet<Piece>());}foreach(var target in targets){var hit=new HashSet<Piece>();target.reserved=false;if(!pieces.Contains(target))continue;foreach(var d in Neighbors){int x=target.x+d.x,y=target.y+d.y;if(Inside(x,y)&&grid[x,y]!=null&&!grid[x,y].normal)Damage(grid[x,y],hit,x,y);}ClearNormal(target);}After(.16f,()=>{foreach(var effect in shakes)Reference20Native.FinishEffect(effect);});}
   else {if(companion!=null&&pieces.Contains(companion)){companion.root.gameObject.SetActive(true);companion.colorConverted=true;targets.Add(companion);}targets.Sort((a,b)=>(a.y*9+a.x).CompareTo(b.y*9+b.x));TriggerConverted(targets);}
   Sound("Audio_Match_SameColorCrusherExplode");
  };
  Action collect=null;collect=()=>{
   var pending=new List<Piece>();for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid[x,y];if(p==null||!p.basic||p.dying||p.falling||p.switching||color!=-2&&p.id!=color||targets.Contains(p)||p.reserved&&!(p==companion&&payload==0)||payload==0&&beeTargets.Contains(new Vector2Int(x,y)))continue;pending.Add(p);targets.Add(p);p.reserved=true;}
   int remaining=pending.Count;if(remaining==0){After(.3f,finish);return;}
   while(pending.Count>0){int index=RandomRange(0,pending.Count-1);var target=pending[index];pending.RemoveAt(index);int special=payload==11||payload==12?(RandomRange(0,1)==0?11:12):payload;float delay=pending.Count*(payload==0?.07f:.1f);Reference20Native.ColorBeam(stage,position,Position(target),delay);
    After(delay+.2f,()=>{if(pieces.Contains(target)){if(special>0){ReplaceBasic(target,special);target.colorConverted=true;}else{Hint(target,"ele_anim_samecolorblock_shake");shakes.Add(Reference20Native.Effect("SamecolorTargetShakeEffect",stage,Position(target),.8875f,3600));}}remaining--;if(remaining==0)collect();});
   }
  };collect();
 }
 void TriggerConverted(List<Piece> targets){int index=0;Action next=null;next=()=>{while(index<targets.Count){var p=targets[index++];if(!pieces.Contains(p)||p.dying||p.falling||p.basic||p.reserved&&!p.colorConverted)continue;p.reserved=false;p.colorConverted=false;Activate(p,0);After(.25f,next);return;}};next();}
 void DoubleColor(Piece a,Piece b,Vector2 position){
  fallingPauseCount++;matchingPauseCount++;a.reserved=b.reserved=true;a.root.gameObject.SetActive(false);b.root.gameObject.SetActive(false);Reference20Native.Effect("SameColorChangeEffect",stage,position);Sound("Audio_Match_SameColorCrusherBoom");
  After(1.8f,()=>{Reference20Native.Effect("AreaEffect",stage,position,.8875f*3);a.reserved=b.reserved=false;ClearNormal(a,false);ClearNormal(b,false);var hit=new HashSet<Piece>();var targets=new List<Piece>(pieces);float last=0;
   foreach(var target in targets){if(target==a||target==b||target.dying)continue;var p=target;float delay=Mathf.Floor(Vector2.Distance(CellPosition(p.x,p.y),position)/200)*.15f;last=Mathf.Max(last,delay);After(delay,()=>{if(!pieces.Contains(p)||p.dying||p.switching||p.reserved)return;if(p.normal){if(p.basic)ClearNormal(p);else Activate(p,0);}else Damage(p,hit);});}
   After(last+.15f,()=>{fallingPauseCount=Mathf.Max(0,fallingPauseCount-1);matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);});
  });
 }
 void Combine(Piece a,Piece b){
  int ia=a.id,ib=b.id,x=a.x,y=a.y;Vector2 pos=Position(a);
  if(ia==15&&ib==15){DoubleColor(a,b,pos);return;}if(ia==15||ib==15){ColorBlast(pos,0,ia==15?ib:ia,ia==15?a:b,ia==15?b:a,ia==15);return;}
  if(ia==13&&ib==13){a.reserved=b.reserved=true;After(2,()=>{a.reserved=b.reserved=false;ClearNormal(a,false);ClearNormal(b,false);});Bomb(x,y,3);return;}
  ClearNormal(a,false);ClearNormal(b,false);
  if(ia==14||ib==14){int payload=ia==14?ib:ia;BeeLaunch(x,y,pos,payload==14?0:payload,payload==14?3:1);}

  else {var hit=new HashSet<Piece>();if(ia==13||ib==13){Vector2? h1=null,h2=null,v1=null,v2=null;for(int yy=y-1;yy<=y+1;yy++)if(Inside(x,yy)&&playable[x,yy]){if(!h1.HasValue){h1=CellPosition(-4,yy);h2=CellPosition(13,yy);}Line(x,yy,true,hit,h1,h2);}for(int xx=x-1;xx<=x+1;xx++)if(Inside(xx,y)&&playable[xx,y]){if(!v1.HasValue){v1=CellPosition(xx,14);v2=CellPosition(xx,-3);}Line(xx,y,false,hit,v1,v2);}Sound("Audio_Match_LineAndAreaCrusher");}else{Line(x,y,true,hit);Line(x,y,false,hit);Sound("Audio_Match_LineAndLineCrusher");}}

 }
}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Reference20 {
public partial class Reference20Game {
 RectTransform magicFocus;int magicFocusLeases;
 void BeginColorFocus(){if(!magicFocus){magicFocus=Box("MagicBallFocusMask",Reference20Native.Overlay(stage),new Vector2(540,960),new Vector2(6000,6000),new Color(0,0,0,.7f));magicFocus.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;}magicFocusLeases++;}
 void EndColorFocus(){magicFocusLeases=Mathf.Max(0,magicFocusLeases-1);if(magicFocusLeases==0&&magicFocus){Destroy(magicFocus.gameObject);magicFocus=null;}}
 AudioSource BeginFlightSound(string name){name=Reference20CompanyAudio.Name(name);if(name==null)return null;AudioClip clip;if(!sounds.TryGetValue(name,out clip)||!clip)return null;AudioSource source;if(availableFlightAudio.Count>0)source=availableFlightAudio.Pop();else{source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;Configure2DAudio(source);source.loop=true;flightAudio.Add(source);}source.clip=clip;source.volume=.8f;source.Play();return source;}
 void EndFlightSound(AudioSource source){if(!source)return;source.Stop();source.clip=null;availableFlightAudio.Push(source);}
 void BeginStandby(){standbyCount++;if(standbyCount>1)return;if(!standbySource){standbySource=gameObject.AddComponent<AudioSource>();standbySource.playOnAwake=false;standbySource.loop=true;Configure2DAudio(standbySource);}AudioClip clip;if(sounds.TryGetValue("Audio_Match_SameColorCrusherStandby",out clip)){standbySource.clip=clip;standbySource.volume=.8f;standbySource.Play();}}
 void EndStandby(){standbyCount=Mathf.Max(0,standbyCount-1);if(standbyCount==0&&standbySource)standbySource.Stop();}
 void Line(int x,int y,bool horizontal,HashSet<Piece> damaged=null,Vector2? endpoint1=null,Vector2? endpoint2=null,bool instant=false){
  if(damaged==null)damaged=new HashSet<Piece>();var hits=damaged;var cells=new List<Vector2Int>();
  if(horizontal){for(int xx=x-1;xx>=0;xx--)if(playable[xx,y])cells.Add(new Vector2Int(xx,y));for(int xx=x;xx<9;xx++)if(playable[xx,y])cells.Add(new Vector2Int(xx,y));}
  else {for(int yy=y;yy<12;yy++)if(playable[x,yy])cells.Add(new Vector2Int(x,yy));for(int yy=y-1;yy>=0;yy--)if(playable[x,yy])cells.Add(new Vector2Int(x,yy));}
  const float flightDuration=1.5f;const float cellInterval=1f/15f;
  float clearDuration=instant?.4f:(horizontal?9:12)*cellInterval;LockCells(cells,clearDuration);
  Reference20Native.LineBlast(stage,CellPosition(x,y),horizontal,flightDuration);HitCell(x,y,hits);
  if(instant){foreach(var c in cells)HitCell(c.x,c.y,hits);Sound("Audio_Match_LineCrusher");return;}
  for(int i=1;i<(horizontal?9:12);i++){int offset=i;After(cellInterval*(i-1),()=>{if(horizontal){HitCell(x+offset,y,hits);HitCell(x-offset,y,hits);}else{HitCell(x,y+offset,hits);HitCell(x,y-offset,hits);}});}Sound("Audio_Match_LineCrusher");
 }
 void Bomb(int x,int y,int radius=1,float wait=-1){
  var cells=new List<Vector2Int>();for(int yy=y-radius;yy<=y+radius;yy++)for(int xx=x-radius;xx<=x+radius;xx++)if(Inside(xx,yy)&&!(radius==3&&Mathf.Abs(xx-x)==3&&Mathf.Abs(yy-y)==3))cells.Add(new Vector2Int(xx,yy));
  float delay=wait>=0?wait:0;LockCells(cells,Mathf.Max(.2f,delay));Reference20Native.Effect("AreaEffect",stage,CellPosition(x,y));Sound("Audio_Match_AreaCrusher");
  After(delay,()=>{var hit=new HashSet<Piece>();foreach(var c in cells)HitCell(c.x,c.y,hit);});
 }
 Vector2Int? BeeTarget(){var obstacles=new List<Vector2Int>();var others=new List<Vector2Int>();var fallback=new List<Vector2Int>();for(int y=0;y<12;y++)for(int x=0;x<9;x++){if(!playable[x,y])continue;var cell=new Vector2Int(x,y);var p=grid[x,y];if(p!=null&&p.bear)continue;fallback.Add(cell);if(beeTargets.Contains(cell)||p==null||p.dying||p.reserved||p.falling||clock<p.markedUntil)continue;if(!p.normal)obstacles.Add(cell);else others.Add(cell);}var candidates=obstacles.Count>0?obstacles:others.Count>0?others:fallback;return candidates.Count==0?(Vector2Int?)null:candidates[RandomRange(0,candidates.Count-1)];}
 void Bee(Vector2 from,int payload=0,int ordinal=0,bool combination=false,bool targetCross=false,Vector2? payloadOffset=null){
  var target=BeeTarget();if(!target.HasValue)return;var cell=target.Value;beeTargets.Add(cell);bool reverse=false;Vector2 end=CellPosition(cell.x,cell.y);float duration=combination?.5f:.7f;
  Reference20Native.BeeFlight(stage,from,end,duration,payload,reverse,payloadOffset);var flight=BeginFlightSound(payload==0?"Audio_Match_Rocket_Flying":"Audio_Match_Rocket_FlyRotate");After(duration,()=>EndFlightSound(flight));After(duration,()=>{beeTargets.Remove(cell);int x=cell.x,y=cell.y;if(payload==11||payload==12)Line(x,y,payload==11,instant:combination);else if(payload==13)Bomb(x,y,1,0);else{var hit=new HashSet<Piece>();HitCell(x,y,hit);if(targetCross)foreach(var d in Neighbors)HitCell(x+d.x,y+d.y,hit);}Sound("Audio_Match_Rocket_Explode");});
 }
 void BeeLaunch(int x,int y,Vector2 position,int payload=0,int count=1,bool combination=false,Vector2? payloadOffset=null){
  if(!combination){var hit=new HashSet<Piece>();foreach(var d in Neighbors)HitCell(x+d.x,y+d.y,hit);}
  for(int i=0;i<count;i++){if(!combination||count==3)Reference20Native.Effect("SquareEffect",stage,position);Bee(position,payload,i,combination,count==3,payloadOffset);}
 }
 void Activate(Piece p,int color){
  if(p==null||!pieces.Contains(p)||p.reserved&&!p.colorConverted||p.dying||p.switching)return;if(p.colorConverted){p.reserved=false;p.colorConverted=false;}int id=p.id,x=p.x,y=p.y;var position=Position(p);
  if(id==15){ColorBlast(position,color,0,p,null);return;}ClearNormal(p,false);
  if(id==11||id==12)Line(x,y,id==11);else if(id==13)Bomb(x,y);else if(id==14)BeeLaunch(x,y,position);
 }
 void ColorBlast(Vector2 position,int color,int payload,Piece source=null,Piece companion=null,bool swapOrigin=false){
  if(color==0){var colors=new List<int>();foreach(var p in pieces)if(p.basic&&!p.dying&&!colors.Contains(p.id))colors.Add(p.id);color=colors.Count==0?-1:colors[RandomRange(0,colors.Count-1)];}
  if(source!=null){source.reserved=true;source.root.gameObject.SetActive(false);}if(companion!=null&&payload>0){companion.reserved=true;companion.root.gameObject.SetActive(false);}
  matchingPauseCount++;var ready=Reference20Native.Effect("SameColorEffect",stage,position,.8875f,3600);var targets=pieces.FindAll(p=>p.basic&&!p.dying&&!p.falling&&!p.switching&&!p.reserved&&(color==-2||p.id==color));
  targets.Sort((a,b)=>Vector2.SqrMagnitude(Position(a)-position).CompareTo(Vector2.SqrMagnitude(Position(b)-position)));
  for(int i=0;i<targets.Count;i++){
   var target=targets[i];target.reserved=true;float delay=payload==0?i*.05f:0;Reference20Native.ColorBeam(stage,position,Position(target),delay);
   After(delay,()=>{if(!target.root||!pieces.Contains(target))return;Reference20Props.ChangeNormal(target.root,target.id);});
   After(delay+.5f,()=>{if(!pieces.Contains(target))return;Reference20Native.BaseBurst(stage,Position(target),target.id);
    if(payload>0){ReplaceBasic(target,payload==11||payload==12?(RandomRange(0,1)==0?11:12):payload);target.reserved=false;target.colorConverted=true;target.root.localScale=Vector3.zero;target.root.DOScale(Vector3.one,.25f).SetEase(Ease.OutBack);float born=Reference20Props.CreationDuration(target.id);After(Mathf.Clamp(born*.6f,.2f,.5f),()=>{if(pieces.Contains(target))Activate(target,0);});}
   });
  }
  float finishAt=targets.Count==0?.3f:(payload==0?(targets.Count-1)*.05f:0)+.5f;
  After(finishAt,()=>{
   if(payload==0){foreach(var target in targets){target.reserved=false;if(!pieces.Contains(target))continue;var hit=new HashSet<Piece>();foreach(var d in Neighbors){int x=target.x+d.x,y=target.y+d.y;if(Inside(x,y)&&grid[x,y]!=null&&!grid[x,y].normal)Damage(grid[x,y],hit,x,y);}ClearNormal(target,false);}}
   if(companion!=null){companion.reserved=false;if(companion.normal)ClearNormal(companion,false);else Damage(companion,new HashSet<Piece>());}
   if(ready){ready.DOScale(Vector3.zero,.3f).SetEase(Ease.InBack);var fade=ready.GetComponent<CanvasGroup>();if(!fade)fade=ready.gameObject.AddComponent<CanvasGroup>();fade.DOFade(0,.3f);}
   After(.3f,()=>{matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);Reference20Native.FinishEffect(ready);if(source!=null){source.reserved=false;ClearNormal(source,false);}});
   Sound("Audio_Match_SameColorCrusherExplode");
  });
 }
 void DoubleColor(Piece a,Piece b,Vector2 position){
  fallingPauseCount++;matchingPauseCount++;a.reserved=b.reserved=true;a.root.gameObject.SetActive(false);b.root.gameObject.SetActive(false);
  BeginColorFocus();
  var center=Reference20Props.ColorComboCenter(position);var merge=Reference20Native.Effect("SameColorChangeEffect",stage,position);merge.DOAnchorPos(center,.6f).SetEase(Ease.InOutQuad);Sound("Audio_Match_SameColorCrusherBoom");
  RectTransform charge=null;After(.6f,()=>{charge=Reference20CompanyEffects.Play("ColorCharge",stage,center,1.5f);Reference20CompanyParticles.RaiseAboveFocus();foreach(var p in pieces)if(p.basic&&!p.dying&&!p.reserved&&p.root)Reference20Props.ChangeNormal(p.root,p.id);});
  After(2.166667f,()=>{EndColorFocus();Reference20CompanyEffects.Play("ColorFinish",stage,center);Reference20Native.FinishEffect(charge);a.reserved=b.reserved=false;ClearNormal(a,false);ClearNormal(b,false);var hit=new HashSet<Piece>();var targets=new List<Piece>(pieces);
   foreach(var p in targets){if(p==a||p==b||p.dying||p.switching||p.reserved)continue;if(p.normal){if(p.basic)ClearNormal(p);else Activate(p,0);}else Damage(p,hit);}
   After(.3f,()=>{Reference20Native.FinishEffect(merge);fallingPauseCount=Mathf.Max(0,fallingPauseCount-1);matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);});
  });
 }
 void Combine(Piece a,Piece b){
  bool color=a.id==15||b.id==15,bombs=a.id==13&&b.id==13,planes=a.id==14&&b.id==14,lines=(a.id==11||a.id==12)&&(b.id==11||b.id==12);
  if(!color&&!bombs&&!planes&&!lines){a.reserved=b.reserved=true;var ap=Position(a);var bp=Position(b);var center=(ap+bp)*.5f;var offset=ap-center;bool horizontal=Mathf.Abs(offset.x)>Mathf.Abs(offset.y);var sa=a.root.localScale;var sb=b.root.localScale;a.root.DOScale(sa*1.3f,.2f);b.root.DOScale(sb*1.3f,.2f);
   foreach(var p in new[]{a,b})if(p.id==14){var visual=p.root.GetComponentInChildren<Reference20PropVisual>();if(visual)visual.Rig.AnimationState.SetAnimation(0,"Bomb_fly1_1",false);Reference20Native.Effect("SquareEffect",stage,Position(p));}
   DOTween.To(()=>0f,angle=>{float c=Mathf.Cos(angle),s=Mathf.Sin(angle);var d=horizontal?new Vector2(offset.x*c,offset.x*s*.3f):new Vector2(offset.y*s*.3f,offset.y*c);if(a.root)a.root.anchoredPosition=center+d;if(b.root)b.root.anchoredPosition=center-d;},Mathf.PI*2,.5f).SetEase(Ease.Linear).SetTarget(a.root);
   After(.5f,()=>{if(a.root){a.root.anchoredPosition=ap;a.root.DOScale(sa,.2f);}if(b.root){b.root.anchoredPosition=bp;b.root.DOScale(sb,.2f);}});
   After(.7f,()=>{a.reserved=b.reserved=false;ResolveCombo(a,b);});return;
  }
  ResolveCombo(a,b);
 }
 void ResolveCombo(Piece a,Piece b){
  int ia=a.id,ib=b.id,x=a.x,y=a.y;Vector2 pos=Position(a);
  if(ia==15&&ib==15){DoubleColor(a,b,pos);return;}if(ia==15||ib==15){ColorBlast(pos,0,ia==15?ib:ia,ia==15?a:b,ia==15?b:a,ia==15);return;}
  if(ia==13&&ib==13){a.reserved=b.reserved=true;After(1.666667f,()=>{a.reserved=b.reserved=false;ClearNormal(a,false);ClearNormal(b,false);Bomb(x,y,2);});Reference20Native.Effect("AreaAreaEffect",stage,pos);return;}

  ClearNormal(a,false);ClearNormal(b,false);
  if(ia==14||ib==14){int payload=ia==14?ib:ia;var from=payload==14?(Position(a)+Position(b))*.5f:ia==14?Position(a):Position(b);var other=ia==14?Position(b):Position(a);BeeLaunch(x,y,from,payload==14?0:payload,payload==14?3:1,true,other-from);}

  else {var hit=new HashSet<Piece>();if(ia==13||ib==13){Reference20Props.ParticleHalo(stage,pos,false);Vector2? h1=null,h2=null,v1=null,v2=null;for(int yy=y-1;yy<=y+1;yy++)if(Inside(x,yy)&&playable[x,yy]){if(!h1.HasValue){h1=CellPosition(-4,yy);h2=CellPosition(13,yy);}Line(x,yy,true,hit,h1,h2,true);}for(int xx=x-1;xx<=x+1;xx++)if(Inside(xx,y)&&playable[xx,y]){if(!v1.HasValue){v1=CellPosition(xx,14);v2=CellPosition(xx,-3);}Line(xx,y,false,hit,v1,v2,true);}Sound("Audio_Match_LineAndAreaCrusher");}else{Line(x,y,true,hit,instant:true);Line(x,y,false,hit,instant:true);Sound("Audio_Match_LineAndLineCrusher");}}

 }
}
}

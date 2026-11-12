using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;

namespace Reference20 {
public partial class Reference20Game {
 Vector2Int? BirdTarget(Piece origin){var empty=new List<Vector2Int>();var basic=new List<Vector2Int>();var nearby=new List<Vector2Int>();var other=new List<Vector2Int>();var from=CellPosition(origin.x,origin.y);
  for(int y=0;y<12;y++)for(int x=0;x<9;x++){if(!playable[x,y])continue;var cell=new Vector2Int(x,y);if(beeTargets.Contains(cell))continue;var p=grid[x,y];if(p==null){empty.Add(cell);continue;}if(p.basic){basic.Add(cell);if(Vector2.Distance(from,CellPosition(x,y))<512*.8875f)nearby.Add(cell);}else if(p.movable&&!p.bear&&!p.dying)other.Add(cell);}
  if(empty.Count==0)RandomRange(0,-1);var candidates=empty.Count>0?empty:nearby.Count>0?nearby:basic.Count>0?basic:other;if(candidates.Count==0)return null;var target=candidates[RandomRange(0,candidates.Count-1)];beeTargets.Add(target);cellLocks[target.x,target.y]++;return target;
 }
 void ReleaseBirdTarget(Vector2Int cell){beeTargets.Remove(cell);cellLocks[cell.x,cell.y]=Mathf.Max(0,cellLocks[cell.x,cell.y]-1);PushEmpty(cell.x,cell.y);PushEmpty(cell.x,cell.y+1);}
 Piece BirdDestination(Vector2Int cell){var old=grid[cell.x,cell.y];if(old!=null){Vacate(old);pieces.Remove(old);motionOrder.Remove(old);renderOrder.Remove(old);landed.Remove(old);if(!old.dying)RecycleNormal(old.root);}var p=CreateNormal(cell.x,cell.y,3,false);p.settledAt=clock;p.previousPosition=p.simPosition=CellPosition(cell.x,cell.y);return p;}
 void LaunchPeacock(Piece source){var from=Position(source);for(int i=0;i<3;i++){var target=BirdTarget(source);if(!target.HasValue)continue;var cell=target.Value;var to=CellPosition(cell.x,cell.y);float duration=Mathf.Max(.7f,Vector2.Distance(from,to)/500);var bird=Reference20ReplacementVisual.Leaf(Reference20Native.Overlay(stage));bird.anchorMin=bird.anchorMax=Vector2.zero;bird.anchoredPosition=from;if(bird)Reference20Native.Fly(bird,from,to,duration);
   After(duration,()=>{Reference20Native.HideEffect(bird);var p=BirdDestination(cell);ReplaceBasic(p,15);Trace("born",p);Reference20Native.Effect("CommonBornPropsEffect",stage,to,.8875f,.5f);Sound("Audio_Match_SameColorCrusherAppear");Sound("shrubPeacock_duang");After(.5f,()=>{ReleaseBirdTarget(cell);if(bird)Destroy(bird.gameObject);});});
  }}
 void CollectCabinetCans(Piece cabinet){if(!cabinet.root)return;var actor=cabinet.root.GetComponent<Reference20Native>();if(!actor)return;Sound("match3_element_dead_cupboard_fishcan");
  for(int i=0;i<2;i++){var can=actor.DetachNestChick(stage);if(!can)break;Vector2 top;RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,new Vector2(Screen.width*.5f,Screen.height+180),null,out top);var destination=top+stage.sizeDelta*.5f+new Vector2(i==0?-45:45,0);var from=can.anchoredPosition;float duration=Mathf.Max(.65f,Vector2.Distance(from,destination)/1500f);pendingCollections++;Reference20Native.Fly(can,from,destination,duration);DOTween.Sequence().SetTarget(can).AppendInterval(duration).AppendCallback(()=>{pendingCollections=Mathf.Max(0,pendingCollections-1);if(can)Destroy(can.gameObject);});
  }
 }
 void ReplaceWithChick(Piece p,SkeletonDataAsset data,string skin){p.root.DOKill();p.root.gameObject.SetActive(false);RecycleNormal(p.root);p.id=100015;p.hp=1;p.w=p.h=1;p.visualOffset=Vector2.zero;p.root=data?Reference20Native.Visual("HenEggBlock",stage):Reference20ReplacementVisual.CreateCan(stage);p.root.anchorMin=p.root.anchorMax=Vector2.zero;p.root.anchoredPosition=Position(p);p.root.localScale=Vector3.one*.8875f;var actor=p.root.gameObject.AddComponent<Reference20Native>();actor.UseChickData(data,skin);actor.Configure(p.id,1);p.image=p.root.GetComponent<Image>();p.simPosition=p.previousPosition=Position(p);Put(p);Trace("born",p);}
}
}

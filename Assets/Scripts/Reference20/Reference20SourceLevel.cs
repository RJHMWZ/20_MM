using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reference20 {
public partial class Reference20Game {
 [Serializable] sealed class SourceTopology {public int seed;public SourceTile[] tiles;}
 [Serializable] sealed class SourceTile {public int x,y;public bool entry;}
 void ApplySourceTopology(){var level=Newtonsoft.Json.JsonConvert.DeserializeObject<SourceTopology>(Resources.Load<TextAsset>("Reference20/Native/level-topology").text);randomSeed=level.seed;playable=new bool[9,12];for(int x=0;x<9;x++)entryRows[x]=-1;foreach(var tile in level.tiles){if(!Inside(tile.x,tile.y))continue;playable[tile.x,tile.y]=true;if(tile.entry)entryRows[tile.x]=tile.y;}}
 sealed class HintMove {public Vector2Int from,to;public int priority;public List<Piece> members;}
 static int HintSkillPriority(int skill){return skill==15?3:skill==13?4:skill==11||skill==12?5:skill==14?6:11;}
 static int HintPowerPriority(int a,int b){if(a==15&&b==15)return 1;if(a==15||b==15)return a<5||b<5?12:0;if(a<5||b<5)return 12;if(a==14&&b==14)return 10;if(a==14||b==14)return 9;if(a==13&&b!=13||b==13&&a!=13)return 7;return 8;}
 HintMove FindHintMove(bool random){var candidates=new List<HintMove>();int priority=int.MaxValue;for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid[x,y];if(p==null||!p.normal||p.dying||p.reserved||p.falling||p.switching)continue;foreach(var d in MoveDirections){int nx=x+d.x,ny=y+d.y;if(!Inside(nx,ny))continue;var q=grid[nx,ny];if(q==null||!q.normal||q.dying||q.reserved||q.falling||q.switching)continue;HintMove move=null;if(!p.basic||!q.basic){move=new HintMove{from=new Vector2Int(x,y),to=new Vector2Int(nx,ny),priority=HintPowerPriority(p.id,q.id),members=new List<Piece>{p,q}};}
    if(move==null&&p.basic&&q.basic){int a=p.id,b=q.id;p.id=b;q.id=a;var groups=FindGroups(p,q);p.id=a;q.id=b;foreach(var group in groups){if(!group.members.Contains(p)&&!group.members.Contains(q))continue;int rank=HintSkillPriority(group.skill);if(move!=null&&move.priority<=rank)continue;var members=new List<Piece>();foreach(var tile in group.members)members.Add(tile==p?q:tile==q?p:tile);var mover=group.members.Contains(p)?q:p;var other=mover==p?q:p;move=new HintMove{from=new Vector2Int(mover.x,mover.y),to=new Vector2Int(other.x,other.y),priority=rank,members=members};}}
    if(move==null||move.priority>priority)continue;if(move.priority<priority){priority=move.priority;candidates.Clear();}candidates.Add(move);
   }}if(candidates.Count==0)return null;return candidates[random?Mathf.RoundToInt(UnityEngine.Random.value*(candidates.Count-1)):0];}
 public bool TryGetValidMove(out Vector2Int from,out Vector2Int to){var move=FindHintMove(false);from=move==null?Vector2Int.zero:move.from;to=move==null?Vector2Int.zero:move.to;return move!=null;}
}
}

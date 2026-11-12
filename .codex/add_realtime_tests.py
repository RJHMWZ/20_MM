from pathlib import Path
Path('Assets/Editor/Reference20RealtimeRunner.cs').write_text('''using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Reference20;

[InitializeOnLoad]
public static class Reference20RealtimeRunner {
 const string Key="Reference20.RealtimeSuite";
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static readonly string[] Names={"line_parallel","bomb_parallel","color_gate","overlapping_swaps","movable_chick","bee_pool_idle"};
 static readonly List<object> results=new List<object>();static readonly List<string> errors=new List<string>();
 static Reference20Game game;static int index,phase,lastFrame;static double at;static bool started,first,second,exchange,duplicate;
 static Reference20RealtimeRunner(){EditorApplication.update+=Tick;}
 public static void Begin(){SessionState.SetString(Key,"running");started=false;index=0;results.Clear();if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
 static object Field(object p,string name){return p.GetType().GetField(name,Flags|BindingFlags.Public).GetValue(p);}
 static object Tile(int x,int y){return ((Array)Field(game,"grid")).GetValue(x,y);}
 static void Replace(int x,int y,int id){game.GetType().GetMethod("ReplaceBasic",Flags).Invoke(game,new[]{Tile(x,y),(object)id});}
 static void Setup(){game.Build();game.InputEnabled=false;game.GetType().GetMethod("RemoveGuide",Flags).Invoke(game,null);errors.Clear();phase=0;first=second=exchange=duplicate=false;at=EditorApplication.timeSinceStartup+.6;
  if(index<3){Replace(7,4,index==0?11:index==1?13:15);Replace(8,7,13);}
  if(index==4){var nest=Tile(2,0);game.GetType().GetMethod("ReplaceWithChick",Flags).Invoke(game,new[]{Tile(5,0),Field(nest,"chickData"),Field(nest,"chickSkin")});}
  if(index==5){Replace(5,0,11);Replace(5,1,12);Replace(6,1,13);Replace(6,2,14);Replace(6,3,15);at+=3;}
 }
 static bool Visible(RectTransform root){if(!root||!root.gameObject.activeInHierarchy)return false;foreach(var graphic in root.GetComponentsInChildren<Graphic>()){if(!graphic.enabled||graphic.color.a<=.01f)continue;bool transparent=false;for(var t=graphic.transform;t!=null;t=t.parent){var alpha=t.GetComponent<CanvasGroup>();if(alpha&&alpha.alpha<=.01f){transparent=true;break;}}if(transparent)continue;var spine=graphic as SkeletonGraphic;if(spine!=null){if(spine.Skeleton.Slots.Any(s=>s.A>.01f&&(s.Attachment is Spine.RegionAttachment||s.Attachment is Spine.MeshAttachment)))return true;}else{var image=graphic as Image;if(image&&image.sprite)return true;}}return false;}
 static void Integrity(){var grid=(Array)Field(game,"grid");var pieces=((IEnumerable)Field(game,"pieces")).Cast<object>().ToList();var valid=(bool[,])Field(game,"playable");var locks=(int[,])Field(game,"cellLocks");for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid.GetValue(x,y);if(valid[x,y]&&p==null)errors.Add("hole:"+x+","+y);if(p!=null&&!pieces.Contains(p))errors.Add("orphan:"+x+","+y);if(locks[x,y]!=0)errors.Add("lock:"+x+","+y);}foreach(var p in pieces){int x=(int)Field(p,"x"),y=(int)Field(p,"y"),w=(int)Field(p,"w"),h=(int)Field(p,"h");if((bool)Field(p,"falling")||(bool)Field(p,"reserved")||(bool)Field(p,"dying")||(bool)Field(p,"switching"))errors.Add("state:"+x+","+y);for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)if(grid.GetValue(xx,yy)!=p)errors.Add("ownership:"+x+","+y);if(!Visible((RectTransform)Field(p,"root")))errors.Add("invisible:"+x+","+y+":"+Field(p,"id"));}}
 static void Finish(){Integrity();results.Add(new{name=Names[index],first,second,exchange,duplicate,game.MovesPlayed,errors=errors.ToArray()});index++;if(index>=Names.Length){System.IO.Directory.CreateDirectory(".codex/exact");System.IO.File.WriteAllText(".codex/exact/realtime-suite.json",Newtonsoft.Json.JsonConvert.SerializeObject(results));SessionState.SetString(Key,"");started=false;return;}Setup();}
 static void Tick(){if(SessionState.GetString(Key,"")!="running"||!Application.isPlaying||Time.frameCount<10||lastFrame==Time.frameCount)return;lastFrame=Time.frameCount;if(!started){game=UnityEngine.Object.FindObjectOfType<Reference20Game>();if(!game)return;started=true;Setup();return;}double now=EditorApplication.timeSinceStartup;if(now<at)return;
  if(index==5){if(phase==0){Integrity();for(int i=0;i<20;i++){Replace(6,2,3);Replace(6,2,14);}phase=1;at=now+3;return;}ScreenCapture.CaptureScreenshot(".codex/exact/bee-idle-fixed.png");Finish();return;}
  if(phase==0){first=index<3?game.TryActivate(7,4):index==3?game.TryMove(7,4,8,4):game.TryMove(5,0,5,1);if(!first)errors.Add("first-rejected");phase=1;at=now+.08;return;}
  if(phase==1){bool shouldAllow=index!=2;exchange=index==4?true:game.TryMove(5,0,5,1);if(index<3)second=game.TryActivate(8,7);duplicate=game.TryMove(5,0,5,1);if(index!=4&&exchange!=shouldAllow)errors.Add("parallel-swap");if(index<3&&second!=shouldAllow)errors.Add("parallel-power");if(duplicate)errors.Add("duplicate-accepted");phase=2;at=now;return;}
  if(now-at>90){errors.Add("timeout");Finish();return;}if(!game.IsBusy&&now-at>.5){int expected=index<2?2:index==2||index==3?1:0;if(game.MovesPlayed!=expected)errors.Add("move-count");Finish();}
 }
}
''',encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Runtime.cs');s=p.read_text(encoding='utf-8-sig');old='IEnumerator CompleteTurn(){if(MovesPlayed>=maxMoves||pieces.TrueForAll(p=>p.normal)){while(pendingCollections>0)yield return null;ShowEnd();}yield break;}'
new='IEnumerator CompleteTurn(){while(MovesPlayed>=maxMoves||pieces.TrueForAll(p=>p.normal)){if(pendingCollections==0&&activeOperations==0&&boardEvents.Count==0&&!boardMoving){ShowEnd();yield break;}yield return ResolveUntilStable();}}'
assert old in s;s=s.replace(old,new);p.write_text(s,encoding='utf-8')

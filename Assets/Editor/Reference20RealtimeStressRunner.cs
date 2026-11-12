using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Reference20;

[InitializeOnLoad]
public static class Reference20RealtimeStressRunner {
 const string Key="Reference20.RealtimeStress";
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static Reference20Game game;static bool started;static int accepted,parallel,attempts,lastFrame;static double begin,next;
 static readonly List<object> actions=new List<object>();
 static Reference20RealtimeStressRunner(){EditorApplication.update+=Tick;}
 public static void Begin(){SessionState.SetString(Key,"running");started=false;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
 static object Field(object p,string name){return p.GetType().GetField(name,Flags|BindingFlags.Public).GetValue(p);}
 static bool Operate(){bool busy=game.IsBusy;foreach(var p in (IEnumerable)Field(game,"pieces")){int id=(int)Field(p,"id");if(id<11||id>15)continue;int x=(int)Field(p,"x"),y=(int)Field(p,"y");if(game.TryActivate(x,y)){actions.Add(new{kind="tap",x,y,id,busy,time=EditorApplication.timeSinceStartup-begin});if(busy)parallel++;return true;}}Vector2Int a,b;if(game.TryGetValidMove(out a,out b)&&game.TryMove(a.x,a.y,b.x,b.y)){actions.Add(new{kind="swap",x=a.x,y=a.y,tx=b.x,ty=b.y,busy,time=EditorApplication.timeSinceStartup-begin});if(busy)parallel++;return true;}return false;}
 static void Tick(){if(SessionState.GetString(Key,"")!="running"||!Application.isPlaying||Time.frameCount<10||lastFrame==Time.frameCount)return;lastFrame=Time.frameCount;if(!started){game=UnityEngine.Object.FindObjectOfType<Reference20Game>();if(!game)return;game.Build();game.InputEnabled=false;actions.Clear();accepted=parallel=attempts=0;begin=EditorApplication.timeSinceStartup;next=begin+.5;started=true;return;}double now=EditorApplication.timeSinceStartup;if(now-begin>180){Finish("timeout");return;}if(game.IsFinished&&!game.IsBusy){Finish(null);return;}if(now<next||!game.CanAcceptInput)return;next=now+.12;attempts++;bool result=accepted==0?game.TryMove(7,4,8,4):Operate();if(result)accepted++;if(!game.IsBusy&&attempts>accepted+40){Finish("no-progress");}}
 static void Finish(string failure){var errors=new List<string>();var grid=(Array)Field(game,"grid");var pieces=((IEnumerable)Field(game,"pieces")).Cast<object>().ToList();var locks=(int[,])Field(game,"cellLocks");int occupied=0;for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid.GetValue(x,y);if(p!=null){occupied++;if(!pieces.Contains(p))errors.Add("orphan:"+x+","+y);}if(locks[x,y]!=0)errors.Add("lock:"+x+","+y);if((bool)game.GetType().GetMethod("CanFillVacancy",Flags).Invoke(game,new object[]{x,y}))errors.Add("reachable-hole:"+x+","+y);}foreach(var p in pieces){int x=(int)Field(p,"x"),y=(int)Field(p,"y"),w=(int)Field(p,"w"),h=(int)Field(p,"h");if((bool)Field(p,"reserved")||(bool)Field(p,"falling")||(bool)Field(p,"dying")||(bool)Field(p,"switching"))errors.Add("state:"+x+","+y);for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)if(grid.GetValue(xx,yy)!=p)errors.Add("occupancy:"+x+","+y);if(!Reference20RealtimeRunner.Visible((RectTransform)Field(p,"root")))errors.Add("invisible:"+x+","+y+":"+Field(p,"id"));}System.IO.File.WriteAllText(".codex/exact/realtime-stress.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{failure,errors,accepted,parallel,attempts,game.MovesPlayed,game.ClearedCount,occupied,seconds=EditorApplication.timeSinceStartup-begin,actions}));ScreenCapture.CaptureScreenshot(".codex/exact/realtime-stress.png");SessionState.SetString(Key,"");started=false;}
}

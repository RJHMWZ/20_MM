using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Reference20;

[InitializeOnLoad]
public static class Reference20ParityRunner {
 const string Key="Reference20.ParityMode";
 static Reference20Game game;static bool started;static double start,next;static int moves,frames,slowFrames,lastFrame;static float worstFrame;
 static readonly List<object> trace=new List<object>();
 static Reference20ParityRunner(){EditorApplication.update+=Tick;}
 public static void Begin(string mode){SessionState.SetString(Key,mode);started=false;game=null;trace.Clear();moves=0;start=EditorApplication.timeSinceStartup;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
 static void Tick(){string mode=SessionState.GetString(Key,"");if(string.IsNullOrEmpty(mode))return;
  if(!Application.isPlaying){if(started){SessionState.SetString(Key,"");started=false;}return;}
  if(Time.frameCount<10)return;
  if(!started){game=UnityEngine.Object.FindObjectOfType<Reference20Game>();if(!game)return;game.InputEnabled=false;game.Build();trace.Clear();game.TraceEvent=(kind,x,y,id,t)=>trace.Add(new{kind,x,y,id,time=t});started=true;start=EditorApplication.timeSinceStartup;next=start+.5;moves=frames=slowFrames=0;worstFrame=0;lastFrame=Time.frameCount;return;}
  if(moves>0&&lastFrame!=Time.frameCount){lastFrame=Time.frameCount;frames++;if(Time.unscaledDeltaTime>.0334f)slowFrames++;worstFrame=Mathf.Max(worstFrame,Time.unscaledDeltaTime);}
  double now=EditorApplication.timeSinceStartup;if(now-start>120){Finish(mode,false);return;}
  if(game.IsBusy||now<next)return;
  int limit=mode=="first"?1:9;if(moves>=limit||game.IsFinished){Finish(mode,true);return;}
  Vector2Int a,b;bool ok=moves==0?game.TryMove(7,4,8,4):game.TryGetValidMove(out a,out b)&&game.TryMove(a.x,a.y,b.x,b.y);
  if(ok){moves++;next=now+.5;}else Finish(mode,false);
 }
 static void Finish(string mode,bool settled){var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;var list=(System.Collections.IEnumerable)game.GetType().GetField("pieces",flags).GetValue(game);var pieces=new List<object>();foreach(var p in list){var type=p.GetType();pieces.Add(new{x=type.GetField("x").GetValue(p),y=type.GetField("y").GetValue(p),id=type.GetField("id").GetValue(p),hp=type.GetField("hp").GetValue(p)});}Directory.CreateDirectory(".codex/exact/timing");File.WriteAllText(".codex/exact/timing/parity-"+mode+".json",Newtonsoft.Json.JsonConvert.SerializeObject(new{settled,frames,slowFrames,worstFrame,game.MovesPlayed,game.ClearedCount,seconds=EditorApplication.timeSinceStartup-start,pieces,trace}));ScreenCapture.CaptureScreenshot(".codex/exact/timing/parity-"+mode+".png");game.TraceEvent=null;SessionState.SetString(Key,"");started=false;}
}

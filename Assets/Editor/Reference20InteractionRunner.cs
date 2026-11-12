using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Reference20;

[InitializeOnLoad]
public static class Reference20InteractionRunner {
 const string Key="Reference20.InteractionSuite";
 static readonly int[,] Cases={{3,3},{11,11},{11,12},{12,12},{11,13},{13,13},{14,14},{14,11},{14,12},{14,13},{15,3},{15,11},{15,12},{15,13},{15,14},{15,15}};
 static Reference20Game game;static bool started,operating;static int index,capture,lastFrame;static double begin,next;static int frames,slowFrames;static float worstFrame;
 static readonly List<object> events=new List<object>(),results=new List<object>();
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static readonly float[] Captures={.15f,.3f,.6f,1.2f};
 static Reference20InteractionRunner(){EditorApplication.update+=Tick;}
 public static void Begin(){SessionState.SetString(Key,"running");started=operating=false;results.Clear();index=0;EditorApplication.isPlaying=true;}
 static object Field(object instance,string name){return instance.GetType().GetField(name,Flags).GetValue(instance);}
 static void Tick(){if(SessionState.GetString(Key,"")!="running"||!Application.isPlaying||Time.frameCount<10||lastFrame==Time.frameCount)return;lastFrame=Time.frameCount;
  if(!started){game=UnityEngine.Object.FindObjectOfType<Reference20Game>();if(!game)return;game.InputEnabled=false;Setup();started=true;return;}
  var now=EditorApplication.timeSinceStartup;
  if(!operating){if(now<next)return;bool accepted=index==0?game.TryMove(5,0,5,1):game.TryMove(7,4,8,4);bool duplicate=index==0?game.TryMove(5,0,5,1):game.TryMove(7,4,8,4);if(!accepted||duplicate){FinishCase("input guard");return;}begin=now;operating=true;return;}
  frames++;if(Time.unscaledDeltaTime>.0334f)slowFrames++;worstFrame=Mathf.Max(worstFrame,Time.unscaledDeltaTime);
  float elapsed=(float)(now-begin);int a=Cases[index,0],b=Cases[index,1];
  if(capture<Captures.Length&&elapsed>=Captures[capture]){if(SessionState.GetBool("Reference20.CaptureFrames",false)&&((a==11&&b==13)||(a==14&&b==13)||(a==15&&(b==3||b==15))))ScreenCapture.CaptureScreenshot(".codex/exact/combos/unity-"+a+"-"+b+"-"+Mathf.RoundToInt(Captures[capture]*1000)+".png");capture++;}
  if(elapsed>90){FinishCase("timeout");return;}if(!game.IsBusy&&elapsed>.35f)FinishCase(null);
 }
 static void Setup(){game.Build();game.GetType().GetMethod("RemoveGuide",Flags).Invoke(game,null);events.Clear();game.TraceEvent=(kind,x,y,id,t)=>events.Add(new{kind,x,y,id,time=t});var grid=(Array)Field(game,"grid");var replace=game.GetType().GetMethod("ReplaceBasic",Flags);if(index>0){replace.Invoke(game,new[]{grid.GetValue(7,4),(object)Cases[index,0]});replace.Invoke(game,new[]{grid.GetValue(8,4),(object)Cases[index,1]});}capture=frames=slowFrames=0;worstFrame=0;operating=false;next=EditorApplication.timeSinceStartup+.6;}
 static void FinishCase(string failure){
  var grid=(Array)Field(game,"grid");var pieces=new List<object>();var errors=new List<string>();foreach(var piece in (IEnumerable)Field(game,"pieces")){var t=piece.GetType();int x=(int)t.GetField("x").GetValue(piece),y=(int)t.GetField("y").GetValue(piece),w=(int)t.GetField("w").GetValue(piece),h=(int)t.GetField("h").GetValue(piece),id=(int)t.GetField("id").GetValue(piece);bool dying=(bool)t.GetField("dying").GetValue(piece),falling=(bool)t.GetField("falling").GetValue(piece),reserved=(bool)t.GetField("reserved").GetValue(piece);if(dying||falling||reserved)errors.Add("unfinished:"+x+","+y);for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)if(xx<0||xx>=9||yy<0||yy>=12||grid.GetValue(xx,yy)!=piece)errors.Add("occupancy:"+x+","+y);var rt=(RectTransform)t.GetField("root").GetValue(piece);if(!Reference20RealtimeRunner.Visible(rt))errors.Add("invisible:"+x+","+y+":"+id);var pos=(Vector2)game.GetType().GetMethod("Position",Flags).Invoke(game,new[]{piece});if(id<100&&Vector2.Distance(rt.anchoredPosition,pos)>.05f)errors.Add("position:"+x+","+y);pieces.Add(new{x,y,id,hp=t.GetField("hp").GetValue(piece)});}
  for(int y=0;y<12;y++)for(int x=0;x<9;x++)if((bool)game.GetType().GetMethod("CanFillVacancy",Flags).Invoke(game,new object[]{x,y}))errors.Add("reachable-hole:"+x+","+y);
  int a=Cases[index,0],b=Cases[index,1];if(game.MovesPlayed!=(index==0?0:1))errors.Add("move-count");var data=new{a,b,failure,errors,game.MovesPlayed,game.ClearedCount,seconds=EditorApplication.timeSinceStartup-begin,frames,slowFrames,worstFrame,pieces,events};Directory.CreateDirectory(".codex/exact/combos");File.WriteAllText(".codex/exact/combos/unity-"+a+"-"+b+".json",JsonConvert.SerializeObject(data));results.Add(new{a,b,failure,errors});game.TraceEvent=null;index++;if(index>=Cases.GetLength(0)){File.WriteAllText(".codex/exact/combos/unity-suite.json",JsonConvert.SerializeObject(results));SessionState.SetString(Key,"");started=false;return;}Setup();
 }
}

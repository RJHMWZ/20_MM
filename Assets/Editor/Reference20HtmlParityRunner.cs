using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Reference20;

// File commands let the running editor validate the same scene used by the playable.
[InitializeOnLoad]
public static class Reference20HtmlParityRunner {
 const string Command=".codex/html-parity/editor-command.json";
 [Serializable] sealed class Request {public string action;public string path;public int id=11;}
 static double next;static Reference20Game game;
 static readonly int[] Cases={11,12,13,14,15,1111,1113,1411,1413,1414,1511,1513,1514,1515,1313};static int test=-1,step,visibleBolts;static double due;static float began;static readonly System.Collections.Generic.List<object> results=new System.Collections.Generic.List<object>();
 public static void BeginSuite(){SessionState.SetBool("Reference20.HtmlSuite",true);test=0;step=0;results.Clear();due=EditorApplication.timeSinceStartup+2;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
 static Reference20HtmlParityRunner(){EditorApplication.update+=Tick;}
 static void Tick(){
  if(test<0&&SessionState.GetBool("Reference20.HtmlSuite",false)&&Application.isPlaying){test=0;step=0;results.Clear();due=EditorApplication.timeSinceStartup+2;}
  if(test>=0&&Application.isPlaying&&!EditorApplication.isCompiling)SuiteTick();
  if(EditorApplication.timeSinceStartup<next||EditorApplication.isCompiling||!File.Exists(Command))return;
  next=EditorApplication.timeSinceStartup+.2;
  try {
   var r=JsonUtility.FromJson<Request>(File.ReadAllText(Command));File.Delete(Command);
   if(r.action=="play"){EditorApplication.isPlaying=true;return;}
   if(r.action=="stop"){EditorApplication.isPlaying=false;return;}
   if(r.action=="refresh"){AssetDatabase.Refresh();return;}
   game=UnityEngine.Object.FindObjectOfType<Reference20Game>();
   if(!game)throw new Exception("Reference20Game missing");
   if(r.action=="build"){game.Build();return;}
   if(r.action=="end"){game.ShowEnd();return;}
   if(r.action=="special"){
    game.Build();game.InputEnabled=false;
    var flags=BindingFlags.Instance|BindingFlags.NonPublic;
    typeof(Reference20Game).GetMethod("RemoveGuide",flags).Invoke(game,null);
    var tile=((Array)typeof(Reference20Game).GetField("grid",flags).GetValue(game)).GetValue(7,4);
    typeof(Reference20Game).GetMethod("ReplaceBasic",flags).Invoke(game,new[]{tile,(object)r.id});return;
   }
   if(r.action=="activate"){game.TryActivate(7,4);return;}
   if(r.action=="capture"){ScreenCapture.CaptureScreenshot(r.path);return;}
   if(r.action=="save"){if(!Application.isPlaying)UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();AssetDatabase.SaveAssets();return;}
   if(r.action=="status")File.WriteAllText(".codex/html-parity/editor-status.json",JsonUtility.ToJson(new Status {playing=Application.isPlaying,finished=game.IsFinished,moves=game.MovesPlayed,width=Screen.width,height=Screen.height}));
  }catch(Exception e){File.WriteAllText(".codex/html-parity/editor-error.txt",e.ToString());Debug.LogException(e);}
 }
 static void SuiteTick(){
  if(EditorApplication.timeSinceStartup<due)return;
  try{
   game=UnityEngine.Object.FindObjectOfType<Reference20Game>();if(!game)return;
   int id=Cases[test];var flags=BindingFlags.Instance|BindingFlags.NonPublic;
   if(step==0){game.Build();game.InputEnabled=false;typeof(Reference20Game).GetMethod("RemoveGuide",flags).Invoke(game,null);var grid=(Array)typeof(Reference20Game).GetField("grid",flags).GetValue(game);var tile=grid.GetValue(7,4);typeof(Reference20Game).GetMethod("ReplaceBasic",flags).Invoke(game,new[]{tile,(object)(id>100?id/100:id)});if(id>100)typeof(Reference20Game).GetMethod("ReplaceBasic",flags).Invoke(game,new[]{grid.GetValue(8,4),(object)(id%100)});step=1;due=EditorApplication.timeSinceStartup+1;return;}
   if(step==1){ScreenCapture.CaptureScreenshot(".codex/html-parity/unity-idle-"+id+".png");step=2;due=EditorApplication.timeSinceStartup+.1;return;}
   if(step==2){bool ok=id>100?game.TryMove(7,4,8,4):game.TryActivate(7,4);began=Time.time;visibleBolts=0;results.Add(new{id,activated=ok});step=3;due=EditorApplication.timeSinceStartup+.12;return;}
   if(step>=3&&step<=5){int visible=0;foreach(var bolt in UnityEngine.Object.FindObjectsOfType<Reference20ColorTrail>()){var renderer=bolt.GetComponent<CanvasRenderer>();if(renderer&&renderer.GetMesh()&&renderer.GetMesh().vertexCount>3)visible++;}visibleBolts=Mathf.Max(visibleBolts,visible);ScreenCapture.CaptureScreenshot(".codex/html-parity/unity-fx-"+id+"-"+(step-3)+".png");step++;due=EditorApplication.timeSinceStartup+.22;return;}
   if(game.IsBusy&&Time.time-began<22){due=EditorApplication.timeSinceStartup+.25;return;}
   results.Add(new{id,settled=!game.IsBusy,moves=game.MovesPlayed,seconds=Time.time-began,activeParticles=Reference20CompanyParticles.Active.Count,visibleLightningLayers=visibleBolts});test++;step=0;due=EditorApplication.timeSinceStartup+.3;
   if(test>=Cases.Length){File.WriteAllText(".codex/html-parity/gameplay-suite.json",Newtonsoft.Json.JsonConvert.SerializeObject(results));test=-1;SessionState.SetBool("Reference20.HtmlSuite",false);game.Build();game.InputEnabled=true;}
  }catch(Exception e){File.WriteAllText(".codex/html-parity/suite-error.txt",e.ToString());test=-1;SessionState.SetBool("Reference20.HtmlSuite",false);Debug.LogException(e);}
 }
 [Serializable] sealed class Status {public bool playing,finished;public int moves,width,height;}
}

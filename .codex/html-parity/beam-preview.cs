var g=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();g.Build();g.InputEnabled=false;
typeof(Reference20.Reference20Game).GetMethod("RemoveGuide",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(g,null);
var overlay=Reference20.Reference20Native.Overlay(g.stage);
var bg=Reference20.Reference20Props.Rect("BeamPreviewBackground",overlay);bg.sizeDelta=new UnityEngine.Vector2(1080,1920);bg.anchorMin=bg.anchorMax=UnityEngine.Vector2.zero;bg.anchoredPosition=new UnityEngine.Vector2(540,960);bg.gameObject.AddComponent<UnityEngine.UI.Image>().color=UnityEngine.Color.black;bg.SetAsFirstSibling();
UnityEngine.Time.timeScale=0;
Reference20.Reference20ColorTrail.Play(g.stage,new UnityEngine.Vector2(200,800),new UnityEngine.Vector2(900,1400));
foreach(var c in UnityEngine.Object.FindObjectsOfType<Reference20.Reference20ColorTrail>())c.GetType().GetField("elapsed",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(c,.32f);
UnityEngine.Canvas.ForceUpdateCanvases();UnityEngine.ScreenCapture.CaptureScreenshot(".codex/html-parity/beam-debug.png");
return "beam frozen at .32";

var game=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();game.Build();game.InputEnabled=false;game.stage.gameObject.SetActive(false);
var canvas=game.stage.parent as UnityEngine.RectTransform;
var root=Reference20.Reference20Props.Rect("HtmlPropsPreview",canvas);root.anchorMin=root.anchorMax=UnityEngine.Vector2.one*.5f;root.sizeDelta=new UnityEngine.Vector2(1080,1920);
var im=root.gameObject.AddComponent<UnityEngine.UI.Image>();im.color=UnityEngine.Color.black;
var ids=new[]{11,12,13,14,15};for(int i=0;i<ids.Length;i++){var rt=Reference20.Reference20Props.Rect("Case"+ids[i],root);rt.anchorMin=rt.anchorMax=UnityEngine.Vector2.zero;rt.anchoredPosition=new UnityEngine.Vector2(170+i*180,1200);var prop=Reference20.Reference20Props.Visual(ids[i],rt,false);prop.localScale*=.8875f;}
return "preview ready";

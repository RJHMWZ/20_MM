var a=typeof(UnityEditor.Editor).Assembly;var flags=System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Static;
var gt=a.GetType("UnityEditor.GameView");var view=UnityEditor.EditorWindow.GetWindow(gt);
var st=a.GetType("UnityEditor.GameViewSizes");var sizes=st.BaseType.GetProperty("instance",flags).GetValue(null,null);var group=st.GetProperty("currentGroup",flags).GetValue(sizes,null);
var labels=(string[])group.GetType().GetMethod("GetDisplayTexts",flags).Invoke(group,null);var selected=gt.GetProperty("selectedSizeIndex",flags);int original=(int)selected.GetValue(view,null);
for(int i=0;i<labels.Length;i++)if(labels[i].Replace(" ","").Contains("1080x1920")){UnityEditor.SessionState.SetInt("Reference20.OriginalGameViewSize",original);selected.SetValue(view,i,null);return new {selected=labels[i],original};}
return labels;

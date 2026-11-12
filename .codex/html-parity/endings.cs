var g=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();g.Build();g.InputEnabled=false;
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
typeof(Reference20.Reference20Game).GetMethod("RemoveGuide",flags).Invoke(g,null);
var grid=(System.Array)typeof(Reference20.Reference20Game).GetField("grid",flags).GetValue(g);var tile=grid.GetValue(7,4);
typeof(Reference20.Reference20Game).GetMethod("ReplaceBasic",flags).Invoke(g,new object[]{tile,13});
typeof(Reference20.Reference20Game).GetField("<MovesPlayed>k__BackingField",flags).SetValue(g,24);
return new {activated=g.TryActivate(7,4),moves=g.MovesPlayed};

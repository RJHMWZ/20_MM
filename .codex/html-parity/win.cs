var g=UnityEngine.Object.FindObjectOfType<Reference20.Reference20Game>();g.Build();g.InputEnabled=false;
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
typeof(Reference20.Reference20Game).GetMethod("RemoveGuide",flags).Invoke(g,null);
var pieces=(System.Collections.IList)typeof(Reference20.Reference20Game).GetField("pieces",flags).GetValue(g);
for(int i=pieces.Count-1;i>=0;i--){var p=pieces[i];if((int)p.GetType().GetField("id").GetValue(p)==Reference20.Reference20Game.BearId){typeof(Reference20.Reference20Game).GetMethod("Vacate",flags).Invoke(g,new[]{p});var rt=(UnityEngine.RectTransform)p.GetType().GetField("root").GetValue(p);UnityEngine.Object.Destroy(rt.gameObject);pieces.RemoveAt(i);}}
typeof(Reference20.Reference20Game).GetMethod("EnsureResolution",flags).Invoke(g,null);
return "last target collected; waiting for settlement";

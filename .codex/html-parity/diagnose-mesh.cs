var result=new System.Collections.Generic.List<object>();
foreach(var g in UnityEngine.Object.FindObjectsOfType<UnityEngine.UI.Graphic>()){
 var mesh=g.canvasRenderer.GetMesh();if(mesh==null)continue;var b=mesh.bounds;
 bool invalid=float.IsNaN(b.center.x)||float.IsNaN(b.center.y)||float.IsInfinity(b.size.x)||float.IsNaN(b.size.x)||float.IsNaN(b.size.y);
 if(invalid||mesh.vertexCount==0)result.Add(new {name=g.name,type=g.GetType().Name,vertices=mesh.vertexCount,bounds=b.ToString(),parent=g.transform.parent?g.transform.parent.name:""});
}
return result;

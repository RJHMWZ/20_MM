from pathlib import Path
import json
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page()
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 data=page.evaluate('''async()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);g.enabled=false;const d=g.elementDatabase.specialElements._items.find(s=>s.behaviorType===3);const fx=Match3.Match3EffectPlayer.Play(d.trailEffectPrefab,pc.Vec3.ZERO.clone(),null,1,0,null,true);window.__fx=fx;window.__rs=fx.GetComponentsInChildren$1(UnityEngine.TrailRenderer,true);await new Promise(r=>setTimeout(r,80));const info=__rs.map(r=>{const n=r.Eib$,model=n._model;return {name:r.name,keys:Object.keys(n),model:!!model,meshKeys:model?.meshInstances?.map(m=>({keys:Object.keys(m),mesh:m.mesh?Object.keys(m.mesh):null,material:m.material?{name:m.material.name,shader:m.material._shader?.name,params:Object.fromEntries(Object.entries(m.material.parameters).map(([k,v])=>[k,k==='_MainTex'?{keys:Object.keys(v),name:v.data?.name,id:v.data?.$id}:v.data]))}:null}))};});return info;}''')
 Path('.codex/html-parity/trail-mesh-api.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
 data=page.evaluate('''async()=>{const layers=__rs.map(r=>{let m=r.Eib$._model.meshInstances[0].material;return {name:r.name,texture:m.parameters._MainTex.data.$id,tint:Array.from((m.parameters._TintColor?.data||m.parameters._Color?.data||[1,1,1,1])),tiling:Array.from(m.parameters._MainTex_ST.data),frames:[]};});const start=UnityEngine.Time.time;let times=[];for(let f=0;f<65;f++){await new Promise(requestAnimationFrame);let t=UnityEngine.Time.time-start;__fx.transform.position=new pc.Vec3(Math.min(1,t/.5)*4,0,0);times.push(t);for(let i=0;i<__rs.length;i++){const mesh=__rs[i].Eib$._model.meshInstances[0].mesh,vb=mesh.vertexBuffer,fmt=vb.format,positions=fmt.getStructureFor('POSITION'),uvs=fmt.getStructureFor('TEXCOORD0'),colors=fmt.getStructureFor('COLOR');if(!positions||!uvs){layers[i].frames.push({time:t,vertices:[],indices:[]});continue;}const view=new DataView(vb.storage),vertices=[];for(let j=0;j<vb.numVertices;j++){const pos=positions.offset+j*positions.stride,uv=uvs.offset+j*uvs.stride;let c=[1,1,1,1];if(colors){let k=colors.offset+j*colors.stride;for(let a=0;a<4;a++)c[a]=colors.dataType===1?view.getUint8(k+a)/255:view.getFloat32(k+a*4,true);}vertices.push([view.getFloat32(pos,true)/4,view.getFloat32(pos+4,true)*113.6,view.getFloat32(uv,true),view.getFloat32(uv+4,true),...c]);}layers[i].frames.push({time:t,vertices,indices:Array.from(mesh.getIndicies(0))});}if(t>1)break;}return {duration:.5,layers,times};}''')
 Path('Assets/Resources/Reference20/Company/Particles/ColorTrail.json').write_text(json.dumps(data,separators=(',',':')),encoding='utf8')
 print([(l['name'],len(l['frames']),max(len(f['vertices']) for f in l['frames'])) for l in data['layers']]);b.close()
 from PIL import Image
 import re,base64,io
 html=src.read_text('utf8')
 for l in data['layers']:
  m=re.search(r'<img id="assets/bundles/-1/'+str(l['texture'])+r'\.[^"]+" src="data:image/[^;]+;base64,([^"]+)"',html)
  Image.open(io.BytesIO(base64.b64decode(m[1]))).save(Path('Assets/Resources/Reference20/Company/Particles')/(str(l['texture'])+'.png'))

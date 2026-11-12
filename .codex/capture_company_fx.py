"""Bake the company's actual Unity/Playworks effects into portable alpha flipbooks.

The source particle systems are simulated at fixed times. Black/white renders of
the same frozen frame recover alpha, including the original translucent glows.
"""
from pathlib import Path
import json,io,sys
import numpy as np
from PIL import Image
from playwright.sync_api import sync_playwright

out=Path('Assets/Resources/Reference20/Company/FX');out.mkdir(exist_ok=True)
proof=Path('.codex/company/fx-frames');proof.mkdir(exist_ok=True)
configs=[
 ('NormalBlue',4,32,2),('NormalGold',4,32,2),('Wave',4,24,1),
 ('Bomb',8,40,1),('BeeImpact',5,24,1),('ColorFinish',10,40,1),
 ('ColorCharge',8,32,1),('ColorCombo',10,40,1),('RocketTrail',5,32,1),('ColorTrail',4,32,1)
]
if len(sys.argv)>1:configs=[x for x in configs if x[0] in sys.argv[1:]]
manifest=[]
with sync_playwright() as p:
 browser=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=browser.new_page(viewport={'width':256,'height':256},device_scale_factor=1)
 page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri(),wait_until='domcontentloaded');page.wait_for_timeout(6500)
 page.evaluate('''()=>{window.__g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);__g.enabled=false;for(const r of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Renderer))r.enabled=false;for(const c of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Canvas))c.enabled=false;window.__fx=[];UnityEngine.Time.timeScale=0;window.__setBg=v=>{let c=UnityEngine.Camera.main.backgroundColor;c.r=c.g=c.b=v;c.a=1;UnityEngine.Camera.main.backgroundColor=c;};window.__spawn=(name,width,scale)=>{for(const go of __fx)UnityEngine.Object.DestroyImmediate(go);__fx=[];const db=__g.elementDatabase;const specials=db.specialElements._items.slice(0,db.specialElements.Count);const spec=b=>specials.find(x=>x.behaviorType===b);let prefabs=[];if(name==='NormalBlue'||name==='NormalGold'){const n=db.normalElements._items[name==='NormalBlue'?2:3];prefabs=[[n.clearEffectPrefab,scale],[n.lightWaveEffectPrefab,1]];}else if(name==='Wave')prefabs=[[db.normalElements._items[2].lightWaveEffectPrefab,1]];else if(name==='Bomb')prefabs=[[spec(1).activationEffectPrefab,scale]];else if(name==='BeeImpact')prefabs=[[spec(2).activationEffectPrefab,scale]];else if(name==='ColorFinish')prefabs=[[spec(3).finishEffectPrefab,scale]];else if(name==='ColorCharge')prefabs=[[spec(3).activationEffectPrefab,scale]];else if(name==='ColorCombo'){const view=spec(3).prefab.GetComponent$1(Match3.SpecialElementView);prefabs=[[view.swapParticlePrefab||spec(3).finishEffectPrefab,scale]];}else if(name==='RocketTrail')prefabs=[[spec(0).trailEffectPrefab,scale]];else if(name==='ColorTrail')prefabs=[[spec(3).trailEffectPrefab,scale]];UnityEngine.Camera.main.orthographicSize=width/2;for(const [prefab,s] of prefabs)if(prefab)__fx.push(Match3.Match3EffectPlayer.Play(prefab,pc.Vec3.ZERO.clone(),null,s,0,null,true));window.__systems=__fx.flatMap(x=>x.GetComponentsInChildren$1(UnityEngine.ParticleSystem,true));window.__fxName=name;window.__startTime=UnityEngine.Time.time;return {systems:__systems.length,prefabs:prefabs.map(x=>x[0]?x[0].name:null)};};window.__step=dt=>{for(const ps of __systems)ps.Simulate(dt,false,false,false);};}''')
 for name,width,count,scale in configs:
  info=page.evaluate('([n,w,s])=>__spawn(n,w,s)',[name,width,scale])
  atlas=Image.new('RGBA',(256*8,256*((count+7)//8)))
  max_alpha=0;times=[]
  for i in range(count):
   page.evaluate('i=>{if(__fxName==="ColorTrail")for(const fx of __fx)fx.transform.position=new pc.Vec3(-1.4+Math.min(1,i/4)*2.8,0,0);UnityEngine.Time.timeScale=1;}',i)
   page.wait_for_timeout(33)
   t=page.evaluate('()=>{UnityEngine.Time.timeScale=0;__setBg(0);return UnityEngine.Time.time-__startTime;}')
   times.append(t)
   page.wait_for_timeout(16)
   black=np.asarray(Image.open(io.BytesIO(page.screenshot())).convert('RGB')).astype(np.float32)/255
   page.evaluate('()=>__setBg(1)');page.wait_for_timeout(16)
   white=np.asarray(Image.open(io.BytesIO(page.screenshot())).convert('RGB')).astype(np.float32)/255
   alpha=np.clip(1-np.min(white-black,axis=2),0,1)
   rgb=np.clip(black/np.maximum(alpha[:,:,None],1/255),0,1)
   rgba=np.dstack((rgb,alpha));frame=Image.fromarray(np.uint8(np.round(rgba*255)),'RGBA')
   atlas.alpha_composite(frame,((i%8)*256,(i//8)*256));max_alpha=max(max_alpha,int(frame.getchannel('A').getextrema()[1]))
   if i in [2,7,15]:frame.save(proof/(name+'-'+str(i)+'.png'))
  atlas.save(out/(name+'.png'),optimize=True)
  manifest.append({'name':name,'columns':8,'rows':(count+7)//8,'frames':count,'fps':30,'cells':width,'times':times,'sourcePrefabs':info['prefabs']})
  print(name,info,'alpha',max_alpha,'bytes',(out/(name+'.png')).stat().st_size,flush=True)
 browser.close()
path=out/'manifest.json'
if path.exists():
 old=json.loads(path.read_text(encoding='utf-8'))['effects'];names={x['name'] for x in manifest};manifest=[x for x in old if x['name'] not in names]+manifest
path.write_text(json.dumps({'effects':manifest},indent=2),encoding='utf-8')

import json
from pathlib import Path
from playwright.sync_api import sync_playwright

out=Path('.codex/company')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':540,'height':960})
 page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri(),wait_until='domcontentloaded');page.wait_for_timeout(2300)
 info=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);window.__g=g;const array=v=>v.ToArray?v.ToArray():v._items?v._items.slice(0,v.Count):Array.from(v);const ref=v=>v?{name:v.name,id:v.GetInstanceID()}:null;const d=g.elementDatabase;return {normal:array(d.normalElements).map(n=>({id:n.id,sprite:ref(n.sprite),clear:ref(n.clearEffectPrefab),scale:n.clearEffectScale,wave:ref(n.lightWaveEffectPrefab),waveScale:n.lightWaveEffectScale})),globalWave:ref(d.globalLightWaveEffectPrefab),special:array(d.specialElements).map(n=>({id:n.elementId,behavior:n.behaviorType,prefab:ref(n.prefab),activation:ref(n.activationEffectPrefab),trail:ref(n.trailEffectPrefab),finish:ref(n.finishEffectPrefab)})),play:Match3.Match3EffectPlayer.Play.toString().slice(0,800),camera:{size:UnityEngine.Camera.main.orthographicSize,pos:UnityEngine.Camera.main.transform.position}};}''')
 debug=page.evaluate('''()=>{const d=__g.elementDatabase;return {keys:Object.keys(d),normalKeys:Object.keys(d.normalElements||{}),normalProto:d.normalElements?Object.getOwnPropertyNames(Object.getPrototypeOf(d.normalElements)):[],normal:String(d.normalElements),dbName:d.name};}''')
 (out/'fx-debug.json').write_text(json.dumps(debug,ensure_ascii=False,indent=2),encoding='utf-8')
 (out/'fx-map.json').write_text(json.dumps(info,ensure_ascii=False,indent=2),encoding='utf-8')
 print(json.dumps(info,ensure_ascii=False))
 b.close()

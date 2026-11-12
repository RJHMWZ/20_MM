from pathlib import Path
import json
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page()
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 x=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager),d=g.elementDatabase.specialElements._items.find(s=>s.behaviorType===3);const fx=Match3.Match3EffectPlayer.Play(d.trailEffectPrefab,pc.Vec3.ZERO.clone(),null,1,0,null,true);const describe=o=>Object.fromEntries(Object.entries(o).map(([k,v])=>[k,v==null?v:typeof v==='object'?{type:v.constructor?.name,keys:Object.keys(v).slice(0,30)}:typeof v==='function'?'function':v]));return {static:Object.fromEntries(Object.getOwnPropertyNames(Match3.BoardManager).filter(k=>/Rocket|Trail/.test(k)).map(k=>[k,String(Match3.BoardManager[k])])),trail:fx.GetComponentsInChildren$1(UnityEngine.Renderer,true).map(r=>({name:r.name,type:r.constructor?.name,info:describe(r),native:describe(r.Eib$)})),viewMethods:Object.fromEntries(['Initialize','PlayCreationAnimation'].map(k=>[k,String(Match3.SpecialElementView.prototype[k])]))};}''')
 Path('.codex/html-parity/trails.json').write_text(json.dumps(x,ensure_ascii=False,indent=2),encoding='utf8');b.close()

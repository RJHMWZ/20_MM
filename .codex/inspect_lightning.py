from pathlib import Path
import json
from playwright.sync_api import sync_playwright
src=Path('C:/Users/yangjinsheng/Desktop/\u6a0a\u601d\u543e.html')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page(viewport={'width':671,'height':1169})
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 data=page.evaluate('''()=>{window.__g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);const defs=__g.elementDatabase.specialElements._items;const d=defs.find(s=>s.behaviorType===3);const scalar=o=>Object.fromEntries(Object.entries(o).filter(([k,v])=>!k.includes('$')).map(([k,v])=>[k,v&&typeof v==='object'?{type:v.constructor?.$$fullname||v.constructor?.name,name:v.name}:typeof v==='function'?String(v):v]));function walk(t){let comps=t.GetComponentsInChildren$1(UnityEngine.MonoBehaviour,true);let o={name:t.name,position:t.localPosition,components:comps.map(c=>({type:c.constructor?.$$fullname,fields:scalar(c)})),children:[]};for(let i=0;i<t.childCount;i++)o.children.push(walk(t.GetChild(i)));return o;}return {trail:walk(d.trailEffectPrefab.transform),activation:walk(d.activationEffectPrefab.transform),board:__g.boardManager.cellSize};}''')
 Path('.codex/html-parity/lightning-prefabs.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
 page.evaluate('''()=>{const board=__g.boardManager;const d=__g.elementDatabase.specialElements._items.find(s=>s.behaviorType===3);const x=7,y=11;const old=board.activeElements.get([x,y]);if(old)UnityEngine.Object.DestroyImmediate(old.gameObject);const go=UnityEngine.Object.Instantiate$3(UnityEngine.GameObject,d.prefab,board.GetWorldPosition(x,y),pc.Quat.IDENTITY.clone(),board.elementsContainer);const v=go.GetComponent$1(Match3.SpecialElementView);v.Initialize(d,x,y,board.GetWorldPosition(x,y),board.cellSize);board.activeElements.set([x,y],v);window.__power=v;board.StartCoroutine$1(board.TriggerSpecialOnClick(v));}''')
 for i in range(6):page.wait_for_timeout(100);page.screenshot(path=f'.codex/html-parity/html-lightning-{i}.png')
 b.close()

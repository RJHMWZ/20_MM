from pathlib import Path
import json
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page(viewport={"width":804,"height":1280})
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 data=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager),n=UnityEngine.Object.FindObjectOfType$1(Match3.NormalElementView);return {normal:Object.fromEntries(Object.entries(n).filter(([k,v])=>!k.includes('$')).map(([k,v])=>[k,v&&typeof v==='object'?{name:v.name,keys:Object.keys(v),json:v.skeletonJSON?.name,scale:v.scale}:v])),elements:g.elementDatabase.normalElements._items.slice(0,g.elementDatabase.normalElements.Count).map(e=>({id:e.id,skin:e.spineSkinName})),cell:g.boardManager.cellSize,rootScale:n.transform.localScale};}''')
 Path('.codex/html-parity/normals.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8');print(data);b.close()

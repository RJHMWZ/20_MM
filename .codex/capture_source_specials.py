from pathlib import Path
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page(viewport={'width':1080,'height':1920})
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);g.enabled=false;for(const r of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Renderer))r.enabled=false;for(const c of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Canvas))c.enabled=false;UnityEngine.Camera.main.orthographicSize=1920/(113.6*2);UnityEngine.Camera.main.backgroundColor=new pc.Color(0,0,0,1);const defs=g.elementDatabase.specialElements._items;window.__spec=[];for(let i=0;i<5;i++){let behavior=[0,4,1,2,3][i],d=defs.find(d=>d.behaviorType===behavior),go=UnityEngine.Object.Instantiate$2(UnityEngine.GameObject,d.prefab,new pc.Vec3((170+i*180-540)/113.6,(1200-960)/113.6,0),pc.Quat.IDENTITY.clone());let v=go.GetComponent$1(Match3.SpecialElementView);v.Initialize(d,i,0,go.transform.position,1);__spec.push(v);}}''')
 page.wait_for_timeout(1000);page.screenshot(path='.codex/html-parity/source-props.png')
 b.close()

from pathlib import Path
import json, re
from playwright.sync_api import sync_playwright

out=Path('.codex/html-parity');out.mkdir(exist_ok=True)
source=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':804,'height':1280})
 errors=[];page.on('pageerror',lambda e:errors.append(str(e)))
 page.goto(source.as_uri());page.wait_for_timeout(5000)
 data=page.evaluate('''()=>{const g=window.__g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);const methods=t=>Object.fromEntries(Object.getOwnPropertyNames(t.prototype).filter(k=>typeof t.prototype[k]==='function'&&!k.includes('$')).map(k=>[k,String(t.prototype[k])]));const scalar=o=>Object.fromEntries(Object.entries(o).filter(([k,v])=>['number','string','boolean'].includes(typeof v)));return {game:methods(Match3.GameManager),ui:methods(Match3.UI.GameUIManager),special:methods(Match3.SpecialElementView),config:scalar(UnityEngine.Object.FindObjectOfType$1(Manager.ConfigManager)),state:scalar(g),board:methods(Match3.BoardManager)};}''')
 (out/'methods.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
 page.screenshot(path=str(out/'start.png'))
 data=page.evaluate('''()=>{const db=__g.elementDatabase;const list=v=>v._items.slice(0,v.Count);const vec=v=>v?{x:v.x,y:v.y,z:v.z,w:v.w}:null;function walk(t){let s=t.GetComponent$1(Spine.Unity.SkeletonAnimation),im=t.GetComponent$1(UnityEngine.SpriteRenderer);let o={name:t.name,position:vec(t.localPosition),scale:vec(t.localScale),rotation:vec(t.localEulerAngles),active:t.gameObject.activeSelf,children:[]};if(s)o.spine={skin:s.initialSkinName,animation:s.AnimationName,loop:s.loop,timeScale:s.timeScale,keys:Object.keys(s),data:s.skeletonDataAsset?.name};if(im)o.sprite={name:im.sprite?.name,color:{r:im.color.r,g:im.color.g,b:im.color.b,a:im.color.a}};for(let i=0;i<t.childCount;i++)o.children.push(walk(t.GetChild(i)));return o;}return list(db.specialElements).map(x=>({behavior:x.behaviorType,fields:Object.fromEntries(Object.entries(x).filter(([k,v])=>!k.includes('$')&&(['number','boolean','string'].includes(typeof v)||v===null)).map(([k,v])=>[k,v])),tree:walk(x.prefab.transform)}));}''')
 (out/'specials.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
 page.evaluate('()=>{__g.isGameOver=true;__g.uiManager.ShowDownloadPanel();}')
 page.wait_for_timeout(1000);page.screenshot(path=str(out/'end.png'))
 (out/'errors.json').write_text(json.dumps(errors),encoding='utf8')
 print('Inspected source methods, specials and ending',flush=True)
 b.close()

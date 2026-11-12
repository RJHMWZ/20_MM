from pathlib import Path
import json
from playwright.sync_api import sync_playwright

out=Path('.codex/company');out.mkdir(exist_ok=True)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':540,'height':960})
 errors=[]
 page.add_init_script('''window.__captured={};for(const name of ['jsons','blobs','sounds','dataUris','images']){const saved={};window.__captured[name]=saved;let target={};const proxy=new Proxy(target,{set(o,k,v){saved[k]=v instanceof ArrayBuffer?v.slice(0):v;o[k]=v;return true;}});Object.defineProperty(window,name,{configurable:true,get(){return proxy},set(v){if(v!==proxy&&v)for(const k of Object.keys(v))proxy[k]=v[k];}});}''')
 page.on('pageerror',lambda e:errors.append(str(e)))
 page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri(),wait_until='domcontentloaded')
 for delay in [100,400,700,1500,3000]:
  page.wait_for_timeout(delay);page.screenshot(path=str(out/f'view-{delay}.png'))
 info=page.evaluate('''()=>({globals:Object.keys(window).filter(k=>/json|blob|audio|asset|match|loading|manager/i.test(k)),jsons:Object.entries(window.jsons||{}).map(([k,v])=>({key:k,type:typeof v,size:JSON.stringify(v).length,preview:JSON.stringify(v).slice(0,130)})),blobs:Object.entries(window.blobs||{}).map(([k,v])=>({key:k,type:typeof v,size:v.byteLength||v.length||v.size||0})),app:!!window.app})''')
 (out/'inventory.json').write_text(json.dumps(info,ensure_ascii=False,indent=2),encoding='utf-8')
 payload=page.evaluate('()=>JSON.stringify(window.__captured.jsons||{})')
 (out/'jsons.json').write_text(payload,encoding='utf-8')
 captures=page.evaluate('''()=>Object.fromEntries(Object.entries(window.__captured).map(([name,items])=>[name,Object.entries(items).map(([k,v])=>({key:k,type:typeof v,size:v.byteLength||v.length||v.size||0}))]))''')
 (out/'captures.json').write_text(json.dumps(captures,indent=2),encoding='utf-8')
 for name in ['sounds','blobs']:
  keys=page.evaluate('(n)=>Object.keys(window.__captured[n])',name)
  for key in keys:
   data=page.evaluate('''([n,k])=>{let v=window.__captured[n][k];if(v instanceof ArrayBuffer)v=new Uint8Array(v);else if(ArrayBuffer.isView(v))v=new Uint8Array(v.buffer,v.byteOffset,v.byteLength);else return null;let s='';for(let i=0;i<v.length;i+=8192)s+=String.fromCharCode.apply(null,v.subarray(i,i+8192));return btoa(s);}''',[name,key])
   if data:
    import base64
    path=out/name/Path(key).name;path.parent.mkdir(exist_ok=True);path.write_bytes(base64.b64decode(data))
 (out/'errors.json').write_text(json.dumps(errors),encoding='utf-8')
 runtime=page.evaluate('''()=>({gameType:Object.getOwnPropertyNames(Match3.GameManager),gameMethods:Object.getOwnPropertyNames(Match3.GameManager.prototype),uiMethods:Object.getOwnPropertyNames(Match3.UI.GameUIManager.prototype),object:Object.getOwnPropertyNames(UnityEngine.Object),gameObject:Object.getOwnPropertyNames(UnityEngine.GameObject),registry:Object.keys(LunaUnity.Objects.Registry)})''')
 (out/'runtime.json').write_text(json.dumps(runtime,indent=2),encoding='utf-8')
 end=page.evaluate('''()=>{const find=UnityEngine.Object.FindObjectOfType$1;const g=find(Match3.GameManager);window.__companyGame=g;return {found:!!g,fn:find.toString().slice(0,400),keys:g?Object.keys(g):[]};}''')
 (out/'game.json').write_text(json.dumps(end,indent=2),encoding='utf-8')
 if end['found']:
  page.evaluate('()=>{__companyGame.isGameOver=true;__companyGame.uiManager.ShowDownloadPanel();}')
  page.wait_for_timeout(1200)
  page.screenshot(path=str(out/'end.png'))
 print(json.dumps({'globals':info['globals'],'jsons':len(info['jsons']),'blobs':len(info['blobs']),'errors':errors},ensure_ascii=False))
 b.close()

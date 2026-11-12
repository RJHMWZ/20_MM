from pathlib import Path
import json
from playwright.sync_api import sync_playwright
from PIL import Image
out=Path('.codex/company')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':512,'height':512},device_scale_factor=1)
 page.add_init_script('''const original=HTMLCanvasElement.prototype.getContext;HTMLCanvasElement.prototype.getContext=function(t,a){if(t.includes('webgl'))a=Object.assign({},a,{alpha:true,preserveDrawingBuffer:true});return original.call(this,t,a);};''')
 page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri(),wait_until='domcontentloaded');page.wait_for_timeout(6000)
 info=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);window.__g=g;g.enabled=false;g.uiManager.gameObject.SetActive(false);const ui=UnityEngine.GameObject.Find('UI');if(ui)ui.SetActive(false);const camera=UnityEngine.Camera.main;camera.orthographicSize=2;var c=camera.backgroundColor;c.r=c.g=c.b=c.a=0;camera.backgroundColor=c;document.body.style.background='transparent';document.documentElement.style.background='transparent';const normal=g.elementDatabase.normalElements._items[2];window.__fx=Match3.Match3EffectPlayer.Play(normal.clearEffectPrefab,pc.Vec3.ZERO.clone(),null,2,0,null,true);return {name:normal.sprite.name,roots:pc.Application.getApplication().getRootObjects().map(r=>r.name)};}''')
 page.evaluate('''()=>{for(const r of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Renderer))if(!r.gameObject.name.includes('clone'))r.enabled=false;for(const c of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Canvas))c.enabled=false;const camera=UnityEngine.Camera.main;const c=camera.backgroundColor;c.r=c.g=c.b=c.a=0;camera.backgroundColor=c;}''')
 page.evaluate('''()=>{UnityEngine.Object.Destroy(__fx);__fx=Match3.Match3EffectPlayer.Play(__g.elementDatabase.normalElements._items[2].clearEffectPrefab,pc.Vec3.ZERO.clone(),null,2,0,null,true);}''')
 page.wait_for_timeout(120)
 page.screenshot(path=str(out/'fx-probe.png'),omit_background=True)
 page.evaluate('()=>{UnityEngine.Time.timeScale=0;}')
 page.wait_for_timeout(30);page.screenshot(path=str(out/'fx-frozen1.png'),omit_background=True)
 page.wait_for_timeout(200);page.screenshot(path=str(out/'fx-frozen2.png'),omit_background=True)
 page.evaluate('()=>{var c=UnityEngine.Camera.main.backgroundColor;c.r=c.g=c.b=c.a=1;UnityEngine.Camera.main.backgroundColor=c;}')
 page.wait_for_timeout(40);page.screenshot(path=str(out/'fx-white.png'),omit_background=True)
 (out/'fx-probe.json').write_text(json.dumps(info,indent=2),encoding='utf-8')
 api=page.evaluate('''()=>({simulate:UnityEngine.ParticleSystem.prototype.Simulate?UnityEngine.ParticleSystem.prototype.Simulate.toString().slice(0,900):null,methods:Object.getOwnPropertyNames(UnityEngine.ParticleSystem.prototype).filter(n=>/simulate|pause|play|seed/i.test(n)),tick:Object.keys(pc.Application.getApplication()).filter(k=>/app|time|tick/i.test(k))})''')
 (out/'fx-capture-api.json').write_text(json.dumps(api,indent=2),encoding='utf-8')
 b.close()
im=Image.open(out/'fx-probe.png').convert('RGBA');print('alpha',im.getchannel('A').getextrema(),'bbox',im.getbbox(),info)

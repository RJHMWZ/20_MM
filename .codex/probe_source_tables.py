from playwright.sync_api import sync_playwright
import json,pathlib,re
out=pathlib.Path('.codex/exact');out.mkdir(exist_ok=True)
with sync_playwright() as p:
 b=p.chromium.launch(headless=True,channel='msedge');page=b.new_page(viewport={'width':540,'height':960});page.goto('file:///C:/Users/ivy/Desktop/20.html');page.wait_for_timeout(3500)
 d=page.evaluate('''() => {let result={};for(let name of ['TiledMap','Tiled','BlockerManager','FSBase','TiledMapDropDataSpawner','ColorManager','MatchTipsManager','HenEggBlocker','HenHouseBlocker','ShrubPeacockBlocker','ScreamChickenBlocker']){let m=__require(name);result[name]={};for(let k of Object.keys(m)){let C=m[k];if(typeof C==='function'){let f={};for(let p of Object.getOwnPropertyNames(C.prototype||{})){let x=Object.getOwnPropertyDescriptor(C.prototype,p);if(typeof x.value==='function')f[p]=x.value.toString()}result[name][k]=f}}}let L=__require('LevelManager').LevelManager.Instance;result.tileKeys=Object.keys(L.CurrentMap.TiledArray[0]);result.mapData=L.CurrentMap.m_lvlData;return result}''')
 d['tables']=page.evaluate('''() => Object.fromEntries([3,4,11,12,13,14,15,100016,100042,100032,100059,100015].map(id=>[id,__require('Game').default.GetBlockData(id).Data]))''')
 (out/'runtime.json').write_text(json.dumps(d,ensure_ascii=False,indent=2),encoding='utf8')
 page.evaluate('''() => {window.trace=[];let M=__require('LevelManager').LevelManager.Instance.CurrentMap;for(let k of ['RandomRange','GetRandomColor','Random','RandomBlocker']){if(typeof M[k]==='function'){let f=M[k];M[k]=function(...a){let before=this.RandomSeed,v=f.apply(this,a);trace.push({k,a,before,after:this.RandomSeed,v,stack:new Error().stack});return v}}}}''')
 page.evaluate('''() => {window.drops=[];window.sourceEvents=[];let BO=__require('Blocker').Blocker.prototype,oldDestroy=BO.OnDestroyObj;BO.OnDestroyObj=function(...a){if(this.ID<100)sourceEvents.push({kind:'clear',id:this.ID,row:this.SelfTiled?.Row,col:this.SelfTiled?.Col,t:performance.now()});return oldDestroy.apply(this,a)};let C=__require('Tiled').EntryTiled.prototype,f=C.RandomBlocker;C.RandomBlocker=function(...a){let M=__require('LevelManager').LevelManager.Instance.CurrentMap,before=M.RandomSeed,v=f.apply(this,a);if(v)drops.push({time:performance.now(),row:this.Row,col:this.Col,id:this.CanMoveBlocker.ID,before,after:M.RandomSeed,last:this.m_lastDropColorId});return v}}''')
 def snap(n):
  page.screenshot(path=str(out/(n+'.png')))
  d=page.evaluate('''() => {let L=__require('LevelManager').LevelManager.Instance;let root=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0');return {step:L.UseStep,seed:L.CurrentMap.RandomSeed,tiles:L.CurrentMap.TiledArray.filter(t=>t.IsValidTiled()).map(t=>({row:t.Row,col:t.Col,id:t.CanMoveBlocker?.ID,hp:t.CanMoveBlocker?.CurHp,entry:t.EnterPoint?.Guid})), nodes:root.children.map(n=>({name:n.name,x:n.x,y:n.y,scale:n.scale,children:n.children.map(c=>({name:c.name,x:c.x,y:c.y,scale:c.scale,skeleton:c.getComponent(sp.Skeleton)?{skin:c.getComponent(sp.Skeleton).defaultSkin,anim:c.getComponent(sp.Skeleton).animation}:null}))}))}}''')
  (out/(n+'.json')).write_text(json.dumps(d,ensure_ascii=False,indent=2),encoding='utf8')
 snap('start');page.mouse.move(440,395);page.mouse.down();page.mouse.move(498,395,steps=12);page.mouse.up();page.wait_for_timeout(6000);snap('first')
 d=page.evaluate('''() => {let root=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0');function w(n){let s=n.getComponent(sp.Skeleton);return {name:n.name,x:n.x,y:n.y,sx:n.scaleX,sy:n.scaleY,anim:s?.animation,skin:s?.defaultSkin,data:s?.skeletonData?.name,children:n.children.map(w)}}return root.children.filter(n=>n.name.endsWith('_14')).map(w)}''');(out/'power.json').write_text(json.dumps(d,indent=2),encoding='utf8')
 (out/'trace.json').write_text(json.dumps(page.evaluate('trace'),indent=2),encoding='utf8')
 (out/'drops.json').write_text(json.dumps(page.evaluate('drops'),indent=2),encoding='utf8')
 (out/'source-events.json').write_text(json.dumps(page.evaluate('sourceEvents'),indent=2),encoding='utf8')
 print('tables',d['tables'])
 print('first',page.evaluate("__require('LevelManager').LevelManager.Instance.UseStep"))
 b.close()
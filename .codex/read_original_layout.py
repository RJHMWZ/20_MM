from playwright.sync_api import sync_playwright
import json,pathlib
with sync_playwright() as p:
 b=p.chromium.launch(headless=True,channel='msedge')
 for wh in [(540,960),(1920,911)]:
  page=b.new_page(viewport={'width':wh[0],'height':wh[1]});page.goto('file:///C:/Users/ivy/Desktop/20.html');page.wait_for_timeout(1800)
  d=page.evaluate('''()=>{let result={size:{w:cc.winSize.width,h:cc.winSize.height},nodes:[]};function desc(n){return {name:n.name,x:n.x,y:n.y,w:n.width,h:n.height,ax:n.anchorX,ay:n.anchorY,sx:n.scaleX,sy:n.scaleY,active:n.active,world:n.convertToWorldSpaceAR(cc.v2())}}for(let path of ['Canvas','Canvas/Bg','Canvas/MapRoot/TiledMap_0','Canvas/UICustom','Canvas/UICustom/DownloadBtn']){let n=cc.find(path);if(n)result.nodes.push(desc(n))}let root=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0'),corn=root.children.find(n=>n.name.endsWith('_100059')),dyn=corn.getChildByName('Root').getChildByName('ArtRoot').getChildByName('Dynamic'),mask=dyn.getChildByName('Mask'),m=mask.getComponent(cc.Mask);result.corn=[corn,dyn,mask,mask.children[0],mask.children[0].children[0]].map(desc);result.mask={type:m.type,inverted:m.inverted,enabled:m.enabled,rect:m._graphics&&m._graphics._impl&&m._graphics._impl._paths};return JSON.parse(JSON.stringify(result))}''')
  print(wh,json.dumps(d,ensure_ascii=True)[:6500]);pathlib.Path('.codex/original-'+str(wh[0])+'.json').write_text(json.dumps(d,indent=2),encoding='utf8');page.close()
 b.close()

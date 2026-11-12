from playwright.sync_api import sync_playwright
from pathlib import Path
import json
out=Path('.codex/exact/timing');out.mkdir(exist_ok=True)
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge')
 page=b.new_page(viewport={'width':540,'height':960});page.goto('file:///C:/Users/ivy/Desktop/20.html');page.wait_for_timeout(3000)
 page.evaluate('''async()=>{await new Promise(r=>cc.assetManager.getBundle('Sub').loadDir('prefab',cc.Prefab,()=>r()));cc.director.pause();window.probes=[];for(let id of [11,13,14,15]){let name={11:'LineBlock',13:'AreaBlock',14:'SquareBlock',15:'SameColorBlock'}[id];let prefab;cc.assetManager.assets.forEach(a=>{if(a instanceof cc.Prefab&&a.name===name)prefab=a});let n=cc.instantiate(prefab);n.setParent(cc.find('Canvas'));n.x=(id-13)*200;n.y=0;probes.push(n);n.getComponentInChildren(cc.Animation)?.play('ele_anim_'+{11:'line',13:'area',14:'square',15:'samecolor'}[id]+'_generate')}}''')
 def snap():return page.evaluate('''() => {function w(n){let s=n.getComponent(sp.Skeleton),a=n.getComponent(cc.Animation);return {name:n.name,x:n.x,y:n.y,sx:n.scaleX,sy:n.scaleY,w:n.width,h:n.height,anim:s?.animation,skel:s?.skeletonData?.name,bones:s?._skeleton?.bones.slice(0,3).map(b=>({name:b.data.name,sx:b.scaleX,sy:b.scaleY,x:b.worldX,y:b.worldY})),clips:a?._clips.map(c=>({name:c.name,data:c.curveData})),children:n.children.map(w)}}return probes.map(w)}''')
 (out/'power-initial.json').write_text(json.dumps(snap(),ensure_ascii=False,default=str),encoding='utf8')
 page.evaluate('cc.director.resume()');page.wait_for_timeout(1500);page.evaluate('cc.director.pause()')
 (out/'power-final.json').write_text(json.dumps(snap(),ensure_ascii=False,default=str),encoding='utf8');page.screenshot(path=str(out/'powers.png'))
 print(page.evaluate('''() => probes.map(n=>{let s=n.getComponentInChildren(sp.Skeleton),o=cc.v2(),z=cc.v2();s._skeleton.getBounds(o,z,[]);let b=s._skeleton.bones[0];return {name:n.name,scale:s._skeleton.scaleX,root:b.a,bounds:z,data:s.skeletonData.skeletonJson.skeleton,bone:{sx:b.data.scaleX,sy:b.data.scaleY},tex:s.skeletonData.textureNames}})'''));b.close()
print('captured original power prefabs')


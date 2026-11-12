from playwright.sync_api import sync_playwright
from pathlib import Path
import json
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge');p=b.new_page(viewport={'width':540,'height':960});p.goto('file:///C:/Users/ivy/Desktop/20.html');p.wait_for_timeout(2000)
 p.evaluate("async()=>{await Promise.all([new Promise(r=>cc.resources.loadDir('prefab',cc.Prefab,()=>r())),new Promise(r=>cc.assetManager.getBundle('Sub').loadDir('prefab',cc.Prefab,()=>r()))])}")
 data=p.evaluate('''()=>{cc.director.pause();let effects={},pieces={};function walk(n){let s=n.getComponent(cc.Sprite),an=n.getComponent(cc.Animation);return {name:n.name,sizeMode:s?s.sizeMode:null,w:n.width,h:n.height,clips:an?an._clips.filter(c=>c).map(c=>c.name):[],children:n.children.map(walk)}};let root=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0');root.children.forEach(n=>pieces[n.name]=walk(n));cc.assetManager.assets.forEach(a=>{if(a instanceof cc.Prefab){let n=cc.instantiate(a);effects[a.name]=walk(n);n.destroy()}});return{effects,pieces}}''')
 Path('.codex/exact/sprite-sizing.json').write_text(json.dumps(data),encoding='utf-8');b.close()
 def walk(n,path):
  if n['sizeMode'] is not None: print(path,n['sizeMode'],n['w'],n['h'])
  if n['clips']:print('CLIP',path,n['clips'])
  for c in n['children']:walk(c,path+'/'+c['name'])
 walk(data['effects']['LineMoveEffect'],'LineMoveEffect');walk(data['pieces']['3_6_100016'],'Nest')

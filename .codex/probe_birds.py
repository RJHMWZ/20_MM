from playwright.sync_api import sync_playwright
from pathlib import Path
import json
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge')
 for kind,row,col in [('peacock',3,4),('nest',3,6)]:
  p=b.new_page(viewport={'width':540,'height':960});p.goto('file:///C:/Users/ivy/Desktop/20.html');p.wait_for_timeout(2000)
  info=p.evaluate('''([row,col])=>{let M=__require('LevelManager').LevelManager.Instance.CurrentMap;__require('UIManager').UIManager.Instance.CloseLevelGuide();window.birdBirth=[];window.t0=performance.now();let T=__require('Tiled').Tiled.prototype,old=T.UpdateToSpecial;T.UpdateToSpecial=function(id,...a){birdBirth.push({id,x:this.Col-4,y:this.Row-3,time:(performance.now()-t0)/1000,seed:M.RandomSeed});return old.call(this,id,...a)};let tile=M.GetTiled(row,col);window.testTile=tile;return {id:tile.MatchBlocker.ID,name:tile.MatchBlocker.constructor.name,seed:M.RandomSeed}}''',[row,col])
  print(kind,info,flush=True)
  for i in range(3):
   print(p.evaluate('''()=>{let t=testTile,b=t.MatchBlocker;b.DecrHP();b.Destroy(t);return{hp:b.CurHp}}'''),flush=True);p.wait_for_timeout(1800)
  p.wait_for_timeout(3000);d=p.evaluate('''()=>({births:birdBirth,seed:__require('LevelManager').LevelManager.Instance.CurrentMap.RandomSeed})''');Path('.codex/exact/timing/source-birds-'+kind+'.json').write_text(json.dumps(d),encoding='utf-8');print(d,flush=True);p.close()
 b.close()

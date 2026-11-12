from playwright.sync_api import sync_playwright
from pathlib import Path
import json,time
out=Path('.codex/exact/combos');out.mkdir(exist_ok=True)
cases=[(11,11),(11,12),(12,12),(11,13),(13,13),(14,14),(14,11),(14,12),(14,13),(15,3),(15,11),(15,12),(15,13),(15,14),(15,15)]
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge')
 for ia,ib in cases:
  p=b.new_page(viewport={'width':540,'height':960});p.goto('file:///C:/Users/ivy/Desktop/20.html');p.wait_for_timeout(2000)
  p.evaluate('''([a,b])=>{let M=__require('LevelManager').LevelManager.Instance.CurrentMap;__require('UIManager').UIManager.Instance.CloseLevelGuide();M.GetTiled(7,11).UpdateToSpecial(a,0,false,true,false,false);M.GetTiled(7,12).UpdateToSpecial(b,0,false,true,false,false);}''',[ia,ib]);p.wait_for_timeout(600)
  p.evaluate('''()=>{window.actionStart=performance.now();window.events=[];let C=__require('Blocker').Blocker.prototype,f=C.OnDestroyObj;C.OnDestroyObj=function(...a){events.push({kind:'clear',id:this.ID,row:this.SelfTiled?.Row,col:this.SelfTiled?.Col,t:performance.now()-actionStart});return f.apply(this,a)};window.drops=[];let E=__require('Tiled').EntryTiled.prototype,old=E.RandomBlocker;E.RandomBlocker=function(...a){let r=old.apply(this,a);if(r)drops.push({col:this.Col,id:this.CanMoveBlocker.ID,t:performance.now()-actionStart});return r}}''')
  p.mouse.move(440,395);p.mouse.down();p.mouse.move(498,395,steps=6);p.mouse.up()
  t=0
  for dt in [150,150,300,600]:
   p.wait_for_timeout(dt);t+=dt
   if (ia,ib)in [(11,13),(14,13),(15,3),(15,15)]:p.screenshot(path=str(out/f'source-{ia}-{ib}-{t}.png'))
  p.wait_for_timeout(6500)
  d=p.evaluate('''()=>{let L=__require('LevelManager').LevelManager.Instance,M=L.CurrentMap;return {step:L.UseStep,seed:M.RandomSeed,events,drops,tiles:M.TiledArray.filter(t=>t.IsValidTiled()&&t.CanMoveBlocker).map(t=>({x:t.Col-4,y:t.Row-3,id:t.CanMoveBlocker.ID,hp:t.CanMoveBlocker.CurHp})),busy:__require('FSBase').FSM.getInstance().m_prepareState.length}}''')
  (out/f'source-{ia}-{ib}.json').write_text(json.dumps(d),encoding='utf-8');print(ia,ib,'step',d['step'],'busy',d['busy'],'drops',len(d['drops']),flush=True);p.close()
 b.close()

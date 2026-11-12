from playwright.sync_api import sync_playwright
from pathlib import Path
import json
out=[]
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge')
 for a in [11,13,15]:
  p=b.new_page(viewport={'width':540,'height':960});p.goto('file:///C:/Users/ivy/Desktop/20.html');p.wait_for_timeout(1800)
  p.evaluate("""a=>{window.M=__require('LevelManager').LevelManager.Instance.CurrentMap;window.F=__require('FSBase').FSM.getInstance();__require('UIManager').UIManager.Instance.CloseLevelGuide();M.GetTiled(7,11).UpdateToSpecial(a,0,false,true,false,false);M.GetTiled(10,12).UpdateToSpecial(13,0,false,true,false,false)}""",a);p.wait_for_timeout(400)
  p.evaluate("()=>F.OnDoubleClick(7,11)");p.wait_for_timeout(80)
  info=p.evaluate("""()=>{let q=M.GetTiled(10,12);return{canOperate:F.CanOperate(),pieceCanSwitch:q.CanMoveBlocker&&q.CanMoveBlocker.IsCanSwitch(),step:__require('LevelManager').LevelManager.Instance.UseStep}}""")
  if info['canOperate']:p.evaluate("()=>F.OnDoubleClick(10,12)")
  p.wait_for_timeout(30);info['afterSecondStep']=p.evaluate("()=>__require('LevelManager').LevelManager.Instance.UseStep");info['first']=a;out.append(info);p.close()
 b.close()
Path('.codex/exact/source-realtime.json').write_text(json.dumps(out),encoding='utf-8');print(out)

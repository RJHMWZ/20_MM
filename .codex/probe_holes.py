from playwright.sync_api import sync_playwright
import json
from pathlib import Path
with sync_playwright() as w:
 b=w.chromium.launch(headless=True,channel='msedge');p=b.new_page(viewport={'width':540,'height':960});p.goto('file:///C:/Users/ivy/Desktop/20.html');p.wait_for_timeout(1800)
 p.evaluate("""()=>{window.M=__require('LevelManager').LevelManager.Instance.CurrentMap;window.F=__require('FSBase').FSM.getInstance();__require('UIManager').UIManager.Instance.CloseLevelGuide();M.GetTiled(7,11).UpdateToSpecial(11,0,false,true,false,false);M.GetTiled(10,12).UpdateToSpecial(13,0,false,true,false,false)}""");p.wait_for_timeout(400);p.evaluate("()=>F.OnDoubleClick(7,11)");p.wait_for_timeout(80);p.evaluate("()=>F.OnDoubleClick(10,12)");p.wait_for_timeout(9000)
 d=p.evaluate("""()=>[[0,2],[1,4]].map(([x,y])=>{let t=M.GetTiled(y+3,x+4);return{x,y,hasBlocker:!!t.MatchBlocker,blockerID:t.MatchBlocker?.ID,middleID:t.MiddleBlocker()?.ID,occupied:t.Occupy(),hasMovable:!!t.CanMoveBlocker,valid:t.IsValidTiled(),marked:t.Marked,above:t.GetNeighborTop().MatchBlocker?.ID}})""");print(d);Path('.codex/exact/source-obstructed-vacancies.json').write_text(json.dumps(d),encoding='utf-8');b.close()

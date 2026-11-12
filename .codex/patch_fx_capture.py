from pathlib import Path
p=Path('.codex/capture_company_fx.py');s=p.read_text(encoding='utf-8')
s=s.replace('for(const ps of __systems)ps.Simulate(0,false,true,false);return','window.__startTime=UnityEngine.Time.time;return')
s=s.replace('  max_alpha=0','  max_alpha=0;times=[]')
s=s.replace("   page.evaluate('()=>{__step(1/30);__setBg(0);}')", "   page.evaluate('()=>{UnityEngine.Time.timeScale=1;}')\n   page.wait_for_timeout(33)\n   t=page.evaluate('()=>{UnityEngine.Time.timeScale=0;__setBg(0);return UnityEngine.Time.time-__startTime;}')\n   times.append(t)")
s=s.replace("'cells':width,'sourcePrefabs'", "'cells':width,'times':times,'sourcePrefabs'")
p.write_text(s,encoding='utf-8')

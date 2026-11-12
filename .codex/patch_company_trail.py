from pathlib import Path
p=Path('.codex/capture_company_fx.py');s=p.read_text(encoding='utf-8')
s=s.replace('window.__startTime=UnityEngine.Time.time;return','window.__fxName=name;window.__startTime=UnityEngine.Time.time;return')
s=s.replace("page.evaluate('()=>{UnityEngine.Time.timeScale=1;}')", "page.evaluate('i=>{if(__fxName===\"ColorTrail\")for(const fx of __fx)fx.transform.position=new pc.Vec3(-1.4+Math.min(1,i/4)*2.8,0,0);UnityEngine.Time.timeScale=1;}',i)")
p.write_text(s,encoding='utf-8')

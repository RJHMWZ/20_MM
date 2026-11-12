from pathlib import Path
import json,re,base64,io
from PIL import Image
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
script=Path('.codex/extract_company_particles.js').read_text('utf8')
script=script.replace('for(const [name,prefabs] of Object.entries(configs)){','for(const [name,prefabs] of Object.entries(configs).filter(([name])=>name===\'RocketTrail\')){')
script=script.replace("name==='RocketTrail'?1.5","name==='RocketTrail'?4")
script=script.replace('n.main.useUnscaledTime=true;n.play(false);','n.main.useUnscaledTime=true;n.main.simulationSpace=1;n.play(false);')
script=script.replace('n.update(dt,n.entity.getWorldTransform());const c=n._cache','if(frame===90)n.emission.enabled=false;n.update(dt,n.entity.getWorldTransform());const c=n._cache')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page()
 page.goto(src.as_uri());page.wait_for_timeout(3500)
 data=page.evaluate(script);b.close()
out=Path('Assets/Resources/Reference20/Company/Particles')
for e in data['effects']:
 (out/(e['name']+'.json')).write_text(json.dumps(e,separators=(',',':')),encoding='utf8')
 print(e['name'],e['duration'],[(l['name'],len(l['tracks'])) for l in e['layers']])

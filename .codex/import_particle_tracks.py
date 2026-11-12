from pathlib import Path
import json,shutil
from PIL import Image
from playwright.sync_api import sync_playwright
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page();page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri());page.wait_for_timeout(6500)
 data=page.evaluate(Path('.codex/extract_company_particles.js').read_text(encoding='utf-8'))
 b.close()
out=Path('Assets/Resources/Reference20/Company/Particles');out.mkdir(exist_ok=True)
for e in data['effects']:
 print(e['name'],[(x['name'],len(x['tracks']),x['texture'],x['shader']) for x in e['layers']],flush=True)
 for layer in e['layers']:
  path=next(Path('.codex/company').glob('assets_bundles_-1_'+str(layer['texture'])+'.*.png'),None)
  if not path: raise ValueError('Missing texture '+str(layer['texture']))
  Image.open(path).convert('RGBA').save(out/(str(layer['texture'])+'.png'))
 (out/(e['name']+'.json')).write_text(json.dumps(e,separators=(',',':')),encoding='utf-8')
Path('.codex/company/particle-track-provenance.json').write_text(json.dumps({'source':'樊思吾.html','fps':60,'effects':[{'name':e['name'],'layers':len(e['layers']),'tracks':sum(len(l['tracks']) for l in e['layers'])} for e in data['effects']]},indent=2),encoding='utf-8')

from pathlib import Path
import json,re,base64,io
from PIL import Image
from playwright.sync_api import sync_playwright
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
script=Path('.codex/extract_company_particles.js').read_text('utf8')
script=script.replace('const result={effects:[]};',"configs.ColorCombo=[[spec(3).prefab.GetComponent$1(Match3.SpecialElementView).swapParticlePrefab,1]];const result={effects:[]};")
script=script.replace('for(const [name,prefabs] of Object.entries(configs)){',"for(const [name,prefabs] of Object.entries(configs).filter(([name])=>name==='ColorCombo')){")
script=script.replace("duration:name==='ColorCharge'", "duration:name==='ColorCombo'?2.2:name==='ColorCharge'")
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page()
 page.goto(src.as_uri());page.wait_for_timeout(3500);data=page.evaluate(script);b.close()
out=Path('Assets/Resources/Reference20/Company/Particles');html=src.read_text('utf8')
for e in data['effects']:
 (out/(e['name']+'.json')).write_text(json.dumps(e,separators=(',',':')),encoding='utf8')
 for l in e['layers']:
  m=re.search(r'<img id="assets/bundles/-1/'+str(l['texture'])+r'\.[^"]+" src="data:image/[^;]+;base64,([^"]+)"',html)
  Image.open(io.BytesIO(base64.b64decode(m[1]))).save(out/(str(l['texture'])+'.png'))
 print(e['name'],[(l['name'],len(l['tracks'])) for l in e['layers']])

from pathlib import Path
import json,re,base64,io,math
from PIL import Image
from playwright.sync_api import sync_playwright
src=Path('C:/Users/yangjinsheng/Desktop/\u6a0a\u601d\u543e.html')
script=Path('.codex/extract_company_particles.js').read_text('utf8')
script=script.replace('for(const [name,prefabs] of Object.entries(configs)){',"for(const [name,prefabs] of Object.entries(configs).filter(([name])=>['Bomb','BeeImpact','ColorCharge','ColorFinish'].includes(name))){")
# Unity keeps animatedVelocity empty when the velocity-over-lifetime module is off.
# The previous exporter added undefined values and lost every stretched billboard.
for k in ['ii','ii+1','ii+2']:
 script=script.replace(f'c.velocity[{k}]+c.animatedVelocity[{k}]',f'(c.velocity[{k}]||0)+(c.animatedVelocity[{k}]||0)')
script=script.replace('r.lengthScale','r._particleSystemRenderer.lengthScale').replace('r.velocityScale','r._particleSystemRenderer.velocityScale')
script=script.replace('if(layer.tracks.length)effect.layers.push(layer);', '''layer.debug={mode:r.renderMode,lengthScale:r.lengthScale,velocityScale:r.velocityScale,keys:Object.keys(r),scale:n.entity.getLossyScale(),size:Array.from(n._cache.s).slice(0,12),rot:Array.from(n._cache.r).slice(0,16)};if(layer.tracks.length)effect.layers.push(layer);''')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader']);page=b.new_page()
 page.goto(src.as_uri());page.wait_for_timeout(3500);result=page.evaluate(script);b.close()
Path('.codex/html-parity/raw-particle-refine.json').write_text(json.dumps(result),encoding='utf8')
out=Path('Assets/Resources/Reference20/Company/Particles');html=src.read_text('utf8');report=[]
for effect in result['effects']:
 for layer in effect['layers']:
  for track in layer['tracks']:
   if not all(math.isfinite(v) for sample in track['samples'] for v in sample):raise ValueError((effect['name'],layer['name']))
  m=re.search(r'<img id="assets/bundles/-1/'+str(layer['texture'])+r'\.[^"]+" src="data:image/[^;]+;base64,([^"]+)"',html)
  Image.open(io.BytesIO(base64.b64decode(m[1]))).convert('RGBA').save(out/(str(layer['texture'])+'.png'))
 (out/(effect['name']+'.json')).write_text(json.dumps(effect,separators=(',',':'),allow_nan=False),encoding='utf8')
 report.append({'name':effect['name'],'layers':len(effect['layers']),'particles':sum(len(l['tracks']) for l in effect['layers'])})
Path('.codex/html-parity/refined-particle-import.json').write_text(json.dumps(report,indent=2),encoding='utf8');print(report)

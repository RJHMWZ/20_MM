from pathlib import Path
import json
from PIL import Image,ImageDraw
source=Path('.codex/company')
bundle=json.loads((source/'jsons.json').read_text(encoding='utf-8'))['assets/bundles/-1/bundle.json']
sprites={s['data'][0]:s for s in bundle['sprites']}
out=Path('Assets/Resources/Reference20/Company/Elements');out.mkdir(exist_ok=True)
sheet=Image.new('RGBA',(512,160),'#777777');draw=ImageDraw.Draw(sheet)
for i,name in enumerate(['brick1_0','brick2_0','brick3_0','brick4_0']):
 s=sprites[name];tid=s['data'][2];p=next(source.glob(f'assets_bundles_-1_{tid}.*.png'))
 im=Image.open(p).convert('RGBA');im.save(out/(name+'.png'));im.thumbnail((120,120));sheet.alpha_composite(im,(i*128,0));draw.text((i*128,125),name,fill='white')
sheet.save(source/'base-elements.png')
print('Imported company base elements')

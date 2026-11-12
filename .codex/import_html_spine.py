from pathlib import Path
import json,re,base64,io
from PIL import Image
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
images={}
for m in re.finditer(r'<img\b([^>]+)>',src.read_text('utf8')):
 a=dict(re.findall(r'([\w-]+)="([^"]*)"',m[1]));images[a.get('id')]=a.get('src')
bundles=json.loads(Path('.codex/company/jsons.json').read_text('utf8'))
b=bundles['assets/bundles/-1/bundle.json'];out=Path('Assets/Resources/Reference20/Props')
for name in ['Bomb','bomb_bomb','ColorBomb_ColorBomb','element']:
 for ext in ['.json','.atlas.txt']:
  item=next(x for x in b['text-assets'] if x['path'].endswith('/'+name+ext))
  (out/(name+ext)).write_text(item['data'][2],encoding='utf8')
 t=next(x for x in b['textures'] if x['data'][0]==name)
 uri=next(v for k,v in images.items() if k and k.startswith(f"assets/bundles/-1/{t['id']}."))
 Image.open(io.BytesIO(base64.b64decode(uri.split(',')[1]))).save(out/(name+'.png'))
 print(name,t['id'])
b=bundles['assets/bundles/-2/bundle.json'];s=next(s for s in b['sprites'] if s['data'][0]=='Background')
uri=next(v for k,v in images.items() if k and k.startswith(f"assets/bundles/-2/{s['data'][2]}."))
Image.open(io.BytesIO(base64.b64decode(uri.split(',')[1]))).save(Path('Assets/Resources/Reference20/Company/UI/Background.png'))

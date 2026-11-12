from pathlib import Path
import json,shutil,zipfile
from PIL import Image
base=Path('C:/Users/ivy/Desktop')/('Spine\u8d44\u6e90')/('\u5143\u7d20\u52a8\u753b\u65b0\u7248');dst=Path('Assets/Resources/Reference20/Skins');dst.mkdir(exist_ok=True)
for name in ['bush','rockchipper']:
 folder=next(base.glob('*_'+name))
 for ext in ['.atlas.txt','.png','.skel.bytes']:shutil.copyfile(folder/(name+ext),dst/(name+ext))
 im=Image.open(dst/(name+'.png')).convert('RGBA');pixels=[p for p in im.getdata()if 8<p[3]<247];print(name,'partial-alpha',len(pixels),'straight fraction',sum(max(p[:3])>p[3]+5 for p in pixels)/max(1,len(pixels)))
cap=dst/'Cabinet';cap.mkdir(exist_ok=True)
z=zipfile.ZipFile(next(p for p in Path('C:/Users/ivy/Desktop').glob('*.zip')if p.name.startswith('\u6a71')))
for n in z.namelist():
 if n.endswith('.png') and not Path(n).name.startswith('000'): (cap/Path(n).name).write_bytes(z.read(n))
g=json.loads(Path('Assets/Resources/Reference20/Native/graph.json').read_text(encoding='utf-8'))
for n in g['pieces']:
 if n['name'].endswith(('_100016','_100042','_100059')):print(n['name'],'pos',n['x'],n['y'],'scale',n['sx'],n['sy']);

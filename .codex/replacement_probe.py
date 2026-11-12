from pathlib import Path
import zipfile
root=Path('C:/Users/ivy/Desktop');base=root/('Spine\u8d44\u6e90')/('\u5143\u7d20\u52a8\u753b\u65b0\u7248')
for suffix in ['bush','rockchipper']:
 folder=next(base.glob('*_'+suffix))
 for p in folder.glob('*.bytes'):print(p.name,p.read_bytes()[:65])
z=zipfile.ZipFile(next(p for p in root.glob('*.zip')if p.name.startswith('\u6a71')));out=Path('.codex/replacement/cabinet');out.mkdir(parents=True,exist_ok=True)
for n in z.namelist():
 if n.endswith('.png'):(out/Path(n).name).write_bytes(z.read(n))
from PIL import Image,ImageDraw
files=list(out.glob('elements*.png'))+list(out.glob('000*.png'))[:3];canvas=Image.new('RGBA',(900,450),'#777777');d=ImageDraw.Draw(canvas)
for i,p in enumerate(files):
 im=Image.open(p);print(p.name,im.size);im.thumbnail((200,180));x=(i%4)*225;y=(i//4)*225;canvas.alpha_composite(im,(x,y));d.text((x,y+180),p.name,fill='white')
canvas.convert('RGB').save('.codex/replacement/cabinet-preview.jpg')

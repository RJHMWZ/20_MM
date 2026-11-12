from pathlib import Path
import re,base64,brotli
source=Path('C:/Users/ivy/Desktop/樊思吾.html').read_text(encoding='utf-8-sig')
out=Path('.codex/company')
matches=list(re.finditer(r'decompressArrayBuffer\(\s*"([A-Za-z0-9+/=]+)"',source))
print('compressed blocks',len(matches))
for i,m in enumerate(matches):
 data=brotli.decompress(base64.b64decode(m[1]))
 (out/f'block-{i}.bin').write_bytes(data)
 print(i,len(data),repr(data[:120]))
for name in ['export_native.py','export_supp.py','export_end.py']:
 p=Path('.codex')/name
 p.write_text(p.read_text(encoding='utf-8-sig').replace('\ufeff',''),encoding='utf-8')
for m in re.finditer(r'<img\b([^>]+)>',source):
 attrs=dict(re.findall(r'([\w-]+)="([^"]*)"',m[1]))
 uri=attrs.get('src','')
 print('image',attrs.get('id'),len(uri),uri[:35])
 if uri.startswith('data:image/'):
  key=attrs.get('id',str(m.start())).replace('/','_')
  (out/(key+'.png')).write_bytes(base64.b64decode(uri.split(',',1)[1]))

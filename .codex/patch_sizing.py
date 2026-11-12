from pathlib import Path
import json
p=Path('Assets/Resources/Reference20/Native/graph.json');g=json.loads(p.read_text(encoding='utf-8-sig'));d=json.loads(Path('.codex/exact/sprite-sizing.json').read_text());count=0

def merge(n,s):
 global count
 if s.get('sizeMode') is not None:n['spriteSizeMode']=s['sizeMode'];count+=1
 for i,c in enumerate(n.get('children',[])):
  candidates=[x for x in s['children'] if x['name']==c['name']]
  if candidates:merge(c,candidates[0])
for n in g['pieces']:
 if n['name'] in d['pieces']:merge(n,d['pieces'][n['name']])
for name,n in g['effects'].items():
 if name in d['effects']:merge(n,d['effects'][name])
p.write_text(json.dumps(g,ensure_ascii=False,separators=(',',':')),encoding='utf-8');print('sizes',count)

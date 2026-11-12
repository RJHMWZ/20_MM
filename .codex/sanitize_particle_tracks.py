from pathlib import Path
import json,math
report=[]
for path in Path('Assets/Resources/Reference20/Company/Particles').glob('*.json'):
 data=json.loads(path.read_text('utf8'));removed=0
 for layer in data.get('layers',[]):
  if 'tracks' not in layer:continue
  tracks=[]
  for track in layer['tracks']:
   run=None
   for i,sample in enumerate(track['samples']):
    valid=all(isinstance(v,(int,float)) and math.isfinite(v) for v in sample)
    if not valid:removed+=1;run=None;continue
    if run is None:run={'start':track['start']+i/60,'samples':[]};tracks.append(run)
    run['samples'].append(sample)
  layer['tracks']=tracks
 if removed:
  data['layers']=[l for l in data['layers'] if l.get('tracks')]
  path.write_text(json.dumps(data,separators=(',',':'),allow_nan=False),encoding='utf8');report.append({'name':path.stem,'invalidSamplesRemoved':removed})
Path('.codex/html-parity/particle-data-validation.json').write_text(json.dumps(report,indent=2),encoding='utf8')
print(report)

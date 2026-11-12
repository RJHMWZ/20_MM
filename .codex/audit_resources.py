"""Find runtime Resources roots; Unity expands their serialized dependency closure."""
import json
from pathlib import Path
root=Path(__file__).resolve().parent.parent
assets=root/'Assets'
paths={}
for p in assets.rglob('*'):
    if not p.is_file() or p.suffix=='.meta': continue
    rel=p.relative_to(assets).as_posix()
    if '/Resources/' in '/'+rel:
        resource=('/'+rel).split('/Resources/',1)[1]
        resource=resource[:resource.rfind('.')]
        paths.setdefault(resource,[]).append(p.relative_to(root).as_posix())
roots=set()
def add(resource,required=True):
    matches=paths.get(resource,[])
    if required and not matches: raise ValueError('Missing runtime root: '+resource)
    roots.update(matches)
base='Reference20/'
for name in ['layout','background','ribbon','white']:
    add(base+name)
layout=json.loads((assets/'Resources/Reference20/layout.json').read_text(encoding='utf-8'))
for layer in layout['layers']:
    if layer['asset'] not in ['decoration','background']:add(base+layer['asset'])
for name in ['SpineGraphic','CocosSprite','graph','spine-scales','level-topology','sprite_0','sprite_1']:
    add(base+'Native/'+name)
def walk(value):
    if isinstance(value,list):
        for child in value:walk(child)
    elif isinstance(value,dict):
        for key,child in value.items():
            if key=='sprite' and isinstance(child,str) and child:add(base+'Native/'+child)
            elif key=='skeleton' and child:
                data=base+'Native/'+child
                if data+'_data' in paths:add(data+'_data')
                else:
                    add(data);add(data+'.atlas')
                    for i in range(8):add(data+'_tex'+str(i),False)
            else:walk(child)
for name in ['graph']:walk(json.loads((assets/('Resources/Reference20/Native/'+name+'.json')).read_text(encoding='utf-8')))
for p in (assets/'Resources/Reference20/Company').rglob('*'):
    if p.is_file() and p.suffix!='.meta':roots.add(p.relative_to(root).as_posix())
for name in ['chandelier','bush','rockchipper']:add(base+'Skins/'+name+'_SkeletonData')
for name in ['elements_sp_chugui_55','elements_sp_chugui_2']:add(base+'Skins/Cabinet/'+name)
for name in ['Bomb','Boom_fly_double_b','bomb_bomb','ColorBomb_ColorBomb']:add(base+'Props/'+name+'_SkeletonData')
add(base+'Props/Fragments/regions')
manifest=json.loads((assets/'Resources/Reference20/Props/Fragments/regions.json').read_text(encoding='utf-8'))
for sheet in manifest['sheets']:add(base+'Props/Fragments/'+sheet['name'])
roots.add('Assets/Scenes/Reference20.unity')
roots.add('Assets/Resources/DOTweenSettings.asset')
(root/'.codex/runtime-roots.json').write_text(json.dumps(sorted(roots),indent=2),encoding='utf-8')
print('Verified runtime roots:',len(roots))

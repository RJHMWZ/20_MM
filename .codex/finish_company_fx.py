from pathlib import Path
import json,re
root=Path(__file__).resolve().parent.parent
scripts=root/'Assets/Scripts/Reference20'
p=scripts/'Reference20Specials.cs';s=p.read_text(encoding='utf-8-sig').replace('Reference20Fragments.Burst(stage,Reference20Props.ColorComboCenter(position),"FX_ColorBomb_suipian_1",24,1.4f);','');p.write_text(s,encoding='utf-8')
p=scripts/'Reference20Runtime.cs';s=p.read_text(encoding='utf-8-sig');old='visual.localScale*=.8875f;if(id==12)';new='visual.localScale*=.8875f;Reference20CompanyEffects.Play("Wave",stage,Position(p),.4f);if(id==12)';assert old in s;s=s.replace(old,new);p.write_text(s,encoding='utf-8')
p=scripts/'Reference20Props.cs';s=p.read_text(encoding='utf-8-sig');s=re.sub(r'^ static readonly Dictionary<int,Material> particleMaterials[^\n]*\n','',s,flags=re.M);p.write_text(s,encoding='utf-8')
p=scripts/'Reference20Native.cs';s=p.read_text(encoding='utf-8-sig').replace('new[]{"CommonBornPropsEffect","SameColorEmit","SamecolorTargetShakeEffect"}','new[]{"SamecolorTargetShakeEffect"}');p.write_text(s,encoding='utf-8')
p=root/'Assets/Resources/Reference20/Native/graph.json';d=json.loads(p.read_text(encoding='utf-8'));d['effects']={k:v for k,v in d['effects'].items() if k not in ['CommonBornPropsEffect','SameColorEmit']};p.write_text(json.dumps(d,ensure_ascii=False,separators=(',',':')),encoding='utf-8')
p=root/'Assets/Resources/Reference20/Props/Fragments/regions.json';d=json.loads(p.read_text(encoding='utf-8'));d['sheets']=[s for s in d['sheets'] if s['name'] not in ['FX_Waffle_01_blend_blue','FX_Giftbox_sui01','FX_zhadan_sui01','FX_ColorBomb_suipian_1','FX_guantou01']];p.write_text(json.dumps(d,separators=(',',':')),encoding='utf-8')
for suffix in ['','.meta']:
 p=scripts/('Reference20UnityParticleGraphic.cs'+suffix)
 if p.exists():p.unlink()
p=root/'.codex/audit_resources.py';s=p.read_text(encoding='utf-8-sig');a=s.index("for name in ['Particles/Particles/Prefabs/Match3/ColorBomb_ColorBomb_2'");b=s.index("add(base+'Props/Fragments/regions')",a);s=s[:a]+s[b:];p.write_text(s,encoding='utf-8')
# Preserve animation atlas texels; scaling whole atlases would blur frames and bleed borders.
p=root/'luna.json';d=json.loads(p.read_text(encoding='utf-8'));overrides=d['assets']['rules']['texture']['overrides'];overrides[:]=[x for x in overrides if '/Company/FX/' not in x.get('name','')]
for atlas in (root/'Assets/Resources/Reference20/Company/FX').glob('*.png'):
 overrides.append({'name':atlas.relative_to(root).as_posix(),'maxWidth':2048,'maxHeight':2048,'format':'png32','compression':'quantSize','quality':85,'script':'','ext':''})
p.write_text(json.dumps(d,ensure_ascii=False,indent=4)+'\n',encoding='utf-8')
print('Removed replaced fragments and native particle baking; kept portable company effects.')

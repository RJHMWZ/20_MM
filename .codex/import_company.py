from pathlib import Path
import json,shutil,hashlib
from PIL import Image

root=Path(__file__).resolve().parent.parent
source=root/'.codex/company'
dst=root/'Assets/Resources/Reference20/Company'
(dst/'UI').mkdir(parents=True,exist_ok=True);(dst/'Audio').mkdir(exist_ok=True)
bundle=json.loads((source/'jsons.json').read_text(encoding='utf-8'))['assets/bundles/-1/bundle.json']
tree=json.loads((source/'panel-tree.json').read_text(encoding='utf-8'))
sprites={x['data'][0]:x for x in bundle['sprites']}
needed=['EnterLevel_Easy_PopUP','EnterLevel_Hard_title','Level_Easyl_PopUp_2','Loading_Logo_en']
for name in needed:
 texture=sprites[name]['data'][2]
 original=next(source.glob(f'assets_bundles_-1_{texture}.*.png'))
 Image.open(original).save(dst/'UI'/(name+'.png'))
(dst/'end-layout.json').write_text(json.dumps(tree,ensure_ascii=False,indent=2),encoding='utf-8')
shutil.copy2(root/'Assets/TextMesh Pro/Fonts/LiberationSans.ttf',dst/'UI/CompanyFont.ttf')
aliases={
 'btn_click':'match3_b_element_move','Audio_Match_WrongMatch':'match3_b_element_move',
 'Audio_UI_Win':'SFX_UI_M3_LevelComplete','Audio_UI_Fail':'SFX_UI_M3_LevelFail',
 'Audio_Match_LineCrushAppear':'match3_b_bomb_merge','Audio_Match_BombAppear':'match3_b_bomb_merge',
 'Audio_Match_RocketAppear':'match3_b_bomb_merge','Audio_Match_SameColorCrusherAppear':'match3_colorbomb_merge',
 'Audio_Match_LineCrusher':'match3_prop_03_hit','Audio_Match_AreaCrusher':'match3_bomb_hit',
 'Audio_Match_BigBomb_Shake':'match3_bomb_bomb_merge',
 'Audio_Match_Rocket_Flying':'match3_plane_fly','Audio_Match_Rocket_FlyRotate':'match3_plane_fly',
 'Audio_Match_Rocket_Explode':'match3_bomb_hit',
 'Audio_Match_SameColorCrusherExplode':'match3_supercolorbomb_supercolorbomb_crash',
 'Audio_Match_SameColorCrusherBoom':'match3_colorbomb_colorbomb_crash',
 'Audio_Match_LineAndAreaCrusher':'match3_bomb_cross_merge','Audio_Match_LineAndLineCrusher':'match3_prop_03_hit',
 'flyout_duang':'match3_element_dead_bear','shrubPeacock_duang':'match3_element_collect_fan',
 'Audio_Match_ElementsCracked':'match3_element_click_rockchipper','Audio_Match_HenEgg0':'match3_b_element_dead_normal_01',
 'screamChicken_break1':'match3_element_dead_bear','screamChicken_break2_0':'match3_element_dead_bear',
 'screamChicken_end':'match3_element_dead_bear',
}
combos={75064:1,75066:2,75068:3,75070:4,75072:5}
for i in range(1,6):aliases['Audio_Match_Base'+str(i)]='match_combo_'+str(i)
used=set(aliases.values())|{'match3_b_element_dead_normal_01','match3_element_dead_cupboard','match3_element_dead_cupboard_fishcan','match3_element_dead_woodchipper_end','match3_bomb_cross_merge'}
imported=[]
for sound in bundle['sounds']:
 name='match_combo_'+str(combos[sound['id']]) if sound['id'] in combos else sound['data'][0]
 if name not in used:continue
 original=source/'sounds'/(str(sound['id'])+'.mp3')
 target=dst/'Audio'/(name+'.mp3');shutil.copy2(original,target)
 imported.append({'name':name,'sourceId':sound['id'],'sourcePath':sound['path'],'sha256':hashlib.sha256(target.read_bytes()).hexdigest()})
assert used=={s['name'] for s in imported},used-{s['name'] for s in imported}
(dst/'audio-aliases.json').write_text(json.dumps(aliases,indent=2),encoding='utf-8')
(source/'import-provenance.json').write_text(json.dumps({'sourceHtml':'C:/Users/ivy/Desktop/樊思吾.html','audio':imported},ensure_ascii=False,indent=2),encoding='utf-8')
# Remove the previous third-party sound effects; no background music is in this folder.
legacy=(root/'Assets/Resources/Reference20/Native/Audio').resolve()
assert legacy.is_relative_to(root/'Assets/Resources/Reference20')
for p in legacy.iterdir():
 if p.is_file():p.unlink()
legacy.rmdir()
meta=Path(str(legacy)+'.meta')
if meta.exists():meta.unlink()
for name in ['match.mp3','wrong.mp3']:
 for suffix in ['', '.meta']:
  p=root/'Assets/Resources/Reference20'/(name+suffix)
  if p.exists():p.unlink()
print('Imported',len(imported),'company SFX, 4 UI images and font; Gameplay music excluded.')

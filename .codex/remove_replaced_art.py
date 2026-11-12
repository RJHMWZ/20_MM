from pathlib import Path
import json,shutil
root=Path(__file__).resolve().parent.parent
assets=(root/'Assets').resolve()
files=[]
for pattern in ['Resources/Reference20/Props/Boom_fly_double_b*','Resources/Reference20/Props/brick*','Scripts/Reference20/Reference20SpecialVisual.cs*','Resources/Reference20/Company/UI/EnterLevel_Easy_PopUP.png*','Resources/Reference20/Company/UI/EnterLevel_Hard_title.png*']:
 files.extend(assets.glob(pattern))
for p in files:
 if not p.resolve().is_relative_to(assets) or not p.is_file():raise ValueError(p)
folders=[root/'.codex/replacement/excluded-assets',root/'.codex/props']
for p in folders:
 if not p.resolve().is_relative_to((root/'.codex').resolve()):raise ValueError(p)
report={'files':len(files),'bytes':sum(p.stat().st_size for p in files),'deleted':[p.relative_to(root).as_posix() for p in files]}
for p in folders:
 if p.exists():
  members=[x for x in p.rglob('*') if x.is_file()];report['files']+=len(members);report['bytes']+=sum(x.stat().st_size for x in members);report['deleted'].append(p.relative_to(root).as_posix()+'/')
print('Validated obsolete art targets:',report['files'],'files,',round(report['bytes']/1024**2,2),'MiB')
for p in files:p.unlink()
for p in folders:
 if p.exists():shutil.rmtree(p)
(root/'.codex/html-parity/deleted-replaced-art.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf8')

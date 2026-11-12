import json,sys
from pathlib import Path
root=Path(__file__).resolve().parent.parent
assets=(root/'Assets').resolve()
keep=set(json.loads((root/'.codex/asset-keep.json').read_text(encoding='utf-8-sig')))
# Retain the complete editor/export/build tools and their source/plugin libraries.
protected=['Assets/Editor/','Assets/Scripts/','Assets/Plugins/','Assets/Spine/','Assets/UIParticle/']
protected_files=['Assets/TextMesh Pro/Fonts/LiberationSans.ttf','Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt','Assets/SVC_Luna.asset']
files=[]
for p in assets.rglob('*'):
    if not p.is_file() or p.suffix=='.meta':continue
    rel=p.relative_to(root).as_posix()
    if p.suffix.lower() in ['.py','.ps1','.cs','.dll','.asmdef','.asmref','.js','.bat','.cmd','.sh'] or rel in keep or rel in protected_files or any(rel.startswith(prefix) for prefix in protected):continue
    files.append(p)
report={'files':len(files),'bytes':sum(p.stat().st_size for p in files),'paths':[p.relative_to(root).as_posix() for p in files]}
(root/'.codex/removed-assets.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print('Unused assets:',len(files),'size MB:',round(report['bytes']/1024**2,2))
if '--delete' in sys.argv:
    # Validate every resolved target before deletion. No backups are created.
    for p in files:
        target=p.resolve()
        if not target.is_relative_to(assets):raise ValueError(target)
        target.unlink()
        meta=Path(str(target)+'.meta')
        if meta.exists():meta.unlink()
    for folder in sorted(assets.rglob('*'),key=lambda p:len(p.parts),reverse=True):
        if folder.is_dir() and not any(folder.iterdir()) and not any(folder.relative_to(root).as_posix().startswith(s.rstrip('/')) for s in protected):
            folder.rmdir()
            meta=Path(str(folder)+'.meta')
            if meta.exists():meta.unlink()
    print('Deleted unused assets and metadata; export/build tools retained.')

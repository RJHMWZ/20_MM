from pathlib import Path
import re,json
root=Path(__file__).resolve().parent.parent
p=root/'Assets/Scripts/Reference20/Reference20Runtime.cs'
s=p.read_text(encoding='utf-8-sig').replace('if(p.id==100016){if(actor)actor.AnimateDamage(before,after);Sound("Audio_Match_ElementsCracked");','if(p.id==100016){if(actor)actor.AnimateDamage(before,after);Sound("match3_element_dead_cupboard");')
s=s.replace('void Release(Piece p){if(!p.root)return;p.root.DOKill();','void Release(Piece p){if(!p.root)return;if(p.id==100059)Sound("match3_element_dead_woodchipper_end");p.root.DOKill();')
p.write_text(s,encoding='utf-8')
# Preserve the source HTML's initial web loader as project-owned build branding.
source=Path('C:/Users/ivy/Desktop/樊思吾.html').read_text(encoding='utf-8-sig')
styles=[m[1] for m in re.findall(r'<style([^>]*)>(.*?)</style>',source,re.S) if '.preloader{' in m[1]]
assert styles
dst=root/'Assets/Editor/CompanyBranding';dst.mkdir(exist_ok=True)
(dst/'preloader.css').write_text('\n'.join(styles),encoding='utf-8')
# The replacement tail no longer uses the old reference's end graph/skeletons.
p=root/'Assets/Scripts/Reference20/Reference20Native.cs';s=p.read_text(encoding='utf-8-sig')
s=re.sub(r'^public class NativeEndGraph[^\n]*\n','',s,flags=re.M)
s=re.sub(r'^ public static RectTransform EndArtwork\([^\n]*\n','',s,flags=re.M)
p.write_text(s,encoding='utf-8')
p=root/'.codex/audit_resources.py';s=p.read_text(encoding='utf-8-sig')
s=s.replace("['layout','background','ribbon','white','endcard','match','wrong']","['layout','background','ribbon','white']")
s=s.replace("['SpineGraphic','CocosSprite','graph','end','spine-scales'","['SpineGraphic','CocosSprite','graph','spine-scales'")
s=s.replace("for name in ['graph','end']:walk", "for name in ['graph']:walk")
start=s.index("for p in (assets/'Resources/Reference20/Native/Audio').glob('*'):")
end=s.index("for name in ['chandelier'",start)
s=s[:start]+"for p in (assets/'Resources/Reference20/Company').rglob('*'):\n    if p.is_file() and p.suffix!='.meta':roots.add(p.relative_to(root).as_posix())\n"+s[end:]
p.write_text(s,encoding='utf-8')
print('Updated company audio, web preloader, resource audit and removed old tail loader code.')

from pathlib import Path
import re,json
root=Path(__file__).resolve().parent.parent
scripts=root/'Assets/Scripts/Reference20'
def change(name,func):
 p=scripts/name;p.write_text(func(p.read_text(encoding='utf-8-sig')),encoding='utf-8')
def native(s):
 s=s.replace('Reference20Props.Reset();Reference20Fragments.Reset();','Reference20Props.Reset();Reference20Fragments.Reset();Reference20CompanyEffects.Reset();')
 s=s.replace('result=Resources.Load<Sprite>("Reference20/Native/"+name);','result=Resources.Load<Sprite>(name=="sprite_0"?"Reference20/Company/Elements/brick2_0":name=="sprite_1"?"Reference20/Company/Elements/brick4_0":"Reference20/Native/"+name);')
 s=re.sub(r'^ public static void BaseBurst\([^\n]*', ' public static void BaseBurst(RectTransform stage,Vector2 position,int color){Reference20CompanyEffects.Play(color==4?"NormalGold":"NormalBlue",stage,position);}',s,flags=re.M)
 s=re.sub(r'^ public static void ColorBeam\([^\n]*', ' public static void ColorBeam(RectTransform stage,Vector2 from,Vector2 to,float delay){DOTween.Sequence().SetTarget(stage).AppendInterval(delay).AppendCallback(()=>Reference20CompanyEffects.Beam(stage,from,to));}',s,flags=re.M)
 s=s.replace('public static void FinishEffect(RectTransform rt){if(!rt)return;','public static void FinishEffect(RectTransform rt){if(!rt)return;foreach(var effect in rt.GetComponentsInChildren<Reference20CompanyFlipbook>(true))effect.Stop();')
 s=s.replace('RectTransform own;if(Reference20Props.TryEffect','RectTransform own;if(name=="CommonBornPropsEffect")return Reference20CompanyEffects.Play("Wave",stage,position,.55f);if(Reference20Props.TryEffect')
 return s
change('Reference20Native.cs',native)
def props(s):
 s=s.replace('if(Application.isPlaying){foreach(bool color in new[]{false,true}){var halo=ParticleHalo(stage,Vector2.zero,color);halo.GetComponent<Reference20EffectInstance>().Complete();}}','Reference20CompanyEffects.Prewarm();')
 start=s.index(' public static RectTransform ParticleHalo(')
 s=s[:start]+''' public static RectTransform ParticleHalo(RectTransform stage,Vector2 position,bool color,float scale=1){return Reference20CompanyEffects.Play(color?"ColorFinish":"Bomb",stage,position,scale);}
}
}
'''
 s=s.replace('Reference20Fragments.Burst(stage,position,"FX_zhadan_sui01",14,1);','')
 s=s.replace('root=SpineEffect(name,stage,position,"Bomb","brick_color","Boom_color_start",life>0?life:3600);','{root=SpineEffect(name,stage,position,"Bomb","brick_color","Boom_color_start",life>0?life:3600);var charge=Reference20CompanyEffects.Play("ColorCharge",stage,position,1,true);if(charge){charge.SetParent(root,false);charge.anchorMin=charge.anchorMax=Vector2.one*.5f;charge.anchoredPosition=Vector2.zero;}}')
 s=s.replace('ParticleHalo(stage,position,false,.5f);','ParticleHalo(stage,position,false,1);')
 s=s.replace('t.localScale=Vector3.one*VisualScale;t.DOAnchorPos','t.localScale=Vector3.one*VisualScale;var trail=Reference20CompanyEffects.Play("RocketTrail",stage,Vector2.zero,.6f);if(trail){trail.SetParent(t,false);trail.anchorMin=trail.anchorMax=Vector2.one*.5f;trail.anchoredPosition=Vector2.zero;trail.localRotation=Quaternion.Euler(0,0,horizontal?direction>0?180:0:direction>0?90:-90);}t.DOAnchorPos')
 return s
change('Reference20Props.cs',props)
change('Reference20Specials.cs',lambda s:s.replace('Reference20Fragments.Burst(stage,CellPosition(x,y),"FX_zhadan_sui01",24,1.15f);','').replace('Sound("Audio_Match_Rocket_Explode");','Reference20CompanyEffects.Play("BeeImpact",stage,CellPosition(x,y),.7f);Sound("Audio_Match_Rocket_Explode");'))
# Production uses the two requested colors only.
for name in ['brick1_0','brick3_0']:
 for suffix in ['.png','.png.meta']:
  p=root/'Assets/Resources/Reference20/Company/Elements'/(name+suffix)
  if p.exists():p.unlink()
# Both exported color bursts use the same source prefab. Share the atlas to save memory.
p=root/'Assets/Resources/Reference20/Company/FX/manifest.json';manifest=json.loads(p.read_text(encoding='utf-8'))
finish=next(e for e in manifest['effects'] if e['name']=='ColorFinish')
for effect in manifest['effects']:
 if effect['name']=='ColorCombo':effect.update({k:v for k,v in finish.items() if k!='name'});effect['texture']='ColorFinish'
p.write_text(json.dumps(manifest,indent=2),encoding='utf-8')
for suffix in ['.png','.png.meta']:
 p=root/'Assets/Resources/Reference20/Company/FX'/('ColorCombo'+suffix)
 if p.exists():p.unlink()
print('Connected company normal clears, sparks, halos, rocket trails, color charge/beam and bee impacts.')

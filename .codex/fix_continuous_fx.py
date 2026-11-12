from pathlib import Path
base=Path('Assets/Scripts/Reference20')
(base/'Reference20CompanyEffects.cs').write_text('''using UnityEngine;
namespace Reference20 {
public static class Reference20CompanyEffects {
 public static void Prewarm(){Reference20CompanyParticles.Load();}
 public static void Reset(){Reference20CompanyParticles.Reset();}
 public static RectTransform Play(string name,RectTransform stage,Vector2 position,float scale=1,bool loop=false){return Reference20CompanyParticles.Play(name,stage,position,scale,loop);}
 public static void Beam(RectTransform stage,Vector2 from,Vector2 to){Reference20CompanyParticles.Beam(stage,from,to);}
}
}
''',encoding='utf-8')
for name in ['Reference20Native.cs','Reference20EffectInstance.cs']:
 p=base/name;s=p.read_text(encoding='utf-8-sig');s=s.replace('foreach(var effect in rt.GetComponentsInChildren<Reference20CompanyFlipbook>(true))effect.Stop();','Reference20CompanyParticles.StopChildren(rt);').replace('foreach(var effect in GetComponentsInChildren<Reference20CompanyFlipbook>(true))effect.Stop();','Reference20CompanyParticles.StopChildren(transform);');p.write_text(s,encoding='utf-8')
p=base/'Reference20Runtime.cs';s=p.read_text(encoding='utf-8-sig');s=s.replace('OnComplete(()=>ReturnCompose(ghost));ClearNormal(p,false);','OnComplete(()=>ReturnCompose(ghost));ClearNormal(p);');p.write_text(s,encoding='utf-8')
p=base/'Reference20Specials.cs';s=p.read_text(encoding='utf-8-sig');start=s.index('  // Source endpoints');end=s.index('Sound("Audio_Match_LineCrusher");',start)
s=s[:start]+'''  const float flightDuration=1.5f;const float cellInterval=.1f;
  float clearDuration=(horizontal?9:12)*cellInterval;LockCells(cells,clearDuration);
  Reference20Native.LineBlast(stage,CellPosition(x,y),horizontal,flightDuration);HitCell(x,y,hits);
  for(int i=1;i<(horizontal?9:12);i++){int offset=i;After(cellInterval*i,()=>{if(horizontal){HitCell(x+offset,y,hits);HitCell(x-offset,y,hits);}else{HitCell(x,y+offset,hits);HitCell(x,y-offset,hits);}});}'''+s[end:]
s=s.replace('Reference20CompanyEffects.Play("ColorFinish",stage,position,.7f);','Reference20CompanyEffects.Play("Wave",stage,position,.5f);')
p.write_text(s,encoding='utf-8')
p=base/'Reference20Props.cs';s=p.read_text(encoding='utf-8-sig').replace('Reference20CompanyEffects.Prewarm();','Reference20CompanyParticles.Prewarm(stage);');start=s.index(' public static void LineBlast(');end=s.index(' public static RectTransform BeeFlight',start)
s=s[:start]+''' public static void LineBlast(RectTransform stage,Vector2 position,bool horizontal,float duration){
  string skin=horizontal?"brick0_heng":"brick0_shu";
  var root=EffectRoot(horizontal?"CompanyLineH":"CompanyLineV",stage,position,duration,r=>{Rig("Bomb",r,skin,null);Rig("Bomb",r,skin,null);},r=>{
   r.localScale=Vector3.one;int direction=-1;
   foreach(var sg in r.GetComponentsInChildren<SkeletonGraphic>()){
    sg.Skeleton.SetToSetupPose();sg.Skeleton.SetSkin(skin);sg.Skeleton.SetSlotsToSetupPose();sg.AnimationState.ClearTracks();sg.AnimationState.SetAnimation(0,"Bomb_still",false);sg.Update(0);sg.LateUpdate();
    var t=sg.rectTransform;t.anchoredPosition=Vector2.zero;t.localRotation=Quaternion.identity;t.localScale=Vector3.one*VisualScale;
    var trail=Reference20CompanyEffects.Play("RocketTrail",stage,Vector2.zero,.85f,true);
    trail.SetParent(t,false);trail.anchorMin=trail.anchorMax=Vector2.one*.5f;trail.anchoredPosition=Vector2.zero;
    trail.localRotation=Quaternion.Euler(0,0,horizontal?direction<0?180:0:direction>0?90:-90);
    t.DOAnchorPos((horizontal?Vector2.right:Vector2.up)*direction*(113.6f*10*duration),duration).SetEase(Ease.Linear);direction=1;
   }
  });
 }
'''+s[end:]
p.write_text(s,encoding='utf-8')

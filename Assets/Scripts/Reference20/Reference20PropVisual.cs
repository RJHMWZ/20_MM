using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace Reference20 {
public sealed class Reference20PropVisual : MonoBehaviour {
 public int Kind;
 public SkeletonGraphic Rig;
 bool hinting;
 public void ResetPose(bool born){
  hinting=false;Rig.timeScale=1;Rig.AnimationState.ClearTracks();Rig.Skeleton.SetToSetupPose();
  string skin=Kind==13?"bomb_cross":Kind==14?"bomb_square":Kind==15?"brick_color":"brick0_heng";
  Rig.Skeleton.SetSkin(skin);Rig.Skeleton.SetSlotsToSetupPose();
  // Birth uses the same pose and dimensions as idle; only opacity changes.
  Play("Bomb_still");
  var fade=GetComponent<CanvasGroup>();if(!fade)fade=gameObject.AddComponent<CanvasGroup>();fade.DOKill();fade.alpha=born?0:1;if(born)fade.DOFade(1,.16f);
  Rig.Update(0);Rig.LateUpdate();
 }
 void Play(string name,bool loop=false){Rig.AnimationState.SetAnimation(0,name,loop);}
 void IdleAfter(){Rig.AnimationState.AddAnimation(0,"Bomb_still",false,0);}
 public void Hint(string name){hinting=true;string suffix=name.EndsWith("right")?"_r":name.EndsWith("left")?"_l":name.EndsWith("up")?"_u":name.EndsWith("down")?"_d":"";Play("Bomb_hint"+suffix,true);}
 public void StopHint(){if(!hinting)return;hinting=false;Play("Bomb_still");}
 public void Falling(){hinting=false;Play("falling_d");}
 public void Landed(){hinting=false;Play("Bomb_dropped_d");IdleAfter();}
}
}

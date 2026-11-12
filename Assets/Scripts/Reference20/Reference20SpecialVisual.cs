using UnityEngine;
using Spine.Unity;

namespace Reference20 {
// This skeleton's idle clips animate bones only: keep the completed birth pose
// underneath them, including the slot attachments absent from its setup pose.
public sealed class Reference20SpecialVisual : MonoBehaviour {
 SkeletonGraphic skeleton;float elapsed,duration;bool idle;
 public void Play(SkeletonGraphic value){skeleton=value;elapsed=0;idle=false;skeleton.AnimationState.ClearTracks();skeleton.Skeleton.SetToSetupPose();var birth=skeleton.Skeleton.Data.FindAnimation("synth");duration=birth==null?0:birth.Duration;if(birth!=null)skeleton.AnimationState.SetAnimation(0,birth,false);}
 void Update(){if(idle||!skeleton)return;elapsed+=Time.deltaTime;if(elapsed<duration)return;idle=true;if(skeleton.Skeleton.Data.FindAnimation("normal")!=null)skeleton.AnimationState.SetAnimation(1,"normal",true);}
}
}

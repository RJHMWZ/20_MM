using System;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

namespace Reference20 {
public sealed class Reference20EffectInstance : MonoBehaviour {
 struct Pose {public Transform transform;public Vector3 position,scale;public Quaternion rotation;public bool active;public CanvasGroup alpha;public float opacity;}
 struct SpinePose {public SkeletonGraphic skeleton;public string animation;public bool loop;}
 public string PoolKey;
 Pose[] poses;SpinePose[] skeletons;Reference20CocosClip[] clips;Reference20ParticleGraphic[] particles;Reference20Particle3DGraphic[] particles3d;float remaining,rootOpacity;Action<Reference20EffectInstance> release;
 public void Capture(){var rootAlpha=GetComponent<CanvasGroup>();rootOpacity=rootAlpha?rootAlpha.alpha:1;var nodes=GetComponentsInChildren<Transform>(true);poses=new Pose[nodes.Length];for(int i=0;i<nodes.Length;i++){var t=nodes[i];var alpha=t.GetComponent<CanvasGroup>();poses[i]=new Pose{transform=t,position=t.localPosition,scale=t.localScale,rotation=t.localRotation,active=t.gameObject.activeSelf,alpha=alpha,opacity=alpha?alpha.alpha:1};}clips=GetComponentsInChildren<Reference20CocosClip>(true);particles=GetComponentsInChildren<Reference20ParticleGraphic>(true);particles3d=GetComponentsInChildren<Reference20Particle3DGraphic>(true);var spine=GetComponentsInChildren<SkeletonGraphic>(true);skeletons=new SpinePose[spine.Length];for(int i=0;i<spine.Length;i++){var track=spine[i].AnimationState.GetCurrent(0);skeletons[i]=new SpinePose{skeleton=spine[i],animation=track==null?null:track.Animation.Name,loop=track!=null&&track.Loop};}}
 public void Restore(){foreach(var clip in clips)clip.Stop(false);foreach(var p in poses){if(!p.transform)continue;p.transform.DOKill();p.transform.localPosition=p.position;p.transform.localScale=p.scale;p.transform.localRotation=p.rotation;p.transform.gameObject.SetActive(p.active);if(p.alpha)p.alpha.alpha=p.opacity;}foreach(var pose in skeletons){var skeleton=pose.skeleton;skeleton.AnimationState.ClearTracks();skeleton.Skeleton.SetToSetupPose();if(!string.IsNullOrEmpty(pose.animation))skeleton.AnimationState.SetAnimation(0,pose.animation,pose.loop);}foreach(var emitter in particles)emitter.Restart();foreach(var emitter in particles3d)emitter.Restart();var rootAlpha=GetComponent<CanvasGroup>();if(rootAlpha)rootAlpha.alpha=rootOpacity;remaining=0;release=null;}
 public void Lease(float seconds,Action<Reference20EffectInstance> complete){remaining=seconds;release=complete;gameObject.SetActive(true);}
 void Update(){if(release==null)return;remaining-=Time.deltaTime;if(remaining<=0)Complete();}
 public void Complete(){if(release==null)return;var complete=release;release=null;foreach(var effect in GetComponentsInChildren<Reference20CompanyFlipbook>(true))effect.Stop();foreach(var pose in poses)if(pose.transform)pose.transform.DOKill();gameObject.SetActive(false);complete(this);}
}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class CocosParticleRange {
 public int mode;public float[] min,max;
 public float Evaluate(float t,float random){if(min==null||min.Length==0)return 0;float frame=Mathf.Clamp01(t)*(min.Length-1);int a=(int)frame,b=Mathf.Min(a+1,min.Length-1);return Mathf.LerpUnclamped(Mathf.LerpUnclamped(min[a],min[b],frame-a),Mathf.LerpUnclamped(max[a],max[b],frame-a),random);}
}
[Serializable] public sealed class CocosParticleGradient {
 public int mode;public float[] times;public float[][] min,max,randomColors;
 static Color ColorAt(float[] c){return new Color(c[0],c[1],c[2],c[3]);}
 public Color Evaluate(float t,float random){if(times==null||times.Length==0)return Color.white;if(mode==4&&randomColors!=null&&randomColors.Length>0)return ColorAt(randomColors[Mathf.Min(randomColors.Length-1,(int)(random*randomColors.Length))]);int a=0;while(a+1<times.Length&&times[a+1]<=t)a++;int b=Mathf.Min(a+1,times.Length-1);float f=times[b]>times[a]?Mathf.Clamp01((t-times[a])/(times[b]-times[a])):0;return Color.LerpUnclamped(Color.LerpUnclamped(ColorAt(min[a]),ColorAt(min[b]),f),Color.LerpUnclamped(ColorAt(max[a]),ColorAt(max[b]),f),random);}
}
[Serializable] public sealed class CocosParticleModule {
 public bool separateAxes;public int numTilesX=1,numTilesY=1,animation,rowIndex;public bool randomRow;public float dampen,cycleCount=1,flipU,flipV;
 public CocosParticleRange size,x,y,z,limit,limitX,limitY,limitZ,frameOverTime,startFrame;public CocosParticleGradient color;
}
[Serializable] public sealed class CocosParticleBurst {public float time,interval;public int repeat;public CocosParticleRange count;}
[Serializable] public sealed class CocosParticle3DData {
 public float duration,speed=1,velocityScale,lengthScale;public int capacity,renderMode,src=770,dst=771;public bool loop,prewarm;public string sprite;public float[] tint,tiling;public Vector3 rotation;public float[][] samples;public CocosParticleBurst[] bursts;
 public CocosParticleRange startDelay,startLifetime,startSize,startSpeed,startRotation,gravityModifier,rateOverTime,rateOverDistance;public CocosParticleGradient color;
 public CocosParticleModule colorOverLifetime,sizeOvertime,limitVelocityOvertime,rotationOvertime,textureAnimation;
}
// Original Cocos 3D billboard emitters projected into the board Canvas. Curves,
// shape samples, texture sheets and blend factors come from the HTML prefabs.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class Reference20Particle3DGraphic : MaskableGraphic {
 public Reference20Particle3DGraphic(){useLegacyMeshGeneration=false;}
 struct Particle {public Vector3 position,velocity,actualVelocity;public float age,life,size,rotation,random;public Color color;}
 readonly List<Particle> particles=new List<Particle>();static int seedCounter;readonly System.Random random=new System.Random(unchecked(Environment.TickCount*397+(++seedCounter)));readonly UIVertex[] quad=new UIVertex[4];
 CocosParticle3DData data;Sprite sprite;float time,emission,startDelay;Vector3 lastPosition;
 public override Texture mainTexture {get{return sprite?sprite.texture:Texture2D.whiteTexture;}}
 float Random01(){return (float)random.NextDouble();}
 public void Configure(CocosParticle3DData value,Sprite image){data=value;sprite=image;raycastTarget=false;particles.Capacity=Mathf.Max(particles.Capacity,value.capacity);Restart();}
 protected override void OnEnable(){base.OnEnable();Restart();}
 public void Restart(){time=emission=0;particles.Clear();lastPosition=transform.position;if(data!=null){startDelay=data.startDelay.Evaluate(0,Random01());if(data.prewarm&&data.loop){int steps=Mathf.Min(120,Mathf.CeilToInt(data.duration*30));for(int i=0;i<steps;i++)Advance(1f/30f);}}SetVerticesDirty();}
 void Emit(float normalizedTime){if(particles.Count>=data.capacity)return;float r=Random01();var position=Vector3.zero;var direction=Vector3.forward;if(data.samples!=null&&data.samples.Length>0){var sample=data.samples[random.Next(data.samples.Length)];position=new Vector3(sample[0],sample[1],sample[2]);direction=new Vector3(sample[3],sample[4],sample[5]);}particles.Add(new Particle{position=position,velocity=direction*data.startSpeed.Evaluate(normalizedTime,r),life=Mathf.Max(.001f,data.startLifetime.Evaluate(normalizedTime,r)),size=Mathf.Max(0,data.startSize.Evaluate(normalizedTime,r)),rotation=data.startRotation.Evaluate(normalizedTime,r),random=r,color=data.color.Evaluate(normalizedTime,r)});}
 void Update(){if(data==null)return;Advance(Time.deltaTime*data.speed);SetVerticesDirty();}
 void Advance(float dt){float before=time-startDelay;time+=dt;float after=time-startDelay;float duration=Mathf.Max(.001f,data.duration);if(after>=0&&(data.loop||before<duration)){float end=data.loop?after:Mathf.Min(after,duration),begin=Mathf.Max(0,before);float normalized=data.loop?Mathf.Repeat(end,duration)/duration:Mathf.Clamp01(end/duration);emission+=Mathf.Max(0,end-begin)*data.rateOverTime.Evaluate(normalized,Random01());if(data.rateOverDistance!=null)emission+=Vector3.Distance(lastPosition,transform.position)*data.rateOverDistance.Evaluate(normalized,Random01());lastPosition=transform.position;while(emission>=1){emission--;Emit(normalized);}if(data.bursts!=null)foreach(var burst in data.bursts){int first=data.loop?Mathf.Max(0,Mathf.FloorToInt(begin/duration)):0,last=data.loop?Mathf.FloorToInt(end/duration):0;for(int cycle=first;cycle<=last;cycle++)for(int repeat=0;repeat<Mathf.Max(1,burst.repeat);repeat++){float at=cycle*duration+burst.time+repeat*burst.interval;if(at>=Mathf.Max(0,before)&&at<after){int count=Mathf.Max(0,Mathf.RoundToInt(burst.count.Evaluate(normalized,Random01())));for(int i=0;i<count;i++)Emit(normalized);}}}}
  for(int i=particles.Count-1;i>=0;i--){var p=particles[i];p.age+=dt;if(p.age>=p.life){particles.RemoveAt(i);continue;}float life=p.age/p.life;p.velocity+=Vector3.down*(9.8f*data.gravityModifier.Evaluate(life,p.random)*dt);var limit=data.limitVelocityOvertime;var velocity=p.velocity;if(limit!=null){if(limit.separateAxes){velocity.x=Dampen(velocity.x,limit.limitX.Evaluate(life,p.random),limit.dampen);velocity.y=Dampen(velocity.y,limit.limitY.Evaluate(life,p.random),limit.dampen);velocity.z=Dampen(velocity.z,limit.limitZ.Evaluate(life,p.random),limit.dampen);}else{float max=limit.limit==null?float.PositiveInfinity:Mathf.Max(0,limit.limit.Evaluate(life,p.random));if(velocity.magnitude>max)velocity=Vector3.LerpUnclamped(velocity,velocity.normalized*max,limit.dampen);}}p.actualVelocity=velocity;p.position+=velocity*dt;var rotation=data.rotationOvertime;if(rotation!=null&&rotation.z!=null)p.rotation+=rotation.z.Evaluate(life,p.random)*dt;particles[i]=p;}
 }
 static float Dampen(float value,float max,float dampen){return Mathf.Abs(value)>max?Mathf.LerpUnclamped(value,Mathf.Sign(value)*max,dampen):value;}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(data==null||!sprite)return;var sheet=data.textureAnimation;int columns=sheet==null?1:Mathf.Max(1,sheet.numTilesX),rows=sheet==null?1:Mathf.Max(1,sheet.numTilesY);Rect rect=sprite.textureRect;foreach(var p in particles){float life=p.age/p.life;Vector2 size=Vector2.one*p.size;var sizes=data.sizeOvertime;if(sizes!=null){if(sizes.separateAxes)size=new Vector2(size.x*sizes.x.Evaluate(life,p.random),size.y*sizes.y.Evaluate(life,p.random));else size*=sizes.size.Evaluate(life,p.random);}float angle=p.rotation;Vector3 position=p.position;var scale=transform.lossyScale;var worldVelocity=transform.TransformVector(p.actualVelocity);if(data.renderMode==1&&p.actualVelocity.sqrMagnitude>.00001f){angle=Mathf.Atan2(worldVelocity.y,worldVelocity.x)-Mathf.PI*.5f;size.y=size.y*data.lengthScale+p.actualVelocity.magnitude*data.velocityScale;}var right=transform.InverseTransformVector(new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0)*size.x*.5f*Mathf.Abs(scale.x));var up=transform.InverseTransformVector(new Vector3(-Mathf.Sin(angle),Mathf.Cos(angle),0)*size.y*.5f*Mathf.Abs(scale.y));Color tint=p.color*color;if(data.colorOverLifetime!=null)tint*=data.colorOverLifetime.color.Evaluate(life,p.random);int frame=0;if(sheet!=null&&sheet.frameOverTime!=null){float value=sheet.frameOverTime.Evaluate(life,p.random)*sheet.cycleCount;if(sheet.startFrame!=null)value+=sheet.startFrame.Evaluate(0,p.random)/(columns*rows);frame=Mathf.FloorToInt(Mathf.Repeat(value,1)*columns*rows);}int col=frame%columns,row=frame/columns;Vector4 uv=new Vector4((rect.x+col*rect.width/columns)/mainTexture.width,(rect.y+(rows-row-1)*rect.height/rows)/mainTexture.height,(rect.x+(col+1)*rect.width/columns)/mainTexture.width,(rect.y+(rows-row)*rect.height/rows)/mainTexture.height);for(int i=0;i<4;i++){quad[i]=UIVertex.simpleVert;quad[i].color=tint;}quad[0].position=position-right-up;quad[1].position=position-right+up;quad[2].position=position+right+up;quad[3].position=position+right-up;quad[0].uv0=new Vector2(uv.x,uv.y);quad[1].uv0=new Vector2(uv.x,uv.w);quad[2].uv0=new Vector2(uv.z,uv.w);quad[3].uv0=new Vector2(uv.z,uv.y);vh.AddUIVertexQuad(quad);}}
}
}

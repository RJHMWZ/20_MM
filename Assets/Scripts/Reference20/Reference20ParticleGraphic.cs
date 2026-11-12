using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20 {
[Serializable] public sealed class Reference20ParticleData {
 public int totalParticles,emitterMode,positionType;
 public float duration,emissionRate,life,lifeVar,angle,angleVar,startSize,startSizeVar,endSize,endSizeVar,startSpin,startSpinVar,endSpin,endSpinVar,speed,speedVar,radialAccel,radialAccelVar,tangentialAccel,tangentialAccelVar,startRadius,startRadiusVar,endRadius,endRadiusVar,rotatePerS,rotatePerSVar;
 public bool rotationIsDir;public Vector2 posVar,sourcePos,gravity;public float[] startColor,startColorVar,endColor,endColorVar;public string sprite;
}
// Cocos gravity/radius emitters rendered in the same Canvas as the original art.
// Visual randomness is independent of the level's deterministic drop generator.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class Reference20ParticleGraphic : MaskableGraphic {
 struct Particle {public Vector2 position,velocity,origin;public float age,life,startSize,endSize,startSpin,endSpin,radial,tangential,angle,radius,endRadius,angular;public Color startColor,endColor;}
 Reference20ParticleData data;Sprite sprite;readonly List<Particle> particles=new List<Particle>();static int seedCounter;readonly System.Random random=new System.Random(unchecked(Environment.TickCount*397+(++seedCounter)));float elapsed,emitted;
 readonly UIVertex[] quad=new UIVertex[4];
 public override Texture mainTexture {get{return sprite?sprite.texture:Texture2D.whiteTexture;}}
 public void Configure(Reference20ParticleData value,Sprite texture){data=value;sprite=texture;raycastTarget=false;particles.Capacity=Mathf.Max(particles.Capacity,value.totalParticles);Restart();}
 protected override void OnEnable(){base.OnEnable();Restart();}
 public void Restart(){particles.Clear();elapsed=emitted=0;SetVerticesDirty();}
 float V(float value,float variance){return value+((float)random.NextDouble()*2-1)*variance;}
 Color C(float[] value,float[] variance){return new Color(Mathf.Clamp01(V(value[0],variance[0])),Mathf.Clamp01(V(value[1],variance[1])),Mathf.Clamp01(V(value[2],variance[2])),Mathf.Clamp01(V(value[3],variance[3])));}
 void Emit(){float life=Mathf.Max(.001f,V(data.life,data.lifeVar));float angle=V(data.angle,data.angleVar)*Mathf.Deg2Rad;Vector2 velocity=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*V(data.speed,data.speedVar);var origin=data.sourcePos+new Vector2(V(0,data.posVar.x),V(0,data.posVar.y));float size=Mathf.Max(0,V(data.startSize,data.startSizeVar));float radius=V(data.startRadius,data.startRadiusVar);particles.Add(new Particle{life=life,position=origin,origin=origin,velocity=velocity,startSize=size,endSize=data.endSize==-1?size:Mathf.Max(0,V(data.endSize,data.endSizeVar)),startSpin=data.rotationIsDir?-angle*Mathf.Rad2Deg:V(data.startSpin,data.startSpinVar),endSpin=V(data.endSpin,data.endSpinVar),radial=V(data.radialAccel,data.radialAccelVar),tangential=V(data.tangentialAccel,data.tangentialAccelVar),startColor=C(data.startColor,data.startColorVar),endColor=C(data.endColor,data.endColorVar),angle=angle,radius=radius,endRadius=data.endRadius==-1?radius:V(data.endRadius,data.endRadiusVar),angular=V(data.rotatePerS,data.rotatePerSVar)*Mathf.Deg2Rad});}
 void Update(){if(data==null)return;float dt=Time.deltaTime;float emissionTime=data.duration<0?dt:Mathf.Min(dt,Mathf.Max(0,data.duration-elapsed));elapsed+=dt;emitted+=emissionTime*data.emissionRate;while(emitted>=1){emitted--;if(particles.Count<data.totalParticles)Emit();}for(int i=particles.Count-1;i>=0;i--){var p=particles[i];p.age+=dt;if(p.age>=p.life){particles.RemoveAt(i);continue;}if(data.emitterMode==0){var radial=(p.position-p.origin).normalized;var tangential=new Vector2(-radial.y,radial.x);p.velocity+=(data.gravity+radial*p.radial+tangential*p.tangential)*dt;p.position+=p.velocity*dt;}else{p.angle+=p.angular*dt;float radius=Mathf.Lerp(p.radius,p.endRadius,p.age/p.life);p.position=p.origin-new Vector2(Mathf.Cos(p.angle),Mathf.Sin(p.angle))*radius;}particles[i]=p;}SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(data==null||!sprite)return;Rect rect=sprite.textureRect;Vector4 uv=new Vector4(rect.xMin/mainTexture.width,rect.yMin/mainTexture.height,rect.xMax/mainTexture.width,rect.yMax/mainTexture.height);foreach(var p in particles){float t=p.age/p.life;float size=Mathf.Lerp(p.startSize,p.endSize,t)*.5f;float spin=-Mathf.Lerp(p.startSpin,p.endSpin,t)*Mathf.Deg2Rad;Vector2 right=new Vector2(Mathf.Cos(spin),Mathf.Sin(spin))*size,up=new Vector2(-Mathf.Sin(spin),Mathf.Cos(spin))*size;Color32 tint=Color.Lerp(p.startColor,p.endColor,t)*color;for(int i=0;i<4;i++){quad[i]=UIVertex.simpleVert;quad[i].color=tint;}quad[0].position=p.position-right-up;quad[1].position=p.position-right+up;quad[2].position=p.position+right+up;quad[3].position=p.position+right-up;quad[0].uv0=new Vector2(uv.x,uv.y);quad[1].uv0=new Vector2(uv.x,uv.w);quad[2].uv0=new Vector2(uv.z,uv.w);quad[3].uv0=new Vector2(uv.z,uv.y);vh.AddUIVertexQuad(quad);}}
}
}

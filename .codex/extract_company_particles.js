() => {
 UnityEngine.Time.timeScale=0;
 const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);g.enabled=false;
 for(const r of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Renderer))r.enabled=false;
 const db=g.elementDatabase,specials=db.specialElements._items.slice(0,db.specialElements.Count),spec=b=>specials.find(x=>x.behaviorType===b);
 const configs={NormalBlue:[[db.normalElements._items[2].clearEffectPrefab,2],[db.normalElements._items[2].lightWaveEffectPrefab,1]],NormalGold:[[db.normalElements._items[3].clearEffectPrefab,2],[db.normalElements._items[3].lightWaveEffectPrefab,1]],Wave:[[db.normalElements._items[2].lightWaveEffectPrefab,1]],RocketTrail:[[spec(0).trailEffectPrefab,1]],ColorTrail:[[spec(3).trailEffectPrefab,1]],ColorCharge:[[spec(3).activationEffectPrefab,1]],ColorFinish:[[spec(3).finishEffectPrefab,1]],Bomb:[[spec(1).activationEffectPrefab,1]],BeeImpact:[[spec(2).activationEffectPrefab,1]]};
 const result={effects:[]};
 for(const [name,prefabs] of Object.entries(configs)){
  const roots=prefabs.map(([p,s])=>Match3.Match3EffectPlayer.Play(p,pc.Vec3.ZERO.clone(),null,s,0,null,true));
  const systems=roots.flatMap(x=>x.GetComponentsInChildren$1(UnityEngine.ParticleSystem,true)).map(x=>x.Eib$._particleSystem);
  const effect={name,duration:name==='ColorCharge'?1.2:name==='RocketTrail'?1.5:1.8,layers:[]};
  for(const n of systems){
   n.main.useUnscaledTime=true;n.play(false);
   const r=n.renderer,m=r.model.meshInstances[0]?.material,texture=m?.parameters._MainTex?.data;
   if(!m||!texture||r.renderMode===5)continue;
   const mesh=r.mesh,vb=mesh.vertexBuffer,f=new Float32Array(vb.storage),format=vb.format,pp=format.getStructureFor('POSITION'),uv=format.getStructureFor('TEXCOORD0');
   const vertices=[];for(let i=0;i<vb.numVertices;i++){let j=(pp.offset+i*pp.stride)/4,k=(uv.offset+i*uv.stride)/4;vertices.push([f[j],f[j+1],f[j+2],f[k],f[k+1]]);}
   const layer={name:n.entity.name,texture:texture.$id,textureName:texture.name,shader:m._shader.name,indices:Array.from(mesh.getIndicies(0)),tracks:[],vertices};
   const tracks=new Map,dt=1/60;const tint=m.parameters._TintColor?.data||m.parameters._Color?.data||[1,1,1,1];
   layer.tint=Array.from(tint);layer.tiling=Array.from(m.parameters._MainTex_ST?.data||[1,1,0,0]);
   for(let frame=0;frame<Math.ceil(effect.duration/dt);frame++){
    if(name==='ColorTrail')for(const root of roots)root.transform.position=new pc.Vec3(-1.4+Math.min(1,frame/8)*2.8,0,0);
    n.update(dt,n.entity.getWorldTransform());const c=n._cache,mat=n.entity.getWorldTransform(),scale=n.main.scalingMode===0?n.entity.getLossyScale():n.main.scalingMode===1?n.entity.getLocalScale():pc.Vec3.ONE;
    for(let i=0;i<c.aliveCount;i++){
     const seed=c.seed[i];let track=tracks.get(seed);if(!track){track={start:frame/60,samples:[]};tracks.set(seed,track);layer.tracks.push(track);}
     const ii=i*3,iv=i*4,pos=new pc.Vec3(c.t[ii],c.t[ii+1],c.t[ii+2]),q=new pc.Quat(c.r[iv],c.r[iv+1],c.r[iv+2],c.r[iv+3]);
     if(n.main.simulationSpace===0){const rot=n.entity.getRotation();rot.transformVector(pos,pos);pos.add(n.entity.getPosition());}
     let sx=c.s[ii]*Math.abs(scale.x),sy=c.s[ii+1]*Math.abs(scale.y),sz=c.s[ii+2]*Math.abs(scale.z);
     if(r.renderMode===1){let speed=Math.hypot(c.velocity[ii]+c.animatedVelocity[ii],c.velocity[ii+1]+c.animatedVelocity[ii+1],c.velocity[ii+2]+c.animatedVelocity[ii+2]);sy=c.s[ii+1]*r.lengthScale+speed*r.velocityScale;}
     const color=Array.from(c.color.slice(iv,iv+4));
     let sheet=n.textureSheetAnimation,uvScaleX=1,uvScaleY=1,uvX=0,uvY=0;
     if(sheet.enabled){uvScaleX=1/sheet.numTilesX;uvScaleY=1/sheet.numTilesY;uvX=c.texSheetOffsetX[i];uvY=sheet.numTilesY-c.texSheetOffsetY[i]-1;}
     const sample=[...color];
     for(const v of vertices){let p=new pc.Vec3(v[0]*sx,v[1]*sy,v[2]*sz);q.transformVector(p,p);if(r.renderMode===4&&n.main.simulationSpace===0)n.entity.getRotation().transformVector(p,p);p.add(pos);sample.push(p.x*113.6,p.y*113.6,v[3]*uvScaleX+uvX*uvScaleX,v[4]*uvScaleY+uvY*uvScaleY);}
     track.samples.push(sample.map(x=>Math.round(x*10000)/10000));
    }
   }
   if(layer.tracks.length)effect.layers.push(layer);
  }
  result.effects.push(effect);for(const root of roots)UnityEngine.Object.DestroyImmediate(root);
 }
 return result;
}

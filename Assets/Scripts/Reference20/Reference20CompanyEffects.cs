using UnityEngine;
namespace Reference20 {
public static class Reference20CompanyEffects {
 public static void Prewarm(){Reference20CompanyParticles.Load();}
 public static void Reset(){Reference20CompanyParticles.Reset();}
 public static RectTransform Play(string name,RectTransform stage,Vector2 position,float scale=1,bool loop=false){return Reference20CompanyParticles.Play(name,stage,position,scale,loop);}
 public static void Beam(RectTransform stage,Vector2 from,Vector2 to){Reference20ColorTrail.Play(stage,from,to);}
}
}

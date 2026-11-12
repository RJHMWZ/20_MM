using UnityEngine;
using UnityEngine.UI;
namespace Reference20 {
public class Reference20Idle : MonoBehaviour {
 public Sprite[] frames;
 public float interval=.16f;
 Image image;float elapsed;
 void Awake(){image=GetComponent<Image>();}
 void Update(){if(frames==null||frames.Length==0)return;elapsed+=Time.deltaTime;int index=(int)(elapsed/interval)%frames.Length;if(frames[index])image.sprite=frames[index];}
}
}

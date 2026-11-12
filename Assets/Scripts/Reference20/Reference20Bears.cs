using DG.Tweening;
using UnityEngine;

namespace Reference20 {
public partial class Reference20Game {
 public const int BearId=100060;

 void CreateBear(int x,int y){
  var p=new Piece{id=BearId,x=x,y=y};
  p.root=Reference20ReplacementVisual.Create(BearId,"Bear_"+x+"_"+y,stage);
  p.root.anchorMin=p.root.anchorMax=Vector2.zero;
  p.root.localScale=Vector3.one*.8875f;
  p.root.anchoredPosition=CellPosition(x,y);
  p.initial=p.simPosition=p.previousPosition=p.root.anchoredPosition;
  pieces.Add(p);
  if(Inside(x,y))playable[x,y]=true;
 }

 bool TryCollectBear(Piece p){
  if(!p.bear||p.dying||p.falling||p.paused||p.reserved||p.switching||!p.root||fallingPauseCount>0)return false;
  if((p.simPosition-Position(p)).sqrMagnitude>.01f)return false;
  // Collection happens only at the board's exit, never on top of another piece.
  for(int y=p.y+1;y<12;y++)if(playable[p.x,y])return false;
  p.dying=true;
  Vacate(p);pieces.Remove(p);motionOrder.Remove(p);renderOrder.Remove(p);landed.Remove(p);
  ClearedCount++;Trace("bear-collected",p);
  var root=p.root;root.DOKill();root.anchoredPosition=p.simPosition;
  Reference20Native.Raise(root,stage);
  var visual=root.GetComponent<Reference20ReplacementVisual>();visual.BearArrived();
  pendingCollections++;
  Vector2 screenTop;
  RectTransformUtility.ScreenPointToLocalPointInRectangle(stage,new Vector2(Screen.width*.5f,Screen.height+200),null,out screenTop);
  float exitY=Mathf.Max(root.anchoredPosition.y+Cell*3,screenTop.y+stage.sizeDelta.y*.5f);
  DOTween.Sequence().SetTarget(root).AppendInterval(.8f)
   .AppendCallback(()=>{if(visual)visual.BearFlying();Sound("flyout_duang",.5f);})
   .Append(root.DOAnchorPosY(exitY,.85f).SetEase(Ease.InQuad))
   .OnComplete(()=>{pendingCollections=Mathf.Max(0,pendingCollections-1);if(root)Destroy(root.gameObject);});
  return true;
 }
}
}

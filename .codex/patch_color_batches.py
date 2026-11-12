from pathlib import Path
p=Path('Assets/Scripts/Reference20/Reference20EffectInstance.cs');s=p.read_text(encoding='utf-8-sig');s=s.replace('void Update(){if(release==null)return;remaining-=Time.deltaTime;if(remaining>0)return;var complete=release;', 'void Update(){if(release==null)return;remaining-=Time.deltaTime;if(remaining<=0)Complete();}\n public void Complete(){if(release==null)return;var complete=release;');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Native.cs');s=p.read_text(encoding='utf-8-sig').replace('public static void HideEffect(', 'public static void FinishEffect(RectTransform rt){if(!rt)return;var instance=rt.GetComponent<Reference20EffectInstance>();if(instance)instance.Complete();}\n public static void HideEffect(');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Runtime.cs');s=p.read_text(encoding='utf-8-sig').replace('comboCount,fallingPauseCount;', 'comboCount,fallingPauseCount,matchingPauseCount;').replace('comboCount=fallingPauseCount=0;', 'comboCount=fallingPauseCount=matchingPauseCount=0;').replace('if(due){var matches=', 'if(due&&matchingPauseCount==0){var matches=');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Birds.cs');s=p.read_text(encoding='utf-8-sig');start=s.index(' Piece BirdDestination(');end=s.index(' void LaunchPeacock',start)
s=s[:start]+''' Piece BirdDestination(Vector2Int cell){var old=grid[cell.x,cell.y];if(old!=null){Vacate(old);pieces.Remove(old);motionOrder.Remove(old);renderOrder.Remove(old);landed.Remove(old);if(!old.dying)RecycleNormal(old.root);}var p=CreateNormal(cell.x,cell.y,3,false);p.settledAt=clock;p.previousPosition=p.simPosition=CellPosition(cell.x,cell.y);return p;}
'''+s[end:];p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Specials.cs');s=p.read_text(encoding='utf-8-sig').replace('using System.Collections.Generic;', 'using System;\nusing System.Collections.Generic;');start=s.index(' void ColorBlast(');end=s.index(' void DoubleColor(',start)
s=s[:start]+''' void ColorBlast(Vector2 position,int color,int payload,Piece source=null,Piece companion=null,bool swapOrigin=false){
  if(color==0){int blue=0,yellow=0;foreach(var p in pieces)if(!p.falling&&!p.dying&&!p.reserved){if(p.id==3)blue++;if(p.id==4)yellow++;}color=blue==0&&yellow==0?-1:blue>=yellow?3:4;}
  if(source!=null){source.reserved=true;source.root.gameObject.SetActive(false);}if(companion!=null){companion.reserved=true;if(payload>0)companion.root.gameObject.SetActive(false);}
  matchingPauseCount++;BeginStandby();var ready=Reference20Native.Effect("SameColorEffect",stage,position,.8875f,3600);var targets=new List<Piece>();var shakes=new List<RectTransform>();
  Action finish=()=>{
   matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);EndStandby();Reference20Native.FinishEffect(ready);
   if(swapOrigin&&source!=null&&companion!=null&&pieces.Contains(source)&&pieces.Contains(companion)){int sx=source.x,sy=source.y;Erase(source);Erase(companion);source.x=companion.x;source.y=companion.y;companion.x=sx;companion.y=sy;Put(source);Put(companion);companion.simPosition=companion.root.anchoredPosition=Position(companion);}
   if(source!=null){source.reserved=false;ClearNormal(source,false);}StopHints();
   if(payload==0){if(companion!=null){companion.reserved=false;ClearNormal(companion);}var hit=new HashSet<Piece>();foreach(var target in targets){target.reserved=false;if(!pieces.Contains(target))continue;foreach(var d in Neighbors){int x=target.x+d.x,y=target.y+d.y;if(Inside(x,y)&&grid[x,y]!=null&&!grid[x,y].normal)Damage(grid[x,y],hit);}ClearNormal(target);}After(.16f,()=>{foreach(var effect in shakes)Reference20Native.FinishEffect(effect);});}
   else {if(companion!=null&&pieces.Contains(companion)){companion.root.gameObject.SetActive(true);targets.Add(companion);}targets.Sort((a,b)=>(a.y*9+a.x).CompareTo(b.y*9+b.x));for(int i=0;i<targets.Count;i++){var target=targets[i];After(i*.25f,()=>{target.reserved=false;if(pieces.Contains(target))Activate(target,0);});}}
   Sound("Audio_Match_SameColorCrusherExplode");
  };
  Action collect=null;collect=()=>{
   var pending=new List<Piece>();for(int y=0;y<12;y++)for(int x=0;x<9;x++){var p=grid[x,y];if(p==null||!p.basic||p.dying||p.falling||p.id!=color||targets.Contains(p)||p.reserved&&!(p==companion&&payload==0)||payload==0&&beeTargets.Contains(new Vector2Int(x,y)))continue;pending.Add(p);targets.Add(p);p.reserved=true;}
   int remaining=pending.Count;if(remaining==0){After(.3f,finish);return;}
   while(pending.Count>0){int index=RandomRange(0,pending.Count-1);var target=pending[index];pending.RemoveAt(index);int special=payload==11||payload==12?(RandomRange(0,1)==0?11:12):payload;float delay=pending.Count*(payload==0?.07f:.1f);Reference20Native.ColorBeam(stage,position,Position(target),delay);
    After(delay+.2f,()=>{if(pieces.Contains(target)){if(special>0)ReplaceBasic(target,special);else{Hint(target,"ele_anim_samecolorblock_shake");shakes.Add(Reference20Native.Effect("SamecolorTargetShakeEffect",stage,Position(target),.8875f,3600));}}remaining--;if(remaining==0)collect();});
   }
  };collect();
 }
'''+s[end:]
s=s.replace('fallingPauseCount++;a.reserved', 'fallingPauseCount++;matchingPauseCount++;a.reserved').replace('After(last+.15f,()=>fallingPauseCount=Mathf.Max(0,fallingPauseCount-1));', 'After(last+.15f,()=>{fallingPauseCount=Mathf.Max(0,fallingPauseCount-1);matchingPauseCount=Mathf.Max(0,matchingPauseCount-1);});')
p.write_text(s,encoding='utf-8')

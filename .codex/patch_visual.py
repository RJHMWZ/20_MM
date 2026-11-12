from pathlib import Path
p=Path('Assets/Scripts/Reference20/Reference20Native.cs');s=p.read_text(encoding='utf-8-sig')
s=s.replace('public int src=770,dst=771;', 'public int src=770,dst=771,spriteSizeMode;')
s=s.replace('static readonly Dictionary<string,Sprite> sprites=', 'static readonly HashSet<int> trimmedSprites=new HashSet<int>();\n static readonly Dictionary<string,Sprite> sprites=')
s=s.replace('public static void ResetCaches(){graph=null;', 'public static void ResetCaches(){trimmedSprites.Clear();graph=null;')
s=s.replace('im.sprite=Sprite(n.sprite);if(n.mask', 'im.sprite=Sprite(n.sprite);if(n.spriteSizeMode==1)trimmedSprites.Add(im.GetInstanceID());if(n.mask')
start=s.index(' public static void LineBlast(');end=s.index(' public static void ColorBeam(',start)
s=s[:start]+''' public static void ApplySpriteFrame(Image image,Sprite sprite){if(!sprite||image.sprite==sprite)return;image.sprite=sprite;if(trimmedSprites.Contains(image.GetInstanceID()))image.SetNativeSize();}
 public static void LineBlast(RectTransform stage,Vector2 position,bool horizontal,float duration){var rt=LeaseEffect("LineMoveEffect",stage,position,duration);if(!rt)return;foreach(string name in new[]{"End1","End2"}){var end=rt.Find(name) as RectTransform;if(end)end.localRotation=Quaternion.Euler(0,0,horizontal?0:-90);}foreach(var clip in rt.GetComponentsInChildren<Reference20CocosClip>(true))clip.Play(0);}
'''+s[end:]
start=s.index(' public void NestFeedback()');end=s.index(' public float DamageDuration',start)
s=s[:start]+''' public void NestFeedback(){var art=transform.Find("Root/ArtRoot");var clip=art?art.GetComponent<Reference20CocosClip>():null;if(clip)clip.Play(0);}
'''+s[end:]
p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20CocosClip.cs');s=p.read_text(encoding='utf-8-sig');s=s.replace('im.sprite=Reference20Native.Sprite((string)av["sprite"]);','Reference20Native.ApplySpriteFrame(im,Reference20Native.Sprite((string)av["sprite"]));');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Specials.cs');s=p.read_text(encoding='utf-8-sig').replace('Reference20Native.LineBlast(stage,CellPosition(x,y),horizontal);','Reference20Native.LineBlast(stage,CellPosition(x,y),horizontal,duration);');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Birds.cs');s=p.read_text(encoding='utf-8-sig').replace('var candidates=empty.Count>0?', 'if(empty.Count==0)RandomRange(0,-1);var candidates=empty.Count>0?');p.write_text(s,encoding='utf-8')
p=Path('Assets/Scripts/Reference20/Reference20Runtime.cs');s=p.read_text(encoding='utf-8-sig').replace('});Sound("Audio_Match_HenEgg0");return;}','});return;}')
s=s.replace('float begin=Mathf.Max(clock,p.lockedUntil);', 'float begin=Mathf.Max(clock+(p.id==100042?.16f:0),p.lockedUntil);')
s=s.replace('After(p.lockedUntil-clock+.16f,()=>', 'After(p.lockedUntil-clock+(p.id==100042?0:.16f),()=>')
p.write_text(s,encoding='utf-8')

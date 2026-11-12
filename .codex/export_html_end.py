from pathlib import Path
import json,base64
from playwright.sync_api import sync_playwright
out=Path('.codex/html-parity')
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':804,'height':1280})
 page.goto(src.as_uri());page.wait_for_timeout(4000)
 data=page.evaluate('''()=>{window.__g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);__g.isGameOver=true;__g.uiManager.ShowDefeatPanel();const vec=v=>v?{x:v.x,y:v.y,z:v.z,w:v.w}:null;function walk(t){const o={name:t.name,position:vec(t.anchoredPosition),size:vec(t.sizeDelta),anchorMin:vec(t.anchorMin),anchorMax:vec(t.anchorMax),pivot:vec(t.pivot),scale:vec(t.localScale),rotation:vec(t.localEulerAngles),active:t.gameObject.activeSelf,children:[]};const im=t.GetComponent$1(UnityEngine.UI.Image);if(im)o.image={color:{r:im.color.r,g:im.color.g,b:im.color.b,a:im.color.a},type:im.type,preserveAspect:im.preserveAspect,sprite:im.sprite?{name:im.sprite.name,texture:im.sprite.texture.name,border:vec(im.sprite.border),rect:{x:im.sprite.rect.x,y:im.sprite.rect.y,w:im.sprite.rect.width,h:im.sprite.rect.height}}:null};const tx=t.GetComponent$1(TMPro.TextMeshProUGUI);if(tx)o.text={text:tx.text,fontSize:tx.fontSize,fontStyle:tx.fontStyle,outlineWidth:tx.outlineWidth,color:{r:tx.color.r,g:tx.color.g,b:tx.color.b,a:tx.color.a}};for(let i=0;i<t.childCount;i++)o.children.push(walk(t.GetChild(i)));return o;}return {defeat:walk(__g.uiManager.defeatPanel.transform),victory:walk(__g.uiManager.victoryPanel.transform),uiScalars:Object.fromEntries(Object.entries(__g.uiManager).filter(([k,v])=>['number','string','boolean'].includes(typeof v))),specials:__g.elementDatabase.specialElements._items.slice(0,__g.elementDatabase.specialElements.Count).map(d=>{let v=d.prefab.GetComponent$1(Match3.SpecialElementView);return {behavior:d.behaviorType,view:Object.fromEntries(Object.entries(v).filter(([k,x])=>['number','string','boolean'].includes(typeof x))),swaps:v.swapAnimations? v.swapAnimations._items.slice(0,v.swapAnimations.Count).map(s=>({partner:s.partnerBehaviorType,animation:s.animationName,scale:s.scaleMultiplier})):[]};}),level:{steps:__g.currentLevelData.maxSteps,targets:__g.activeTargets._items.slice(0,__g.activeTargets.Count).map(t=>({amount:t.remainingAmount,type:t.definition.targetType,keys:Object.keys(t.definition)}))}};}''')
 page.wait_for_timeout(900);page.screenshot(path=str(out/'defeat.png'))
 (out/'end-source.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf8')
 print(json.dumps(data,ensure_ascii=False),flush=True)
 b.close()

import json
from pathlib import Path
from playwright.sync_api import sync_playwright

out=Path('.codex/company')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':540,'height':960})
 page.goto(Path('C:/Users/ivy/Desktop/樊思吾.html').as_uri(),wait_until='domcontentloaded');page.wait_for_timeout(2500)
 info=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);window.__g=g;g.isGameOver=true;g.uiManager.ShowDownloadPanel();const panel=g.uiManager.downloadPanel;const t=panel.transform;return {panel:panel.name,transformKeys:Object.getOwnPropertyNames(Object.getPrototypeOf(t)),getComponent:t.GetComponent$1?t.GetComponent$1.toString().slice(0,500):null,getComponents:t.GetComponentsInChildren$1?t.GetComponentsInChildren$1.toString().slice(0,400):null};}''')
 page.wait_for_timeout(1200)
 (out/'panel-api.json').write_text(json.dumps(info,indent=2),encoding='utf-8')
 tree=page.evaluate('''()=>{const vec=v=>v?{x:v.x,y:v.y,z:v.z}:null;function walk(t){const o={name:t.name,position:vec(t.anchoredPosition),size:vec(t.sizeDelta),anchorMin:vec(t.anchorMin),anchorMax:vec(t.anchorMax),pivot:vec(t.pivot),scale:vec(t.localScale),rotation:vec(t.localEulerAngles),active:t.gameObject.activeSelf,children:[]};const im=t.GetComponent$1(UnityEngine.UI.Image);if(im){o.image={color:{r:im.color.r,g:im.color.g,b:im.color.b,a:im.color.a},type:im.type,preserveAspect:im.preserveAspect,pixelsPerUnitMultiplier:im.pixelsPerUnitMultiplier,sprite:im.sprite?{name:im.sprite.name,id:im.sprite.GetInstanceID(),texture:im.sprite.texture.name,border:{x:im.sprite.border.x,y:im.sprite.border.y,z:im.sprite.border.z,w:im.sprite.border.w},rect:{x:im.sprite.rect.x,y:im.sprite.rect.y,w:im.sprite.rect.width,h:im.sprite.rect.height}}:null};}const text=t.GetComponent$1(TMPro.TextMeshProUGUI);if(text)o.text={text:text.text,fontSize:text.fontSize,font:text.font?text.font.name:null,fontStyle:text.fontStyle,color:{r:text.color.r,g:text.color.g,b:text.color.b,a:text.color.a}};for(let i=0;i<t.childCount;i++)o.children.push(walk(t.GetChild(i)));return o;}return walk(__g.uiManager.downloadPanel.transform);}''')
 (out/'panel-tree.json').write_text(json.dumps(tree,ensure_ascii=False,indent=2),encoding='utf-8')
 page.screenshot(path=str(out/'end.png'))
 audio=page.evaluate('''()=>{const a=UnityEngine.Object.FindObjectOfType$1(Match3.AudioManager);return Object.fromEntries(Object.keys(a).filter(k=>/clip$/i.test(k)).map(k=>[k,a[k]?a[k].name:null]));}''')
 (out/'audio-map.json').write_text(json.dumps(audio,ensure_ascii=False,indent=2),encoding='utf-8')
 print('Exported company panel tree')
 b.close()

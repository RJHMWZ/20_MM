from pathlib import Path
import json,re,base64,sys
from PIL import Image
from playwright.sync_api import sync_playwright
mode=sys.argv[1] if len(sys.argv)>1 else 'victory'
layout='end-layout' if mode=='victory' else 'end-defeat-layout'
label_name='EndDownloadLabel' if mode=='victory' else 'EndTryAgainLabel'
out=Path('Assets/Resources/Reference20/Company/UI')
src=next(p for p in Path(r'C:\Users\yangjinsheng\Desktop').glob('*.html') if p.stat().st_size==5080582)
html=src.read_text('utf8')
images={}
for m in re.finditer(r'<img\b([^>]+)>',html):
 attrs=dict(re.findall(r'([\w-]+)="([^"]*)"',m[1]));images[attrs.get('id')]=attrs.get('src')
bundle=json.loads(Path('.codex/company/jsons.json').read_text('utf8'))['assets/bundles/-1/bundle.json']
sprites={s['data'][0]:s for s in bundle['sprites']}
end=json.loads(Path('.codex/html-parity/end-source.json').read_text('utf8'))[mode]
end['scale']={'x':1,'y':1,'z':1}
def visit(n):
 if n.get('image',{}).get('sprite'):
  name=n['image']['sprite']['name']
  if name in sprites:
   s=sprites[name];prefix=f"assets/bundles/{s['assetBundleId']}/{s['data'][2]}."
   uri=next((v for k,v in images.items() if k and k.startswith(prefix)),None)
   if uri:
    import io
    Image.open(io.BytesIO(base64.b64decode(uri.split(',')[1]))).save(out/(name+'.png'))
 for c in n.get('children',[]):visit(c)
visit(end)
Path('Assets/Resources/Reference20/Company/'+layout+'.json').write_text(json.dumps(end,ensure_ascii=False,indent=2),encoding='utf8')
with sync_playwright() as p:
 b=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
 page=b.new_page(viewport={'width':1080,'height':1920})
 page.goto(src.as_uri());page.wait_for_timeout(4000)
 page.evaluate('''mode=>{window.__g=UnityEngine.Object.FindObjectOfType$1(Match3.GameManager);__g.isGameOver=true;__g.boardManager.backgroundContainer.gameObject.SetActive(false);__g.boardManager.elementsContainer.gameObject.SetActive(false);__g.uiManager.centerBorderRect.gameObject.SetActive(false);__g.uiManager[mode==='victory'?'ShowVictoryPanel':'ShowDefeatPanel']();}''',mode)
 page.wait_for_timeout(1000);page.screenshot(path='.codex/html-parity/'+mode+'.png')
 info=page.evaluate('''mode=>{const panel=mode==='victory'?__g.uiManager.victoryPanel:__g.uiManager.defeatPanel;const text=panel.GetComponentInChildren$1(TMPro.TextMeshProUGUI,true);window.__label=text;let keep=new Set();for(let t=text.transform;t;t=t.parent)keep.add(t.gameObject.GetInstanceID());function isolate(t){for(let i=0;i<t.childCount;i++){let c=t.GetChild(i);if(!keep.has(c.gameObject.GetInstanceID()))c.gameObject.SetActive(false);else isolate(c);}let im=t.GetComponent$1(UnityEngine.UI.Image);if(im)im.enabled=false;}let root=text.transform;while(root.parent)root=root.parent;isolate(root);for(const r of UnityEngine.Object.FindObjectsOfType$1(UnityEngine.Renderer))r.enabled=false;return {font:text.font?.name,fontSize:text.fontSize,material:text.fontSharedMaterial?.name,rect:{x:text.rectTransform.rect.x,y:text.rectTransform.rect.y,w:text.rectTransform.rect.width,h:text.rectTransform.rect.height}};}''',mode)
 for name,value in [('black',0),('white',1)]:
  page.evaluate('v=>{const c=UnityEngine.Camera.main.backgroundColor;c.r=c.g=c.b=v;c.a=1;UnityEngine.Camera.main.backgroundColor=c;document.body.style.background=v?"white":"black";}',value)
  page.wait_for_timeout(100);page.screenshot(path=f'.codex/html-parity/label-{name}.png')
 print(info)
 b.close()
black=Image.open('.codex/html-parity/label-black.png').convert('RGB')
white=Image.open('.codex/html-parity/label-white.png').convert('RGB')
rgba=Image.new('RGBA',black.size)
result=[]
for a,b in zip(black.getdata(),white.getdata()):
 alpha=max(0,min(255,255-min(b[i]-a[i] for i in range(3))))
 result.append(tuple(min(255,round(v*255/alpha)) if alpha else 0 for v in a)+(alpha,))
rgba.putdata(result);box=rgba.getbbox()
if not box or box[2]-box[0]>800:raise ValueError(('Invalid isolated label',box))
rgba.crop(box).save(out/(label_name+'.png'))
# The glyph raster keeps the HTML font's outline, metrics and baseline exactly.
label=end['children'][-1]['children'][0]
label.pop('text');label['image']={'type':0,'preserveAspect':False,'color':{'r':1,'g':1,'b':1,'a':1},'sprite':{'name':label_name}}
button=end['children'][-1]
label['position']={'x':(box[0]+box[2])/2-540-button['position']['x'],'y':1920-(box[1]+box[3])/2-960-end['position']['y']-button['position']['y']}
label['size']={'x':box[2]-box[0],'y':box[3]-box[1]}
end['renderedColor']=dict(zip(['r','g','b'],[v/255 for v in Image.open('.codex/html-parity/'+mode+'.png').convert('RGB').getpixel((0,0))]))
Path('Assets/Resources/Reference20/Company/'+layout+'.json').write_text(json.dumps(end,ensure_ascii=False,indent=2),encoding='utf8')
print('Imported victory panel and original glyph raster',box)

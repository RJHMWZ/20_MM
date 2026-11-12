from playwright.sync_api import sync_playwright
from PIL import Image
import json,pathlib,base64,io
out=pathlib.Path('Assets/Resources/Reference20');out.mkdir(parents=True,exist_ok=True)
with sync_playwright() as p:
 b=p.chromium.launch(headless=True,channel='msedge');page=b.new_page(viewport={'width':540,'height':960});page.goto('file:///C:/Users/ivy/Desktop/20.html');page.wait_for_timeout(2400)
 page.evaluate('''() => {window.cap=(n)=>{let cam=cc.find('Canvas/Main Camera').getComponent(cc.Camera);let rt=new cc.RenderTexture();rt.initWithSize(1080,1920,cc.game._renderContext.STENCIL_INDEX8);cam.targetTexture=rt;cam.backgroundColor=cc.color(0,0,0,0);cam.render(n);let data=rt.readPixels();cam.targetTexture=null;let cv=document.createElement('canvas');cv.width=1080;cv.height=1920;let ctx=cv.getContext('2d'),id=ctx.createImageData(1080,1920);for(let y=0;y<1920;y++)id.data.set(data.subarray((1919-y)*4320,(1920-y)*4320),y*4320);ctx.putImageData(id,0,0);rt.destroy();return cv.toDataURL()};cc.director.pause();window.br=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0');}''')
 def capture(expr,file):
  data=page.evaluate('cap('+expr+')');im=Image.open(io.BytesIO(base64.b64decode(data.split(',')[1])));box=im.getbbox()
  if not box:return None
  im.crop(box).save(out/(file+'.png'));return dict(asset=file,left=box[0],top=box[1],width=box[2]-box[0],height=box[3]-box[1])
 manifest={'layers':[],'pieces':[]}
 for path,file in [('Canvas/Bg','background'),('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/TiledRoot_0','tiles'),('Canvas/UICustom','decoration')]:
  d=capture('cc.find('+json.dumps(path)+')',file)
  if d:manifest['layers'].append(d)
 # Ground grass is below the movable blockers.
 page.evaluate("br.children.forEach(n=>{n._saved=n.active;if(!n.name.endsWith('_201'))n.active=false})")
 d=capture('br','grass');manifest['layers'].insert(2,d)
 page.evaluate('br.children.forEach(n=>n.active=n._saved)')
 nodes=page.evaluate("br.children.map((n,i)=>({index:i,name:n.name,x:n.x,y:n.y})).filter(n=>n.name!='EntryBlockRoot'&&!n.name.endsWith('_201'))")
 for j,n in enumerate(nodes):
  file='piece_'+str(j);d=capture('br.children['+str(n['index'])+']',file)
  if not d:continue
  n.update(d);n['id']=int(n['name'].split('_')[-1]);manifest['pieces'].append(n)
 # Capture a clean hand independent of the dark tutorial mask.
 hand=page.evaluate("(()=>{let a=[];function w(n){if(n.name.toLowerCase().includes('hand'))a.push(n.name);n.children.forEach(w)}w(cc.find('Canvas/UIRoot'));return a})()")
 print('hand',hand)
 pathlib.Path(out/'layout.json').write_text(json.dumps(manifest,indent=2),encoding='utf8');print('captured',len(manifest['pieces']))
 b.close()

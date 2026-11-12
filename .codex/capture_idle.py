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

 manifest=json.loads((out/'layout.json').read_text(encoding='utf8'))
 storage={n['asset']:[] for n in manifest['pieces'] if n['id']>=100}
 for frame in range(8):
  page.evaluate("(()=>{function walk(n){let c=n.getComponent(sp.Skeleton);if(c)c.update(.16);n.children.forEach(walk)}walk(br)})()")
  for n in manifest['pieces']:
   if n['id']<100:continue
   data=page.evaluate('cap(br.children['+str(n['index'])+'])');im=Image.open(io.BytesIO(base64.b64decode(data.split(',')[1])));storage[n['asset']].append(im)
 for n in manifest['pieces']:
  if n['asset'] not in storage:continue
  images=storage[n['asset']];boxes=[im.getbbox() for im in images];box=(min(x[0] for x in boxes),min(x[1] for x in boxes),max(x[2] for x in boxes),max(x[3] for x in boxes))
  n.update(left=box[0],top=box[1],width=box[2]-box[0],height=box[3]-box[1])
  for i,im in enumerate(images):im.crop(box).save(out/(n['asset']+'_idle_'+str(i)+'.png'))
  images[0].crop(box).save(out/(n['asset']+'.png'))
 (out/'layout.json').write_text(json.dumps(manifest,indent=2),encoding='utf8')
 print('Animated pieces',len(storage))
 b.close()

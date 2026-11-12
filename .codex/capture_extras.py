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

 handpath=page.evaluate("(()=>{let found='';function w(n,path){if(n.name==='CommonHandBlock')found=path+'/'+n.name;n.children.forEach(c=>w(c,path+'/'+n.name))}w(cc.find('Canvas/UIRoot'),'');return found.replace('/UIRoot','Canvas/UIRoot')})()")
 print(handpath)
 d=capture("cc.find("+json.dumps(handpath)+")",'hand');print(d)
 page.evaluate("cc.director.resume();__require('UIManager').UIManager.Instance.CloseLevelGuide();__require('UIManager').UIManager.Instance.OpenLevelPass()")
 page.wait_for_timeout(2000)
 d=capture("cc.find('Canvas/UIRoot')",'endcard');print(d)
 page.screenshot(path='.codex/reference-end.png')
 b.close()

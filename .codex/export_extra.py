from playwright.sync_api import sync_playwright
import pathlib,json,base64
out=pathlib.Path('Assets/Resources/Reference20/Native')
with sync_playwright() as p:
 b=p.chromium.launch(headless=True,channel='msedge');page=b.new_page(viewport={'width':540,'height':960});page.goto('file:///C:/Users/ivy/Desktop/20.html');page.wait_for_timeout(3500)
 d=page.evaluate('''() => {let root=cc.find('Canvas/MapRoot/TiledMap_0/TiledMapRoot_0/BlockerRoot_0');let n=root.children.find(n=>n.name.endsWith('_100016'));let c=n.getComponent(__require('HenHouseBlockerCom').default);let data=c.SpineSkeletonDatas[0],files={},name='skeleton_chick_yellow',atlas=data.atlasText;data.textureNames.forEach((old,i)=>{let tex=data.textures[i],img=tex.getHtmlElementObj(),cv=document.createElement('canvas');cv.width=tex.width;cv.height=tex.height;cv.getContext('2d').drawImage(img,0,0);let file=name+'_tex'+i+'.png';files[file]=cv.toDataURL();atlas=atlas.split(old).join(file)});files[name+'.json']=JSON.stringify(data.skeletonJson);files[name+'.atlas.txt']=atlas;return {files,name:data.name,active:c.eggNode.children.map(n=>({name:n.name,data:n.getComponent(sp.Skeleton).skeletonData.name}))}}''')
 for name,data in d.pop('files').items():
  if data.startswith('data:'):(out/name).write_bytes(base64.b64decode(data.split(',',1)[1]))
  else:(out/name).write_text(data,encoding='utf8')
 print(d);b.close()

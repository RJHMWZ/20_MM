import json
from pathlib import Path
from playwright.sync_api import sync_playwright

with sync_playwright() as p:
    browser=p.chromium.launch(channel='msedge',headless=True,args=['--use-angle=swiftshader','--enable-unsafe-swiftshader'])
    page=browser.new_page(viewport={'width':540,'height':960})
    errors=[];console=[];missing=[]
    page.on('pageerror',lambda e:errors.append(str(e)))
    page.on('console',lambda m:console.append(m.text) if m.type=='error' else None)
    page.on('response',lambda r:missing.append(r.url) if r.status>=400 and r.request.method=='GET' and '/assets/' in r.url and r.url.startswith('http://127.0.0.1:8136/') else None)
    page.goto('http://127.0.0.1:8136/iframe.html',wait_until='domcontentloaded',timeout=60000)
    page.wait_for_timeout(100)
    page.screenshot(path='.codex/company/browser-preloader.png')
    page.wait_for_timeout(700)
    page.screenshot(path='.codex/company/browser-loading.png')
    page.wait_for_timeout(7000)
    page.screenshot(path='.codex/playworks-browser-start.png')
    status=page.evaluate('''() => ({title:document.title,canvas:[...document.querySelectorAll('canvas')].map(c=>({w:c.width,h:c.height})),body:document.body.innerText.slice(0,1000),reference:typeof Reference20!=='undefined'})''')
    gameplay=page.evaluate('''()=>{const g=UnityEngine.Object.FindObjectOfType$1(Reference20.Reference20Game);window.__game=g;return {found:!!g,input:g?(g.getCanAcceptInput?g.getCanAcceptInput():g.CanAcceptInput):false,loading:g?g.loadingUIActive:null};}''')
    page.evaluate('()=>__game.ShowEnd()');page.wait_for_timeout(700)
    page.screenshot(path='.codex/company/browser-end.png')
    result={'errors':errors,'missingAssets':missing,'status':status,'gameplay':gameplay,'consoleErrors':console}
    Path('.codex/playworks-browser-smoke.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps(result,ensure_ascii=False))
    browser.close()

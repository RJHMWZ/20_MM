import urllib.request,json,pathlib,sys
u='http://127.0.0.1:8080/mcp'; sf=pathlib.Path('.codex/mcp-session')
def call(method,params={}):
 h={'Content-Type':'application/json','Accept':'application/json, text/event-stream'}
 if sf.exists():h['Mcp-Session-Id']=sf.read_text()
 req=urllib.request.Request(u,data=json.dumps(dict(jsonrpc='2.0',id=1,method=method,params=params)).encode(),headers=h)
 r=urllib.request.urlopen(req,timeout=120)
 if r.headers.get('Mcp-Session-Id'):sf.write_text(r.headers['Mcp-Session-Id'])
 s=r.read().decode();return next(d for d in [json.loads(l[6:]) for l in s.splitlines() if l.startswith('data: ')] if 'result' in d or 'error' in d) if any(l.startswith('data: ') for l in s.splitlines()) else json.loads(s)
if __name__=='__main__':
 method=sys.argv[1];params=json.loads(sys.argv[2]) if len(sys.argv)>2 else {};d=call(method,params);pathlib.Path('.codex/mcp-last.json').write_text(json.dumps(d,ensure_ascii=False),encoding='utf8');print(json.dumps(d,ensure_ascii=False))

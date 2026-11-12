import urllib.request,json,pathlib
u='http://127.0.0.1:8080/mcp'
def call(method,params={},id=1):
 req=urllib.request.Request(u,data=json.dumps(dict(jsonrpc='2.0',id=id,method=method,params=params)).encode(),headers={'Content-Type':'application/json','Accept':'application/json, text/event-stream'})
 r=urllib.request.urlopen(req);s=r.read().decode();print(s[:25000]);return s
call('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'codex','version':'1'}})
call('tools/list');call('resources/list')

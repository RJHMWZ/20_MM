import json,sys,pathlib
from mcp import call
if sys.argv[1]=='code':
 arguments={'action':'execute','code':pathlib.Path(sys.argv[2]).read_text('utf8')};name='execute_code'
else:
 name=sys.argv[1];arguments=json.loads(sys.argv[2]) if len(sys.argv)>2 else {}
result=call('tools/call',{'name':name,'arguments':arguments})
pathlib.Path('.codex/html-parity/last-unity-result.json').write_text(json.dumps(result,ensure_ascii=False),encoding='utf8')
print(json.dumps(result,ensure_ascii=False))

from pathlib import Path
import json
from clip_schema import convert

root=Path(__file__).resolve().parent.parent
scripts=root/'Assets/Scripts/Reference20'
def edit(name, fn):
    p=scripts/name
    p.write_text(fn(p.read_text(encoding='utf-8-sig')),encoding='utf-8')

for name in ['graph.json','end.json']:
    p=root/'Assets/Resources/Reference20/Native'/name
    p.write_text(json.dumps(convert(json.loads(p.read_text(encoding='utf-8'))),ensure_ascii=False,separators=(',',':')),encoding='utf-8')
for name in ['export_native.py','export_supp.py','export_end.py']:
    p=root/'.codex'/name
    text=p.read_text(encoding='utf-8')
    if 'from clip_schema import convert' not in text:
        text='from clip_schema import convert\n'+text
        text=text.replace('json.dumps(result,ensure_ascii=False)','json.dumps(convert(result),ensure_ascii=False)').replace('json.dumps(g,ensure_ascii=False)','json.dumps(convert(g),ensure_ascii=False)')
        p.write_text(text,encoding='utf-8')

def native(s):
    s=s.replace('using Newtonsoft.Json.Linq;\n','').replace('public JObject data;','public NativeClipData data;')
    start=s.index(' public static NativeGraph Graph {')
    end=s.index('\n public static RectTransform Piece',start)
    s=s[:start]+''' public static NativeGraph Graph {get {if(graph==null)graph=JsonConvert.DeserializeObject<NativeGraph>(Resources.Load<TextAsset>("Reference20/Native/graph").text);return graph;}}'''+s[end:]
    old='var rt=Reference20ReplacementVisual.Supports(id)?Reference20ReplacementVisual.Create(id,name,stage):Node(n,stage);'
    new='var rt=id>=11&&id<=15?Reference20Props.Rect(name,stage):Reference20ReplacementVisual.Supports(id)?Reference20ReplacementVisual.Create(id,name,stage):Node(n,stage);if(id>=11&&id<=15){var prop=Reference20Props.Visual(id,rt,false);if(id==12)prop.Find("Root/ArtRoot/Dynamic").localRotation=Quaternion.Euler(0,0,-90);}'
    assert old in s
    s=s.replace(old,new).replace('var handle=', 'var controlPoint=').replace('from,handle,handle,to','from,controlPoint,controlPoint,to')
    return s
edit('Reference20Native.cs',native)
for name,typ in [('Reference20Game.cs','Layout'),('Reference20SourceLevel.cs','SourceTopology'),('Reference20Fragments.cs','Manifest')]:
    edit(name,lambda s:s.replace('JsonUtility.FromJson<'+typ+'>','Newtonsoft.Json.JsonConvert.DeserializeObject<'+typ+'>'))
edit('Reference20Game.cs',lambda s:s.replace('Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")','(Font)Resources.GetBuiltinResource(typeof(Font),"LegacyRuntime.ttf")').replace('clip.LoadAudioData();','').replace('audioSource.ignoreListenerPause=false;',''))
edit('Reference20Runtime.cs',lambda s:s.replace('Array.Clear(pullReady,0,pullReady.Length);Array.Clear(cellLocks,0,cellLocks.Length);','for(int x=0;x<9;x++)for(int y=0;y<12;y++){pullReady[x,y]=0;cellLocks[x,y]=0;}'))
for name in ['Reference20ParticleGraphic.cs','Reference20Particle3DGraphic.cs']:
    edit(name,lambda s:s.replace('System.Threading.Interlocked.Increment(ref seedCounter)','++seedCounter'))
edit('Reference20Props.cs',lambda s:s.replace('public static class Reference20Props {','public static class Reference20Props {\n public const float VisualScale=.90f;').replace('Vector3.one*.82f','Vector3.one*VisualScale').replace('main.stopAction=ParticleSystemStopAction.None;main.cullingMode=ParticleSystemCullingMode.AlwaysSimulate;',''))
# Atlas regions are source pixels, not board units. Bound shard size independently of texture resolution.
edit('Reference20Fragments.cs',lambda s:s.replace('float angle=UnityEngine.Random.Range(0,Mathf.PI*2);parts.Add(new Fragment{region=regions[UnityEngine.Random.Range(0,regions.Length)],','var region=regions[UnityEngine.Random.Range(0,regions.Length)];float angle=UnityEngine.Random.Range(0,Mathf.PI*2);parts.Add(new Fragment{region=region,').replace('size=UnityEngine.Random.Range(.5f,.95f),','size=UnityEngine.Random.Range(.5f,.95f)*Mathf.Min(1,40f/Mathf.Max(region.w,region.h)),'))

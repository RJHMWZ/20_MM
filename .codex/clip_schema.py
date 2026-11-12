"""Convert exported animation curves to Playworks-compatible typed JSON."""
def convert(value):
    if isinstance(value, list):
        return [convert(item) for item in value]
    if not isinstance(value, dict):
        return value
    if 'props' in value or 'paths' in value or 'comps' in value:
        result = dict(value)
        for section in ('props',):
            if section in value:
                result[section] = curves(value[section])
        if 'comps' in value:
            result['comps'] = {key: curves(items) for key, items in value['comps'].items()}
        if 'paths' in value:
            result['paths'] = {key: convert(items) for key, items in value['paths'].items()}
        return result
    result = {key: convert(item) for key, item in value.items()}
    if 'pieces' in result and 'effects' in result:
        def hint(node):
            if node.get('name') == 'ArtRoot' and node.get('clips'):
                return node['clips']
            for child in node.get('children', []):
                found = hint(child)
                if found: return found
        if 'basicHintClips' not in result:
            result['basicHintClips'] = hint(result['pieces'][0])
        for node in result['pieces']:
            for key in list(node):
                if key not in ('name', 'x', 'y', 'sx', 'sy', 'angle'):
                    del node[key]
        required = ('CommonBornPropsEffect', 'SameColorEmit', 'SamecolorTargetShakeEffect', 'HenEggBlock')
        result['effects'] = {k: v for k, v in result['effects'].items() if k in required}
    return result

def curves(props):
    result = {}
    for name, keys in props.items():
        result[name] = []
        for key in keys:
            if 'value' not in key:  # Already converted.
                result[name].append(key)
                continue
            value = key['value']
            item = {'frame': key['frame']}
            if isinstance(value, bool): item['active'] = value
            elif isinstance(value, (int, float)): item['number'] = value
            elif isinstance(value, list): item['vector'] = value
            elif isinstance(value, dict): item['sprite'] = value.get('sprite')
            result[name].append(item)
    return result

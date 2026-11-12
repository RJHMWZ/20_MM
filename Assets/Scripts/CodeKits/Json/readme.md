# 作者
马特·舍恩（Matt Schoen）<schoen@defectivestudios.com>，隶属于[Defective Studios](http://www.defectivestudios.com)

# 简介
在我的一个项目中，需要与服务器之间传输结构化数据，我认为使用JSON格式会是个不错的选择。调研相关方案时，我尝试了json.org网站上列出的几款C# JSON解析实现，但发现它们使用和扩展起来都过于复杂。因此，我编写了一个极简的JSONObject类，可通用地将数据编码/解码为简易的容器结构。本文档假定你已了解JSON的定义及工作原理——JSON本身十分简单，若需直观了解其编码格式，可访问json.org网站。

# 使用方法
用户无需自行修改JSONObject类，只需遵循以下极简的使用流程：

示例数据（JSON格式）：
```JSON
{
    "TestObject": {
        "SomeText": "Blah",
        "SomeObject": {
            "SomeNumber": 42,
            "SomeBool": true,
            "SomeNull": null
        },
        
        "SomeEmptyObject": { },
        "SomeEmptyArray": [ ],
        "EmbeddedObject": "{\"field\":\"Value with \\\"escaped quotes\\\"\"}"
    }
}
```

测试类中包含了该API的最佳使用示例。

## 功能特性
* 将JSON格式字符串解码为可直接使用的数据结构
* 将结构化数据编码为JSON格式字符串
* 可与`Dictionary`（字典）和`WWWForm`（表单）互通使用
* 优化后的`parse`（解析）/`stringify`（序列化）函数——仅产生极少（不可避免的）内存垃圾
* 异步`stringify`函数，可序列化大量数据而不造成游戏帧丢失
* 解析时支持`MaxDepth`（最大深度）配置，可跳过无需处理的嵌套数据
* 特殊（非标准兼容）的`Baked`（固化）对象类型，可在解析后的对象中存储序列化后的字符串数据
* 支持复制到新的`JSONObject`实例
* 支持与另一个`JSONObject`合并（实验性功能）
* 支持随机访问（通过`int`索引或`string`键名）
* `ToString()`方法返回JSON数据，可选"pretty"（格式化）标识，用于包含换行符和制表符
* 可根据精度需求切换数值存储类型（双精度double/单精度float），确保数值解析/序列化的准确性
* 支持`Infinity`（无穷大）和`NaN`（非数值）值
* `JSONTemplates`静态类提供常用类（如`Vector3`、`Matrix4x4`）的序列化函数
* 对象池实现（实验性功能）
* 实用的`JSONChecker`窗口，可测试示例数据的解析效果

该解析器的能力边界应当十分清晰。如果有JSON资深爱好者（真的有这种人吗？）看到本文档，欢迎扩展和修改该解析器以提升标准兼容性。目前我使用.NET的`System.Convert`命名空间函数来解析数据，仅实现了字符串和数值的解析（满足我的核心需求），但如果某些格式不被`System.Convert`支持，解析器可能无法处理所有合规的JSON字符串。此外，由于这是我第一次编写JSON解析器，我毫不怀疑其效率和准确性仍有优化空间。这个解析器满足了我的项目需求，也希望能对你的项目有所帮助！如果你做了任何改进，欢迎告诉我 :)

另外，各位JSON爱好者（说真的，谁会承认自己是JSON爱好者呢……）可能从功能列表中注意到，该解析器并非完全符合官方规范。以下是具体的差异点：
* 单独的"字符串"会被视为有效的JSON。解析器提供可选的"strict"（严格模式）参数，开启后此类输入会触发解析失败（如果这对你很重要的话）。
* `Baked`模式是完全自定义的扩展特性。
* `MaxDepth`解析机制是完全自定义的扩展特性。
* `NaN`和`Infinity`是在JSON标准的后续版本中引入的，部分代码检查工具（linter）会将其标记为错误。

## 编码
编码过程在一定程度上是硬编码实现的。这是因为我无法预知你的数据结构！如果能设计一个通用接口，直接接收整个类并编码其数值/字符串字段，那当然最好，但目前该解析器暂未实现此功能。我摸索出了一些灵活的技巧，通过循环和/或递归方法减少使用该工具时的编码量，但这些技巧都高度依赖具体项目场景。

构造函数、Add和AddField函数现已支持嵌套委托结构。如果你需要在单行代码中创建嵌套的JSONObject，这个特性会非常实用。示例如下：

```C#
void DoRequest(string url, string jsonString) {
    // 网络请求逻辑
}

void Test(string url) {
    DoRequest(url, new JSONObject(request => {
        request.AddField("sort", sort => sort.AddField("_timestamp", "desc"));
        request.AddField("query", new JSONObject(query => query.AddField("match_all", JSONObject.emptyObject)));
        request.AddField("fields", fields => fields.Add("_timestamp"));
    }).ToString());
}
```

## 解码
解码在输入层面要简单得多，同样，如何使用解码后的`JSONObject`也因项目而异。提取数据的一种较复杂的方式是使用递归函数，如下所示。使用格式正确的JSON字符串调用构造函数，会返回包含所有子对象（或数组）的根对象（或根数组），所有数据都封装在一个简洁的引用中！数据存储在名为`list`的公共`ArrayList`中；如果根节点是`Object`类型，还会有一个对应的键列表（名为`keys`！）。如果这部分逻辑让你感到困惑，可以参考以下代码示例和`JSONObject`类中的`print()`方法。如果JSON格式存在错误（或我的代码本身有问题！），调试控制台会输出"improper JSON formatting"（JSON格式不正确）。

```C#
void Test() {
    var encodedString = "{\"field1\": 0.5,\"field2\": \"sampletext\",\"field3\": [1,2,3]}";
    var jsonObject = new JSONObject(encodedString);
    AccessData(jsonObject);
}

void AccessData(JSONObject jsonObject) {
    switch (jsonObject.type) {
        case JSONObject.Type.Object:
            for (var i = 0; i < jsonObject.list.Count; i++) {
                var key = jsonObject.keys[i];
                var value = jsonObject.list[i];
                Debug.Log(key);
                AccessData(value);
            }
            break;
        case JSONObject.Type.Array:
            foreach (JSONObject element in jsonObject.list) {
                AccessData(element);
            }
            break;
        case JSONObject.Type.String:
            Debug.Log(jsonObject.stringValue);
            break;
        case JSONObject.Type.Number:
            Debug.Log(jsonObject.floatValue);
            break;
        case JSONObject.Type.Bool:
            Debug.Log(jsonObject.boolValue);
            break;
        case JSONObject.Type.Null:
            Debug.Log("Null");
            break;
        case JSONObject.Type.Baked:
            Debug.Log(jsonObject.stringValue);
            break;
    }
}
```

解码还支持委托格式，该格式会自动检查字段是否存在，再处理数据，并为字段未找到的场景提供可选的OnFieldNotFound回调参数。示例如下：

```C#
void Test(string jsonString) {
    var list = new JSONObject(jsonString);
    list.GetField("users", users => {
        foreach (var user in users.list) {
            var thisUser = user;
            users.GetField("sessions", sessions => {
                foreach (JSONObject gameSession in sessions.list) {
                    Debug.Log(gameSession);
                }
            }, name => Debug.LogWarning(string.Format("No sessions for user {0}", thisUser["name"].stringValue)));
        }
        
    });
}
```

## 随机访问（时间复杂度O(n)）
我为该类添加了字符串和整数索引器，现在你可以通过以下方式获取数据（基于上文示例）：

```C#
void Test() {
    var jsonObject = new JSONObject("{\"field\":[0,1,2]}");
    var array = jsonObject["field"];
    Debug.Log(array[2].intValue); // 应输出"2"
}
```

## 版本更新日志

### v2.1.3
* 修复输入仅为空数组或空对象时抛出异常的问题
* 清理JSONObject时，将list和keys字段放入对象池并置空，确保与新创建的对象保持一致；
  此前该问题导致MaxDepthWithExcessLevels测试在检查结果时随机失败（部分池化对象包含list但无keys）
* 升级JSONChecker工具，增加对象池相关说明，并补充关于不验证标准JSON格式的备注

### v2.1.2
* 修复解析冒号后包含空白字符的JSON字符串时的异常问题

### v2.1.1
* 修复解析包含多个元素的嵌套数组时的异常问题
* 重构MaxDepth相关测试用例
* 修复MaxDepth功能的若干问题

### v2.1
* 新增异步解析方法
* 重写并优化解析器逻辑
* 修复解析错误
* 重构VectorTemplates，改为使用扩展方法实现
* 新增更多测试用例

### v2.0
* 将JSONObject归入Defective.JSON命名空间
* 优化API命名，提升可读性
* 修复解析错误
* 新增测试用例

### v1.4
重大更新！

* 垃圾回收（GC）性能大幅提升，彻底减少内存垃圾产生！
	* 仍存在的内存垃圾来源：`StringBuilder.Append`/`AppendFormat`、`String.Substring`、`List.Add`/`GrowIfNeeded`、`Single.ToString`等系统内置方法产生的内部垃圾
* 新增异步`Stringify`函数，可在运行时序列化大量数据而不造成帧丢失
* 新增`Baked`类型
* 为解析函数新增`MaxDepth`配置项
* 根据ReSharper的建议完成多项代码清理和重构

### v1.3.2
* 新增对`NaN`的支持
* 新增严格模式，对格式不规范的JSON主动触发解析失败。目前严格模式的判定逻辑为：若解析字符串不以`[`或`{`开头，会打印警告并返回`null`的`JSONObject`实例。
* 重构`infinity`和`NaN`的实现逻辑，改为使用`float`和`double`类型，替代原有的`Mathf`实现
* 优化空对象/空数组的处理逻辑
* 为打印方法和`ToString`方法新增格式化输出开关。顶部的编译宏定义现在可全局禁用格式化输出功能
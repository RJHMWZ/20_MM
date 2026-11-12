# Match-3 三消试玩广告模板完整使用说明书

## 1. 环境要求
| 组件 / 依赖 | 说明 / 推荐版本 |
| --- | --- |
| **Unity 版本** | 当前工程版本为 **Unity 2022.3.62f2**；建议使用相同版本打开，避免 Prefab、粒子和 Spine 资源被自动升级。 |
| **Spine 运行库 (Spine-Unity)** | 用于渲染复杂的 2D 骨骼动画（如消除特效、生成器工作动画）。 |
| **Luna Playable 插件** | 用于最终将项目打包为 HTML5 试玩广告。 |
| **Newtonsoft.Json** | 用于关卡编辑器和打包工具的数据序列化。 |
| **DOTween** | 处理 UI、交换、掉落、特殊元素和结算动画，属于运行时必要依赖。 |

---

## 2. 快速开始
1. **打开场景**：在 Unity 中打开 `Assets/Scenes/main.unity`。
2. **确认主控节点**：在 Hierarchy 窗口中找到 `GameManager` 和 `ConfigManager` 节点。
3. **加载关卡**：确保 `GameManager` 的 `Level Data List` 列表中至少有一个配置好的 `LevelData` 文件，并且挂载了正确的 `ElementDatabase`。
4. **运行测试**：点击 Unity 编辑器顶部的 Play 按钮，体验游戏流程与消除规则。
5. **打包试玩包**：点击顶部菜单 `ExportTools -> Open Window %t`，在弹出的 `Playable Tools` 面板中配置参数，点击“导出”生成可用于投放的 `.zip` 试玩包。

---

## 3. 项目结构完整导览
项目核心目录集中在 `Assets/` 下：
```text
Assets/
├── Scripts/
│   ├── Match3/                     # 三消业务逻辑核心
│   │   ├── Data/                   # 存放所有数据模型（关卡数据、元素花名册、各类型元素的属性定义）
│   │   ├── View/                   # 表现层代码（负责方块、障碍物在棋盘上的实例化、动画播放和位移逻辑）
│   │   ├── UI/                     # 界面表现代码（包含 GameUIManager.cs，控制顶部步数/目标、底部下载按钮、全局遮罩）
│   │   ├── Editor/                 # Unity 编辑器扩展（包含可视化关卡编辑器 LevelEditorWindow.cs 及导出试玩包的工具 PlayableTools.cs）
│   │   ├── GameManager.cs          # 游戏主控（核心管线）
│   │   ├── BoardManager.cs         # 棋盘逻辑（核心管线）
│   │   ├── AudioManager.cs         # 音效（核心管线）
│   │   └── GuideManager.cs         # 强制新手引导（核心管线）
│   │
│   └── Managers/
│       └── ConfigManager.cs        # 全局与运营配置（负责与 Luna Playable 后台对接参数暴露）
│
├── Resources/                      # 运行时 Resources.Load 使用的配置、预制体、图片、音频和 Spine 资源
│   ├── Config/Block/               # ElementDatabase 与各类元素 Data
│   ├── Config/Level/               # level1～Level8 等关卡资产
│   ├── Prefabs/                    # 方块、障碍物、收集物、传送带等预制体
│   └── Arts/                       # UI、方块和启动页图片
├── Particles/                      # 粒子预制体、材质与纹理
├── Effect/                         # 其他特效资源
└── Scenes/main.unity               # 当前主场景
```

---

## 4. 核心概念详解
游戏内主要包含以下几种核心元素：

* **普通元素 (Normal Elements)**：最基础的可消除色块，凑齐 3 个及以上即可消除。
* **特殊/技能元素 (Special Elements)**：
  * 包括横/竖火箭、炸弹、飞鸟和魔力球。
  * 可由玩家四消/T型/L型消除合成，也可直接配置在关卡初始棋盘上。
* **障碍物 (Obstacles)**：阻碍消除或掉落的物体，如木箱、冰块。拥有生命值 (`maxHealth`)，可通过旁边消除或仅受特殊技能伤害（`onlyDamagedBySpecials`）来破坏。
* **长条收集物 (Collectibles)**：如贪吃蛇/火车，占用多个连续格子。具有朝向和长度属性，当头部前方被消除时会整体向前移动。
* **生成器 (Generators)**：如打字机。受到伤害后，激活并生成特定元素到棋盘。可配置产生元素的种类和最大产出数量 (`maxGenerations`)。
* **子母元素 (Mother & Child)**：散落的子元素被消除后，会飞向占据多格的母元素（如苹果飞入果篮）。达成设定数量 (`requiredCount`) 后母元素完成收集。
* **传送带 (Conveyor)**：附着在可玩格上的方向通道，每轮结算后按方向搬运元素；拐角外观由相邻传送带的流入与流出方向自动判断。

---

## 5. 配置系统详解
整个游戏的配置是由**全局配置**、**元素库**和**具体元素档案**组成的铁三角。

### 5.1 全局与运营配置 (ConfigManager)
挂载在场景中的 `ConfigManager` 节点上，这里的参数不仅控制本地运行，还会同步给网页运营后台（Playground），方便投手随时换皮、调优。
* **美术与音频替换**：`LogPic` (Logo)、`IconPic` (应用图标)、`BGPic` (UI底图)、`GridBackgroundPic` (棋盘底图)、`BackgroundMusic` (默认 BGM)。
* **转化率控制 (Ad Settings - 极重要)**：
  * **`adTriggerStep` (触发步数)**：格式为 `关卡索引_步数`（例：`1_3` 表示第1关第3步后强行拦截并弹出广告）。为空则依靠正常的通关/失败触发。
  * **`adTriggerType` (触发类型)**：`InvisibleOverlay` (全屏透明遮罩，玩家一点击屏幕即跳转商店) 或 `DownloadPanel` (弹出明显的 Download UI 面板)。
* **过渡控制 (Level Settings)**：
  * **`levelTransitionType`**：通关后进入下一关的方式。`Popup` (弹窗点击继续) 或 `Fade` (无缝黑屏淡入淡出，保持心流)。

### 5.2 元素总管家 (Element Database)
* **作用**：游戏的总花名册。无论你建了多少种颜色块、炸弹或木箱，都必须把它们拖到这本花名册里，游戏才会承认它们。
* **当前资产路径**：`Assets/Resources/Config/Block/ElementDatabase.asset`。脚本定义位于 `Assets/Scripts/Match3/Data/ElementDatabase.cs`。
* **参数**：包含 `Unified Normal Prefab`（所有普通方块通用的身体），以及下方分门别类的 List（如 Normal Elements, Special Elements 等）。

### 5.3 元素档案详解 (各类 Data)
如果你想新增一种元素，需要在 `Assets` 目录右键 `Create -> Match3 -> 对应的 Data`，填好参数后放入 `Element Database`。

* **普通元素 (NormalElementInfo)**：直接在 Element Database 里配置。
  * `Id`：**（核心）** 身份证号（1=红，2=蓝）。做目标或引导时靠它识别。
  * `Sprite / Spine Skin Name`：贴图或骨骼皮肤名称。
* **技能元素 (Special Element Data)**：炸弹、火箭等。
  * `Behavior Type`：触发时效果（如 `LineClearHorizontal` 横向消除、`BombExplosion` 范围爆炸）。
  * `Creation Condition`：玩家合成的条件（如 `Line4Horizontal` 四消合成）。
  * `Prefab / Sprite`：运行时实体与目标栏图标来源；创建后还需加入 Element Database 的 `Special Elements` 列表。
* **障碍物 (Obstacle Data)**：木箱、冰块等。
  * `Max Health`：在旁边消除几次后才会销毁。
  * `Only Damaged By Specials`：打勾则只能被炸弹/火箭炸掉，普通消除无效。
  * `Is Movable`：打勾则下方空了会往下掉；不打勾则钉死在空中。
* **生成器 (Generator Data)**：打字机、吐币机等。
  * `Hits To Generate`：在旁边消除几次后生成一次东西。
  * `Generated Item Prefab`：它生成的东西的 Prefab。
  * `Max Generations`：最多生成多少个（填 `-1` 为无限）。
  * `Hits To Generate / State Skins / State Animations / Transition Animations`：配置多次受击、阶段皮肤和过渡动画。
* **长条收集物 (Collectible Element Data)**：贪吃蛇等。
  * 仅需配置头、身、尾的三张图片。长度和朝向是在关卡编辑器里画的时候决定的。
* **子母元素 (Mother & Child Data)**：消除苹果飞入果篮。
  * **Mother Data (果篮)**：配置 `Grid Width/Height` (占地大小) 和 `Required Count` (需要收集多少个子元素)。
  * **Child Data (苹果)**：**（核心）** `Target Mother Id` 必须精确填入对应果篮的 ID，否则苹果被消除后不知道飞去哪，会报错。
* **可移动/可交换开关**：障碍物、生成器、母元素等 Data 中的 `Is Movable` 控制重力掉落，`Is Swappable` 控制能否与普通元素交换。多格母元素通常不建议开启交换。

### 5.4 特效与特殊元素配置

特效配置按职责分为“数据资产”“元素预制体”和“统一播放逻辑”三层。新增或替换美术效果时，优先修改 Inspector 中的配置，不要在 `BoardManager` 中直接新增 `Instantiate`。

#### 特殊元素通用特效

在 Project 面板打开 `Assets/Resources/Config/Block/DefaultSpecialElements/`，选择对应特殊元素的 `.asset`，在 Inspector 的 **Effects** 区域配置：

| 字段 | 用途 |
| --- | --- |
| `Activation Effect Prefab` | 特殊元素被触发时的主体效果，例如炸弹爆炸。 |
| `Trail Effect Prefab` | 移动或射线类效果，例如火箭尾迹、魔力球光线。 |
| `Finish Effect Prefab` | 演出结束后的收尾效果；魔力球未配置时会回退使用 `Activation Effect Prefab`。 |

字段定义位于 `Assets/Scripts/Match3/Data/SpecialElementData.cs`。

#### 特殊元素预制体自身效果

在 `Assets/Resources/Prefabs/block/` 中选择对应的 `Special_*.prefab`，在 `SpecialElementView` 组件中配置：

| 字段 | 用途 |
| --- | --- |
| `Idle Particles` | 特殊元素创建后持续播放的待机粒子，可留空。 |
| `Swap Particle Prefab` | 两个特殊元素交换或组合时播放的中心粒子，可留空。 |
| `Creation / Idle / Trigger Animation` | Spine 的创建、待机与触发动画名称。 |
| `Swap Animations` | 按交换对象类型配置的组合 Spine 动画。 |

字段定义位于 `Assets/Scripts/Match3/View/SpecialElementView.cs`。

#### 普通元素消除特效

在 `Assets/Resources/Config/Block/ElementDatabase.asset` 的普通元素列表和 **Global Effects** 区域配置：

| 字段 | 用途 |
| --- | --- |
| `Clear Effect Prefab / Scale` | 每种普通元素自己的消除粒子及其格子缩放倍率。 |
| `Light Wave Effect Prefab / Scale` | 每种普通元素自己的光波及其缩放倍率。 |
| `Global Light Wave Effect Prefab` | 普通元素没有单独光波时使用的全局回退效果。 |

字段定义位于 `Assets/Scripts/Match3/Data/ElementDatabase.cs`。

#### 统一播放规则

`Assets/Scripts/Match3/View/Match3EffectPlayer.cs` 是运行时统一播放入口，不需要挂载到 GameObject，也没有 Inspector 配置。当前普通消除、光波、魔力球收尾和特殊元素交换粒子均通过它播放，统一负责：

* 将特效放入指定棋盘父节点，便于广告或结算时随棋盘一起隐藏和清理。
* 仅在调用方提供缩放值时覆盖预制体缩放，避免素材尺寸被意外重置。
* 显式激活并播放所有子粒子系统，兼容 `playOnAwake = false` 和 Luna/Playworks。
* 一次性特效默认停止循环并在 2 秒后销毁；持续型交换粒子可保留循环，并按演出时长销毁。

配置完成后至少验证普通消除、单个特殊元素触发、特殊元素组合和魔力球组合四类场景，检查尺寸、排序、持续时间及棋盘清理后是否仍有残留。

### 5.5 特殊元素组合行为

两个特殊元素可以直接交换并触发组合。当前实现如下：

| 组合 | 当前行为 |
| --- | --- |
| 火箭 + 火箭 | 以交换位置为中心，同时清除一整行和一整列。 |
| 火箭 + 炸弹 | 清除中心附近 3 行和 3 列。 |
| 炸弹 + 炸弹 | 产生 5×5 范围爆炸。 |
| 飞鸟 + 飞鸟 | 生成 3 只飞鸟，分别寻找优先目标，并清除目标及其上下左右区域。 |
| 飞鸟 + 火箭 | 飞向一个优先目标，再从目标位置发动对应方向的整行或整列清除。 |
| 飞鸟 + 炸弹 | 飞向一个优先目标，在目标位置发动 3×3 爆炸。 |
| 魔力球 + 魔力球 | 两球移动至棋盘中心，聚光演出后清除全棋盘。 |
| 魔力球 + 火箭/炸弹/飞鸟 | 选择一种普通颜色，将该颜色元素转换为对应特殊元素并依次触发。 |

组合动画通过特殊元素 Prefab 上 `SpecialElementView > Swap Animations` 按对方的 `Behavior Type` 配置。没有匹配动画时仍会执行组合逻辑，但演出可能缺失。炸弹+炸弹、魔力球+魔力球和魔力球+其他特殊元素还可在 `AudioManager` 中配置专用音效。

### 5.6 音频配置

场景中的 `AudioManager` 将声音分为三类：

* **BGM**：当前实际播放来源是场景 `AudioManager.Bgm Clip`。`ConfigManager.BackgroundMusic` 虽已暴露给 Playground，但现有运行代码尚未自动同步到 `AudioManager`；如需运营后台换 BGM，应补充同步逻辑后再使用该字段。
* **持续音效**：交换、收集、长条回退、障碍物、生成器与特殊元素触发音效允许重叠播放。
* **帧优先级音效**：同一帧只播放最高优先级请求，目前特殊元素生成高于普通消除，避免连锁时声音堆叠失控。

特殊元素单体音效按 `SpecialBehaviorType` 分别配置横火箭、竖火箭、炸弹、飞鸟和魔力球；障碍物、生成器、子母元素的专属音效配置在各自 Data 中。

---

## 6. 创建新关卡完整教程 (Level Editor)
像画画一样编辑关卡棋盘的地方。
* **如何打开**：顶部菜单 `Tools -> Match3 -> Level Editor`。

### 6.1 基础与规则设定
* **Level Data / Element Database**：把你的关卡文件和花名册拖进最顶部的两个槽位。（如果没有关卡文件，点 `Create New Level Data` 新建）。
* **Max Steps**：设定这关最多走几步。
* **Level Targets**：点 `+` 号添加过关目标（选类型、填 ID、填需要收集的数量）。
* **Grid Settings**：设定棋盘的宽和高（如 7x9）。

### 6.2 新手强制引导 (Tutorial Settings)
* 勾选 `Has Tutorial` 开启，点 `+` 号添加多个 `Tutorial Steps`，步骤会按列表顺序执行。
* 设定玩家按下的格子坐标 (`Swipe From`) 和松开的格子坐标 (`Swipe To`)。
* 在 `Highlight Cells` 添加需要亮起的格子坐标，其余屏幕会变黑。
* `Finger Scale / Finger Offset` 用于微调引导手指；`Show Mask` 控制是否显示暗色遮罩。
* `Show Popup / Popup Prefab / Popup Offset` 可在某一步额外显示提示弹窗及调整位置。
* *注意：被引导滑动的格子，你在下方的画板里必须画成固定的颜色（不能是随机块），否则游戏随机出的颜色不对，玩家滑不动，游戏死锁！*

### 6.3 画笔区与绘制 (Palette & Grid)
* **画笔种类**：
  * `Toggle Playable`：点击格子变红 X，表示此处是空洞。
  * `Erase`：橡皮擦。
  * `Random Normal` (问号 `?`)：开局随机掉落颜色，最常用。
  * `具体元素`：选中特定的颜色、炸弹、障碍物。
  * `Paint Conveyor`：给可玩格添加传送带并选择流出方向。
* **特殊绘制手法**：
  * **长条收集物**：先选好朝向和长度。**去网格上点击时，点的位置是它的“尾巴”**，它会自动长出身子和头。
  * **多占位母元素**：**去网格上点击时，点的位置是它的“左下角”**，它会自动向右和向上覆盖。
  * **传送带**：不能与障碍物、生成器、母元素、子元素或特殊元素重叠；请保证路线方向连贯，并在 Play 模式检查汇入口是否产生冲突。
* **保存**：做完任何修改，按 `Ctrl + S` (或 Cmd + S) 保存。

### 6.4 逐列补充序列 (Column Refill Sequences)

`LevelData.Column Refill Sequences` 的列表索引对应棋盘列索引。每列可预设一串普通元素 ID，补充时按顺序生成；序列耗尽后自动恢复随机普通元素。它适合制作可复现的引导局面和控制前几轮掉落，但 ID 必须存在于 Element Database。

---

## 7. 游戏主控配置 (Game Manager)
关卡做好了，怎么让它正常运行起来？
1. 选中场景中的 `GameManager` 节点。
2. 将你做好的关卡文件（`.asset`）拖入 `Level Data List` 列表中（游戏启动默认读取第 0 个）。
3. 确保 `ElementDatabase` 槽位已挂载你的花名册。
4. 确保相关的 UI (UIManager) 和 棋盘 (BoardManager) 已挂载。

---

## 8. 界面与棋盘引擎说明 (UIManager & BoardManager)

### 8.1 界面管理：GameUIManager
负责游戏里所有“不是方块”的视觉元素。
* **Top Section (顶部区域)**：控制 Logo、目标收集栏、步数显示。
* **Middle Section (中间棋盘区域)**：`Board Area Rect` 是用来框定“棋盘能在屏幕上占多大位置”的隐形框。`BoardManager` 会严格参考这个框，确保方块不溢出屏幕。
* **Endgame Section (结算面板)**：通关或失败时显示的 UI 面板。
* **Global Ad Section (全局强弹广告区)**：对应 `ConfigManager` 里配置的强制弹出广告遮罩。

### 8.2 棋盘主引擎：BoardManager
负责读取关卡数据，生成方块，处理滑动、消除和重力掉落。
* **Board Settings**：`Cell Size` (格子大小)、`Cell Spacing` (间距)、`Board Edge Padding` (边缘内边距，调大可让棋盘整体缩小)。
* **自动适配**：会根据 `GameUIManager` 传来的区域大小，自动缩放 `Elements Container`，适应横竖屏。
* **广告拦截**：每次玩家按下屏幕时，会询问 `ConfigManager` 当前步数是否需要弹广告，如果是则阻断交互并强弹下载页面。
* **传送带**：直线和拐角预制体分别通过 `Resources.Load("Prefabs/ConveyorStraight")` 与 `Resources.Load("Prefabs/ConveyorCorner")` 加载；`Corner Configs` 可针对流入/流出方向调整旋转、翻转和动画名称。
* **特效父节点**：棋盘内临时特效应放在 `Elements Container` 或棋盘主节点下，避免关卡切换、广告遮罩或结算后留下游离对象。

### 8.3 启动加载界面

`GameManager` 启动时会通过 `RuntimeLoadingScreen` 动态创建全屏加载层，不需要在场景中预先搭建 Canvas：

* Logo 从 `Assets/Resources/Arts/Icon/Loading_Logo_en.png` 加载。
* `GameManager > Simulated Loading Duration` 控制模拟进度时长，当前默认值为 1.6 秒。
* 加载层使用 `Screen Space Overlay` 且排序值很高；初始化完成或引用缺失时都会关闭。
* 想换 Logo 时保持 Resources 路径和文件名不变即可；想改背景色、进度条颜色或布局需修改 `RuntimeLoadingScreen.cs`。

---

## 9. 试玩包打包导出 (Playable Tools)
1. 顶部菜单 `ExportTools -> Open Window %t`。
2. 填入 `gameName`、`version`。
3. 点击 **“导出”**。工具会自动将代码参数序列化，并将图片、音效转为 Base64 或压缩格式注入 HTML 中。
4. 最终生成的 `.zip` 包位于 `LunaTemp/stage4/create-hub/` 目录下。
5. 导出工具需要 `Assets/Resources/Screenshot/preview.jpg` 作为预览图；替换时保留路径与文件名。

---

## 10. 常见问题与避坑指南 (FAQ)

**Q1：新手引导卡死，玩家无法滑动？**
* **原因**：引导位置配置了 `Random Normal`（随机块），实际生成的方块凑不成 3 个。
* **解决**：在关卡编辑器中，使用 `Paint Specific Normal`（指定颜色块画笔），把引导区域画成必定能消除的固定阵型。

**Q2：过关目标始终无法完成，或者游戏提前判负？**
* **原因**：目标要求的数量大于棋盘上的总数量，且没有生成器补充；或者要求摧毁的生成物数量大于生成器的最大库存 (`maxGenerations`)。
* **解决**：核对目标数量与实际供应量。若需无限补充，将 Generator 的 `maxGenerations` 设为 `-1`。

**Q3：子元素 (Child) 消除后报错 NullReferenceException 或不飞向果篮？**
* **原因**：`ChildData` 中的 `Target Mother Id` 与果篮的名字拼写不一致，或者棋盘上根本没画果篮。
* **解决**：检查两边 ID 是否完全一致（区分大小写），并确保母元素已被画在棋盘上。

**Q4：触发广告拦截（如第1关第3步）时，棋盘特效挡住了下载按钮？**
* **原因**：WebGL 渲染时，某些 Spine 动画或粒子的 Sorting Layer 过高，穿透了 UI。
* **解决**：`GameManager` 的 `TriggerGlobalAd()` 已经会自动隐藏棋盘。如果你新增了游离在棋盘外的特效，请确保触发广告时将其一同 `SetActive(false)`。

**Q5：点击导出包报错“未能找到 preview 图片”？**
* **原因**：打包工具写死了需要读取一张封面图。
* **解决**：确保工程存在 `Assets/Resources/Screenshot/preview.jpg` 图片。

**Q6：粒子在 Unity 中正常，在 Luna/Playworks 中不显示？**
* **原因**：粒子子节点未激活、`playOnAwake` 关闭、缩放或父节点不一致，或者使用了导出环境不支持的 Shader。
* **解决**：棋盘临时特效统一调用 `Match3EffectPlayer.Play()`；检查粒子材质、Renderer、Sorting Layer 和导出后的 Shader Variant Collection，不要通过 `Simulate()` 强制推进粒子。

**Q7：特殊元素组合能消除，但没有交换动画？**
* **原因**：组合逻辑和 Spine 表演是分开的；Prefab 的 `Swap Animations` 中没有与对方 `Behavior Type` 匹配的绑定，或动画名不存在于所选 SkeletonData。
* **解决**：在对应 `Special_*.prefab` 的 `SpecialElementView` 中补充绑定，并确认 `Swap Spine`、动画名和缩放倍率正确。

**Q8：传送带显示方向不对或多个元素抢同一格？**
* **原因**：相邻格的流出方向不连续、多个入口指向同一目标，或拐角配置缺少对应的流入/流出组合。
* **解决**：在 Level Editor 中逐格检查方向；在 `BoardManager > Corner Configs` 中补齐拐角旋转/翻转设置，并在 Play 模式完整运行至少两个结算回合。

---

## 11. 修改与交付检查清单

提交或导出前建议依次检查：

1. 打开 `Assets/Scenes/main.unity`，确认 Console 没有编译错误和丢失引用。
2. 从 `Level Data List` 中至少试玩一个普通关和一个含引导、传送带或多格元素的复杂关卡。
3. 验证普通三消、特殊元素生成、五类特殊元素单体触发以及 README 5.5 中用到的组合。
4. 检查横竖屏切换后棋盘、目标栏、引导手指、结算 UI 和下载按钮均未溢出。
5. 触发配置的广告步数，确认棋盘、Spine 和粒子不会覆盖下载 UI。
6. 确认胜利、失败、下一关、BGM、普通消除和特殊组合音效没有缺失或异常叠音。
7. 导出前确认 `preview.jpg` 存在，并在生成的 HTML/zip 中再次验证加载页、首局交互、结算和跳转。

维护文档时以 Inspector 字段和脚本实际行为为准。新增 Data 字段、Resources 固定路径、特殊组合或导出前置条件时，应同步更新本 README。

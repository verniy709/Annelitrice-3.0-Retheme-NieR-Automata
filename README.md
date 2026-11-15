# Annelitrice 3.0 Retheme — NieR:Automata

将安妮莉模组的相关设定、物品、建筑、小机械体、文字与四组技能表现改为NieR风格。主视觉采用黑白数据片、暖白柔光，逻辑病毒感染带少量红色侵蚀。

## 使用与依赖

- 适用RimWorld 1.7，依赖原模组Annelitrice 3.0及其依赖，加载顺序位于原模组之后。
- 包名为`Verniy709.Annelitrice3.0.Retheme.NieRAutomata`；原模组包名为`Verniy709.Annelitrice3.0`。
- 不建议同时启用其他针对同一原模组的retheme，以免同一Def的显示内容和效果被覆盖。
- 修改Def、贴图、声音配置或DLL后，重启游戏以重新加载。
- 此目录中的源码用于维护，游戏运行使用`1.7/Assemblies/Annelitrice.Retheme.NieRAutomata.dll`。

## 主题与中文用语

| 原有对象 | 本retheme的显示内容 |
|---|---|
| 安妮莉种族 | 安妮莉机械生命体 |
| 幼体 | 安妮莉小机械体 |
| 卵 | 已注册或未注册的存储单元 |
| 蛹 | 机械茧 |
| 营养液 | 回收生物质 |
| 驱虫药 | 病毒疫苗 |
| 喷泉设施 | 资源回收单元 |
| 木偶剧场 | YoRHa终端 |
| 寄生感染能力 | 逻辑病毒感染 |

原有DefName及翻译键保留，不把显示名称写回标识。例如`Anneli_Pupa`仍是机械茧的ThingDef，`Anneli_Infest`仍是感染能力，背景故事标识中的`Worm`也沿用原模组，以保持引用稳定。

## 四组特效与声音

| 事件 | 视觉 | 声音 |
|---|---|---|
| 逻辑病毒感染 | 黑白数据传递、红色侵蚀、粒子与柔光 | core_72为主体；视觉结束后约0.18秒加入core_79沙沙声 |
| 存储单元激活 | 启动闪光、数据展开、分段光环与扫描显现 | core_35，完整约2.148秒 |
| 机械茧形成 | 数据向内吸入、分层包裹、封口亮闪与向下扫描 | core_12主体；第29tick加入core_2扫描提示 |
| 重建完成 | 封壳向两侧解锁、暖白亮闪、壳片与数据向外释放 | core_96主体约2.657秒；第6tick加入三下短促拆解声 |

四组声音共10个SoundDef，游戏音量统一为`75~75`，音高为`1~1`，保留已确认的分层比例。声音位于`Sounds/Anneli_Retheme_NieRAutomata/`下对应四组效果的子目录。十份音频统一采用 Ogg Vorbis；九份原 WAV 以质量等级 8 转换，保留 48 kHz 采样率与原有单声道/双声道布局；已有的存储激活 OGG 逐字节保留，没有重复压缩。主体、尾声、扫描与拆解是独立的一次性声音，视觉清理不会主动截断已经播放的声音。游戏加速和中途切换速度可能改变自然播放声音之间的重叠关系；感染主体另有标准加速版本。

感染可见效果约1.33秒；存储激活约1秒；机械茧形成约1.4秒；重建完成约1.6秒。效果以游戏tick推进，精确生命周期及强度参数以对应源码为准。视觉仅负责表现，不接管原模组的生成、复活、阵营或行动流程。

开发者模式可用小机械体的`DEV:Cocoon`检查结茧；用存储单元的`DEV:Hatch`检查激活；用机械茧的`DEV:Hatch`检查重建完成。两个Hatch按钮属于不同对象。

## 文件布局与命名

```text
About/About.xml
LoadFolders.xml
README.md
1.7/
  Assemblies/Annelitrice.Retheme.NieRAutomata.dll
  Defs/SoundDefs/                 四组效果与建筑环境音
  Patches/                       按原模组Def类型分类
  Languages/ChineseSimplified/  DefInjected与Keyed
  Languages/English/Keyed/
  Source/Annelitrice.Retheme.NieRAutomata/
    LogicVirus/
    StorageActivation/
    CocoonFormation/
    ReconstructionComplete/
Sounds/Anneli_Retheme_NieRAutomata/  四组效果与BuildingAmbience
Textures/Anneli_Retheme_NieRAutomata/
```

- 补丁按`AbilityDef.xml`、`EffecterDef.xml`、`ThingDef.xml`等类型组织，沿用原模组的DefName和字段路径。
- 新声音以`Anneli_Retheme_NieRAutomata_`为前缀，避免覆盖原模组或其他retheme的SoundDef。
- 新C#类型统一使用`Annelitrice.Retheme.NieRAutomata`命名空间，位于原模组`Annelitrice`命名空间之下；XML中的类名与DLL类型一致。
- 中文翻译按原模组的Def类型分类。`AlienRace.ThingDef_AlienRace`、`AlienRace.AlienBackstoryDef`目录与原模组一致，键使用`DefName.字段`；代码界面文案使用原有`Anneli.*` Keyed键。
- 源码和程序集均位于对应版本的`1.7`目录。工程默认按安装位置解析RimWorld目录，也可通过`RimWorldDirectory`属性指定。

## 贴图与维护

常规贴图位于`Textures/Anneli_Retheme_NieRAutomata/`下的`Anneli_Building`、`Anneli_Item`、`Anneli_Larva`、`Anneli_UI`目录。方向后缀`_north`、`_east`、`_south`与染色遮罩后缀`m`沿用原模组范式；`Dessicated_Larva_east.png`沿用原资源路径的拼写。方向图即使内容相同，也保留各自文件名供Graphic_Multi读取。

存储单元和机械茧使用目录内的随机外观；批量物品使用原有堆叠图规则。添加新图时应保留对应前缀和方向/遮罩命名。

四组程序特效共用两张基础贴图：

- `Textures/Anneli_Retheme_NieRAutomata/Anneli_Effect/Solid.png`：实色矩形。
- `Textures/Anneli_Retheme_NieRAutomata/Anneli_Effect/SoftGlow.png`：带透明渐变的柔光。

具体形状、数量、运动、颜色与透明度由`LogicVirusGeometry.cs`、`StorageActivationGeometry.cs`、`CocoonFormationGeometry.cs`、`ReconstructionCompleteGeometry.cs`生成。对应控制器负责地图绘制和声音触发，`*AudioTiming.cs`负责时序。

有一处刻意保留的原路径贴图：`Textures/Anneli_Item/Pupa/PupaA.png`。原模组`CompLarva`按这个固定路径读取结茧开关图标，因此它与独立贴图根中的机械茧A外观同时保留。不要仅按文件内容重复就删除它。

旧Fleck的无效贴图重定向已移除；四组新效果不依赖那些旧粒子图。旧FleckDef仍由原模组提供并使用原资源路径。

## 源码编译

需要.NET 9 SDK及当前RimWorld 1.7的程序集。在本模组根目录执行以下PowerShell命令，把编译临时文件放到模组以外：

```powershell
dotnet build .\1.7\Source\Annelitrice.Retheme.NieRAutomata\Annelitrice.Retheme.NieRAutomata.csproj -c Release -p:BaseIntermediateOutputPath="$env:TEMP\AnneliNieR\obj\" -o "$env:TEMP\AnneliNieR\bin"
Copy-Item -LiteralPath "$env:TEMP\AnneliNieR\bin\Annelitrice.Retheme.NieRAutomata.dll" -Destination .\1.7\Assemblies\Annelitrice.Retheme.NieRAutomata.dll
```

## 本次整理（2026-11-15）

十一份源码按四组效果归档，文件内容逐字节保留；项目使用四组目录的明确编译路径，避免误包含构建缓存。四组声音定义分别为 `LogicVirus.xml`、`StorageActivation.xml`、`CocoonFormation.xml`、`ReconstructionComplete.xml`。SoundDef 名称、音量、音高与并发参数保持原样，仅更新 clipPath 的分组路径。

九份 WAV 转为高质量 OGG，已有的一份 OGG 只移动归档。转换后检查了解码格式、时长、波形时间对齐与音量。原始 WAV 和安装前完整备份保存在模组目录外，游戏目录不保留重复 WAV。声音自然尾音及各组的延迟逻辑保持原样。

源码重新编译通过；游戏目录继续保留此前已确认的程序集，视觉几何与声音时序未改动。所有常规贴图、方向图、固定路径图标、中文翻译和原有补丁保留。现有配方的 `Cook` 工作效果和 `Recipe_Machining` 工作声音改动也纳入备份并保留。

此前已清理的旧 Fleck 无效重定向与占位文件不再加入；两张共用特效基础贴图继续使用。`.git` 和 `.gitattributes` 保留。本文件为模组内唯一说明；检查记录、完整备份、回退脚本与发布包存放在目录外。

安装本次更新后，请重启游戏以加载新的音频路径和 OGG 文件。此次整理完成编译与离线检查，格式转换后的实际游戏听感需重启后体验。


## 资源回收装置的上浮光粒（Fountain）

`Anneli_Fountain` 仅保留暖白色发光粒子。光点从基座左右两侧及建筑表面生成，缓缓向上飘动、淡出。左右分别每 9 / 11 tick、建筑表面每 15 tick 生成一个光点，寿命约 1.8 秒，大小 0.035～0.060。

使用三个原生 `CompProperties_ThrownFleckEmitter` 与一个 `Anneli_Retheme_NieRAutomata_RecoveryLightPoint` FleckDef。`velocityX` 为运动角度，围绕 0 度小幅变化；`velocityY = 0.275~0.375` 为移动速度，为之前的一半。表面光点的角度限制为 -4～4 度，主要竖直上浮。MoteGlow 材质复用原有 `Anneli_Effect/SoftGlow.png`。已移除光丝群定义及两张专用光丝贴图。

定义位于 `1.7/Defs/Effects/Fleck_Fountain.xml`，接入位于 `1.7/Patches/ThingDef.xml`。条件补丁保留现有组件，使用 Normal tick；放置后持续播放，暂停随游戏 tick 停止，移除建筑后停止生成。更新后请重启游戏加载。预览直接读取实际贴图与参数，属于离线模拟，并非游戏实录。


## YoRHa终端的偶发静电与电火花（PuppetTheatre）

`Anneli_PuppetTheatre` 的屏幕、机身表面与左右边缘偶尔闪现冷白略带淡蓝的细小电弧，外侧夹杂短促电火花。每次约 0.14～0.17 秒，六处发射器使用 211 / 277 / 337 / 419 / 311 / 389 tick 的错开间隔；单处约每 3.5～7 秒触发一次，大部分时间保持清晰。

使用六个原生 `CompProperties_ThrownFleckEmitter` 和三个原生 FleckDef，采用普通 Graphic_Fleck 与 MoteGlow 材质。BuildingOnTop 层使静电显示在建筑表面；电弧停留在出生处，少量火花向外短促飘动。配合下方配置的终端广播。

定义位于 `1.7/Defs/Effects/Fleck_PuppetTheatre.xml`，贴图位于 `Textures/Anneli_Retheme_NieRAutomata/Anneli_Effect/Anneli_PuppetTheatre/`：`StaticArc.png`、`StaticFork.png`、`SparkBurst.png`。贴图为确定性生成的透明几何遮罩；预览直接使用同一批贴图和参数，属于离线模拟。

已移除月之泪定义、贴图和专用摇摆源码、工程与程序集。Fountain 的慢速上浮光斑及原有四组技能、音效、配方与建筑美术保持原样。接入仍在 `1.7/Patches/ThingDef.xml`，保留原组件与 Normal tick。更新后重启游戏加载。


## 两项建筑循环环境音

PuppetTheatre 使用用户提供的 `vo_core_1_000_1.ogg` 作为 YoRHa 终端循环广播，保留现有版本及用户设置。

Fountain 以用户提供的 `BGM_0_008_9.ogg` 为主音轨，完整约 107.28 秒。2026-11-16 按用户要求，在上一版基础上再次对主音轨增加 **2 dB**（振幅约乘 1.2589），九段蒸汽、齿轮、活塞声音分别再增加 **4 dB**（振幅约乘 1.5849）。两次调整累计：相对首次增响前的原主音轨 **+4 dB**、厚重机械声版本 **+8 dB**。本轮从首次增响前的备份重新编码，减少反复重编码造成的损失；仅做恒定增益，保留速度、音高、完整时长与动态，不对主音乐添加压缩或限幅。

所有运行声音继续使用 Ogg Vorbis、48 kHz、质量等级 10。主音轨为双声道，机械变体为单声道。主文件位于 `Sounds/Anneli_Retheme_NieRAutomata/BuildingAmbience/FountainAmbience.ogg`，九段机械声位于同目录 `FountainMechanics/`。

机械声保留厚重的低中频、金属拖动、齿轮咬合、活塞落位和短余振。三个独立原生 SubSoundDef 各有三段变体，使用 `sustainLoop=false`、随机等待及 `NeverTwice` 选择；音乐循环不重置机械声的时间。所有声层暂停时静音，随建筑移除停止，最近音源规则仍为同类建筑一个。

安装时保留用户当前 SoundDef 的全部参数：

| 声层 | 本轮文件增益 | volumeRange | 首次等待（秒） | 每段结束后等待（秒） | distRange |
| --- | --- | --- | --- | --- | --- |
| 主音轨 | +2 dB | `60~100` | 保持原设置 | 连续循环 | `5~20` |
| 蒸汽泄压 | +4 dB | `60~100` | 3~6 | 2~6 | `5~20` |
| 齿轮/棘轮 | +4 dB | `60~100` | 5~9 | 2~6 | `5~20` |
| 活塞移动与落位 | +4 dB | `60~100` | 7~12 | 2~6 | `5~20` |

### 调整机械声出现频率

修改 `1.7/Defs/SoundDefs/FountainAmbience.xml` 中 Steam、Gears、Pistons 三个机械声层的 `sustainIntervalRange`。数值越小，触发越频繁；单位是秒。当前保留用户设置的 `2~6`，例如可进一步缩短为：

```xml
<sustainIntervalRange>1~3</sustainIntervalRange>
```

含义是每段声音播放完之后，再随机等待 1～3 秒播放下一段；完整重复周期还包含声音本身的时长。`startDelayRange` 只控制建筑声源启动后第一次播放的等待时间，不控制后续频率。只调整机械声层，主音轨保持连续循环。保存配置后重启游戏加载。

声音仍使用 `Anneli_Retheme_NieRAutomata_FountainAmbience` 和一个 `CompProperties_AmbientSound`。本次未新增组件、SoundDef 或程序集；合计 21 份 OGG、12 个 SoundDef。技能、贴图、特效代码及其他音效均保留现有版本。增益前版本完整备份于模组目录外，解码及随机叠加峰值检查已完成。

### Fountain 机械素材来源

以下附加素材作者均为 **bart**，来源页标注为 **CC0 1.0**。素材已剪辑、柔化、降调和重新混音；这些许可仅适用于附加机械素材，不改变用户提供的 NieR 音轨权属。

- [Steam release sounds](https://opengameart.org/content/steam-release-sounds)：`steam hisses - Marker #1.wav`、`#3.wav`、`#5.wav`，用于泄压及活塞排气。
- [Chain winch sounds](https://opengameart.org/content/chain-winch-sounds)：`winch - Marker #6.wav`，用于齿轮/绞盘转动。
- [68 Workshop Sounds](https://opengameart.org/content/68-workshop-sounds)：`workshop - ratchet1.wav`、`ratchet2.wav`、`thuds.wav`、`quiet scrape.wav`、`dragging metal.wav`、`clink thud.wav`、`low hammering.wav`、`lower hammering.wav`、`machine.wav`，用于齿轮咬合、金属拖动、活塞落位、短余振和蒸汽压力的低频质感。

原文件、制作母带、55 秒试听、素材记录和完整旧版备份均在模组目录外。试听只模拟一种可能的随机组合，实际游戏不会固定重复该试听顺序。原生解码器和字段检查已通过；安装后重启游戏加载新的分层定义。临时诊断程序已移除。

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

四组声音共10个SoundDef，游戏音量统一为`75~75`，音高为`1~1`，保留已确认的分层比例。声音位于`Sounds/Anneli_Retheme_NieRAutomata/`。主体、尾声、扫描与拆解是独立的一次性声音，视觉清理不会主动截断已经播放的声音。游戏加速和中途切换速度可能改变自然播放声音之间的重叠关系；感染主体另有标准加速版本。

感染可见效果约1.33秒；存储激活约1秒；机械茧形成约1.4秒；重建完成约1.6秒。效果以游戏tick推进，精确生命周期及强度参数以对应源码为准。视觉仅负责表现，不接管原模组的生成、复活、阵营或行动流程。

开发者模式可用小机械体的`DEV:Cocoon`检查结茧；用存储单元的`DEV:Hatch`检查激活；用机械茧的`DEV:Hatch`检查重建完成。两个Hatch按钮属于不同对象。

## 文件布局与命名

```text
About/About.xml
LoadFolders.xml
README.md
1.7/
  Assemblies/Annelitrice.Retheme.NieRAutomata.dll
  Defs/SoundDefs/SoundEffects.xml
  Patches/                       按原模组Def类型分类
  Languages/ChineseSimplified/  DefInjected与Keyed
  Languages/English/Keyed/
  Source/Annelitrice.Retheme.NieRAutomata/
Sounds/Anneli_Retheme_NieRAutomata/
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

## 本次整理

已合并分散的效果补丁、声音定义和说明，删除空占位说明、旧粒子兼容图及无效贴图补丁，将完全相同的效果基础贴图合为共享资源，并把版本专用源码归入`1.7/Source`。原有音频、常规美术、四组效果运动、时序及音量保持。

`.git`和`.gitattributes`是版本管理资料，整理时保留；本说明为模组内唯一Markdown说明。检查记录、备份、回退脚本与完整发布包保存在模组目录以外。

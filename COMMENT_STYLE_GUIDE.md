# CalamityEntropy PRT 迁移注释风格指南

> 给"为 PRT 粒子迁移改动补注释"这件事定规矩：写什么、不写什么、用什么语气。
> 目标只有一个：注释读起来像**这个模组的作者边迁移边随手写下来的**，而不是事后由 AI 盖章生成的文档。
> 技术事实一律以 `PRT_MIGRATION_PLAN.md`（尤其附录 D 常见陷阱清单）和 InnoVault 源码为准，本指南只管"怎么说"。

---

## 1. 目的与定位

### 1.1 为什么要补注释

本次迁移把 EParticle 自研系统（52 个子类、约 480 处 spawn）、VoidParticles/AbyssalParticles、PixelParticle、以及约 580 处 CalamityMod `GeneralParticleHandler` 调用**全部**翻译成了 InnoVault 的 `BasePRT`。迁移纪律是"翻译不是改进"，所以产物是干净但**零注释**的机械翻译结果。问题在于：

- 迁移过程里踩过的坑（双重位移、进度方向反转、跨模组贴图占位符、SpriteBatch 配对）只活在迁移手册里，代码现场没有任何痕迹。半年后有人（包括未来的自己）改这些类，大概率原地再踩一遍。
- 一些看起来"多余"或"怪"的代码（`Texture => CEUtils.WhiteTexPath`、`sb.End()` 后紧跟 `BeginDrawingWithMode`、恒 `return false` 的 `PreDraw`）没有解释，会被好心人当垃圾清理掉，然后炸。

### 1.2 什么叫"活人感"

判断标准是一个图灵测试：**把补完注释的文件拿给一个不知情的 tModLoader 模组作者看，他应该认为这些注释是原作者在迁移当时顺手写的**。具体特征：

- 注释是"备忘"不是"文档"：写给三个月后的自己，不是写给评审
- 有情绪、有取舍、有具体事实；不均匀、不对称、不完美
- 该省的字全省，该啰嗦的地方连写三行
- 本仓库已经存在这样的注释（见 3.2、3.4 的存量示例），新注释混进去要无违和

### 1.3 适用范围

- **适用**：本次 PRT 迁移触及的源码注释（`///` XML 文档注释、`//` 行注释），主要在 `Content/Particles/`、`Content/Particles/CalamityPorts/`、共享资源类、`Common/EffectLoader.cs` 接线处、以及各调用点文件
- **不适用**：聊天、PR 描述、Wiki、hjson 本地化文本（面向玩家的文字全部走 hjson，这是继承自 CWR 的底线）
- 本任务**只补注释不改代码**；如果发现注释没法写（因为代码本身可疑），记下来单独处理，不要顺手改

---

## 2. 继承的基础规范（来自 CalamityOverhaul Doc）

底线规则取自 `CalamityOverhaul/Doc/modules/CONVENTIONS.md` 的 **"Comments (style discipline)"** 一节（`Doc/INDEX.md` 与 `Doc/AI-QUICK-REFERENCE.md` §12 均指向该节，无独立内容）。逐条列出继承情况：

| # | CWR 规则 | 本仓库执行 |
|---|----------|-----------|
| 1 | 注释读起来像开发者的简短备忘，不是 AI"伪人"文档 | **全盘继承**，这是本指南的立身之本 |
| 2 | `//` 双斜杠后**不加空格**（`//中文`，禁止 `// 中文`）；代码与 `//` 之间的对齐空格保留（`float x;  //0-1`） | **继承为默认**。本仓库存量注释绝大多数就是 `//中文` 无空格风格；偶尔写出带空格的不必返工（真人不会 100% 一致，见 3.5），但批量补注释时默认无空格 |
| 3 | `///` 三斜杠 XML 文档注释不适用上条：保持 C# 惯例，`///` 与 `<summary>` 之间留空格 | **继承** |
| 4 | 禁 `——` 破折号：每文件 ≤1 处；用 `：`、`、`、`,` 或拆行替代 | **继承** |
| 5 | 中文 summary 用短语/标签，句尾**不加 `。`**（英文注释用英文标点） | **继承**，并推广到 `//` 行注释：默认不写句尾句号 |
| 6 | 禁"职责套话"：`负责/用于/实现/处理/确保/注意/一个额外的/该方法/此字段/当…时会…` | **全盘继承**，已并入第 3.6 节 AI 腔黑名单 |
| 7 | 不复述明显代码；不写面向玩家的机制说明或设定文案（那些进 hjson / Wiki） | **继承** |
| 8 | `///` 在它该在的地方保留：public API、魔法数字、`ai[]` 索引、帧窗口、服务端/myPlayer/MP 分歧、`see cref` 耦合。问题在风格，不在有没有 summary | **继承**（对应本指南 4.5：正经场合照样写正经 XML 注释） |
| 9 | 清理=改写/缩短，不是批量删除；保留魔法数字单位；不追求"注释行数"KPI | **继承**，且反向同样成立：本任务是补注释，**不要为凑数逐行加** |
| 10 | `.fx` 文件：不用 `///`，uniform 含义/范围写在声明行尾 `//` | 本次迁移未触及 `.fx`，暂不适用；将来碰到按 CWR 规则执行 |

一句话总结继承关系：**CWR 管底线（格式、禁语、何时必须写 XML），本指南在底线之上加"活人感"要求（语气、密度分布、吐槽尺度）**。两者冲突时以 CWR 底线为准——活人感不是违反格式规则的借口。

---

## 3. 活人感核心原则

### 3.1 写"为什么"，不写"是什么"

代码已经说了"是什么"，注释只值得花在"为什么"上。本次迁移的"为什么"分三类，全都真实存在：

1. **为什么这么迁**（设计决策）：Configure 统一签名是为了 480 个调用点能机械替换；PRTDrawMode 放 Configure 不放 SetProperty 是因为同一个粒子类在不同调用点传的 BlendState 不一样
2. **为什么不能动**（行为一致性约束）：数值全是老代码原样照抄，改了就不是迁移前的手感了
3. **为什么这么怪**（坑的现场）：`Texture` 指着一张白图、`PreDraw` 恒 `return false`、`sb.End()` 连着 `BeginDrawingWithMode`

```csharp
//反例(复述代码,零信息):
Velocity *= 0.85f;   //将速度乘以0.85

//正例(讲约束):
Velocity *= 0.85f;   //衰减系数是Calamity原值,迁移纪律:数值一个不改
```

### 3.2 吐槽可以，对象是代码不是人

允许骂旧实现、骂 XNA、骂框架设计、骂自己当年（或迁移时）的失误。**不允许**点名骂任何真人：Red、Fabsol、Calamity 团队、HoCha113、队友、玩家，一个都不行（详见第 9 节红线）。

本仓库存量的语气锚点（`Common/EModPlayer.cs` 1578 行附近，真人手笔）：

```csharp
//恶意代码debuff
//加了大修增加一些效果强度
//那很恶意了
```

`Common/EffectLoader.cs` 99–102 行有一段更凶的存量吐槽（骂 `EndCapture` API 那段）：**能量和真实感可以学，但它后半段指名道姓针对了真人，那半句是越线示范，新注释禁止效仿**。存量注释不动它，新写的以"骂 API 不骂人"为界：

```csharp
//可以:
//要我说EndCapture这套设计就应该沉没在历史里

//不可以:
//(任何把矛头指向具体真人的句子)
```

### 3.3 口语化、幽默与爆粗分级

按强度分三级，重点是**频率预算**——脏话的冲击力靠稀缺性，满屏都是就成了另一种 AI 腔：

| 级别 | 词汇示例 | 频率预算 | 使用场景 |
|------|---------|---------|---------|
| L0 平实 | 无情绪词，就事论事 | 默认，占 85% 以上 | 绝大多数注释 |
| L1 轻度 | 草、绝了、离谱、服了、要命、什么玩意、吐了 | 每文件 ≤2 处 | 确实反直觉/麻烦的地方 |
| L2 重度 | 卧槽、tm、他妈的、见鬼；"傻逼"只能形容代码或设计 | **整个迁移范围内个位数** | 真踩过的大坑（双倍位移、占位符白图、批次炸掉这种查了很久的） |
| 禁区 | 歧视性词汇（针对任何群体）、涉政、性羞辱、针对真人的任何攻击 | 零容忍 | 无 |

幽默的最好来源是**真实存在的怪事**，不用编。现成素材：`PRT_CustomPulse.PreDraw` 里天顶世界+周二会把贴图换成猛犸象（Calamity 官方彩蛋，移植时保留了）：

```csharp
//是的,天顶世界的周二会画猛犸象,Calamity原版彩蛋,照搬,不是bug别删
```

### 3.4 踩坑叙事

"这里坑了我一晚上"式注释是活人感的核心构件，但有一条铁律：**坑必须真实存在**（对照 `PRT_MIGRATION_PLAN.md` 附录 D），语气可以夸张，事实不能编造。句式参考：

```csharp
//框架自己会Position+=Velocity,旧AI里那行忘删就是双倍速度,坑过一次了
//旧的进度是1走到0,新的LifetimeCompletion是0走到1,方向反的,替换错了动画倒放,别问我怎么知道的
//End忘一次查一下午,XNA的SpriteBatch状态机就这德行
```

存量参照（`Common/EGlobalProjectile.cs` 1081 行，真人手笔的坑位警告长这样）：

```csharp
//修复部分射弹导致其他射弹不显示
//不要在绘制完射弹把SpriteSortMode设置成Immediate
```

署名与日期：一般不签名。个别大段吐槽/重要决策可以留个日期（存量代码有 `//----HoCha113 2025-5-6` 的先例），**只能用真实的迁移时间（2026-07），且不得伪造他人签名**。

### 3.5 不完美感

真人注释的分布是**聚集式**的：坑多的文件话痨，顺的文件一路无话。刻意追求以下"不完美"：

- **长短不一**：有的地方三行小作文，有的地方仨字（`//懒得改`）
- **密度不均**：约三分之一的文件可以一条注释都没有（简单模板类真的没什么可说的）
- **偶尔留 TODO / HACK / FIXME**：必须带具体内容和条件，空洞的 `//TODO:优化` 是 AI 腔（见 4.4）
- **标点随意**：句尾不加句号（继承规则）；中英文逗号混用不追究；可以用 `~` `…`；**不用 emoji**
- **允许轻微的风格漂移**：比如某几处 `//` 后带了空格、某处用了全角括号，不必回头修——但这是"允许发生"，不是"刻意制造"，批量生成时仍按默认规则写

### 3.6 AI 腔黑名单

出现以下任何一条，直接重写：

**句式黑名单**

| # | AI 腔句式 | 问题 |
|---|----------|------|
| 1 | "此方法用于…" / "该函数负责…" / "这个类的主要作用是…" | CWR 明令禁止的职责套话 |
| 2 | "我们在此实现…" / "我们需要确保…" | 谁是"我们"？备忘录没有"我们" |
| 3 | "首先…其次…然后…最后…" | 真人不给自己的代码写演讲稿 |
| 4 | "值得注意的是…" / "需要注意的是…" | 直接说坑是什么，别铺垫 |
| 5 | "为了确保…以便…从而…" 连接词串烧 | 翻译腔 |
| 6 | "// 初始化变量" "// 返回结果" "// 遍历所有粒子" | 逐行复述代码 |
| 7 | "// 更新粒子的生命周期状态以反映当前帧" | 完整主谓宾的翻译腔长句 |
| 8 | "// TODO: 后续优化" "// TODO: 实现该功能" | 空洞 TODO，没有内容和条件 |
| 9 | "// 设置透明度 // 设置缩放 // 设置旋转" | 排比对称三连 |
| 10 | "【注意】" "✨" "⚠️" 及一切 emoji 花活 | 一眼机器 |

**分布特征黑名单**（单句没问题，整体一看就是 AI）：

- 注释密度均匀：每个方法头上都有一段、每段等长
- 每个 public 成员都配了 XML 注释（真人只给会被外部调用、或含义不自明的写）
- 所有注释都是完整句子、标点完美、以句号结尾
- 每条注释都彬彬有礼、零情绪、零取舍痕迹

---

## 4. 注释类型清单

每类给出基于**真实迁移改动**的示例。示例里的技术事实全部可以在 `PRT_MIGRATION_PLAN.md`、InnoVault 源码（`PRTLoader.cs`、`BasePRT.cs`）或本仓库现有代码里验证。

### 4.1 迁移原因注释（为什么从 EParticle 换成 BasePRT）

用在类头或关键成员上，讲清"这个形状为什么长这样"。不需要每个类都有，同一句话全仓库写一遍就够了。

```csharp
//老EParticle是自己维护List每帧updateAll/drawAll,接线散在EModSys和EffectLoader好几处
//换成InnoVault的PRT后生命周期和分桶绘制全是框架的事,这类只管AI和PreDraw
```

```csharp
//EParticle系迁移类的Configure签名统一成(opacity, glow, mode, rotation, lifetime)
//为的是400多个调用点能机械替换,改签名前先想想那些调用点
//CalamityPorts那批不遵守这个,它们的Configure对齐的是Calamity原构造参数
```

```csharp
//旧构造参数全转public字段,NewParticle<T>要求无参构造
//spawn完要么走Configure要么直接赋字段,和旧对象初始化器一一对应
```

```csharp
//PRTDrawMode故意放Configure不放SetProperty:同一个粒子在不同调用点传的BlendState不一样
//固定在SetProperty里就把NonPremultiplied那批调用点的表现改了
```

### 4.2 坑位警告注释（这里动了会炸）

**这是本次补注释的最高优先级**。四大坑必须在现场留痕：

**坑 1：跨模组贴图走 `Texture` 属性会拿到占位图**

InnoVault 的 `PRTLoader` 用 `ModContent.HasAsset` 校验 `Texture` 路径，`@CalamityMod/...` 这种 `@` 前缀语法只有 `[VaultLoaden]` 认，`HasAsset` 直接查不到，结果是日志里一条 Warn、游戏里一块占位图。所以本仓库跨模组贴图统一走 `PRTSharedAssets`：

```csharp
//Texture属性走不了@CalamityMod/...语法(@只有VaultLoaden认,HasAsset查不到)
//不报错,就是日志一条Warn然后游戏里给你一块占位图
//跨模组贴图一律走PRTSharedAssets的VaultLoaden,这里随便指张白图应付框架
public override string Texture => CEUtils.WhiteTexPath;
```

```csharp
//这类不走常规绘制(PreDraw恒false,数据由EffectLoader拿去做RT合成)
//但Texture留空串框架会按命名空间猜路径,猜不到又刷Warn,干脆借PRT_Light的图堵上
public override string Texture => "CalamityEntropy/Content/Particles/PRT_Light";
```

```csharp
//CustomPulse的贴图路径是每个调用点现传的,没法VaultLoaden,走PRTPathTextures做运行时缓存
//千万别在PreDraw里直接ModContent.Request,每帧一次能把帧率吃出坑
Texture2D tex = PRTPathTextures.Get(TexPath);
```

**坑 2：SpriteBatch Begin/End 配对**

`PreDraw` 被调用时框架的 SpriteBatch 批次是开着的。自己 `End()` 画图元/换状态之后，必须用 `PRTLoader.BeginDrawingWithMode(PRTDrawMode, sb)` 把批次按当前模式接回去：

```csharp
//进来时PRT的批次是开着的,要画图元只能先End
//画完必须BeginDrawingWithMode按当前模式重开,少这一步同桶后面的粒子全遭殃
sb.End();
//...图元绘制...
sb.End();
PRTLoader.BeginDrawingWithMode(PRTDrawMode, sb);
```

```csharp
//DrawGlow(setState:true)内部End完会把批次停在Deferred+AlphaBlend,跟PRT批次状态对不上
//所以这里还得End一次再让框架重开,这两行看着多余,删了就花屏
CEUtils.DrawGlow(Position, Color * 0.8f, Scale * 0.4f);
sb.End();
PRTLoader.BeginDrawingWithMode(PRTDrawMode, sb);
```

```csharp
//shader和顶点构建从旧Draw原样搬的,一个字没动
//动过的只有首尾:开头End掉框架批次,结尾BeginDrawingWithMode还回去
```

**坑 3：CanPool 池化状态泄漏**

```csharp
//CanPool开着,实例会复用:新加字段必须来Reset()登记
//忘了登记=下一个粒子带着上一个的脏状态出生,这种视觉bug极难查
public override void Reset()
```

```csharp
//odp是轨迹点List,池化复用忘Clear就是上一条轨迹凭空闪现
//这类干脆不开CanPool,spawn频率也不高,不差这点GC
```

```csharp
//字段在声明处用Main.rand初始化的注意:池化复用不会重新跑字段初始化
//随机值要么挪SetProperty要么Reset里重掷,不然复用的粒子全长一个样
```

**坑 4：寿命与位移语义**

```csharp
//Lifetime默认-1=永生,漏设兜底这粒子就永远不死,一路堆到上限
if (Lifetime <= 0) Lifetime = 200;   //旧基类默认值,子类改过默认的用子类的值
```

```csharp
//框架自动Position+=Velocity,旧AI里那行必须删,忘删就是双倍速度
//位置由贝塞尔插值算的这种,直接ShouldUpdatePosition()=>false把框架位移关掉
```

```csharp
//旧系统从不杀出屏粒子,这里必须false
//默认true的话追踪类粒子一出屏就没,行为就变了
ShouldKillWhenOffScreen = false;
```

### 4.3 吐槽注释（骂旧代码 / 骂 XNA / 骂自己）

对象只能是代码、框架、设计、自己。频率遵守 3.3 的预算。

```csharp
//旧代码spawnNew和NewParticle是同一个函数的两个名字,签名一模一样
//用哪个全看当天心情,迁移时统计调用点统计到吐
```

```csharp
//旧逻辑只认Additive和AlphaBlend,其他一律扔进NonPremultiplied桶
//包括你老老实实传BlendState.NonPremultiplied的情况,对,写等价逻辑的时候我也愣了一下
//迁移保持这个语义,别自作聪明加分支
```

```csharp
//XNA的SpriteBatch就是个全局状态机,End忘一次查一下午
```

```csharp
//EXPLOSION这个类名全大写,保留原样没做美化,迁移纪律是翻译不是重构
//看着难受?看着难受也别改,贴图文件名跟类名绑着呢
```

```csharp
//vd=速度衰减 ad=透明度衰减,老字段名,难听但调用点太多,改名的收益配不上风险
```

### 4.4 临时方案 HACK / TODO / FIXME 注释

必须带**具体内容**和**解除条件**，不写空头支票：

```csharp
//HACK:PRT_Abyssal继承PRT_Void一半是为了让EffectLoader分流时能is出来
//虚空遍历那边过滤写的是 is PRT_Void && is not PRT_Abyssal,别嫌怪,RT合成分两套shader
//另一半是它的AI阈值0.05和没有multShrink分支,都是旧AbyssalParticles的原值,别合并回父类
```

```csharp
//TODO:这类暂时没开CanPool,spawn频率不高先不折腾
//哪天要开记得把entity引用在Reset里置null,不然池子里攒一堆死NPC的引用
```

```csharp
//FIXME:Calamity的GeneralDrawLayer和PRT的RenderLayer对不上,没有"弹幕前/后"这种层
//Silence的黑色VoidSpark原意图是盖在蓝光上面,现在用默认层
//游戏里如果发现黑的被蓝光盖了,把这类的RenderLayer提到AfterPlayers
```

```csharp
//HACK:密度节流阈值256/128是拿全体在场PRT数近似旧的EParticle.particles.Count
//语义不完全等价(旧的只数EParticle),但阈值本来就是拍的,效果一致就行
```

### 4.5 XML 文档注释（正经场合）

public API、共享资源类、接口契约照样写正经 `///`——正经不等于套话，summary 用短语/标签、结尾不加句号、有信息量：

```csharp
/// <summary>
/// 跨模组粒子贴图统一入口：PRT 的 Texture 属性走不了 @ 语法（HasAsset 认不出），
/// 跨模组贴图全部在这里用 <see cref="InnoVault.VaultLoadenAttribute"/> 静态加载
/// 禁止在 PreDraw 里 ModContent.Request
/// </summary>
internal static class PRTSharedAssets
```

```csharp
/// <summary>
/// 像素化 RT 通道粒子，受 Config.EnablePixelEffect 门控，配置关闭时不显示（旧行为，别加回退绘制）
/// 绘制发生在 EffectLoader.PreparePixelShader，不走 PRT 常规分桶
/// </summary>
internal interface IPixelPassPRT
```

```csharp
/// <summary>
/// 动态路径贴图缓存：CustomPulse 系的贴图路径是调用点现传的，没法 VaultLoaden，只能运行时缓存
/// </summary>
internal static class PRTPathTextures
```

```csharp
/// <summary>统一补参入口，参数顺序对齐旧 NewParticle 尾参：(opacity, glow, mode, rotation, lifetime)</summary>
public PRT_Smoke Configure(float opacity, bool glow, PRTDrawModeEnum mode, float rotation = 0f, int lifetime = -1)
```

---

## 5. 密度与位置规范

### 5.1 必须注释的位置

| 位置 | 原因 |
|------|------|
| `Texture` 指向白图/借用贴图的类 | 不解释必被"好心修复" |
| `PreDraw` 里自己 End/Begin 的每一处（含 `DrawGlow` 后的收尾） | 坑 2，删一行就花屏 |
| `CanPool == true` 类的 `Reset()` | 坑 3，新增字段漏登记极难查 |
| **有意不开** CanPool 的类（轨迹 List、Player 引用、播音效的） | 防止后人"顺手优化"成池化 |
| `ShouldUpdatePosition() => false` | 说明位移由谁接管（贝塞尔/子步进/entity 跟随） |
| `HomingSpiritParticle` 的 6 次子步进循环 | 唯一用 UpdateTimes 的类，循环里保留 `Position += Velocity` 是故意的 |
| `PRT_Abyssal : PRT_Void` 继承 | 类型分流给 EffectLoader 过滤用；AI 阈值与父类不同是旧值不是笔误 |
| `1f - LifetimeCompletion` 出现且上下文不自明处 | 进度方向反转，替换错=动画倒放 |
| BlendState 第三分支落桶（调用点映射不直观处） | 旧语义"非 Additive 非 AlphaBlend 全进 NonPremultiplied" |
| 批量 spawn 外的 `Main.dedServ` 守卫 | 孤儿实例语义，别用 null 判断 |
| `NegentropyBulletProjectile` 密度节流阈值 | 新旧计数语义不完全等价 |
| 周二猛犸彩蛋 | 不解释必被当 bug 删 |

### 5.2 禁止注释的位置

- `Configure` 的逐参数说明（签名全项目统一，说一遍就够）
- 逐行绘制代码（`sb.Draw(...)` 上面写"绘制主体"这种）
- 字段名已经自明的字段（`public bool Glow`）
- `using`、`namespace`、类声明上的空洞横幅
- 每个 `override` 上写"重写自基类"
- 全仓库出现几十次的模板行**不要每处都注释**：`ShouldKillWhenOffScreen = false` 和寿命兜底在一两个代表性文件里写透即可，其余文件留白（每处都写同一句话是最显眼的 AI 特征）

### 5.3 每文件条数指导

| 文件类型 | 建议条数 |
|---------|---------|
| 简单模板化粒子类（A 类，纯搬运） | 0~2 条 |
| 带自定义批次/池化/接口的粒子类（B 类） | 2~5 条 |
| 共享资源类（PRTSharedAssets 等） | 类级 XML 1 条 + 个别条目行尾 0~2 条 |
| CalamityPorts 移植类 | 1~3 条 |
| 武器/NPC/Buff 调用点文件 | 通常 0 条，最多 1~2 条 |
| EffectLoader 等接线处 | 按坑的数量，宁缺毋滥 |

整体感觉：**约三分之一的文件一条都没有**。注释总量宁少勿多，一条有信息量的顶十条凑数的。

---

## 6. 语言混用规则

- **中文为主**。专有名词、类型名、API、字段、枚举值保留英文原文：`BasePRT`、`PRTDrawMode`、`AdditiveBlend`、`sb.End()`、`VaultLoaden`、RT、shader、spawn、tick
- **禁止翻译 API**："精灵批次"（×）→ SpriteBatch（√）；"渲染目标"看语境，代码相关写 RenderTarget/RT
- 中英混排不加空格也行（`旧EParticle是`），加了也行，不追究
- 单位随手写：tick、帧、px；数值范围写 `0-1` 或 `0~1` 都可以
- 标点：句尾句号默认不写（继承 CWR）；中英文逗号混用不追究；`——` 每文件最多 1 处；`~` `…` 可以用；emoji 禁止
- 全角/半角括号不强求统一
- `///` XML 注释里稍微收敛一点：短语式、可用 `<see cref>`，仍然不写句尾句号

---

## 7. Do / Don't 对照表

| # | Don't（AI 腔） | Do（活人版） |
|---|---------------|-------------|
| 1 | `// 此方法用于设置粒子的生命周期` | `//旧基类默认200,照抄,子类改过默认的用子类的` |
| 2 | `// 我们在此处结束SpriteBatch以便开始新的绘制批次` | `//End完必须BeginDrawingWithMode接回去,不然同桶后面的粒子全遭殃` |
| 3 | `// 遍历所有顶点并将其添加到列表中` | （删掉，这行代码不需要注释） |
| 4 | `// 注意：这里需要特别注意服务端的情况` | `//dedServ时NewParticle返回的是孤儿实例不是null,别拿null判断` |
| 5 | `// 将透明度乘以0.98以实现淡出效果` | `//衰减系数是老代码原值,迁移纪律:数值一个不改` |
| 6 | `// 首先获取纹理，然后计算动画帧，最后进行绘制` | `//6帧竖排,80x80,Variant横排7列`（只注帧布局这个非直观点） |
| 7 | `// 该字段表示速度衰减系数` | `//vd=速度衰减 ad=透明度衰减,老字段名,调用点太多懒得改` |
| 8 | `// TODO: 优化此处性能` | `//TODO:要开CanPool的话先把entity引用在Reset置null,不然池子里攒死引用` |
| 9 | `// 为了确保行为一致性，我们将ShouldKillWhenOffScreen设置为false` | `//旧系统从不杀出屏粒子,默认true会把追踪粒子出屏就杀了,行为就变了` |
| 10 | `// 使用白色纹理作为占位符` | `//Texture走不了跨模组路径,随便指张白图应付框架,真图在PreDraw里拿` |
| 11 | `// 重置所有字段到初始状态，以便对象池复用` | `//池化复用,新加字段忘了来这登记=下一个粒子带脏状态出生` |
| 12 | `/// <summary>此类是从CalamityMod移植而来的烟雾粒子类</summary>` | `//照Calamity的HeavySmokeParticle翻的,Update逻辑一行没动,贴图跨模组走PRTSharedAssets` |
| 13 | `// 判断是否到达生命周期终点，如果是则移除粒子` | `//到点自杀是框架的事,这里只管alpha衰减到0.02提前Kill` |
| 14 | `// 此接口定义了像素化绘制的相关方法` | `//EnablePixelEffect关着的时候这批粒子压根不画,旧行为,别加回退` |
| 15 | `// 值得注意的是，LifetimeCompletion的方向与旧系统相反` | `//旧的是1→0剩余比例,新的是0→1已过比例,方向反的,替换错了动画倒放` |

---

## 8. 按文件类型的注释侧重

### 8.1 PRT 粒子类（`Content/Particles/*.cs`）

- 侧重：迁移语义（进度方向、双重位移、寿命兜底）与绘制批次配对
- A 类模板化搬运的类大多留白；有自定义批次（`Trail.cs`、`ERing.cs`、`ShadeDashParticle.cs`、`GlowSpark.cs`）的类批次配对**必须**写
- 池化类（47 个）的 `Reset()` 至少在高频代表类里写脏状态警告；**有意跳过池化**的类（`PRT_TrailParticle`、`PRT_PlayerShadow`、`PRT_RealisticExplosion` 等）写明跳过原因，防止后人顺手"优化"
- `SetProperty` 与 `Configure` 的执行顺序坑（SetProperty 在 NewParticle 内部先跑，读不到 Configure 的值）在受影响的类里点一句

### 8.2 共享资源类（`PRTSharedAssets` / `PRTExtraTextures` / `PRTPathTextures` / `IAdditivePRT.cs`）

- 侧重：**为什么存在**。类级 XML 注释讲清占位符坑（@ 语法只有 VaultLoaden 认）和每帧 Request 的性能问题
- 个别条目可以行尾短注（`MammothParticle` 是彩蛋用图这种）
- 这是全仓库最"正经"的注释区，风格向 4.5 靠拢

### 8.3 武器 / NPC / Buff 调用点（`Content/Items/Weapons/` 等约 180 个文件）

- 默认**零注释**。spawn 一个粒子不需要说"生成粒子"
- 只有三种情况值得写：批量 spawn 外补的 `Main.dedServ` 守卫（为什么突然多了这行）、BlendState 第三分支落桶（旧调用传的值和新 mode 看起来对不上时）、旧对象初始化器转字段赋值里的非直观映射（`alpha → p.Opacity` 这种改名）
- 每文件最多 1~2 条，多数文件一条不加

### 8.4 CalamityPorts 移植类（`Content/Particles/CalamityPorts/`）

- 文件头一条：从 Calamity 哪个类翻的、逻辑动没动（"照 CalamityMod 的 XxxParticle 翻的，数值没动"）
- 贴图注意事项：跨模组引用方式（走 PRTSharedAssets / PRTPathTextures，不复制贴图文件）
- `GeneralDrawLayer → PRTRenderLayer` 对应缺口用 FIXME 型注释挂在受影响的类/调用点上（Silence 的遮挡关系那条）
- 命名后缀 `Cal` 的由来（避免和本模组同名粒子撞车）值得在一两处提一句

### 8.5 接线处（`Common/EffectLoader.cs`、`Common/EModSys.cs`）

- 旧系统拆除后的空位**不要写墓碑注释**（"这里曾经是 EParticle.updateAll"——git 记得，代码不用记得）
- 保留下来的 RT 合成遍历（`is PRT_Void && is not PRT_Abyssal` 过滤）必须写清为什么过滤条件长这么怪
- `PreparePixelShader` 里 IPixelPassPRT 的三桶顺序（AlphaBlend → NonPremultiplied → Additive，复刻旧序）值得一条

---

## 9. 红线（不可越）

1. **技术内容必须真实**。注释声称的行为必须与代码/框架实际行为一致。写之前对照 `PRT_MIGRATION_PLAN.md` 附录 D 或 InnoVault 源码；拿不准的细节宁可不写
2. **不写与代码矛盾的注释**。数值、路径、类名、行为描述与代码对不上的注释比没有注释更糟；将来改代码必须同步改注释
3. **踩坑故事的坑必须真实存在**。语气可以夸张（"查一下午"），坑本身不能编造；不给不存在的问题立牌坊
4. **爆粗永远不针对真人**。不点名 Red、Fabsol、任何模组作者、贡献者、队友、玩家；骂只骂代码、API、设计。存量注释里的越线内容不动也不学
5. **无歧视性词汇**（针对任何民族、地域、性别、群体）、不涉政、不涉黄
6. **不留敏感信息**：真实姓名、本机路径（`C:\Users\...`）、联系方式、内部群号、密钥
7. **不伪造历史**：日期只写真实迁移期（2026-07）；不伪造他人签名；不编造"某某版本就有这个问题"的时间线
8. **不在注释里保存大段被删的旧代码**（git 有）；一两行新旧对照可以

---

## 附录：写注释前的事实速查

写手（人或 AI）动笔前过一遍，保证第 9 节第 1 条：

| 事实 | 出处 |
|------|------|
| `Lifetime < 0` 永生（默认 -1）、`== 0` 当帧消亡、兜底必须 | 迁移手册 §2.1 / 附录 D-1 |
| 框架自动 `Position += Velocity` 与寿命计数 | 附录 D-2/D-3 |
| 进度方向：旧 `Lifetime/TimeLeftMax` 1→0，新 `LifetimeCompletion` 0→1 | 附录 D-4 |
| dedServ 孤儿实例非 null | 附录 D-5 |
| BlendState 三分支：非 Additive 非 AlphaBlend 全进 NonPremultiplied | 附录 D-6 / `EParticle.cs`(已删) 216–226 的旧逻辑 |
| `ShouldKillWhenOffScreen = false` 保持旧行为 | 附录 D-7 |
| `Texture` 属性不认 `@` 跨模组前缀，`HasAsset` 查不到给占位图+Warn | InnoVault `PRTLoader.cs` 贴图加载逻辑；`@` 前缀仅 `VaultLoad.cs` 的 VaultLoaden 路径处理支持 |
| `RenderLayer` 只能在 SetProperty 设 | 附录 D-9 |
| `SetProperty` 先于 `Configure` 执行 | 附录 D-11 |
| 池化 47 类；跳过名单及原因 | 迁移手册阶段 7 / 迁移日志 |
| 周二猛犸彩蛋 | `PRT_CustomPulse.PreDraw`（zenithWorld + Tuesday） |
| DrawGlow(setState:true) 收尾停在 Deferred+AlphaBlend | `CEUtils.cs` DrawGlow 实现 |
| 密度节流 256/128 换成全体在场 PRT 数 | 迁移手册阶段 4 B-9 |

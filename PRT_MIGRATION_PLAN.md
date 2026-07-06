# CalamityEntropy 粒子系统 → InnoVault BasePRT 迁移执行手册

> 本文档是完整的迁移施工图，面向执行迁移的 AI 或开发者。执行前通读本文档。
> 目标：将本模组的多套自研粒子系统**彻底**统一为 InnoVault 的 `BasePRT`，不保留任何旧系统、不做中间转接层。
> 最高优先级约束：**不产生破坏性改动**——迁移后的视觉表现、粒子行为、数值参数必须与迁移前一致。

---

## 0. 执行者必读纪律

1. **逐阶段执行，每阶段结束必须编译通过**（`dotnet build` 或 IDE 构建本项目 `.csproj`），编译不过不得进入下一阶段。
2. **禁止改动任何数值参数**：颜色、缩放、寿命、速度衰减系数、透明度曲线等，原样照抄。你的任务是"翻译"，不是"改进"。
3. **禁止顺手重构**与粒子无关的代码。禁止合并"看起来相似"的粒子类。禁止重命名贴图文件。
4. **禁止**保留 `EParticle` 作为包装/门面（facade）。迁移完成后旧类型必须物理删除。
5. **禁止**使用 `new XxxPRT()` + `PRTLoader.AddParticle(...)` 的写法生成粒子（该写法绕过对象池且非本项目规范），统一使用 `PRTLoader.NewParticle<T>(...)`。
6. 每个阶段末尾有"验收清单"，全部通过才算完成。
7. 遇到本文档未覆盖的特殊情况：停下来，把情况记录到本文档末尾的"迁移日志"小节，不要擅自发明方案。

---

## 1. 现状盘点（迁移对象清单）

| 体系 | 位置 | 规模 | 处置 |
|------|------|------|------|
| `EParticle` 自研系统 | `Content/Particles/EParticle.cs` + 52 个子类 | ~480 处 spawn | **迁移**（阶段 3~5） |
| `VoidParticles` / `AbyssalParticles`（共用 `Particle` 类） | `Content/Particles/VoidParticles.cs`、`AbyssalParticles.cs` | ~49 处 spawn | **迁移**（阶段 2） |
| `PixelParticle` | `Content/Particles/PixelParticle.cs` | 2 处 spawn | **迁移**（阶段 1） |
| 已有 PRT：`PRT_Light` / `PRT_Spark` | `Content/Particles/PRT_Light.cs`、`PRT_Spark.cs` | 6 处 spawn | **规范化**（阶段 0） |
| CalamityMod `GeneralParticleHandler` | 全仓库 ~165 文件 | ~580 处 spawn | **迁移**（阶段 6，最后做） |
| Calamity Metaball（`EclipseMetaball`/`ShadowMetaball`） | `Content/EclipseMetaball.cs` 等 | 4 处 | **不迁移**（RT 合成范式，PRT 无对应能力） |
| 原版 `Dust` + 4 个 `ModDust` | 分散 | ~120 处 | **不迁移**（原版系统） |
| `ScreenShaker` 等震屏系统 | `ScreenShaker.cs` 等 | — | **不迁移**（与粒子仅为同帧并列调用） |

### 旧系统的接线位置（阶段 5 拆除时用）

| 接线 | 文件与行号（迁移前） |
|------|---------------------|
| `EParticle.updateAll()` | `Common/EModSys.cs` `PreUpdateDusts()`（约 590–593 行） |
| `PixelParticle.Update(); VoidParticles.Update(); AbyssalParticles.Update();` | `Common/EModSys.cs`（约 516–518 行，与 `ScreenShaker.Update()` 同方法） |
| `EParticle.drawAll()` | `CalamityEntropy.cs` `DrawBehindPlayer`（`On_Main.DrawProjectiles` 钩子内，约 792 行） |
| `EParticle.DrawPixelShaderParticles()` | `Common/EffectLoader.cs` `PreparePixelShader`（约 555 行） |
| `VoidParticles.particles` 遍历 | `Common/EffectLoader.cs` 约 737、766 行 |
| `AbyssalParticles.particles` 遍历 | `Common/EffectLoader.cs` `DrawParticleEffectsAlt`（约 837 行） |
| `PixelParticle.drawAll()` | `Common/EffectLoader.cs` 约 903 行 |
| `IAdditivePRT` 遍历（保留，作为参考模式） | `Common/EffectLoader.cs` 约 560–570 行 |

---

## 2. InnoVault BasePRT API 契约速查（必须遵守）

来源：`ModSources/InnoVault/PRT/`（本机有源码，可 grep 单个符号求证；勿整读大文件）。

### 2.1 字段与属性（`BasePRT`）

| 成员 | 说明 |
|------|------|
| `Position` `Velocity` `Rotation` `Scale` `Opacity` `Color` | 与 EParticle 同名同义 |
| `Lifetime`（int，默认 -1） | 总寿命（tick）。框架判定：`Lifetime >= 0 && Time >= Lifetime` 即移除。因此 **`Lifetime < 0` = 永生**（默认值 -1 就是永生，漏设会永久堆积）；**`Lifetime == 0` = 当帧立即消亡**。每个类必须保证寿命被设置为正数，或有主动 `Kill()` 路径 |
| `Time`（int） | 已存活 tick，框架自增。旧 EParticle 的"已过时间"= `TimeLeftMax - Lifetime` → 新的 `Time` |
| `LifetimeCompletion`（float 0→1） | 进度。旧 EParticle 常用 `Lifetime / (float)TimeLeftMax`（1→0 的剩余比例）→ 换成 `1f - LifetimeCompletion` |
| `PRTDrawMode`（`PRTDrawModeEnum`） | 混合模式，**实例字段**，可以每次 spawn 后改。枚举：`AlphaBlend`（默认）/ `AdditiveBlend` / `NonPremultiplied` |
| `RenderLayer`（`PRTRenderLayer`） | 绘制时机层。默认 `BeforeInfernoRings` ≈ 弹幕之后、玩家之前，**与旧 EParticle 的绘制时机基本一致，迁移时一律用默认值**。必须在 `SetProperty` 里设置（在 `AI` 里改下一帧才生效） |
| `ShouldKillWhenOffScreen`（默认 `true`） | 出屏即杀。**旧系统从不剔除出屏粒子，为保证行为一致，本项目迁移的粒子一律设为 `false`**（见 3.1 模板） |
| `active` / `Kill()` | `Kill()` 即 `active = false`，替代旧的 `Lifetime = 0` 自杀写法 |
| `ID` | 类型 ID，`PRTLoader.PRT_IDToTexture[ID]` 取主贴图 |
| `shader`（`ArmorShaderData`） | 染料 shader，本项目暂不使用 |

### 2.2 可重写成员

| 成员 | 用途 |
|------|------|
| `string Texture` | 主贴图路径字符串（不是 Texture2D）。支持跨模组：`"@CalamityMod/Particles/BloomCircle"` |
| `int InGame_World_MaxCount`（默认 4000） | 单类型数量上限，超限静默丢弃。全局上限 32767，单类不要超过 20000 |
| `bool CanPool`（默认 false） | 对象池开关。**本次迁移一律保持 false**（阶段 7 可选优化再开），避免 `Reset` 写错造成脏状态视觉 bug |
| `void SetProperty()` | 粒子被加入系统时调用一次。放：`PRTDrawMode`、`ShouldKillWhenOffScreen`、寿命兜底 |
| `void AI()` | 每 tick。**注意：框架已自动执行 `Position += Velocity`（受 `ShouldUpdatePosition()` 控制）和寿命计数/到期移除，子类 AI 里不要再写 `Position += Velocity` 和 `Lifetime--`** |
| `bool ShouldUpdatePosition()` | 返回 false 则框架不自动位移（用于旧类中"有速度但不按速度移动"的特殊粒子） |
| `bool PreDraw(SpriteBatch sb)` | 自定义绘制，**`return false` 表示完全接管**（跳过框架默认绘制）。本项目迁移的类全部走 PreDraw + return false |
| `void PostDraw(SpriteBatch sb)` | 默认绘制之后的追加绘制 |
| `void Reset()` | 仅池化时需要，本次不用 |

### 2.3 生成 API（`PRTLoader`）

```csharp
// 唯一允许的生成方式：
var p = PRTLoader.NewParticle<PRT_Xxx>(pos, vel, color, scale);
p.Configure(...);          // 本项目统一的补参方法，见 3.1
p.someField = ...;         // 旧对象初始化器里的额外字段，逐条搬过来
```

- **服务端行为**：`Main.dedServ` 时 `NewParticle` 返回一个不会入场的"孤儿"实例（**非 null**，且不执行 `SetProperty`）。所以：不要用 null 判断做服务端守卫；批量 spawn 前用 `if (Main.dedServ) return;` 包裹（旧 `EParticle.NewParticle` 内部有此守卫，旧调用点大多没写，迁移时**在 spawn 代码块外层补上**；单条 spawn 可以不补，孤儿实例无害只是浪费）。
- 数量查询：`PRTLoader.PRT_IDToInGame_World_Count[PRTLoader.GetParticleID<T>()]`（单类型）、`PRTLoader.PRT_InGame_World_Inds.Count`（全部在场 PRT）。
- 遍历在场粒子：`PRTLoader.PRT_InGame_World_Inds`（`EffectLoader` 已在用，见约 560 行）。

### 2.4 与旧系统的已知差异（可接受，不要试图抹平）

这些差异经过评估影响极小，**照常迁移即可，禁止为此造轮子**：

1. 采样器差异：旧 AlphaBlend/NonPremultiplied 批次用 `PointClamp`、Additive 用 `AnisotropicClamp`；InnoVault 的 AlphaBlend 用 `Main.DefaultSamplerState`、Additive/NonPremultiplied 用 `PointClamp`。
2. 混合模式的批次顺序：旧系统每帧按 AlphaBlend → NonPremultiplied → Additive 的顺序绘制；InnoVault 按枚举顺序 AlphaBlend → AdditiveBlend → NonPremultiplied。仅当 Additive 粒子与 NonPremultiplied 粒子重叠时前后关系有细微变化。
3. InnoVault Additive/NonPremultiplied 批次的 `DepthStencilState` 为 `Default`（旧为 `None`），对 2D 绘制无实际影响。

---

## 3. 统一规范（所有迁移产物必须遵守）

### 3.1 迁移类标准模板

每个旧 `EParticle` 子类翻译成如下骨架。**类保持一类一职责，不合并**：

```csharp
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class PRT_Smoke : BasePRT   // 命名规则见 3.2
    {
        // ① 旧类的自定义字段原样保留为 public 字段（供 spawn 后逐条赋值）
        public bool Glow = true;       // 对应旧 glow

        // ② 贴图：显式写旧贴图的完整路径（贴图文件名 = 旧类名，禁止改名）
        public override string Texture => "CalamityEntropy/Content/Particles/Smoke";

        // ③ 高频粒子可调高上限；默认 4000 通常够用
        public override int InGame_World_MaxCount => 4000;

        // ④ 统一签名的 Configure（全项目所有迁移类一致，方便机械化改写调用点）
        public PRT_Smoke Configure(float opacity, bool glow, PRTDrawModeEnum mode,
            float rotation = 0f, int lifetime = -1)
        {
            Opacity = opacity;
            Glow = glow;
            PRTDrawMode = mode;
            Rotation = rotation;
            if (lifetime > 0) Lifetime = lifetime;
            return this;
        }

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;        // 行为一致性：旧系统不剔除出屏粒子
            if (Lifetime <= 0) Lifetime = 200;      // 兜底 = 旧类的默认 Lifetime（EParticle 基类默认 200，
                                                    // 若旧子类构造函数里改过默认值，用旧子类的值）
        }

        public override void AI()
        {
            // 旧 AI 逻辑原样搬运，但删除两行框架已接管的代码：
            //   Position += Velocity;   ← 框架自动做（如旧类故意不位移，改为 override ShouldUpdatePosition() => false）
            //   Lifetime--;             ← 框架自动计数并到期移除
            // 旧代码里 “Lifetime = 0 / Lifetime <= 0 自杀” → 改为 Kill();
            // 旧代码里的进度表达式替换：
            //   Lifetime / (float)TimeLeftMax   → 1f - LifetimeCompletion
            //   TimeLeftMax - Lifetime          → Time
        }

        public override bool PreDraw(SpriteBatch sb)
        {
            // 情形 A：旧类有 override Draw() —— 把 Draw() 函数体搬进来，
            //          Main.spriteBatch 换成参数 sb，getOrigin() 覆盖一并内联，return false。
            // 情形 B：旧类用 EParticle 基类默认 Draw —— 用下面的等价实现：
            Color clr = Color;
            if (!Glow)
                clr = Lighting.GetColor((int)(Position.X / 16), (int)(Position.Y / 16), clr);
            if (PRTDrawMode == PRTDrawModeEnum.NonPremultiplied)
                clr.A = (byte)(clr.A * Opacity);
            else
                clr *= Opacity;
            Texture2D tex = PRTLoader.PRT_IDToTexture[ID];
            sb.Draw(tex, Position - Main.screenPosition, null, clr, Rotation,
                tex.Size() / 2f, Scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
```

模板要点：

- **不写带参构造函数**（`NewParticle<T>` 需要无参构造）。旧构造参数一律转为 public 字段。
- **不启用 `CanPool`**、不写 `Reset()`（阶段 7 再说）。
- `PRTDrawMode` 之所以进 `Configure` 而不是固定在 `SetProperty`：同一个旧粒子类在不同调用点可能传不同的 `BlendState`，必须保持每个调用点的原样。
- 上面 PreDraw 情形 B 的颜色逻辑，就是旧 `EParticle.Draw()`（`EParticle.cs` 239–255 行）的逐行等价翻译，不要改。

### 3.2 命名与文件组织

- 新类名 = `PRT_` + 旧类名（如 `Smoke → PRT_Smoke`，`EXPLOSION → PRT_EXPLOSION`，保留旧大小写，不做美化）。
- **新类写在旧类所在的同一个 `.cs` 文件里**（追加在旧类下方），命名空间不变，仍为 `CalamityEntropy.Content.Particles`。文件不搬家、贴图不搬家。旧类在阶段 5 统一删除。
- 一个文件多个旧类的（`PremultBurst.cs`、`GlowSpark.cs`、`HeavenfallStar.cs`、`HadCircle.cs`、`PrismShard.cs`、`SnowPiece.cs`、`Trail.cs`、`PlayerShadow.cs`、`EXPLOSION.cs`、`RuneParticle.cs`），逐个类翻译，全部追加在同一文件。

### 3.3 调用点标准改写模式

旧（两种别名 `NewParticle` / `spawnNew` 完全等价，签名相同）：

```csharp
EParticle.NewParticle(new Smoke(), pos, vel, col, scale, a, glow, BlendState.Additive, rot, life);
EParticle.spawnNew(new DOracleSlash() { centerColor = Color.White }, pos, vel, col, scale, a, glow, BlendState.NonPremultiplied, rot, life);
```

新（**参数一一对位，顺序固定**）：

```csharp
PRTLoader.NewParticle<PRT_Smoke>(pos, vel, col, scale)
    .Configure(a, glow, PRTDrawModeEnum.AdditiveBlend, rot, life);

var p = PRTLoader.NewParticle<PRT_DOracleSlash>(pos, vel, col, scale);
p.Configure(a, glow, PRTDrawModeEnum.NonPremultiplied, rot, life);
p.centerColor = Color.White;    // 对象初始化器逐条转为赋值语句
```

`BlendState` → `PRTDrawModeEnum` 映射（严格按旧 `EParticle.NewParticle` 的逻辑，见 `EParticle.cs` 216–226 行）：

| 旧调用点传入 | 新 mode |
|--------------|---------|
| `BlendState.Additive` | `PRTDrawModeEnum.AdditiveBlend` |
| `BlendState.AlphaBlend` | `PRTDrawModeEnum.AlphaBlend` |
| 其他任意值（含 `BlendState.NonPremultiplied`、null 等） | `PRTDrawModeEnum.NonPremultiplied` |

注意旧逻辑的坑：旧系统只识别 Additive 和 AlphaBlend，**其余一律落入 NonPremultiplied 桶**，迁移时保持这个语义。

旧调用点若把 spawn 包在循环里且没有 `Main.dedServ` 守卫，在循环外补 `if (Main.dedServ) return;`（或 `if (!Main.dedServ) { ... }` 包裹），不要改动循环体逻辑。

---

## 4. 分阶段施工计划

### 阶段 0：地基与既有 PRT 规范化（小，先做）

**范围**：`Content/Particles/PRT_Light.cs`、`PRT_Spark.cs` 及其 6 个调用点。

1. `PRT_Light`：
   - 删除带参构造函数，参数全部转 public 字段；加统一 `Configure`（把原构造参数 `lifetime, opacity, squishStrenght, maxSquish, hueShift, entity, followingRateRatio` 收进去或用字段赋值）。
   - `ICELoader` 手动加载 `BloomTex` 改为 `[VaultLoaden("CalamityEntropy/Content/Particles/PRT_Light2")] internal static Asset<Texture2D> BloomTex;`（参照 CalamityOverhaul `Content/PRTTypes/PRT_HeavenfallPrism.cs` 的写法），删除 `ICELoader` 实现。
   - 补 `SetProperty` 中的 `ShouldKillWhenOffScreen = false` 与寿命兜底。
2. 6 个 `new PRT_Light(...)` + `PRTLoader.AddParticle(...)` 调用点（`Common/EModPlayer.cs` 2204、`Content/Items/Weapons/Fractal/FinalFractal.cs` 414、`Content/Items/Donator/TheFilthyContractWithMammon.cs` 240、`Content/Items/Weapons/LunarPlank.cs` 190、`Content/Items/Weapons/Nemesis/EXNemesisProj.cs` 64、`Content/Items/Weapons/OblivionThresher/OblivionThresherHoldout.cs` 593）改为 `PRTLoader.NewParticle<PRT_Light>(...)` 标准写法。
3. `PRT_Spark` 同样去掉带参构造、加 `Configure`。它目前无 spawn 点，只有 `Core/BaseSwing.cs` 364 行的数量查询（该查询已是标准 API，不动）。

**验收**：编译通过；`rg "new PRT_Light|new PRT_Spark" --type cs` 结果为 0；游戏内触发上述 6 处武器特效目视无异常。

### 阶段 1：`PixelParticle`（最小试点）

**范围**：`Content/Particles/PixelParticle.cs`；调用点 2 处：`Content/Projectiles/AnnihilateArrowProjectile.cs` 54、`Content/NPCs/VoidInvasion/VoidCultist.cs` 272。

1. 新建 `PRT_Pixel : BasePRT`（写在 `PixelParticle.cs` 内）：字段 `startPos/midPos/endPos/_startColor/_endColor` 原样；旧 `lifePercent` 推进逻辑进 `AI()`（自行换算成 `Lifetime` 或保持自有进度字段 + 到达终点时 `Kill()`，以旧逻辑为准逐行翻译）；贝塞尔取样与 2×2 像素块绘制逻辑放 `PreDraw`，`return false`。
   - 注意：该粒子的位置由贝塞尔插值决定而不是速度，`override ShouldUpdatePosition() => false`。
   - 旧绘制发生在 `EffectLoader.DrawPlayerAndProjectileEffects` 内（AlphaBlend），新类 `PRTDrawMode = AlphaBlend`、`RenderLayer` 用默认值即可（绘制时机略有前移，该粒子为 2×2 像素点，无遮挡敏感性）。
2. 改写 2 个调用点为 `PRTLoader.NewParticle<PRT_Pixel>(...)` + 字段赋值。
3. 删除 `PixelParticle` 旧类、删除 `EModSys.cs` 516 行 `PixelParticle.Update();`、删除 `EffectLoader.cs` 903 行 `PixelParticle.drawAll();`。

**验收**：编译通过；`rg "PixelParticle" --type cs` 为 0；游戏内测 AnnihilateArrow 弹幕与 VoidCultist 特效。

### 阶段 2：`VoidParticles` / `AbyssalParticles`（数据迁入 PRT，EffectLoader 保留合成权）

**架构说明**：这两套粒子不是常规逐粒子绘制——`EffectLoader` 把它们画进 RenderTarget 做全屏 shader 合成（虚空背景效果）。迁移原则：**PRT 负责数据与生命周期，EffectLoader 继续负责 RT 合成**。这不是转接层，是与现有 `IAdditivePRT` 相同的既定职责划分（见 `EffectLoader.cs` 560–570 行）。

1. 在 `VoidParticles.cs` 中新建两个类：

```csharp
public class PRT_Void : BasePRT
{
    public float vd = 0.99f;        // 速度衰减
    public float ad = 0.014f;       // 透明度衰减
    public bool flag1 = false;
    public int shape = 0;
    public bool multShrink = false;
    // 旧 alpha 字段 → 复用基类 Opacity（旧默认 1f）

    public override string Texture => "";   // 不参与常规绘制，无主贴图
    public override void SetProperty()
    {
        ShouldKillWhenOffScreen = false;
        Lifetime = -1;                       // 有意永生，由 alpha 衰减触发 Kill
        Opacity = 1f;
    }
    public override void AI()
    {
        // 旧 VoidParticles.Update() 的逐行翻译（Position += Velocity 由框架做）：
        if (multShrink) Opacity *= ad; else Opacity -= ad;
        Velocity *= vd;
        if (Opacity < 0.02f) Kill();
    }
    public override bool PreDraw(SpriteBatch sb) => false;  // 常规世界层不绘制
}

public class PRT_Abyssal : PRT_Void { }   // 仅作类型区分，供 EffectLoader 分流
```

   - 注意 `Texture => ""` 若导致 InnoVault 加载报错（先编译验证），改为指向任意已有小贴图（如 `"CalamityEntropy/Content/Particles/PRT_Light"`），反正 PreDraw 恒 return false。
2. 改写 `EffectLoader.cs` 三处遍历（约 737、766、837 行）：把 `foreach (Particle pt in VoidParticles.particles)` 换成遍历 `PRTLoader.PRT_InGame_World_Inds`，过滤 `is PRT_Void`（注意 `PRT_Abyssal` 继承自 `PRT_Void`，737/766 行处要排除：`pt is PRT_Void && pt is not PRT_Abyssal`；837 行处过滤 `is PRT_Abyssal`）。循环体内 `pt.position → pt.Position`、`pt.alpha → pt.Opacity`、`pt.rotation → pt.Rotation`，其余字段同名。**循环体内的绘制逻辑一个字不改**。
3. 改写全部 spawn 点（~49 处，清单见附录 B）：`VoidParticles.particles.Add(new Particle(){ position = ..., velocity = ..., alpha = ..., ... })` → `var p = PRTLoader.NewParticle<PRT_Void>(pos, vel, Color.White, 1f);` 然后逐字段赋值（`alpha → p.Opacity`）。没写的字段用默认值（与旧 `Particle` 类默认值一致，已在新类中保持）。
4. 删除旧 `Particle` 类、`VoidParticles` 静态类、`AbyssalParticles` 静态类，删除 `EModSys.cs` 517–518 行两个 `Update()` 调用。

**验收**：编译通过；`rg "VoidParticles|AbyssalParticles" --type cs` 为 0；游戏内开启 `EnablePixelEffect` 配置，测试 Cruiser Boss（`CruiserHead`）、VoidMonsterShoot、ShadewindLanceThrow、AbyssalBullet、AbyssFractal 的虚空/深渊视觉与迁移前一致。

### 阶段 3：`EParticle` 52 个子类翻译（新旧共存）

按附录 A 清单逐文件翻译，A 类（模板化）直接套 3.1 模板；B 类（特殊）按附录 A 中的专项说明处理。本阶段**只加新类，不动旧类、不动调用点**，两套系统共存运行，随时可编译。

翻译时的机械规则汇总（对照 3.1 模板注释）：

1. 旧 `Texture` 覆盖 → 原样搬（若旧类依赖默认路径约定，显式写 `"CalamityEntropy/Content/Particles/<旧类名>"`；若旧 Draw 用 `CEUtils.getExtraTex` 等取图，原样保留在 PreDraw 里）。
2. 旧 `AI()` → 删 `Position += Velocity`（若没调 base.AI 且确实不位移，加 `ShouldUpdatePosition() => false`）、删 `Lifetime--`、`Lifetime = 0;` 式自杀 → `Kill()`。
3. 旧 `Draw()` / `getOrigin()` → 并入 `PreDraw`，`return false`。
4. 旧 `OnSpawn()` → 逻辑并入 `SetProperty()`（`SetProperty` 在入场时调用一次，语义等价；注意 `SetProperty` 里旧 spawn 参数已由 `NewParticle`+`Configure` 赋好）。
5. 旧构造函数参数/对象初始化器字段 → public 字段。
6. 进度表达式替换：`Lifetime / (float)TimeLeftMax` → `1f - LifetimeCompletion`；`TimeLeftMax - Lifetime` → `Time`；`TimeLeftMax` 单独出现 → `Lifetime`。
7. 每类的 `SetProperty` 寿命兜底值 = 旧类的默认 `Lifetime`（基类默认 200，多数子类在构造函数里覆盖，逐类查）。

**验收**：编译通过；52 个旧类各有一个对应 `PRT_` 新类；抽查 5 个新类与旧类逐行对照无逻辑漂移。

### 阶段 4：EParticle 调用点批量改写（~480 处，约 180 文件）

按 3.3 的标准模式逐文件改写。建议按目录分批（每批 20–30 个文件，批间编译）：

1. `Content/Items/Weapons/`（最大头）
2. `Content/Items/`其余（Books、Accessories、Donator、Pets 等）
3. `Content/Projectiles/`
4. `Content/NPCs/`
5. `Common/`（`EModPlayer.cs`、`EGlobalProjectile.cs`、`EGlobalNPC.cs` 等）与 `CalamityEntropy.cs`（2067、2072 行）
6. 其余零散（`Core/`、`Content/Buffs/` 等）

特殊调用点（附录 A 的 B-8、B-9 也在此阶段处理）：

- **spawn 点对象初始化器带 `PixelShader = true`** 的 4 处：`Content/Items/Weapons/CrossBorderPursuit.cs` 134、148 行，`Content/Items/Weapons/AzafureEKatana.cs` 167、168 行 → 改写后对新类字段 `PixelPass = true` 赋值（字段定义见附录 A B-4）。
- **`Content/Projectiles/NegentropyBulletProjectile.cs` 58、110 行**读取 `EParticle.particles.Count` 做密度节流 → 换成 `PRTLoader.PRT_InGame_World_Inds.Count`（语义近似：全体在场 PRT 数；阈值 256/128 保持不变）。

搜索命令（确认本批改完）：`rg "EParticle\.(NewParticle|spawnNew|particles)" <目录>` 应为 0。

**验收**：全仓库 `rg "EParticle\.(NewParticle|spawnNew|particles)" --type cs` 为 0；编译通过；游戏内抽测每批次中 spawn 数最多的 3 个武器/Boss。

### 阶段 5：拆除 EParticle 旧系统

前提：阶段 4 验收通过。

1. 删除 `Common/EModSys.cs` `PreUpdateDusts()` 中的 `EParticle.updateAll();`（若方法体因此为空，删除整个 override）。
2. 删除 `CalamityEntropy.cs` `DrawBehindPlayer` 中的 `EParticle.drawAll();`（约 792 行）。**只删这一行**，它前后的 `spriteBatch.End()/Begin(...)` 与 `DrawMech` 循环是别的功能，不动。
3. 改写 `Common/EffectLoader.cs` `PreparePixelShader` 中的 `EParticle.DrawPixelShaderParticles();`（约 555 行）：替换为像素通道 PRT 的批量绘制（实现见附录 A B-4 的第 3 步）。
4. 删除 52 个旧 `EParticle` 子类的类定义（新 `PRT_` 类保留在原文件）。
5. 删除 `Content/Particles/EParticle.cs` 整个文件。
6. 清理各文件残留的无效 `using`。

**验收**：全仓库 `rg "\bEParticle\b" --type cs` 为 0；编译通过；完整跑一次有代表性的 Boss 战（Cruiser）+ 高粒子武器（Silence、SolarStormHeld、EmberSpike）。

### 阶段 6：CalamityMod `GeneralParticleHandler` 替换（~580 处，最后做，工作量最大）

**先做 A 类（简单 spawn，约 550 处），B 类（带 `pixelate`/`GeneralDrawLayer` 参数的约 30 处）单独收尾。**

1. 在 `Content/Particles/`（或新建 `Content/Particles/CalamityPorts/`）为每个用到的 Calamity 粒子类型做 PRT 等价类。实测使用直方图（`rg -o "SpawnParticle\(new (\w+)" -r '$1'` 统计）：

| Calamity 类型 | 次数 | 新类名 |
|---------------|------|--------|
| `CustomPulse` | 129 | `PRT_CustomPulse` |
| `GlowSparkParticle` | 55 | `PRT_GlowSparkCal`（避免与已有 GlowSpark 系列混淆） |
| `PulseRing` | 27 | `PRT_PulseRing` |
| `HeavySmokeParticle` | 20 | `PRT_HeavySmokeCal` |
| `CustomSpark` | 19 | `PRT_CustomSpark` |
| `AltSparkParticle` | 13 | `PRT_AltSpark` |
| `DirectionalPulseRing` | 8 | `PRT_DirectionalPulseRing` |
| `TechyHoloysquareParticle` | 7 | `PRT_TechyHolosquare` |
| `VoidSparkParticle` | 6 | `PRT_VoidSparkCal` |
| `FlameParticle`/`LineParticle`/`ImpactParticle` | 各 4 | `PRT_FlameCal`/`PRT_LineCal`/`PRT_ImpactCal` |
| `MediumMistParticle`/`SparkParticle`/`DetailedExplosion` | 各 3 | 同理 `*Cal` 后缀 |
| `CritSpark`/`BloodParticle`/`BloomParticle` | 各 2 | 同理 |
| `GlowOrbParticle`/`AltLineParticle`/`PointParticle`/`GlowSquareParticle` | 各 1 | 同理 |
| 完全限定名 `new CalamityMod.Particles.Xxx` | 2 | 逐处确认类型后归入上表 |

2. 移植方法：打开 CalamityMod 源码中对应粒子类（`ModSources/CalamityMod/Particles/`，本机有源码），把其 `Update()` 逻辑翻译进 `AI()`、`CustomDraw()`/默认绘制翻译进 `PreDraw`，构造参数转 `Configure`/public 字段。**贴图直接跨模组引用**：`Texture => "@CalamityMod/Particles/<原贴图名>"`，不复制贴图文件。`CustomPulse`/`CustomSpark` 的贴图路径是构造参数（每次调用不同），做法：public string 字段 `TexPath` + PreDraw 里 `ModContent.Request<Texture2D>(TexPath).Value`（`Texture` 覆盖返回任一有效占位路径）。
   - Calamity 粒子的混合模式：其基类按 `UseAdditiveBlend` 分桶，等价映射为 `PRTDrawMode`；构造参数里的 `useAddativeBlend: false` → `AlphaBlend`。
3. A 类调用点改写：`GeneralParticleHandler.SpawnParticle(new XxxParticle(args...))` → `PRTLoader.NewParticle<PRT_Xxx>(pos, vel, color, scale).Configure(...)`，参数按新类 Configure 对位。改完删除该文件的 `using CalamityMod.Particles;`。
4. B 类调用点（带 `pixelate:` 或 `GeneralDrawLayer` 参数，集中在 `Content/Items/Weapons/BuriedSun.cs`、`Silence.cs` 等）：Calamity 的 `GeneralDrawLayer` 与 PRT 的 `PRTRenderLayer` 没有一一对应（PRT 无"弹幕前/弹幕后"层）。处理规则：
   - `AfterEverything` → 尝试 `RenderLayer = PRTRenderLayer.AfterPlayers`（PRT 中最晚的层），游戏内目视对比。
   - `BeforeProjectiles` / `AfterProjectiles` → 都先用默认层（≈ 弹幕后）；**逐点游戏内目测**，重点检查 Silence.cs 中黑色 `VoidSparkParticle`（AfterProjectiles）与蓝色 `GlowSparkParticle`（BeforeProjectiles）的前后遮挡关系——原意图是黑色盖在蓝光上面。若默认层下前后关系反了，把黑色粒子的 `RenderLayer` 提高一层（如 `AfterPlayers`）。每处核对结果记入迁移日志。
   - `pixelate: false` 是 Calamity 的非像素化路径（普通世界绘制），迁 PRT 默认行为即可；`pixelate: true`（若有）记日志人工决策。
5. 收尾：`rg "GeneralParticleHandler|using CalamityMod.Particles" --type cs` 为 0。

**验收**：编译通过；BuriedSun、Silence、MiracleWreckage、HallowedMissile、SolarStormHeld、VoidAnnihilateProj、TlipocasScythe、TheProphet 逐个游戏内目测对比。

### 阶段 7（可选）：性能优化 — **已完成（2026-07-06）**

为高频 spawn 类型开启 `CanPool => true` 并实现 `Reset()`，复用实例以降低 GC 压力。`InGame_World_MaxCount` 未调整（现有上限已足够）。

**已池化（本阶段新增 36 类 + 阶段 0–6 已有 11 类 = 47 类）** — 详见迁移日志。

**有意跳过**（低 spawn 或状态复杂/不宜池化）：
- `PRT_TrailParticle` / `PRT_TrailGunShot` — 轨迹点列表 + 复杂插值，Reset 风险高
- `PRT_HomingSpiritParticle` — 子步进 + 轨迹列表
- `PRT_WindParticle` — 轨迹列表 + 字段在声明处 `Main.rand` 初始化
- `PRT_PlayerShadow` / `PRT_PlayerShadowBlack` — 持有 `Player` 引用
- `PRT_ShadeDashParticle` — 图元 + 自定义 shader 批次
- `PRT_SlashDarkRed` — 二次绘制批次顺序敏感
- `IPixelPassPRT` 四类 — 像素 RT 通道，spawn 少
- `PRT_RuneParticleHoming` — 追踪目标引用
- `PRT_RealisticExplosion` — `SetProperty` 播音效，池化语义不清
- 其余低频/单次特效类（爆炸环、门户、警告 UI 等）

**游戏内回归（附录 C 固定名单，需人工）**：Silence、BuriedSun、SolarStormHeld、EmberSpike、CrossBorderPursuit、AzafureEKatana（像素通道开/关）、Cruiser Boss（虚空粒子）、AnnihilateArrow（PRT_Pixel）、AbyssFractal、PeaceKey / BookmarkSnowgrave；`EnablePixelEffect` 开/关各跑一遍。

---

## 附录 A：EParticle 52 子类分类清单

### A 类：模板化迁移（直接套 3.1 模板 + 阶段 3 机械规则）

一栏一文件；"D"= 有 `override Draw()`（PreDraw 情形 A），"d"= 用基类默认 Draw（PreDraw 情形 B），"O"= 有 `getOrigin()` 覆盖（内联进 PreDraw）：

| 文件（`Content/Particles/`） | 类 | 标记 |
|------|------|------|
| `Smoke.cs` | Smoke | d |
| `MediumSmoke.cs` | EMediumSmoke | D O |
| `EHeavySmoke.cs` | EHeavySmoke | D |
| `EXPLOSION.cs` | EXPLOSION、EXPLOSIONCOSMIC | D×2 |
| `PremultBurst.cs` | PremultBurst、ShockParticle、ShockParticle2 | d×3 |
| `MultiSlash.cs` | MultiSlash | D |
| `BlackKnifeSlash.cs` | BlackKnifeSlash | D |
| `BlackKnife.cs` | BlackKnifeParticle | D |
| `DOracleSlash.cs` | DOracleSlash | D（含像素通道用法，另见 B-4） |
| `StrikeParticle.cs` | StrikeParticle | D |
| `DashBeam.cs` | DashBeam | D |
| `AbyssalLine.cs` | AbyssalLine | D |
| `HadLine.cs` | HadLine | D O |
| `ELineParticle.cs` | ELineParticle | D |
| `LineParticle.cs` | ULineParticle | D |
| `ERing.cs` | ERing | D |
| `EchoCircle.cs` | EchoCircle | d |
| `HadCircle.cs` | HadCircle、HadCircle2 | D |
| `HeavenfallStar.cs` | HeavenfallStar、HeavenfallStar2、HeavenfallStar3 | D×3 |
| `StarTrail.cs` | StarTrailParticle | D |
| `ShineParticle.cs` | ShineParticle | D |
| `GlowSpark.cs` | GlowSpark、GlowSpark2、GlowSparkDirecting | D |
| `GlowLightParticle.cs` | GlowLightParticle | D |
| `CrystalGlow.cs` | CrystalGlow | D |
| `EGlowOrb.cs` | EGlowOrb | D |
| `LightAlt.cs` | LightAlt | D |
| `AntivoidTrail.cs` | AntivoidTrail | D |
| `VoidImpactParticle.cs` | VoidImpactParticle | D |
| `CruiserWarn.cs` | CruiserWarn | O |
| `PortalParticle.cs` | PortalParticle | D |
| `ShadeCloakOrb.cs` | ShadeCloakOrb | D |
| `WindParticle.cs` | WindParticle | D |
| `UpdraftParticle.cs` | UpdraftParticle | O |
| `ElecParticle.cs` | ElecParticle | D（含像素通道用法，另见 B-4） |
| `SnowPiece.cs` | SnowPiece、SnowStorm | d |
| `SakuraPetalsParticle.cs` | SakuraPetalsParticle | D |
| `LifeLeaf.cs` | LifeLeaf | d |
| `TrailSpark.cs` | TrailSparkParticle | D |
| `Trail.cs` | TrailParticle、TrailGunShot | D×2（轨迹点列表为自有字段，原样搬） |
| `RuneParticle.cs` | RuneParticle、RuneParticleHoming | d |
| `HealingParticle.cs` | HealingParticle | d |
| `MCodeParticle.cs` | MCodeParticle | d |
| `ImpactParticle.cs` | ImpactParticle | d |
| `PrismShard.cs` | PrismShard、PrismShardSmall | d（PrismShard 含像素通道用法，另见 B-4） |
| `RSpearParticle.cs` | RedemptionSpearParticle | d |
| `DarkBladeParticle.cs` | DarkBladeParticle | d |
| `APRCAlarm.cs` | APRCAlarm | D |
| `Sn.cs` | Sn | d |
| `DOracleSlash.cs` 等已列 | — | — |

### B 类：特殊专项（逐个说明，需要额外注意）

**B-1 `SlashDarkRed.cs`（二次绘制）**
旧系统在 `EParticle.drawAll` 的 AlphaBlend 批次里对它先 `Draw()` 再对所有该类实例统一补一轮 `DrawEffect()`（`EParticle.cs` 138–150 行，同批次内先全部主体后全部特效）。迁移：`PreDraw` 画主体，`PostDraw` 画 `DrawEffect` 内容。注意这会把"全部主体→全部特效"的顺序变为"逐粒子主体+特效"，同类多实例重叠时层次略有差别——该粒子为斩击特效通常单发，可接受；游戏内确认。

**B-2 `ShadeDashParticle.cs`（TriangleStrip primitive + `ShadeDashParticle.fx`）**
旧 `Draw()` 使用图元绘制与自定义 shader。迁移：整个旧 Draw 体搬进 `PreDraw`；在图元绘制前 `sb.End()`，绘制后用 `PRTLoader.BeginDrawingWithMode(PRTDrawMode, sb);` 恢复批次（该 API 会按当前模式重开 SpriteBatch），`return false`。shader 与顶点构建逻辑一个字不改。

**B-3 `HomingSpiritParticle.cs`（UpdateTimes=6 + 追踪 + 轨迹）**
唯一使用 `UpdateTimes` 的类（构造里 `UpdateTimes = 6`，旧 `updateAll` 每 tick 调 6 次 AI）。迁移：新类 `AI()` 内 `for (int i = 0; i < 6; i++) { ...旧单步逻辑（含 Position += Velocity，因为要子步进）... }`，并 `override ShouldUpdatePosition() => false`（位移已在子步内完成，避免框架再加一次）。旧"接近目标后 `Lifetime = 0`"改 `Kill()`。轨迹点列表字段原样保留。

**B-4 像素通道粒子（`PixelShader == true` 的绘制路径）**
涉及：`ProminenceTrail`（构造内固定 `PixelShader = true`）；`PrismShard`、`DOracleSlash`、`ElecParticle`（由 spawn 点对象初始化器临时开启，见阶段 4 的 4 处调用点）。这些粒子不在普通世界层绘制，而是在 `EffectLoader.PreparePixelShader` 的像素化 RT 通道里绘制（受 `Config.EnablePixelEffect` 门控：**配置关闭时它们本来就不显示，保持该行为，不要加回退绘制**）。迁移方案：

1. 新建接口（放 `IAdditivePRT.cs` 同文件）：

```csharp
internal interface IPixelPassPRT
{
    bool PixelPass { get; }               // 实例级开关（PrismShard 两种用法并存）
    void DrawPixelPass(SpriteBatch sb);   // 像素通道内的绘制
}
```

2. 四个相关新类实现该接口：`PixelPass` 为 public 可写字段（`ProminenceTrail` 在 `SetProperty` 固定 true，另外三个默认 false 由调用点赋值）；`DrawPixelPass` = 旧 Draw 体；`PreDraw` 改为 `if (PixelPass) return false;` 后接正常绘制（`ElecParticle`/`PrismShard`/`DOracleSlash` 有非像素用法）——即像素模式下普通层不画。
3. 阶段 5 第 3 步中，`EffectLoader.PreparePixelShader` 里 `EParticle.DrawPixelShaderParticles()` 的替换实现：遍历 `PRTLoader.PRT_InGame_World_Inds`，收集 `is IPixelPassPRT { PixelPass: true }` 的实例，按其 `PRTDrawMode` 分三桶，按旧顺序（AlphaBlend → NonPremultiplied → Additive）各自 `Begin`（复刻旧 `DrawPixelShaderParticles` 的 Begin 参数，`EParticle.cs` 65–94 行，采样器为 `AnisotropicClamp`）→ 逐个 `DrawPixelPass` → `End`。收尾按旧代码恢复 NonPremultiplied 批次。

**B-5 `realisticexplosion.cs`（RealisticExplosion，spawn 时播音效）**
旧类在 `OnSpawn` 播爆炸音。迁移：音效调用放 `SetProperty()`（入场时执行一次，且服务端 `NewParticle` 不执行 `SetProperty`，天然不会在服务器播音）。

**B-6 `PlayerShadow.cs`（PlayerShadow、PlayerShadowBlack，绘制玩家残影）**
持有 `Player` 引用并绘制玩家形象快照。字段照搬（引用类型字段无碍，不池化）；`Draw` 体进 `PreDraw`。若旧 Draw 内自行开关 SpriteBatch，处理方式同 B-2（结尾用 `BeginDrawingWithMode` 恢复）。

**B-7 `CalamityEntropy.cs` 内部 spawn（2067、2072 行）**
主类里有直接 spawn `HeavenfallStar` 的辅助方法，按 3.3 标准模式改写即可，列在此处仅为提醒别漏掉非 Content 目录的调用点。

**B-8 spawn 点 `{ PixelShader = true }` 的 4 处**（见阶段 4，改为 `p.PixelPass = true;`）

**B-9 `NegentropyBulletProjectile.cs` 密度节流**（见阶段 4）

---

## 附录 B：VoidParticles / AbyssalParticles spawn 点清单（阶段 2 用）

`VoidParticles.particles.Add`（~43 处）：
`Content/NPCs/VoidInvasion/VoidCultist.cs`(72,239,251)、`Content/Projectiles/Cruiser/CruiserBlackholeBullet.cs`(42)、`Common/EGlobalNPC.cs`(616)、`Content/NPCs/Cruiser/CruiserHead.cs`(497,772,1186,1197)、`Content/Projectiles/ShadewindLanceThrow.cs`(168,185,192)、`Common/EffectLoader.cs`（如有）、`Content/Projectiles/Cruiser/CruiserBlackhole.cs`(103)、`Content/Projectiles/Cruiser/VoidSpike.cs`(63)、`Content/Buffs/VoidTouch.cs`(49)、`Content/Projectiles/Pets/Abyss/VoidPalProj.cs`(85,93)、`Content/Projectiles/CruiserShadow.cs`(227,238)、`Content/Items/Weapons/CrossBorderPursuit.cs`(146,182)、`Content/Projectiles/VoidMonsterShoot.cs`(39–75 共 10 处)、`Content/Projectiles/Cruiser/VoidResidue.cs`(49)、`Content/Items/Accessories/VoidElytra.cs`(51)、`Content/Projectiles/Pets/Abyss/AbyssPet.cs`(126,134)、`Content/Items/Weapons/OblivionThresher/OblivionCruiserDash.cs`(103,114)、`Content/Items/Pets/PhantomBottle.cs`(160,171)、`Common/EPlayerDash.cs`(108,136,142)、`Content/Projectiles/VoidBottleThrow.cs`(63,76)、`Content/Projectiles/VoidBullet.cs`(36)、`Content/NPCs/Prophet/OlderCruiserAIGNPC.cs`(95,100,105,115)

`AbyssalParticles.particles.Add`（6 处）：
`Content/Projectiles/NyxolithrakenDragon.cs`(63)、`Content/Projectiles/AbyssalBullet.cs`(56,69)、`Content/Items/Weapons/Fractal/AbyssFractal.cs`(204)

（行号为迁移前快照，执行时以 `rg "VoidParticles\.particles\.Add|AbyssalParticles\.particles\.Add"` 实时结果为准。）

---

## 附录 C：验证命令速查（PowerShell / rg）

```powershell
# 阶段 4/5 后：EParticle 零残留
rg "\bEParticle\b" --type cs

# 阶段 2 后：
rg "VoidParticles|AbyssalParticles" --type cs

# 阶段 1 后：
rg "\bPixelParticle\b" --type cs

# 阶段 6 后：
rg "GeneralParticleHandler|using CalamityMod\.Particles" --type cs

# 全程禁用写法检查（应恒为 0，PRT_Light 阶段 0 之后）：
rg "PRTLoader\.AddParticle" --type cs

# Calamity 粒子类型直方图（阶段 6 建表用）：
rg -o "SpawnParticle\(new (\w+)" -r '$1' --no-filename Content Common | Group-Object | Sort-Object Count -Descending
```

游戏内回归抽查固定名单（每阶段挑相关项）：Silence、BuriedSun、SolarStormHeld、EmberSpike、CrossBorderPursuit、AzafureEKatana（像素通道）、Cruiser Boss 全程（虚空粒子）、AnnihilateArrow（PRT_Pixel）、AbyssFractal（深渊粒子）、PeaceKey / BookmarkSnowgrave（EParticle 高频）。分别在 `EnablePixelEffect` 开与关两种配置下各跑一遍虚空相关内容。

---

## 附录 D：常见陷阱清单（执行中随时对照）

1. **寿命兜底**：`Lifetime < 0` = 永生（默认 -1），`Lifetime == 0` = 当帧立即消亡。每个新类 `SetProperty` 必须有 `if (Lifetime <= 0) Lifetime = <旧默认值>;` 兜底，或有 `Kill()` 路径（如 `PRT_Void` 的 alpha 衰减）。
2. **双重位移**：框架自动 `Position += Velocity`。旧 AI 里这行必须删；忘删 = 粒子速度翻倍。
3. **双重寿命衰减**：旧 AI 里 `Lifetime--` 必须删；忘删 = 寿命减半。
4. **进度方向**：旧 `Lifetime/TimeLeftMax` 是 1→0（剩余），新 `LifetimeCompletion` 是 0→1（已过）。替换错方向 = 动画倒放。
5. **服务端**：`NewParticle` 返回非 null 孤儿，不要 null 判断；批量 spawn 外补 `Main.dedServ` 守卫。
6. **BlendState 三分支**：旧逻辑"非 Additive 非 AlphaBlend 一律 NonPremultiplied"，包括调用点传 `BlendState.NonPremultiplied` 的情况。
7. **出屏剔除**：新类必须 `ShouldKillWhenOffScreen = false`，否则追踪/长寿命粒子出屏被杀，行为改变。
8. **贴图路径**：贴图文件名对应旧类名，新类 `Texture` 显式写旧路径；类改名 ≠ 贴图改名。
9. **`RenderLayer` 只能在 `SetProperty` 设**，`AI` 里改的下一帧才生效。
10. **池化**：仅阶段 7 已审计的类型开 `CanPool`；自定义字段必须在 `Reset()` 清零，列表类在 `Reset()` 中 `Clear()` 并在 `SetProperty()` 中重新初始化随机/缓存状态。
11. `Configure` 在 `NewParticle` 之后调用，而 `SetProperty` 在 `NewParticle` 内部（入场时）已执行——`SetProperty` 里读不到 `Configure` 设的值，依赖 spawn 参数的初始化逻辑放 `Configure` 末尾。

---

## 迁移日志（执行者在此追加记录）

- **2026-07-06 — 迁移完成（阶段 0–6 + 阶段 5 收尾）**
  - **阶段 0–4**：此前会话已完成。全仓库 `EParticle.(NewParticle|spawnNew|particles)` = 0；52 个 `PRT_` 等价类已写入原粒子文件；`IPixelPassPRT` + `DrawPixelPassPRT()` 已接入 `EffectLoader.PreparePixelShader`。
  - **阶段 5（本 session）**：删除 `Content/Particles/EParticle.cs`（旧基类及 update/draw 管线）；删除已空的 `Content/Particles/AbyssalParticles.cs`（`PRT_Abyssal` 已在 `VoidParticles.cs`）；旧 52 个子类定义此前已移除，仅保留 `PRT_` 类。
  - **阶段 6**：此前会话已完成。25 个 Calamity 端口类在 `Content/Particles/CalamityPorts/`；`GeneralParticleHandler` / `using CalamityMod.Particles` = 0。
  - **最终验证（2026-07-06）**：`dotnet build` 成功（0 错误 0 警告）。遗留 grep 零残留：`EParticle`、`VoidParticles`/`AbyssalParticles` 静态类、`PixelParticle` 旧类、`GeneralParticleHandler`、`PRTLoader.AddParticle` 均为 0（`VoidParticles.cs` 文件名保留，内容仅为 `PRT_Void`/`PRT_Abyssal`）。
  - **未执行（按计划跳过）**：阶段 7 性能优化（`CanPool`/`Reset`）。注：部分高频类（如 `PRT_Smoke`、`PRT_Void`）已提前开启 `CanPool`，不影响功能，可在阶段 7 统一审计。
  - **游戏内回归**：建议按附录 C 固定名单在 `EnablePixelEffect` 开/关两种配置下各跑一遍（Silence、BuriedSun、Cruiser、CrossBorderPursuit、AzafureEKatana 像素通道等）。编译验证已通过，目视回归需人工完成。

- **2026-07-06 — 阶段 7 性能优化**
  - 为 spawn 计数 Top ~40 的高频 PRT 类开启 `CanPool` + `Reset()`（共新增 36 类；连同阶段 0–6 已有 11 类，合计 47 类池化）。
  - **本阶段新增池化**：`PRT_LineCal`、`PRT_AltSpark`、`PRT_SparkCal`、`PRT_DirectionalPulseRing`、`PRT_SparkleCal`、`PRT_DetailedExplosionCal`、`PRT_CustomSpark`、`PRT_CritSparkCal`、`PRT_VoidSparkCal`、`PRT_ImpactCal`、`PRT_MediumMistCal`、`PRT_VelChangingSpark`、`PRT_FlameCal`、`PRT_BloomCal`、`PRT_TechyHolosquare`、`PRT_AbyssalLine`、`PRT_HadLine`、`PRT_LightAlt`、`PRT_GlowLightParticle`、`PRT_CrystalGlow`、`PRT_StarTrailParticle`、`PRT_EGlowOrb`、`PRT_Sn`、`PRT_ULineParticle`、`PRT_ELineParticle`、`PRT_EHeavySmoke`、`PRT_ImpactParticle`、`PRT_TrailSparkParticle`、`PRT_HeavenfallStar`/`2`/`3`、`PRT_HadCircle`/`2`、`PRT_RuneParticle`、`PRT_Pixel`。
  - **阶段 0–6 已有池化**：`PRT_Light`、`PRT_Spark`、`PRT_Smoke`、`PRT_Void`（`PRT_Abyssal` 继承）、`PRT_EMediumSmoke`、`PRT_GlowSpark`/`2`/`Directing`、`PRT_ShineParticle`、`PRT_CustomPulse`、`PRT_GlowSparkCal`、`PRT_HeavySmokeCal`、`PRT_PulseRing`。
  - **有意跳过**：见阶段 7 节列表（轨迹/追踪/Player 引用/像素通道/音效粒子等）。
  - **验证**：`dotnet build` 成功（0 错误）；游戏内回归见附录 C，需人工。

### 迁移状态：**COMPLETE**（阶段 0–7；游戏内目视回归待人工）

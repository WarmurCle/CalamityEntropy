using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityEntropy.Assets.Register
{
    //PostSetupContent 赋值,卸载置 null;dedServ 上恒为 null,非绘制路径先判
    //类路径 + 字段名 = 资源路径,大小写和下划线都要一致
    //只放普通 static,const / readonly / 实例字段不加载

    /// <summary>高频字面名直接读字段,低频和动态拼接仍走 CEUtils.getExtraTex</summary>
    [VaultLoaden("CalamityEntropy/Assets/Extra/")]
    public static class CEExtraAssets
    {
        //拖尾、斩痕、涂抹
        public static Texture2D Streak1;
        public static Texture2D Streak2;
        public static Texture2D Streak2Trans;
        public static Texture2D StreakGoop;
        public static Texture2D StreakSolid;
        public static Texture2D StreakFaded;
        public static Texture2D SplitTrail;
        public static Texture2D MotionTrail2;
        public static Texture2D MotionTrail5;
        public static Texture2D MegaStreakBacking2;
        public static Texture2D MegaStreakBacking2b;
        public static Texture2D MegaStreakBacking2c;
        public static Texture2D MegaStreakInner;
        public static Texture2D VoltTrailThicc;
        public static Texture2D CircularSmear;
        public static Texture2D CircularSmearSmokey;
        public static Texture2D SemiCircularSmear;
        public static Texture2D SlashSmear;
        public static Texture2D wohslash;
        public static Texture2D SylvestaffStreak;
        public static Texture2D SwordSlashTexture;
        public static Texture2D EternityStreak;
        public static Texture2D BasicTrail;
        public static Texture2D rvslash;

        //圆形、光晕、星形
        public static Texture2D a_circle;
        public static Texture2D Circle;
        public static Texture2D AbyssalCircle2;
        public static Texture2D AbyssalCircle3;
        public static Texture2D AbyssalCircle4;
        public static Texture2D BasicCircle;
        public static Texture2D HollowCircleSoftEdge;
        public static Texture2D Glow;
        public static Texture2D Glow2;
        public static Texture2D GlowCone;
        public static Texture2D SpearArrowGlow2;
        public static Texture2D BloomRing;
        public static Texture2D lightball;
        public static Texture2D StarTexture;
        public static Texture2D StarTexture_White;
        public static Texture2D StarChromatic;
        public static Texture2D SoftRoundExplosion;
        public static Texture2D ShatteredExplosion;
        public static Texture2D StarlessNightGlow;
        public static Texture2D Enchanted;

        //射线、几何形
        public static Texture2D Ray;
        public static Texture2D DeathRay;
        public static Texture2D DeathRay2;
        public static Texture2D Diamond;
        public static Texture2D Triangle;
        public static Texture2D B1;
        public static Texture2D T1;
        public static Texture2D T2;
        public static Texture2D FLEND;
        public static Texture2D vlbw;
        public static Texture2D vlend;
        public static Texture2D LTLine;
        public static Texture2D impact;

        //Cruiser 系激光条带
        public static Texture2D clback;
        public static Texture2D cllight;
        public static Texture2D clinghth;
        public static Texture2D cllight2;

        //噪声、杂项
        public static Texture2D VoronoiShapes;
        public static Texture2D PatchyTallNoise;
        public static Texture2D white;
        public static Texture2D Empty;
        public static Texture2D Extra_201;
        public static Texture2D Noise_10;
        public static Texture2D Perlin;
        public static Texture2D TurbulentNoise;
        public static Texture2D Smoke;
        public static Texture2D VoidBack;

        //Ports 子目录(字段名与文件名一致,路径需单独指定)
        [VaultLoaden("CalamityEntropy/Assets/Extra/Ports/AtlasMunitionsDropPodGlow")]
        public static Texture2D AtlasMunitionsDropPodGlow;
        [VaultLoaden("CalamityEntropy/Assets/Extra/Ports/SmallGreyscaleCircle")]
        public static Texture2D SmallGreyscaleCircle;

        //Asset<Texture2D> 形态:SetShaderTexture 等杂项着色器接口只收 Asset 句柄,
        //与同名裸字段共存,底层资产同一份,不重复占用显存。
        [VaultLoaden("CalamityEntropy/Assets/Extra/Streak1")]
        public static Asset<Texture2D> Streak1Asset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/Streak2")]
        public static Asset<Texture2D> Streak2Asset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/StreakGoop")]
        public static Asset<Texture2D> StreakGoopAsset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/StreakSolid")]
        public static Asset<Texture2D> StreakSolidAsset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/StreakFaded")]
        public static Asset<Texture2D> StreakFadedAsset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/SylvestaffStreak")]
        public static Asset<Texture2D> SylvestaffStreakAsset;
        [VaultLoaden("CalamityEntropy/Assets/Extra/Enchanted")]
        public static Asset<Texture2D> EnchantedAsset;
    }

    /// <summary>
    /// pass 名不是「文件名+Pass」的,用字段级标签指明
    /// VaultLoaden 会注册 Filters.Scene["CalamityEntropy:文件名"],和 EntropySkies 同 key 先核对
    /// </summary>
    [VaultLoaden("CalamityEntropy/Assets/Effects/")]
    public static class CEEffectAssets
    {
        //物品幻彩描边,实际 pass 是 EnchantedPass
        [VaultLoaden("CalamityEntropy/Assets/Effects/Wisp", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect Wisp;

        //白化/透明变换族,pass 都是 EnchantedPass
        [VaultLoaden("CalamityEntropy/Assets/Effects/WhiteTrans", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect WhiteTrans;
        [VaultLoaden("CalamityEntropy/Assets/Effects/Trans", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect Trans;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SlashTrans", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect SlashTrans;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SlashTrans2", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect SlashTrans2;
        [VaultLoaden("CalamityEntropy/Assets/Effects/Fire", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect Fire;
        [VaultLoaden("CalamityEntropy/Assets/Effects/Transform3", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect Transform3;

        //刀光拖尾族,pass 都是 EffectPass
        [VaultLoaden("CalamityEntropy/Assets/Effects/SwordTrail", AssetMode.EffectValue, "EffectPass")]
        public static Effect SwordTrail;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SwordTrail2", AssetMode.EffectValue, "EffectPass")]
        public static Effect SwordTrail2;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SwordTrail3", AssetMode.EffectValue, "EffectPass")]
        public static Effect SwordTrail3;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SwordTrail4", AssetMode.EffectValue, "EffectPass")]
        public static Effect SwordTrail4;
        [VaultLoaden("CalamityEntropy/Assets/Effects/SwordTrail5", AssetMode.EffectValue, "EffectPass")]
        public static Effect SwordTrail5;
        [VaultLoaden("CalamityEntropy/Assets/Effects/RedAdd", AssetMode.EffectValue, "EffectPass")]
        public static Effect RedAdd;

        //漩涡、红移
        [VaultLoaden("CalamityEntropy/Assets/Effects/Vortex", AssetMode.EffectValue, "Pass1")]
        public static Effect Vortex;
        [VaultLoaden("CalamityEntropy/Assets/Effects/RedTrans", AssetMode.EffectValue, "Pass1")]
        public static Effect RedTrans;
        [VaultLoaden("CalamityEntropy/Assets/Effects/ColorLerp2", AssetMode.EffectValue, "Pass1")]
        public static Effect ColorLerp2;

        //先知系,pass 名与文件名相同的单独指明
        [VaultLoaden("CalamityEntropy/Assets/Effects/fableeyelaser", AssetMode.EffectValue, "fableeyelaser")]
        public static Effect fableeyelaser;

        //以下为 CompileFX.ps1 产出的 .fxc 着色器,tML 的 FxcReader 认这个后缀,与 .xnb 一样走 Assets.Request
        //巡游者天幕扭曲滤镜:交给 ScreenShaderData 的 Asset 形态构造器,由 EntropySkies 在 PostSetupContent 注册,取代旧 CrSky 的 RenderTarget 扭曲流程
        [VaultLoaden("CalamityEntropy/Assets/Effects/CruiserSkyFilter", AssetMode.Effects, "CruiserSkyPass")]
        public static Asset<Effect> CruiserSkyFilter;
        //虚空驱逐舰全息投影(红恶魔/丛林陆龟/小白龙等原版贴图的染色扫描线),与 CruiserSkyFilter 同为 .fxc,取 .Value 后 Passes[0].Apply
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDHologram", AssetMode.Effects, "HologramPass")]
        public static Asset<Effect> VDHologram;
        //虚空驱逐舰能量射线(轨道光柱/湮灭主炮/红射线):双向滚动噪声 + 白热核心 + 边缘辉光,噪声图绑 s1
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDVoidBeam", AssetMode.Effects, "BeamPass")]
        public static Asset<Effect> VDVoidBeam;
        //虚空驱逐舰透视射线(梯形光锥:红射线/主炮越肩锥/瞄准锥/点阵射线):顶点纹理坐标是像素单位的「沿轴 / 横向」仿射量,
        //跨三角剖分精确插值,没有归一化 UV 梯形的中线折断;透视校正沿轴参数 + 两端雾化/变暗,噪声图绑 s1。消费口 VDBeamDraw.DrawTapered / TaperedQuad
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDBeamTapered", AssetMode.Effects, "TaperedPass")]
        public static Asset<Effect> VDBeamTapered;
        //虚空驱逐舰全屏滤镜(引力透镜/空间裂隙/暗角/冲击帧),由 VDScreenShaderData 每帧喂参,键 CalamityEntropy:VoidDestroyer
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDScreenFx", AssetMode.Effects, "ScreenFxPass")]
        public static Asset<Effect> VDScreenFx;
        //虚空驱逐舰奇点吸积盘(极坐标噪声旋流 + 事件视界黑盘 + 热边),噪声图绑 s1
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDSingularity", AssetMode.Effects, "SingularityPass")]
        public static Asset<Effect> VDSingularity;
        //虚空驱逐舰「轨道封锁」天幕(深空底幕 + 程序化星野 + 被侵蚀星球 + 六边形封锁力场),VDSky 在跨 0 切片画全屏白方块;噪声 s1 TurbulentNoise、s2 Perlin
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDSky", AssetMode.Effects, "SkyPass")]
        public static Asset<Effect> VDSky;
        //虚空驱逐舰能量逸散描边(alpha 八邻内缘 + 噪声侵蚀 + 蓄力热色/出手爆闪),噪声图绑 s1;外扩光晕由 VoidDestroyer.Draw 多偏移叠画同一遍着色器
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDRimLight", AssetMode.Effects, "RimPass")]
        public static Asset<Effect> VDRimLight;
        //虚空驱逐舰描边光晕(实心剪影涂成描边色 + 与 VDRimLight 同名的噪声侵蚀/热色参数),噪声图绑 s1;
        //VoidDestroyer.Draw 在屏幕空间偏移叠画多抽垫在本体之下,本体压住剪影内部,剩下的那圈就是外扩描边带
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDRimHalo", AssetMode.Effects, "HaloPass")]
        public static Asset<Effect> VDRimHalo;
        //虚空驱逐舰纵深雾化(噪声热闪 + 菱形模糊 + 去饱和 + 雾色),远景层里的本体与深度弹幕贴图都经它;噪声图绑 s1,AlphaBlend 预乘输出。消费口 VDDepthDraw
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDDepthFog", AssetMode.Effects, "DepthFogPass")]
        public static Asset<Effect> VDDepthFog;
    }
}

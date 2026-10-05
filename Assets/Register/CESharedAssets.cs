using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityEntropy.Assets.Register
{
    //VaultLoaden 在 PostSetupContent 给这些字段赋值,卸载时置 null;dedServ 上它们始终是 null,非绘制路径要先判断
    //资源路径由类路径加上字段名拼出,大小写和下划线都要一致
    //这里只放普通 static 字段,const、readonly 和实例字段不会加载

    /// <summary>调用方遇到高频字面名时直接读这些字段,低频和动态拼接仍走 CEUtils.getExtraTex</summary>
    [VaultLoaden("CalamityEntropy/Assets/Extra/")]
    public static class CEExtraAssets
    {
        //这一组是拖尾、斩痕和涂抹贴图
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

        //这一组是圆形、光晕和星形贴图
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

        //这一组是射线和几何形贴图
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

        //这一组是巡游者的激光条带
        public static Texture2D clback;
        public static Texture2D cllight;
        public static Texture2D clinghth;
        public static Texture2D cllight2;

        //这一组是噪声和杂项贴图
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

        //Ports 子目录的字段名和文件名一致,路径要单独指定
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
    /// pass 名不是「文件名+Pass」时,字段上的标签要写明真正的 pass
    /// VaultLoaden 会注册 Filters.Scene["CalamityEntropy:文件名"],和 EntropySkies 用了同一个 key 时要先核对
    /// </summary>
    [VaultLoaden("CalamityEntropy/Assets/Effects/")]
    public static class CEEffectAssets
    {
        //Wisp 画物品幻彩描边,实际 pass 是 EnchantedPass
        [VaultLoaden("CalamityEntropy/Assets/Effects/Wisp", AssetMode.EffectValue, "EnchantedPass")]
        public static Effect Wisp;

        //白化和透明变换这组着色器的 pass 都是 EnchantedPass
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

        //刀光拖尾这组着色器的 pass 都是 EffectPass
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

        //这一组是漩涡和红移着色器
        [VaultLoaden("CalamityEntropy/Assets/Effects/Vortex", AssetMode.EffectValue, "Pass1")]
        public static Effect Vortex;
        [VaultLoaden("CalamityEntropy/Assets/Effects/RedTrans", AssetMode.EffectValue, "Pass1")]
        public static Effect RedTrans;
        [VaultLoaden("CalamityEntropy/Assets/Effects/ColorLerp2", AssetMode.EffectValue, "Pass1")]
        public static Effect ColorLerp2;

        //先知这组着色器的 pass 名和文件名相同,要单独写明
        [VaultLoaden("CalamityEntropy/Assets/Effects/fableeyelaser", AssetMode.EffectValue, "fableeyelaser")]
        public static Effect fableeyelaser;

        //下面这些是 CompileFX.ps1 产出的 .fxc 着色器,tML 的 FxcReader 认这个后缀,和 .xnb 一样走 Assets.Request
        //CruiserSkyFilter 交给 ScreenShaderData 的 Asset 构造器,EntropySkies 在 PostSetupContent 里注册它,取代旧 CrSky 用 RenderTarget 做的扭曲
        [VaultLoaden("CalamityEntropy/Assets/Effects/CruiserSkyFilter", AssetMode.Effects, "CruiserSkyPass")]
        public static Asset<Effect> CruiserSkyFilter;
        //VDHologram 给红恶魔、丛林陆龟、小白龙这类原版贴图做染色扫描线,它和 CruiserSkyFilter 一样是 .fxc,调用时取 .Value 再 Passes[0].Apply
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDHologram", AssetMode.Effects, "HologramPass")]
        public static Asset<Effect> VDHologram;
        //VDVoidBeam 画轨道光柱、湮灭主炮和红射线,噪声双向滚动,中心是白热的,边缘有辉光,噪声图绑在 s1
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDVoidBeam", AssetMode.Effects, "BeamPass")]
        public static Asset<Effect> VDVoidBeam;
        //VDBeamTapered 画红射线、主炮越肩锥、瞄准锥和点阵射线,顶点坐标按像素记沿轴和横向
        //这些坐标跨三角剖分时做仿射插值,所以没有归一化 UV 梯形的中线折断,沿轴参数做透视校正,两端再雾化和变暗,噪声图绑在 s1,调用处是 VDBeamDraw.DrawTapered 和 TaperedQuad
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDBeamTapered", AssetMode.Effects, "TaperedPass")]
        public static Asset<Effect> VDBeamTapered;
        //VDScreenFx 做引力透镜、空间裂隙、暗角和冲击帧,VDScreenShaderData 每帧喂参数,场景键是 CalamityEntropy:VoidDestroyer
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDScreenFx", AssetMode.Effects, "ScreenFxPass")]
        public static Asset<Effect> VDScreenFx;
        //VDSingularity 画奇点吸积盘,噪声按极坐标旋流,中间是事件视界黑盘,边上是热边,噪声图绑在 s1
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDSingularity", AssetMode.Effects, "SingularityPass")]
        public static Asset<Effect> VDSingularity;
        //VDSky 画轨道封锁天幕,里面有深空底幕、程序化星野、被侵蚀的星球和六边形封锁力场,它在跨 0 的切片上画全屏白方块,噪声 s1 用 TurbulentNoise,s2 用 Perlin
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDSky", AssetMode.Effects, "SkyPass")]
        public static Asset<Effect> VDSky;
        //VDRimLight 沿 alpha 八邻画内缘,再用噪声侵蚀,蓄力时偏热色,出手时爆闪,噪声图绑在 s1,外扩光晕由 VoidDestroyer.Draw 多次偏移叠画同一遍着色器
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDRimLight", AssetMode.Effects, "RimPass")]
        public static Asset<Effect> VDRimLight;
        //VDRimHalo 把实心剪影涂成描边色,噪声侵蚀和热色参数与 VDRimLight 同名,噪声图绑在 s1
        //VoidDestroyer.Draw 在屏幕空间里错开多画几遍,垫在本体下面,本体压住剪影内部,剩下的那圈就是外扩描边带
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDRimHalo", AssetMode.Effects, "HaloPass")]
        public static Asset<Effect> VDRimHalo;
        //VDDepthFog 给远景做纵深雾,含噪声热闪、菱形模糊、去饱和和雾色,远景层里的本体和深度弹幕贴图都经过它,噪声图绑在 s1,输出预乘 alpha 并用 AlphaBlend 混合,调用处是 VDDepthDraw
        [VaultLoaden("CalamityEntropy/Assets/Effects/VDDepthFog", AssetMode.Effects, "DepthFogPass")]
        public static Asset<Effect> VDDepthFog;
    }
}

using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityEntropy.Content.Particles
{
    /// <summary>
    /// 跨模组粒子贴图统一入口：PRT 的 Texture 属性走不了 @ 语法（HasAsset 认不出），
    /// 跨模组贴图全部在这里用 <see cref="InnoVault.VaultLoadenAttribute"/> 静态加载
    /// 禁止在 PreDraw 里 ModContent.Request；下面按粒子族分组，加贴图先找对应组
    /// </summary>
    internal static class PRTSharedAssets
    {
        //本模组粒子贴图,Texture属性能直接认CalamityEntropy/...路径,PreDraw也可走这拿
        [VaultLoaden("CalamityEntropy/Content/Particles/PRT_Light2")]
        internal static Asset<Texture2D> PRT_Light2;

        [VaultLoaden("CalamityEntropy/Content/Particles/StarTrail")]
        internal static Asset<Texture2D> StarTrail;

        [VaultLoaden("CalamityEntropy/Content/Particles/AntivoidTrail")]
        internal static Asset<Texture2D> AntivoidTrail;

        [VaultLoaden("CalamityEntropy/Content/Particles/DashBeam")]
        internal static Asset<Texture2D> DashBeam;

        [VaultLoaden("CalamityEntropy/Content/Particles/UpdraftParticle")]
        internal static Asset<Texture2D> UpdraftParticle;

        [VaultLoaden("CalamityEntropy/Content/Particles/ShadeDashParticle")]
        internal static Asset<Texture2D> ShadeDashParticle;

        [VaultLoaden("CalamityEntropy/Content/Particles/LargeSpark")]
        internal static Asset<Texture2D> LargeSpark;

        [VaultLoaden("CalamityEntropy/Content/Particles/Trail")]
        internal static Asset<Texture2D> Trail;

        [VaultLoaden("CalamityEntropy/Content/Particles/Smoke")]
        internal static Asset<Texture2D> Smoke;

        [VaultLoaden("CalamityEntropy/Content/Particles/Wind")]
        internal static Asset<Texture2D> Wind;

        //Effect shader,ProminenceTrail/PortalParticle/ShadeDashParticle/Trail PreDraw里用
        [VaultLoaden("CalamityEntropy/Assets/Effects/Vortex")]
        internal static Asset<Effect> Vortex;

        [VaultLoaden("CalamityEntropy/Assets/Effects/Prominence")]
        internal static Asset<Effect> Prominence;

        [VaultLoaden("CalamityEntropy/Assets/Effects/ShadeDashParticle")]
        internal static Asset<Effect> ShadeDashParticleShader;

        //下面@前缀只有VaultLoaden认,写进粒子Texture属性HasAsset查不到,游戏里就是占位图
        //Bloom光晕,CustomPulse/BloomCal/SparkleCal/CritSparkCal叠层
        [VaultLoaden("@CalamityMod/Particles/BloomCircle")]
        internal static Asset<Texture2D> BloomCircle;

        //Spark/GlowSpark系,VoidSparkCal/CustomSpark/GlowSparkCal/AltSpark/SparkCal
        [VaultLoaden("@CalamityMod/Particles/GlowSpark")]
        internal static Asset<Texture2D> GlowSpark;

        [VaultLoaden("@CalamityMod/Particles/GlowSpark2")]
        internal static Asset<Texture2D> GlowSpark2;

        [VaultLoaden("@CalamityMod/Particles/ThinSparkle")]
        internal static Asset<Texture2D> ThinSparkle;

        [VaultLoaden("@CalamityMod/Particles/Sparkle2")]
        internal static Asset<Texture2D> Sparkle2;

        [VaultLoaden("@CalamityMod/Projectiles/StarProj")]
        internal static Asset<Texture2D> StarProj;

        [VaultLoaden("@CalamityMod/Particles/MammothParticle")]
        internal static Asset<Texture2D> MammothParticle;   //天顶周二彩蛋,CustomPulse会用到,别删

        //Trail streak,Trail.cs SetShaderTexture
        [VaultLoaden("@CalamityMod/ExtraTextures/Trails/BasicTrail")]
        internal static Asset<Texture2D> BasicTrail;

        //HeavySmoke/Mist烟雾,HeavySmokeCal/MediumMistCal
        [VaultLoaden("@CalamityMod/Particles/HeavySmoke")]
        internal static Asset<Texture2D> HeavySmoke;

        [VaultLoaden("@CalamityMod/Particles/MediumMist")]
        internal static Asset<Texture2D> MediumMist;

        //Line/Drain线型,LineCal/AltLineCal
        [VaultLoaden("@CalamityMod/Particles/DrainLineBloom")]
        internal static Asset<Texture2D> DrainLineBloom;

        [VaultLoaden("@CalamityMod/Particles/DrainLine")]
        internal static Asset<Texture2D> DrainLine;

        //Pulse环,PulseRing/DirectionalPulseRing
        [VaultLoaden("@CalamityMod/Particles/HollowCircleHardEdge")]
        internal static Asset<Texture2D> HollowCircleHardEdge;

        //Explosion,DetailedExplosionCal/PlasmaExplosionCal
        [VaultLoaden("@CalamityMod/Particles/DetailedExplosion")]
        internal static Asset<Texture2D> DetailedExplosion;

        [VaultLoaden("@CalamityMod/Particles/PlasmaExplosion")]
        internal static Asset<Texture2D> PlasmaExplosion;

        //Flame,FlameCal
        [VaultLoaden("@CalamityMod/Particles/Flames")]
        internal static Asset<Texture2D> Flames;

        //Holosquare,护盾/科技方块VFX,TechyHolosquare
        [VaultLoaden("@CalamityMod/Particles/TechyHolosquare")]
        internal static Asset<Texture2D> TechyHolosquare;

        //Point/Orb/Square光点基元,PointCal/GlowOrbCal/GlowSquare系
        [VaultLoaden("@CalamityMod/Particles/PointParticle")]
        internal static Asset<Texture2D> PointParticle;

        [VaultLoaden("@CalamityMod/Particles/GlowSquareParticle")]
        internal static Asset<Texture2D> GlowSquareParticle;

        [VaultLoaden("@CalamityMod/Particles/GlowOrbParticle")]
        internal static Asset<Texture2D> GlowOrbParticle;

        //Blood粒子
        [VaultLoaden("@CalamityMod/Particles/Blood")]
        internal static Asset<Texture2D> Blood;
    }
}

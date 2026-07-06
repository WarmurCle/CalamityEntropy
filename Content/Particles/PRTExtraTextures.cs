using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityEntropy.Content.Particles
{
    /// <summary>
    /// Assets/Extra 贴图入口：本模组路径，VaultLoaden 静态加载
    /// 和 PRTSharedAssets 分开只是因为目录不同；用法一样——PreDraw 里 .Value，别 ModContent.Request
    /// 粒子 Texture 属性能认 CalamityEntropy/... 路径，但跨模组 @ 语法仍走 PRTSharedAssets
    /// </summary>
    internal static class PRTExtraTextures
    {
        //条纹/拖尾材质,ERing/StarTrail/ProminenceTrail shader采样用
        [VaultLoaden("CalamityEntropy/Assets/Extra/MegaStreakBacking2")]
        internal static Asset<Texture2D> MegaStreakBacking2;

        [VaultLoaden("CalamityEntropy/Assets/Extra/Streak1w")]
        internal static Asset<Texture2D> Streak1w;

        //星形/光晕,HeavenfallStar/EGlowOrb/HomingSpirit/ShineParticle
        [VaultLoaden("CalamityEntropy/Assets/Extra/StarTexture_White")]
        internal static Asset<Texture2D> StarTexture_White;

        [VaultLoaden("CalamityEntropy/Assets/Extra/Glow2")]
        internal static Asset<Texture2D> Glow2;

        //几何圆,Smoke备用圆/AbyssalLine/BlackKnifeSlash
        [VaultLoaden("CalamityEntropy/Assets/Extra/a_circle")]
        internal static Asset<Texture2D> ACircle;

        [VaultLoaden("CalamityEntropy/Assets/Extra/Circle")]
        internal static Asset<Texture2D> Circle;

        [VaultLoaden("CalamityEntropy/Assets/Extra/white")]
        internal static Asset<Texture2D> White;   //PixelParticle/SlashDarkRed,1x1白图

        //shader采样纹理,PortalParticle/ProminenceTrail PreDraw里绑gd.Textures[]
        [VaultLoaden("CalamityEntropy/Assets/Extra/CrystalGlow")]
        internal static Asset<Texture2D> CrystalGlow;

        [VaultLoaden("CalamityEntropy/Assets/Extra/VoronoiShapes")]
        internal static Asset<Texture2D> VoronoiShapes;

        [VaultLoaden("CalamityEntropy/Assets/Extra/colormap_fire")]
        internal static Asset<Texture2D> ColormapFire;

        [VaultLoaden("CalamityEntropy/Assets/Extra/SimpleNoise")]
        internal static Asset<Texture2D> SimpleNoise;

        //命中/UI
        [VaultLoaden("CalamityEntropy/Assets/Extra/Impact2")]
        internal static Asset<Texture2D> Impact2;

        [VaultLoaden("CalamityEntropy/Assets/Extra/APRCAlarm2")]
        internal static Asset<Texture2D> APRCAlarm2;

        [VaultLoaden("CalamityEntropy/Assets/Extra/APRCAlarm")]
        internal static Asset<Texture2D> APRCAlarm;
    }
}

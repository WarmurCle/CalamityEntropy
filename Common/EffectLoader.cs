using CalamityEntropy.Content.UI.EntropyBookUI;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityEntropy.Common
{
    /// <summary>
    /// VaultLoaden 尾斜杠表示字段名即文件名;Asset&lt;Effect&gt; 延迟取 Value,裸 Effect 立即加载
    /// 贴图字段必须自带字段级标签,否则按字段名去 Effects 找;dedServ 上为 null,PostSetupContent 前未赋值
    /// </summary>
    [VaultLoaden("CalamityEntropy/Assets/Effects/")]
    internal class EffectLoader
    {
        public const string AssetPath = "CalamityEntropy/Assets/";
        public const string AssetPath2 = "Assets/";

        [VaultLoaden("CalamityEntropy/Assets/Extra/cvmask")]
        internal static Asset<Texture2D> cvmask;
        [VaultLoaden("CalamityEntropy/Assets/Extra/StarrySky")]
        internal static Asset<Texture2D> planetarium_blue_base;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/CruiserSlash")]
        internal static Asset<Texture2D> cruiserSlash;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/Cruiser/CruiserBlackholeBullet")]
        internal static Asset<Texture2D> cruiserBlackholeBullet;
        [VaultLoaden("CalamityEntropy/Assets/Extra/ksc1")]
        internal static Asset<Texture2D> ksc1;
        [VaultLoaden("CalamityEntropy/Assets/Extra/shockwave")]
        internal static Asset<Texture2D> shockwave;
        [VaultLoaden("CalamityEntropy/Content/Projectiles/Cruiser/VoidStar")]
        internal static Asset<Texture2D> voidStar;
        [VaultLoaden("CalamityEntropy/Assets/Extra/shield")]
        internal static Asset<Texture2D> ShieldTex;
        [VaultLoaden("CalamityEntropy/Assets/Extra/cvdt")]
        internal static Asset<Texture2D> CvdtTex;
        [VaultLoaden("CalamityEntropy/Assets/Extra/BlurryPerlinNoise")]
        internal static Asset<Texture2D> BlurryPerlinNoiseTex;
        [VaultLoaden("CalamityEntropy/Assets/Extra/AwSky1")]
        internal static Asset<Texture2D> AwSky1Tex;

        [VaultLoaden("CalamityEntropy/Assets/Effects/Pixel", AssetMode.EffectValue, "Pixel")]
        internal static Effect PixelShader;
        [VaultLoaden("CalamityEntropy/Assets/Effects/blur", AssetMode.EffectValue, "P0")]
        internal static Effect BlurShader;

        //以下这批 .fx 的 pass 名不是「文件名+Pass」,必须显式指明,
        //否则 VaultLoaden 自动注册的 Filters.Scene 项会带一个不存在的 pass 名
        [VaultLoaden("CalamityEntropy/Assets/Effects/Cylinder", AssetMode.Effects, "P0")]
        public static Asset<Effect> Cylinder;
        [VaultLoaden("CalamityEntropy/Assets/Effects/kscreen", AssetMode.EffectValue, "kscreen")]
        public static Effect kscreen;
        [VaultLoaden("CalamityEntropy/Assets/Effects/fscreen", AssetMode.EffectValue, "fscreen")]
        public static Effect fscreen;
        [VaultLoaden("CalamityEntropy/Assets/Effects/kscreen2", AssetMode.EffectValue, "kscreen2")]
        public static Effect kscreen2;
        [VaultLoaden("CalamityEntropy/Assets/Effects/cvoid", AssetMode.EffectValue, "cvoid")]
        public static Effect cvoid;
        [VaultLoaden("CalamityEntropy/Assets/Effects/cvoid2", AssetMode.EffectValue, "cvoid")]
        public static Effect cvoid2;
        [VaultLoaden("CalamityEntropy/Assets/Effects/cvoid3", AssetMode.EffectValue, "cvoid")]
        public static Effect cvoid3;
        [VaultLoaden("CalamityEntropy/Assets/Effects/cabyss", AssetMode.EffectValue, "cabyss")]
        public static Effect cabyss;
        [VaultLoaden("CalamityEntropy/Assets/Effects/cblood", AssetMode.EffectValue, "cabyss")]
        public static Effect cblood;

        /// <summary>
        /// 转发到 EBookUI.shader,那个着色器在 Mod.Load 里手动请求
        /// 属性上的标签只为让 VaultLoaden 认得这个 pass 名
        /// </summary>
        [VaultLoaden("CalamityEntropy/Assets/Effects/Outline", AssetMode.EffectValue, "Pass1")]
        public static Effect OutlineShader {
            get {
                return EBookUI.shader;
            }
            set {
                EBookUI.shader = value;
            }
        }
    }
}

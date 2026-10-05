using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityEntropy.Assets.Register
{
    //原自动属性改成了普通静态字段,避免私有 setter 用反射赋值时不确定
    //这些字段在专用服务器上始终是 null,只能在绘制这类客户端路径读取
    public class TextureRegister : ModSystem
    {
        //这一组是轨迹贴图
        [VaultLoaden("CalamityEntropy/Assets/Extra/Slash_Wrap")]
        public static Asset<Texture2D> Trail_SlashWrap;
        [VaultLoaden("CalamityEntropy/Assets/DoubleLineTrail")]
        public static Asset<Texture2D> Trail_DoubleLine;
        //原注册器从未给 Trail_MotionTrail1 赋值,所以它保持没有标签,始终是 null
        public static Asset<Texture2D> Trail_MotionTrail1;
        [VaultLoaden("CalamityEntropy/Assets/MotionTrail2")]
        public static Asset<Texture2D> Trail_MotionTrail2;
        [VaultLoaden("CalamityEntropy/Assets/MotionTrail3")]
        public static Asset<Texture2D> Trail_MotionTrail3;
        [VaultLoaden("CalamityEntropy/Assets/MotionTrail4")]
        public static Asset<Texture2D> Trail_MotionTrail4;

        //这一组是噪声贴图
        [VaultLoaden("CalamityEntropy/Assets/MiscNoise01")]
        public static Asset<Texture2D> Noise_Misc1;
        [VaultLoaden("CalamityEntropy/Assets/MiscNoise02")]
        public static Asset<Texture2D> Noise_Misc2;

        //这一组是通用形状贴图
        [VaultLoaden("CalamityEntropy/Assets/Extra/ShinyOrbParticle")]
        public static Asset<Texture2D> General_WhiteOrb;
        [VaultLoaden("CalamityEntropy/Assets/Extra/WhiteCube")]
        public static Asset<Texture2D> General_WhiteCube;
    }
}

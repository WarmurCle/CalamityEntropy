using CalamityEntropy.Common;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityEntropy.Content.Skies
{
    /// <summary>
    /// 键 CalamityEntropy:Cruiser,强度取 CruiserSkyDrive.Intensity,关像素效果时为 0
    /// 开关在 CBScene.ManageSpecialBiomeVisuals,本类不自灭,免得和 VoidMonolith 抢触发
    /// </summary>
    public class CrScreenShaderData : ScreenShaderData
    {
        public CrScreenShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName) {
        }

        public override void Apply() {
            //强度门:uOpacity = 本值 × Filter 淡入;为 0 时 IsVisible 为假,滤镜整体被跳过
            UseOpacity(Config.Instance.EnablePixelEffect ? CruiserSkyDrive.Intensity : 0f);

            //镜头视差与缩放补偿(滤镜阶段 screenWidth/Height 是真实值,无背景预除)
            Shader?.Parameters["uScreenOffCE"]?.SetValue(
                Main.screenPosition * 0.5f / Main.ScreenSize.ToVector2() * new Vector2(1f, Main.LocalPlayer.gravDir));
            Shader?.Parameters["uCoordMultCE"]?.SetValue(Vector2.One / Main.GameViewMatrix.Zoom);
            //扭曲带中心:常规重力在天空侧(屏高 35%),反重力翻到下侧
            Shader?.Parameters["uBandCenterCE"]?.SetValue(Main.LocalPlayer.gravDir == 1f ? 0.35f : 0.65f);
            base.Apply();
        }
    }
}

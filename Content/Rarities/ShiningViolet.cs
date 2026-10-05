using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static CalamityEntropy.Content.Rarities.CERarityNameEffects;

namespace CalamityEntropy.Content.Rarities
{
    /// <summary>水晶字走 DrawCrystal;FlowingLight、FadingRoseateReverie 换色复用同一原语</summary>
    public sealed class ShiningViolet : CERarity
    {
        /// <summary>拾取主色</summary>
        public static readonly Color Violet = Color.Violet;
        //亮边起始色偏暗,渐变到右端的正紫罗兰
        public static readonly Color EdgeStart = Color.Violet * 0.6f;
        public static readonly Color Light = Color.Purple;

        public override Color BaseColor => Violet;

        public override void DrawName(SpriteBatch sb, Item item, string text, Vector2 pos, Color lineColor, Vector2 scale, float time) {
            float fade = FadeOf(lineColor);
            DrawCrystal(sb, item, text, pos, scale, time,
                Fade(EdgeStart, fade), Fade(Violet, fade), Fade(Light, fade), Fade(Violet, fade), true);
        }
    }
}

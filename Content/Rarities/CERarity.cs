using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Rarities
{
    /// <summary>
    /// 不再借灾厄 ModRarity;BaseColor 给只读颜色,名称行自绘
    /// 在当前 SpriteBatch 直绘,不 End/Begin,不切混合,不碰渲染目标
    /// </summary>
    public abstract class CERarity : ModRarity
    {
        /// <summary>静态主色。各档必须彼此可辨,拾取飘字只看这一个值</summary>
        public abstract Color BaseColor { get; }

        public sealed override Color RarityColor => BaseColor;

        /// <summary>lineColor 已含 mouseTextColor 呼吸,用 FadeOf 取衰减;scale 是行基准缩放</summary>
        public virtual void DrawName(SpriteBatch sb, Item item, string text, Vector2 pos, Color lineColor, Vector2 scale, float time) {
            CERarityNameEffects.DrawPlain(sb, text, pos, lineColor, scale);
        }
    }
}

using CalamityEntropy.Content.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class SoarWingDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead || (drawInfo.drawPlayer.Entropy().vanityWing != null && !(drawInfo.drawPlayer.Entropy().vanityWing.ModItem is RuneWing)))
                return false;
            return drawInfo.drawPlayer.Entropy().hasAccVisual("RuneWing");
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Wings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            if (player.Entropy().wingData.FrameCount >= player.Entropy().wingData.MaxFrame)
            {
                player.Entropy().wingData.FrameCount = 0;
            }
            Texture2D tex = CEUtils.getExtraTex("SoarRuneWings/" + (player.Entropy().wingData.FrameCount == -1 ? "f" : "f" + player.Entropy().wingData.FrameCount.ToString()));
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height);
            drawInfo.DrawDataCache.Add(new DrawData(tex, offset, null, drawInfo.colorArmorBody, 0, new Vector2(drawInfo.drawPlayer.direction == 1 ? 59 : tex.Width - 59, 44), 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.cWings });
        }
    }
}

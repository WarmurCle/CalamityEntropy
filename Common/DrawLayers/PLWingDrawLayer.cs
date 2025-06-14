using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class PLWingDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead || (drawInfo.drawPlayer.Entropy().vanityWing != null && !(drawInfo.drawPlayer.Entropy().vanityWing.ModItem is PhantomLightWing)))
                return false;
            return drawInfo.drawPlayer.Entropy().hasAccVisual("PLWing");
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Wings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;

            Texture2D tex = CEUtils.getExtraTex("PLWing/" + (player.Entropy().wingData.FrameCount == -1 ? "f" : "f" + player.Entropy().wingData.FrameCount.ToString()));
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height);
            drawInfo.DrawDataCache.Add(new DrawData(tex, offset, null, drawInfo.colorArmorBody, 0, new Vector2(drawInfo.drawPlayer.direction == 1 ? 52 : tex.Width - 52, 54), 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.cWings });
        }

    }
}

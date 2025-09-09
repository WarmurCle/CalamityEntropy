using CalamityEntropy.Content.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class WEDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.colorArmorBody.A != 0)
            {
                drawInfo.drawPlayer.Entropy().alpha = drawInfo.colorArmorBody.A / 255f;
            }
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.Entropy().hasAccVisual(RustyDetectionEquipment.ID);
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.BackAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex = CEUtils.getExtraTex("WEVisual");
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height * 0.5f);
            drawInfo.DrawDataCache.Add(new DrawData(tex, offset + new Vector2(-10 * drawInfo.drawPlayer.direction, -2), null, drawInfo.colorArmorBody, 0, tex.Size() * 0.5f, 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.cBody });
        }

    }
}

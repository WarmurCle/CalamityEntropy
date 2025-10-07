using CalamityEntropy.Content.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class ADEDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.Entropy().hasAccVisual(AzafureDetectionEquipment.ID);
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.BackAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex = CEUtils.getExtraTex("ADEVisual");
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height * 0.5f);
            drawInfo.DrawDataCache.Add(new DrawData(tex, offset + new Vector2(-12 * drawInfo.drawPlayer.direction, 4), null, drawInfo.colorArmorBody, 0, tex.Size() * 0.5f, 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.Entropy().JetpackDye });
        }

    }
}

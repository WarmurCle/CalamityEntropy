using CalamityEntropy.Content.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class OathBannerDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.Entropy().oathBannerVisual;
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Wings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D tex = CEUtils.RequestTex("CalamityEntropy/Content/Items/Accessories/Oath/OathBannerHoldout");
            int MaxFrame = 8;
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(player.width, player.height);
            int th = tex.Height / MaxFrame;
            int frame = player.Entropy().OathBannerFrameCount;
            Vector2 origin = new Vector2(tex.Width / 2 - 44, th / 2 + (player.gravDir < 0 ? 16 : 38));
            if (player.direction * player.gravDir > 0)
                origin.X = tex.Width - origin.X;
            Rectangle rect = new Rectangle(0, th * frame, tex.Width, th - 2);
            float rot = player.Entropy().FlagRot + (player.gravDir > 0 ? 0 : MathHelper.Pi);
            drawInfo.DrawDataCache.Add(new DrawData(tex, offset, rect, drawInfo.colorArmorBody, rot, origin, 1, player.direction * player.gravDir < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = player.Entropy().oathBannerDye });
        }
    }
}

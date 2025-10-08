using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class WyrmTailLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.legs == EquipLoader.GetEquipSlot(Mod, "MariviniumLeggings", EquipType.Legs);
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Leggings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture = CEUtils.getExtraTex("WyrmTail");

            Vector2 dpos = drawInfo.HeadPosition();
            dpos += new Vector2(player.direction * -8, 16).RotatedBy(player.fullRotation);
            float rot = player.Entropy().VanityTailRot;

            drawInfo.DrawDataCache.Add(new DrawData(texture, dpos, null, drawInfo.colorArmorLegs, drawInfo.drawPlayer.fullRotation + rot * drawInfo.drawPlayer.direction, new Vector2(drawInfo.playerEffect == SpriteEffects.FlipHorizontally ? 0 : texture.Width, texture.Height / 2f), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cLegs });

        }

    }
}

using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Items.Vanity.Luminar;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class MHTailLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.legs == EquipLoader.GetEquipSlot(Mod, "ScarletKilt", EquipType.Legs) || drawInfo.drawPlayer.legs == EquipLoader.GetEquipSlot(Mod, "KitsunesFan", EquipType.Legs);
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Leggings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture = CEUtils.getExtraTex("MHTail");

            Vector2 dpos = drawInfo.HeadPosition();
            dpos += new Vector2(player.direction * -8, 16).RotatedBy(player.fullRotation);
            float rot = player.Entropy().MHVanityRot;

            drawInfo.DrawDataCache.Add(new DrawData(texture, dpos, null, drawInfo.colorArmorLegs, drawInfo.drawPlayer.fullRotation + rot * drawInfo.drawPlayer.direction, new Vector2(drawInfo.playerEffect == SpriteEffects.FlipHorizontally ? 0 : texture.Width, texture.Height / 2f), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cLegs });

        }

    }
}

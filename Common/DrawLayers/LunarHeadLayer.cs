using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Vanity.Luminar;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class LunarHeadLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "LuminarRing", EquipType.Head);
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture = CEUtils.getExtraTex("LunarHairs/Stand" + ((Main.GameUpdateCount / 8) % 3).ToString());

            if (player.velocity.Y > 0)
            {
                texture = CEUtils.getExtraTex("LunarHairs/Fall");
            }
            else
            {
                if (Math.Abs(player.velocity.X) > 0.4f)
                {
                    texture = CEUtils.getExtraTex("LunarHairs/Walk" + ((Main.GameUpdateCount / 4) % 4).ToString());
                }
            }
            Vector2 headPos = drawInfo.HeadPosition(true);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(drawInfo.playerEffect == SpriteEffects.FlipHorizontally ? texture.Width - 28 : 28, texture.Height / 2f + 3), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });

            if (Main.GameUpdateCount % 320 > 310)
            {
                texture = CEUtils.getExtraTex("LunarHairs/Blink");
                drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(drawInfo.playerEffect == SpriteEffects.FlipHorizontally ? texture.Width - 28 : 28, texture.Height / 2f + 3), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });
            }

            texture = CEUtils.getExtraTex("LuminarRing");
            headPos = drawInfo.HeadPosition(false);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, Color.White * (float)(Math.Cos(Main.GlobalTimeWrappedHourly) * 0.15f + 0.8f), drawInfo.drawPlayer.headRotation, new Vector2(texture.Width / 2, texture.Height + 22 + (float)(Math.Cos(Main.GlobalTimeWrappedHourly) * 2)), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });

        }

    }
}

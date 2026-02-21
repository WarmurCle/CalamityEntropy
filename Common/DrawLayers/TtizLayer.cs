using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class TtizHeadLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "TerraTiz", EquipType.Head);
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture = CEUtils.RequestTex("CalamityEntropy/Content/Items/Vanity/Ttiz/Horn");
            Vector2 headPos = drawInfo.HeadPosition(true) + new Vector2(0, -2);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(-1 * player.direction + texture.Width * 0.5f, texture.Height), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });
        }
    }
    public class TtizBodyLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.body == EquipLoader.GetEquipSlot(Mod, "TerraTiz", EquipType.Body);
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Wings);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture = CEUtils.RequestTex("CalamityEntropy/Content/Items/Vanity/Ttiz/Wings");
            if (player.velocity.Y != 0 && (player.mount == null || !player.mount.Active))
                texture = CEUtils.RequestTex("CalamityEntropy/Content/Items/Vanity/Ttiz/Wings2");
            Vector2 headPos = drawInfo.HeadPosition(true) + new Vector2(player.direction * 2, 12);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorBody, drawInfo.drawPlayer.fullRotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cBody });
        }
    }
}

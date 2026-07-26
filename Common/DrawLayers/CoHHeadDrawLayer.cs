using CalamityEntropy;
using CalamityEntropy.Content.Items.Vanity;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoHHeadDrawLayer.Common.DrawLayers
{
    public class CoHHeadDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "CrystalofHeart", EquipType.Head);
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            var mp = player.GetModPlayer<VanityModPlayer>();
            Texture2D texture = CEUtils.RequestTex("CalamityEntropy/Content/Items/Vanity/CrystalofHeart_Hair");
            Vector2 headPos = drawInfo.HeadPosition(true);

            Vector2 offset = new Vector2(-2, 0);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(texture.Width * 0.5f - player.direction * offset.X, texture.Height * 0.5f - offset.Y), 1, drawInfo.playerEffect));
        }

    }
}

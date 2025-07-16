using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Vanity.Luminar;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class SoraRingLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "MysteriousBook", EquipType.Head);
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture;
            Vector2 headPos;
            
            texture = CEUtils.getExtraTex("sRing");
            headPos = drawInfo.HeadPosition(true);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, Color.White, drawInfo.drawPlayer.headRotation, new Vector2(texture.Width / 2 - 1, texture.Height + 24), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });
            
        }

    }
}

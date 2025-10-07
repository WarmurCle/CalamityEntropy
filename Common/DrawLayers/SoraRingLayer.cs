using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class SoraRingLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            var drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.dead)
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
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, Color.White, drawInfo.drawPlayer.headRotation, new Vector2(texture.Width / 2 - 3 * player.direction, texture.Height + 20), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });

        }

    }
}

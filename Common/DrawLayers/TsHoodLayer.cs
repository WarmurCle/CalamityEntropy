using CalamityEntropy.Content.Items.Vanity;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class TsHoodLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            var drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "TsumugisHood", EquipType.Head) && drawPlayer.GetModPlayer<VanityModPlayer>().SpecialFlag == 0;
        }

        public override bool IsHeadLayer => true;

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            Texture2D texture;
            Vector2 headPos;

            texture = CEUtils.RequestTex("CalamityEntropy/Content/Items/Vanity/TsumugisHood_Hood");
            headPos = drawInfo.HeadPosition(true);
            drawInfo.DrawDataCache.Add(new DrawData(texture, headPos, null, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(texture.Width / 2 - 1 * player.direction, texture.Height / 2 + 9 * player.gravDir), 1, drawInfo.playerEffect) { shader = drawInfo.drawPlayer.cHead });
        }
    }
}

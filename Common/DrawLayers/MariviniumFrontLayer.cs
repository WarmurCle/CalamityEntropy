using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class MariviniumFrontLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.body == EquipLoader.GetEquipSlot(Mod, "MariviniumBodyArmor", EquipType.Body);
        }

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.ArmOverItem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D front = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Armor/Marivinium/Front").Value;
            Player player = drawInfo.drawPlayer;
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height - 16) + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawInfo.drawPlayer.gravDir;
            drawInfo.DrawDataCache.Add(new DrawData(front, offset, null, drawInfo.colorArmorBody, player.fullRotation, (front.Size() / 2f), 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.cBody });

        }

    }
}

using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.DrawLayers
{
    public class MariviniumBackLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
                return false;
            return drawInfo.drawPlayer.Entropy().mariviniumBody;
        }
            
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.ArmorLongCoat);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D back = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Armor/Marivinium/Back").Value;
            Player player = drawInfo.drawPlayer;
            Vector2 offset = drawInfo.GetFrameOrigin() + new Vector2(drawInfo.drawPlayer.width, drawInfo.drawPlayer.height - 10);
            drawInfo.DrawDataCache.Add(new DrawData(back, offset, null, drawInfo.colorArmorBody, player.fullRotation, (new Vector2(player.direction > 0 ? 28 : 48 - 28, 20)), 1, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) { shader = drawInfo.drawPlayer.cBody });
        
        }

    }
}

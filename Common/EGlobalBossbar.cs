using CalamityEntropy.Content.NPCs.Cruiser;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EGlobalBossbar : GlobalBossBar
    {

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {

            if (npc.type == ModContent.NPCType<CruiserHead>())
            {
                if (((CruiserHead)npc.ModNPC).noaitime > 0)
                {
                    return false;
                }
                Texture2D barv = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/CruiserBarB").Value;
                Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/CruiserBar").Value;
                Texture2D barp = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/CruiserBarProgress").Value;

                spriteBatch.Draw(barv, drawParams.BarCenter, null, Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(barp, drawParams.BarCenter, new Rectangle(0, 0, (int)(70 + 416f * ((CruiserHead)npc.ModNPC).ProgressDraw), 74), Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(bar, drawParams.BarCenter, null, Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);

                return false;
            }
            return true;
        }
    }

}

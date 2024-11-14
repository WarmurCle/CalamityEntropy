using CalamityEntropy.NPCs.Cruiser;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy
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
                Texture2D barv = ModContent.Request<Texture2D>("CalamityEntropy/Bossbar/CruiserBarB").Value;
                Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Bossbar/CruiserBar").Value;
                Texture2D barp = ModContent.Request<Texture2D>("CalamityEntropy/Bossbar/CruiserBarProgress").Value;

                spriteBatch.Draw(barv, drawParams.BarCenter, null, Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(barp, drawParams.BarCenter, new Rectangle(0, 0, (int)(70 + 416f * ((CruiserHead)npc.ModNPC).ProgressDraw), 74), Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(bar, drawParams.BarCenter, null, Color.White, 0, new Vector2(bar.Width, bar.Height) / 2, 1, SpriteEffects.None, 0);

                return false;
            }
            return true;
        }
    }
    
}

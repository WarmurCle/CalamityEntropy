using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityMod.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class RepMusicScene : ModSceneEffect
    {
        
        public override SceneEffectPriority Priority => (SceneEffectPriority)16;

        public override bool IsSceneEffectActive(Player player)
        {
            if(ModContent.GetInstance<Config>() == null)
            {
                return false;
            }
            if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.boss && Util.Util.getDistance(npc.Center, player.Center) < 8000 && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || npc.ModNPC == null || npc.ModNPC.Mod is not CalamityMod.CalamityMod) && !BossRushEvent.BossRushActive)
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/RepBossTrack");

       
    }
}

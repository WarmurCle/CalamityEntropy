using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityMod;
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
    public class OtherMusScene : ModSceneEffect
    {
        
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            if(NPC.FindFirstNPC(ModContent.NPCType<NihilityActeriophage>()) != -1)
            {
                return true;
            }
            return false;

        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/vtfight");

       
    }
}

using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.NPCs.Cruiser;
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
    public class DeliriumMusicScene : ModSceneEffect
    {
        
        public override SceneEffectPriority Priority => (SceneEffectPriority)100;

        public override bool IsSceneEffectActive(Player player)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.TryGetGlobalNPC<DeliriumGlobalNPC>(out var d))
                {
                    if (d.delirium)
                    {
                        return true;
                    }
                }
            }
            
            return false;

        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/deliriumfight");

       
    }
}

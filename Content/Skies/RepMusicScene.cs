using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Events;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class RepMusicScene : ModSceneEffect
    {

        public override SceneEffectPriority Priority => (SceneEffectPriority)16;

        public override bool IsSceneEffectActive(Player player)
        {
            if (ModContent.GetInstance<Config>() == null)
            {
                return false;
            }
            if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.IsABoss() && CEUtils.getDistance(npc.Center, player.Center) < 8000 && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || npc.ModNPC == null || npc.ModNPC.Mod is not CalamityMod.CalamityMod) && !BossRushEvent.BossRushActive)
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

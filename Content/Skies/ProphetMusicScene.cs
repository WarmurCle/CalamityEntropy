using CalamityEntropy.Content.NPCs.Prophet;
using CalamityMod.Events;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class ProphetMusicScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => (SceneEffectPriority)12;

        public override bool IsSceneEffectActive(Player player)
        {
            if (BossRushEvent.BossRushActive)
                return false;
            if (NPC.FindFirstNPC(ModContent.NPCType<TheProphet>()) != -1)
            {
                return true;
            }
            return false;

        }
        public override int Music
        {
            get
            {
                int n = NPC.FindFirstNPC(ModContent.NPCType<TheProphet>());
                if (n != -1)
                    return n.ToNPC().ModNPC.Music;
                return base.Music;
            }
        }
    }
}

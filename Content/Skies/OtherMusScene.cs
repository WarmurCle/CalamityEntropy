using CalamityEntropy.Content.NPCs.NihilityTwin;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class OtherMusScene : ModSceneEffect
    {

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.FindFirstNPC(ModContent.NPCType<NihilityActeriophage>()) != -1)
            {
                return true;
            }
            return false;

        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/vtfight");


    }
}

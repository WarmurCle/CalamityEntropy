using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class CBScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<CruiserHead>()) || Main.LocalPlayer.Entropy().crSky > 0;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityEntropy:Cruiser", isActive);
        }
    }
}

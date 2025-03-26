using CalamityMod;
using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Silence : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Calamity().wearingRogueArmor = true;
            player.Calamity().rogueStealthMax += 0.01f;
        }
        public override Color getColor()
        {
            return Color.Black;
        }
        public override int getRollChance()
        {
            return 4;
        }

        public override bool Precious()
        {
            return true;
        }
    }
}

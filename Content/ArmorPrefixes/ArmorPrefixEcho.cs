using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class ArmorPrefixEcho : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.Entropy().RogueStealthRegen += 0.04f;
        }
        public override Color getColor()
        {
            return Color.DarkBlue;
        }
        public override int getRollChance()
        {
            return 0;
        }
        public override bool Dramatic()
        {
            return true;
        }
        public override bool Precious()
        {
            return true;
        }
    }
}

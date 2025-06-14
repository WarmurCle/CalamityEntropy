using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Miracle : ArmorPrefix
    {
        public override void UpdateEquip(Player player, Item item)
        {
            player.Entropy().damageReduce += 0.01f;
            player.Entropy().light += 0.4f;
            player.GetDamage(DamageClass.Generic) += 0.04f;
        }
        public override float AddDefense()
        {
            return 0.1f;
        }
        public override int getRollChance()
        {
            return 1;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
        public override bool Dramatic()
        {
            return true;
        }
    }
}

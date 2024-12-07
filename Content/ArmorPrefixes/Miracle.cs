using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Miracle : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().damageReduce += 0.02f;
            player.Entropy().light += 0.2f;
            player.GetDamage(DamageClass.Generic) += 0.05f;
        }
        public override float AddDefense()
        {
            return 0.25f;
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

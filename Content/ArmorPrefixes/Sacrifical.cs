using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Sacrifical : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.6f;
        }
        public override float AddDefense()
        {
            return -0.9f;
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
        public override bool Precious()
        {
            return true;
        }
        public override bool? canApplyTo(Item item)
        {
            return Main.rand.NextBool(2);
        }
    }
}

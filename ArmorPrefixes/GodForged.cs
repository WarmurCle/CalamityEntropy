using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.ArmorPrefixes
{
    public class GodForged : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().damageReduce += 0.05f;
            player.GetDamage(DamageClass.Generic) += 0.05f;
        }
        public override float AddDefense()
        {
            return 0.15f;
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

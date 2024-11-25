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
    public class Reckless : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.3f;
        }
        public override Color getColor()
        {
            return Color.Red;
        }
        public override int getRollChance()
        {
            return 2;
        }
        public override float AddDefense()
        {
            return -0.5f;
        }
    }
}

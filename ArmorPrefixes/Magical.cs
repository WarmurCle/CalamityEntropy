using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Magical : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().ManaCost -= 0.1f;
            player.statManaMax2 += 10;
            player.Entropy().enhancedMana += 0.1f;
        }
        public override Color getColor()
        {
            return Color.LightBlue;
        }
        public override int getRollChance()
        {
            return 1;
        }
    }
}

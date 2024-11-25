using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Regen : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().lifeRegenPerSec += 3;
        }
        public override int getRollChance()
        {
            return 3;
        }
        public override Color getColor()
        {
            return Color.Pink;
        }
    }
}

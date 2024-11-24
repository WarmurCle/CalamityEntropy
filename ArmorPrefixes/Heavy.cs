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
    public class Heavy : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().moveSpeed -= 0.1f;
        }
        public override Color getColor()
        {
            return Color.Gray;
        }
        public override int getRollChance()
        {
            return 10;
        }
    }
}

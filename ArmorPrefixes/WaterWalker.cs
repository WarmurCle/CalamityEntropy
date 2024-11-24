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
    public class WaterWalker : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.waterWalk = true;
        }
        public override int getRollChance()
        {
            return 5;
        }
        public override bool? canApplyTo(Item item)
        {
            if (item.legSlot == -1)
            {
                return false;
            }
            return base.canApplyTo(item);
        }
    }
}

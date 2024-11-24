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
    public class Shining : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().light += 0.15f;
        }
        public override int getRollChance()
        {
            return 10;
        }
        public override bool? canApplyTo(Item item)
        {
            if (item.headSlot == -1)
            {
                return false;
            }
            return base.canApplyTo(item);
        }
    }
}

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
    public class Resilient : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.jumpSpeedBoost += 2f;
        }
        public override int getRollChance()
        {
            return 10;
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

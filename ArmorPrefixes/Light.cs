using CalamityEntropy.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Light : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().moveSpeed += 0.1f;
        }
    }
}

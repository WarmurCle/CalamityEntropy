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
    public class Thorny : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().Thorn += 1f;
        }
        public override int getRollChance()
        {
            return 5;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}

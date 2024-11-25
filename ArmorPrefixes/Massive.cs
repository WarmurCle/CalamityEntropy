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
    public class Massive : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().moveSpeed -= 0.15f;
        }
        public override float AddDefense()
        {
            return 0.35f;
        }
        public override int getRollChance()
        {
            return 4;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}

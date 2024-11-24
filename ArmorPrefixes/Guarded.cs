using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Guarded : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            
        }
        public override float AddDefense()
        {
            return 0.2f;
        }
        public override int getRollChance()
        {
            return 4;
        }
        public override Color getColor()
        {
            return Color.Orange;
        }
    }
}

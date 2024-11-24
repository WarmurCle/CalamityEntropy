using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Hard : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            
        }
        public override float AddDefense()
        {
            return 0.1f;
        }
        public override int getRollChance()
        {
            return 10;
        }
    }
}

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
    public class Biochemistry : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().DebuffImmuneChance += 0.35f;
        }
        public override int getRollChance()
        {
            return 1;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}

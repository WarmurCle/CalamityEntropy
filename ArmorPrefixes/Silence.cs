using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.ArmorPrefixes
{
    public class Silence : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Calamity().wearingRogueArmor = true;
            player.Calamity().rogueStealthMax += 0.2f;
        }
        public override Color getColor()
        {
            return Color.Black;
        }
        public override int getRollChance()
        {
            return 4;
        }
    }
}

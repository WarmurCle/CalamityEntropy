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
    public class Great : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
        }
        public override float AddDefense()
        {
            return 0.1f;
        }
        public override int getRollChance()
        {
            return 2;
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
    }
}

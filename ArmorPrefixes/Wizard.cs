using CalamityEntropy.Util;
using CalamityMod;
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
    public class Wizard : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.maxMinions += 1;
        }
        public override int getRollChance()
        {
            return 2;
        }
    }
}

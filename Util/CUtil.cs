using CalamityMod.Items.Weapons.Rogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalamityMod;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Util
{
    public static class CUtil
    {
        public static DamageClass rougeDC;
        public static void load()
        {
            rougeDC = ModContent.GetInstance<RogueDamageClass>();
        }
    }
}

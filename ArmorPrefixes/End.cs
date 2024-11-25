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
    public class End : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.3f;
            player.GetCritChance(DamageClass.Generic) += 15;
            player.GetKnockback(DamageClass.Generic) += 0.3f;
            player.Entropy().Thorn += 2f;
            player.Entropy().AttackVoidTouch += 0.1f;
        }
        public override float AddDefense()
        {
            return 0.2f;
        }
        public override Color getColor()
        {
            return Color.DarkRed;
        }
        public override int getRollChance()
        {
            return 1;
        }
        public override bool Dramatic()
        {
            return true;
        }
        public override bool? canApplyTo(Item item)
        {
            return Main.rand.NextBool(2);
        }
        public override bool Precious()
        {
            return true;
        }
    }
}

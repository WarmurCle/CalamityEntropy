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
    public class VoidTouched : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.Entropy().AttackVoidTouch += 0.4f;
        }
        public override int getRollChance()
        {
            return 1;
        }
        public override bool? canApplyTo(Item item)
        {
            return Main.rand.NextBool(2);
        }
        public override Color getColor()
        {
            return Color.Violet;
        }
        public override bool Dramatic()
        {
            return true;
        }

        public override bool Precious()
        {
            return true;
        }
    }
}

using CalamityEntropy.Projectiles;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityEntropy.Buffs
{
    public class SoyMilkBuff : ModBuff
    {
        public int counter;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) *= 2;
            player.GetDamage(DamageClass.Generic) *= 0.54f;
            player.Calamity().rogueStealth = 0;
        }
    }
}
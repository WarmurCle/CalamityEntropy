using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.FurnitureStatigel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEKingSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreKingSlime>();
        public override void UpdateEffects(Player player)
        {
            player.jumpSpeedBoost += 1f;
            player.Entropy().moveSpeed -= 0.16f;
        }
    }
    public class LEDesertScourge : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreDesertScourge>();
        public override void UpdateEffects(Player player)
        {
            player.breathMax += 40;
        }
    }
    public class LEEOC : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreEyeofCthulhu>();
        public static float Value = 0.05f;
        public override void UpdateEffects(Player player)
        {
            player.Entropy().DashCD -= Value;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        { 
            tooltip.Text = tooltip.Text.Replace("{1}", Value.ToPercent().ToString());
        }
    }
    public class LECabulon : LoreEffect
    {
        public static int BuffTime = 15;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", BuffTime.ToString());
        }
        public override int ItemType => ModContent.ItemType<LoreCrabulon>();
    }

    public class LEBOC : LoreEffect
    {
        public static int Crit = 3;
        public static float DamageDec = 0.02f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", Crit.ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", DamageDec.ToPercent().ToString());
        }
        public override void UpdateEffects(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += Crit;
            player.GetDamage(DamageClass.Generic) -= DamageDec;
        }
        public override int ItemType => ModContent.ItemType<LoreBrainofCthulhu>();
    }
    public class LEQueenBee : LoreEffect
    {
        public static int CritDec = 3;
        public static float Damage = 0.04f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", CritDec.ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", Damage.ToPercent().ToString());
        }
        public override void UpdateEffects(Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= CritDec;
            player.GetDamage(DamageClass.Generic) += Damage;
        }
        public override int ItemType => ModContent.ItemType<LoreQueenBee>();
    }
}
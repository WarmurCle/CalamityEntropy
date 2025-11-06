using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEKingSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreKingSlime>();
        public override void UpdateEffects(Player player)
        {
            player.jumpSpeedBoost += 1f;
            player.Entropy().moveSpeed -= 0.2f;
        }
    }
    public class LEDesertScourge : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreDesertScourge>();
        public override void UpdateEffects(Player player)
        {
            player.breathMax += 20;
        }
    }
    public class LEEOC : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreEyeofCthulhu>();
        public static float Value = 0.04f;
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
        public static int BuffTime = 5;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", BuffTime.ToString());
        }
        public override int ItemType => ModContent.ItemType<LoreCrabulon>();
    }

    public class LEBoc : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreBrainofCthulhu>();
        public override void UpdateEffects(Player player)
        {
            player.buffImmune[BuffID.Bleeding] = true;
        }
    }

    public class LEEOW : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreEaterofWorlds>();
        public override void UpdateEffects(Player player)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
        }
    }

    public class LEHiveCrimson : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LorePerforators>();
        public static int HealAmount = 20;
        public static float chance = 0.15f;

        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", chance.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", HealAmount.ToString());
        }
    }
    public class LEHiveCorrupt : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreHiveMind>();
        public static float DamageAddition = 0.04f;
        public static int TimeSec = 3;

        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", (TimeSec * 60).ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", DamageAddition.ToPercent().ToString());
        }
    }
    public class LEQueenBee : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreQueenBee>();
        public static float Damage = 0.05f;
        public static int CritDecrese = 4;
        public override void UpdateEffects(Player player)
        {
            player.GetDamage(DamageClass.Generic) += Damage;
            player.GetCritChance(DamageClass.Generic) -= CritDecrese;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", Damage.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", CritDecrese.ToString());
        }
    }
    public class LESkeletron : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreSkeletron>();
        public static float Perc = 0.15f;
        public static int AmountLimit = 300;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", AmountLimit.ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", Perc.ToPercent().ToString());
        }
    }
    public class LESlimeGod : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreSlimeGod>();
        public static float JumpSpeedBoost = 1;
        public override void UpdateEffects(Player player)
        {
            player.jumpSpeedBoost += JumpSpeedBoost;
        }
    }
}
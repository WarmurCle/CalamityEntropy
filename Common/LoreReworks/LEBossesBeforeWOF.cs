using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            tooltip.Text = tooltip.Text.Replace("[0]", chance.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("[1]", HealAmount.ToString());
        }
    }
}
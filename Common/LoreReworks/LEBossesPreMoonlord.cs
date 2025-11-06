using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEWof : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreWallofFlesh>();
        public static int Cooldown = 20;
        public static float DmgReduce = 0.05f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DmgReduce.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", Cooldown.ToString());
        }
    }
    public class LEQueenSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreQueenSlime>();
        public static float FallingSpeedAdd = 0.05f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", FallingSpeedAdd.ToPercent().ToString());
        }
        public override void UpdateEffects(Player player)
        {
            player.Entropy().FallSpeed += FallingSpeedAdd;
        }
    }
    public class LECryo : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreArchmage>();
        public override void UpdateEffects(Player player)
        {
            for (int b = 0; b < Player.MaxBuffs; b++)
            {
                if (player.buffType[b] == BuffID.Chilled || player.buffType[b] == BuffID.Frozen || player.buffType[b] == ModContent.BuffType<GlacialState>())
                {
                    if (player.buffTime[b] > 2 && Main.GameUpdateCount % 2 == 0)
                    {
                        player.buffTime[b] -= 1;
                    }
                }
            }
        }
    }
}
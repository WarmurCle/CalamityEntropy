using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class RuneWing : ModItem
    {
        public static float HorSpeed = 7f;
        public static float AccMul = 1.2f;
        public static int wTime = 170;
        public static int MAXDASHTIME = 30;//最大冲刺时间（帧）
        public static int DashVelo = 56; //冲刺速度（像素）
        public static int MaxCooldownTick = 30 * 60; //最大冲刺时间时的冷却（帧）
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", HorSpeed);
            tooltips.Replace("[B]", AccMul);
            tooltips.Replace("[C]", wTime);
            tooltips.Replace("[DT]", (float)Math.Round(MAXDASHTIME / 60f, 1));
            tooltips.IntegrateHotkey(CEKeybinds.RuneDashHotKey);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip("RuneWing", !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual("RuneWing");
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenRising = 0.12f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.8f;
            constantAscend = 0.13f;
        }

    }
}

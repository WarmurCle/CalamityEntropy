using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories.Wings;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class RuneWing : BaseWings, ISpecialDrawingWing
    {
        public static float HorSpeed = 7.5f;
        public static float AccMul = 1.2f;
        public static int wTime = 170;
        public static int MAXDASHTIME = 30;//最大冲刺时间（帧）
        public static int DashVelo = 56; //冲刺速度（像素）
        public static int MaxCooldownTick = 40 * 60; //最大冲刺时间时的冷却（帧）
        public override float BonusAscentWhileFalling => 1f;
        public override float BonusAscentWhileRising => 0.12f;
        public override float RisingSpeedThreshold => 1f;
        public override float MaxAscentSpeed => 2.8f;
        public override float BaseAscent => 0.13f;
        public int AnimationTick => 4;
        public int FallingFrame => 0;
        public int MaxFrame => 5;
        public int SlowFallingFrame => 5;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
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

    }
}

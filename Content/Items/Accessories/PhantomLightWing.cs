using CalamityEntropy.Common;
using CalamityEntropy.Utilities;
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
    public class PhantomLightWing : ModItem, ISpecialDrawingWing
    {
        public static float HorSpeed = 5.8f;
        public static float AccMul = 1.2f;
        public static int wTime = 160;
        public int AnimationTick => 4;
        public int FallingFrame => 2;
        public int MaxFrame => 8;
        public int SlowFallingFrame => 1;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", HorSpeed);
            tooltips.Replace("[B]", AccMul);
            tooltips.Replace("[C]", wTime);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip("PLWing", !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual("PLWing");
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenRising = 0.12f;
            maxCanAscendMultiplier = 1.2f;
            maxAscentMultiplier = 2.8f;
            constantAscend = 0.13f;
        }

    }
}

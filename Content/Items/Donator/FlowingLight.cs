using CalamityEntropy.Common;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    [AutoloadEquip(EquipType.Wings)]
    public class FlowingLight : BaseWings, IDonatorItem
    {
        public override float BonusAscentWhileFalling => 1f;
        public override float BonusAscentWhileRising => 0.17f;
        public override float RisingSpeedThreshold => 1.5f;
        public override float MaxAscentSpeed => 4f;
        public override float BaseAscent => 0.15f;

        public string DonatorName => "五彩斑斓的黑";

        public override void SetStaticDefaults() => ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(540, 12f, 3f);


        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 50;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.accessory = true;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.Text.StartsWith("$"))
            {
                DrawableTooltipLine nLine = new DrawableTooltipLine(new(Mod, "-", line.Text.Replace("$", "")), line.Index, line.X, line.Y, line.Color);
                BurnishedAuric.Draw(Item, nLine);
                return false;
            }
            return true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip("FlowingLightWing", !hideVisual);
            CalamityPlayer modPlayer = player.Calamity();
            player.accRunSpeed = 9f;
            player.moveSpeed += 0.18f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.noFallDmg = true;
            modPlayer.seraphTracers = true;

            if (player.controlJump && player.controlDown && player.wingTime > 0)
            {
                player.velocity.Y = BonusAscentWhileFalling + 0.142f;
            }
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual("FlowingLightWing");
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SeraphTracers>()
                .AddIngredient<WingsofRebirth>()
                .AddIngredient<FadingRunestone>(2)
                .AddTile<VoidWellTile>()
                .DisableDecraft()
                .Register();
        }
    }
}

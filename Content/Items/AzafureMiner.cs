﻿using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class AzafureMiner : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 50;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 6;
            Item.useTime = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AzafureMinerTile>();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<EnergyCore>()
                .AddIngredient<DubiousPlating>(10)
                .AddIngredient<MysteriousCircuitry>(6)
                .AddIngredient(ItemID.IronBar)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}

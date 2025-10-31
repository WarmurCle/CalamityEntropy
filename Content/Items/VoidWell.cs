using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class VoidWell : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 48;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<VoidWellTile>();
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidCondenser>())
                .AddIngredient(ModContent.ItemType<VoidScales>(), 10)
                .AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 10)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}

using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Util;
using CalamityMod.CalPlayer;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;

namespace CalamityEntropy.Content.Items
{
    public class AbyssalAltar : ModItem
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
            Item.createTile = ModContent.TileType<AbyssalAltarTile>();
            Item.rare = ModContent.RarityType<AbyssalBlue>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AltarOfTheAccursedItem>())
                .AddIngredient(ModContent.ItemType<WyrmTooth>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

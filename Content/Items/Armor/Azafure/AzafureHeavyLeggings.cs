using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Azafure
{
    [AutoloadEquip(EquipType.Legs)]
    public class AzafureHeavyLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 4;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override void UpdateEquip(Player player)
        {
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(6)
                .AddIngredient(ItemID.Obsidian, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

}

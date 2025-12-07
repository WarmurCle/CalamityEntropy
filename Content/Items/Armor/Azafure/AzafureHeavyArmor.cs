using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Azafure
{
    [AutoloadEquip(EquipType.Body)]
    public class AzafureHeavyArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 8;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override void UpdateEquip(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(10)
                .AddIngredient(ItemID.Obsidian, 8)
                .AddIngredient(ItemID.HellstoneBar, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }
}

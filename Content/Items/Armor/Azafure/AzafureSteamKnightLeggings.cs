using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Azafure
{
    [AutoloadEquip(EquipType.Legs)]
    public class AzafureSteamKnightLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.defense = 34;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateEquip(Player player)
        {
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzafureHeavyLeggings>()
                .AddRecipeGroup(CERecipeGroups.AnyOrichalcumBar, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

}

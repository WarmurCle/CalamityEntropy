using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Azafure
{
    [AutoloadEquip(EquipType.Body)]
    public class AzafureSteamKnightArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.defense = 15;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateEquip(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzafureHeavyArmor>()
                .AddRecipeGroup(CERecipeGroups.AnyOrichalcumBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

    }
}

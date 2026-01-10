using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.AzafureT3
{
    [AutoloadEquip(EquipType.Legs)]
    public class AcropolisHeavyLegArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.defense = 26;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.15f;
            player.jumpSpeedBoost += 0.2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzafureSteamKnightLeggings>()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient<UnholyEssence>(4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

}

using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Legs)]
    public class VoidEaterLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 20;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.18f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NihilityFragments>(5)
                .AddIngredient(ModContent.ItemType<Necroplasm>(), 6)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

}

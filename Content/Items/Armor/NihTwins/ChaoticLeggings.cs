using CalamityMod.Items;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Legs)]
    public class ChaoticLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 34;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.08f;
            player.GetCritChance(DamageClass.Generic) += 13;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChaoticPiece>(5)
                .AddIngredient<ExodiumCluster>(6)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

}

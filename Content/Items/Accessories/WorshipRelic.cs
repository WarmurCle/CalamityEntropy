using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class WorshipRelic : ModItem
    {
        public static int ArrowDamage = 100;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().worshipRelic = true;
            player.Entropy().NoNaturalStealthRegen = true;
            player.Entropy().WeaponsNoCostRogueStealth = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowPact>(1)
                .AddIngredient<ScoriaBar>(6)
                .AddIngredient<SolarVeil>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

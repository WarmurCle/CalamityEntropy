using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
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
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 20;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.18f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OmegaBlueTentacles>()
                .AddIngredient<WyrmTooth>(5)
                .AddIngredient<FadingRunestone>()
                .AddTile<AbyssalAltarTile>()
                .Register();
        }
    }

}

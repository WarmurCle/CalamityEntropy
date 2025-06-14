using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class HeartOfStorm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<GlowPurple>();
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().heartOfStorm = true;
        }

        public override void AddRecipes()
        {
        }
    }
}

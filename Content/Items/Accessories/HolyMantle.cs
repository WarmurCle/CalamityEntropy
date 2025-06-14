using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class HolyMantle : ModItem
    {
        public static int Cooldown = 100 * 60;
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 86;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().holyMantle = true;
        }

        public override void AddRecipes()
        {
        }
    }
}

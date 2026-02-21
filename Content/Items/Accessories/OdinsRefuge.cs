using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OdinsRefuge : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 86;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.accessory = true;
            Item.defense = 24;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().holyMantle = true;
            ModContent.GetInstance<AsgardianAegis>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<RampartofDeities>().UpdateAccessory(player, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AsgardianAegis>(), 1).
                AddIngredient(ModContent.ItemType<RampartofDeities>(), 1).
                AddIngredient(ModContent.ItemType<HolyMantle>(), 1).
                AddIngredient(ModContent.ItemType<VoidBar>(), 10).
                AddTile(ModContent.TileType<VoidWellTile>()).
                Register();
        }
    }
}

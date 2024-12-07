using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
	public class HolyMoonlight : ModItem
	{

		public override void SetDefaults() {
			Item.width = 86;
			Item.height = 86;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType <VoidPurple>();
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 100;
            player.GetDamage(DamageClass.Magic) *= 1.1f;
            player.Entropy().holyMoonlight = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<RoverDrive>(), 1).
                AddIngredient(ModContent.ItemType<ManaPolarizer>(), 1).
                AddIngredient(ModContent.ItemType<CryoStone>(), 1).
                AddIngredient(ModContent.ItemType<VoidBar>(), 6).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}

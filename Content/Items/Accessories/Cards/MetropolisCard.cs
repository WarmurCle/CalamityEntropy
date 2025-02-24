using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Cards
{
	public class MetropolisCard : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rogueStealthMax += 0.10f;
            player.GetModPlayer<EModPlayer>().metropolisCard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 5).
                AddIngredient(ModContent.ItemType<EssenceofHavoc>(), 3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

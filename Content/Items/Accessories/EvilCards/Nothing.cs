using CalamityEntropy.Common;
using CalamityEntropy.Util;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
	public class Nothing : ModItem
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
            player.GetModPlayer<EModPlayer>().NothingCard = true;
            player.Entropy().AttackVoidTouch += 0.6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TissueSample, 3)
                .AddIngredient(ItemID.CrimtaneBar, 5).Register();

            CreateRecipe().AddIngredient(ItemID.ShadowScale, 3)
                .AddIngredient(ItemID.DemoniteBar, 5).Register();
        }
    }
}

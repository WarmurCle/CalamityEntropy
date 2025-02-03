using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
	public class WhipOfService : ModItem
	{

		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<WhipOfServiceProjectile>(), 24, 2, 4, 36);
			Item.rare = ModContent.RarityType<Violet>();
			Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
		}


        public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.BlandWhip)
				.AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 2)
				.Register();
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}

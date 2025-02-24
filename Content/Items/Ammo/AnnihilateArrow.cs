using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Ammo
{
	// This example is similar to the Wooden Arrow item
	public class AnnihilateArrow : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 14;
			Item.height = 36;

			Item.damage = 25; // Keep in mind that the arrow's final damage is combined with the bow weapon damage.
			Item.DamageType = DamageClass.Ranged;
			Item.rare = ModContent.RarityType<VoidPurple>();
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 1.8f;
			Item.value = Item.sellPrice(silver: 92);
			Item.shoot = ModContent.ProjectileType<AnnihilateArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 2f; // The speed of the projectile.
			Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
		}

        public override void AddRecipes()
        {
			CreateRecipe(100).AddIngredient(ModContent.ItemType<VoidBar>())
				.AddTile(ModContent.TileType<CosmicAnvil>())
				.Register();
        }
    }
}

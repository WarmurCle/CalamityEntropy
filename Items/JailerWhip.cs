using CalamityEntropy.Buffs;
using CalamityEntropy.Projectiles;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Items
{
	public class JailerWhip : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(JailerWhipDebuff.TagDamage);

		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<JailerWhipProjectile>(), 18, 2, 4);
			Item.rare = ItemRarityID.Orange;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 8)
				.AddIngredient(ItemID.Chain, 6)
				.AddIngredient(ItemID.Silk, 4)
				.AddTile(TileID.Anvils).Register();
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}

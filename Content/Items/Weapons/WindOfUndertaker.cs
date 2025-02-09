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
	public class WindOfUndertaker : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CruiserWhipDebuff.TagDamage);

		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<WindOfUndertakerProjectile>(), 220, 2, 8, 28);
			Item.rare = ModContent.RarityType<Violet>();
			Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
        }
        public override void AddRecipes() {
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}

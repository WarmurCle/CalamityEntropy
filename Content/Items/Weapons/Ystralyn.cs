using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
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
	public class Ystralyn : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DragonWhipDebuff.TagDamage);

		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<YstralynProj>(), 460, 2, 4, 24);
			Item.rare = ModContent.RarityType<AbyssalBlue>();
			Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.autoReuse = true;
        }


        public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.RainbowWhip)
				.AddIngredient(ModContent.ItemType<WyrmTooth>(), 5)
				.AddTile(ModContent.TileType<AbyssalAltarTile>())
				.Register();
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}

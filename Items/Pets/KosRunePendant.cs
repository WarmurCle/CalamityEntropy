using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets;
using CalamityEntropy.Buffs.Pets;
using CalamityEntropy.Projectiles.Pets.Wyrm;
using CalamityEntropy.Projectiles.Pets.DarkFissure;
using CalamityEntropy.Projectiles.Pets.Abyss;
using CalamityEntropy.Projectiles.Pets.DoG;
namespace CalamityEntropy.Items.Pets
{	
	public class KosRunePendant : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.shoot = ModContent.ProjectileType<DoG>();
			Item.buffType = ModContent.BuffType<DevourerAndTheApostles>();
		}
		
		public override bool? UseItem(Player player)
        {
			if (player.whoAmI == Main.myPlayer) {
				player.AddBuff(Item.buffType, 3600);
			}
   			return true;
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<BottleDarkMatter>())
				.AddIngredient(ModContent.ItemType<LightningPendant>())
				.AddIngredient(ModContent.ItemType<SoulCandle>())
				.AddIngredient((ModContent.ItemType<GodsSnack>()))
				.AddTile(TileID.WorkBenches).Register();
        }
    }
}
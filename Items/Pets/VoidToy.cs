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
namespace CalamityEntropy.Items.Pets
{	
	public class VoidToy: ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.shoot = ModContent.ProjectileType<VoidPalProj>();
			Item.buffType = ModContent.BuffType<VoidPal>();
		}
		
		public override bool? UseItem(Player player)
        {
			if (player.whoAmI == Main.myPlayer) {
				player.AddBuff(Item.buffType, 3600);
			}
   			return true;
		}
	}
}
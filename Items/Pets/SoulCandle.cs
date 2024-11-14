using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets;
using CalamityEntropy.Buffs.Pets;
using CalamityEntropy.Projectiles.Pets.Signus;
namespace CalamityEntropy.Items.Pets
{	
	public class SoulCandle: ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.UseSound = SoundID.Item58;
			Item.shoot = ModContent.ProjectileType<SigPetProj>();
			Item.buffType = ModContent.BuffType<DevourersAssassin>();
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
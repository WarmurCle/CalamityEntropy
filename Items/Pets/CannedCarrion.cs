using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets;
using CalamityEntropy.Buffs.Pets;
using System.IO;
using CalamityMod.Items.Materials;
using CalamityEntropy.Projectiles.Pets.Deus;
using CalamityEntropy.Projectiles.Pets.StormWeaver;
using CalamityEntropy.Projectiles.Pets.Eater;
namespace CalamityEntropy.Items.Pets
{	
	public class CannedCarrion : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.shoot = ModContent.ProjectileType<EaterProj>();
			Item.buffType = ModContent.BuffType<EaterBuff>();
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
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
using CalamityEntropy.Projectiles.Pets.DoG;
using CalamityMod.NPCs.DevourerofGods;
using Terraria.Audio;
namespace CalamityEntropy.Items.Pets
{	
	public class LostSoul: ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.UseSound = null;
			Item.shoot = ModContent.ProjectileType<LostSoulProj>();
			Item.buffType = ModContent.BuffType<LostSoulBuff>();
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
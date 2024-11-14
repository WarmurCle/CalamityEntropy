using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets;
using CalamityEntropy.Buffs.Pets;
using Terraria.Audio;
using CalamityEntropy.Projectiles.Pets.StormWeaver;
namespace CalamityEntropy.Items.Pets
{	
	public class LightningPendant: ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
            SoundStyle s = new("CalamityEntropy/Sounds/vmspawn");
            s.Volume = 0.6f;
            Item.UseSound = s;
			Item.shoot = ModContent.ProjectileType<StormWeaverPet>();
			Item.buffType = ModContent.BuffType<Rainstorm>();
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
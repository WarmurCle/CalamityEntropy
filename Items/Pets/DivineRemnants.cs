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
namespace CalamityEntropy.Items.Pets
{	
	public class DivineRemnants : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.UseSound = SoundID.Item58;
			Item.shoot = ModContent.ProjectileType<AstrumDeus>();
			Item.buffType = ModContent.BuffType<AstrumDeusBuff>();
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
            string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy");
            string myDataFilePath = Path.Combine(modFolder, "DeusKilled.txt");
            CreateRecipe().
				AddIngredient(ItemID.FallenStar, 5).
				AddIngredient(ItemID.Wood, 5).
                AddCondition(new Condition("Deus Killed", () => File.Exists(myDataFilePath))).
                AddTile(TileID.WorkBenches).
				Register();
        }
    }
}
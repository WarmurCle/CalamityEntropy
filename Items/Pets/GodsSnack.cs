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
	public class GodsSnack: ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.UseSound = DevourerofGodsHead.SpawnSound;
			Item.shoot = ModContent.ProjectileType<DoG>();
			Item.buffType = ModContent.BuffType<DoGBuff>();
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
            string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); // 获取模组文件夹路径
            string myDataFilePath = Path.Combine(modFolder, "DoGKilled.txt"); // 定义文件路径

				CreateRecipe().
				AddIngredient(ItemID.Apple, 5).
				AddCondition(new Condition("DoG Killed", () => File.Exists(myDataFilePath))).
				Register();
                CreateRecipe().
                AddIngredient(ItemID.Peach, 5).
                AddCondition(new Condition("DoG Killed", () => File.Exists(myDataFilePath))).
                Register();
                CreateRecipe().
                AddIngredient(ItemID.Mango, 5).
                AddCondition(new Condition("DoG Killed", () => File.Exists(myDataFilePath))).
                Register();
        }
    }
}
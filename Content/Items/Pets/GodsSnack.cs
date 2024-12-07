using System.IO;
using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Content.Projectiles.Pets.DoG;
using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Pets
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
            string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); // ��ȡģ���ļ���·��
            string myDataFilePath = Path.Combine(modFolder, "DoGKilled.txt"); // �����ļ�·��

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
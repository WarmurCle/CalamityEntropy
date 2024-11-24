using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityEntropy.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using CalamityEntropy.Projectiles.Chainsaw;
using CalamityMod;
namespace CalamityEntropy.Items.Chainsaw
{	
	public class MechanicalChainsaw : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.width = 42;
            Item.height = 42;
            Item.noUseGraphic = true;
			Item.useTime = 16;
			Item.useAnimation = 0;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
            Item.ArmorPenetration = 45;
			Item.value = 2000000;
			Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item23;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<MechanicalChainsaw0>();
			Item.shootSpeed = 1f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrokenChainsaw>().
                AddIngredient(ItemID.AdamantiteBar, 10).
                AddIngredient<EssenceofHavoc>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient<BrokenChainsaw>().
                AddIngredient(ItemID.TitaniumBar, 10).
                AddIngredient<EssenceofHavoc>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityEntropy.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using CalamityEntropy.Projectiles.Chainsaw;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityEntropy.Projectiles;
using CalamityMod.Particles;
namespace CalamityEntropy.Items.Chainsaw
{	
	public class BrokenChainsaw : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 6;
			Item.DamageType = DamageClass.Melee;
			Item.width = 42;
            Item.height = 42;
            Item.noUseGraphic = true;
			Item.useTime = 16;
			Item.useAnimation = 0;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 36;
			Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item23;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<BrokenChainsaw0>();
			Item.shootSpeed = 1f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DubiousPlating>(5).
                AddIngredient(ItemID.IronBar, 10).
                AddIngredient(ItemID.Chain, 1).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe().
                AddIngredient<DubiousPlating>(5).
                AddIngredient(704, 10).
                AddIngredient(ItemID.Chain, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
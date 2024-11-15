using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityEntropy.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using CalamityEntropy.Projectiles.Chainsaw;
using CalamityMod.Rarities;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
namespace CalamityEntropy.Items.Chainsaw
{	
	public class Pioneer : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 1;
			Item.DamageType = DamageClass.Melee;
			Item.width = 42;
            Item.height = 42;
            Item.noUseGraphic = true;
			Item.useTime = 16;
			Item.useAnimation = 0;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 36;
			Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item23;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<Pioneer0>();
			Item.shootSpeed = 1f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Euangelion>().
                AddIngredient<ExodiumCluster>(20).
                AddIngredient<DarksunFragment>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
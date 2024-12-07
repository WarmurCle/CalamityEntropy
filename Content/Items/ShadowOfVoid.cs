using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{	
	public class ShadowOfVoid : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 6700;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 50;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.knockBack = 4;
			Item.value = 145000;
			Item.rare = ModContent.RarityType<HotPink>();
			Item.UseSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact"){ Volume = 0.4f, Pitch = 0.1f };
			Item.shoot = ModContent.ProjectileType <CruiserShadow>();
			Item.shootSpeed = 6f;
			Item.mana = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = false;
		}

        public override void AddRecipes()
		{
			CreateRecipe().AddIngredient(ModContent.ItemType<VoidBar>(), 16)
				.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 10)
				.AddIngredient(ModContent.ItemType<VoidAnnihilate>())
				.AddIngredient(ModContent.ItemType<Silence>())
				.AddIngredient(ModContent.ItemType<VoidEcho>())
				.AddIngredient(ModContent.ItemType<WingsOfHush>())
				.AddIngredient(ModContent.ItemType<VoidRelics>())
				.AddTile(ModContent.TileType<CosmicAnvil>())
				.Register();
                
        }
	}
}
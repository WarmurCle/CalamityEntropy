using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{	
	public class ShadowOfVoid : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 1600;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 50;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.knockBack = 4;
			Item.UseSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact"){ Volume = 0.4f, Pitch = 0.1f };
			Item.shoot = ModContent.ProjectileType <CruiserShadow>();
			Item.shootSpeed = 6f;
			Item.mana = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override void AddRecipes()
		{
			CreateRecipe().AddIngredient(ModContent.ItemType<VoidBar>(), 8)
				.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5)
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
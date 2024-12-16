using CalamityEntropy.Content.Projectiles.Chainsaw;
using CalamityMod;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Chainsaw
{	
	public class EnslavedStar : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 60;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.width = 42;
            Item.height = 42;
            Item.noUseGraphic = true;
			Item.useTime = 16;
			Item.useAnimation = 0;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 36;
			Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item23;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<EnslavedStar0>();
			Item.shootSpeed = 1f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MechanicalChainsaw>().
                AddIngredient<PlagueCellCanister>(10).
                AddIngredient(ItemID.Wire ,5).
                AddIngredient<ScoriaBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
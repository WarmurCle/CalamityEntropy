using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class RustyGrenade : ModItem
    {
        public override void SetStaticDefaults()
        {
            AmmoID.Sets.IsSpecialist[Type] = true; // This item will benefit from the Shroomite Helmet.

            // This is where we tell the game which projectile to spawn when using this rocket as ammo with certain launchers.
            // This specific rocket ammo is like Rocket I's.
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.RocketLauncher].Add(Type, ModContent.ProjectileType<RustyGrenadeProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.GrenadeLauncher].Add(Type, ModContent.ProjectileType<RustyGrenadeProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.ProximityMineLauncher].Add(Type, ModContent.ProjectileType<RustyGrenadeProjectile>());
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.SnowmanCannon].Add(Type, ModContent.ProjectileType<RustyGrenadeProjectile>());
            // We also need to say which type of Celebration Mk2 rockets to use.
            // The Celebration Mk 2 only has four types of rockets. Change the projectile to match your ammo type.
            // Rocket I like   == ProjectileID.Celeb2Rocket
            // Rocket II like  == ProjectileID.Celeb2RocketExplosive
            // Rocket III like == ProjectileID.Celeb2RocketLarge
            // Rocket IV like  == ProjectileID.Celeb2RocketExplosiveLarge
            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.Celeb2].Add(Type, ProjectileID.Celeb2Rocket);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = AmmoID.Rocket;
            Item.damage = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe(250).AddIngredient(ModContent.ItemType<DubiousPlating>(), 2).AddIngredient(ItemID.IronBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}

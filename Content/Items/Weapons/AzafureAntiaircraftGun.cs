using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureAntiaircraftGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 535;
            Item.crit = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 194;
            Item.height = 42;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<BatteringRamProj>();
            Item.shootSpeed = 8;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AerialiteBar>(5)
                .AddIngredient<DubiousPlating>(15)
                .AddIngredient<EnergyCore>(2)
                .AddIngredient(ItemID.HellstoneBar, 18)
                .AddIngredient(ItemID.IronBar, 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AzAAGunHoldout : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
        }
        public float counter { get { return Projectile.ai[0]; } set { Projectile.ai[0] = value; } }
        public float BarrelOffset = 0;
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            float progress = counter / player.itemTimeMax;
        }
    }
}

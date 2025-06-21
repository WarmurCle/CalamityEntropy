using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Xytheron : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10000;
            Item.crit = 17;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 86;
            Item.height = 86;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<XytheronProj>();
            Item.shootSpeed = 16f;
            Item.ArmorPenetration = 32;
            Item.autoReuse = true;
        }

        public float charge = 0;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<WyrmDash>();
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<XytheronProj>();
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && charge > 0)
            {
                Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.UnitX) * (42 + charge * 1.9f), type, (int)(damage / 2.8f * (charge + 1)), knockback, player.whoAmI, charge);
                charge = 0;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<StarlessNight>())
                .AddIngredient(ModContent.ItemType<WyrmTooth>(), 10)
                .AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5)
                .AddTile(ModContent.TileType<AbyssalAltarTile>())
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}

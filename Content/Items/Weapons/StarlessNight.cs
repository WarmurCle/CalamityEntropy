using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class StarlessNight : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.crit = 10;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 86;
            Item.noUseGraphic = true;
            Item.height = 86;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<StarlessNightProj>();
            Item.shootSpeed = 16f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
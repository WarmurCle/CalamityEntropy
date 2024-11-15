﻿using CalamityEntropy.Projectiles;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items
{
    public class WingsOfHush : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.damage = 1240;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WohLaser>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WohLaser>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(16)), ModContent.ProjectileType<WohShot>(), (int)(damage * 0.6f), knockback, player.whoAmI, 0, Main.MouseWorld.X, Main.MouseWorld.Y);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-16)), ModContent.ProjectileType<WohShot>(), (int)(damage * 0.6f), knockback, player.whoAmI, 0, Main.MouseWorld.X, Main.MouseWorld.Y);

            return false;
        }
    }
}

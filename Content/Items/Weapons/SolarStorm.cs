using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class SolarStorm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 138;
            Item.damage = 185;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.channel = true;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SolarStormHeld>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);

        public override void AddRecipes()
        {
        }
        public override bool RangedPrefix()
        {
            return true;
        }
    }
}

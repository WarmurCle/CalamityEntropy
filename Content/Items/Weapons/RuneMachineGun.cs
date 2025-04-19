using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class RuneMachineGun : ModItem
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 70;
            Item.damage = 46;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 8;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-24, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Util.Util.PlaySound("gunshot_small" + Main.rand.Next(1, 4).ToString(), Main.rand.NextFloat(0.7f, 1.3f), position, 10, 0.6f);
            Util.Util.PlaySound("crystalsound" + Main.rand.Next(1, 3).ToString(), Main.rand.NextFloat(0.7f, 1.3f), position, 10, 0.6f);

            Projectile.NewProjectile(source, position + new Vector2(0, Main.rand.NextFloat(-10, 10)).RotatedBy(velocity.ToRotation()), velocity / 2f, ModContent.ProjectileType<RuneTorrentRanger>(), damage, 4, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}

using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Neltharion : ModItem
    {
        public int UseCount = 0;
        public override void SetDefaults()
        {
            Item.width = 146;
            Item.height = 86;
            Item.damage = 640;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 0;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<NeltharionHoldout>();
            Item.UseSound = null;
            Item.shootSpeed = 5f;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;
            Item.ArmorPenetration = 10;
        }
        public static int AmmoSavedPercent = 95;
        public override bool RangedPrefix()
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                return Main.rand.Next(100) >= AmmoSavedPercent;
            }
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI).velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }
    }

    #region Projectiles
    public class NeltharionHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Neltharion";
        public override void SetDefaults()
        {
            Projectile.width = 146;
            Projectile.height = 86;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }
    #endregion
}

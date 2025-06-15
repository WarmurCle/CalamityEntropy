using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
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
            Item.shoot = ModContent.ProjectileType<AzAAGunHoldout>();
            Item.shootSpeed = 12;
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
        public bool shoot = true;
        public bool steamSound = true;
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            int MaxTime = player.itemTimeMax * Projectile.MaxUpdates;
            float progress = counter / MaxTime;
            if(progress < 0.4f)
            {
                BarrelOffset = CEUtils.Parabola(progress / 0.4f, 40);
            }
            else
            {
                BarrelOffset = 0;
            }
            if(progress > 0.4f)
            {
                if (steamSound)
                {
                    steamSound = false;
                    for(int i = 0; i < 64; i++)
                    {
                        Color smokeColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                        smokeColor = Color.Lerp(smokeColor, Color.Gray, 0.55f);
                        HeavySmokeParticle smoke = new(Projectile.Center - Projectile.velocity.normalize() * 120, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1.2f) * -1 * Main.rand.NextFloat(6, 8), smokeColor, 40, 1.8f, 1f, 0.03f, true, 0.075f);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                    CEUtils.PlaySound("SteamAAG", 1, Projectile.Center);
                }
            }
            if(shoot && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 130, Projectile.velocity.normalize() * 12, ProjectileID.EnchantedBeam, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (shoot)
            {
                CEUtils.PlaySound("AAGShot", 1, Projectile.Center);
                CEUtils.SetShake(Projectile.Center, 10);
            }
            shoot = false;
            if(progress < MaxTime)
            {
                player.itemAnimation = player.itemTime = 3;
                player.heldProj = Projectile.whoAmI;
                Projectile.Center = player.MountedCenter + player.gfxOffY * Vector2.UnitY;
                player.Calamity().mouseWorldListener = true;
                Projectile.rotation = (player.Calamity().mouseWorld - player.MountedCenter).ToRotation();
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * player.HeldItem.shootSpeed;
            }
            else
            {
                player.itemTime = player.itemAnimation = 0;
                Projectile.Kill();
            }
            counter++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D alt = this.getTextureAlt(); 

            Main.EntitySpriteDraw(alt, Projectile.Center - Projectile.rotation.ToRotationVector2() * BarrelOffset * Projectile.scale - Main.screenPosition, null, lightColor, Projectile.rotation, alt.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            
            return false;
        }
    }
}

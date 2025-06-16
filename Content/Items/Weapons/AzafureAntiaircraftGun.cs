using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureAntiaircraftGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            AmmoID.Sets.SpecificLauncherAmmoProjectileFallback[Type] = ItemID.RocketLauncher;
        }
        public override void SetDefaults()
        {
            Item.damage = 2450;
            Item.crit = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 194;
            Item.height = 42;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<AzAAGunHoldout>();
            Item.shootSpeed = 12;
            Item.useAmmo = AmmoID.Rocket;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, type);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ScoriaBar>(6)
                .AddIngredient<DubiousPlating>(10)
                .AddIngredient(ItemID.HellstoneBar, 18)
                .AddIngredient(ItemID.MeteoriteBar, 6)
                .AddTile(TileID.MythrilAnvil)
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
        public float counter = 0;
        public float BarrelOffset = 0;
        public bool shoot = true;
        public bool steamSound = true;
        public bool slSound = true;
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            int MaxTime = player.itemTimeMax * Projectile.MaxUpdates;
            float progress = counter / MaxTime;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (progress < 0.2f)
            {
                BarrelOffset = CEUtils.Parabola(progress / 0.2f, 60);
            }
            else
            {
                BarrelOffset = 0;
            }
            if(progress > 0.6f && slSound)
            {
                slSound = false;
                CEUtils.PlaySound("shellLand", 1, Projectile.Center);
            }
            if(progress > 0.2f)
            {
                if (steamSound)
                {
                    steamSound = false;
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity.normalize() * 120, Projectile.velocity.RotatedBy(-2 * player.direction).normalize() * 16, ModContent.ProjectileType<AntiaircraftShell>(), 0, 0, Projectile.owner);
                    }
                    for(int i = 0; i < 14; i++)
                    {
                        Color smokeColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), new Color[3] {Color.White, Color.Gray, Color.LightGray});
                        smokeColor = Color.Lerp(smokeColor, Color.Gray, 0.6f) * 0.65f;
                        HeavySmokeParticle smoke = new(Projectile.Center - Projectile.velocity.normalize() * 120, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1.2f) * -1 * Main.rand.NextFloat(16, 24), smokeColor, 40, 1f, 1f, 0.03f, true, 0.075f);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                    CEUtils.PlaySound("SteamAAG", 1, Projectile.Center);
                    CEUtils.PlaySound("AAGLB", 1, Projectile.Center);
                }
            }
            if(shoot && Main.myPlayer == Projectile.owner)
            {
                player.velocity -= Projectile.velocity.normalize() * 8;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 130, Projectile.rotation.ToRotationVector2() * 42, ModContent.ProjectileType<AzAGShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
            if (shoot)
            {
                EParticle.NewParticle(new Particles.ImpactParticle(), Projectile.Center + Projectile.velocity.normalize() * 150, Vector2.Zero, Color.LightGoldenrodYellow, 0.12f, 1, true, BlendState.Additive, Projectile.rotation);

                CEUtils.PlaySound("AAGShot", 1, Projectile.Center);
                CEUtils.SetShake(Projectile.Center, 16);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 top = Projectile.Center + Projectile.velocity.normalize() * 130;
                    Vector2 sparkVelocity2 = Projectile.rotation.ToRotationVector2().RotateRandom(0.3f) * Main.rand.NextFloat(16f, 36f);
                    int sparkLifetime2 = Main.rand.Next(6, 10);
                    float sparkScale2 = Main.rand.NextFloat(0.6f, 1.4f);
                    var sparkColor2 = Color.Lerp(Color.Goldenrod, Color.Yellow, Main.rand.NextFloat(0, 1));

                    LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            shoot = false;
            if (progress < MaxTime)
            {
                player.itemAnimation = player.itemTime = 3;
                Projectile.Center = player.MountedCenter + player.gfxOffY * Vector2.UnitY + Projectile.rotation.ToRotationVector2() * 40 + new Vector2(0, -20);
                player.Calamity().mouseWorldListener = true;
                float targetRot = (player.Calamity().mouseWorld - player.MountedCenter).ToRotation();
                Projectile.velocity = CEUtils.rotatedToAngle(Projectile.velocity.ToRotation(), targetRot, 4, true).ToRotationVector2() * player.HeldItem.shootSpeed;
                player.direction = Math.Sign(Projectile.velocity.X);
            }
            else
            {
                player.itemTime = player.itemAnimation = 1;
                if (Projectile.timeLeft > 4)
                {
                    Projectile.timeLeft = 4;
                }
            }
            counter++;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
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
    public class AzAGShot : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.light = 0.4f;
            Projectile.scale = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("ystn_hit", 0.8f, Projectile.Center);
            for (int i = 0; i < 16; i++)
            {
                Vector2 top = target.Center;
                Vector2 sparkVelocity2 = Projectile.rotation.ToRotationVector2().RotateRandom(0.3f) * Main.rand.NextFloat(16f, 36f);
                int sparkLifetime2 = Main.rand.Next(16, 26);
                float sparkScale2 = Main.rand.NextFloat(1f, 1.8f);
                var sparkColor2 = Color.Lerp(Color.Goldenrod, Color.Yellow, Main.rand.NextFloat(0, 1));

                LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            var p = new Projectile();
            p.SetDefaults((int)Projectile.ai[0]);
            p.whoAmI = 0;
            dmgMult *= 0.76f;
            ProjectileLoader.OnHitNPC(p, target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= dmgMult;
        }
        public float dmgMult = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for(float i = 0; i < 1; i+= 0.2f)
            {
                EParticle.spawnNew(new Smoke() { timeleftmax = 16, Lifetime = 16 }, Projectile.Center + CEUtils.randomPointInCircle(4) - Projectile.velocity * i, Projectile.velocity * 0.6f + CEUtils.randomPointInCircle(0.4f), Color.OrangeRed, Main.rand.NextFloat(0.04f, 0.06f), 1, true, BlendState.Additive, CEUtils.randomRot());
            }
            EParticle.spawnNew(new EMediumSmoke(), Projectile.Center, CEUtils.randomPointInCircle(4), Color.LightGoldenrodYellow, Main.rand.NextFloat(0.4f, 0.8f), 1, true, BlendState.AlphaBlend, CEUtils.randomRot());
        }
    }
}

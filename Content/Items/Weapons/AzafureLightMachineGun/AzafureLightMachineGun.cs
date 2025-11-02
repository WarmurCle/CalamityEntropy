using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.AzafureLightMachineGun
{
    public class AzafureLightMachineGun : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = CEUtils.RogueDC;
            Item.width = 82;
            Item.height = 32;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AzafureLightMachineGunHeld>();
            Item.shootSpeed = 26;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Minishark)
                .AddIngredient<HellIndustrialComponents>(4)
                .AddIngredient(ItemID.AdamantiteBar, 6)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                p.ToProj().netUpdate = true;
                CEUtils.CostStealthForPlr(player);
            }
            return false;
        }
        public override float StealthDamageMultiplier => 2;
        public override float StealthKnockbackMultiplier => 2;
    }
    public class AzafureLightMachineGunHeld : ModProjectile
    {
        public float rotup = 0;
        public float rotv = 0.16f;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.HeldProjSetDefaults(CEUtils.RogueDC);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Projectile.GetOwner().Calamity().mouseWorldListener = true;
            Player player = Projectile.GetOwner();
            if (Projectile.Calamity().stealthStrike)
            {
                rotup += rotv;
                rotv *= 0.8f;
                rotup *= 0.82f;
                if (Projectile.ai[0]++ == 0)
                {
                    CEUtils.PlaySound("AAGShot", 1.55f, Projectile.Center, 2, 0.41f);
                    Projectile.timeLeft = 32;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.normalize() * 32, Projectile.velocity, ModContent.ProjectileType<AzafureLightMachineGunStealth>(), Projectile.damage * 14, Projectile.knockBack * 10, Projectile.owner).ToProj().Calamity().stealthStrike = true; ;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity.normalize() * 2, Projectile.velocity.RotatedBy(-2.3f * player.direction).normalize() * 12, ModContent.ProjectileType<ALMGShell>(), 0, 0, Projectile.owner);
                    }
                }
                player.Calamity().mouseWorldListener = true;
                Projectile.rotation = (player.Calamity().mouseWorld - player.Center).ToRotation();
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 16;
                player.SetHandRot(((player.Calamity().mouseWorld - player.Center).ToRotation().ToRotationVector2() + new Vector2(0, 1f)).ToRotation());
                player.itemAnimation = player.itemTime = 4;
                player.heldProj = Projectile.whoAmI;
                Projectile.Center = player.GetDrawCenter() + Projectile.rotation.ToRotationVector2() * 24;
                return;
            }

            if (player.channel)
            {
                Projectile.timeLeft = 4;
                player.Calamity().mouseWorldListener = true;
                Projectile.rotation = (player.Calamity().mouseWorld - player.Center).ToRotation();
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 16;
                player.SetHandRot(((player.Calamity().mouseWorld - player.Center).ToRotation().ToRotationVector2() + new Vector2(0, 1f)).ToRotation());
                player.itemAnimation = player.itemTime = 4;
                player.heldProj = Projectile.whoAmI;
                if (Projectile.ai[2]-- <= 0)
                {
                    Projectile.ai[2] = 4;
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.normalize() * 32, Projectile.velocity, ModContent.ProjectileType<ALMGLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.Center = player.GetDrawCenter() + Projectile.rotation.ToRotationVector2() * 24;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = Projectile.GetTexture();
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition - Projectile.rotation.ToRotationVector2() * 10, CEUtils.GetCutTexRect(t, 2, (int)Main.GameUpdateCount / 4 % 2, false), lightColor, Projectile.rotation + (Math.Sign(Projectile.velocity.X) * -rotup), t.Size() / new Vector2(2, 4), Projectile.scale, (Projectile.velocity.X > 0) ? SpriteEffects.None : SpriteEffects.FlipVertically);
            
            return false;
        }
    }
    public class ALMGLaser : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.light = 0.6f;
            Projectile.timeLeft = 12;
            Projectile.penetrate = -1; 
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public float dist = 0;
        public override void AI()
        {
            
            if (Projectile.ai[0]++ == 0)
            {
                Vector2 mousew = Projectile.GetOwner().Calamity().mouseWorld;
                Projectile.Center = Projectile.GetOwner().GetDrawCenter();
                Projectile.velocity = new Vector2(8, 0).RotatedBy((mousew - Projectile.Center).ToRotation());
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.Center += (mousew - Projectile.Center).normalize() + new Vector2(60, -8 * (Projectile.velocity.X > 0 ? 1 : -1)).RotatedBy(Projectile.rotation);
                dist = 0;
                List<NPC> checkNpcs = new();
                foreach(NPC n in Main.ActiveNPCs)
                {
                    if(!n.dontTakeDamage && !n.friendly && CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 1800, n.getRect(), 6))
                    {
                        checkNpcs.Add(n);
                    }
                }
                for (float d = 0; d < 1800; d += 4)
                {
                    dist = d;
                    CEUtils.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * dist, new Color(255, 120, 120), 0.5f);
                    if (!CEUtils.isAir(Projectile.Center + Projectile.rotation.ToRotationVector2() * d))
                    {
                        break;
                    }
                    bool brk = false;
                    foreach(var n in checkNpcs)
                    {
                        if((Projectile.Center + Projectile.rotation.ToRotationVector2() * d).getRectCentered(6, 6).Intersects(n.Hitbox))
                        {
                            dist += 4;
                            brk = true;
                            break;
                        }
                    }
                    if (brk)
                        break;
                }
                CEUtils.PlaySound("DudFire", 2f, Projectile.Center, 6, 0.4f);
                EParticle.NewParticle(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red, 0.5f, 1, true, BlendState.Additive, 0, 12);
                EParticle.NewParticle(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.2f, 1, true, BlendState.Additive, 0, 12);

                
                Vector2 edp = Projectile.Center + Projectile.rotation.ToRotationVector2() * dist;
                EParticle.NewParticle(new ShineParticle(), edp, Vector2.Zero, Color.Red, 0.5f, 1, true, BlendState.Additive, 0, 12);
                EParticle.NewParticle(new ShineParticle(), edp, Vector2.Zero, Color.White, 0.2f, 1, true, BlendState.Additive, 0, 12);

            }

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * dist, targetHitbox, 6);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = CEUtils.getExtraTex("MaskLaserLine");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255), Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(dist / tex.Width, Projectile.scale * 0.3f * (Projectile.timeLeft / 12f)), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 10), Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(dist / tex.Width, Projectile.scale * 0.5f * (Projectile.timeLeft / 12f)), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.GetOwner().Entropy().worshipStealthRegenTime = 18;
        }
    }

    public class AzafureLightMachineGunStealth : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, true, 1);
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 5;
        }
        public TrailParticle trail = new TrailParticle();
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ == 0)
            {
                Vector2 mousew = Projectile.GetOwner().Calamity().mouseWorld;
                Projectile.Center = Projectile.GetOwner().GetDrawCenter();
                Projectile.velocity = new Vector2(8, 0).RotatedBy((mousew - Projectile.Center).ToRotation());
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.Center += (mousew - Projectile.Center).normalize() + new Vector2(60, -8 * (Projectile.velocity.X > 0 ? 1 : -1)).RotatedBy(Projectile.rotation);

                for (int i = 0; i < 16; i++)
                {
                    Vector2 top = Projectile.Center;
                    Vector2 velocity = Projectile.velocity;
                    Vector2 sparkVelocity2 = velocity.normalize().RotateRandom(0.22f) * Main.rand.NextFloat(6f, 36f);
                    int sparkLifetime2 = Main.rand.Next(6, 8);
                    float sparkScale2 = Main.rand.NextFloat(0.6f, 1.4f);
                    var sparkColor2 = Color.Lerp(Color.Goldenrod, Color.Yellow, Main.rand.NextFloat(0, 1));

                    LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                trail = new TrailParticle() { maxLength = 40};
                EParticle.NewParticle(trail, Projectile.Center, Vector2.Zero, new Color(255, 120, 120), 0.6f, 1, true, BlendState.Additive);

            }
            trail.Lifetime = 13;
            trail.AddPoint(Projectile.Center + Projectile.velocity);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("pulseBlast", 0.8f, Projectile.Center, 6, 0.55f);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Firebrick, 0.1f, 1.7f, 8));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 4f, 1, true, BlendState.Additive, 0, 16);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 2.8f, 1, true, BlendState.Additive, 0, 16);
            if (Projectile.owner == Main.myPlayer)
            {
                CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage, 256, Projectile.DamageType);
            }
            for (int i = 0; i < 32; i++)
            {
                var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Firework_Yellow);
                d.scale = 0.8f;
                d.velocity = CEUtils.randomPointInCircle(14);
                d.position += d.velocity * 4;
            }
        }
    }
}

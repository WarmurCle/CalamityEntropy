using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.SpiritFountain;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureRailgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 82;
            Item.height = 32;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AzafureRailgunHeld>();
            Item.shootSpeed = 6;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SeaPrism>(8)
                .AddIngredient<HellIndustrialComponents>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }
    public class AzafureRailgunHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzafureRailgun";
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.HeldProjSetDefaults(DamageClass.Ranged);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public float Charge = 0;
        public LoopSound chargeSnd = null;
        public Vector2 FirePos => Projectile.Center + new Vector2(23, 4 * Math.Sign(Projectile.velocity.X)).RotatedBy(Projectile.rotation);
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if (!Main.dedServ)
            {
                if (chargeSnd == null)
                {
                    chargeSnd = new LoopSound(CalamityEntropy.ofCharge);
                    chargeSnd.instance.Pitch = 0;
                    chargeSnd.instance.Volume = 0;
                    chargeSnd.play();
                }
                chargeSnd.setVolume_Dist(Projectile.Center, 100, 700, Charge * 0.7f);
                chargeSnd.instance.Pitch = Charge * 0.15f;
                if (Charge < 1)
                {
                    chargeSnd.timeleft = 3;
                }
            }
            if (player.channel)
            {
                if (Charge < 1)
                {
                    Charge += 0.01f;
                    if (Charge >= 1f)
                    {
                        Charge = 1;
                        GeneralParticleHandler.SpawnParticle(new PulseRing(FirePos, player.velocity, Color.Firebrick, 0.1f, 0.6f, 10));
                    }
                }
                Projectile.timeLeft = 3;
                player.Calamity().mouseWorldListener = true;
                Projectile.rotation = (player.Calamity().mouseWorld - player.Center).ToRotation();
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 16;
                player.SetHandRot(((player.Calamity().mouseWorld - player.Center).ToRotation().ToRotationVector2() + new Vector2(0, 1f)).ToRotation());
                player.itemAnimation = player.itemTime = 4;
                player.heldProj = Projectile.whoAmI;
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Charge >= 1)
                    {
                        CEUtils.PlaySound("railgunShoot", 1, FirePos);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), FirePos, Projectile.velocity * 0.32f, ModContent.ProjectileType<RailgunChargeShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    else
                    {
                        CEUtils.PlaySound("shockBlast", 1.5f - 0.5f * Charge, FirePos, volume: Charge);
                        int bulletCounts = 1 + (int)(Charge * 9);
                        for(int i = 0; i < bulletCounts; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), FirePos, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.6f, 1) * 2.6f * (0.3f + 0.7f*Charge), ModContent.ProjectileType<RailgunSmallShot>(), (int)(Charge * Projectile.damage / bulletCounts), Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                player.velocity += Projectile.velocity * -0.4f * Charge;
                Projectile.Kill();
            }
            Projectile.light = Charge * 0.6f;
            Projectile.Center = player.GetDrawCenter() + new Vector2(0, -4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = Projectile.GetTexture();
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 16, null, lightColor, Projectile.rotation, t.Size() / 2f, Projectile.scale, (Projectile.velocity.X > 0) ? SpriteEffects.None : SpriteEffects.FlipVertically);
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Vector2 pos = FirePos;
            float size = Charge;
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(250, 250, 250), Projectile.rotation, tex.Size() * 0.5f, size * 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 80, 80), Projectile.rotation, tex.Size() * 0.5f, size * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
            return false;
        }
    }
    public class RailgunChargeShot : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.light = 0.4f;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 6;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 32;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[1]++ % 24 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.6f, ModContent.ProjectileType<RailgunSmallShot>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 0.6f, ModContent.ProjectileType<RailgunSmallShot>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }
            if (trail == null)
            {
                trail = new TrailParticle();
                trail.maxLength = 1200;
                trail.TimeLeftMax = 20;
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, Color.Red, 1.6f, 1, true, BlendState.Additive);
                trail2 = new TrailParticle();
                trail2.maxLength = 1200;
                trail2.TimeLeftMax = 20;
                EParticle.spawnNew(trail2, Projectile.Center, Vector2.Zero, Color.White, 0.7f, 1, true, BlendState.Additive);

            }
            trail.Lifetime = 20;
            trail2.Lifetime = 20;
            trail.AddPoint(Projectile.Center + Projectile.velocity);
            trail2.AddPoint(Projectile.Center + Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = 1 * Projectile.scale;
            DrawEnergyBall(Projectile.Center, scale, Projectile.Opacity);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("pulseBlast", 0.8f, Projectile.Center);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Firebrick, 0.1f, 0.6f, 8));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 1.6f, 1, true, BlendState.Additive, 0, 12);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 1f, 1, true, BlendState.Additive, 0, 12);
            CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage, 128, Projectile.DamageType);
        }

        public void DrawEnergyBall(Vector2 pos, float size, float alpha)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(250, 250, 250) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.4f), 1) * size * 0.18f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 80, 80) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.4f), 1) * size * 0.32f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
        }
        public TrailParticle trail = null;
        public TrailParticle trail2 = null;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Firebrick, 0.1f, 0.4f, 8));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 1.2f, 1, true, BlendState.Additive, 0, 12);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.7f, 1, true, BlendState.Additive, 0, 12);

            CEUtils.PlaySound("pulseBlast", 1f, Projectile.Center, 6, 0.7f);
        }
    }
    public class RailgunSmallShot : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public TrailParticle trail = null;
        public TrailParticle trail2 = null;

        public override void AI()
        {
            if (Projectile.damage < 1)
                Projectile.damage = 1;
            Projectile.velocity *= 0.98f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 160);
            if(target != null)
            {
                Projectile.velocity *= 0.92f;
                Projectile.velocity += (target.Center - Projectile.Center).normalize() * 2f;
            }
            if(trail == null)
            {
                trail = new TrailParticle();
                trail.maxLength = 32;
                trail.TimeLeftMax = 12;
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, Color.Firebrick, 0.8f, 1f, true, BlendState.Additive);
            }
            trail.Lifetime = 12;
            trail.AddPoint(Projectile.Center + Projectile.velocity);

            if (trail2 == null)
            {
                trail2 = new TrailParticle();
                trail2.maxLength = 32;
                trail2.TimeLeftMax = 12;
                EParticle.spawnNew(trail2, Projectile.Center, Vector2.Zero, Color.White * 0.8f, 0.4f, 1, true, BlendState.Additive);
            }
            trail2.Lifetime = 12;
            trail2.AddPoint(Projectile.Center + Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = 0.4f * Projectile.scale;
            DrawEnergyBall(Projectile.Center, scale, Projectile.Opacity);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 0.24f, 1, true, BlendState.Additive, 0, 12);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.06f, 1, true, BlendState.Additive, 0, 12);

        }

        public void DrawEnergyBall(Vector2 pos, float size, float alpha)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(250, 250, 250) * alpha, Projectile.rotation, tex.Size() * 0.5f, size * 0.18f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 80, 80) * alpha, Projectile.rotation, tex.Size() * 0.5f, size * 0.32f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
        }
    }
}

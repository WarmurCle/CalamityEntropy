using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.AzafureLightMachineGun
{
    public class AzafureLightMachineGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
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
            Item.shootSpeed = 6;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
        }

    }
    public class AzafureLightMachineGunHeld : ModProjectile
    {
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
            Player player = Projectile.GetOwner();
            if (player.channel)
            {
                Projectile.timeLeft = 3;
                player.Calamity().mouseWorldListener = true;
                Projectile.rotation = (player.Calamity().mouseWorld - player.Center).ToRotation();
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 16;
                player.SetHandRot(((player.Calamity().mouseWorld - player.Center).ToRotation().ToRotationVector2() + new Vector2(0, 1f)).ToRotation());
                player.itemAnimation = player.itemTime = 4;
                player.heldProj = Projectile.whoAmI;
                if (Projectile.ai[2] <= 0)
                {
                    Projectile.ai[2] = Utils.Remap(Projectile.ai[2], 0, 120, 26, 5);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.normalize() * 32, Projectile.velocity, ModContent.ProjectileType<ALMGLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.Center = player.GetDrawCenter();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = Projectile.GetTexture();
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition - Projectile.rotation.ToRotationVector2() * 10, CEUtils.GetCutTexRect(t, 2, (int)Main.GameUpdateCount / 4 % 2, false), lightColor, Projectile.rotation, t.Size() / new Vector2(2, 4), Projectile.scale, (Projectile.velocity.X > 0) ? SpriteEffects.None : SpriteEffects.FlipVertically);
            
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
        }
        public float dist = 0;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ == 0)
            {
                CEUtils.PlaySound("DudFire", 1.6f, Projectile.Center, 6, 0.4f);
                EParticle.NewParticle(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red, 0.3f, 1, true, BlendState.Additive, 0, 12);
                EParticle.NewParticle(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.12f, 1, true, BlendState.Additive, 0, 12);

                for (float d = 0; d < 1800; d += 4)
                {
                    dist = d;
                    CEUtils.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * dist, new Color(255, 120, 120), 0.5f);
                    if (!CEUtils.isAir(Projectile.Center + Projectile.rotation.ToRotationVector2() * d))
                    {
                        break;
                    }
                }
                Vector2 edp = Projectile.Center + Projectile.rotation.ToRotationVector2() * dist;
                EParticle.NewParticle(new ShineParticle(), edp, Vector2.Zero, Color.Red, 0.3f, 1, true, BlendState.Additive, 0, 12);
                EParticle.NewParticle(new ShineParticle(), edp, Vector2.Zero, Color.White, 0.12f, 1, true, BlendState.Additive, 0, 12);

            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = CEUtils.getExtraTex("MaskLaserLine");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255) * 0.4f, Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(dist / tex.Width, Projectile.scale), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 10), Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(dist / tex.Width, Projectile.scale * 0.4f), SpriteEffects.None, 0);

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
}

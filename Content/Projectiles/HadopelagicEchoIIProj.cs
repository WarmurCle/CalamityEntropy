using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Util;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class HadopelagicEchoIIProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 1;
            Projectile.height = 1; 
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        
        public override void AI(){
            Player owner = Projectile.owner.ToPlayer();
            if (owner.HeldItem.type == ModContent.ItemType<HadopelagicEchoII>())
            {
                Projectile.timeLeft = 3;
            }
            Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY - new Vector2(22, 0).RotatedBy(Projectile.rotation) + new Vector2(0, -12);
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 newVec = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.UnitX);
                if (!Projectile.velocity.Equals(newVec))
                {
                    Projectile.velocity = newVec;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if(Projectile.frameCounter % 4 == 0)
            {
                Projectile.frame++;
                if(Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.velocity.X >= 0)
            {
                owner.direction = 1;
                Projectile.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            }
            else
            {
                owner.direction = -1;
                Projectile.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            }
            Vector2 topPos = Projectile.Center + new Vector2(52, 0).RotatedBy(Projectile.rotation);
            if (active)
            {
                if(Main.GameUpdateCount % 60 == 0)
                {
                    gfxXAdd = 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 30, ProjectileID.EnchantedBeam, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(topPos + Projectile.velocity * 3, Vector2.Zero, new Color(170, 170, 255), new Vector2(2f, 2f), 0, 0.1f, 0.3f, 20);
                    GeneralParticleHandler.SpawnParticle(pulse);

                    CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(topPos + Projectile.velocity * 6, Vector2.Zero, new Color(140, 140, 255) * 0.6f, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.16f, 12);
                    GeneralParticleHandler.SpawnParticle(explosion2);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), topPos, Projectile.velocity, ModContent.ProjectileType<Impact>(), 0, 0, Projectile.owner, 0, 1.2f);
                    }
                    float sparkCount = 34;
                    for (int i = 0; i < sparkCount; i++)
                    {
                        Vector2 sparkVelocity2 = new Vector2(Main.rand.NextFloat(10, 20), 0).RotateRandom(1f).RotatedBy(Projectile.velocity.ToRotation());
                        int sparkLifetime2 = Main.rand.Next(26, 35);
                        float sparkScale2 = Main.rand.NextFloat(1.2f, 1.6f);
                        Color sparkColor2 = Color.Lerp(Color.SkyBlue, Color.LightSkyBlue, Main.rand.NextFloat(0, 1));
                        LineParticle spark = new LineParticle(topPos + Projectile.velocity * 3, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);

                    }
                }
            }
            gfxOffX += gfxXAdd;
            gfxOffX *= 0.87f;
            gfxXAdd *= 0.87f;
        }
        public float gfxOffX = 0;
        public float gfxXAdd = 0;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public bool active {  get { return Projectile.owner.ToPlayer().channel; } }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Util.Util.getExtraTex("HadopelagicEchoIIGlow");
            Rectangle frame = new Rectangle(active ? 126 : 0, Projectile.frame * 66, 126, 66);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(-gfxOffX + 40, 0).RotatedBy(Projectile.rotation), frame, lightColor, Projectile.rotation, (Projectile.velocity.X > 0 ? new Vector2(86, 42) : new Vector2(86, 66 - 42)), Projectile.scale, (Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically));
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(-gfxOffX + 40, 0).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, (Projectile.velocity.X > 0 ? new Vector2(86, 42) : new Vector2(86, 66 - 42)), Projectile.scale, (Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically));

            return false;
        }
    }

}
using System;
using System.Collections.Generic;
using CalamityEntropy.Util;
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
    public class GravityGazeHoldout : ModProjectile
    {
        public class SparkleParticle
        {
            public bool kill = false;
            public float rot;
            public float size;
            public float addSize = 1;
            public void update()
            {
                size += addSize;
                addSize -= 0.12f;
                if(size < 0)
                {
                    kill = true;
                }
            }

            public SparkleParticle()
            {
                rot = Util.Util.randomRot();
                size = 0;
            }
        }
        public List<SparkleParticle> particles = new List<SparkleParticle>();
        public override void SetStaticDefaults()
        {
        }
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
            if (Projectile.ai[0]++ > 0 && (Projectile.ai[0] % 40 == 6 || Projectile.ai[0] % 40 == 12 || Projectile.ai[0] % 40 == 18))
            {
                if (owner.CheckMana(6, true))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.06f) * 3.4f, ModContent.ProjectileType<GravityGazeBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    if (!Main.dedServ)
                    {
                        Util.Util.PlaySound("soulshine", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 8, 0.4f);
                    }
                    particles.Add(new SparkleParticle());
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (!owner.channel)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 3;
            }
            foreach(SparkleParticle particle in particles)
            {
                particle.update();
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].kill)
                {
                    particles.RemoveAt(i);
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 nv = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.One) * 16;
                if (nv != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = nv;
                
            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
                owner.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
                owner.direction = -1;
            }
            Player player = Projectile.owner.ToPlayer();
            Projectile.Center = owner.MountedCenter + player.gfxOffY * Vector2.UnitY + Projectile.velocity.SafeNormalize(Vector2.Zero) * 28;
            owner.itemRotation = Projectile.rotation * Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 30;
            owner.itemAnimation = 30;
            if (Projectile.velocity.X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public float counter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            lightColor = Color.White;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.ToRadians(90), texture.Size() / 2, Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Glow").Value;
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.Lerp(new Color(104, 127, 255), new Color(238, 244, 213), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)) * 0.7f, 0, light.Size() / 2, 0.5f * Projectile.scale * (1 + (float)Math.Cos((counter) * 0.02f) * 0.2f), SpriteEffects.None, 0);
            Texture2D spark = Util.Util.getExtraTex("Sparkle");
            foreach(SparkleParticle p in particles)
            {
                Main.spriteBatch.Draw(spark, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.LightBlue * 0.6f, p.rot, spark.Size() / 2, 0.06f * Projectile.scale * p.size, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(spark, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.LightGreen * 0.6f, p.rot + MathHelper.PiOver2, spark.Size() / 2, 0.06f * Projectile.scale * p.size, SpriteEffects.None, 0);

            }

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }

}
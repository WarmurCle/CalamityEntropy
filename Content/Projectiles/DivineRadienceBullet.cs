using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class DivineRadienceBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public List<Vector2> odp = new List<Vector2>();
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            odp.Add(Projectile.Center);
            if (odp.Count > 9)
            {
                odp.RemoveAt(0);
            }
            Projectile.rotation += 0.16f;
            NPC target = Projectile.FindTargetWithinRange(1600, false);
            if (target != null && drawcount > 8)
            {
                Projectile.velocity *= 0.94f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 2.4f;
            }
            drawcount++;
        }
        float drawcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, new Color(255, 60, 60), new Vector2(2f, 2f), 0, 0.1f, 0.5f, 20);
            GeneralParticleHandler.SpawnParticle(pulse);

            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, new Color(255, 60, 60), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.36f, 16);
            GeneralParticleHandler.SpawnParticle(explosion2);

            Util.Util.PlaySound("CrystalBallActive", 1, Projectile.Center, 4, 0.4f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(255, 60, 60);
            if (Projectile.timeLeft < 30)
            {
                lightColor *= ((float)Projectile.timeLeft / 30f);
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            float opc = 1;
            Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D t = tx;
            odp.Add(Projectile.Center);
            if (odp.Count > 2)
            {
                float size = 10;
                float sizej = size / odp.Count;
                Color cl = new Color(255, 90, 90);
                for (int i = odp.Count - 1; i >= 1; i--)
                {
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, odp[i], odp[i - 1], cl * (((float)(255 - Projectile.alpha)) / 255f), size * 0.7f);
                    size -= sizej;
                }
            }
            odp.RemoveAt(odp.Count - 1);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, lightColor * opc, MathHelper.ToRadians(drawcount * 4f), t.Size() / 2, new Vector2(1.3f, 1) * Projectile.scale * 0.6f, SpriteEffects.None, 0);
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, lightColor * opc, MathHelper.ToRadians((drawcount + 64) * 14f), t.Size() / 2, new Vector2(1.3f, 1) * Projectile.scale * 0.6f, SpriteEffects.None, 0);
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, lightColor * opc, MathHelper.ToRadians((drawcount + 154) * 34f), t.Size() / 2, new Vector2(1.3f, 1) * Projectile.scale * 0.6f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Glow").Value;
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition * Projectile.scale, null, lightColor, 0, light.Size() / 2, 0.6f * Projectile.scale, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }


}
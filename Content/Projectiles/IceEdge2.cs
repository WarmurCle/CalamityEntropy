using CalamityEntropy.Content.Items.Books;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class IceEdge2 : EBookBaseProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 160;
            Projectile.MaxUpdates = 3;
        }
        public override bool ShouldUpdatePosition()
        {
            return white <= 0;
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.ai[0] == 0)
            {
                Utilities.Util.PlaySound("bne_hit2", 1, Projectile.Center, 1, 0.36f);
                Projectile.rotation = Utilities.Util.randomRot();
            }
            if (op < 1)
            {
                op += 0.1f;
            }
            if (white > 0)
            {
                white -= 0.025f;
            }
            Projectile.ai[0]++;
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 9)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
            Projectile.rotation = Utilities.Util.rotatedToAngle(Projectile.rotation, Projectile.velocity.ToRotation(), 0.1f, false);

        }
        public float op = 0;
        public float white = 1;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 400);
            target.AddBuff(BuffID.Frostburn, 400);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center + Projectile.velocity * 3, Vector2.Zero, new Color(170, 170, 255), new Vector2(2f, 2f), 0, 0.1f, 0.5f, 20);
            GeneralParticleHandler.SpawnParticle(pulse);

            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center + Projectile.velocity * 6, Vector2.Zero, new Color(140, 140, 255), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.36f, 16);
            GeneralParticleHandler.SpawnParticle(explosion2);

            float sparkCount = 14;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(Main.rand.NextFloat(10, 20), 0).RotateRandom(1f).RotatedBy(Projectile.velocity.ToRotation());
                int sparkLifetime2 = Main.rand.Next(26, 35);
                float sparkScale2 = Main.rand.NextFloat(1.2f, 1.6f);
                Color sparkColor2 = Color.Lerp(Color.SkyBlue, Color.LightSkyBlue, Main.rand.NextFloat(0, 1));
                LineParticle spark = new LineParticle(Projectile.Center + Projectile.velocity * 3, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/IceEdge").Value;
            Texture2D tx2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/IceEdge2").Value;
            float x = 0f;
            for (int i = 0; i < odp.Count; i++)
            {
                Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, Color.White * x * 0.3f, odr[i], new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);
                x += 1 / 10f;
            }
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * op, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tx2, Projectile.Center - Main.screenPosition, null, Color.White * op * white, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

    }

}
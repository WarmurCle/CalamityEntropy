using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Util;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
	public class AnnihilateArrowProjectile : ModProjectile
	{

		public override void SetDefaults() {
			Projectile.width = 10; 
			Projectile.height = 10; 
			Projectile.MaxUpdates = 5;
			Projectile.arrow = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200;
			Projectile.light = 0.2f;
			Projectile.tileCollide = false;
		}

		public override void AI() {
			if (homing && Projectile.ai[0]++ > 10)
			{
				NPC target = Projectile.FindTargetWithinRange(400);
				if (target != null)
				{
					float rot = Util.Util.randomRot(); 
					int pjex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 270 * Projectile.WhipSettings.RangeMultiplier, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 0, Projectile.owner, 0, -0.8f);
                    pjex.ToProj().hostile = false;
					pjex.ToProj().MaxUpdates *= 2;
                    for (int i = 0; i < 4; i++)
					{
						float a = rot + MathHelper.ToRadians(i * 90);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, a.ToRotationVector2() * Projectile.velocity.Length() * 0.36f, ModContent.ProjectileType<AnnihilateArrowSplit>(), Projectile.damage / 3, Projectile.knockBack / 4, Projectile.owner);
					}
					Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((target.Center - Projectile.Center).ToRotation());
					homing = false;
					for (int i = 0; i < 16; i++)
					{
                        Vector2 top = Projectile.Center;
                        Vector2 sparkVelocity2 = Projectile.velocity.RotateRandom(0.26f) * -1 * Main.rand.NextFloat(1, 5);
                        int sparkLifetime2 = Main.rand.Next(18, 22);
                        float sparkScale2 = Main.rand.NextFloat(1f, 1.8f);
                        Color sparkColor2 = Color.Lerp(Color.Blue, Color.DeepSkyBlue, Main.rand.NextFloat(0, 1));
                        LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
					for(int i = 0; i < 42; i++)
                    {
                        PixelParticle p = new PixelParticle(Projectile.Center, Projectile.Center + Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(100, 256), Projectile.Center, Main.rand.Next(20, 42), Color.White, new Color(180, 180, 255));
                        PixelParticle.particles.Add(p);
                    }
                }
            }
			{
                Vector2 top = Projectile.Center - Projectile.velocity;
                Vector2 sparkVelocity2 = Projectile.velocity * -1.2f;
                int sparkLifetime2 = Main.rand.Next(8, 12);
                float sparkScale2 = Main.rand.NextFloat(0.6f, 1.2f);
                Color sparkColor2 = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, Main.rand.NextFloat(0, 1));
                LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
		public bool homing = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Util.Util.PlaySound("bne_hit", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, volume: 0.4f);
            EParticle.spawnNew(new AbyssalLine() { lx = 1.9f, xadd = 1.9f }, target.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Util.Util.randomRot());

			if (homing)
			{
                float rot = Util.Util.randomRot();
                for (int i = 0; i < 3; i++)
                {
                    float a = rot + MathHelper.ToRadians(i * 120);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, a.ToRotationVector2() * Projectile.velocity.Length() * 0.5f, ModContent.ProjectileType<AnnihilateArrowSplit>(), Projectile.damage / 4, Projectile.knockBack / 4, Projectile.owner);
                }
                int pjex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 270 * Projectile.WhipSettings.RangeMultiplier, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 0, Projectile.owner, 0, -0.8f);
				pjex.ToProj().hostile = false;
                pjex.ToProj().MaxUpdates *= 2;
            }
		}
        public override void OnKill(int timeLeft) {
			Util.Util.PlaySound("bne_hit2", Main.rand.NextFloat(0.7f, 1.3f), Projectile.Center);
		}
	}
}
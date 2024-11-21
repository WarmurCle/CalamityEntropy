using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Projectiles
{
	public class GodSlayerRocketProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true; // Deals double damage on direct hits.
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.
			ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
			// This set handles some things for us already:
			// Sets the timeLeft to 3 and the projectile direction when colliding with an NPC or player in PVP (so the explosive can detonate).
			// Explosives also bounce off the top of Shimmer, detonate with no blast damage when touching the bottom or sides of Shimmer, and damage other players in For the Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;

			// This set makes it so the rocket doesn't deal damage to players. Only used for vanilla rockets.
			// Simply remove the Projectile.HurtPlayer() part to stop the projectile from damaging its user.
			// ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
		}
		public override void SetDefaults() {
			Projectile.width = 36;
			Projectile.extraUpdates = 8;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.penetrate = -1; // Infinite penetration so that the blast can hit all enemies within its radius.
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = false;
			// Rockets use explosive AI, ProjAIStyleID.Explosive (16). You could use that instead here with the correct AIType.
			// But, using our own AI allows us to customize things like the dusts that the rocket creates.
			// Projectile.aiStyle = ProjAIStyleID.Explosive;
			// AIType = ProjectileID.RocketI;
		}
		public List<Vector2> odp = new List<Vector2>();
		public List<float> odr = new List<float>();
		public override void AI() {
			
			// If timeLeft is <= 3, then explode the rocket.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				Projectile.PrepareBombToBlow();
			}
			else {
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 226 : 272, -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.8f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.15f, 0.35f);
                }
                CalamityUtils.HomeInOnNPC(Projectile, true, 500f, 60, 200f);
                

                    
                odp.Add(Projectile.Center);
				odr.Add(Projectile.rotation);
				if (odp.Count > 12)
				{
					odp.RemoveAt(0);
					odr.RemoveAt(0);
				}
				// Increase the speed of the rocket if it is moving less than 1 block per second.
				// It is not recommended to increase the number past 16f to increase the speed of the rocket. It could start no clipping through blocks.
				// Instead, increase extraUpdates in SetDefaults() to make the rocket move faster.
				if (Projectile.velocity.Length() < 8) {
					Projectile.velocity *= 1.1f;
				}
				else
				{
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
				}
			}

			// Rotate the rocket in the direction that it is moving.
			if (Projectile.velocity != Vector2.Zero) {
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
			}
		}

		// When the rocket hits a tile, NPC, or player, get ready to explode.

        public override void OnHitNPC(NPC t, NPC.HitInfo hit, int damageDone)
        {
			EGlobalNPC.AddVoidTouch(t, 60, 3);
			PrepareBombToBlow();
			Projectile.timeLeft = 3;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion will be in the wrong place if the rocket explodes on slopes.
			Projectile.alpha = 255; // Make the rocket invisible.
            SoundStyle s = new SoundStyle("CalamityEntropy/Sounds/DevourerDeathImpact");
            s.Volume = 0.36f;
            SoundEngine.PlaySound(s, Projectile.position);
			// Resize the hitbox of the projectile for the blast "radius".
			// Rocket I: 128, Rocket III: 200, Mini Nuke Rocket: 250
			// Measurements are in pixels, so 128 / 16 = 8 tiles.
			Projectile.Resize(320, 320);
			Projectile.hostile = false;
			// Set the knockback of the blast.
			// Rocket I: 8f, Rocket III: 10f, Mini Nuke Rocket: 12f
			Projectile.knockBack = 8f;
		}

		public override void OnKill(int timeLeft) {
            // Vanilla code takes care ensuring that in For the Worthy or Get Fixed Boi worlds the blast can damage other players because
            // this projectile is ProjectileID.Sets.Explosive[Type] = true;. It also takes care of hurting the owner. The Projectile.PrepareBombToBlow
            // and Projectile.HurtPlayer methods can be used directly if needed for a projectile not using ProjectileID.Sets.Explosive

            // Play an exploding sound.

            

			// Resize the projectile again so the explosion dust and gore spawn from the middle.
			// Rocket I: 22, Rocket III: 80, Mini Nuke Rocket: 50
			Projectile.Resize(22, 22);

            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Aqua, new Vector2(2f, 2f), 0, 0.1f, 0.85f * 1.4f, (int)(36 * 1.1f));
            GeneralParticleHandler.SpawnParticle(pulse);
            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Magenta, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.65f * 1.4f, (int)(26 * 1.1f));
            GeneralParticleHandler.SpawnParticle(explosion2);
            float sparkCount = Projectile.Calamity().stealthStrike ? 26 : 16;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(16, 0).RotatedBy((float)Main.rand.NextDouble() * 3.14159f * 2) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(20, 24);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Color.DarkBlue;

                float velc = 1.2f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f), sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f), sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Purple : Color.Purple);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
			Util.Util.DrawAfterimage(TextureAssets.Projectile[Projectile.type].Value, odp, odr);
			return true;
        }
    }
}
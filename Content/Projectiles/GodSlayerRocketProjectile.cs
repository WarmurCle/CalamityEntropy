using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class GodSlayerRocketProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true; ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.extraUpdates = 8;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1; Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
        }
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void AI()
        {

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.PrepareBombToBlow();
            }
            else
            {
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
                if (Projectile.velocity.Length() < 8)
                {
                    Projectile.velocity *= 1.1f;
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
                }
            }

            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            }
        }


        public override void OnHitNPC(NPC t, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(t, 60, 4.5f, 800, 24);
            PrepareBombToBlow();
            Projectile.timeLeft = 3;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false; Projectile.alpha = 255; SoundStyle s = new SoundStyle("CalamityEntropy/Assets/Sounds/DevourerDeathImpact");
            s.Volume = 0.36f;
            SoundEngine.PlaySound(s, Projectile.position);
            Projectile.Resize(320, 320);
            Projectile.hostile = false;
            Projectile.knockBack = 8f;
        }

        public override void OnKill(int timeLeft)
        {




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
            if (CalamityEntropy.AprilFool)
            {
                EParticle.spawnNew(new EXPLOSIONCOSMIC(), Projectile.Center + new Vector2(0, -38), Vector2.Zero, Color.White, 2, 1, true, BlendState.NonPremultiplied, 0);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Utilities.Util.DrawAfterimage(TextureAssets.Projectile[Projectile.type].Value, odp, odr);
            return true;
        }
    }
}
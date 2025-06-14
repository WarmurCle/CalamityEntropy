using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Chainsaw
{
    public class Euangelion0 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.6f;
            Projectile.scale = 1.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.ArmorPenetration = 75;
        }
        int frame = 2;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale += Projectile.owner.ToPlayer().Entropy().WeaponBoost * 0.8f;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] < 5 ? false : null;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 8 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
            }

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                HandleChannelMovement(player, playerRotatedPoint);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.localAI[0]++ > 3 ? 0 : Projectile.getOwner().direction * -MathHelper.ToRadians(4 - Projectile.localAI[0]) * 48);
            if (Projectile.localAI[0] == 4)
            {
                CEUtils.PlaySound("chainsawHit", 1, Projectile.Center, volume: 0.4f);
            }
            Projectile.Center = player.Center + player.gfxOffY * Vector2.UnitY + Projectile.rotation.ToRotationVector2() * 46 * Projectile.scale;
            if (Projectile.Entropy().OnProj != -1)
            {
                Projectile.Center = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * 46 * Projectile.scale;
            }
            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;
            }
            else
            {
                player.direction = -1;
            }
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (!player.channel)
            {
                Projectile.timeLeft = 1;
            }
            soundCd--;
            spawnCd--;
        }
        public int soundCd = 0;
        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            int bsize = ((int)(122 * Projectile.scale));
            Vector2 c = player.Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
            if (Projectile.Entropy().OnProj != -1)
            {
                c = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
            }
            return new Rectangle((int)c.X - bsize / 2, (int)c.Y - bsize / 2, bsize, bsize).Intersects(targetHitbox);

        }
        public int spawnCd = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (spawnCd <= 0)
            {
                spawnCd = 12;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 2, Projectile.velocity.RotatedByRandom(1.6f) * 20, ModContent.ProjectileType<LunarLightball>(), Projectile.damage / 6, Projectile.knockBack, Projectile.owner).ToProj().DamageType = DamageClass.Melee;
            }
            var rand = Main.rand;
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(target.Center, 16, 16, DustID.Firework_Yellow, rand.Next(-3, 4), rand.Next(-3, 4));
            }
            Player Owner = Main.player[Projectile.owner];
            SoundStyle hitSound = new SoundStyle("CalamityEntropy/Assets/Sounds/chainsaw", SoundType.Ambient) { Volume = 0.3f };
            if (soundCd <= 0)
            {
                SoundEngine.PlaySound(hitSound, Projectile.Center);
                soundCd = 16;
            }
            if (Projectile.owner == Main.myPlayer && ModContent.GetInstance<Config>().ChainsawShakeScreen)
            {
                CalamityEntropy.Instance.screenShakeAmp = 1;
            }
            float sparkCount = 4;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(16, 0).RotatedByRandom(3.14159f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(7, 10);
                float sparkScale2 = Main.rand.NextFloat(1.2f, 2f);
                Color sparkColor2 = new Color(125, 150, 135);

                float velc = 1.6f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? new Color(164, 236, 200) : new Color(106, 212, 166));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }

        public override bool PreDraw(ref Color dc)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Chainsaw/Euangelion" + (((int)(Projectile.ai[0] / 4)) % frame).ToString()).Value;
            var rand = Main.rand;
            SpriteEffects ef = SpriteEffects.None;
            if (Projectile.velocity.X < 0)
            {
                ef = SpriteEffects.FlipVertically;
            }
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition + new Vector2(rand.Next(-2, 3), rand.Next(-2, 3)), null, dc, Projectile.rotation, tx.Size() / 2, new Vector2(Projectile.scale, Projectile.scale), ef, 0);
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }


}
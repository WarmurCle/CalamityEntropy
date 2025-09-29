using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.VoidBlade
{
    public class VoidBladeProj : ModProjectile
    {
        SoundStyle hitSound = new("CalamityEntropy/Assets/Sounds/vb_hit");
        SoundStyle hs = new("CalamityEntropy/Assets/Sounds/vbuse");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 6000;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.scale = 1.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public int damageo = -1;
        public override void AI()
        {
            hs.Volume = 0.6f * CEUtils.WeapSound;
            hitSound.Volume = 0.2f * CEUtils.WeapSound;
            if (damageo == -1)
            {
                damageo = Projectile.damage;
            }
            soundCd--;
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 3 == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 12)
                {
                    Projectile.ai[1] = 0;
                }
            }
            if (player.channel)
            {
                Projectile.timeLeft = 20;
            }
            else
            {
                if (Projectile.ai[1] == 4 || Projectile.ai[1] == 12)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[1] == 0)
            {
                hitSound.Pitch = 1f;
                hs.Pitch = 1.3f;
            }
            if (Projectile.ai[1] == 6)
            {
                hitSound.Pitch = 0.9f;
                hs.Pitch = 1f;
            }
            if (Projectile.ai[1] > 6)
            {
                Projectile.damage = (int)(damageo * 1.5f);
            }
            else
            {
                Projectile.damage = damageo;
            }
            if (Projectile.ai[1] == 7 || Projectile.ai[1] == 1)
            {
                if (Projectile.ai[0] % 3 == 0)
                {
                    SoundEngine.PlaySound(hs, Projectile.Center);
                }
            }
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] % 3 == 0)
                {
                    if (Projectile.ai[1] == 6 || Projectile.ai[1] == 0)
                        HandleChannelMovement(player, playerRotatedPoint);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + Projectile.rotation.ToRotationVector2() * 64 * Projectile.scale;
            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;
            }
            else
            {
                player.direction = 0;
            }
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 1f);
        }
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
            if (Projectile.ai[0] % 3 == 0)
            {
                Player player = Main.player[Projectile.owner];
                if (Projectile.ai[1] == 1)
                {
                    int bsize = ((int)(230 * Projectile.scale));
                    Vector2 c = player.Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
                    if (Projectile.Entropy().OnProj != -1)
                    {
                        c = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
                    }
                    return new Rectangle((int)c.X - bsize / 2, (int)c.Y - bsize / 2, bsize, bsize).Intersects(targetHitbox);
                }
                if (Projectile.ai[1] == 7)
                {
                    int bsize = ((int)(280 * Projectile.scale));
                    Vector2 c = player.Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
                    if (Projectile.Entropy().OnProj != -1)
                    {
                        c = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
                    }
                    return new Rectangle((int)c.X - bsize / 2, (int)c.Y - bsize / 2, bsize, bsize).Intersects(targetHitbox);
                }
            }
            return false;
        }
        int soundCd = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (soundCd <= 0)
            {
                soundCd = 5;
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/da3") { MaxInstances = 1, Volume = 0.4f * CEUtils.WeapSound }, target.Center);
            }
            target.immune[Projectile.owner] = 3;
            if (Projectile.owner == Main.myPlayer)
            {
                int pj = Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<VoidBladeHit>(), Projectile.damage, 0, Projectile.owner, 0, (target.Center - Main.player[Projectile.owner].Center).ToRotation());

            }
            EGlobalNPC.AddVoidTouch(target, 60, 0.5f + 0.3f * Projectile.owner.ToPlayer().Entropy().WeaponBoost, 460, 1);
            target.Entropy().vtnoparticle = target.Entropy().VoidTouchTime;
            float sparkCount = 3;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = (Projectile.rotation.ToRotationVector2() * 12).RotatedByRandom(1.6f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(23, 35);
                float sparkScale2 = Main.rand.NextFloat(1.4f, 2.6f);
                Color sparkColor2 = Color.Purple;
                if (Projectile.ai[1] > 4)
                {
                    sparkColor2 = Color.LightBlue;
                }
                float velc = 2f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? sparkColor2 : Color.Blue);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }

        public override bool PreDraw(ref Color dc)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidBlade/f" + ((int)Projectile.ai[1]).ToString()).Value;
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(200, 200), new Vector2(Projectile.scale, Projectile.scale), SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }


}
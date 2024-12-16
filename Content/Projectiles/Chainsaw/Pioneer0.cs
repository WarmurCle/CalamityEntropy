using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Particle = CalamityEntropy.Content.Particles.Particle;

namespace CalamityEntropy.Content.Projectiles.Chainsaw
{
    public class Pioneer0 : ModProjectile
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
            Projectile.light = 0f;
            Projectile.scale = 1.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 1024;
        }
        int frame = 2;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale += Projectile.owner.ToPlayer().Entropy().WeaponBoost * 0.8f;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + Projectile.rotation.ToRotationVector2() * 64 * Projectile.scale;
            if (Projectile.Entropy().OnProj != -1)
            {
                Projectile.Center = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * 64 * Projectile.scale;
            }
            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;
            }
            else { 
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
            foreach (NPC n in Main.npc)
            {
                if (!n.friendly)
                {
                    if (Colliding(Projectile.Hitbox, n.Hitbox) != null && (bool)(Colliding(Projectile.Hitbox, n.Hitbox)) && n.active)
                    {
                        SoundEngine.PlaySound(n.HitSound, n.Center);
                        OnHitNPC(n, new NPC.HitInfo(), 60);
                        for (int i = 0; i <= 30; i++)
                        {
                            if (n.life > 50) {
                                int td = (int)(50 * (1f - n.Calamity().DR)) - n.defense;
                                if (td < 10)
                                {
                                    td = 10;
                                }
                                if (n.realLife >= 0)
                                {
                                    n.realLife.ToNPC().life -= td;
                                }
                                else
                                {
                                    n.life -= td;
                                }
                                player.dpsDamage += td;
                            }
                            else
                            {
                                if (n.life > 1) { n.life = 1; }
                                player.ApplyDamageToNPC(n, 500, 0, 0, false, DamageClass.Melee, false);
                            }
                            if (i < 6)
                            {
                                Particle pt = new Particle();
                                pt.position = n.Center;
                                var rand = Main.rand;
                                pt.velocity = new Vector2(rand.Next(-30, 31) * 0.06f, rand.Next(-30, 31) * 0.06f);
                                pt.alpha = 0.4f;
                                VoidParticles.particles.Add(pt);
                            }
                        }
                    }
                }
            }
            soundCd--;
        }

        public int soundCd = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            SoundStyle hitSound = new SoundStyle("CalamityEntropy/Assets/Sounds/chainsaw", SoundType.Ambient);
            if (soundCd <= 0)
            {
                SoundEngine.PlaySound(hitSound, Projectile.Center);
                soundCd = 16;
            }
            if (Projectile.owner == Main.myPlayer && ModContent.GetInstance<Config>().ChainsawShakeScreen)
            {
                CalamityEntropy.Instance.screenShakeAmp = 1;
            }
            float sparkCount = 6;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(16, 0).RotatedByRandom(3.14159f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(23, 35);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Color.DarkBlue;

                float velc = 2.6f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.LightBlue : Color.Purple);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
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
            Player player = Main.player[Projectile.owner];
            int bsize = ((int)(170 * Projectile.scale));
            Vector2 c = player.Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
            if (Projectile.Entropy().OnProj != -1)
            {
                c = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * bsize / 2;
            }
            return new Rectangle((int)c.X - bsize / 2, (int)c.Y - bsize / 2, bsize, bsize).Intersects(targetHitbox);

        }
        public override bool PreDraw(ref Color dc){
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Chainsaw/Pioneer" + (((int)(Projectile.ai[0] / 4)) % frame).ToString()).Value;
            var rand = Main.rand;
            SpriteEffects ef = SpriteEffects.None;
            if (Projectile.velocity.X < 0)
            {
                ef = SpriteEffects.FlipVertically;
            }
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition + new Vector2(rand.Next(-2, 3), rand.Next(-2, 3)), null, Color.White, Projectile.rotation, tx.Size() / 2, new Vector2(Projectile.scale, Projectile.scale), ef, 0);
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
    

}
using CalamityEntropy.Content.Items.Books;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class BrimstoneHellblastFriendly : EBookBaseProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            base.AI();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            int target = Player.FindClosest(Projectile.Center, 1, 1);


            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            float sine = (float)Math.Sin(Projectile.timeLeft * 0.575f / MathHelper.Pi);

            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 16f;

            SparkParticle orb = new(Projectile.Center + offset, -Projectile.velocity * 0.05f, false, 8, 0.8f, Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
            GeneralParticleHandler.SpawnParticle(orb);

            SparkParticle orb2 = new(Projectile.Center - offset, -Projectile.velocity * 0.05f, false, 8, 0.8f, Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
            GeneralParticleHandler.SpawnParticle(orb2);


            if (Projectile.timeLeft < 21)
                Projectile.Opacity -= 0.05f;

            if (Projectile.ai[2] == 0f)
            {
                Projectile.ai[2] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }


            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 6 * 60);
        }

    }
    public class BrimstoneGigaBlastFriendly : EBookBaseProjectile
    {
        public static readonly SoundStyle ImpactSound = new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneGigablastImpact");

        public bool withinRange;

        public bool setLifetime;


        public override void SetStaticDefaults()
        {
            Main.projFrames[base.Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            base.Projectile.width = 50;
            base.Projectile.height = 50;
            base.Projectile.ignoreWater = true;
            base.Projectile.penetrate = -1;
            base.Projectile.timeLeft = 120;
            base.Projectile.Opacity = 0f;
            base.Projectile.tileCollide = false;
        }
        public bool flag = true;
        public override void AI()
        {
            base.AI();
            if (flag)
            {
                Projectile.ai[1] = 2;
                Projectile.velocity *= 0.3f;
                flag = false;
            }
            base.Projectile.frameCounter++;
            if (base.Projectile.frameCounter > 4)
            {
                base.Projectile.frame++;
                base.Projectile.frameCounter = 0;
            }

            if (base.Projectile.frame > 5)
            {
                base.Projectile.frame = 0;
            }

            if (!withinRange)
            {
                if (base.Projectile.ai[2] == 1f)
                {
                    base.Projectile.Opacity = MathHelper.Clamp((float)base.Projectile.timeLeft / 60f, 0f, 1f);
                }
                else
                {
                    base.Projectile.Opacity = MathHelper.Clamp(1f - (float)(base.Projectile.timeLeft - 130) / 20f, 0f, 1f);
                }
            }

            Lighting.AddLight(base.Projectile.Center, 0.9f * base.Projectile.Opacity, 0f, 0f);
            base.Projectile.rotation = base.Projectile.velocity.ToRotation() + MathF.PI / 2f;
            if (base.Projectile.localAI[0] == 0f)
            {
                base.Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(in SoundID.Item20, base.Projectile.Center);
            }
            var t = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 3600);
            int num = -1;
            if (t != null) num = t.whoAmI;
            if (!withinRange && num != -1)
            {
                float num2 = base.Projectile.velocity.Length();
                Vector2 vector = Main.npc[num].Center - base.Projectile.Center;
                vector.Normalize();
                vector *= num2;
                base.Projectile.velocity = (base.Projectile.velocity * 24f + vector) / 25f;
                base.Projectile.velocity.Normalize();
                base.Projectile.velocity *= num2;
            }

            float num3 = ((num == -1 || !Main.npc[num].active || !Main.player[num].active || Main.npc[num] == null) ? 1000f : Vector2.Distance(Main.npc[num].Center, base.Projectile.Center));
            if (base.Projectile.ai[1] == 2f && Main.rand.NextBool())
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(base.Projectile.Center - base.Projectile.velocity + Main.rand.NextVector2Circular(30f, 30f), -base.Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), affectedByGravity: false, 14, Main.rand.NextFloat(0.5f, 0.75f), (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * base.Projectile.Opacity));
            }

            if ((base.Projectile.timeLeft == 1 && !withinRange) || (num3 < 224f && base.Projectile.Opacity == 1f))
            {
                if (!setLifetime)
                {
                    base.Projectile.timeLeft = 60;
                    setLifetime = true;
                }

                withinRange = true;
            }

            if (!withinRange || base.Projectile.ai[2] != 0f)
            {
                return;
            }

            base.Projectile.velocity *= 0.9f;
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(base.Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                dust.noGravity = true;
                dust.velocity = new Vector2(4f, 4f).RotatedByRandom(100.0) * Main.rand.NextFloat(0.5f, 1.3f);
                dust.scale = Main.rand.NextFloat(0.7f, 1.8f);
            }

            if (base.Projectile.timeLeft <= 40 && base.Projectile.Opacity > 0f)
            {
                base.Projectile.Opacity -= 0.05f;
            }

            if (base.Projectile.timeLeft == 30)
            {
                base.Projectile.Opacity = 0f;
                base.Projectile.velocity *= 0f;
                for (int j = 0; j < 2; j++)
                {
                    Particle particle = new BloomParticle(base.Projectile.Center, Vector2.Zero, new Color(121, 21, 77), 0.1f, 0.85f, 30, fade: false);
                    GeneralParticleHandler.SpawnParticle(particle);
                    if (base.Projectile.ai[2] == 1f)
                    {
                        particle.Lifetime = 0;
                    }
                }
            }

            if (base.Projectile.timeLeft == 15)
            {
                GeneralParticleHandler.SpawnParticle(new BloomParticle(base.Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.8f, 15, fade: false));
            }

            if (base.Projectile.timeLeft == 8)
            {
                GeneralParticleHandler.SpawnParticle(new BloomParticle(base.Projectile.Center, Vector2.Zero, Color.White, 0.1f, 0.7f, 8, fade: false));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = TextureAssets.Projectile[base.Projectile.type].Value;
            int num = value.Height / Main.projFrames[base.Projectile.type];
            int y = num * base.Projectile.frame;
            lightColor.R = (byte)(255f * base.Projectile.Opacity);

            Main.EntitySpriteDraw(value, base.Projectile.Center - Main.screenPosition + new Vector2(0f, base.Projectile.gfxOffY), new Rectangle(0, y, value.Width, num), base.Projectile.GetAlpha(lightColor), base.Projectile.rotation, new Vector2((float)value.Width / 2f, (float)num / 2f), base.Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (!(Projectile.Opacity == 1f))
                return false;
            return null;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(in ImpactSound, base.Projectile.Center);
            if (base.Projectile.ai[2] != 0f)
            {
                return;
            }

            if (base.Projectile.owner == Main.myPlayer)
            {
                int num = 30;
                float num2 = MathF.PI * 2f / (float)num;
                int type = ModContent.ProjectileType<BrimstoneShootSmall>();
                float num3 = 6.5f;
                Vector2 spinningpoint = new Vector2(0f, 0f - num3);
                for (int i = 0; i < num; i++)
                {
                    Vector2 velocity = spinningpoint.RotatedBy(num2 * (float)i);
                    if (this.ShooterModProjectile is EntropyBookHeldProjectile ebook)
                    {
                        ebook.ShootSingleProjectile(type, Projectile.Center, velocity, 0.4f, initAction: (projectile) => { projectile.ai[0] = 0f; projectile.ai[1] = Projectile.ai[1]; projectile.ai[2] = num3 * 1.5f; }, shotSpeedMul: 0.7f);
                    }
                }
            }

            if (base.Projectile.ai[1] == 2f)
            {
                for (int j = 0; j < 25; j++)
                {
                    Vector2 vector = new Vector2(15f, 15f).RotatedByRandom(100.0);
                    GeneralParticleHandler.SpawnParticle(new PointParticle(base.Projectile.Center + vector, vector * Main.rand.NextFloat(0.3f, 1f), affectedByGravity: false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f));
                }

                for (int k = 0; k < 25; k++)
                {
                    Dust dust = Dust.NewDustPerfect(base.Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                    dust.noGravity = true;
                    dust.velocity = new Vector2(20f, 20f).RotatedByRandom(100.0) * Main.rand.NextFloat(0.5f, 1.3f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                }
            }
        }
    }
    public class BrimstoneShootSmall : EBookBaseProjectile
    {
        public int time;
        public bool flag = true;

        public override void SetStaticDefaults()
        {
            Main.projFrames[base.Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            base.Projectile.width = 18;
            base.Projectile.height = 44;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
            base.Projectile.penetrate = -1;
            base.Projectile.timeLeft = 690;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(base.Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.Projectile.localAI[0] = reader.ReadSingle();
        }
        public float ctr = 0;
        public override void ApplyHoming()
        {
            if (ctr > 8)
            {
                base.ApplyHoming();
            }
        }
        public override void AI()
        {
            ctr++;
            if (flag)
            {
                flag = false;
                this.homing += 0.6f;
                this.homingRange *= 3;
            }
            base.AI();
            bool bossRushActive = BossRushEvent.BossRushActive;
            int num = Player.FindClosest(base.Projectile.Center, 1, 1);
            float num2 = ((num == -1 || Main.player[num].dead || !Main.player[num].active || Main.player[num] == null) ? 1000f : Vector2.Distance(Main.player[num].Center, base.Projectile.Center));
            if (base.Projectile.velocity.Length() < base.Projectile.ai[2])
            {
                base.Projectile.velocity *= (bossRushActive ? 1.0125f : 1.01f);
                if (base.Projectile.velocity.Length() > base.Projectile.ai[2])
                {
                    base.Projectile.velocity.Normalize();
                    base.Projectile.velocity *= base.Projectile.ai[2];
                }
            }

            base.Projectile.rotation = base.Projectile.velocity.ToRotation() + MathF.PI / 2f;
            base.Projectile.frameCounter++;
            if (base.Projectile.frameCounter > 4)
            {
                base.Projectile.frame++;
                base.Projectile.frameCounter = 0;
            }

            if (base.Projectile.frame > 3)
            {
                base.Projectile.frame = 0;
            }

            if (base.Projectile.timeLeft < 60)
            {
                base.Projectile.Opacity = MathHelper.Clamp((float)base.Projectile.timeLeft / 60f, 0f, 1f);
            }

            if (base.Projectile.ai[0] == 2f && base.Projectile.timeLeft > 570)
            {
                int num3 = Player.FindClosest(base.Projectile.Center, 1, 1);
                Vector2 vector = Main.player[num3].Center - base.Projectile.Center;
                float num4 = base.Projectile.velocity.Length();
                vector.Normalize();
                vector *= num4;
                base.Projectile.velocity = (base.Projectile.velocity * 15f + vector) / 16f;
                base.Projectile.velocity.Normalize();
                base.Projectile.velocity *= num4;
            }

            if ((base.Projectile.ai[1] == 2f || (base.Projectile.ai[1] == 4f && time > 10)) && num2 < 1400f)
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(base.Projectile.Center - base.Projectile.velocity * 0.5f, -base.Projectile.velocity * Main.rand.NextFloat(0.1f, 0.6f), affectedByGravity: false, (int)MathHelper.Clamp(9f * Utils.GetLerpValue(630f, 690f, base.Projectile.timeLeft), 2f, 9f), 1.1f, (Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f)) * base.Projectile.Opacity * 0.85f));
            }

            if (base.Projectile.ai[1] == 3f)
            {
                if (base.Projectile.timeLeft > 600)
                {
                    base.Projectile.velocity *= 1.015f;
                }

                base.Projectile.scale = 0.85f;
                Dust dust = Dust.NewDustPerfect(base.Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), 182);
                dust.noGravity = true;
                dust.velocity = -base.Projectile.velocity * 0.5f * Main.rand.NextFloat(0.1f, 0.9f);
                dust.scale = Main.rand.NextFloat(0.2f, 0.6f);
            }

            if (base.Projectile.localAI[0] == 0f)
            {
                base.Projectile.localAI[0] = 1f;
                if (base.Projectile.ai[0] == 0f)
                {
                    base.Projectile.damage = (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) ? base.Projectile.GetProjectileDamage(ModContent.NPCType<SupremeCalamitas>()) : base.Projectile.GetProjectileDamage(ModContent.NPCType<CalamitasClone>()));
                }
            }

            Lighting.AddLight(base.Projectile.Center, 0.75f * base.Projectile.Opacity, 0f, 0f);
            time++;
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.Projectile.Opacity == 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage > 0 && base.Projectile.Opacity == 1f)
            {
                if (base.Projectile.ai[0] == 0f || Main.zenithWorld)
                {
                    target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255f * base.Projectile.Opacity);
            if (CalamityGlobalNPC.SCal != -1 && NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) && Main.npc[CalamityGlobalNPC.SCal].active && Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().permafrost)
            {
                lightColor.G = (byte)(255f * base.Projectile.Opacity);
                lightColor.B = (byte)(255f * base.Projectile.Opacity);
                lightColor.R = 0;
            }

            CalamityUtils.DrawAfterimagesCentered(base.Projectile, ProjectileID.Sets.TrailingMode[base.Projectile.type], lightColor);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(base.Projectile.Center, 10f * base.Projectile.scale, targetHitbox);
        }
    }
}

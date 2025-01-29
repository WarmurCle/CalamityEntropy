using System;
using System.Collections.Generic;
using System.IO;
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
using Terraria.GameContent;
using Terraria.ModLoader;
using Particle = CalamityEntropy.Content.Particles.Particle;

namespace CalamityEntropy.Content.Projectiles
{
    public class ShadewindLanceThrow : ModProjectile, IJavelin
    {
        public bool SetHandRot { get; set; }
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ArmorPenetration = 86;
            SetHandRot = true;
        }
        public float handrot = 0;
        public float handrotspeed = 0;
        public Vector2 ownerMouse = Vector2.Zero;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(handrot);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
            handrot = reader.ReadSingle();
        }
        public override void OnSpawn(IEntitySource source)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.whoAmI != Projectile.whoAmI)
                {
                    if (p.ModProjectile is IJavelin jv)
                    {
                        jv.SetHandRot = false;
                    }
                }
            }
            
        }
        public override void AI(){
            if(Projectile.owner.ToPlayer().Entropy().WeaponBoost > 0 && Projectile.ai[0] > 12)
            {
                int boost = Projectile.owner.ToPlayer().Entropy().WeaponBoost;
                if (Main.myPlayer == Projectile.owner)
                {
                    if(Projectile.timeLeft % Math.Max(((int)(10 / boost)), 1) == 0)
                    {
                        Projectile p = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.4f, ModContent.ProjectileType<VoidStarF>(), (int)(Projectile.damage * 0.1f), 5, Projectile.owner)];
                        p.DamageType = CUtil.rogueDC;
                    }
                }
            }
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 16)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
            if (Projectile.ai[0] == 0)
            {
                handrotspeed = -0.3f;
            }
            else if (Projectile.ai[0] < 12)
            {
                handrotspeed += 0.056f;
            }
            if (Projectile.ai[0] < 12)
            {
                Projectile.owner.ToPlayer().heldProj = Projectile.whoAmI;
                var owner = Projectile.owner.ToPlayer();

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                    Projectile.netUpdate = true;
                }
                if (this.SetHandRot)
                {
                    if (owner.direction == 1)
                    {
                        Projectile.Center = owner.MountedCenter + new Vector2(26, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2 - handrot);
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - handrot - MathHelper.Pi);
                    }
                    else
                    {
                        Projectile.Center = owner.MountedCenter + new Vector2(26, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 + handrot);
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + handrot);
                    }
                }
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Projectile.rotation);
            }
            else if (Projectile.ai[0] < 36)
            {
                handrotspeed *= 0.84f;
                var owner = Projectile.owner.ToPlayer();
                if (this.SetHandRot)
                {
                    if (owner.direction == 1)
                    {
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - handrot - MathHelper.Pi);
                    }
                    else
                    {
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + handrot);

                    }
                }
                Projectile.owner.ToPlayer().heldProj = -1;
            }
            handrot -= handrotspeed;
            if (Projectile.ai[0] == 10)
            {

                SoundStyle SwingSound = new SoundStyle("CalamityMod/Sounds/Item/TerratomereSwing");
                SwingSound.Pitch = 1.6f;
                if (Projectile.Calamity().stealthStrike)
                {
                    SwingSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact");
                    SwingSound.Pitch = 0.2f;
                }
                
                SoundEngine.PlaySound(SwingSound, Projectile.Center);
            }
            if (Projectile.ai[0] == 12)
            {
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f) * (Projectile.Calamity().stealthStrike ? 3 : 1.8f);

                Projectile.extraUpdates = (Projectile.extraUpdates + 1) * 2 - 1;
            }
            if (Projectile.ai[0] > 12)
            {
                for (int i = 0; i < 10; i++)
                {
                    Particle p = new Particle();
                    p.position = Projectile.Center + Projectile.rotation.ToRotationVector2() * 30 - Projectile.velocity * ((float)i * 0.1f);
                    p.alpha = 0.105f;
                    VoidParticles.particles.Add(p);
                }
                for (int i = 0; i < 2; i++)
                {
                    Vector2 direction = new Vector2(-1, 0).RotatedBy(Projectile.rotation);
                    Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.1f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                    CalamityMod.Particles.Particle smoke = new HeavySmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * 30 + direction * 46f, smokeSpeed + Projectile.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.NextBool(3))
                    {
                        CalamityMod.Particles.Particle smokeGlow = new HeavySmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * 30 + direction * 46f, smokeSpeed + Projectile.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0.01f, true, 0.01f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
            }

            Projectile.ai[0]++;
            
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] >= 12;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 10 && !target.friendly;
        }
        public bool eff = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, Color.Purple, new Vector2(2f, 2f), 0, 0.1f, (Projectile.Calamity().stealthStrike ? 2 : 1) * 0.85f, (Projectile.Calamity().stealthStrike ? 46 : 36));
            GeneralParticleHandler.SpawnParticle(pulse);
            if (Projectile.Calamity().stealthStrike && Main.myPlayer == Projectile.owner && eff)
            {
                eff = false;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidRExp>(), 0, 0, Projectile.owner);
            }
            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(target.Center, Vector2.Zero, Color.Purple, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, (Projectile.Calamity().stealthStrike ? 2.2f : 1) * 0.65f, (Projectile.Calamity().stealthStrike ? 30 : 26));
            GeneralParticleHandler.SpawnParticle(explosion2);
            EGlobalNPC.AddVoidTouch(target, Projectile.Calamity().stealthStrike ? 360 : 100, Projectile.Calamity().stealthStrike ? 5 : 2, 800, 10);
            float sparkCount = Projectile.Calamity().stealthStrike ? 26 : 16;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(16, 0).RotatedBy((float)Main.rand.NextDouble() * 3.14159f * 2) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(20, 24);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Color.DarkBlue;

                float velc = Projectile.Calamity().stealthStrike ? 1.5f : 0.9f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Purple : Color.Purple);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void draw()
        {
            Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
            float rj = 0;
            if (Projectile.ai[0] < 12)
            {
                rj = -handrot * Projectile.owner.ToPlayer().direction;
            }
            Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4 + rj, tx.Size() / 2, Projectile.scale, SpriteEffects.None);

        }

    }

}
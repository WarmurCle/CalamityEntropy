using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public abstract class BaseMissleProj : ModProjectile
    {
        public const int AmmoType = 2035;
        public virtual int MaxStick => (int)Projectile.ai[0];
        public virtual float ExplodeRadius => Projectile.ai[1];
        public virtual NPC StickOnNPC => Projectile.ai[2] < 0 ? null : Main.npc[(int)Projectile.ai[2]];
        public virtual float adjustRotation => MathHelper.PiOver2;
        public virtual int Lifetime { get { return (int)Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        public Vector2 StickOffset = Vector2.Zero;
        public virtual int MaxStickTime => 8 * 60;
        public virtual float Gravity => 0;
        public virtual int FallingTime => 16;
        public bool NoGrav = false;
        public float winding = 0;
        public float Homing = 0;
        public float HomingRange = 500;
        public float MinVel = 0;
        public virtual float StickDamageAddition => 0.05f;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(MinVel);
            writer.WriteVector2(StickOffset);
            writer.Write(NoGrav);
            writer.Write(Homing);
            writer.Write(HomingRange);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MinVel = reader.ReadSingle();
            StickOffset = reader.ReadVector2();
            NoGrav = reader.ReadBoolean();
            Homing = reader.ReadSingle();
            HomingRange = reader.ReadSingle();
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 1200;
        }
        public virtual void OnExplodeHitNPC(NPC npc, NPC.HitInfo info, int damage) { }
        public override void OnKill(int timeLeft)
        {
            ExplodeVisual();
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile expl = CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromThis(), Projectile.GetOwner(), Projectile.Center, Projectile.damage, ExplodeRadius, Projectile.DamageType);
                if(expl.ModProjectile is CommonExplotionFriendly cef)
                {
                    cef.onHitAction = OnExplodeHitNPC;
                    expl.Entropy().applyBuffs = Projectile.Entropy().applyBuffs;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (CEUtils.LineThroughRect(Projectile.Center - Projectile.velocity, Projectile.Center, targetHitbox, Projectile.height))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }
        public virtual void ExplodeVisual()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.1f, scale * 0.46f, 14));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.OrangeRed, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);

        }
        public virtual void SetupStats() { }
        public virtual void StickUpdate(NPC target)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(StickOnNPC == null)
            {
                int count = 0;
                while(Projectile.Colliding(Projectile.Hitbox, target.Hitbox))
                {
                    if (count++ > 256)
                        break;
                    Projectile.position += Projectile.velocity.SafeNormalize(Vector2.UnitX) * -2;
                }
                Projectile.position += Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5;
                Projectile.timeLeft = MaxStickTime;
                Projectile.ai[2] = target.whoAmI;
                StickOffset = Projectile.Center - target.Center;
                CEUtils.SyncProj(Projectile.whoAmI);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (StickOnNPC != null)
                modifiers.SourceDamage /= 9;
        }
        public bool tileCollide = false;
        public override void AI()
        {
            if (Lifetime == 0)
            {
                tileCollide = Projectile.tileCollide;
                Projectile.ai[2] = -1;
                SetupStats();
                Projectile.localAI[1] = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            Lifetime++;
            if (StickOnNPC != null)
            {
                Projectile.tileCollide = false;
                if (!StickOnNPC.active)
                {
                    Projectile.tileCollide = tileCollide;
                    Projectile.ai[2] = -1;
                    Projectile.velocity *= 0;
                }
                else
                {
                    Projectile.Center = StickOnNPC.Center + StickOffset;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        StickUpdate(StickOnNPC);
                    }
                    StickOnNPC.Entropy().StickByMissle = 10;
                    CheckExplode();
                }
            }
            else
            {
                if (!NoGrav && Lifetime > FallingTime)
                {
                    Projectile.velocity += new Vector2(0, Gravity);
                }
            }
;
            
            if(Lifetime <= 12 || !Projectile.HomingToNPCNearby(Homing, 1 - Homing * 0.015f, HomingRange))
            {
                if (Projectile.velocity.Length() < MinVel)
                {
                    Projectile.velocity *= 1.06f;
                }
            }
            Projectile.localAI[1] += 0.32f;
            if (StickOnNPC == null)
            {
                Vector2 adv = Projectile.velocity.RotatedBy(Math.Sin(Projectile.localAI[1]) * winding * 0.42f);
                Projectile.rotation = adv.ToRotation() + adjustRotation;
                SpawnParticle(adv);
                Projectile.Center += adv;
            }

        }
        public virtual void SpawnParticle(Vector2 vel)
        {
            for (int i = 0; i < 4; i++)
            {
                EParticle.NewParticle(new Smoke() { timeleftmax = 26, Lifetime = 26 }, Projectile.Center + vel * 0.25f * i, CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.02f, 0.04f), 0.5f, true, BlendState.Additive, CEUtils.randomRot());
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public void CheckExplode()
        {
            NPC target = StickOnNPC;
            int type = Projectile.type;
            int count = 0;
            Projectile oldest = null;
            int lifetime = 0;
            foreach(Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == type && proj.ai[2] == Projectile.ai[2])
                {
                    count++;
                    if (proj.localAI[0] > lifetime)
                    {
                        oldest = proj;
                        lifetime = (int)proj.localAI[0];
                    }
                }
            }
            if (count > MaxStick)
            {
                oldest.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}

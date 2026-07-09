using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static CalamityEntropy.CEUtils;

namespace CalamityEntropy.Content.Projectiles
{
    public class LightningSpear : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        private bool sd = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            Player Owner = Projectile.GetOwner();
            if (!sd && Projectile.owner == Main.myPlayer)
            {
                sd = true;
                Particle pulse = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 16, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 22, 0.07f, Color.MediumTurquoise, new Vector2(1f, 1.7f), shrinkSpeed: -0.2f);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 24, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 18, 0.05f, Color.DeepSkyBlue, new Vector2(1f, 1.7f), shrinkSpeed: -0.23f);
                GeneralParticleHandler.SpawnParticle(pulse2);
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5)
            {
                NPC target = Projectile.FindTargetWithinRange(1200, false);
                if (target != null)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).ToRotation().ToRotationVector2() * 12f;
                    Projectile.velocity *= 0.9f;
                }
            }
            odr.Add(Projectile.rotation);
            odp.Add(Projectile.Center);
            if (odp.Count > 12)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexPointSets> v = new List<VertexPointSets>();
            var gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = getExtraTex("VoltTrailThicc");
            Main.spriteBatch.UseAdditive();
            for (int i = 0; i < odp.Count; i++)
            {
                float a = (i / (odp.Count - 1f));
                v.Add(new VertexPointSets(odp[i], Color.LightBlue, 26, (i / (odp.Count - 1f)) * 3f + Main.GlobalTimeWrappedHourly * 4));
            }
            var ve = GetVertexesList(v);
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            v.Clear();
            for (int i = 0; i < odp.Count; i++)
            {
                float a = (i / (odp.Count - 1f));
                v.Add(new VertexPointSets(odp[i], Color.White, 12, (i / (odp.Count - 1f)) * 3f + Main.GlobalTimeWrappedHourly * 6));
            }
            ve = GetVertexesList(v);
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            Main.spriteBatch.ExitShaderRegion();
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/LightningSpear").Value;
            float x = 0f;
            for (int i = 0; i < odp.Count; i++)
            {
                Color tc = Color.White;
                if (Projectile.ai[2] == 1)
                {
                    tc = new Color(255, 0, 255);
                }
                Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, tc * x * 0.6f, odr[i], new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);
                x += 1 / 14f;
            }

            return true;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle sd = new("CalamityMod/Sounds/Item/AnomalysNanogunMPFBExplosion");
            sd.Volume = 0.6f;
            SoundEngine.PlaySound(sd, Projectile.Center);
            var r = Main.rand;
            for (int i = 0; i < 8 + 4 * Projectile.owner.ToPlayer().Entropy().WeaponBoost; i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Projectile.Center, new Vector2(40, 0).RotateRandom(Math.PI * 2), ModContent.ProjectileType<LightningBolt>(), (int)(Projectile.damage * 0.3f), 1);
            }

        }
    }


}
using System;
using System.Collections.Generic;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

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
        }

        public override void AI(){
            if (!sd && Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(Projectile.owner.ToPlayer().GetSource_FromAI(), Projectile.Center + Projectile.velocity * 1.4f, Vector2.Zero, ModContent.ProjectileType<Impact>(), 0, 0, Projectile.owner);
                sd = true;
                p.ToProj().rotation = Projectile.velocity.ToRotation();
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 8)
            {
                NPC target = Projectile.FindTargetWithinRange(1200, false);
                if (target != null)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).ToRotation().ToRotationVector2() * 8f;
                    Projectile.velocity *= 0.88f;
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
            SoundEngine.PlaySound(sd, Projectile.Center);
            var r = Main.rand;
            for (int i = 0; i < 8; i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Projectile.Center, new Vector2(40, 0).RotateRandom(Math.PI * 2), ModContent.ProjectileType<LightningBolt>(), (int)(Projectile.damage * 0.3f), 1);
            }

        }
    }
    

}
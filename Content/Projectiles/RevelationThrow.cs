using System;
using System.Collections.Generic;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class RevelationThrow : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        float angle;
        float speed = 30;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 114;
            Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;

        }

        public override void AI(){
            if (Projectile.ai[0] == 0)
            {
                angle = Projectile.velocity.ToRotation();
            }
            if (speed < 0)
            {
                angle = (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation();
                if (Util.Util.getDistance(Projectile.Center, Main.player[Projectile.owner].Center) < Projectile.velocity.Length() * 1.12f)
                {
                    Projectile.Kill();
                }
            }
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 6)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
            Projectile.velocity = angle.ToRotationVector2() * speed * 4;
            speed -= 2;
            Projectile.ai[0]++;
            Projectile.rotation += 0.3f;
            if (Projectile.ai[0] % 2 == 0 && speed > 0)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(90)) * 0.1f, ModContent.ProjectileType<EmpyreanStellarDetritus>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(-90)) * 0.1f, ModContent.ProjectileType<EmpyreanStellarDetritus>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);

            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
            float rt = (float)(Main.rand.NextDouble() * Math.PI * 2);
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Projectile.Center, rt.ToRotationVector2() * 26, ModContent.ProjectileType<EmpyreanStellarDetritus>(), (int)(Projectile.damage * 0.33f), Projectile.knockBack, Projectile.owner);
                rt += MathHelper.ToRadians(120);
            }
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/RevelationThrow").Value;
            float c = 0;
            for (int i = 0; i < odp.Count; i++) {
                Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, Color.White * c* 0.6f, odr[i], new Vector2(tx.Width / 2, tx.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0);
                c += 1f / odp.Count;
            }
            return true;
        }

    }

}
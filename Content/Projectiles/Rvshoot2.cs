using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class Rvshoot2 : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        bool htd = false;
        float exps = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 114;
            Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {


            Projectile.ai[0]++;
            if (htd)
            {
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);

                }
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation);
                if (odp.Count > 12)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }

                NPC target = Projectile.FindTargetWithinRange(1600, false);
                if (target != null)
                {
                    Projectile.velocity *= 0.96f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 3f;
                }
            }
            Projectile.rotation += 0.3f;
            exps *= 0.9f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (htd)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!htd)
            {
                target.immune[Projectile.owner] = 0;
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                base.OnHitNPC(target, hit, damageDone);
                Projectile.timeLeft = 20;
                htd = true;
                exps = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color cl = Color.Lerp(Color.Black, Color.White, Projectile.ai[0] / 30f);

            if (odp.Count > 1 && (!htd))
            {

                Texture2D ht = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Rvshoot2").Value;
                if (!htd)
                {
                    Main.spriteBatch.Draw(ht, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(ht.Width, ht.Height) / 2, 1, SpriteEffects.None, 0);
                }
                Utilities.Util.DrawAfterimage(ht, odp, odr);

            }

            if (exps > 0)
            {
                if (htd)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/E_Exp").Value;

                    Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * exps, 0, new Vector2(tx.Height, tx.Width) / 2, new Vector2((1 - exps) * 2f, (1 - exps) * 2f), SpriteEffects.None, 0);
                }
            }
            return false;
        }

    }

}
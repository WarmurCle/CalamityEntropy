using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class Rvshoot : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        float angle;
        float speed = 30;
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
            if (Projectile.ai[0] == 0)
            {
                angle = Projectile.velocity.ToRotation();
            }
            if (speed < 0)
            {
                angle = (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation();
                if (Utilities.Util.getDistance(Projectile.Center, Main.player[Projectile.owner].Center) < Projectile.velocity.Length() * 1.12f)
                {
                    Projectile.Kill();
                }
            }



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
                if (odp.Count > 16)
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
            float c = 0;


            if (odp.Count > 1)
            {

                Texture2D ht = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/sth").Value;
                if (!htd)
                {
                    Main.spriteBatch.Draw(ht, odp[odp.Count - 1] - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), new Vector2(ht.Width, ht.Height) / 2, 1, SpriteEffects.None, 0);
                }


            }
            c = 0;
            if (odp.Count > 1)
            {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<Vertex> ve = new List<Vertex>();
                Color b = Color.Black;
                ve.Add(new Vertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 30,
                          new Vector3((float)0, 1, 1),
                          b));
                ve.Add(new Vertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 30,
                      new Vector3((float)0, 0, 1),
                      b));
                for (int i = 1; i < odp.Count; i++)
                {


                    c += 1f / odp.Count;
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 20,
                          new Vector3((float)i / odp.Count, 1, 1),
                          b));
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 20,
                          new Vector3((float)i / odp.Count, 0, 1),
                          b));

                }

                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/rvslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                c = 0;
                for (int i = 1; i < odp.Count; i++)
                {
                    c += 1f / odp.Count;
                    Utilities.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, odp[i - 1], odp[i], Color.Black, 14 * c, 2);


                    Utilities.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, odp[i - 1], odp[i], Color.White, 2 * c, 2);


                }

            }
            if (exps > 0)
            {
                if (htd)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/E_Exp").Value;

                    Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * exps, 0, new Vector2(tx.Height, tx.Width) / 2, new Vector2((1 - exps) * 2f, (1 - exps) * 2f), SpriteEffects.None, 0);
                }
            }
            return true;
        }

    }

}
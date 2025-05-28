using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Prophet
{
    public class ProphetVoidSpike : ModProjectile
    {
        public float length = 0;
        public float lengthAdd = 17f;
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 800;
        }
        public override void AI()
        {
            if (!((int)Projectile.ai[1]).ToNPC().active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = ((int)Projectile.ai[1]).ToNPC().Center;
            length += lengthAdd;
            lengthAdd *= 0.977f;
            if (Projectile.ai[0] == 70)
            {
                lengthAdd = 160;
            }
            if (Projectile.ai[0] < 70)
            {
                lengthAdd -= 0.14f;
            }
            if (Projectile.ai[0] > 82)
            {
                length *= 0.75f;
                length -= 1;
                Projectile.Opacity *= 0.9f;
            }
            if (Projectile.ai[0] == 122)
            {
                Projectile.Kill();
            }
            Projectile.ai[0]++;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulDisorder>(), 5 * 60);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 70)
                return false;
            return Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * length, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public void Draw()
        {
            List<Vertex> ve = new List<Vertex>();
            Color b = Color.White * Projectile.Opacity;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < 24; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * length, (float)i / 31f) + Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 12 + i * 0.26f) * (i / 31f) * length * 0.065f * (Projectile.ai[0] < 70 ? 0.5f : 1));
            }
            float lc = 1;
            float jn = 0;

            for (int i = 1; i < points.Count - 1; i++)
            {
                jn = (float)(i - 1) / (points.Count - 2);
                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * lc,
                      new Vector3(jn, 1, 1),
                      b));
                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * lc,
                      new Vector3(jn, 0, 1),
                      b));
            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = Projectile.GetTexture();
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            //Main.EntitySpriteDraw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.velocity.ToRotation(), new Vector2(0, 12), new Vector2(length / 90f, 1), SpriteEffects.None, 0);
        }

    }

}
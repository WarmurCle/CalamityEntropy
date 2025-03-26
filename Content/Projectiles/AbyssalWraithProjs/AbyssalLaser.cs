using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.AbyssalWraithProjs
{

    public class AbyssalLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public float counter = 0;
        public int drawcount = 0;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
        }
        public bool ss = true;
        Vector2 targetPosLaser { get { return new Vector2(Projectile.ai[0], Projectile.ai[1]); } set { Projectile.ai[0] = value.X; Projectile.ai[1] = value.Y; } }
        Vector2 targetPosLaserVel = Vector2.Zero;
        public int soundcounter = 0;
        public override void AI()
        {
            if (ss)
            {
                ss = false;
                targetPosLaser = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 800;
            }
            if (((int)Projectile.ai[2]).ToNPC().active && ((int)Projectile.ai[2]).ToNPC().ModNPC is AbyssalWraith aw)
            {
                if (aw.deathAnm)
                {
                    Projectile.Kill();
                }
            }
            if (soundcounter == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/light_bolt_delayed"));
            }
            if (soundcounter == 50)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/laser"));
                s = 1;
            }
            soundcounter++;
            if (s < 0.3f)
            {
                s += 0.025f;
            }
            Player target = Main.player[0];
            if ((((int)Projectile.ai[2]).ToNPC().active && ((int)Projectile.ai[2]).ToNPC().HasValidTarget))
            {
                target = ((int)Projectile.ai[2]).ToNPC().target.ToPlayer();

            }
            float spc = 1;
            if (Projectile.timeLeft < 120)
            {
                spc = (float)Projectile.timeLeft / 120f;
            }
            targetPosLaserVel += (target.Center - targetPosLaser).SafeNormalize(Vector2.Zero) * 0.8f * spc;
            targetPosLaserVel *= 0.9f;
            targetPosLaser += targetPosLaserVel;
            if (Projectile.timeLeft < 20)
            {
                opc -= 0.05f;
            }

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return Projectile.timeLeft > 20 && s >= 0.9f;
        }
        float opc = 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            List<Vector2> points = new List<Vector2>();
            Vector2 pb = Projectile.Center + Projectile.velocity;
            for (int i = 0; i < 10; i++)
            {
                float prog = (float)i / 10f;
                Vector2 ba = Vector2.Lerp(Projectile.Center, pb, prog);
                Vector2 bb = Vector2.Lerp(pb, targetPosLaser, prog);
                Vector2 vc = Vector2.Lerp(ba, bb, prog);
                points.Add(vc);
            }
            points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]).SafeNormalize(new Vector2(1, 1)) * 2800);
            points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]).SafeNormalize(new Vector2(1, 1)) * 2800);
            for (int i = 1; i < points.Count; i++)
            {
                if (Util.Util.LineThroughRect(points[i - 1], points[i], targetHitbox, 30))
                {
                    return true;
                }
            }
            return false;
        }
        float s = 0f;
        public void drawLaser()
        {
            counter += 0.14f;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();
            Color b = new Color(60, 60, 160) * opc;
            List<Vector2> points = new List<Vector2>();
            Vector2 pb = Projectile.Center + Projectile.velocity;
            points.Add(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.One) * 2);
            for (int i = 0; i < 10; i++)
            {
                float prog = (float)i / 10f;
                Vector2 ba = Vector2.Lerp(Projectile.Center, pb, prog);
                Vector2 bb = Vector2.Lerp(pb, targetPosLaser, prog);
                Vector2 vc = Vector2.Lerp(ba, bb, prog);
                points.Add(vc);
            }
            points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]).SafeNormalize(new Vector2(1, 1)) * 2800);
            points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]).SafeNormalize(new Vector2(1, 1)) * 2800);
            float lc = 0.6f * s;
            float jn = -counter;

            for (int i = 1; i < points.Count - 1; i++)
            {
                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 130 * lc,
                      new Vector3(jn, 1, 1),
                      b));
                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 130 * lc,
                      new Vector3(jn, 0, 1),
                      b));
                jn += Util.Util.getDistance(points[i - 1], points[i]) / (float)tex.Width * lc;

            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }

}
using System.Collections.Generic;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SlimeYoyoProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 5f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12f;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3;
        }
        Vector2 lmp = Vector2.Zero;
        public int dmgandkbUp = 30;
        public override void AI()
        {
            if (dmgandkbUp == 30)
            {
                Projectile.damage *= 3 + Projectile.owner.ToPlayer().Entropy().WeaponBoost * 2;
                Projectile.knockBack *= 6f;
            }
            if (dmgandkbUp == 0)
            {
                Projectile.damage /= 3 + Projectile.owner.ToPlayer().Entropy().WeaponBoost * 2;
                Projectile.knockBack /= 6f;
            }
            dmgandkbUp--;
            if (rope == null)
            {
                rope = new Rope(Projectile.owner.ToPlayer().Center, Projectile.Center, 25, 0, new Vector2(0, 0.6f), 0.06f, 15, true);
            }
            rope.segmentLength = Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) / 25f;
            rope.Start = Projectile.owner.ToPlayer().Center;
            rope.End = Projectile.Center;
            rope.Update();
            Projectile.rotation += 0.5f;
            Projectile.timeLeft = 3;
            if (Projectile.owner.ToPlayer().channel)
            {

            }
            else
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.netUpdate = true;
                    Projectile.ai[0] = 1;
                }
            }
            if (Projectile.ai[0] == 1)
            {
                rope.tileCollide = false;
                Projectile.tileCollide = false;
                Vector2 targetPos = Projectile.owner.ToPlayer().MountedCenter;
                Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * ProjectileID.Sets.YoyosTopSpeed[Projectile.type] * 1.6f;
                Projectile.velocity *= 0.9f;
                if (Util.Util.getDistance(Projectile.Center, targetPos) < Projectile.velocity.Length() * 1.16f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    if (lmp != Main.MouseWorld)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.ai[1] = Main.MouseWorld.X;
                    Projectile.ai[2] = Main.MouseWorld.Y;
                }
                Vector2 targetPos = new Vector2(Projectile.ai[1], Projectile.ai[2]);
                if (Util.Util.getDistance(targetPos, Projectile.owner.ToPlayer().Center) > ProjectileID.Sets.YoyosMaximumRange[Projectile.type])
                {
                    targetPos = Projectile.owner.ToPlayer().Center + (targetPos - Projectile.owner.ToPlayer().Center).SafeNormalize(Vector2.One) * ProjectileID.Sets.YoyosMaximumRange[Projectile.type];
                }
                Projectile.velocity = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * ProjectileID.Sets.YoyosTopSpeed[Projectile.type] * 3;
                if (Util.Util.getDistance(Projectile.Center, targetPos) < ProjectileID.Sets.YoyosTopSpeed[Projectile.type] * 3.1f)
                {
                    Projectile.velocity = targetPos - Projectile.Center;
                    
                }

                if (Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) > ProjectileID.Sets.YoyosMaximumRange[Projectile.type] + 60)
                {
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.owner.ToPlayer().direction = (Projectile.Center.X > Projectile.owner.ToPlayer().Center.X ? 1 : -1);
            Projectile.owner.ToPlayer().itemRotation = (Projectile.Center - Projectile.owner.ToPlayer().Center).ToRotation() * Projectile.owner.ToPlayer().direction;
            Projectile.owner.ToPlayer().itemTime = 6;
            Projectile.owner.ToPlayer().itemAnimation = 6;
            
        }
        Rope rope;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();
            Color b = lightColor;
            List<Vector2> points = new List<Vector2>();
            points = rope.GetPoints();

            points.Insert(0, Projectile.owner.ToPlayer().Center);
            points.Add(Projectile.Center);
            points.Add(Projectile.Center);
            float lc = 1;
            float jn = 0;

            for (int i = 1; i < points.Count - 1; i++)
            {
                jn += Util.Util.getDistance(points[i - 1], points[i]) / (float)16 * lc;

                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 5 * lc,
                      new Vector3(jn, 1, 1),
                      Lighting.GetColor(new Point((int)(points[i].X / 16f), (int)(points[i].Y / 16f)))));
                ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 5 * lc,
                      new Vector3(jn, 0, 1),
                      Lighting.GetColor(new Point((int)(points[i].X / 16f), (int)(points[i].Y / 16f)))));
                
            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/SlimeRope").Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            /*for (int i = 1; i < points.Count; i++)
            {
                Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value;
                Util.Util.drawLine(Main.spriteBatch, t, points[i - 1], points[i], Color.White, 8);
            }*/

            Texture2D pt = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 lu = new Vector2(-1, -1) * pt.Width / 2 * Projectile.scale * 1.4f;
            Vector2 ru = new Vector2(1, -1) * pt.Width / 2 * Projectile.scale * 1.4f;
            Vector2 ld = new Vector2(-1, 1) * pt.Width / 2 * Projectile.scale * 1.4f;
            Vector2 rd = new Vector2(1, 1) * pt.Width / 2 * Projectile.scale * 1.4f;
            lu = lu.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            ru = ru.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            ld = ld.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            rd = rd.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());

            float decayFactor = 0.02f;

            float yo = 1.0f / (1.0f + decayFactor * Projectile.velocity.Length());
            lu.Y *= yo;
            ru.Y *= yo;
            ld.Y *= yo;
            rd.Y *= yo;
            lu = lu.RotatedBy(Projectile.velocity.ToRotation());
            ru = ru.RotatedBy(Projectile.velocity.ToRotation());
            ld = ld.RotatedBy(Projectile.velocity.ToRotation());
            rd = rd.RotatedBy(Projectile.velocity.ToRotation());
            Util.Util.drawTextureToPoint(Main.spriteBatch, pt, lightColor, Projectile.Center + lu - Main.screenPosition, Projectile.Center + ru - Main.screenPosition, Projectile.Center + ld - Main.screenPosition, Projectile.Center + rd - Main.screenPosition);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
    

}
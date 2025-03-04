using System;
using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NetherRiftBlade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.ArmorPenetration = 1000;
        }
        public int counter1 = 0;
        public int counter2 = 0;
        public float length { get { return Projectile.ai[1];} set { Projectile.ai[1] = value; } }
        float rotspeed = 0;
        float l = 0;
        bool soundplay = false;
        int counter = 0;
        public override bool? CanHitNPC(NPC target)
        {
            if(counter < 30)
            {
                return false;
            }
            return null;
        }
        public override void AI()
        {
            counter++;
            counter1++; 
            if (rope == null)
            {
                rope = new Rope(Projectile.owner.ToPlayer().Center, Projectile.Center, 25, 0, new Vector2(0, 0.6f), 0.1f, 26, false);
            }
            rope.segmentLength = Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) / 25f;
            rope.Start = Projectile.owner.ToPlayer().Center;
            rope.End = Projectile.Center;
            rope.Update();
            List<Vector2> p = rope.GetPoints();
            Projectile.rotation = (p[p.Count - 1].GetSymmetryPoint(Projectile.owner.ToPlayer().Center, Projectile.Center) - p[p.Count - 2].GetSymmetryPoint(Projectile.owner.ToPlayer().Center, Projectile.Center)).ToRotation();
            Player player = Projectile.owner.ToPlayer();
            Projectile.timeLeft = 3;
            if (player.channel)
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
                Projectile.velocity *= 0.9f;
                counter2 += 1;
                if (counter2 > 16)
                {
                    Projectile.velocity += (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 8;
                }
                if (Util.Util.getDistance(Projectile.Center, player.Center) < Projectile.velocity.Length() + 6)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (counter1 > 2)
                {
                    l = l + (length - l) * 0.1f;
                    rotspeed += (0.1f - rotspeed) * 0.01f;
                    Vector2 targetpos = player.Center + new Vector2(l, 0).RotatedBy((Projectile.Center - player.Center).ToRotation() + rotspeed * (426 / Util.Util.getDistance(Projectile.Center, player.Center)));
                    Projectile.velocity = targetpos - Projectile.Center;
                }
                float a = (Projectile.Center - player.Center).ToRotation();
                if(a < 0)
                {
                    soundplay = true;
                }
                else
                {
                    if (soundplay)
                    {
                        soundplay = false;
                        if (Main.rand.NextBool(2))
                        {
                            Util.Util.PlaySound("spin1", 1f);
                        }
                        else
                        {
                            Util.Util.PlaySound("spin2", 1f);
                        }
                    }
                }
            }
            if(Main.myPlayer == Projectile.owner)
            {
                length = Math.Max(256, Math.Abs(Util.Util.getDistance(Main.MouseWorld, player.Center)));
            }
            player.itemTime = 2;
            player.itemAnimation = 2;
            if(Projectile.Center.X > player.Center.X)
            {
                player.direction = 1;
            }
            else
            {
                player.direction = -1;
            }
            player.heldProj = Projectile.whoAmI;
        }
        Rope rope;

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Projectile.owner.ToPlayer();
            List<Vector2> points = new List<Vector2>();
            points = rope.GetPoints();
            Texture2D handle = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/NetherRiftHandle").Value;
            Main.spriteBatch.Draw(handle, Projectile.owner.ToPlayer().Center + player.gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White, (points[1] - points[0]).ToRotation(), new Vector2(28,  handle.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();
            Color b = lightColor;

            points.Insert(0, Projectile.owner.ToPlayer().Center);
            points.Add(Projectile.Center);

            float lc = 1;
            float jn = 0;
            
            for (int i = 1; i < points.Count - 1; i++)
            {
                jn += Util.Util.getDistance(points[i - 1], points[i]) / (float)70 * lc;

                ve.Add(new Vertex(points[i].GetSymmetryPoint(Projectile.owner.ToPlayer().Center, Projectile.Center) - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 30 * lc,
                      new Vector3(jn, 1, 1),
                      Color.White));
                ve.Add(new Vertex(points[i].GetSymmetryPoint(Projectile.owner.ToPlayer().Center, Projectile.Center) - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 30 * lc,
                      new Vector3(jn, 0, 1),
                      Color.White));
                
            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/NetherChain").Value;
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
            Main.spriteBatch.Draw(pt, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, pt.Size() / 2, Projectile.scale * 2, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 60, 2, 600, 16);
        }
    }
    

}
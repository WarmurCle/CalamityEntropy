using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityEntropy.Util;
using CalamityMod.Graphics.Primitives;
using CalamityMod;
using System.Runtime.Intrinsics.Arm;
using Terraria.Graphics.Shaders;
namespace CalamityEntropy.Projectiles.AbyssalWraithProjs
{
    
    public class SighterPin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 520;
            Projectile.extraUpdates = 2;
        }
        public bool setv = true;
        public List<Vector2> odp = new List<Vector2>();
        public override void AI()
        {
            odp.Add(Projectile.Center + Projectile.rotation.ToRotationVector2() * 30);
            if (odp.Count > 36)
            {
                odp.RemoveAt(0);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft % 30 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<SpImpact>(), 0, 0);
                }
            }
            
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.Cos(Projectile.ai[0]++ * 0.2f) * 0.022f);
            if (Projectile.timeLeft < 450)
            {
                Projectile.velocity *= 1.007f;
            }
            else
            {
                Projectile.velocity *= 1.002f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            drawT();
            return false;
        }

        public Color TrailColor(float completionRatio)
        {
            Color result = Color.Lerp(new Color(100, 150, 255), Color.Gold, completionRatio);
            return result * completionRatio;
        }

        public float TrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0, 70 * Projectile.scale, completionRatio);
        }

        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            var mp = this;
            if (mp.odp.Count > 1)
            {
                List<Vertex> ve = new List<Vertex>();
                Color b = Color.LightBlue;

                float a = 0;
                float lr = 0;
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 22,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 22,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)//因为顶点需要围成一个三角形才能画出来 所以需要判顶点数>=3 否则报错
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }


            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D value = TextureAssets.Projectile[base.Projectile.type].Value;
            Vector2 position = base.Projectile.Center - Main.screenPosition + Vector2.UnitY * base.Projectile.gfxOffY;
            Vector2 origin = value.Size() * 0.5f;
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            Main.EntitySpriteDraw(value, position, null, base.Projectile.GetAlpha(Color.White), base.Projectile.rotation, origin, base.Projectile.scale, SpriteEffects.None);

        }
    }

}
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{

    public class CruiserEnergyBall : ModProjectile
    {
        public override string Texture => Util.Util.WhiteTexPath;
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
        }
        public float Scale = 0;
        public NPC owner { get { return ((int)Projectile.ai[0]).ToNPC(); } set { Projectile.ai[0] = value.whoAmI; } }
        public override void AI()
        {
            if(Projectile.timeLeft > 120)
            {
                Scale += 1 / 180f;
                Projectile.Center = owner.Center;
            }
            if(Projectile.timeLeft == 120)
            {
                Scale = 1;
                Projectile.velocity = owner.velocity.normalize() * 30;
            }
            if(Projectile.timeLeft < 120)
            {
                Projectile.velocity *= 0.98f;
            }
            if(Projectile.timeLeft < 10)
            {
                Scale *= 1.1f;
                Projectile.Opacity -= 0.1f;
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Center.getRectCentered(Scale * 60, Scale * 60);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            float rj = Util.Util.randomRot();
            for(float i = 0; i < 360; i += 20f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (rj + MathHelper.ToRadians(i)).ToRotationVector2() * 18, ModContent.ProjectileType<VoidSpike>(), Projectile.damage, Projectile.knockBack);
            }
        }
        public List<Vector2> GP(float distAdd = 0, float c = 1)
        {
            float dist = distAdd;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i <= 60; i++)
            {
                points.Add(new Vector2(dist, 0).RotatedBy(MathHelper.ToRadians(i * 6 - 80 * c * Main.GlobalTimeWrappedHourly)));
            }
            return points;
        }
        public void Draw()
        {

            float a = Scale;
            if(a > 1)
            {
                a = 1;
            }
            a *= Projectile.Opacity;
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            {
                List<Vertex> ve = new List<Vertex>();
                List<Vector2> points = GP(0);
                List<Vector2> pointsOutside = GP(70 * Scale);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + points[i],
                    new Vector3((float)i / points.Count, 1, 1f),
                          Color.LightBlue * a));
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          Color.LightBlue * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = Util.Util.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<Vertex> ve = new List<Vertex>();
                List<Vector2> points = GP(0, -1);
                List<Vector2> pointsOutside = GP(60 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          Color.White * a));
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          Color.White * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = Util.Util.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<Vertex> ve = new List<Vertex>();
                List<Vector2> points = GP(0, 0.6f);
                List<Vector2> pointsOutside = GP(80 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          Color.LightBlue * a));
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          Color.LightBlue * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = Util.Util.getExtraTex("AbyssalCircle4");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }

}
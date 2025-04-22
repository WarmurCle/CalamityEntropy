using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class ProphecyRuneBM : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 400;

        }
        public override bool? CanHitNPC(NPC target)
        {
            if(counter < 100)
            {
                return false;
            }
            return null;
        }
        public float rotCount = 0;
        public float counter = 0;
        public override void AI()
        {
            rotCount += 0.32f * ((100 - counter) * 0.01f);
            counter++;
            if (counter > 100)
            {
                NPC target = Util.Util.findTarget(Projectile.getOwner(), Projectile, 2800);
                if (target != null)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * 1.9f;
                    Projectile.velocity *= 0.96f;
                }
            }
            else
            {
                NPC target = Util.Util.findTarget(Projectile.getOwner(), Projectile, 2800);
                if (target != null)
                {
                    Projectile.Center = target.Center + (Projectile.ai[0]).ToRotationVector2().RotatedBy(rotCount) * counter * 1.8f;
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = Util.Util.getExtraTex("lightball");
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, Color.White * (counter > 100 ? 1 : counter / 100f) * 0.8f, Projectile.rotation, light.Size() / 2, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tx = Util.Util.getExtraTex("runes/rune" + ((int)Projectile.ai[2]).ToString());
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * (counter > 100 ? 1 : counter / 100f) * (0.8f + (float)(Math.Cos(Main.GameUpdateCount * 0.2f) * 0.2f)), 0, tx.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

    }

}
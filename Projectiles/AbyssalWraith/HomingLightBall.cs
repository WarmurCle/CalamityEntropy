using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using CalamityEntropy.Util;
using Terraria.Audio;
using System.IO;
namespace CalamityEntropy.Projectiles.AbyssalWraith
{
    
    public class HomingLightBall: ModProjectile
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
            Projectile.timeLeft = 600;
            
        }

        public override void AI()
        {

            if ((((int)Projectile.ai[2]).ToNPC().active && ((int)Projectile.ai[2]).ToNPC().HasValidTarget))
            {
                Player target = ((int)Projectile.ai[2]).ToNPC().target.ToPlayer();
                if (Util.Util.getDistance(Projectile.Center, target.Center) > 200)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                    Projectile.velocity *= 0.96f;
                }
            }
            if (Projectile.timeLeft < 100)
            {
                opc -= 0.01f;
            }
        }
        public override bool CanHitPlayer(Player target)
        {
            return Projectile.timeLeft > 70;
        }
        float opc = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            drawcount++;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D warn = ModContent.Request<Texture2D>("CalamityEntropy/Extra/vlbw").Value;
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White * opc, MathHelper.ToRadians(drawcount * 4f), t.Size() / 2, new Vector2(1.5f, 1) * Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White * opc, MathHelper.ToRadians((drawcount + 64) * 14f), t.Size() / 2, new Vector2(1.5f, 1) * Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White * opc, MathHelper.ToRadians((drawcount + 154) * 34f), t.Size() / 2, new Vector2(1.5f, 1) * Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

}
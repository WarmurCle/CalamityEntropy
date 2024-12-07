using System;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.HBProj
{
    public class HB : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 1.2f;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.scale = 1.2f;
        }
        public bool right = true;
        public override void AI(){
            Player player = Projectile.owner.ToPlayer();
            Projectile.Center = player.Center;
            right = player.direction == 1;
            if (!player.HeldItem.IsAir && player.HeldItem.type == ModContent.ItemType<Mercy>())
            {
                Projectile.timeLeft = 2;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        float counter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            counter++;
            Vector2 drawpos = Projectile.owner.ToPlayer().Center + new Vector2(0, -32);
            Vector2 ep = new Vector2(0, 0) * Projectile.owner.ToPlayer().direction;
            float fs = (float)(1 + Math.Cos(counter / 3) * 0.1);
            SpriteBatch sb = Main.spriteBatch;
            bool flip = false;
            SpriteEffects ef = SpriteEffects.None;
            if (!right)
            {
                flip = true;
                ef = SpriteEffects.FlipHorizontally;
            }


            Texture2D part1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/HBProj/HB").Value;
            Texture2D eye = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/HBProj/iMark").Value;


            int scl = (int)(230 + 25 * Math.Cos(counter / 6));
            sb.Draw(part1, drawpos + ep * 0.4f - Main.screenPosition, null, new Color(scl, scl, scl), counter * 0.1f, part1.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            sb.Draw(eye, drawpos + ep * 0.4f - Main.screenPosition, Util.Util.GetCutTexRect(eye, 4, (int)(counter / 4) % 4), Color.White, 0, new Vector2((flip ? 86 - 54 : 54), 46), Projectile.scale, ef, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    

}
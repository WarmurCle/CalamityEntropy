using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.VoidEchoProj
{
    public class VoidEchoProj : ModProjectile
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
        public override void AI()
        {
            Player player = Projectile.owner.ToPlayer();
            Projectile.Center = player.Center + player.gfxOffY * Vector2.UnitY;
            right = player.direction == 1;
            if (!player.HeldItem.IsAir && player.HeldItem.type == ModContent.ItemType<VoidEcho>())
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
            Player player = Projectile.owner.ToPlayer();
            Vector2 drawpos = Projectile.owner.ToPlayer().Center + player.gfxOffY * Vector2.UnitY + new Vector2(0, -44);
            Vector2 ep = Vector2.Zero;
            float fs = (float)(1 + Math.Cos(counter / 3) * 0.1);
            SpriteBatch sb = Main.spriteBatch;

            SpriteEffects ef = SpriteEffects.None;
            if (!right)
            {
                ef = SpriteEffects.FlipHorizontally;
            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;
            Main.spriteBatch.Draw(tx, drawpos + ep * 0.4f - Main.screenPosition, null, new Color(230, 150, 250) * 0.6f, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, 0.8f * fs, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D part1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidEchoProj/part1").Value;
            Texture2D part2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidEchoProj/part2").Value;
            Texture2D mark = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidEchoProj/VoidEcho").Value;


            int scl = (int)(230 + 25 * Math.Cos(counter / 6));
            sb.Draw(part1, drawpos + ep * 0.4f - Main.screenPosition, null, new Color(scl, scl, scl), 0, part1.Size() / 2, Projectile.scale, SpriteEffects.None, 0);


            sb.Draw(part2, drawpos + ep * 0.4f - Main.screenPosition, null, Color.White, counter * 0.06f, part2.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            sb.Draw(mark, Projectile.owner.ToPlayer().Center - Main.screenPosition - new Vector2(0, 38), Utilities.Util.GetCutTexRect(mark, 8, (int)(counter / 4) % 8, false), Color.White, 0, new Vector2(57, 43), Projectile.scale, ef, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }


}
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class HolyBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 3000;

        }
        public float counter = 0;
        public int drawcount = 0;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 80;

        }
        float opc = 1;
        public override void AI()
        {
            if (counter == 0)
            {
                SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/angel_blast1"), Projectile.Center);
            }
            counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (counter > 60)
            {
                opc -= 1f / 20f;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 900, targetHitbox, 24);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            drawcount++;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D warn = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/vlbw").Value;

            spriteBatch.Draw(warn, Projectile.Center - Main.screenPosition, null, Color.Yellow * opc, Projectile.rotation, warn.Size() / 2 * new Vector2(0, 1), new Vector2(10, 1.2f) * Projectile.scale * 1.46f * new Vector2(1, opc), SpriteEffects.None, 0);
            spriteBatch.Draw(warn, Projectile.Center - Main.screenPosition, null, ((drawcount / 2) % 2 == 0 ? Color.White : Color.Yellow) * opc, Projectile.rotation, warn.Size() / 2 * new Vector2(0, 1), new Vector2(10, 1) * Projectile.scale * 1.46f * new Vector2(1, opc), SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

}
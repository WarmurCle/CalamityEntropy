using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.AbyssalWraithProjs
{
    
    public class VoidFeather : ModProjectile
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
            Projectile.timeLeft = 260;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;
        
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (((int)Projectile.ai[1]).ToNPC().active && ((int)Projectile.ai[1]).ToNPC().HasValidTarget)
            {
                Player target = ((int)Projectile.ai[1]).ToNPC().target.ToPlayer();
                if (Projectile.timeLeft > 190 && Projectile.timeLeft < 240)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.24f;
                    Projectile.velocity *= 0.998f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            Util.Util.DrawAfterimage(t, Projectile.Entropy().odp, Projectile.Entropy().odr);
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

}
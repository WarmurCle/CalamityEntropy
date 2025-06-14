using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class WelkinFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 0.16f;
            Projectile.timeLeft = 340;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;
        bool c = true;
        public NPC homingTarget { get { if (Projectile.ai[1] < 0) { return null; } else { return ((int)(Projectile.ai[1])).ToNPC(); } } }
        public override void AI()
        {
            if (c)
            {
                c = false;
                Projectile.ai[1] = -1;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (homingTarget != null && homingTarget.active)
            {
                NPC target = homingTarget;
                Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.34f;
                Projectile.velocity *= 0.98f;

            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

}
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class NetherRiftCrack : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 56;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Utilities.Util.randomRot();
            }
            Projectile.Opacity = Projectile.timeLeft / 60f;
            Projectile.ai[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D t = Utilities.Util.getExtraTex("Cracks");
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 255) * Projectile.Opacity, Projectile.rotation, t.Size() / 2f, 3.6f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
            return false;
        }
    }

}
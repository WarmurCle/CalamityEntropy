using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class CommonExplotion : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.getDistance(projHitbox.Center.ToVector2(), targetHitbox.Center.ToVector2()) < Projectile.ai[0];
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }

    public class CommonExplotionFriendly : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.getDistance(projHitbox.Center.ToVector2(), targetHitbox.Center.ToVector2()) < Projectile.ai[0];
        }
    }
}
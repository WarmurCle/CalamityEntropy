using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.BNE
{
    public class Echo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 80;
            Projectile.light = 1;
            Projectile.MaxUpdates = 4;
        }
        public override void AI()
        {
            EParticle.spawnNew(new EchoCircle(), Projectile.Center, Projectile.velocity, Color.White, 1, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }


    }

}
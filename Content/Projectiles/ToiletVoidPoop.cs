using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class ToiletVoidPoop : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 1000;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.08f;
            Projectile.velocity.X *= 0.96f;
            Projectile.velocity.Y += 0.74f;
        }
        public override void OnKill(int timeLeft)
        {
            EParticle.spawnNew(new EXPLOSIONCOSMIC(), Projectile.Center + new Vector2(0, -38), Vector2.Zero, Color.White, 2, 1, true, BlendState.NonPremultiplied, 0);
            Util.Util.PlaySound("explosion", Main.rand.NextFloat(0.6f, 1.4f), Projectile.Center, 8);
        }
    }

}
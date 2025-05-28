using CalamityEntropy.Content.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            Projectile.ArmorPenetration = 100;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.08f;
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.56f;
        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.Calamity().GeneralScreenShakePower += Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f) * 2;
            EParticle.NewParticle(new EXPLOSIONCOSMIC(), Projectile.Center + new Vector2(0, -30), Vector2.Zero, Color.White, 1, 1, true, BlendState.NonPremultiplied, 0);
            CEUtils.PlaySound("explosion", Main.rand.NextFloat(0.6f, 1.4f), Projectile.Center, 8);
        }
    }

}
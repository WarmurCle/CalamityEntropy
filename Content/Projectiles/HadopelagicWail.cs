using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class HadopelagicWail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 230;
            Projectile.height = 230;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 36;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                EParticle.NewParticle(new HadLine(), Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 120, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
                EParticle.NewParticle(new HadLine(), Projectile.Center + Projectile.velocity.RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 120, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());

            }
            Projectile.ai[0]++;
            EParticle.NewParticle(new HadCircle(), Projectile.Center, Vector2.Zero, Color.White, 0.6f, 0, true, BlendState.Additive, Projectile.velocity.ToRotation());

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<LifeOppress>(600);
            EParticle.NewParticle(new HadCircle2(), target.Center, Vector2.Zero, new Color(170, 170, 255), 0, 0, true, BlendState.Additive, 0);
            EParticle.NewParticle(new HadCircle2(), target.Center, Vector2.Zero, new Color(170, 170, 255), 0, 0, true, BlendState.Additive, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }


}
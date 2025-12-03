using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class GoozmaStarShot : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.light = 1;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft == 2)
            {
                if (this.ShooterModProjectile is EntropyBookHeldProjectile mp)
                {
                    NPC target = Projectile.FindTargetWithinRange(2400);
                    mp.ShootSingleProjectile(mp.getShootProjectileType(), Projectile.Center, (target == null ? Projectile.velocity : (target.Center - Projectile.Center)), 1);
                }
            }

            CalamityMod.Particles.Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.rotation.ToRotationVector2() * -20 + Projectile.velocity, Main.DiscoColor, 18, Main.rand.NextFloat(0.3f, 0.5f), 1f, 0.012f, true, 0.01f, true);
            GeneralParticleHandler.SpawnParticle(smokeGlow);

            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float r = CEUtils.randomRot();
            EParticle.spawnNew(new AbyssalLine() { lx = 0.4f, xadd = 0.4f }, Projectile.Center, Vector2.Zero, Color.LightBlue, 1, 1, true, BlendState.Additive, r);
            EParticle.spawnNew(new AbyssalLine() { lx = 0.4f, xadd = 0.4f }, Projectile.Center, Vector2.Zero, Color.LightBlue, 1, 1, true, BlendState.Additive, r + MathHelper.PiOver2);
            for (int i = 0; i < Main.rand.Next(2); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CEUtils.randomVec(24), ModContent.ProjectileType<RainbowRocket>(), Projectile.damage / 6, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CEUtils.randomVec(24), ModContent.ProjectileType<PartySparkle>(), Projectile.damage / 16, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CEUtils.getExtraTex("StarTexture");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, 4 * Main.GlobalTimeWrappedHourly, tex.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, -1 * Main.GlobalTimeWrappedHourly, tex.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, -2 * Main.GlobalTimeWrappedHourly, tex.Size() / 2, Projectile.scale * 0.44f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
            return false;
        }
    }


}
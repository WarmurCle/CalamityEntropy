using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Particles.CalamityPorts;
using CalamityMod.Projectiles.Magic;
using InnoVault.PRT;
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

            //HeavySmokeCal Configure是Calamity原构造顺序,跟EParticle统一尾参不是一回事
            PRTLoader.NewParticle<PRT_HeavySmokeCal>(Projectile.Center, Projectile.rotation.ToRotationVector2() * -20 + Projectile.velocity, Main.DiscoColor, Main.rand.NextFloat(0.3f, 0.5f)).Configure(1f, 18, 0.012f, true, 0.01f, true);

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
            //AbyssalLine带lifetime的Configure是CalamityPorts签名
            var __prt = PRTLoader.NewParticle<PRT_AbyssalLine>(Projectile.Center, Vector2.Zero, Color.LightBlue, 1).Configure(1, true, PRTDrawModeEnum.AdditiveBlend, r);
            __prt.lx = 1.4f;
            __prt.xadd = 0.6f;
            var __prt2 = PRTLoader.NewParticle<PRT_AbyssalLine>(Projectile.Center, Vector2.Zero, Color.LightBlue, 1).Configure(1, true, PRTDrawModeEnum.AdditiveBlend, r + MathHelper.PiOver2);
            __prt2.lx = 1.4f;
            __prt2.xadd = 0.6f;
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CEUtils.randomVec(24), ModContent.ProjectileType<GRainbowRocket>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CEUtils.randomVec(24), ModContent.ProjectileType<PartySparkle>(), Projectile.damage / 8, Projectile.knockBack, Projectile.owner);
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
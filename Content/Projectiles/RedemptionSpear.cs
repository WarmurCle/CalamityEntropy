using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class RedemptionSpear : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 15;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Center.getRectCentered(52 * Projectile.scale, 52 * Projectile.scale);
        }
        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 top = Projectile.Center;
            Vector2 sparkVelocity2 = Projectile.velocity * -0.1f;
            int sparkLifetime2 = 4;
            float sparkScale2 = Main.rand.NextFloat(0.6f, 1f);
            Color sparkColor2 = Color.Lerp(Color.OrangeRed, Color.Gold, Main.rand.NextFloat(0, 1));
            LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
            GeneralParticleHandler.SpawnParticle(spark);
            CEUtils.AddLight(Projectile.Center, Color.LightGoldenrodYellow);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, this.color, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            CEUtils.DrawGlow(Projectile.Center, new Color(this.color.R * 0.6f, this.color.G * 0.5f, this.color.B * 0.05f) * 0.3f, Projectile.scale * 4);
            return false;
        }
        public override Color baseColor => Color.White;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            EParticle.NewParticle(new StrikeParticle(), Projectile.Center - Projectile.velocity * 7, Projectile.velocity * 3, color, Projectile.scale * 0.6f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            EParticle.NewParticle(new RedemptionSpearParticle(), Projectile.Center, Projectile.velocity, Color.White, Projectile.scale, 1, true, BlendState.AlphaBlend, Projectile.rotation);
        }
    }
}
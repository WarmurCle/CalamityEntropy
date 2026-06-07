using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Effects;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
        public TrailParticle trail = null;
        public override void AI()
        {
            base.AI();
            if(trail == null)
            {
                trail = new TrailParticle() { maxLength = 8, SameAlpha = true };
                trail.ShouldDraw = false;
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, Color.Yellow, 2, 1, true, BlendState.Additive);
            }
            trail.AddPoint(Projectile.Center + Projectile.velocity + Projectile.velocity.normalize() * 12);
            trail.Lifetime = 12;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 top = Projectile.Center;
            Vector2 sparkVelocity2 = Projectile.velocity * -0.03f;
            int sparkLifetime2 = 6;
            float sparkScale2 = Main.rand.NextFloat(1f, 1.2f);
            Color sparkColor2 = Color.Lerp(Color.OrangeRed, Color.Gold, Main.rand.NextFloat(0, 1));
            LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
            GeneralParticleHandler.SpawnParticle(spark);
            CEUtils.AddLight(Projectile.Center, Color.LightGoldenrodYellow);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.spriteBatch.UseAdditive();
            if (trail != null)
                trail.Draw();
            for(float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + i.ToRotationVector2() * 4, null, this.color, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + i.ToRotationVector2() * 4, null, this.color, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, this.color, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            CEUtils.DrawGlow(Projectile.Center, new Color(this.color.R * 0.4f, this.color.G * 0.36f, this.color.B * 0.05f) * 0.3f, Projectile.scale * 4);
            return false;
        }
        public override Color baseColor => Color.White;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            EParticle.NewParticle(new StrikeParticle(), Projectile.Center - Projectile.velocity * 7, Projectile.velocity * 3, color, Projectile.scale * 0.6f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
            for (int i = 0; i < 24; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), -Projectile.velocity);
                dust.scale = Main.rand.NextFloat(2f, 2.5f);
                dust.velocity = (new Vector2(35, 35).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.7f)) * Main.rand.NextFloat(0.4f, 1f);
                dust.noGravity = false;
                dust.color = Color.Goldenrod;
                dust.fadeIn = 2f;
            }
            SoundEngine.PlaySound(SoundID.Item96 with { Pitch = 0.6f, Volume = 1f}, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            EParticle.NewParticle(new RedemptionSpearParticle(), Projectile.Center, Projectile.velocity, Color.White, Projectile.scale, 1, true, BlendState.AlphaBlend, Projectile.rotation);
        }
    }
}
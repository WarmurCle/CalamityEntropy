using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class UpdraftBullet : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.2f;
            Projectile.timeLeft = 200;
            Projectile.ArmorPenetration = 9;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Center.getRectCentered(46 * Projectile.scale, 46 * Projectile.scale);
        }
        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.GameUpdateCount % 3 == 0)
            {
                EParticle.NewParticle(new WindParticle() { v1 = 9, v2 = 3, r = Projectile.rotation + MathHelper.Pi, dir = -1 }, Projectile.Center + Projectile.velocity * 4, Vector2.Zero, new Color(240, 245, 255), 1f, 1, true, BlendState.Additive, Projectile.rotation + MathHelper.Pi);
                EParticle.NewParticle(new WindParticle() { v1 = 9, v2 = 3, r = Projectile.rotation + MathHelper.Pi, dir = 1 }, Projectile.Center + Projectile.velocity * 4, Vector2.Zero, new Color(240, 245, 255), 1f, 1, true, BlendState.Additive, Projectile.rotation + MathHelper.Pi);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Texture2D tex = Projectile.GetTexture();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(200, tex.Height / 2), Projectile.scale * 0.4f, SpriteEffects.None, 0);

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override Color baseColor => Color.LightBlue;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            for (int i = 0; i < 10; i++)
            {
                EParticle.NewParticle(new ULineParticle(4, 0.8f, 0.85f, 0.032f), target.Center + new Vector2(Main.rand.NextFloat(0, target.width) - (target.width / 2f), Main.rand.NextFloat(0, target.height) - (target.height / 2f)), new Vector2(0, -34), Color.Lerp(this.color, Color.LightBlue, 0.5f), 1, 1, true, BlendState.AlphaBlend, 0);
            }
            if (target.velocity.Length() > 0.1f && target.type != NPCID.WallofFlesh)
            {
                target.velocity += Projectile.velocity * (0.6f + 0.4f * target.knockBackResist);
            }
            EParticle.NewParticle(new HadCircle2(), target.Center, Vector2.Zero, new Color(170, 170, 255), 0, 0, true, BlendState.Additive, 0);
            EParticle.NewParticle(new WindParticle(), Projectile.Center, Vector2.Zero, new Color(240, 245, 255), 2, 1, true, BlendState.Additive, CEUtils.randomRot());
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 3; i++)
            {
                EParticle.NewParticle(new WindParticle(), Projectile.Center, Vector2.Zero, new Color(240, 245, 255), 2, 1, true, BlendState.Additive, CEUtils.randomRot());
            }
            EParticle.NewParticle(new UpdraftParticle(), Projectile.Center, Projectile.velocity, Color.White, Projectile.scale * 0.4f, 1, true, BlendState.Additive, Projectile.rotation);
        }
    }

}
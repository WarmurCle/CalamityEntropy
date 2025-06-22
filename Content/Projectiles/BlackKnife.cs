using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class BlackKnife : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.penetrate = 1;
        }
        public int counter = 0;
        public float j = 1;
        public Color clr = Color.White;
        public int white = 0;
        public int w = 0;
        public override void AI()
        {
            NPC target = ((int)(Projectile.ai[0])).ToNPC();
            if(!target.active || target.dontTakeDamage)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.rotation = Projectile.ai[1] + MathHelper.Pi;
                white--;
                counter++;
                float d = (target.width + target.height) * 0.5f + 200;
                w = (int)d;
                if(counter > 60)
                {
                    clr = Color.White;
                }
                else
                {
                    clr = Color.Lerp(Color.White, Color.Red, counter / 60f);
                }
                if(counter == 60)
                {
                    clr = Color.White;
                    white = 5;
                }
                if(counter == 65)
                {
                    CEUtils.PlaySound("Dizzy", 1, Projectile.Center);
                    EParticle.spawnNew(new BlackKnifeParticle(), Projectile.Center, Projectile.rotation.ToRotationVector2() * 260, Color.Red, Projectile.scale * 0.8f, 1, true, BlendState.AlphaBlend, Projectile.rotation);
                    EParticle.spawnNew(new BlackKnifeSlash(), target.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Projectile.ai[1], 6);
                }
                if(counter > 70)
                {
                    Projectile.Kill();
                }
                Projectile.Center = target.Center + Projectile.ai[1].ToRotationVector2() * d * j;
            }

        }
        public override bool? CanHitNPC(NPC target)
        {
            return counter < 65 ? false : null;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center + Projectile.ai[1].ToRotationVector2() * w, Projectile.Center - Projectile.ai[1].ToRotationVector2() * w, targetHitbox, 56);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (counter > 64)
            {
                return false;
            }
            Texture2D tex = white > 0 ? this.getTextureGlow() : Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, clr * Projectile.Opacity, Projectile.rotation, tex.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None);
            return false;
        }
    }
}
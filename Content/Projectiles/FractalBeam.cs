using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class FractalBeam : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, (Vector2)(Projectile.Center + CEUtils.normalize(Projectile.velocity) * 1200), targetHitbox, 16);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * 1200, 16, DelegateMethods.CutTiles);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;

                Vector2 norl = Projectile.velocity.normalize();
                float sengs = 3;
                var color = Color.SkyBlue;

                for (int j = 0; j < 120; j++)
                {
                    var spark = new HeavenfallStar();
                    EParticle.NewParticle(spark, Projectile.Center, norl * (0.1f + j * 0.34f) * sengs, color, Main.rand.NextFloat(0.6f, 1.3f), 1, true, BlendState.Additive, norl.ToRotation(), 24);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
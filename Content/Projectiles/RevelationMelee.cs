using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class RevelationMelee : ModProjectile
    {
        List<Texture2D> jtx = new List<Texture2D>();
        float angle;
        int frame = 0;
        int fc = 0;
        float ca;
        bool nts = true;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 30;
            for (int i = 0; i < 9; i++)
            {
                jtx.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/RVM/s" + (i + 1).ToString()).Value);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (nts)
            {
                Projectile.ai[0] = -8;
                angle = Projectile.velocity.ToRotation();
                Projectile.ai[1] = -0.6f;
                Projectile.rotation = angle - MathHelper.ToRadians(-45);
                ca = angle;
                nts = false;
            }
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Main.player[Projectile.owner].Center, (ca.ToRotationVector2() * 26).RotatedBy(MathHelper.ToRadians(0)), ModContent.ProjectileType<Rvshoot2>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Main.player[Projectile.owner].Center, (ca.ToRotationVector2() * 26).RotatedBy(MathHelper.ToRadians(32)), ModContent.ProjectileType<Rvshoot2>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Main.player[Projectile.owner].Center, (ca.ToRotationVector2() * 26).RotatedBy(MathHelper.ToRadians(-32)), ModContent.ProjectileType<Rvshoot2>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
                    for (int i = 0; i < Projectile.owner.ToPlayer().Entropy().WeaponBoost; i++)
                    {
                        Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromAI(), Main.player[Projectile.owner].Center, (ca.ToRotationVector2() * 26).RotatedByRandom(0.4f), ModContent.ProjectileType<Rvshoot2>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);

                    }
                }
            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.rotation += Projectile.ai[1];
            }
            else
            {
                Projectile.rotation -= Projectile.ai[1];
            }
            if (Projectile.ai[0] < 6)
            {
                Projectile.ai[1] += 0.075f;
            }
            else
            {
                if (Projectile.velocity.X > 0)
                {
                    Projectile.ai[1] *= 0.86f;
                }
                else
                {
                    Projectile.ai[1] *= 0.92f;
                }
            }
            Projectile.Center = Main.player[Projectile.owner].Center + Projectile.rotation.ToRotationVector2() * 120;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 6)
            {
                fc++;
                if (fc >= 3)
                {
                    fc = 0;
                    frame++;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects se = SpriteEffects.None;
            float jr = MathHelper.ToRadians(45);
            float z = 0;
            if (Projectile.velocity.X < 0)
            {
                se = SpriteEffects.FlipHorizontally;
                jr = MathHelper.ToRadians(-45 + 180);
                z = MathHelper.ToRadians(180);
            }
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/RevelationMelee").Value;

            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + jr, new Vector2(tx.Width / 2, tx.Height / 2), new Vector2(1, 1), se, 0);
            if (Projectile.ai[0] > 6)
            {
                if (frame < jtx.Count)
                {
                    tx = jtx[frame];
                    Main.spriteBatch.Draw(tx, Main.player[Projectile.owner].Center + ca.ToRotationVector2() * 80 - Main.screenPosition, null, Color.White, ca + z, new Vector2(tx.Width / 2, tx.Height / 2), new Vector2(1, 1), se, 0);

                }
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.ai[0] > 0 && projHitbox.Intersects(targetHitbox);
        }
    }

}
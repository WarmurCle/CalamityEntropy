using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Prophet
{
    public class RuneCrystalTop : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 20;
            Projectile.light = 1;
            Projectile.timeLeft = 120 + 64;
        }
        public List<Vector2> segs = new List<Vector2>();
        public Vector2 orgPos = Vector2.Zero;
        public override void AI()
        {
            if (orgPos == Vector2.Zero)
            {
                orgPos = Projectile.Center;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 64)
            {
                segs.Add(Projectile.Center);
                Projectile.Center += Projectile.velocity.SafeNormalize(Vector2.One) * 16;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulDisorder>(), 5 * 60);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (orgPos == Vector2.Zero)
            {
                return false;
            }
            return Utilities.Util.LineThroughRect(orgPos, Projectile.Center, targetHitbox, (int)(20 * Projectile.scale));
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D t1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Prophet/RuneCrystal").Value;
            Texture2D t2 = Projectile.GetTexture();
            if (Projectile.timeLeft < 30)
            {
                lightColor *= Projectile.timeLeft / 30f;
            }
            foreach (var p in segs)
            {
                Main.EntitySpriteDraw(t1, p - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation(), t1.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t2, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation(), t2.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

}
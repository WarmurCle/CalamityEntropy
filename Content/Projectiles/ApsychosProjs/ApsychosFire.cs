using CalamityEntropy.Content.Buffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.ApsychosProjs
{
    public class ApsychosFire : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 100000;
            Projectile.penetrate = 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
        public float scale = 0;
        public float p = 0;
        
        public override void AI()
        {
            p += 1 / Projectile.ai[0];
            if (p > 1)
                Projectile.Kill();
            scale = 1 + p * 0.2f;
            if (p > 0.65f)
                scale += (p - 0.65f) * 3;
            Projectile.velocity *= 0.96f;
        }
        public override bool? CanDamage()
        {
            return p < 0.95f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            float d = 0.5f;
            Color clr = new Color(240, 194, 20);
            float alpha = 1;
            if (p > 1 - d)
            {
                alpha = Utils.Remap(p, 1 - d, 1, 1, 0);
                clr = Color.Lerp(clr, Color.Black, Utils.Remap(p, 1 - d, 1, 0, 1)) * alpha;
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, clr * 0.6f, Main.GlobalTimeWrappedHourly * 16, tex.Size() * 0.5f, scale * Projectile.scale * 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, clr, Main.GlobalTimeWrappedHourly * 16, tex.Size() * 0.5f, scale * Projectile.scale * 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Main.GlobalTimeWrappedHourly * 16, tex.Size() * 0.5f, scale * Projectile.scale * 0.65f, SpriteEffects.None, 0);
            return false;
        }
    }


}

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class VoidImpact : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 116;
            Projectile.height = 116;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 2;
            Projectile.scale = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            oldPos.Add(Projectile.Center);
            if (oldPos.Count > 12)
            {
                oldPos.RemoveAt(0);
            }
        }

        public List<Vector2> oldPos = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            if (Projectile.timeLeft < 30)
            {
                lightColor *= ((float)Projectile.timeLeft / 30f);
            }
            float scale = 0;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            for (int i = 0; i < oldPos.Count; i++)
            {
                scale += 1f / oldPos.Count;
                Main.spriteBatch.Draw(tex, oldPos[i] - Main.screenPosition, null, lightColor * ((float)i / (float)oldPos.Count) * 0.6f, Projectile.rotation, tex.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0);

            return false;
        }
    }


}
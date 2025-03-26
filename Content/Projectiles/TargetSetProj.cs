using CalamityEntropy.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class TargetSetProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
        }
        public override void PostAI()
        {
            Projectile.damage = 1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }
            int id = target.whoAmI;
            if (target.realLife >= 0)
            {
                id = target.realLife;
            }
            EGlobalNPC.setFriendly(id, Projectile.owner);
            foreach (NPC n in Main.npc)
            {
                if (n.active && n.realLife == id)
                {
                    EGlobalNPC.setFriendly(n.whoAmI, Projectile.owner);
                }
            }
        }
        public override void AI()
        {
            Projectile.rotation += 0.16f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                lightColor *= ((float)Projectile.timeLeft / 30f);
            }
            Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StarTrail").Value;
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, lightColor * 0.6f, Projectile.velocity.ToRotation(), t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tx.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }


}
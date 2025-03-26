using CalamityEntropy.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AbyssDragonProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = 60;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 4;
        }
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 90, 3.6f, 1000, 16);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Util.Util.recordOldPosAndRots(Projectile, ref odp, ref odr, 12);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                lightColor *= ((float)Projectile.timeLeft / 30f);
            }
            Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
            Util.Util.DrawAfterimage(tx, odp, odr);
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tx.Size() / 2, Projectile.scale, (Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);
            return false;
        }
    }


}
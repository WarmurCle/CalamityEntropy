using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityEntropy.Projectiles;
using Terraria.Audio;
namespace CalamityEntropy.Projectiles.monument
{
    public class Vmwave : ModProjectile 
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 9;
            Projectile.penetrate = -1;
        }

        public override void AI(){
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx1 = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/monument/wave1").Value;
            Texture2D tx2 = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/monument/wave2").Value;
            Texture2D tx3 = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/monument/wave3").Value;
            Texture2D draw = tx1;
            if (Projectile.timeLeft < 7)
            {
                draw = tx2;
            }
            if (Projectile.timeLeft < 4)
            {
                draw = tx3;
            }
            if (Projectile.timeLeft < 1)
            {
                return false;
            }

            Main.spriteBatch.Draw(draw, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, draw.Width)/2, 1, SpriteEffects.None, 0);

            return false;
        }
    }
    

}
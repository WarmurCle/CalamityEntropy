using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Prophet
{
    public class ProphetVoidSpike : ModProjectile
    {
        public float length = 0;
        public float lengthAdd = 16f;
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 800;
        }
        public override void AI()
        {
            if (!((int)Projectile.ai[1]).ToNPC().active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = ((int)Projectile.ai[1]).ToNPC().Center;
            length += lengthAdd;
            lengthAdd *= 0.974f;
            if (Projectile.ai[0] == 44)
            {
                lengthAdd = 160;
            }
            if (Projectile.ai[0] < 44)
            {
                lengthAdd -= 0.46f;
            }
            if (Projectile.ai[0] > 56)
            {
                length *= 0.75f;
                length -= 1;
                Projectile.Opacity *= 0.9f;
            }
            if (Projectile.ai[0] == 86)
            {
                Projectile.Kill();
            }
            Projectile.ai[0]++;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulDisorder>(), 5 * 60);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * length, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public void Draw()
        {
            Main.EntitySpriteDraw(Projectile.getTexture(), Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.velocity.ToRotation(), new Vector2(0, 12), new Vector2(length / 90f, 1), SpriteEffects.None, 0);
        }

    }

}
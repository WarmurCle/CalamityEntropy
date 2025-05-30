﻿using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class LightningBolt : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 12;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 80;
        }

        public override void AI()
        {
            Vector2 ps = Projectile.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
            odp.Add(ps);
            if (odp.Count > 16)
            {
                odp.RemoveAt(0);
            }
            foreach (Vector2 p in odp)
            {
                Lighting.AddLight(p, 1, 1, 1);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(odp[0], Projectile.Center, targetHitbox, (int)(20 * Projectile.scale), (int)(16 * Projectile.scale));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (odp.Count > 1)
            {
                for (int i = 1; i < odp.Count; i++)
                {
                    CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, odp[i - 1], odp[i], Color.White * 0.4f, 20 * Projectile.scale);

                    CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, odp[i - 1], odp[i], Color.White, 12 * Projectile.scale, (int)(8 * Projectile.scale));

                }
            }
            return false;
        }
    }


}
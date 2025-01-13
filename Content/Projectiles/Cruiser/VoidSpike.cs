using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{
    
    public class VoidSpike: ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 520;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;
        public override void AI()
        {
            if (setv)
            {
                setv = false;
                Projectile.velocity *= 0.5f;
            }
            odp.Add(Projectile.Center);
            if (odp.Count > 24)
            {
                odp.RemoveAt(0);
            }

            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }
            for (int i = 0; i < 2; i++)
            {
                Particle p = new Particle();
                p.alpha = 0.22f;
                p.position = Projectile.Center;
                p.velocity = new Vector2(0.3f, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2);
                VoidParticles.particles.Add(p);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Cruiser/VoidSpike").Value;
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2, 2, SpriteEffects.None, 0);

            return false;
        }
    }

}
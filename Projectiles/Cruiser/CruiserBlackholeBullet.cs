using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
namespace CalamityEntropy.Projectiles.Cruiser
{
    
    public class CruiserBlackholeBullet: ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;

        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkProj.Add(Projectile);
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
            Projectile.timeLeft = 1000;
            Projectile.extraUpdates = 1;
        }
        public float ap = 0;
        public override void AI(){
            Particle p = new Particle();
            p.alpha = 0.5f;
            p.position = Projectile.Center;
            VoidParticles.particles.Add(p);
            Projectile.rotation = (new Vector2(Projectile.ai[1], Projectile.ai[2]) - Projectile.position).ToRotation();
            Projectile.velocity += Projectile.rotation.ToRotationVector2() * 0.08f;
            if (Util.Util.getDistance(new Vector2(Projectile.ai[1], Projectile.ai[2]), Projectile.Center) < Projectile.velocity.Length() + 20)
            {
                Projectile.Kill();
            }
            if (ap < 1)
            {
                ap += 0.01f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, Projectile.Center, new Vector2(Projectile.ai[1], Projectile.ai[2]), Color.Purple * ap * 0.45f, 5 * ap);
            return false;
        }
    }

}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    
    public class VoidStarF: ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkProj.Add(Projectile);
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;
        public override void AI(){
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
            Projectile.velocity *= 0.999f;
            if (Projectile.timeLeft < 380)
            {
                NPC target = Projectile.FindTargetWithinRange(4000, false);
                if (target != null)
                {
                    Projectile.velocity *= 0.99f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 0.4f;
                    if (Util.Util.getDistance(Projectile.Center, target.Center) < 180)
                    {
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((target.Center - Projectile.Center).ToRotation());
                    }
                }
            }
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }

}
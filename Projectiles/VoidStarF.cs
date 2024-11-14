using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod;
using Terraria.DataStructures;
namespace CalamityEntropy.Projectiles
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 160;
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
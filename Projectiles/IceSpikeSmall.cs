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
namespace CalamityEntropy.Projectiles
{
    public class IceSpikeSmall: ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
        }

        public override void AI(){

            Projectile.velocity.Y += 0.4f;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

    }
    

}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{
    
    public class VoidStar: ModProjectile
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
            Projectile.width = 16;
            Projectile.height = 16;
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
        
        public override void AI(){
            Projectile.ai[0]++;
            if (Projectile.ai[2] == 1 && Projectile.ai[0] < 60)
            {
                return;
            }
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
            Projectile.velocity *= 0.996f;
            
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            if (Projectile.ai[2] == 1 && Projectile.ai[0] < 60)
            {
                return false;
            }
            return base.ShouldUpdatePosition();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] == 1)
            {
                if (Projectile.ai[0] < 60)
                {
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, Projectile.Center, Projectile.Center + Projectile.velocity * 1000, Color.Purple * (0.8f * Projectile.ai[0] / 60f), 2);
                }
            }
            return false;
        }
    }

}
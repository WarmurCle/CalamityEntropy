using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SilenceHook : ModProjectile {
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
            Projectile.DamageType = CUtil.rougeDC;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 2000;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public int hNPC = -1;
        public Vector2 offset;
        public bool backing = false;
        public override void AI(){
            Projectile sl = ((int)Projectile.ai[1]).ToProj();
            if (Util.Util.getDistance(sl.Center, Projectile.Center) > 1100)
            {
                Projectile.Kill();
            }
            if (hNPC != -1)
            { 
                if (hNPC.ToNPC().active == false)
                {
                    hNPC = -1;
                    backing = true;
                }
                else
                {
                    Projectile.Center = hNPC.ToNPC().Center + offset;
                    Projectile.velocity = Vector2.Zero;
                    sl.velocity += (Projectile.Center - sl.Center).SafeNormalize(Vector2.Zero) * 2f;
                    sl.velocity *= 0.96f;
                }
            }
            else
            {
                if (Util.Util.getDistance(sl.Center, Projectile.Center) > 960)
                {
                    backing = true;

                }
                if (backing)
                {
                    Projectile.velocity = (sl.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 25;
                    if (Util.Util.getDistance(sl.Center, Projectile.Center) < 30)
                    {
                        Projectile.Kill();
                    }
                }
            }
            
            if (!sl.active)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile sl = ((int)Projectile.ai[1]).ToProj();
            if (hNPC == -1)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/chain2"), Projectile.Center);
                hNPC = target.whoAmI;
                offset = Projectile.Center - target.Center;
            }    
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

    }

}
using CalamityEntropy.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class Detector : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        bool back = false;
        public override void AI()
        {
            Projectile.pushByOther(1.4f);
            if (back)
            {
                Projectile.velocity *= 0.86f;
                Projectile.velocity += (Projectile.getOwner().Center - Projectile.Center).normalize() * 6;
                if (Utilities.Util.getDistance(Projectile.Center, Projectile.getOwner().Center) < Projectile.velocity.Length() * 1.2f)
                {
                    Projectile.Kill();
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                NPC target = Projectile.FindTargetWithinRange(3000);
                if (target != null)
                {
                    Projectile.velocity *= 0.99f;
                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * 0.7f;
                    if (Utilities.Util.getDistance(Projectile.Center, target.Center) < 500)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).normalize() * 12, 88, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            p.ToProj().usesLocalNPCImmunity = true;
                            p.ToProj().localNPCHitCooldown = 16;
                        }
                        SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                        back = true;
                    }
                    Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
                }
                else
                {
                    if (Projectile.getOwner().Distance(Projectile.Center) > 120)
                    {
                        Projectile.velocity *= 0.99f;
                        Projectile.velocity += (Projectile.getOwner().Center - Projectile.Center).normalize() * 0.6f;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }


}
using Terraria;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopRedProjectile : PoopProj
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Util.Util.PlaySound("firedeath hiss", 1, Projectile.Center);
        }
        public override bool BreakWhenHitNPC => false;
        public override bool damageNPCAfterLand => true;
    }

}
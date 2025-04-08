using CalamityEntropy.Content.Items.Books;
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Util
{
    public static class CUtil
    {
        public static DamageClass rogueDC;
        public static void load()
        {
            rogueDC = ModContent.GetInstance<RogueDamageClass>();
        }
        public static bool hasEffect<T>(this Projectile p) where T : EBookProjectileEffect
        {
            if (p.ModProjectile is EBookBaseProjectile ep)
            {
                foreach (var ef in ep.ProjectileEffects)
                {
                    if (ef.GetType() == typeof(T))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

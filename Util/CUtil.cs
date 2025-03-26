using CalamityMod;
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
    }
}

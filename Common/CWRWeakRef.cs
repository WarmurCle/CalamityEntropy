using CalamityOverhaul;
using CalamityOverhaul.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    internal static partial class CWRWeakRef
    {
        [JITWhenModsEnabled("CalamityOverhaul")]
        internal static class CWRRef
        {
            public static float GetPlayersPressure(Player plr)
            {
                return plr.GetModPlayer<CWRPlayer>().PressureIncrease;
            }
            public static void SetCartridge(Item item, int m)
            {
                //item.CWR().HasCartridgeHolder = true;
                //item.CWR().AmmoCapacity = m;
                //CWRItems已经改名为CWRItem，模组更新会导致适配问题，所以这里的功能暂时注释掉
            }
        }
    }
}

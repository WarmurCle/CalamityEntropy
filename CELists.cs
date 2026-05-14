using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Items.Weapons.Fractal;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public static class CELists
    {
        public static List<string> tooltipNameUpList = new() { "zh-Hans" };
        public static List<int> SpecialTaintedEnchantmentList;
        public static void Load()
        {
            int I<T>() where T : ModItem
            {
                return ModContent.ItemType<T>();
            }
            SpecialTaintedEnchantmentList = new()
            {
                I<ShatteredFractal>(),
                I<WelkinFractal>(),
                I<BrilliantFractal>(),
                I<AbyssFractal>(),
                I<StarlitFractal>(),
                I<ElementalFractal>(),
                I<SpiritFractal>(),
                I<VoidFractal>(),
                I<FinalFractal>(),
                I<AshesSword>(),
                I<MoonlightSword>(),
                I<TrueMoonlightSword>(),
                I<Voidshade>(),
                I<RuneSong>()
            };
        }
        public static void Unload()
        {
            SpecialTaintedEnchantmentList = null;
        }
    }
}

using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Items.Weapons.Fractal;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Projectiles.Melee;
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
            int P<T>() where T : ModProjectile
            {
                return ModContent.ProjectileType<T>();
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
            SoyMilkProjectileBlacklist = new()
            {
                P<RailPulseBowProjectile>(),
                P<GhostdomWhisperHoldout>(),
                P<HadopelagicEchoIIProj>(),
                P<SolarStormHeld>(),
                P<HellkiteHoldout>(),
                P<GrandGuardianHoldout>(),
                P<MajesticGuardHoldout>(),
                P<GrandDadHoldout>(),
                P<EarthHoldout>(),
                P<BatteringRamProj>(),
                P<CinderConvergencerHoldout>()
            };
        }
        public static List<int> SoyMilkProjectileBlacklist;
        public static void Unload()
        {
            SpecialTaintedEnchantmentList = null;
            SoyMilkProjectileBlacklist = null;
        }
    }
}

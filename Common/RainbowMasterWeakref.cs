using System.Collections.Generic;
using Terraria.ModLoader;
using ColouredModsRelics.Common.Mods;
using ColouredModsRelics.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Common
{

    internal static partial class RMWeakRef
    {
        public static List<string> relics = new List<string>() { "AquaticScourgeRelic", "AstrumAureusRelic", "AstrumDeusRelic", "BrimstoneElementalRelic", "CalamitasCloneRelic", "CalamitasRelic", "CeaselessVoidRelic", "CrabulonRelic", "CragmawMireRelic", "CryogenRelic", "DesertScourgeRelic", "DevourerOfGodsRelic", "DraedonRelic", "DragonfollyRelic", "GiantClamRelic", "GreatSandSharkRelic", "HiveMindRelic", "LeviathanAnahitaRelic", "MaulerRelic", "NuclearTerrorRelic", "OldDukeRelic", "PerforatorsRelic", "PlaguebringerGoliathRelic", "PolterghastRelic", "ProfanedGuardiansRelic", "ProvidenceRelic", "RavagerRelic", "SignusRelic", "SlimeGodRelic", "StormWeaverRelic", "YharonRelic" };
        public static List<string> erelics = new List<string>() { "NihilityTwinRelicTile", "CruiserRelicTile" };
        public static List<string> clamityRelic = new List<string>() { "Clamity/Content/Bosses/Clamitas/Drop/ClamitasRelicTile", "Clamity/Content/Bosses/WoB/Drop/WoBRelicTile" };
        [JITWhenModsEnabled("ColouredModsRelics")]
        internal static class CMRWeakRef
        {
            public static void setTexs()
            {
                foreach (string s in relics)
                {
                    RainbowLoader.RainbowRelicIntances[RainbowLoader.ModID["CalamityEntropy"]].ColoredRelicTileAssets[ModContent.GetInstance<CalamityMod.CalamityMod>().Find<ModTile>(s).Type] = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/BossRelics/" + s);
                    if (ModLoader.TryGetMod("Clamity", out Mod _))
                    {
                        RainbowLoader.RainbowRelicIntances[RainbowLoader.ModID["Clamity"]].ColoredRelicTileAssets[ModContent.GetInstance<CalamityMod.CalamityMod>().Find<ModTile>(s).Type] = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/BossRelics/" + s);
                    }
                }
                if (ModLoader.TryGetMod("Clamity", out Mod clam))
                {
                    foreach(string s in erelics)
                    {
                        RainbowLoader.RainbowRelicIntances[RainbowLoader.ModID["Clamity"]].ColoredRelicTileAssets[CalamityEntropy.Instance.Find<ModTile>(s).Type] = ModContent.Request<Texture2D>("CalamityEntropy/Content/Tiles/" + s);
                    }
                    foreach(string s in clamityRelic)
                    {
                        RainbowLoader.RainbowRelicIntances[RainbowLoader.ModID["CalamityEntropy"]].ColoredRelicTileAssets[clam.Find<ModTile>(s.Split("/")[s.Split("/").Length - 1]).Type] = ModContent.Request<Texture2D>(s);
                    }
                }
            }
        }
        public static void init()
        {
            ModLoader.TryGetMod("ColouredModsRelics", out mod);
        }
        public static Mod mod = null;
    }

    
}

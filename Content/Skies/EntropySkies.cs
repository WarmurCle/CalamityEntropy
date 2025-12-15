using CalamityEntropy.Content.Items.Books.BookMarks;
using Terraria.Graphics.Effects;

namespace CalamityEntropy.Content.Skies
{
    public static class EntropySkies
    {
        public static void setUpSkies()
        {
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:Cruiser"] = new Filter(new CrScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:Cruiser"] = new CrSky();
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:DimensionLens"] = new Filter(new TransScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:DimensionLens"] = new LlSky();
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:NihTwin"] = new Filter(new TransScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:NihTwin"] = new NihTwinSky();
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:VoidVortex"] = new Filter(new TransScreenShaderData("FilterMiniTower").UseColor(new Color(60, 30, 100)).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:VoidVortex"] = new VoidVortexSky();
            Terraria.Graphics.Effects.Filters.Scene["CalamityEntropy:Snowgrave"] = new Filter(new TransScreenShaderData("FilterMiniTower").UseColor(new Color(200, 200, 255)).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityEntropy:Snowgrave"] = new SnowgraveSky();

        }
    }
}

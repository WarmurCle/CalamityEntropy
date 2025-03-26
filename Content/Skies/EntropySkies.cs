using Microsoft.Xna.Framework;
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

        }
    }
}

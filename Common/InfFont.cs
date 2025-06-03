using InfernumMode.Assets.Fonts;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    internal static partial class InfFont
    {
        [JITWhenModsEnabled("InfernumMode")]
        internal static class InfernumFont
        {
            public static void SetFont()
            {
                try
                {
                    var fInfo = InfernumFontRegistry.BossIntroScreensFont.GetType().GetField("OtherFonts", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    var otherFonts = (Dictionary<GameCulture, DynamicSpriteFont>)fInfo.GetValue(InfernumFontRegistry.BossIntroScreensFont);
                    var ofs = otherFonts;
                    ofs[GameCulture.FromCultureName((GameCulture.CultureName)7)] = CalamityEntropy.Instance.Assets.Request<DynamicSpriteFont>("Assets/Fonts/BossIntroScreensFont", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    fInfo.SetValue(InfernumFontRegistry.BossIntroScreensFont, ofs);
                }
                catch { }
            }
        }
    }
}

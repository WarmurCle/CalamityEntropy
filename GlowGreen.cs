using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class GlowGreen : ModRarity
    {
        public override Color RarityColor => new Color(60, 255, 60);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}

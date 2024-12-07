using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Rarities
{
    public class GlowGreen : ModRarity
    {
        public override Color RarityColor => new Color(60, 255, 60);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}

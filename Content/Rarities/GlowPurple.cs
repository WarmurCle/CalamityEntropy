using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Rarities
{
    public class GlowPurple : ModRarity
    {
        public override Color RarityColor => new Color(190, 0, 255);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}

using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Rarities
{
    public class VoidPurple : ModRarity
    {
        public override Color RarityColor => new Color(106, 40, 190);

        public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
        {
            -2 => ModContent.RarityType<DarkBlue>(),
            -1 => ModContent.RarityType<Violet>(),
            1 => ModContent.RarityType<VoidPurple>(),
            2 => ModContent.RarityType<VoidPurple>(),
            _ => Type,
        };
    }
}

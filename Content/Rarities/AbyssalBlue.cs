using CalamityMod.Rarities;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Rarities
{
    public class AbyssalBlue : ModRarity
    {
        public override Color RarityColor => new Color(106, 40, 190);

        public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
        {
            -2 => ModContent.RarityType<BurnishedAuric>(),
            -1 => ModContent.RarityType<CalamityRed>(),
            1 => ModContent.RarityType<AbyssalBlue>(),
            2 => ModContent.RarityType<AbyssalBlue>(),
            _ => Type,
        };
    }
}

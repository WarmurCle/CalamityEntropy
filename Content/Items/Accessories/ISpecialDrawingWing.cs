using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public interface ISpecialDrawingWing
    {
        public int MaxFrame { get; }
        public int SlowFallingFrame {  get; }
        public int FallingFrame { get; }
        public int AnimationTick { get; }

    }
}

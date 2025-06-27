using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class LuminarisRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/LuminarisRelicTile";

        public override int AssociatedItem => ModContent.ItemType<LuminarisRelic>();
    }
}

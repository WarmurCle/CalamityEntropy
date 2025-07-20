using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class AcropolisRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/AcropolisRelicTile";

        public override int AssociatedItem => ModContent.ItemType<AcropolisRelic>();
    }
}

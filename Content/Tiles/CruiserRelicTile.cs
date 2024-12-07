using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class CruiserRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/CruiserRelicTile";

        public override int AssociatedItem => ModContent.ItemType<CruiserRelic>();
    }
}

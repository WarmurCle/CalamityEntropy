using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class ApsychosRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/ApsychosRelicTile";

        public override int AssociatedItem => ModContent.ItemType<ApsychosRelic>();
    }
}

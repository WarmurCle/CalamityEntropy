using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class ProphetRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/ProphetRelicTile";

        public override int AssociatedItem => ModContent.ItemType<ProphetRelic>();
    }
}

using CalamityEntropy;
using CalamityEntropy.Items;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Tiles
{
    public class CruiserRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Tiles/CruiserRelicTile";

        public override int AssociatedItem => ModContent.ItemType<CruiserRelic>();
    }
}

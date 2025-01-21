using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Tiles
{
    public class NihilityTwinRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/NihilityTwinRelicTile";

        public override int AssociatedItem => ModContent.ItemType<NihilityTwinRelic>();
    }
}

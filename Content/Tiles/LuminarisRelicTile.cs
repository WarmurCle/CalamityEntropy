using CalamityEntropy.Content.Items;
using CalamityMod.Tiles.BaseTiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class LuminarisRelicTile : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityEntropy/Content/Tiles/LuminarisRelicTile";

        public override int AssociatedItem => ModContent.ItemType<NihilityTwinRelic>();
    }
}

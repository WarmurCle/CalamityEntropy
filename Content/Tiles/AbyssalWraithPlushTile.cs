using CalamityEntropy.Content.Items;
using CalamityMod.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class AbyssalWraithPlushTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<AbyssalWraithPlush>());
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.addTile(Type);
            Main.tileFrameImportant[(int)base.Type] = true;
            AddMapEntry(new Color(63, 72, 200));

            DustType = (int)CalamityDusts.PurpleCosmilite;
        }

    }
}

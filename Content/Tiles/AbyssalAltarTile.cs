using CalamityEntropy.Content.Items;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class AbyssalAltarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<AbyssalAltar>());
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.addTile(Type);
            Main.tileFrameImportant[(int)base.Type] = true;
            AddMapEntry(new Color(134, 180, 240), CalamityUtils.GetItemName<AbyssalAltar>());

            DustType = (int)CalamityDusts.PurpleCosmilite;
        }

    }
}

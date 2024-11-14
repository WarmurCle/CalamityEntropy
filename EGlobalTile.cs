using CalamityMod;
using CalamityMod.Items.Placeables.FurnitureAuric;
using CalamityMod.Tiles.FurnitureAuric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class EGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            if (type == ModContent.TileType<AuricToiletTile>()) {
                WorldGen.ReplaceTile(i, j, (ushort)ModContent.TileType<AToilet>(), 0);
                
            }
        }
    }

}

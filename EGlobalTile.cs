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
using CalamityMod.Items.Accessories.Vanity;
using Terraria.DataStructures;
using Terraria.Enums;
using CalamityMod.Tiles;
using CalamityEntropy.Items.Vanity;

namespace CalamityEntropy
{
    public class EGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            if (type == ModContent.TileType<AuricToiletTile>())
            {
                WorldGen.ReplaceTile(i, j, (ushort)ModContent.TileType<AToilet>(), 0);

            }
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];

            if (!effectOnly && !fail && TileID.Sets.IsShakeable[type])
            {
                CalamityGlobalTile.GetTreeBottom(i, j, out int treeX, out int treeY);
                TreeTypes treeType = WorldGen.GetTreeType(Main.tile[treeX, treeY].TileType);
                if (treeType != TreeTypes.None)
                {
                    treeY--;
                    while (treeY > 10 && Main.tile[treeX, treeY].HasTile && TileID.Sets.IsShakeable[Main.tile[treeX, treeY].TileType])
                        treeY--;

                    treeY++;

                    if (WorldGen.IsTileALeafyTreeTop(treeX, treeY) && !Collision.SolidTiles(treeX - 2, treeX + 2, treeY - 2, treeY + 2))
                    {
                        int randomAmt = WorldGen.genRand.Next(1, 3);
                        for (int z = 0; z < randomAmt; z++)
                        {
                            int treeDropItemType = 0;
                            switch (treeType)
                            {
                                case TreeTypes.Snow:
                                    treeDropItemType = WorldGen.genRand.NextBool(7) ? ModContent.ItemType<SilverFramedGlasses>() : 0;
                                    break;


                                default:
                                    break;
                            }

                            if (treeDropItemType != 0)
                            {
                                Item.NewItem(new EntitySource_TileBreak(treeX, treeY), treeX * 16, treeY * 16, 16, 16, treeDropItemType);
                            }
                        }
                    }
                }
            }
        }
    }
}

using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Tiles;
using CalamityMod.Tiles.FurnitureAuric;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EGlobalTile : GlobalTile
    {
        public override bool CanExplode(int i, int j, int type)
        {
            return !SubworldSystem.IsActive<DimDungeonSubworld>();
        }
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            return !SubworldSystem.IsActive<DimDungeonSubworld>();
        }
        public override bool CanPlace(int i, int j, int type)
        {
            return !SubworldSystem.IsActive<DimDungeonSubworld>();
        }

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
            if(!fail)
            {
                if(tile.TileType == TileID.CorruptThorns || tile.TileType == TileID.CrimsonThorns)
                {
                    if (Main.rand.NextBool(70)) {
                        Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<VenomPiece>());
                    }
                }
            }
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

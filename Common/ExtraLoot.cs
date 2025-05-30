﻿using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.Cards;
using CalamityEntropy.Content.Items.Accessories.SoulCards;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class ExtraLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            for (int i = 0; i < 3; i++)
            {
                int bc = 0;
                while (bc++ < 4096)
                {
                    int px = Main.rand.Next(Main.tile.Width);
                    int py = Main.rand.Next(Main.tile.Height);
                    if (Main.tile[px, py].HasTile && Main.tileBrick[Main.tile[px, py].TileType])
                    {
                        Main.tile[px, py].ResetToType((ushort)ModContent.TileType<TheHeatDeath>());
                        break;
                    }
                }
            }
            int itemsPlaced = 0;
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                {
                    continue;
                }
                Tile chestTile = Main.tile[chest.x, chest.y];
                if (chestTile.TileType == TileID.Containers)
                {
                    if (chestTile.TileFrameX == 1 * 36)
                    {
                        if (!WorldGen.genRand.NextBool(10))
                            continue;
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AuraCard>());
                                itemsPlaced++;
                                break;
                            }
                        }

                    }
                    if (chestTile.TileFrameX == 13 * 36)
                    {
                        if (!WorldGen.genRand.NextBool(2))
                            continue;
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<IndigoCard>());
                                itemsPlaced++;
                                break;
                            }
                        }

                    }
                }
                if (chestTile.TileType == TileID.Containers2)
                {
                    if (chestTile.TileFrameX == 10 * 36)
                    {
                        if (!WorldGen.genRand.NextBool(3))
                            continue;
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {

                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {

                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<InspirationCard>());
                                itemsPlaced++;
                                break;
                            }
                        }

                    }
                }
                if (chestTile.TileType == ModContent.TileType<CalamityMod.Tiles.Abyss.AbyssTreasureChest>())
                {
                    if (!WorldGen.genRand.NextBool(3))
                        continue;
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {

                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            int type = ModContent.ItemType<WispLantern>();

                            if (WorldGen.genRand.NextBool(4))
                            {
                                type = ModContent.ItemType<AbyssLantern>();
                            }
                            if (WorldGen.genRand.NextBool(2))
                            {
                                type = ModContent.ItemType<EnduranceCard>();
                            }

                            chest.item[inventoryIndex].SetDefaults(type);
                            itemsPlaced++;
                            break;
                        }
                    }


                }
            }
        }
    }
}

using CalamityEntropy.Items.Accessories.Cards;
using CalamityMod.Items.Placeables.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy
{
public class ExtraLoot : ModSystem
    {
public override void PostWorldGen()
        {
            int itemsPlaced = 0;
            // Loop over all the chests
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                {
                    continue;
                }
                Tile chestTile = Main.tile[chest.x, chest.y];
                // We need to check if the current chest is the Frozen Chest. We need to check that it exists and has the TileType and TileFrameX values corresponding to the Frozen Chest.
                // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Frozen Chest. Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding. An alternate approach is to check the wiki and looking for the "Internal Tile ID" section in the infobox: https://terraria.wiki.gg/wiki/Frozen_Chest
                if (chestTile.TileType == TileID.Containers)
                {
                    if (chestTile.TileFrameX == 1 * 36)
                    {
                        if (!WorldGen.genRand.NextBool(10))
                            continue;
                        // Next we need to find the first empty slot for our item
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {

                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {

                                // Place the item
                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AuraCard>());
                                // Alternate approach: var instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
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
                        // Next we need to find the first empty slot for our item
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {

                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {

                                // Place the item
                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<InspirationCard>());
                                // Alternate approach: var instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
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
                    // Next we need to find the first empty slot for our item
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {

                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {

                            // Place the item
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<EnduranceCard>());
                            // Alternate approach: var instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
                            itemsPlaced++;
                            break;
                        }
                    }


                }
            }
        }
    }
}

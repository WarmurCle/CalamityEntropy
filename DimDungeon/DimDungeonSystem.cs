using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria.ID;

namespace CalamityEntropy.DimDungeon;

public class DimDungeonSystem : ModSystem
{
    public static List<Room> rooms = null;
    public static bool[,] doors = null;
    public override void PostDrawTiles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
        if (rooms != null && SubworldSystem.IsActive<DimDungeonSubworld>()) {
            foreach (Room room in rooms) {
                Rectangle r = room.getRect();
                Util.Util.DrawRectAlt(r, Color.Blue, 4, 0);
            }
        }
        Main.spriteBatch.End();
    }
    public bool hasEnemyLast = false;
    public override void PostUpdatePlayers()
    {
        if (rooms != null && SubworldSystem.IsActive<DimDungeonSubworld>())
        {
            foreach (Room room in rooms)
            {
                if (!room.SpawnedEnemies)
                {
                    foreach (Player plr in Main.player)
                    {
                        if (plr.active && room.getRect().Intersects(plr.Hitbox))
                        {
                            room.SpawnedEnemies = true;
                            NPC.NewNPC(NPC.GetSource_NaturalSpawn(), room.getRect().Center.X, room.getRect().Center.Y, NPCID.Zombie);
                        }
                    }
                }
            }
        }
        bool hasEnemy = false;
        foreach (NPC n in Main.npc)
        {
            
            if (n.active)
            {
                if (!n.friendly)
                {
                    hasEnemy = true;
                    break;
                }
            }
        }
        ushort doorTile = TileID.PoopBlock;
        if (hasEnemy && !hasEnemyLast)
        {
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (doors[x, y])
                    {
                        Tile t = Main.tile[x, y];
                        t.TileType = doorTile;
                        t.HasTile = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, x, y);
                    }
                }
            }

        }
        if (!hasEnemy && hasEnemyLast)
        {
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (doors[x, y])
                    {
                        Tile t = Main.tile[x, y];
                        t.HasTile = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, x, y);
                    }
                }
            }
        }
        hasEnemyLast = hasEnemy;
    }
}

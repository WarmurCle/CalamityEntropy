using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Content.DimDungeon;

public class DimDungeonGenPass : GenPass
{

    public static void CreateRooms(RoomMetadata rootRoom, List<RoomGenAction> map, out ShapeData shapeData, out List<Room> rooms, out List<Corridor> corridors, out bool[,] doors)
    {
        shapeData = new ShapeData();
        rooms = new List<Room>();
        corridors = new List<Corridor>();
        doors = new bool[Main.maxTilesX, Main.maxTilesY];
        WorldUtils.Gen(START_POINT, new Shapes.Rectangle(rootRoom.Width, rootRoom.Height), new Actions.Blank().Output(shapeData));

        rooms.Add(new RootRoom(
            new Rectangle(
                START_POINT.X,
                START_POINT.Y,
                rootRoom.Width,
                rootRoom.Height
            ), rootRoom)
        { SpawnedEnemies = true });

        foreach (RoomGenAction action in map)
        {

            Direction direction = action.Direction;

            CorridorMetadata corridorMetadata = action.Corridor;

            Room lastRoom = rooms.Last();

            bool isHorizonal = direction.IsHorizontal();


            var (corridorX, corridorY) = corridorMetadata.GetOffset(lastRoom.Bounds, direction);
            WorldUtils.Gen(START_POINT, new Shapes.Rectangle(
                    isHorizonal ? corridorMetadata.Length : corridorMetadata.Width,
                    isHorizonal ? corridorMetadata.Width : corridorMetadata.Length
                ), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Offset(corridorX - START_POINT.X, corridorY - START_POINT.Y),
                        new Actions.Blank().Output(shapeData)
                    }
                )
            );
            if (isHorizonal)
            {
                for (int i = 0; i < corridorMetadata.Width; i++)
                {
                    int x = corridorX;
                    int y = corridorY;
                    doors[x, y + i] = true;
                    doors[x + corridorMetadata.Length - 1, y + i] = true;
                    doors[x + 1, y + i] = true;
                    doors[x + corridorMetadata.Length - 2, y + i] = true;
                    doors[x + 2, y + i] = true;
                    doors[x + corridorMetadata.Length - 3, y + i] = true;
                    doors[x + 3, y + i] = true;
                    doors[x + corridorMetadata.Length - 4, y + i] = true;

                }
            }
            else
            {
                for (int i = 0; i < corridorMetadata.Width; i++)
                {
                    int x = corridorX;
                    int y = corridorY;
                    doors[x + i, y] = true;
                    doors[x + i, y + corridorMetadata.Length - 1] = true;
                    doors[x + i, y + 1] = true;
                    doors[x + i, y + corridorMetadata.Length - 2] = true;
                    doors[x + i, y + 2] = true;
                    doors[x + i, y + corridorMetadata.Length - 3] = true;
                    doors[x + i, y + 3] = true;
                    doors[x + i, y + corridorMetadata.Length - 4] = true;
                }
            }
            Corridor newCorridor = new Corridor(
                new Rectangle(corridorX, corridorY, corridorMetadata.Width, corridorMetadata.Length),
                direction,
                corridorMetadata
                );


            RoomMetadata room = action.NextRoom;
            var (roomX, roomY) = room.GetOffset(corridorX, corridorY, corridorMetadata.Width, corridorMetadata.Length,
                direction);
            room.genPos = new Point(roomX - START_POINT.X, roomY - START_POINT.Y);

            WorldUtils.Gen(START_POINT, new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(roomX - START_POINT.X, roomY - START_POINT.Y),
                new Actions.Blank().Output(shapeData)
            }));

            rooms.Add(new Room(
                new Rectangle(
                    roomX,
                    roomY,
                    room.Width,
                    room.Height
                ),
                room).ConnectsWith(newCorridor, direction.Reverse(), lastRoom));
            corridors.Add(newCorridor);
        }
        int corridorWidth = 4;
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currentRoom = rooms[i];
            int maxSearchDistance = 128;

            Rectangle searchHorizonal = currentRoom.Bounds;
            searchHorizonal.Inflate(maxSearchDistance, 0);
            Rectangle searchVertical = currentRoom.Bounds;
            searchVertical.Inflate(0, maxSearchDistance);

            foreach (Room room in rooms.Skip(i + 1))
            {
                CorridorMetadata corridorMetadata = null;
                Direction direction = Direction.Up;
                int offsetAmount = 0;
                if (currentRoom.Bounds.Intersects(room.Bounds)) continue;
                if (currentRoom.ConnectedRooms.Contains(room)) continue;
                if (searchHorizonal.Intersects(room.Bounds))
                {
                    if (room.Bounds.Y < currentRoom.Bounds.Y)
                    {
                        offsetAmount = currentRoom.Bounds.Y - room.Bounds.Y;
                    }
                    else
                    {
                        offsetAmount = room.Bounds.Y - currentRoom.Bounds.Y;
                    }

                    if (room.Bounds.X < currentRoom.Bounds.X)
                    {
                        corridorMetadata = new CorridorMetadata(corridorWidth, currentRoom.Bounds.X - room.Bounds.X);
                        direction = Direction.Left;
                    }
                    else
                    {
                        corridorMetadata = new CorridorMetadata(corridorWidth, room.Bounds.X - currentRoom.Bounds.X);
                        direction = Direction.Right;
                    }
                }

                if (searchVertical.Intersects(room.Bounds))
                {
                    if (room.Bounds.X < currentRoom.Bounds.X)
                    {
                        offsetAmount = currentRoom.Bounds.X - room.Bounds.X;
                    }
                    else
                    {
                        offsetAmount = room.Bounds.X - currentRoom.Bounds.X;
                    }

                    if (room.Bounds.Y < currentRoom.Bounds.Y)
                    {
                        corridorMetadata = new CorridorMetadata(corridorWidth, currentRoom.Bounds.Y - room.Bounds.Y - room.Bounds.Height);
                        direction = Direction.Up;
                    }
                    else
                    {
                        corridorMetadata = new CorridorMetadata(corridorWidth, room.Bounds.Y - currentRoom.Bounds.Y - currentRoom.Bounds.Height);
                        direction = Direction.Down;
                    }
                }

                if (corridorMetadata == null) continue;

                var (corridorX, corridorY) = corridorMetadata.GetOffset(currentRoom.Bounds, direction, offsetAmount);

                WorldUtils.Gen(START_POINT, new Shapes.Rectangle(
                        direction.IsHorizontal() ? corridorMetadata.Length : corridorMetadata.Width,
                        direction.IsHorizontal() ? corridorMetadata.Width : corridorMetadata.Length
                    ), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Offset(corridorX - START_POINT.X, corridorY - START_POINT.Y),
                            new Actions.Blank().Output(shapeData)
                        }
                    )
                );
                if (direction.IsHorizontal())
                {
                    for (int h = 0; h < corridorMetadata.Width; h++)
                    {
                        int x = corridorX;
                        int y = corridorY;
                        doors[x, y + h] = true;
                        doors[x + corridorMetadata.Length - 1, y + h] = true;
                        doors[x + 1, y + h] = true;
                        doors[x + corridorMetadata.Length - 2, y + h] = true;
                        doors[x + 2, y + h] = true;
                        doors[x + corridorMetadata.Length - 3, y + h] = true;
                        doors[x + 3, y + h] = true;
                        doors[x + corridorMetadata.Length - 4, y + h] = true;
                    }
                }
                else
                {
                    for (int h = 0; h < corridorMetadata.Width; h++)
                    {
                        int x = corridorX;
                        int y = corridorY;
                        doors[x + h, y] = true;
                        doors[x + h, y + corridorMetadata.Length - 1] = true;
                        doors[x + h, y + 1] = true;
                        doors[x + h, y + corridorMetadata.Length - 2] = true;
                        doors[x + h, y + 2] = true;
                        doors[x + h, y + corridorMetadata.Length - 3] = true;
                        doors[x + h, y + 3] = true;
                        doors[x + h, y + corridorMetadata.Length - 4] = true;
                    }
                }
                Corridor newCorridor = new Corridor(
                    new Rectangle(corridorX, corridorY, corridorMetadata.Width, corridorMetadata.Length),
                    direction,
                    corridorMetadata
                    );

                currentRoom.ConnectsWith(newCorridor, direction, room);
            }
        }


    }

    public static Point START_POINT = new Point(500, 500);

    public DimDungeonGenPass() : base("Terrain", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Generating terrain"; Main.worldSurface = Main.maxTilesY - 42; Main.rockLayer = Main.maxTilesY;
        List<RoomGenAction> map = new List<RoomGenAction>()
        {
            new(new CorridorMetadata(4, 64), Direction.Left, new RoomMetadata(64, 16)),
            new(new CorridorMetadata(4, 16), Direction.Up, new RoomMetadata(16, 64)),
            new(new CorridorMetadata(4, 16), Direction.Right, new RoomMetadata(64, 64))
        };
        RoomMetadata rootRoom = new RoomMetadata(16, 16);

        START_POINT = new Point(Main.maxTilesX / 2 - rootRoom.Width / 2, Main.maxTilesY / 2 - rootRoom.Height / 2);

        CreateRooms(rootRoom, map, out ShapeData shapeData, out List<Room> rooms, out List<Corridor> corridors, out bool[,] Doors);
        DimDungeonSystem.rooms = rooms;
        DimDungeonSystem.doors = Doors;
        WorldUtils.Gen(START_POINT, new ModShapes.OuterOutline(shapeData, true), new Actions.SetTile(TileID.AmethystGemspark, true));
        /*for(int x = 0; x < Main.maxTilesX; x++)
        {
            for(int y = 0; y < Main.maxTilesY; y++)
            {
                if(Doors[x, y])
                {
                    Tile t = Main.tile[x, y];
                    t.TileType = TileID.PoopBlock;
                    t.HasTile = true;

                }
            }
        }*/
    }
}
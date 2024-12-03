using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace CalamityEntropy.DimDungeon;

public class DimDungeonGenPass : GenPass
{

    public static void CreateRooms(RoomMetadata rootRoom, List<RoomGenAction> map, out ShapeData shapeData, out List<Room> rooms, out List<Corridor> corridors)
    {
        shapeData = new ShapeData();
        rooms = new List<Room>();
        corridors = new List<Corridor>();
        // 初始房间
        WorldUtils.Gen(START_POINT, new Shapes.Rectangle(rootRoom.Width, rootRoom.Height), new Actions.Blank().Output(shapeData));
                
        rooms.Add(new RootRoom(
            new Rectangle(
                START_POINT.X,
                START_POINT.Y,
                rootRoom.Width,
                rootRoom.Height
            ), rootRoom));
        // int lastWidth = 64;
        // int lastHeight = 64;
        //
        // int lastRoomX = START_POINT.X;
        // int lastRoomY = START_POINT.Y;
        
        // 生成一条线房间和走廊
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
            
            Corridor newCorridor = new Corridor(
                new Rectangle(corridorX, corridorY, corridorMetadata.Width, corridorMetadata.Length),
                direction,
                corridorMetadata
                );
            
            
            RoomMetadata room = action.NextRoom;
            var (roomX, roomY) = room.GetOffset(corridorX, corridorY, corridorMetadata.Width, corridorMetadata.Length,
                direction);
            
            
            WorldUtils.Gen(START_POINT, new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(roomX - START_POINT.X, roomY - START_POINT.Y),
                new Actions.Blank().Output(shapeData)
            }));
            // lastWidth = roomMetadata.Width;
            // lastHeight = roomMetadata.Height;

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
        // 生成额外走廊
        int corridorWidth = 4;
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currentRoom = rooms[i];
            int maxSearchDistance = 128;
            
            Rectangle searchHorizonal = currentRoom.Bounds;
            searchHorizonal.Inflate(maxSearchDistance, 0);
            Rectangle searchVertical = currentRoom.Bounds;
            searchVertical.Inflate(0, maxSearchDistance);

            foreach (Room room in rooms.Skip(i+1))
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
        progress.Message = "Generating terrain"; // Sets the text displayed for this pass
        Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
        Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            
        // TODO 把这个byd map做成随机的
        List<RoomGenAction> map = new List<RoomGenAction>()
        {
            new(new CorridorMetadata(4, 64), Direction.Left, new RoomMetadata(64, 16)),
            new(new CorridorMetadata(4, 16), Direction.Up, new RoomMetadata(16, 64)),
            new(new CorridorMetadata(4, 16), Direction.Right, new RoomMetadata(64, 64))
        };
        RoomMetadata rootRoom = new RoomMetadata(16, 16);
        // List<RoomGenAction> map = new();
        
        START_POINT = new Point(Main.maxTilesX / 2 - rootRoom.Width / 2, Main.maxTilesY / 2 - rootRoom.Height / 2);
        
        CreateRooms(rootRoom, map, out ShapeData shapeData , out List<Room> rooms, out List<Corridor> corridors);
            
        WorldUtils.Gen(START_POINT, new ModShapes.OuterOutline(shapeData, true), new Actions.SetTile(TileID.AmethystGemspark, true));
    }
}
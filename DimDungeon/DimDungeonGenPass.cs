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

    public static void CreateRooms(List<RoomGenAction> map, out ShapeData shapeData, out List<Room> rooms)
    {
        shapeData = new ShapeData();
        rooms = new List<Room>();
        // 初始房间
        WorldUtils.Gen(START_POINT, new Shapes.Rectangle(16, 16), new Actions.Blank().Output(shapeData));
                
        rooms.Add(new RootRoom(
            new Rectangle(
                START_POINT.X,
                START_POINT.Y,
                16,
                16
            )));
        // int lastWidth = 64;
        // int lastHeight = 64;
        //
        // int lastRoomX = START_POINT.X;
        // int lastRoomY = START_POINT.Y;
            
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
                )));
        }
            
            
            
            
            
    }
        
    public static Point START_POINT = new Point(500, 500);
        
    public DimDungeonGenPass() : base("Terrain", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Generating terrain"; // Sets the text displayed for this pass
        Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
        Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            
        List<RoomGenAction> map = new List<RoomGenAction>()
        {
            new(new CorridorMetadata(4, 64), Direction.Left, new RoomMetadata(16, 16)),
            new(new CorridorMetadata(4, 16), Direction.Up, new RoomMetadata(16, 16)),
            new(new CorridorMetadata(4, 16), Direction.Right, new RoomMetadata(16, 16))
        };
            
        // WorldUtils.Gen(point, new Shapes.Rectangle(16, 16), new Actions.Blank().Output(shapeData));
        //
        // WorldUtils.Gen(point, new Shapes.Rectangle(16, 4), Actions.Chain(new GenAction[]
        // {
        //     new Modifiers.Offset(16, 8),
        //     new Actions.Blank().Output(shapeData)
        // }));
        //
        // WorldUtils.Gen(point, new Shapes.Rectangle(16, 16), Actions.Chain(new GenAction[]
        // {
        //     new Modifiers.Offset(32, 0),
        //     new Actions.Blank().Output(shapeData)
        // }));
        CreateRooms(map, out ShapeData shapeData , out List<Room> rooms);
            
        WorldUtils.Gen(START_POINT, new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile(TileID.AmethystGemspark, true));
    }
}
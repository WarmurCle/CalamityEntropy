using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityEntropy.DimDungeon
{
    public class EmptyMapGenPass : GenPass
    {
        public static List<RoomGenAction> map = new List<RoomGenAction>()
        {
            new(new Corridor(4), Direction.Left, new Room(16, 16)),
            new(new Corridor(4), Direction.Up, new Room(16, 16)),
            new(new Corridor(4), Direction.Right, new Room(16, 16))
        };

        public static void ExportShapeData(in ShapeData shapeData, List<RoomGenAction> map)
        {
            // 初始房间
            WorldUtils.Gen(START_POINT, new Shapes.Rectangle(16, 16), new Actions.Blank().Output(shapeData));

            int lastWidth = 16;
            int lastHeight = 16;

            int offsetX = 0;
            int offsetY = 0;
            
            foreach (RoomGenAction action in map)
            {
                void AddOffset ()
                {
                    switch (action.Direction)
                    {
                        case Direction.Up:
                            offsetY -= lastHeight;
                            break;
                        case Direction.Down:
                            offsetY += lastHeight;
                            break;
                        case Direction.Left:
                            offsetX -= lastWidth;
                            break;
                        case Direction.Right:
                            offsetX += lastWidth;
                            break;
                    }
                }
                AddOffset();

                bool isHorizontal()
                {
                    return action.Direction == Direction.Left || action.Direction == Direction.Right;
                }
                
                Corridor corridor = action.Corridor;
                int length = 16;
                WorldUtils.Gen(START_POINT, new Shapes.Rectangle(
                    isHorizontal() ? length : corridor.Width,
                    isHorizontal() ? corridor.Width : length
                    ), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(
                        !isHorizontal() ? offsetX + corridor.Width : offsetX,
                        !isHorizontal() ? offsetY : offsetY + corridor.Width
                        ),
                    new Actions.Blank().Output(shapeData)
                }));
                lastWidth = isHorizontal() ? length : corridor.Width;
                lastHeight = isHorizontal() ? corridor.Width : length;
                
                AddOffset();
                
                Room room = action.NextRoom;
                
                WorldUtils.Gen(START_POINT, new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(offsetX, offsetY),
                    new Actions.Blank().Output(shapeData)
                }));
                lastWidth = room.Width;
                lastHeight = room.Height;
            }
            
            
            
            
            
        }
        
        public static Point START_POINT = new Point(500, 500);
        
        public EmptyMapGenPass() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating terrain"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            
            Point point = new Point(500, 500);
            ShapeData shapeData = new ShapeData();
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
            ExportShapeData(in shapeData, map);
            
            WorldUtils.Gen(point, new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile(TileID.AmethystGemspark, true));
        }
    }
    
    public class DimDungeon : Subworld
    {
        public override int Width => 1000;
        public override int Height => 1000;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;
        
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = true;
        }

        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            color = new Vector3(180, 180, 255);
            return true;
        }
        
        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new EmptyMapGenPass(),
        };

        public override void DrawMenu(GameTime gameTime)
        {
            base.DrawMenu(gameTime);
        }
    }
}

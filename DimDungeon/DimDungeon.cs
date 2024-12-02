using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityEntropy.DimDungeon
{
    public class EmptyMapGenPass : GenPass
    {

        public static void ExportShapeData(in ShapeData shapeData, List<RoomGenAction> map)
        {
            
            
            foreach (RoomGenAction action in map)
            {
                Point offset_ = new Point(0, 0);
                
                void genRoom(RoomGenAction roomAction, in ShapeData shapeData, Point offsetR, int lastWidth, int lastHeight)
                {
                    Point offset;
                    void AddOffset(Corridor c)
                    {
                        switch (c.Direction)
                        {
                            case Direction.Up:
                                offset.Y -= lastHeight;
                                break;
                            case Direction.Down:
                                offset.Y += lastHeight;
                                break;
                            case Direction.Left:
                                offset.X -= lastWidth;
                                break;
                            case Direction.Right:
                                offset.X += lastWidth;
                                break;
                        }
                    }
                    foreach (Corridor corridor in roomAction.room.Corridor)
                    {
                        offset = offsetR;
                        AddOffset(corridor);

                        bool isHorizontal()
                        {
                            return corridor.Direction == Direction.Left || corridor.Direction == Direction.Right;
                        }

                        int length = corridor.Length;
                        WorldUtils.Gen(START_POINT, new Shapes.Rectangle(
                            isHorizontal() ? length : corridor.Width,
                            isHorizontal() ? corridor.Width : length
                            ), Actions.Chain(new GenAction[]
                        {
                    new Modifiers.Offset(
                        !isHorizontal() ? offset.X + corridor.Width : offset.X,
                        !isHorizontal() ? offset.Y : offset.Y + corridor.Width
                        ),
                    new Actions.Blank().Output(shapeData)
                        }));
                        lastWidth = isHorizontal() ? length : corridor.Width;
                        lastHeight = isHorizontal() ? corridor.Width : length;

                        AddOffset(corridor);

                        Room room = corridor.NextRoom;

                        WorldUtils.Gen(START_POINT, new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new GenAction[]
                        {
                    new Modifiers.Offset(offset.X, offset.Y),
                    new Actions.Blank().Output(shapeData)
                        }));
                        lastWidth = room.Width;
                        lastHeight = room.Height;
                        if ((offset + START_POINT).X > 200 && (offset + START_POINT).X < 2800 && (offset + START_POINT).Y > 200 && (offset + START_POINT).Y < 2800)
                        {
                            genRoom(new RoomGenAction(room), in shapeData, offset, lastWidth, lastHeight);
                        }
                    }

                }

                genRoom(action, in shapeData, offset_, action.room.Width, action.room.Height);
                
                
            }
            
            
        }
        
        public static Point START_POINT = new Point(1500, 1500);
        
        public EmptyMapGenPass() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating terrain"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            
            Point point = new Point(1500, 1500);
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
            ExportShapeData(in shapeData, MapGen.Gen());
            
            WorldUtils.Gen(point, new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile(TileID.AmethystGemspark, true));
        }
    }
    
    public class DimDungeon : Subworld
    {
        public override int Width => 3000;
        public override int Height => 3000;

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

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
        public EmptyMapGenPass() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating terrain"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            
            Point point = new Point(500, 500);
            ShapeData shapeData = new ShapeData();
            WorldUtils.Gen(point, new Shapes.Rectangle(16, 16), new Actions.Blank().Output(shapeData));
            
            WorldUtils.Gen(point, new Shapes.Rectangle(16, 4), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(16, 8),
                new Actions.Blank().Output(shapeData)
            }));
            
            WorldUtils.Gen(point, new Shapes.Rectangle(16, 16), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(32, 0),
                new Actions.Blank().Output(shapeData)
            }));
            
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

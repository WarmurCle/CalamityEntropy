using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Content.DimDungeon
{
    public class DimDungeonSubworld : Subworld
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
            new DimDungeonGenPass(),
        };

        public override void DrawMenu(GameTime gameTime)
        {
            base.DrawMenu(gameTime);
        }
    }
}

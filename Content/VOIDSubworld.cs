using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;

namespace CalamityEntropy.Content.DimDungeon
{
    public class VOIDSubworld : Subworld
    {
        public override int Width => 1600;
        public override int Height => 1600;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => false;

        public override void OnEnter()
        {
            SubworldSystem.noReturn = true;
            SubworldSystem.hideUnderworld = true;
        }

        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            color = new Vector3(180, 180, 255);
            return true;
        }

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new GP()
        };

        public class GP : GenPass
        {
            public GP() : base("G", 1) { }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                Main.worldSurface = Main.maxTilesY - 2;
                Main.rockLayer = Main.maxTilesY - 4;
            }
        }

        public override void DrawMenu(GameTime gameTime)
        {
            Main.spriteBatch.UseSampleState_UI(SamplerState.PointWrap);
            Main.spriteBatch.Draw(Util.Util.getExtraTex("noise"), Vector2.Zero, new Rectangle(Main.rand.Next(-4096, 4096), Main.rand.Next(-4096, 4096), Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
        }
    }
}

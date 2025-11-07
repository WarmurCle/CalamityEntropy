using CalamityMod.Tiles.FurnitureMonolith;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.CodeDom;
using Terraria.ModLoader;

namespace CalamityEntropy.Assets.Register
{
    public class TextureRegister : ModSystem
    {
        private const string AssetsPath = "CalamityEntropy/Assets/";
        private const string ExtraPath = "CalamityEntropy/Assets/Extra/";
        public static Asset<Texture2D> Trail_SlashWrap { get; private set; }
        public static Asset<Texture2D> Trail_DoubleLine { get; private set; }
        public static Asset<Texture2D> Trail_MotionTrail1 { get; private set; }
        public static Asset<Texture2D> Trail_MotionTrail2 { get; private set; }
        public static Asset<Texture2D> Trail_MotionTrail3 { get; private set; }
        public static Asset<Texture2D> Trail_MotionTrail4 { get; private set; }
        public static Asset<Texture2D> Trail_Streak1w { get; private set; }
        public static Asset<Texture2D> Noise_Misc1 {  get; private set; }
        public static Asset<Texture2D> Noise_Misc2 {  get; private set; }
        public static Asset<Texture2D> General_Shockwave {  get; private set; }
        public static Asset<Texture2D> General_WhiteOrb {  get; private set; }
        public static Asset<Texture2D> General_WhiteCube {  get; private set; }
        public static Asset<Texture2D> General_WhiteCircle {  get; private set; }
        public override void Load()
        {
            //轨迹
            Trail_SlashWrap = RegisByExtra("Slash_Wrap");
            Trail_DoubleLine = RegisByAssets("DoubleLineTrail");
            Trail_MotionTrail2 = RegisByAssets("MotionTrail2");
            Trail_MotionTrail3 = RegisByAssets("MotionTrail3");
            Trail_MotionTrail4 = RegisByAssets("MotionTrail4");
            Trail_Streak1w = RegisByExtra("Streak1w");

            Noise_Misc1 = RegisByAssets("MiscNoise01");
            Noise_Misc2 = RegisByAssets("MiscNoise02");
            General_WhiteCircle = RegisByExtra("BasicCircle");
            General_WhiteCube = RegisByExtra("WhiteCube");
            General_WhiteOrb = RegisByExtra("ShinyOrbParticle");
            General_Shockwave = RegisByExtra("shockwave");
        }
        private Asset<Texture2D> RegisByExtra(string name) => ModContent.Request<Texture2D>($"{ExtraPath}{name}");
        private Asset<Texture2D> RegisByAssets(string name) => ModContent.Request<Texture2D>($"{AssetsPath}{name}");

        public override void Unload()
        {
            Trail_DoubleLine = null;
            Trail_MotionTrail1 = null;
            Trail_MotionTrail2 = null;
            Trail_MotionTrail3 = null;
            Trail_MotionTrail4 = null;
            Trail_Streak1w = null;
            Trail_SlashWrap = null;
            Noise_Misc1 = null;
            Noise_Misc2 = null;
            General_Shockwave = null;
            General_WhiteCube= null;
            General_WhiteOrb = null;
            General_WhiteCircle = null;
        }
    }
}
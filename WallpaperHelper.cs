using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
namespace CalamityEntropy
{
    class WallpaperHelper
    {
        private const int SPI_GETDESKWALLPAPER = 0x0073;
        private const int MAX_PATH = 260;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        public static string GetDesktopWallpaper()
        {
            return CopyWallpaperToTemp();
        }

        public static string CopyWallpaperToTemp()
        {
            StringBuilder wallpaperPath = new StringBuilder(MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaperPath, 0);
            string originalPath = wallpaperPath.ToString();
            string tempPath = Path.Combine(Main.SavePath, "CalamityEntropy/Wallpaper.jpg");

            try
            {
                File.Copy(originalPath, tempPath, true);
                return tempPath;
            }
            catch (UnauthorizedAccessException ex)
            {
                return null;
            }
            
        }

        public static Texture2D wallpaper = null;
        public static Texture2D getWallpaper()
        {
            if (wallpaper != null)
            {
                return wallpaper;
            }
            try
            {
                string wallpaperPath = GetDesktopWallpaper();
                if (wallpaperPath == null)
                {
                    wallpaper = ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value;
                    return wallpaper;
                }
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                if (File.Exists(wallpaperPath))
                {
                    using (FileStream stream = new FileStream(wallpaperPath, FileMode.Open))
                    {
                        wallpaper = Texture2D.FromStream(graphicsDevice, stream);
                        return wallpaper;
                    }
                }
                else
                {
                    wallpaper = ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value;
                }
            }
            catch { wallpaper = ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value; }
            return wallpaper;

        }
    }
}
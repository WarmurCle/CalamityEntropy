using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public static class CommonEffects
    {
        public static Effect rotation;
        public static Effect LoadShader(string name)
        {
            return ModContent.Request<Effect>($"CalamityEntropy/Assets/Effects/{name}", AssetRequestMode.ImmediateLoad).Value;
        }
        public static void Load()
        {
            rotation = LoadShader("rotation");
        }
        public static void Unload()
        {
            rotation = null;
        }
    }
}

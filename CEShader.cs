using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
namespace CalamityEntropy
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public class CEShaders
    {
        private const string ShaderPath = "Effects/";
        internal const string CalamityShaderPrefix = "CalamityEntropy:";

        internal static Effect dogma;

        private static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{CalamityShaderPrefix}{registrationName}"] = passParamRegistration;
        }
        
        public static void LoadShaders()
        {
            AssetRepository calAss = CalamityEntropy.Instance.Assets;
            Effect LoadShader(string path) => calAss.Request<Effect>(path, AssetRequestMode.ImmediateLoad).Value;

            dogma = LoadShader("Effects/Dogma");
            RegisterMiscShader(dogma, "GlitchPass", "Dogma");

        }
    }
}

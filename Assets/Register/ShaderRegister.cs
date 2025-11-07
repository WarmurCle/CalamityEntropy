using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Assets.Register
{
    //我不会用你们自己搓的shader，所以我手动注册了一个简单的系统。
    //有问题别找我
    public class EntropyShaderHandler : ModSystem
    {
        private const string ShaderPath = "CalamityEntropy/Assets/Effects/";
        internal const string ShaderPrefix = "Entropy";
        public static Effect FlowWithAShader;
        public static Effect PolarDistortShader;
        public static Effect DisplacemenShader;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            //将大部分shader静态缓存进来
            FlowWithAShader = LoadShader(nameof(FlowWithAShader));
            PolarDistortShader = LoadShader(nameof(PolarDistortShader));
            DisplacemenShader = LoadShader(nameof(DisplacemenShader));
            RegisShader(FlowWithAShader, "EntropyFlowWithAShader", "FlowWithAShader");
            RegisShader(PolarDistortShader, "EntropyPolarDistortShader", "PolarDistortShader");
            RegisShader(DisplacemenShader, "EntropyDisplacementShader", "EntropyDisplacementShader");
        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;
            FlowWithAShader = null;
            PolarDistortShader = null;
            DisplacemenShader = null;
        }
        private static Effect LoadShader(string name) => ModContent.Request<Effect>($"{ShaderPath}{name}", AssetRequestMode.ImmediateLoad).Value;
        private static void RegisShader(Effect shader) => RegisShader(shader, "Entropy" + nameof(shader), nameof(shader));
        private static void RegisShader(Effect shader, string passName, string regisName)
        {
            Ref<Effect> shaderPtr = new(shader);
            //忽视掉这个，反正我也不会用原版的shader索引。
            MiscShaderData passParamRegis = new(shaderPtr, passName);
            GameShaders.Misc[$"{ShaderPrefix}{regisName}"] = passParamRegis;
            
        }
    }
}
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Common
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("Misc")]
        
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(0f)]
        [Increment(0.01f)]
        public float CraftArmorWithPrefixChance { get; set; }


        [DefaultValue(true)]
        public bool EnableSethomeCommand {  get; set; }
    }
}

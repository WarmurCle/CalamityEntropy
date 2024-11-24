using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace CalamityEntropy
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Header("Misc")]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1920f)]
        [DefaultValue(900f)]
        [Increment(1f)]
        [DrawTicks]
        public float VoidChargeBarX { get; set; }

        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1080f)]
        [DefaultValue(100f)]
        [Increment(1f)]
        [DrawTicks]
        public float VoidChargeBarY { get; set; }

        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(0.7f)]
        [Increment(0.01f)]
        [DrawTicks]
        public float CraftArmorWithPrefixChance { get; set; }

    }
}

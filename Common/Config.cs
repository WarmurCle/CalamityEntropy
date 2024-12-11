﻿using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Common
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
        [DefaultValue(0f)]
        [Increment(0.01f)]
        [DrawTicks]
        public float CraftArmorWithPrefixChance { get; set; }

        [DefaultValue(true)]
        public bool ChainsawShakeScreen { get; set; }

        [Header("Other")]

        [DefaultValue(false)]
        public bool BindingOfIsaac_Rep_BossMusic { get; set; }

        [DefaultValue(false)]
        public bool RepBossMusicReplaceCalamityMusic { get; set; }

    }
}
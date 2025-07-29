using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Common
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("Misc")]



        [DefaultValue(false)]
        public bool BramblecleaveAlwaysUnlockAllSkill { get; set; }


        [DefaultValue(true)]
        public bool LoreSpecialEffect { get; set; }
    }
}

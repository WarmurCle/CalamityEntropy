using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;

namespace CalamityEntropy.Cooldowns
{
    public class DivingCd : CooldownHandler
    {
        public static new string ID => "DivingSield";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.CdDS");
        public override string Texture => "CalamityEntropy/Cooldowns/DivingShield";
        public override Color OutlineColor => new Color(197, 165, 108);
        public override Color CooldownStartColor => new Color(144, 84, 29);
        public override Color CooldownEndColor => Color.Khaki;

        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Item/AscendantOff");
    }
}

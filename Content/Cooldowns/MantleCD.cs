using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class MantleCD : CooldownHandler
    {
        public static new string ID => "HolyMantleCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.CdMantle");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/MantleCD";
        public override Color OutlineColor => Color.LightBlue;
        public override Color CooldownStartColor => Color.LightBlue;
        public override Color CooldownEndColor => Color.LightBlue;

        public override SoundStyle? EndSound => null;


    }
}

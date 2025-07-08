using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class AntivoidDashCooldown : CooldownHandler
    {
        public static new string ID => "AntivoidDash";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.CdAntivoid");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/AntivoidDashCooldown";
        public override Color OutlineColor => new Color(197, 165, 108);
        public override Color CooldownStartColor => Color.Pink;
        public override Color CooldownEndColor => Color.LightBlue;

        public override SoundStyle? EndSound => new("CalamityEntropy/Assets/Sounds/DeadSunShot");
    }
}

using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class ProtectiveCannonCooldown : CooldownHandler
    {
        public static new string ID => "ProtectiveCannonCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.ProtectiveCannonCooldown");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/ProtectiveCannonCooldown";
        public override Color OutlineColor => Color.Red;
        public override Color CooldownStartColor => Color.Orange;
        public override Color CooldownEndColor => Color.Firebrick;

        public override SoundStyle? EndSound => new("CalamityEntropy/Assets/Sounds/alert2");
    }
}

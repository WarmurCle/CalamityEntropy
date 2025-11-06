using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class DamageReduceCD : CooldownHandler
    {
        public static new string ID => "DamageReduceCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.DamageReduceCD");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/DamageReduceCD";
        public override Color OutlineColor => new Color(235, 235, 160);
        public override Color CooldownStartColor => new Color(235, 235, 160);
        public override Color CooldownEndColor => new Color(235, 235, 160);

        public override SoundStyle? EndSound => null;
    }
}

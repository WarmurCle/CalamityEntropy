using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class BlockingCooldown : CooldownHandler
    {
        public static new string ID => "BlockingCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.BlockingCooldown");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/BlockingCooldown";
        public override Color OutlineColor => new Color(190, 190, 255);
        public override Color CooldownStartColor => new Color(190, 190, 255);
        public override Color CooldownEndColor => new Color(190, 190, 255);

        public override SoundStyle? EndSound => CEUtils.GetSound("ARCD");
    }
}

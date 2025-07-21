using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class OblivionThretherCooldown : CooldownHandler
    {
        public static new string ID => "OblivionThretherCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.OblivionThretherCooldown");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/OblivionThretherCooldown";
        public override Color OutlineColor => Color.SkyBlue;
        public override Color CooldownStartColor => Color.SkyBlue;
        public override Color CooldownEndColor => Color.SkyBlue;

        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Item/CometShardUse");
    }
}

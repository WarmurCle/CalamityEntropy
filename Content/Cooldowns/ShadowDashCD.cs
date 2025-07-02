using CalamityMod.Cooldowns;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class ShadowDashCD : CooldownHandler
    {
        public static new string ID => "ShadowDashCD";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.ShadowDashCD");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/ShadowDashCD";
        public override Color OutlineColor => Color.SkyBlue;
        public override Color CooldownStartColor => Color.Purple;
        public override Color CooldownEndColor => Color.LightBlue;

        public override SoundStyle? EndSound => new("CalamityEntropy/Assets/Sounds/MantleCDEnd");
    }
}

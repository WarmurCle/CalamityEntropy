using CalamityEntropy.Content.Items.Donator;
using CalamityMod.Cooldowns;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;

namespace CalamityEntropy.Content.Cooldowns
{
    public class YoungMasterDashCD : CooldownHandler
    {
        public static new string ID => "YoungMasterDashCD";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.YoungMasterDashCD");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/YoungMasterDashCD";
        public override Color OutlineColor => Color.SkyBlue;
        public override Color CooldownStartColor => Color.DarkRed;
        public override Color CooldownEndColor => Color.Firebrick;
        public override SoundStyle? EndSound => new("CalamityEntropy/Assets/Sounds/SwordHit0");
    }
}

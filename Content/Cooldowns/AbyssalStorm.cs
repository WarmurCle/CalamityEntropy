using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;

namespace CalamityEntropy.Content.Cooldowns
{
    public class AbyssalStorm : CooldownHandler
    {
        public static new string ID => "AbyssalBeam";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.CalamityEntropy.AbyssalStorm");
        public override string Texture => "CalamityEntropy/Content/Cooldowns/AbyssalStorm";
        public override Color OutlineColor => new Color(180, 180, 255);
        public override Color CooldownStartColor => new Color(120, 120, 212);
        public override Color CooldownEndColor => new Color(120, 120, 212);
        public override SoundStyle? EndSound => CEUtils.GetSound("AugerHit", 1.2f);
    }
}

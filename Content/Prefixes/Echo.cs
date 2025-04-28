using CalamityMod;
using CalamityMod.Prefixes;
using System.Collections.Generic;
using System;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;

namespace CalamityEntropy.Content.Prefixes
{
    public class Echo : RogueWeaponPrefix
    {
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            TooltipLine t = new TooltipLine(Mod, "PrefixDescription", AdditionalTooltip.Value)
            {
                IsModifier = true,
                IsModifierBad = false
            };
            yield return t;
        }
        public LocalizedText AdditionalTooltip => Language.GetOrRegister(Mod.GetLocalizationKey("Prefix" + this.Name + "Descr"));

    }
}

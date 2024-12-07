using System.Collections.Generic;
using CalamityEntropy.Util;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Prefixes
{
	// This class serves as an example for declaring item 'prefixes', or 'modifiers' in other words.
	public class Enchanted : ModPrefix
	{

		public override PrefixCategory Category => PrefixCategory.Accessory;

		public override float RollChance(Item item) {
			return 1f;
		}

		public override bool CanRoll(Item item) {
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			
		}
        public override void ApplyAccessoryEffects(Player player)
        {
			player.Entropy().enhancedMana += 0.12f;
        }

        // Modify the cost of items with this modifier with this function.
        public override void ModifyValue(ref float valueMult) {
			valueMult *= 1.1f;
		}

		// This is used to modify most other stats of items which have this modifier.
		public override void Apply(Item item) {
			//
		}

		// This prefix doesn't affect any non-standard stats, so these additional tooltiplines aren't actually necessary, but this pattern can be followed for a prefix that does affect other stats.
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
			// Due to inheritance, this code runs for ExamplePrefix and ExampleDerivedPrefix. We add 2 tooltip lines, the first is the typical prefix tooltip line showing the stats boost, while the other is just some additional flavor text.

			// The localization key for Mods.ExampleMod.Prefixes.PowerTooltip uses a special format that will automatically prefix + or - to the value.
			// This shared localization is formatted with the Power value, resulting in different text for ExamplePrefix and ExampleDerivedPrefix.
			// This results in "+1 Power" for ExamplePrefix and "+2 Power" for ExampleDerivedPrefix.
			// Power isn't an actual stat, the effects of Power are already shown in the "+X% damage" tooltip, so this example is purely educational.
			yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AdditionalTooltip.Value) {
				IsModifier = true, // Sets the color to the positive modifier color.
			};
			// If possible and suitable, try to reuse the name identifier and translation value of Terraria prefixes. For example, this code uses the vanilla translation for the word defense, resulting in "-5 defense". Note that IsModifierBad is used for this bad modifier.
			/*yield return new TooltipLine(Mod, "PrefixAccDefense", "-5" + Lang.tip[25].Value) {
				IsModifier = true,
				IsModifierBad = true,
			};*/
		}

		// PowerTooltip is shared between ExamplePrefix and ExampleDerivedPrefix. 

		// AdditionalTooltip shows off how to do the inheritable localized properties approach. This is necessary this this example uses inheritance and we want different translations for each inheriting class. https://github.com/tModLoader/tModLoader/wiki/Localization#inheritable-localized-properties
		public LocalizedText AdditionalTooltip => Mod.GetLocalization($"PrefixEnchantedDesc");

		public override void SetStaticDefaults() {
			// this.GetLocalization is not used here because we want to use a shared key
			// This seemingly useless code is required to properly register the key for AdditionalTooltip
			_ = AdditionalTooltip;
		}
	}
}

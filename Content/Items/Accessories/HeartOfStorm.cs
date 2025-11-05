using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using InnoVault;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class HeartOfStorm : ModItem
    {
        public static LocalizedText CWRGanged;
        public override void SetStaticDefaults()
        {
            CWRGanged = this.GetLocalization("CWRGanged", () => "灾厄大修联动效果:召唤风暴女神残魂配合你作战");
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<GlowPurple>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().heartOfStorm = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.HasMod("CalamityOverhaul")) 
            {
                var line = new TooltipLine(Mod, "CWRGanged", CWRGanged.Value);
                line.OverrideColor = VaultUtils.MultiStepColorLerp(Main.LocalPlayer.miscCounter % 300 / 300f
                    , Color.AliceBlue, Color.BlueViolet, Color.White, Color.BlueViolet, Color.AliceBlue);
                tooltips.Add(line);
            }
        }
    }
}

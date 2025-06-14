using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items.LoreItems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class NihilityTwinLore : LoreItem
    {
        public static float VoidRes = 0.1f;
        public static int HealPreSec = 1;
        public static float MaxFlyTimeAddition = 0.05f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModContent.GetInstance<ServerConfig>().LoreSpecialEffect)
            {
                TooltipLine tooltipLineEF = new TooltipLine(base.Mod, "Entropy:Effect", Language.GetTextValue("Mods.CalamityEntropy.UseToggle"));
                if (LoreColor.HasValue)
                {
                    tooltipLineEF.OverrideColor = LoreColor.Value;
                }
                tooltips.Add(tooltipLineEF);
                TooltipLine tooltipLineA = new TooltipLine(base.Mod, "Entropy:Effect", Language.GetTextValue("Mods.CalamityEntropy.NihTwinLoreEffect"));
                if (LoreColor.HasValue)
                {
                    tooltipLineA.OverrideColor = LoreColor.Value;
                }
                tooltipLineA.Text = tooltipLineA.Text.Replace("{1}", VoidRes.ToPercent().ToString());
                tooltipLineA.Text = tooltipLineA.Text.Replace("{2}", HealPreSec.ToString());
                tooltipLineA.Text = tooltipLineA.Text.Replace("{3}", MaxFlyTimeAddition.ToPercent().ToString());

                tooltips.Add(tooltipLineA);

                TooltipLine tooltipLineE = new TooltipLine(base.Mod, "Entropy:Effect", Language.GetTextValue("Mods.CalamityEntropy." + (Main.LocalPlayer.Entropy().NihilityTwinLoreBonus ? "Enabled" : "Disabled")));
                tooltipLineE.OverrideColor = Main.LocalPlayer.Entropy().NihilityTwinLoreBonus ? Color.Yellow : Color.Gray;
                tooltips.Add(tooltipLineE);
            }

            TooltipLine tooltipLine = new TooltipLine(base.Mod, "CalamityMod:Lore", Language.GetTextValue("Mods.CalamityEntropy.loreNihTwin"));
            if (LoreColor.HasValue)
            {
                tooltipLine.OverrideColor = LoreColor.Value;
            }

            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[1] { tooltipLine }, hideNormalTooltip: true);
        }
        public override bool CanUseItem(Player player)
        {
            return ModContent.GetInstance<ServerConfig>().LoreSpecialEffect;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Cyan;
            SoundStyle s = new("CalamityEntropy/Assets/Sounds/CastTriangles");
            s.Volume = 0.4f;
            s.Pitch = 1.4f;
            Item.UseSound = s;
            Item.maxStack = 1;
            Item.useTurn = true;
        }
        public override bool? UseItem(Player player)
        {
            EModPlayer modPlayer = player.Entropy();
            player.itemTime = Item.useTime;
            modPlayer.NihilityTwinLoreBonus = !modPlayer.NihilityTwinLoreBonus;
            return true;
        }
    }
}

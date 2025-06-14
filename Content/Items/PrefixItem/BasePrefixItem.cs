using CalamityEntropy.Content.ArmorPrefixes;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.PrefixItem
{

    public abstract class BasePrefixItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 46;
            Item.rare = ItemRarityID.Yellow;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var prefix = ArmorPrefix.findByName(PrefixName);
            tooltips.Replace("|", prefix.GivenName);
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria")
                {
                    line.OverrideColor = prefix.getColor();
                }
            }
            tooltips.Add(prefix.getDescTooltipLine());
            tooltips.Add(new TooltipLine(Mod, "Armor Prefix Item Description", Mod.GetLocalization("PrefixitemDesc").Value) { OverrideColor = Color.Yellow });
        }
        public virtual string PrefixName => "";
        public override string Texture => "CalamityEntropy/Content/Items/PrefixItem/Textures/" + PrefixName;
    }
}
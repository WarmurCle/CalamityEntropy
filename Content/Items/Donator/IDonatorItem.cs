using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public interface IDonatorItem
    {
        public string DonatorName { get; }
    }
    public class DonatorGItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (entity.ModItem != null && entity.ModItem is IDonatorItem i)
            {
                entity.Calamity().donorItem = true;
            }
        }

        public override void ModifyTooltips(Item entity, List<TooltipLine> tooltips)
        {
            if (entity.ModItem != null && entity.ModItem is IDonatorItem i) 
            {
                TooltipLine tl = new TooltipLine(Mod, "EntropyDonorName", Mod.GetLocalization("Donor").Value + " " + i.DonatorName);
                tl.OverrideColor = Color.Yellow;
                tooltips.Add(tl);
            }
        }
    }
}

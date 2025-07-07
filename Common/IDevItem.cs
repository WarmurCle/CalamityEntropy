using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public interface IDevItem
    {
        public string DonatorName { get; }
    }
    public class DevGItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (entity.ModItem != null && entity.ModItem is IDevItem i)
            {
                entity.Calamity().donorItem = true;
            }
        }

        public override void ModifyTooltips(Item entity, List<TooltipLine> tooltips)
        {
            if (entity.ModItem != null && entity.ModItem is IDevItem i)
            {
                TooltipLine tl = new TooltipLine(Mod, "EntropyDonorName", Mod.GetLocalization("Owner").Value + " " + i.DonatorName);
                tl.OverrideColor = Color.Yellow;
                tooltips.Add(tl);
            }
        }
    }
}

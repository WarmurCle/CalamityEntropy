using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEWof : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreWallofFlesh>();
        public static int Cooldown = 20;
        public static float DmgReduce = 0.05f; 
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DmgReduce.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", Cooldown.ToString());
        }
    }
}
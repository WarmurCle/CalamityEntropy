using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class CDRemove : ModItem
    {

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Master;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.EverythingElse;
        }

        public override bool? UseItem(Player player)
        {
            player.Calamity().cooldowns.Clear();
            return true;
        }
    }
}

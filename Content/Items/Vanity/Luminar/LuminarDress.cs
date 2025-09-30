using CalamityEntropy.Content.Items.Donator;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity.Luminar
{
    [AutoloadEquip(EquipType.Body)]
    public class LuminarDress : ModItem, IDonatorItem
    {
        public string DonatorName => "玲瓏";
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.vanity = true;
        }
    }
}

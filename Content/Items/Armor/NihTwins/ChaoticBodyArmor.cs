using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Body)]
    public class ChaoticBodyArmor : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 50;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
            player.lifeRegen += 8;
            player.endurance += 0.1f;
            player.statManaMax2 += 120;
        }

        public override void AddRecipes()
        {
        }
    }
}

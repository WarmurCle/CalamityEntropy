using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Legs)]
    public class ChaoticLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 34;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 6;
        }
        public override void AddRecipes()
        {
        }
    }

}

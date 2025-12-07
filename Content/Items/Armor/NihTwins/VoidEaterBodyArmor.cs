using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Body)]
    public class VoidEaterBodyArmor : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 35;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.4f;
        }

        public override void AddRecipes()
        {
        }
    }
}

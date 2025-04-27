using CalamityEntropy.Common;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
    public class Fool : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().ConfuseCard = true;
            player.Entropy().ManaCost += 0.2f;
            player.GetDamage(DamageClass.Magic) += 0.2f;
        }

        public override void AddRecipes()
        {
        }
    }
}

using CalamityEntropy.Common;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
    public class Perplexed : ModItem
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
            player.GetModPlayer<EModPlayer>().PerplexedCard = true;
            player.GetCritChance(DamageClass.Generic) -= 10;
            player.GetAttackSpeed(DamageClass.Generic) += 0.2f;
        }

        public override void AddRecipes()
        {
        }
    }
}

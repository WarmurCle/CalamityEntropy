using CalamityEntropy.Common;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
    public class Sacrifice : ModItem
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
            player.GetModPlayer<EModPlayer>().SacrificeCard = true;
            player.lifeRegen = (int)(player.lifeRegen * 0.4f);
            player.Entropy().lifeRegenPerSec = (int)(player.Entropy().lifeRegenPerSec * 0.4f);
            player.GetDamage(DamageClass.Generic) += 0.2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CoreofHavoc>(), 6)
                .AddIngredient(ModContent.ItemType<PerennialBar>(), 2)
                .Register();
        }
    }
}

﻿using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class DeusCore : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<GlowPurple>();
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().deusCore = true;
            player.Entropy().damageReduce -= 1.0f;
        }

        public override void AddRecipes()
        {
        }
    }
}

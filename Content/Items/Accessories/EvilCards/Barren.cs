﻿using CalamityEntropy.Common;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
    public class Barren : ModItem
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
            player.GetModPlayer<EModPlayer>().BarrenCard = true;
            player.GetDamage(CEUtils.RogueDC) -= 0.30f;
            player.GetCritChance(CEUtils.RogueDC) -= 30;
        }

        public override void AddRecipes()
        {
        }
    }
}

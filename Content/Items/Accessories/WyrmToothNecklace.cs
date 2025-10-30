﻿using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class WyrmToothNecklace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<GenericDamageClass>() += 0.25f;
            player.GetArmorPenetration<GenericDamageClass>() += 100;
            player.GetCritChance(DamageClass.Generic) += 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ReaperToothNecklace>().
                AddIngredient<WyrmTooth>(9).
                AddIngredient<FadingRunestone>().
                AddTile(ModContent.TileType<AbyssalAltarTile>()).
                Register();
        }
    }
}

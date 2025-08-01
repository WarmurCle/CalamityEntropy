﻿using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Legs)]
    public class VoidFaquirCuises : ModItem, ILocalizedModType
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 39;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().VFLeg = true;
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 12)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 6)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}

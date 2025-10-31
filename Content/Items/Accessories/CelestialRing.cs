﻿using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class CelestialRing : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.defense = 20;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetKnockback(DamageClass.Summon) += 0.75f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.pickSpeed *= 1.2f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            player.Entropy().CRing = true;
            player.lifeRegen += 5;
            player.maxMinions += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CelestialShell).
                AddIngredient(ModContent.ItemType<DarkSunRing>()).
                AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4).
                AddIngredient(ModContent.ItemType<AuricBar>(), 5)
                .AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}

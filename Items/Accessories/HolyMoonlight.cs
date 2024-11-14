using CalamityEntropy.Buffs;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	public class HolyMoonlight : ModItem
	{

		public override void SetDefaults() {
			Item.width = 86;
			Item.height = 86;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType <VoidPurple>();
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 100;
            player.GetDamage(DamageClass.Magic) *= 1.1f;
            player.Entropy().holyMoonlight = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<RoverDrive>(), 1).
                AddIngredient(ModContent.ItemType<ManaPolarizer>(), 1).
                AddIngredient(ModContent.ItemType<CryoStone>(), 1).
                AddIngredient(ModContent.ItemType<VoidBar>(), 6).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}

using CalamityEntropy.Buffs;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	public class CelestialRing : ModItem
	{

		public override void SetDefaults() {
			Item.width = 26;
            Item.defense = 30;
			Item.height = 26;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.18f;
            player.GetAttackSpeed(DamageClass.Generic) += 0.18f;
            player.GetKnockback(DamageClass.Summon) += 0.75f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.pickSpeed *= 1.2f;
            player.Entropy().CRing = true;
            player.Entropy().lifeRegenPerSec += 5;
            player.maxMinions += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CelestialShell).
                AddIngredient(ModContent.ItemType<DarkSunRing>()).
                AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 6).
                AddIngredient(ModContent.ItemType<AuricBar>(), 6).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
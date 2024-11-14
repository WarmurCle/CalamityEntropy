using CalamityEntropy.Buffs;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories.Cards
{
	public class OracleDeck : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().oracleDeskInInv = true;
            player.GetCritChance(DamageClass.Generic) += 13;
            player.GetAttackSpeed(DamageClass.Generic) *= 1.15f;
            player.Calamity().rogueStealthMax += 0.2f;
            player.maxMinions += 3;
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetModPlayer<EModPlayer>().oracleDeck = true;
            player.Calamity().defenseDamageRatio *= 0.5;
        }

        public override void UpdateInventory(Player player)
        {
            player.Entropy().oracleDeskInInv = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AuraCard>(), 1)
                .AddIngredient(ModContent.ItemType<BrillianceCard>(), 1)
                .AddIngredient(ModContent.ItemType<EntityCard>(), 1)
                .AddIngredient(ModContent.ItemType<InspirationCard>(), 1)
                .AddIngredient(ModContent.ItemType<MetropolisCard>(), 1)
                .AddIngredient(ModContent.ItemType<WisdomCard>(), 1)
                .AddIngredient(ModContent.ItemType<RadianceCard>(), 1)
                .AddIngredient(ModContent.ItemType<TemperanceCard>(), 1)
                .AddIngredient(ModContent.ItemType<EnduranceCard>(), 1)
                .AddIngredient(ModContent.ItemType<ThreadOfFate>(), 1)
                .AddTile(TileID.Bookcases).Register();
        }
    }
}

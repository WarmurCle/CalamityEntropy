using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Accessories.EvilCards;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Cards
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
            player.Entropy().oracleDeckInInv = true;
            player.GetCritChance(DamageClass.Generic) += 11;
            player.Calamity().rogueStealthMax += 0.16f;
            player.maxMinions += 2;
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetModPlayer<EModPlayer>().oracleDeck = true;
            player.Calamity().defenseDamageRatio *= 0.5;
        }

        public override void UpdateInventory(Player player)
        {
            player.Entropy().oracleDeckInInv = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.ModItem is not TaintedDeck;
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

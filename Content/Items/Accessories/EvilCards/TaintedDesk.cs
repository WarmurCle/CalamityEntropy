using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Accessories.Cards;
using CalamityEntropy.Util;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
	public class TaintedDesk : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().EvilDesk = true;

            player.GetModPlayer<EModPlayer>().BarrenCard = true;

            player.GetModPlayer<EModPlayer>().ConfuseCard = true;

            player.GetModPlayer<EModPlayer>().FoolCard = true;
            player.Entropy().ManaCost += 0.16f;

            player.Entropy().FrailCard = true;
            player.Entropy().damageReduce -= 0.08f;

            player.GetModPlayer<EModPlayer>().GreedCard = true;

            player.GetModPlayer<EModPlayer>().NothingCard = true;
            player.Entropy().AttackVoidTouch += 0.8f;

            player.GetModPlayer<EModPlayer>().PerplexedCard = true;
            player.GetCritChance(DamageClass.Generic) -= 4;
            player.GetAttackSpeed(DamageClass.Generic) += 0.18f;

            player.GetModPlayer<EModPlayer>().SacrificeCard = true;
            player.lifeRegen = (int)(player.lifeRegen * 0.5f);
            player.Entropy().lifeRegenPerSec = (int)(player.Entropy().lifeRegenPerSec * 0.5f);

            player.GetDamage(DamageClass.Generic) += 0.6f;

            player.GetModPlayer<EModPlayer>().TarnishCard = true;

            player.Entropy().taintedDeskInInv = true;

        }
        public override void UpdateInventory(Player player)
        {
            player.Entropy().taintedDeskInInv = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ThreadOfAbyss>())
                .AddIngredient(ModContent.ItemType<GreedCard>())
                .AddIngredient(ModContent.ItemType<Frail>())
                .AddIngredient(ModContent.ItemType<Barren>())
                .AddIngredient(ModContent.ItemType<Tarnish>())
                .AddIngredient(ModContent.ItemType<Confuse>())
                .AddIngredient(ModContent.ItemType<Perplexed>())
                .AddIngredient(ModContent.ItemType<Sacrifice>())
                .AddIngredient(ModContent.ItemType<Nothing>())
                .AddIngredient(ModContent.ItemType<Fool>())
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}

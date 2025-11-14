using CalamityEntropy.Common;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.EvilCards
{
    public class TaintedDeck : ModItem, IDeck
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;

        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !(equippedItem.ModItem is IDeck && incomingItem.ModItem is IDeck);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().EvilDeck = true;

            player.GetModPlayer<EModPlayer>().BarrenCard = true;

            player.GetModPlayer<EModPlayer>().ConfuseCard = true;

            player.GetModPlayer<EModPlayer>().FoolCard = true;
            player.Entropy().ManaCost += 0.16f;

            player.Entropy().FrailCard = true;
            player.Entropy().damageReduce -= 0.3f;

            player.GetModPlayer<EModPlayer>().GreedCard = true;

            player.GetModPlayer<EModPlayer>().NothingCard = true;
            player.Entropy().AttackVoidTouch += 0.06f;

            player.GetModPlayer<EModPlayer>().PerplexedCard = true;
            player.GetCritChance(DamageClass.Generic) -= 12;

            player.GetModPlayer<EModPlayer>().SacrificeCard = true;

            player.GetDamage(DamageClass.Generic) += 0.35f;

            player.GetModPlayer<EModPlayer>().TarnishCard = true;

            player.Entropy().taintedDeckInInv = true;
            ModContent.GetInstance<Perplexed>().UpdateAccessory(player, hideVisual);
            player.Entropy().addEquip("TDeck", !hideVisual);
        }
        public override void UpdateInventory(Player player)
        {
            player.Entropy().taintedDeckInInv = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GreedCard>()
                .AddIngredient<Frail>()
                .AddIngredient<Barren>()
                .AddIngredient<Tarnish>()
                .AddIngredient<Confuse>()
                .AddIngredient<Perplexed>()
                .AddIngredient<Sacrifice>()
                .AddIngredient<Nothing>()
                .AddIngredient<Fool>()
                .AddIngredient<ThreadOfAbyss>()
                .AddIngredient<CoreofCalamity>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}

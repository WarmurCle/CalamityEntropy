using CalamityMod.Items;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.SoulCards
{
    public class SoulDeck : ModItem, IDeck
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        public float WingSpeedAddition => IndigoCard.WingSpeedAddition;
        public float WingTimeAddition => IndigoCard.WingTimeAddition;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().soulDeckInInv = true;
            player.Entropy().bitternessCard = true;
            player.Entropy().grudgeCard = true;
            player.Entropy().devouringCard = true;
            player.Entropy().WingSpeed += WingSpeedAddition;
            player.Entropy().WingTimeMult += WingTimeAddition;
            player.Entropy().mourningCard = true;
            player.Entropy().obscureCard = true;
            player.Entropy().DebuffTime -= PurificationCard.DebuffTimeReduce;
            player.Entropy().CooldownTimeMult -= RequiemCard.CooldownDec;
            player.Entropy().HitCooldown += WisperCard.ImmuneAdd;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[CD]", RequiemCard.CooldownDec.ToPercent());
            tooltips.Replace("[IM]", WisperCard.ImmuneAdd.ToPercent());
            tooltips.Replace("[DB]", PurificationCard.DebuffTimeReduce.ToPercent());
            tooltips.Replace("[FL]", WingSpeedAddition.ToPercent());
            tooltips.Replace("[FT]", WingTimeAddition.ToPercent());
            tooltips.Replace("[BE]", BitternessCard.enduMax.ToPercent());
            tooltips.Replace("[BD]", BitternessCard.DmgMax.ToPercent());
            tooltips.Replace("[AP]", DevouringCard.ArmorPene.ToPercent());
        }
        public override void UpdateInventory(Player player)
        {
            player.Entropy().soulDeckInInv = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BitternessCard>(), 1)
                .AddIngredient(ModContent.ItemType<DevouringCard>(), 1)
                .AddIngredient(ModContent.ItemType<GrudgeCard>(), 1)
                .AddIngredient(ModContent.ItemType<IndigoCard>(), 1)
                .AddIngredient(ModContent.ItemType<MourningCard>(), 1)
                .AddIngredient(ModContent.ItemType<ObscureCard>(), 1)
                .AddIngredient(ModContent.ItemType<PurificationCard>(), 1)
                .AddIngredient(ModContent.ItemType<RequiemCard>(), 1)
                .AddIngredient(ModContent.ItemType<WisperCard>(), 1)
                .AddIngredient(ModContent.ItemType<CursedThread>(), 1)
                .AddIngredient<CoreofCalamity>()
                .AddTile(TileID.Bookcases).Register();
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !(equippedItem.ModItem is IDeck && incomingItem.ModItem is IDeck);
        }
    }
}

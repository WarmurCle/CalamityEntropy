using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Accessories.EvilCards;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Cards
{
    public class OracleDeck : ModItem, IDeck
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;

        }
        public static int CRIT = 10;
        public static float STEALTH = 0.05f;
        public static int MINIONADD = 1;
        public static int ArmorPenet = 5;
        public static float MELEEAS = 0.05f;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().oracleDeckInInv = true;
            player.GetCritChance(DamageClass.Generic) += CRIT;
            player.Calamity().rogueStealthMax += STEALTH;
            player.maxMinions += MINIONADD;
            player.GetArmorPenetration(DamageClass.Generic) += ArmorPenet;
            player.GetAttackSpeed(DamageClass.Melee) += MELEEAS;
            player.GetModPlayer<EModPlayer>().oracleDeck = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[CR]", CRIT);
            tooltips.Replace("[ST]", STEALTH.ToPercent());
            tooltips.Replace("[MN]", MINIONADD);
            tooltips.Replace("[AP]", ArmorPenet);
            tooltips.Replace("[ATS]", MELEEAS.ToPercent());
        }
        public override void UpdateInventory(Player player)
        {
            player.Entropy().oracleDeckInInv = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.ModItem == null || incomingItem.ModItem is not IDeck;
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
                .AddIngredient<CoreofCalamity>()
                .AddTile(TileID.Bookcases).Register();
        }
    }
}

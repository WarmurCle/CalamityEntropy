using CalamityEntropy.Common;
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
            Item.defense = 8;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;

        }
        public static int CRIT = 10;
        public static float STEALTH = 0.05f;
        public static int MINIONADD = 1;
        public static int ArmorPenet = 10;
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
            return !(equippedItem.ModItem is IDeck && incomingItem.ModItem is IDeck);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AuraCard>()
                .AddIngredient<BrillianceCard>()
                .AddIngredient<EntityCard>()
                .AddIngredient<InspirationCard>()
                .AddIngredient<MetropolisCard>()
                .AddIngredient<WisdomCard>()
                .AddIngredient<RadianceCard>()
                .AddIngredient<TemperanceCard>()
                .AddIngredient<EnduranceCard>()
                .AddIngredient<ThreadOfFate>()
                .AddIngredient<CoreofCalamity>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}

using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class AzafureDetectionEquipment : ModItem, IAzafureEnhancable
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            Item.defense = 4;
        }
        public static string ID = "AzafureDetectorEquipment";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.rocketBoots += 90;
            player.noFallDmg = true;
            player.jumpSpeedBoost += player.AzafureEnhance() ? 1.6f : 0.8f;
            player.maxRunSpeed *= 1.12f;
            player.Entropy().addEquip(ID, !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RustyDetectionEquipment>().
                AddIngredient<HellIndustrialComponents>(4).
                AddIngredient<AerialiteBar>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

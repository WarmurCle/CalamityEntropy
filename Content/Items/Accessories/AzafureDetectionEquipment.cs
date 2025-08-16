using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class AzafureDetectionEquipment : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.defense = 6;
            Item.height = 46;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        public static string ID = "AzafureDetectorEquipment";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.rocketBoots += 90;
            player.noFallDmg = true;
            player.jumpSpeedBoost += 1.8f;
            player.maxRunSpeed *= 1.25f;
            player.Entropy().addEquip(ID, !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<RustyDetectionEquipment>()).
                AddIngredient(ModContent.ItemType<HellIndustrialComponents>(), 4).
                AddIngredient(ModContent.ItemType<AerialiteBar>(), 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

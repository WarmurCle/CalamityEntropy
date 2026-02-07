using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class RustyDetectionEquipment : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 46;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public static string ID = "RustyDetectorEquipment";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Item.Calamity().Charge > 0)
            {
                if (player.controlJump)
                {
                    Item.Calamity().Charge -= 64f / (180 * 60f);
                }
                player.rocketBoots += 30;
                player.noFallDmg = true;
                player.jumpSpeedBoost += 0.5f;
                player.maxRunSpeed *= 1.10f;
                player.Entropy().addEquip(ID, !hideVisual);
                if (Item.Calamity().Charge < 0)
                    Item.Calamity().Charge = 0;
            }
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 20).
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15).
                AddIngredient(ModContent.ItemType<SuspiciousScrap>(), 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

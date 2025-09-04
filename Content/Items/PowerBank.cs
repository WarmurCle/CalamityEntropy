using CalamityMod;
using CalamityMod.Items.Placeables.DraedonStructures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class PowerBank : ModItem
    {
        public static float CHARGE_PER_TICK = 0.02f;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 14));
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<PowerCellFactoryItem>(), 1).
                AddIngredient(ModContent.ItemType<ChargingStationItem>(), 1).
                AddTile(TileID.Anvils).
                Register();
        }

        public override void UpdateInventory(Player player)
        {
            foreach (Item item in player.inventory.Combine<Item>(player.miscEquips).Combine<Item>(player.armor))
            {
                if (!item.IsAir)
                {
                    if (item.Calamity().UsesCharge)
                    {
                        if (item.Calamity().Charge < item.Calamity().MaxCharge)
                        {
                            item.Calamity().Charge += CHARGE_PER_TICK;
                            if (item.Calamity().Charge > item.Calamity().MaxCharge)
                            {
                                item.Calamity().Charge = item.Calamity().MaxCharge;
                            }
                        }
                    }
                }
            }
        }
    }
}
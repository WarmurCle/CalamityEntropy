using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ShadeCloak : ModItem
    {
        public static float BaseDamage = 25;
        public static int CooldownTicks = 45 * 60;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            Item.expert = true;
        }
        public static string ID = "ShadeCloak";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip(ID, !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            /*CreateRecipe().AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient<SolarVeil>(12)
                .AddIngredient<DepthCells>(4)
                .AddIngredient(ItemID.Ectoplasm, 8);*/
        }
    }
    public class SCDashMP : ModPlayer
    {
        public bool flag = true;
        public int Cooldown = 0;
        public override void PostUpdate()
        {

        }
    }
}

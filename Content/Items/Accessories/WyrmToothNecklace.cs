using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class WyrmToothNecklace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<GenericDamageClass>() += 0.25f;
            player.GetArmorPenetration<GenericDamageClass>() += 45;
	        player.GetCritChance(DamageClass.Generic) += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ReaperToothNecklace>().
                AddIngredient<WyrmTooth>(9).
                 AddTile(ModContent.TileType<AbyssalAltarTile>()).
                Register();
        }
    }
}

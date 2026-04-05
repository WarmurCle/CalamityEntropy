using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Smoldering
{
    [AutoloadEquip(EquipType.Body)]
    public class SmolderingBreastplate : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.defense = 10;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 4;
            player.endurance += 0.08f;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Burning] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TectonicShard>(10)
                .AddIngredient(ItemID.MoltenBreastplate)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}

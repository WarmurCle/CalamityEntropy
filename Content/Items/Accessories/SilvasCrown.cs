using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class SilvasCrown : ModItem
    {
        public static float DDR = 0.25f;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.defense = 5;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ModContent.RarityType<GlowGreen>();
            Item.accessory = true;


        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().defenseDamageRatio = DDR;

            player.Entropy().SCrown = true;
        }

    }
}

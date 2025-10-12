using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class SilvasCrown : ModItem
    {

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
            player.Calamity().defenseDamageRatio = 0.01f;
            player.Calamity().nextHitDealsDefenseDamage = false;

            player.Entropy().SCrown = true;
        }

    }
}

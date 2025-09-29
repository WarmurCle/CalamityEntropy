using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class TargetingWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 1;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item28;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shootSpeed = 20f;
        }


    }
}

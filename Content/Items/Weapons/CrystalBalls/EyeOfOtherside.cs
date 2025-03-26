using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.CrystalBalls
{
    public class EyeOfOtherside : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 320;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.UseSound = Util.Util.GetSound("soulshine");
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.shoot = ModContent.ProjectileType<EyeOfOthersideHoldout>();
            Item.shootSpeed = 16f;
            Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Necroplasm>(), 5)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 1)
                .AddIngredient(ItemID.CrystalBall, 1)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}

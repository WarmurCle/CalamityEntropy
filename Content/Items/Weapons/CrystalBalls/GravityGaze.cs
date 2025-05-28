using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.CrystalBalls
{
    public class GravityGaze : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 37;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.UseSound = CEUtils.GetSound("soulshine");
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<GravityGazeHoldout>();
            Item.shootSpeed = 16f;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AerialiteBar>(), 5)
                .AddIngredient(ItemID.Glass, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}

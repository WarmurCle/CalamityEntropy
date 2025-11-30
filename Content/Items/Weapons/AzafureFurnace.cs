using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureFurnace : ModItem, IAzafureEnhancable
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 3;
            Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AzafureFurnaceHoldout>();
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shootSpeed = 25f;
            Item.channel = true;
            Item.noUseGraphic = true;
            var modItem = Item.Calamity();
            modItem.UsesCharge = true;
            Item.mana = 15;
            modItem.MaxCharge = 200f;
            modItem.ChargePerUse = 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OverloadFurnace>().
                AddIngredient<HellIndustrialComponents>(4).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool MagicPrefix()
        {
            return true;
        }
    }
}

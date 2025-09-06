using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureFurnace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 26;
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
            Item.shootSpeed = 22f;
            Item.channel = true;
            Item.noUseGraphic = true;
            var modItem = Item.Calamity();
            modItem.UsesCharge = true;
            Item.mana = 10;
            modItem.MaxCharge = 200f;
            modItem.ChargePerUse = 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<OverloadFurnace>()).
                AddIngredient(ModContent.ItemType<HellIndustrialComponents>(), 4).
                AddIngredient(ItemID.SoulofLight, 2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool MagicPrefix()
        {
            return true;
        }
    }
}

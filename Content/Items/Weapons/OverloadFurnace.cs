using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class OverloadFurnace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 3;
            Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<OverloadFurnaceHoldout>();
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shootSpeed = 22f;
            Item.channel = true;
            Item.noUseGraphic = true;
            var modItem = Item.Calamity();
            modItem.UsesCharge = true;
            Item.mana = 6;
            modItem.MaxCharge = 100f;
            modItem.ChargePerUse = 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 5).
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 2).
                AddIngredient(ItemID.Ruby, 1).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool MagicPrefix()
        {
            return true;
        }
    }
}

using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class HadopelagicEchoII : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 126;
            Item.height = 66;
            Item.damage = 16000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.shoot = ModContent.ProjectileType<HadopelagicEchoIIProj>();
            Item.shootSpeed = 16f;
            Item.mana = 40;
            Item.DamageType = DamageClass.Magic;
            Item.ArmorPenetration = 100;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override bool MagicPrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<EidolicWail>())
                .AddIngredient(ModContent.ItemType<WyrmTooth>(), 12)
                .AddTile(ModContent.TileType<AbyssalAltarTile>())
                .Register();
        }
    }
}

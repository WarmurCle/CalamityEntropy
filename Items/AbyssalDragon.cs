using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using CalamityEntropy.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Util;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using Terraria.GameContent;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Security.Policy;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityEntropy.Items
{
    public class AbyssalDragon :ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 750;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.ArmorPenetration = 46;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item28;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<AbyssDragonProj>();
            Item.shootSpeed = 20f;
            Item.mana = 65;
            Item.DamageType = DamageClass.Magic;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, ModContent.ProjectileType<AbyssDragonProj>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity.RotatedByRandom(MathHelper.ToRadians(6)), ModContent.ProjectileType<AbyssalStar>(), (int)(damage * 0.3f), knockback, player.whoAmI);

            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 8)
                .AddIngredient(ModContent.ItemType<ReaperTooth>(), 4)
                .AddIngredient(ModContent.ItemType<DeathhailStaff>())
                .AddIngredient(ModContent.ItemType<ClamorNoctus>())
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
    }
}

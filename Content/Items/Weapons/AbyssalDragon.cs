﻿using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AbyssalDragon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 630;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.ArmorPenetration = 10;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item28;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<AbyssDragonProj>();
            Item.shootSpeed = 20f;
            Item.mana = 34;
            Item.DamageType = DamageClass.Magic;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, ModContent.ProjectileType<AbyssDragonProj>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < 6 + player.Entropy().WeaponBoost * 3; i++)
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

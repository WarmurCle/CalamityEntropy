using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.OblivionThresher
{
    public class OblivionThresher : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 46;
            Item.damage = 1200;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 1.75f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<OblivionThresherHoldout>();
            Item.shootSpeed = 24;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SuperradiantSlaughterer>()
                .AddIngredient<VoidBar>(5)
                .AddIngredient<RuinousSoul>(4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                player.AddCooldown(OblivionThretherCooldown.ID, 320);
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return !player.HasCooldown(OblivionThretherCooldown.ID);
        }

        public override bool CanShoot(Player player)
        {
            Item.shoot = player.altFunctionUse == 2 ? ModContent.ProjectileType<OblivionCruiserDash>() : ModContent.ProjectileType<OblivionThresherHoldout>();
            return base.CanShoot(player);
        }
    }
}
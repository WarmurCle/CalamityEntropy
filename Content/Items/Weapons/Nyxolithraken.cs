using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Nyxolithraken : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 5;
        }

        public override void SetDefaults()
        {
            Item.damage = 3000;
            Item.crit = 0;
            Item.DamageType = DamageClass.Summon;
            Item.width = 90;
            Item.height = 88;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 2;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<NyxolithrakenDragon>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.mana = 10;
            Item.buffType = ModContent.BuffType<NyxolithrakenBuff>();
            Item.rare = ModContent.RarityType<AbyssalBlue>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 3);
            int projectile = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, Item.damage, knockback, player.whoAmI, 0, 1, 0);
            Main.projectile[projectile].originalDamage = Item.damage;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EndoHydraStaff>()
                .AddIngredient<YharonsKindleStaff>()
                .AddIngredient<WyrmTooth>(10)
                .AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5)
                .AddTile<AbyssalAltarTile>().Register();
        }
    }
}

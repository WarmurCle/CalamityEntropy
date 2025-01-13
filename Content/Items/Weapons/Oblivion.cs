using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Oblivion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 29;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 2;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<OblivionArrow>();
            Item.UseSound = new Terraria.Audio.SoundStyle("CalamityEntropy/Assets/Sounds/feathershot") { MaxInstances = 10, Volume = 0.4f };
            Item.autoReuse = false;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.ArmorPenetration = 120;
            Item.noUseGraphic = true;
            Item.crit = 8;
        }
        public bool cs = false;
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool(12);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<OblivionArrow>();
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1), type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Onyxplate>(), 8)
                .AddIngredient(ModContent.ItemType<Voidstone>(), 4)
                .AddIngredient(ModContent.ItemType<DivineGeode>(), 6)
                .Register();
        }
    }
}

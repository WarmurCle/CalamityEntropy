using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class ShadewindLance : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 3000;
            Item.ArmorPenetration = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<ShadewindLanceThrow>();
            Item.shootSpeed = 46f;
            Item.DamageType = CEUtils.RogueDC;
        }



        public override float StealthDamageMultiplier => 1.2f;
        public override float StealthVelocityMultiplier => 1.2f;
        public override float StealthKnockbackMultiplier => 3f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                p.ToProj().extraUpdates += 1;
                p.ToProj().netUpdate = true;
                p.ToProj().penetrate = -1;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 8)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}

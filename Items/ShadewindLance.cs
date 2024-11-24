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

namespace CalamityEntropy.Items
{
    public class ShadewindLance : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 3400;
            Item.ArmorPenetration = 56;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 86;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<ShadewindLanceThrow>();
            Item.shootSpeed = 46f;
            Item.DamageType = CUtil.rougeDC;
        }

       
         
        public override float StealthDamageMultiplier => 1.2f;
        public override float StealthVelocityMultiplier => 1.2f;
        public override float StealthKnockbackMultiplier => 3f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles)) {
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
                .AddIngredient(ModContent.ItemType<PhantasmalRuin>(), 1)
                .AddIngredient(ModContent.ItemType<DarksunFragment>(), 10)
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
    }
}

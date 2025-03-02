using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AmethystFrisbee : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 58;
            Item.damage = 22;
            Item.ArmorPenetration = 8;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 34;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 86;
            Item.knockBack = 1f;
            Item.UseSound = null;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<AmethystFrisbeeProjectile>();
            Item.shootSpeed = 36f;
            Item.DamageType = CUtil.rogueDC;
        }
        public int altShotCount = 0;
       
         
        public override float StealthDamageMultiplier => 1.2f;
        public override float StealthVelocityMultiplier => 1.5f;
        public override float StealthKnockbackMultiplier => 3f;
        public override void UpdateInventory(Player player)
        {
            if(altShotCount > 0)
            {
                altShotCount--;
            }
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(altShotCount > 0)
            {
                velocity *= 0.34f;
            }
            Util.Util.PlaySound("throw", 1, player.Center);
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, altShotCount > 0 ? 1 : 0);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                }
                return false;
            }
            else
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, altShotCount > 0 ? 1 : 0);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Amethyst, 8)
                .AddIngredient(ItemID.Diamond, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
